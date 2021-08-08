using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cureos.Numerics;

namespace hs071_cs
{
    internal class FixedRadius3dAdaptor : BaseAdaptor, IDisposable
    {
        public readonly int ballsCount; // количество шаров
        public readonly double[] radius;
        public readonly double[] unsortedRadius;
        private readonly double K1 = 1;
        private readonly double K2 = 1;

        public List<double[]> AllIteration { get; set; }

        private double[] Weight { get; set; }

        public FixedRadius3dAdaptor(Data data)
        {
            AllIteration = new List<double[]>();

            ballsCount = data.ballCount; // задаём количетво кругов
            _n = ballsCount * 3 + 1; // количество переменных в векторе
            radius = new double[ballsCount];
            unsortedRadius = new double[ballsCount];
            //вспомогательные счетчики
            double sumR = 0;
            int it = 0;

            foreach (Ball item in data.ball)
            {
                radius[it++] = item.R;
                sumR += item.R;
            }
            radius = radius.OrderBy(a => a).ToArray();

            /*    Restrictions
            * *************************************************************************************/

            if (ballsCount <= 10)
            {
                K1 = 1;
            }
            else
            {
                K1 = 0.4;
            }

            _x_L = new double[_n];
            _x_U = new double[_n];

            double max = radius[ballsCount - 1];

            // обнуляем необходимые счетчики
            int countXYZR = 0,
                countCoordinate = 0;
            for (int i = 0; i < ballsCount; i++)
            {
                //if (data.ball[i].ObjectType == (ObjectType)2)
                //{
                //    _x_L[3 * i] = data.ball[i].X;
                //    _x_U[3 * i] = data.ball[i].X;

                //    _x_L[3 * i + 1] = data.ball[i].Y;
                //    _x_U[3 * i + 1] = data.ball[i].Y;

                //    _x_L[3 * i + 2] = data.ball[i].Z;
                //    _x_U[3 * i + 2] = data.ball[i].Z;
                //}
                //else
                //{
                _x_L[3 * i] = Ipopt.NegativeInfinity;// -K1 * DiametrSum(radius) + radius[countCoordinate];
                _x_U[3 * i] = Ipopt.PositiveInfinity;// K1 * DiametrSum(radius) - radius[countCoordinate];

                _x_L[3 * i + 1] = Ipopt.NegativeInfinity; // - K1 * DiametrSum(radius) + radius[countCoordinate];
                _x_U[3 * i + 1] = Ipopt.PositiveInfinity;// K1 * DiametrSum(radius) - radius[countCoordinate];

                _x_L[3 * i + 2] = 0;// Ipopt.NegativeInfinity;// - K1 * DiametrSum(radius) + radius[countCoordinate];
                _x_U[3 * i + 2] = 0;// Ipopt.PositiveInfinity; // K1 * DiametrSum(radius) - radius[countCoordinate];
                //}
                if (max < radius[countCoordinate])
                {
                    max = radius[countCoordinate];
                }

                countCoordinate++;
                countXYZR++;
            }

            _x_L[_n - 1] = max;
            _x_U[_n - 1] = sumR * K1;

            /*    Огрaничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0;

            // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            _nele_jac += 4 * ballsCount; // x, y, z , R
            _m += ballsCount;

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            int v = 3 * ballsCount * (ballsCount - 1);
            _nele_jac += v;  // 2*3 два ограничения по 3 ненулевых частных производных
            int v1 = ballsCount * (ballsCount - 1) / 2;
            _m += v1;

            //m[i]*x[i] + count
            // _nele_jac += 3 * countCircles;
            // _m += countCircles;

            _g_L = new double[_m];
            _g_U = new double[_m];
            int op = 0;
            for (int j = 0; j < ballsCount; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }
            for (int i = 0; i < ballsCount - 1; i++)
            {
                for (int j = i + 1; j < ballsCount; j++)
                {
                    _g_L[op] = Math.Pow((radius[i] + radius[j]), 2);
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
            _nele_hess = 0;

        }

        private void AddNewIteration(object element)
        {
            AllIteration.Add((double[])element);
        }

        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            obj_value = K2 * x[_n - 1];
            AddNewIteration(x);
            return true;
        }

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            grad_f[_n - 1] = K1 * 1;
            return true;
        }

        public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int kk = 0;
            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            // from 0 to count-1
            Parallel.For(0, ballsCount, i =>
            {
                g[kk++] = Math.Pow((x[_n - 1] - radius[i]), 2) -
                    x[3 * i] * x[3 * i] -          // x
                    x[3 * i + 1] * x[3 * i + 1] -  // y
                    x[3 * i + 2] * x[3 * i + 2];   // z
            });
            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            // from count to count*(count-1)/2 - 1

            for (int i = 0; i < ballsCount - 1; i++) // на каждой итерации увеличиваем на 3 счетч. по Z
            {
                for (int j = i + 1; j < ballsCount; j++)
                {
                    g[kk++] = Math.Pow((x[3 * i] - x[3 * j]), 2.0)
                                  + Math.Pow((x[3 * i + 1] - x[3 * j + 1]), 2.0)
                                  + Math.Pow((x[3 * i + 2] - x[3 * j + 2]), 2.0)
                                  - Math.Pow((radius[i] - radius[j]), 2.0);
                }
            }
            ////Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
            //Parallel.For(0, countCircles, i =>
            //{
            //    g[kk++] = Math.Pow(Weight[i] * x[3 * i], 2) + Math.Pow(Weight[i] * x[3 * i + 1], 2) + Math.Pow(Weight[i] * x[3 * i + 2], 2);
            //});
            return true;
        }

        public override bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int kk = 0,
                    g = 0;

                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                // позиции R, Х и У, Z
                for (g = 0; g < ballsCount; ++g)
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
                for (int i = 0; i < ballsCount - 1; ++i)
                {
                    for (int j = i + 1; j < ballsCount; ++j)
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
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                int kk = 0;
                for (int i = 0; i < ballsCount; i++)// шаг по Z это каждый третий эл
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
                //  Print("---------------------------------------");

                for (int i = 0; i < ballsCount - 1; i++)
                {
                    for (int j = i + 1; j < ballsCount; j++)
                    {
                        values[kk++] = 2.0 * (x[3 * i] - x[3 * j]); //X[i]'
                        values[kk++] = -2.0 * (x[3 * i] - x[3 * j]); //X[j]'

                        values[kk++] = 2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[i]'
                        values[kk++] = -2.0 * (x[3 * i + 1] - x[3 * j + 1]); //Y[j]'

                        values[kk++] = 2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[i]'
                        values[kk++] = -2.0 * (x[3 * i + 2] - x[3 * j + 2]); //Z[j]'
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

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        // Вычисляем диаметр как сумму всех радиусов
        private double DiametrSum(double[] radius)
        {
            double sum = 0.0;
            foreach (double rad in radius)
            {
                sum += 2 * rad;
            }
            return sum;
        }

        public void Dispose()
        {
            _m = 0;
        }
    }
}
