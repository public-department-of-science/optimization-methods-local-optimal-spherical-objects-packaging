using Cureos.Numerics;
using System;
using System.Linq;

namespace hs071_cs
{
    class FixedRadius3dAdaptor : BaseAdaptor, IDisposable
    {
        public readonly int countCircles; // количество шаров
        public double[] X { get; private set; } //массив х (входят все координаты и радиусы)
        public readonly double[] radius;
        private double SummedWeight;
        private double K1 = 5.0 / 10.0;
        private double K2 = 5.0 / 10.0;
        public double Fine = 1.5; // траф функции

        private double[] Weight { get; set; }
        private double[,] C { get; set; }

        public FixedRadius3dAdaptor(Data data)
        {
            countCircles = data.circleCount; // задаём количетво кругов
            _n = countCircles * 3 + 1; // количество переменных в векторе
            radius = new double[countCircles];
            //вспомогательные счетчики
            double sumR = 0;
            int it = 0;

            foreach (var item in data.ball)
            {
                radius[it++] = item.R;
                sumR += item.R;
            }
            radius = radius.OrderBy(a => a).ToArray();
            X = new double[_n];
            try
            {
                if (data.ball != null && data != null && data.R >= 0)
                {
                    for (int i = 0; i < countCircles; i++)
                    {
                        X[3 * i] = data.ball[i].X;
                        X[3 * i + 1] = data.ball[i].Y;
                        X[3 * i + 2] = data.ball[i].Z;
                    }
                    X[_n - 1] = data.R;
                }
            }
            catch (Exception ex)
            {
                new PrintErrorMessageDel(OutPut.ErrorMessage)(ex.Message);
            }

            /*    Ограничения переменных
            * *************************************************************************************/
            _x_L = new double[_n];
            double max = radius[countCircles - 1];

            // обнуляем необходимые счетчики
            int countXYZR = 0,
                countCoordinate = 0;

            while (countCoordinate < countCircles)
            {
                _x_L[countXYZR] = -DiametrSum(radius) + radius[countCoordinate];
                _x_L[++countXYZR] = -DiametrSum(radius) + radius[countCoordinate];
                _x_L[++countXYZR] = -DiametrSum(radius) + radius[countCoordinate];
                if (max < radius[countCoordinate])
                    max = radius[countCoordinate];

                countCoordinate++;
                countXYZR++;
            }
            _x_L[_n - 1] = max;

            // обнуляем необходимые счетчики
            countXYZR = 0;
            countCoordinate = 0;
            _x_U = new double[_n];

            while (countCoordinate < countCircles)
            {
                _x_U[countXYZR] = DiametrSum(radius) - radius[countCoordinate];
                _x_U[++countXYZR] = DiametrSum(radius) - radius[countCoordinate];
                _x_U[++countXYZR] = DiametrSum(radius) - radius[countCoordinate];

                countCoordinate++;
                countXYZR++;
            }
            _x_U[_n - 1] = sumR; //Ipopt.PositiveInfinity;

            /*    Огрaничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0;

            // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            _nele_jac += 4 * countCircles; // x, y, z , R
            _m += countCircles;

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            _nele_jac += 2 * 3 * countCircles * (countCircles - 1) / 2;  // 2*3 два ограничения по 3 ненулевых частных производных
            _m += countCircles * (countCircles - 1) / 2;

            _g_L = new double[_m];
            _g_U = new double[_m];
            int op = 0;
            for (int j = 0; j < countCircles; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }
            for (int i = 0; i < countCircles - 1; i++)
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    _g_L[op] = Math.Pow((radius[i] + radius[j]), 2);
                    _g_U[op++] = Ipopt.PositiveInfinity;
                }
            }

            //Hesian
            Weight = new double[data.circleCount];
            for (int i = 0; i < data.circleCount; i++)
            {
                Weight[i] = data.ball[i].Weight;
            }
            SummedWeight = SummWeight(Weight);
            C = data.C ?? null;
            _nele_hess = 0;

        } // End_Конструктор 

        public override bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            obj_value = x[_n - 1]
                - C_Multiply_by_Length(x)
                - Fine * (Math.Pow(SummedWeight * SummedWeight * SummX(x), 2.0)
                + Math.Pow(SummedWeight * SummedWeight * SummY(x), 2.0)
                + Math.Pow(SummedWeight * SummedWeight * SummZ(x), 2.0));
            return true;
        }
        public override bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            double[] Xderivative;
            double[] Yderivative;
            double[] Zderivative;
            ArrayOfDerivatives(x, out Xderivative, out Yderivative, out Zderivative);
            for (int i = 0; i < countCircles; i++)
            {
                grad_f[3 * i] = -Xderivative[i];
                grad_f[3 * i + 1] = -Yderivative[i];
                grad_f[3 * i + 2] = -Zderivative[i];
            }
            grad_f[_n - 1] = 1;
            return true;
        }
        public override bool eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int kk = 0;
            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            // from 0 to count-1
            for (int i = 0; i < countCircles; i++)
            {
                g[kk++] = Math.Pow((x[_n - 1] - radius[i]), 2) -
                    x[3 * i] * x[3 * i] -     // x
                    x[3 * i + 1] * x[3 * i + 1] -  // y
                    x[3 * i + 2] * x[3 * i + 2];      // z
            }
            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            // from count to count*(count-1)/2 - 1

            for (int i = 0; i < countCircles - 1; i++) // на каждой итерации увеличиваем на 3 счетч. по Z
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    g[kk++] = Math.Pow((x[3 * i] - x[3 * j]), 2.0)
                                  + Math.Pow((x[3 * i + 1] - x[3 * j + 1]), 2.0)
                                  + Math.Pow((x[3 * i + 2] - x[3 * j + 2]), 2.0)
                                  - Math.Pow((radius[i] - radius[j]), 2.0);
                }
            }
            return true;
        }
        public override bool eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int kk = 0,
                    g = 0;

                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                // позиции R, Х и У, Z
                for (g = 0; g < countCircles; ++g)
                {
                    //R0 -> внешний шар 
                    iRow[kk] = g;
                    jCol[kk++] = _n - 1;

                    //X
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g;

                    //Y
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g + 1;

                    //Z
                    iRow[kk] = g;
                    jCol[kk++] = 3 * g + 2;
                }

                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
                for (int i = 0; i < countCircles - 1; ++i)
                {
                    for (int j = i + 1; j < countCircles; ++j)
                    {
                        // -------  X[i], X[j] ------- 
                        iRow[kk] = g;
                        jCol[kk++] = 3 * i;
                        iRow[kk] = g;
                        jCol[kk++] = 3 * j;

                        // -------  Y[i], Y[j] ------- 
                        iRow[kk] = g; ;
                        jCol[kk++] = 3 * i + 1;
                        iRow[kk] = g;
                        jCol[kk++] = 3 * j + 1;

                        // -------  Z[i], Z[j] ------- 
                        iRow[kk] = g;
                        jCol[kk++] = 3 * i + 2;
                        iRow[kk] = g;
                        jCol[kk++] = 3 * j + 2;

                        ++g;
                    }
                }
            }
            else
            {

                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                int kk = 0;
                for (int i = 0; i < countCircles; i++)// шаг по Z это каждый третий эл
                {
                    values[kk] = 2.0 * (x[_n - 1] - radius[i]); // R0'
                    kk++;
                    values[kk] = -2.0 * x[3 * i]; //X'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 1]; //Y'
                    kk++;
                    values[kk] = -2.0 * x[3 * i + 2]; //Z'
                    kk++;
                }
                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
                //  Console.WriteLine("---------------------------------------");

                for (int i = 0; i < countCircles - 1; i++)
                {
                    for (int j = i + 1; j < countCircles; j++)
                    {
                        values[kk++] = 2.0 * (x[3 * i] - x[3 * j]); //X[i]'
                        values[kk++] = -2.0 * (x[3 * i] - x[3 * j]); //X[j]'

                        values[kk++] = 2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[i]'
                        values[kk++] = -2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[j]'

                        values[kk++] = 2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[i]'
                        values[kk++] = -2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[j]'
                    }
                }
            }
            return true;
        }
        public override bool eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            /* if (values == null)
             {
                 int idx = 0;
                 for (int row = 0; row < 4; row++)
                 {
                     for (int col = 0; col <= row; col++)
                     {
                         iRow[idx] = row;
                         jCol[idx] = col;
                         idx++;
                     }
                 }
             }
             else
             {
                 values[0] = obj_factor * 2 * Fine * SummedWeight; // 0,0!

                 values[1] = 0;   // 1,0
                 values[2] = obj_factor * 2 * Fine * SummedWeight;                     // 1,1!

                 values[3] = 0;   // 2,0
                 values[4] = 0;                     // 2,1
                 values[5] = obj_factor * 2 * Fine * SummedWeight;                     // 2,2!

                 values[6] = 0; // 3,0
                 values[7] = 0;                 // 3,1
                 values[8] = 0;                 // 3,2
                 values[9] = obj_factor * 2;                                   // 3,3!


                 // add the portion for the first constraint
                 values[0] += lambda[0] * -2; // 1,0
                 values[2] += lambda[0] * -2; // 1,0
                 values[5] += lambda[0] * -2; // 1,0
                 values[9] += lambda[0] * 2; // 1,0


                 // add the portion for the second constraint
                 values[0] += lambda[1] * 2; // 0,0

                 values[2] += lambda[1] * 2; // 1,1

                 values[5] += lambda[1] * 2; // 2,2

                 values[9] += lambda[1] * -2; // 3,3
             }*/

            return false;
        }
        public override string ToString()
        {
            return "AdaptorFixedRadius";
        }
        // Вычисляем диаметр как сумму всех радиусов
        private double DiametrSum(double[] radius)
        {
            var sum = 0.0;
            foreach (var rad in radius)
            {
                sum += 2 * rad;
            }
            return sum;
        }
        private double SummX(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < countCircles; i++)
            {
                sum += x[3 * i];
            }
            return sum;
        }
        private double SummY(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < countCircles; i++)
            {
                sum += x[3 * i + 1];
            }
            return sum;
        }
        private double SummZ(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < countCircles; i++)
            {
                sum += x[3 * i + 2];
            }
            return sum;
        }

        // // ///////////////

        private double C_Multiply_by_Length(double[] x)
        {
            double Sum = 0;
            for (int i = 0; i < countCircles - 1; i++)
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    Sum += C[i, j] * Math.Sqrt(Math.Pow(x[3 * i] - x[3 * j], 2)
                        + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2)
                        + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2));
                }
            }
            return Sum;
        }
        private void ArrayOfDerivatives(double[] x, out double[] Xderivative, out double[] Yderivative, out double[] Zderivative)
        {
            Xderivative = new double[countCircles];
            Yderivative = new double[countCircles];
            Zderivative = new double[countCircles];
            for (int i = 0; i < countCircles - 1; i++)
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    Xderivative[i] += C[i, j] * (x[3 * i] - x[3 * j]) /
                        (
                        Math.Sqrt(
                            Math.Pow(x[3 * i], 2) - 2 * x[3 * i] * x[3 * j] + Math.Pow(x[3 * j], 2) // x
                            + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2) // y
                            + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2) // z
                                 )
                        );

                    Yderivative[i] += C[i, j] * (x[3 * i + 1] - x[3 * j + 1]) /
                        (
                        Math.Sqrt(
                            Math.Pow(x[3 * i] - x[3 * j], 2) // x
                            + Math.Pow(x[3 * i + 1], 2) - 2 * x[3 * i + 1] * x[3 * j + 1] + Math.Pow(x[3 * j + 1], 2) // y
                            + Math.Pow(x[3 * i + 2] - x[3 * j + 2], 2) // z
                                 )
                        );

                    Zderivative[i] += C[i, j] * (x[3 * i + 2] - x[3 * j + 2]) /
                        (
                        Math.Sqrt(
                            +Math.Pow(x[3 * i] - x[3 * j], 2) // x
                            + Math.Pow(x[3 * i + 1] - x[3 * j + 1], 2) // y
                            + Math.Pow(x[3 * i + 2], 2) - 2 * x[3 * i + 2] * x[3 * j + 2] + Math.Pow(x[3 * j + 2], 2) // z
                                 )
                        );
                }
            }
        }
        private double SummWeight(double[] weight)
        {
            double Summ = 0;
            for (int i = 0; i < weight.Length; i++)
            {
                Summ += weight[i];
            }
            return Summ;
        }
        public void Dispose()
        {
            _m = 0;
        }
    }
}