using Cureos.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace hs071_cs
{
    public class FixedRadius3dAdaptor : IDisposable
    {
        public readonly int _n;
        public int _m;
        public int _nele_jac;
        public int _nele_hess;
        public double[] _x_L;
        public double[] _x_U;
        public double[] _g_L;
        public double[] _g_U;

        public double SummedWeight;

        public double[] X { get; private set; } // Для решателя массив х, в него входят все координаты и радиусы. Т.е. переменные
        public double[] Weight { get; } // переданные веса шаров
        public double Fine = 1.5; // траф функции
        public double[] Z { get; }

        public readonly int count; // количество кругов
        public readonly double[] radius;
        public readonly double[] unsortedRadius;
        private Random _rnd = new Random(); // для генерирования случайной начальной точки

        public FixedRadius3dAdaptor(double[] r, double[] x = null, double[] y = null, double[] z = null, double[] weight = null, double R = 0)
        {
            count = r.Length; // задаём количетво кругов
            _n = count * 3 + 1; // количество переменных в векторе
            radius = new double[count];
            unsortedRadius = r;
            Z = z;
            //вспомогательные счетчики
            double sumR = 0;
            int it = 0;
            int countXYZR = 0,
                countCoordinate = 0;
            //

            foreach (var item in r)
            {
                radius[it++] = item;
                sumR += item;
            }
            radius = radius.OrderBy(a => a).ToArray();
            X = new double[_n];
            // Если передаём исходные x и y, то инициализируем ими, иначе генерим
            if (x != null && y != null && z != null && R != 0)
            {
                if (x.Length != y.Length || x.Length != r.Length || z.Length != y.Length)
                    throw new Exception();

                for (int i = 0; i < count; i++)
                {
                    X[3 * i] = x[i];
                    X[3 * i + 1] = y[i];
                    X[3 * i + 2] = z[i];
                }
                X[_n - 1] = R;
            }
            else
            {
                Console.WriteLine("!!!!!!!!!!!!!!");
            }

            /*    Ограничения переменных
            * *************************************************************************************/
            _x_L = new double[_n];
            double max = radius[count - 1];
            int q = 0;// счетчик по Z

            // обнуляем необходимые счетчики
            countXYZR = 0;
            countCoordinate = 0;

            while (countCoordinate < count)
            {
                for (int i = 0; i < 2; i++)
                {
                    _x_L[countXYZR] = x[i];
                    _x_L[++countXYZR] = y[i];
                    _x_L[++countXYZR] = z[i];
                    continue;
                }
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

            while (countCoordinate < count)
            {
                for (int i = 0; i < 2; i++)
                {
                    _x_U[countXYZR] = x[i];
                    _x_U[++countXYZR] = y[i];
                    _x_U[++countXYZR] = z[i];
                    continue;
                }
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
            _nele_jac += 4 * count; // x, y, z , R
            _m += count;

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0

            _nele_jac += 2 * 3 * count * (count - 1) / 2;  // 2*3 два ограничения по 3 ненулевых частных производных

            _m += count * (count - 1) / 2; //
            _g_L = new double[_m];
            _g_U = new double[_m];
            int op = 0;
            for (int j = 0; j < count; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }
            for (int i = 0; i < count - 1; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    _g_L[op] = Math.Pow((radius[i] + radius[j]), 2);
                    _g_U[op++] = Ipopt.PositiveInfinity;
                }
            }

            //Hesian
            _nele_hess = 4;

            // 
            Weight = new double[count * 3];
            for (int i = 0; i < count; i++)
            {
                Weight[3 * i] = weight[i];
                Weight[3 * i + 1] = weight[i];
                Weight[3 * i + 2] = weight[i];
            }
            SummedWeight = SummWeight();
        } // End_Конструктор 

        public bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            //M it's Fine for Function f(x)
            // R + M * Summ(mi*xi)^2 + M * Summ(mi*yi)^2 + M * Summ(mi*zi)^2 -> min

            obj_value = x[_n - 1] + Fine * (Math.Pow(SummedWeight * SummedWeight * SummX(x), 2.0)) +
                Fine * (Math.Pow(SummedWeight * SummedWeight * SummY(x), 2.0)) +
                Fine * (Math.Pow(SummedWeight * SummedWeight * SummZ(x), 2.0));

            return true;
        }

        public bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            for (int i = 0; i < _n - 1; i++)
            {
                grad_f[i] = 2 * Fine * x[i];
            }
            grad_f[_n - 1] = 1;
            return true;
        }

        public bool eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int kk = 0;
            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            // from 0 to count-1
            for (int i = 0; i < count; i++) // счетчик по Z
            {
                g[kk++] = Math.Pow((x[_n - 1] - radius[i]), 2) -
                    x[3 * i] * x[3 * i] -     // x
                    x[3 * i + 1] * x[3 * i + 1] -  // y
                    x[3 * i + 2] * x[3 * i + 2];      // z
            }
            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            // from count to count*(count-1)/2 - 1

            for (int i = 0; i < count - 1; i++) // на каждой итерации увеличиваем на 3 счетч. по Z
            {
                for (int j = i + 1; j < count; j++)
                {
                    g[kk++] = Math.Pow((x[3 * i] - x[3 * j]), 2.0)
                                  + Math.Pow((x[3 * i + 1] - x[3 * j + 1]), 2.0)
                                  + Math.Pow((x[3 * i + 2] - x[3 * j + 2]), 2.0)
                                  - Math.Pow((radius[i] - radius[j]), 2.0);
                }
            }
            return true;
        }

        public bool eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int kk = 0,
                    g = 0;

                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                // позиции R, Х и У, Z
                for (g = 0; g < count; ++g)
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

                for (int i = 0; i < count - 1; ++i)
                {
                    for (int j = i + 1; j < count; ++j)
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
                for (int i = 0; i < count; i++)// шаг по Z это каждый третий эл
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

                for (int i = 0; i < count - 1; i++)
                {
                    for (int j = i + 1; j < count; j++)
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

        public bool eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            Console.WriteLine("+_+_+_+_+_+_+");
            if (values == null)
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
            }

            return true;
        }

        public override string ToString()
        {
            return "AdaptorFixedRadius";
        }

        // Вычисляем диаметр как сумму всех радиусов
        public double DiametrSum(double[] radius)
        {
            var sum = 0.0;
            foreach (var rad in radius)
            {
                sum += 2 * rad;
            }
            return sum;
        }

        // методы сумирования по отдельным переменным
        public double SummWeight()
        {
            double summ = 0.0;
            for (int i = 0; i < Weight.Length; i++)
            {
                summ += Weight[i];
            }
            return summ;
        }
        public double SummX(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < count; i++)
            {
                sum += x[3 * i];
            }
            return sum;
        }
        public double SummY(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < count; i++)
            {
                sum += x[3 * i + 1];
            }
            return sum;
        }
        public double SummZ(double[] x)
        {
            double sum = 0.0;
            for (int i = 0; i < count; i++)
            {
                sum += x[3 * i + 2];
            }
            return sum;
        }

        public void Dispose()
        {
            _m = 0;
        }
    }
}