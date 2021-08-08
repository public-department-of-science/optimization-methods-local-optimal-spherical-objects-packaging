using Cureos.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace hs071_cs
{
    class UnFixedRadius3dAdaptor : BaseAdaptor, IDisposable
    {
        public readonly int countCircles; // количество шаров
        public double[] X { get; } //массив х (входят все координаты и радиусы)
        public readonly double[] sortedRadius;
        public readonly double[] unsortedRadius;

        private double K1 = 1;
        private double K2 = 1;
        double TAU = 0; // accuracy

        public List<double[]> _allIteration;
        public List<double[]> AllIteration { get => _allIteration; set => _allIteration = value; }

        private double[] Weight { get; set; }
        private double[,] C { get; set; }

        Data GetDataFromUser;
        public UnFixedRadius3dAdaptor(Data data)
        {
            try
            {
                GetDataFromUser = data ?? throw new Exception("Source object doesn't exist!");
            }
            catch (Exception p)
            {
                Console.WriteLine(p.Message);
            }

            AllIteration = new List<double[]>();

            countCircles = data.ballCount; // задаём количетво кругов
            _n = countCircles * 4 + 1; // количество переменных в векторе
            sortedRadius = new double[countCircles];
            unsortedRadius = new double[countCircles];
            //вспомогательные счетчики
            double sumR = 0;
            int it = 0;

            foreach (var item in data.ball)
            {
                sortedRadius[it++] = item.R;
                sumR += item.R;
            }
            sortedRadius = sortedRadius.OrderBy(a => a).ToArray();

            X = new double[_n];
            try
            {
                if (data.ball != null && data != null && data.R >= 0)
                {
                    for (int i = 0; i < countCircles; i++)
                    {
                        X[4 * i] = data.ball[i].X;
                        X[4 * i + 1] = data.ball[i].Y;
                        X[4 * i + 2] = data.ball[i].Z;
                        X[4 * i + 3] = data.ball[i].R;
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
            _x_U = new double[_n];

            double max = sortedRadius[countCircles - 1];

            // обнуляем необходимые счетчики
            int countXYZR = 0,
                countCoordinate = 0;
            for (int i = 0; i < countCircles; i++)
            {
                if (data.ball[i].ObjectType == (ObjectType)2)
                {
                    _x_L[4 * i] = data.ball[i].X;
                    _x_U[4 * i] = data.ball[i].X;

                    _x_L[4 * i + 1] = data.ball[i].Y;
                    _x_U[4 * i + 1] = data.ball[i].Y;

                    _x_L[4 * i + 2] = data.ball[i].Z = data.ball[i].Z;
                    _x_U[4 * i + 2] = data.ball[i].Z = data.ball[i].Z;

                    _x_L[4 * i + 3] = data.ball[i].R;
                    _x_U[4 * i + 3] = data.ball[i].R;
                }
                else
                {
                    _x_L[4 * i] = -K1 * DiametrSum(sortedRadius) + sortedRadius[countCoordinate];
                    _x_U[4 * i] = K1 * DiametrSum(sortedRadius) - sortedRadius[countCoordinate];

                    _x_L[4 * i + 1] = -K1 * DiametrSum(sortedRadius) + sortedRadius[countCoordinate];
                    _x_U[4 * i + 1] = K1 * DiametrSum(sortedRadius) - sortedRadius[countCoordinate];

                    _x_L[4 * i + 2] = -K1 * DiametrSum(sortedRadius) + sortedRadius[countCoordinate];
                    _x_U[4 * i + 2] = K1 * DiametrSum(sortedRadius) - sortedRadius[countCoordinate];

                    _x_L[4 * i + 3] = 0;
                    _x_U[4 * i + 3] = Ipopt.PositiveInfinity;// K1 * DiametrSum(sortedRadius) - sortedRadius[countCoordinate];
                }
                if (max < sortedRadius[countCoordinate])
                    max = sortedRadius[countCoordinate];
                countCoordinate++;
                countXYZR++;
            }

            _x_L[_n - 1] = data.R;// sumR * K1;
            _x_U[_n - 1] = data.R;// sumR * K1;

            /*    Огрaничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0;

            // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            _nele_jac += 4 * countCircles; // x, y, z , R
            _m += countCircles;

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            _nele_jac += 2 * 4 * countCircles * (countCircles - 1) / 2;  // 2 * 3 два ограничения по 3 ненулевых частных производных
            _m += countCircles * (countCircles - 1) / 2;

            //m[i]*x[i] + count
            // _nele_jac += 3 * countCircles;
            // _m += countCircles;

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
                    _g_L[op] = 0; //Math.Pow((sortedRadius[i] + sortedRadius[j]), 2);
                    _g_U[op++] = Ipopt.PositiveInfinity;
                }
            }

            //Constraints of variebles For  Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
            //for (int i = 0; i < countCircles; i++)
            //{
            //    _g_L[op] = 0;
            //    _g_U[op++] = eps;
            //}

            //Hesian
            Weight = new double[data.ballCount];
            for (int i = 0; i < data.ballCount; i++)
            {
                Weight[i] = data.ball[i].Weight;
            }
            C = data.C ?? null;
            _nele_hess = 0;

        } // End_Конструктор 

        private void AddNewIteration(object element)
        {
            AllIteration.Add((double[])element);
        }
        private double SumRadiusPowered(double[] x, int pow)
        {
            double val = 0.0;
            for (int i = 0; i < countCircles; i++)
            {
                val += Math.Pow(x[4 * i + 3], pow);
            }
            return val;
        }

        public override bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            obj_value = K2 * C_Multiply_by_Length(x) - (4.0 / 3.0) * Math.PI * SumRadiusPowered(x, 3);//+ K1 * x[_n - 1];
            ThreadPool.GetMinThreads(out int minWorker, out int minIOC);
            ThreadPool.SetMaxThreads(minWorker, minIOC);
            ThreadPool.QueueUserWorkItem(new WaitCallback(AddNewIteration), x);
            return true;
        }

        public override bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            #region Length Bond
            double[] Xderivative = new double[countCircles];
            double[] Yderivative = new double[countCircles];
            double[] Zderivative = new double[countCircles];
            double[] Rderivative = new double[countCircles];
            ////
            Parallel.For(0, countCircles - 1, i =>
            {
                Parallel.For(i + 1, countCircles, body: j =>
                {
                    Xderivative[i] += C[i, j] * (x[4 * i] - x[4 * j]) /
                     (
                     Math.Sqrt(
                         Math.Pow(x[4 * i], 2) - 2 * x[4 * i] * x[4 * j] + Math.Pow(x[4 * j], 2) // x
                         + Math.Pow(x[4 * i + 1] - x[4 * j + 1], 2) // y
                         + Math.Pow(x[4 * i + 2] - x[4 * j + 2], 2) // z
                         + Math.Pow(x[4 * i + 3] - x[4 * j + 3], 2) // r
                              )
                     );

                    Yderivative[i] += C[i, j] * (x[4 * i + 1] - x[4 * j + 1]) /
                        (
                        Math.Sqrt(
                            Math.Pow(x[4 * i] - x[4 * j], 2) // x
                            + Math.Pow(x[4 * i + 1], 2) - 2 * x[4 * i + 1] * x[4 * j + 1] + Math.Pow(x[4 * j + 1], 2) // y
                            + Math.Pow(x[4 * i + 2] - x[4 * j + 2], 2) // z
                            + Math.Pow(x[4 * i + 3] - x[4 * j + 3], 2) // r
                                 )
                        );

                    Zderivative[i] += C[i, j] * (x[4 * i + 2] - x[4 * j + 2]) /
                        (
                        Math.Sqrt(
                            Math.Pow(x[4 * i] - x[4 * j], 2) // x
                            + Math.Pow(x[4 * i + 1] - x[4 * j + 1], 2) // y
                            + Math.Pow(x[4 * i + 2], 2) - 2 * x[4 * i + 2] * x[4 * j + 2] + Math.Pow(x[4 * j + 2], 2) // z
                            + Math.Pow(x[4 * i + 3] - x[4 * j + 3], 2) // r
                                 )
                        );

                    Rderivative[i] += C[i, j] * (x[4 * i + 3] - x[4 * j + 3]) /
                        (
                        Math.Sqrt(
                            Math.Pow(x[4 * i] - x[4 * j], 2) // x
                            + Math.Pow(x[4 * i + 1] - x[4 * j + 1], 2) // y
                            + Math.Pow(x[4 * i + 2] - x[4 * j + 2], 2) // y
                            + Math.Pow(x[4 * i + 3], 2) - 2 * x[4 * i + 3] * x[4 * j + 3] + Math.Pow(x[4 * j + 3], 2) // r
                                 )
                        );
                });
            });
            Parallel.For(0, countCircles, i =>
            {
                grad_f[4 * i] = K2 * Xderivative[i];
                grad_f[4 * i + 1] = K2 * Yderivative[i];
                grad_f[4 * i + 2] = K2 * Zderivative[i];
                grad_f[4 * i + 3] = K2 * Rderivative[i] - (4.0 / 3.0) * 3 * Math.PI * SumRadiusPowered(x, 2);
            });
            #endregion
            grad_f[_n - 1] = 0;// K1 * 1;
            return true;
        }
        public override bool eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int kk = 0;
            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            // from 0 to count-1
            Parallel.For(0, countCircles, i =>
            {
                g[kk++] = Math.Pow((x[_n - 1] - x[4 * i + 3]), 2) -
                    x[4 * i] * x[4 * i] -          // x
                    x[4 * i + 1] * x[4 * i + 1] -  // y
                    x[4 * i + 2] * x[4 * i + 2];   // z
            });
            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            // from count to count*(count-1)/2 - 1

            for (int i = 0; i < countCircles - 1; i++)
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    g[kk++] = Math.Pow((x[4 * i] - x[4 * j]), 2.0)
                                  + Math.Pow((x[4 * i + 1] - x[4 * j + 1]), 2.0)
                                  + Math.Pow((x[4 * i + 2] - x[4 * j + 2]), 2.0)
                                  - Math.Pow((x[4 * i + 3] + x[4 * j + 3]), 2.0);
                }
            }
            ////Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
            //Parallel.For(0, countCircles, i =>
            //{
            //    g[kk++] = Math.Pow(Weight[i] * x[3 * i], 2) + Math.Pow(Weight[i] * x[3 * i + 1], 2) + Math.Pow(Weight[i] * x[3 * i + 2], 2);
            //});
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
                    jCol[kk++] = 4 * g;

                    //Y
                    iRow[kk] = g;
                    jCol[kk++] = 4 * g + 1;

                    //Z
                    iRow[kk] = g;
                    jCol[kk++] = 4 * g + 2;
                }

                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
                for (int i = 0; i < countCircles - 1; ++i)
                {
                    for (int j = i + 1; j < countCircles; ++j)
                    {
                        // -------  X[i], X[j] ------- 
                        iRow[kk] = g;
                        jCol[kk++] = 4 * i;
                        iRow[kk] = g;
                        jCol[kk++] = 4 * j;

                        // -------  Y[i], Y[j] ------- 
                        iRow[kk] = g; ;
                        jCol[kk++] = 4 * i + 1;
                        iRow[kk] = g;
                        jCol[kk++] = 4 * j + 1;

                        // -------  Z[i], Z[j] ------- 
                        iRow[kk] = g;
                        jCol[kk++] = 4 * i + 2;
                        iRow[kk] = g;
                        jCol[kk++] = 4 * j + 2;

                        // -------  r[i], r[j] ------- 
                        iRow[kk] = g;
                        jCol[kk++] = 4 * i + 3;
                        iRow[kk] = g;
                        jCol[kk++] = 4 * j + 3;

                        ++g;
                    }
                }

                //// Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
                //Parallel.For(0, countCircles, i =>
                //{
                //    // Weight[i] * X[i]
                //    iRow[kk] = g;
                //    jCol[kk++] = 0;

                //    // Weight[i] * Y[i] 
                //    iRow[kk] = g;
                //    jCol[kk++] = 1;

                //    // Weight[i] * Z[i]
                //    iRow[kk] = g;
                //    jCol[kk++] = 2;
                //    ++g;
                //});
            }
            else
            {
                int kk = 0;
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                for (int i = 0; i < countCircles; i++)// шаг по Z это каждый третий эл
                {
                    values[kk] = 2.0 * (x[_n - 1] - x[4 * i + 3]); // R0'
                    kk++;
                    values[kk] = -2.0 * x[4 * i]; //X'
                    kk++;
                    values[kk] = -2.0 * x[4 * i + 1]; //Y'
                    kk++;
                    values[kk] = -2.0 * x[4 * i + 2]; //Z'
                    kk++;
                }
                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i] + r[j])^2 >=0
                //  Console.WriteLine("---------------------------------------");

                for (int i = 0; i < countCircles - 1; i++)
                {
                    for (int j = i + 1; j < countCircles; j++)
                    {
                        values[kk++] = 2.0 * (x[4 * i] - x[4 * j]); //X[i]'
                        values[kk++] = -2.0 * (x[4 * i] - x[4 * j]); //X[j]'

                        values[kk++] = 2.0 * (x[4 * i + 1] - x[4 * j + 1]); //Y[i]'
                        values[kk++] = -2.0 * (x[4 * i + 1] - x[4 * j + 1]); //Y[j]'

                        values[kk++] = 2.0 * (x[4 * i + 2] - x[4 * j + 2]); //Z[i]'
                        values[kk++] = -2.0 * (x[4 * i + 2] - x[4 * j + 2]); //Z[j]'

                        values[kk++] = -2.0 * (x[4 * i + 3] + x[4 * j + 3]); //r[i]'
                        values[kk++] = -2.0 * (x[4 * i + 3] + x[4 * j + 3]); //r[j]'
                    }
                }

                //////mx;my;mz
                //Parallel.For(0, countCircles, i =>
                //{
                //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i];
                //    kk++;
                //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i + 1];
                //    kk++;
                //    values[kk] = 2 * Math.Pow(Weight[i], 2) * x[3 * i + 2];
                //    kk++;
                //});
            }
            return true;
        }

        public override bool eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
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
        private double C_Multiply_by_Length(double[] x)
        {
            double Sum = 0;
            Parallel.For(0, countCircles - 1, i =>
                {
                    Parallel.For(i + 1, countCircles, j =>
                       {
                           Sum += C[i, j] * Math.Sqrt(Math.Pow(x[4 * i] - x[4 * j], 2)
                        + Math.Pow(x[4 * i + 1] - x[4 * j + 1], 2)
                        + Math.Pow(x[4 * i + 2] - x[4 * j + 2], 2)
                        + Math.Pow(x[4 * i + 3] - x[4 * j + 3], 2));
                       });
                });
            return Sum;
        }
        public void Dispose()
        {
            _m = 0;
        }
    }
}