/* Date: 01-03-2017
 * Author: K. Korobchinskiy
 * Круги в круге 
 * Задача для ФИКСИРОВАННЫХ РАДИУСОВ
 ******************************** */
using System;
using Cureos.Numerics;
using System.Linq;

namespace hs071_cs
{
    public class OptimalPoints7 :  IDisposable
    {
        public readonly int _n;
        public int _m;
        public int _nele_jac;
        public int _nele_hess;
        public double[] _x_L;
        public double[] _x_U;
        public double[] _g_L;
        public double[] _g_U;

        public double[] X { get; private set; } // Для решателя массив х, в него входят все координаты и радиусы. Т.е. переменные
        public readonly int count; // количество кругов
        public readonly double[] radius;
        private Random _rnd = new Random(); // для генерирования случайной начальной точки

        public OptimalPoints7(double[] r, double[] x = null, double[] y = null, double R = 0)
        {
            count = r.Length; // задаём количетво кругов
            _n = count * 2 + 1; // количество переменных в векторе
            radius = new double[count];
            double sumR = 0;
            int it = 0;
            foreach (var item in r)
            {
                radius[it++] = item;
                sumR += item;
            }
            radius = radius.OrderBy(a => a).ToArray();
            X = new double[_n];
            // Если передаём исходные x и y, то инициализируем ими, иначе генерим
            if (x != null && y != null && R != 0)
            {
                if (x.Length != y.Length || x.Length != r.Length) throw new Exception();
                for (int i = 0; i < count; ++i)
                {
                    X[2 * i] = x[i];
                    X[2 * i + 1] = y[i];
                }
                X[_n - 1] = R;
            }
            else
            {
                // генеририруем R из массивов Х и У
                x = new double[count];
                y = new double[count];
                double maxX = 0;
                double maxY = 0;
                double maxR = 0;
                double maxRXY = 0;
                for (int i = 0; i < count; ++i)
                {
                    X[2 * i] = radius[count - 1] * (_rnd.NextDouble() - 0.5);
                    X[2 * i + 1] = radius[count - 1] * (_rnd.NextDouble() - 0.5);

                    maxX = Math.Max(Math.Abs(X[2 * i] + radius[i]), Math.Abs(X[2 * i] - radius[i]));
                    maxY = Math.Max(Math.Abs(X[2 * i + 1] + radius[i]), Math.Abs(X[2 * i + 1] - radius[i]));
                    maxR = Math.Max(maxX, maxY);
                    maxRXY = Math.Max(maxRXY, maxR);
                }
                X[_n - 1] = maxRXY; // sumR;
            }

            /*    Ограничения переменных
            * *************************************************************************************/
            _x_L = new double[_n];
            double max = radius[count - 1];

            for (var i = 0; i < count; ++i)
            {
                _x_L[2 * i] = _x_L[2 * i + 1] = -DiametrSum(radius) + radius[i];
                if (max < radius[i]) max = radius[i];
            }
            _x_L[_n - 1] = max;

            _x_U = new double[_n];
            for (var i = 0; i < count; ++i)
            {
                _x_U[2 * i] = _x_U[2 * i + 1] = DiametrSum(radius) - radius[i];
            }
            _x_U[_n - 1] = sumR; //Ipopt.PositiveInfinity;

            /*    Огрничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0;
            // (R-r[i])^2-x[i]^2-y[i]^2 >= 0
            _nele_jac += 3 * count;
            _m += count;
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 - (r[i]-r[j])^2 >=0
            _nele_jac += 4 * count * (count - 1) / 2;
            _m += count * (count - 1) / 2;
            _g_L = new double[_m];
            _g_U = new double[_m];
            for (var i = 0; i < _m; i++)
            {
                _g_L[i] = 0;
                _g_U[i] = Ipopt.PositiveInfinity;
            }

            _nele_hess = 0;
        } // End_Конструктор 

        public bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            obj_value = x[_n - 1];

            return true;
        }

        public bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            for (var i = 0; i < _n - 1; i++)
            {
                grad_f[i] = 0;
            }
            grad_f[_n - 1] = 1;
            return true;
        }

        public bool eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int kk = 0;
            // (R-r[i])^2-x[i]^2-y[i]^2 >= 0
            // from 0 to count-1
            for (var i = 0; i < count; ++i)
                g[kk++] = Math.Pow((x[_n - 1] - radius[i]), 2) - x[2 * i] * x[2 * i] - x[2 * i + 1] * x[2 * i + 1];
            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 - (r[i]-r[j])^2 >=0
            // from count to count*(count-1)/2 - 1
            for (var i = 0; i < count - 1; ++i)
            {
                for (var j = i + 1; j < count; ++j)
                    g[kk++] = Math.Pow((x[2 * i] - x[2 * j]), 2) + Math.Pow((x[2 * i + 1] - x[2 * j + 1]), 2) - Math.Pow((radius[i] + radius[j]), 2);
            }

            return true;
        }

        public bool eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int kk = 0;
                var g = 0;
                // позиции по Х и У
                for (g = 0; g < count; ++g)
                {
                    iRow[kk] = g;
                    jCol[kk++] = _n - 1;
                    iRow[kk] = g;
                    jCol[kk++] = 2 * g;
                    iRow[kk] = g;
                    jCol[kk++] = 2 * g + 1;
                }
                for (var i = 0; i < count - 1; ++i)
                {
                    for (var j = i + 1; j < count; ++j)
                    {
                        iRow[kk] = g;
                        jCol[kk++] = 2 * i;
                        iRow[kk] = g;
                        jCol[kk++] = 2 * j;
                        iRow[kk] = g;
                        jCol[kk++] = 2 * i + 1;
                        iRow[kk] = g++;
                        jCol[kk++] = 2 * j + 1;
                    }
                }
            }
            else
            {
                int kk = 0;
                // (R-r[i])^2-x[i]^2-y[i]^2 >= 0
                for (int i = 0; i < count; ++i)
                {
                    values[kk++] = 2 * (x[_n - 1] - radius[i]); // R0'
                    values[kk++] = -2 * x[2 * i]; //X'
                    values[kk++] = -2 * x[2 * i + 1]; //Y'
                }
                // (x[i]-x[j])^2 + (y[i]-y[j])^2 - (r[i]-r[j])^2 >=0
                for (var i = 0; i < count - 1; ++i)
                {
                    for (var j = i + 1; j < count; ++j)
                    {
                        values[kk++] = 2 * (x[2 * i] - x[2 * j]); //X[i]'
                        values[kk++] = -2 * (x[2 * i] - x[2 * j]); //X[j]'
                        values[kk++] = 2 * (x[2 * i + 1] - x[2 * j + 1]); //Y[i]'
                        values[kk++] = -2 * (x[2 * i + 1] - x[2 * j + 1]); //Y[j]'
                    }
                }
            }

            return true;
        }

        public bool eval_h(int n, double[] x, bool new_x, double obj_factor,
                   int m, double[] lambda, bool new_lambda,
                   int nele_hess, int[] iRow, int[] jCol,
                   double[] values)
        {
            return false;
        }

        public override string ToString()
        {
            return "OptimalPoints7";
        }
        /// <summary>
        /// Вычисляем диаметр как сумму всех радиусов
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public double DiametrSum(double[] radius)
        {
            var sum = 0.0;
            foreach (var rad in radius)
            {
                sum += 2 * rad;
            }
            return sum;
        }

        /// <summary>
        /// Инициализация значений всего массива _х
        /// </summary>
        /// <param name="InitialValues">Чем инициализируем</param>
        /// <returns>Проининициализированный массив</returns>
        public double[] InitialValuesManually(double[] InitialValues)
        {
            double[] x = new double[count * 2 + 1];
            for (int i = 0; i < count; ++i)
            {
                x[2 * i] = InitialValues[2 * i];
                x[2 * i + 1] = InitialValues[2 * i + 1];
            }
            x[_n - 1] = InitialValues[_n - 1];
            return x;
        }

        public void Dispose()
        {
            _m = 0;
        }
    }
}