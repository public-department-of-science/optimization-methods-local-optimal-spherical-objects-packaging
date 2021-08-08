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
        public double[] X { get; } //массив х (входят все координаты и радиусы)

        private readonly int countCircles; // количество шаров
        private readonly double[] sortedRadius;
        private double[] unsortedRadius;
        private readonly int elementsInTranspositionPolyhedron; // 2^circleCount
        private readonly int elementsInJacLinPolyhedron; // OneCounter
        private readonly double[] arrayWithRightPart;
        private readonly string[] leftPatOfArrayInBinary;
        private double K1 = 1;
        private readonly double TAU;
        public List<double[]> AllIteration { get; set; }

        public UnFixedRadius3dAdaptor(Data data)
        {
            AllIteration = new List<double[]>();
            countCircles = data.ballCount; // задаём количетво кругов
            _n = countCircles * 4 + 1; // количество переменных в векторе
            sortedRadius = new double[countCircles];
            unsortedRadius = new double[countCircles];
            //вспомогательные счетчики
            for (int i = 0; i < data.ball.Length; ++i)
            {
                sortedRadius[i] = data.ball[i].R;
                unsortedRadius[i] = data.ball[i].R;
            }
            double sumR = sortedRadius.Sum();
            sortedRadius = sortedRadius.OrderBy(a => a).ToArray();
            TAU = 0.00;// sumR / data.ballCount;
            RightAndLeftPartOfArrayForMethodUnfixedRadius(out leftPatOfArrayInBinary, out arrayWithRightPart, data.ballCount, data);
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

            if (countCircles <= 10)
                K1 = 1;
            else
                K1 = 0.4;

            _x_L = new double[_n];
            _x_U = new double[_n];
            var diametrSum = 2 * sortedRadius.Sum();
            for (int i = 0; i < countCircles; i++)
            {
                _x_L[4 * i] = _x_L[4 * i + 1] = _x_L[4 * i + 2] = -K1 * diametrSum + sortedRadius[i];
                _x_U[4 * i] = _x_U[4 * i + 1] = _x_U[4 * i + 2] = K1 * diametrSum - sortedRadius[i];
                _x_L[4 * i + 3] = sortedRadius[0];
                _x_U[4 * i + 3] = sortedRadius[countCircles - 1];
            }
            _x_L[_n - 1] = sortedRadius[countCircles - 1]; // От максимального радиуса
            _x_U[_n - 1] = sumR; // до суммы радиусов

            /*    Огрaничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0;
            // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            _nele_jac += 5 * countCircles; // x, y, z , r, R
            _m += countCircles;
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]+r[j])^2 >=0
            _nele_jac += 2 * 4 * countCircles * (countCircles - 1) / 2;  // 2 * 3 два ограничения по 3 ненулевых частных производных
            _m += countCircles * (countCircles - 1) / 2;

            // перестановочный многогранник
            elementsInTranspositionPolyhedron = (int)Math.Pow(2, countCircles);
            elementsInJacLinPolyhedron = OneCounter(leftPatOfArrayInBinary);
            _nele_jac += elementsInJacLinPolyhedron;
            _m += elementsInTranspositionPolyhedron;

            _g_L = new double[_m];
            _g_U = new double[_m];
            int op = 0;
            for (int j = 0; j < countCircles; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }
            // Constraint for "Формирование условий попарного непересечания объектов"
            for (int i = 0; i < countCircles - 1; i++)
            {
                for (int j = i + 1; j < countCircles; j++)
                {
                    _g_L[op] = 0;
                    _g_U[op++] = Ipopt.PositiveInfinity;
                }
            }
            // Constraint for "перестановочного многоранника"
            // В правую часть помещаем константы, т.е. от констрант до +inf
            for (int i = 0; i < elementsInTranspositionPolyhedron; i++)
            {
                _g_L[op] = arrayWithRightPart[i]; // TODO: Это сумма?
                _g_U[op++] = Ipopt.PositiveInfinity;
            }
            _g_L[op - 1] = arrayWithRightPart[arrayWithRightPart.Length - 1];
            _g_U[op - 1] = arrayWithRightPart[arrayWithRightPart.Length - 1];
            _nele_hess = 0;
        } // End_Конструктор 
        private int OneCounter(string[] leftPatOfArrayInBinary)
        {
            int res = 0;
            // r + r + r ... >= cr + cr + cr ...
            for (int i = 0; i < leftPatOfArrayInBinary.Length; ++i)
                for (int b = 0; b < leftPatOfArrayInBinary[i].Length; ++b)
                    if (leftPatOfArrayInBinary[i][b] == '1')
                        res++;
            // sum[r-tau]^2 = sum[cr-tau]^2
            for (int i = 0; i < countCircles; ++i)
                res++;
            return res;
        }
        private void RightAndLeftPartOfArrayForMethodUnfixedRadius(out string[] leftPart, out double[] rightPart, int circleCount, Data balls)
        {
            int countOfelements = (int)Math.Pow(2, circleCount);
            double sum = 0;
            int ballCount = balls.ballCount;
            leftPart = new string[countOfelements]; // binary string for 2^n elements
            rightPart = new double[countOfelements]; // right part for left string value (in binary format)
            StringBuilder str = new StringBuilder();
            string temp;

            // Left part
            for (int i = 1; i < countOfelements; i++)
            {
                temp = Convert.ToString(i, 2);
                if (temp.Length != ballCount)
                {
                    str.Append(temp);
                    while (str.Length != ballCount)
                    {
                        str.Insert(0, "0", 1);
                    }
                }
                else
                    str.Append(temp);
                leftPart[i - 1] = str.ToString();
                str.Clear();
            }
            leftPart[countOfelements - 1] = Convert.ToString((int)sum, 2); //

            // right part
            for (int i = 1; i < countOfelements; i++)
            {
                temp = leftPart[i - 1];
                for (int k = 0; k < ballCount; k++)
                {
                    if (temp[k] == '1')
                        rightPart[i - 1] += balls.ball[k].R;
                }
            }
            foreach (var item in balls.ball)
            {
                sum += Math.Pow(item.R - TAU, 2);
            }
            rightPart[countOfelements - 1] = sum;
        }
        private void AddNewIteration(object element)
        {
            AllIteration.Add((double[])element);
        }
        public override bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            int minWorker, minIOC;
            //obj_value = K2 * x[_n - 1] + K2 * C_Multiply_by_Length(x);
            obj_value = x[_n - 1];
            ThreadPool.GetMinThreads(out minWorker, out minIOC);
            ThreadPool.SetMaxThreads(minWorker, minIOC);
            ThreadPool.QueueUserWorkItem(new WaitCallback(AddNewIteration), x);
            return true;
        }
        public override bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            for (var i = 0; i < _n - 1; i++)
            {
                grad_f[i] = 0;
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
                g[kk++] = Math.Pow((x[_n - 1] - x[4 * i + 3]), 2) //(R-r)^2
                         - x[4 * i] * x[4 * i]            // x^2
                         - x[4 * i + 1] * x[4 * i + 1]    // y^2
                         - x[4 * i + 2] * x[4 * i + 2];   // z^2
            };
            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]+r[j])^2 >=0
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
            double[] a = null;

            NewRadius(out a, x); // TODO: Не пойму что происходит
            for (int i = 0; i < elementsInTranspositionPolyhedron; i++)
            {
                g[kk++] = a[i]; // - arrayWithRightPart[i];
            }
            return true;
        }
        // Transition polyhedron
        private void NewRadius(out double[] arrayWithNewUnfixedRadius, double[] x)
        {
            int countOfelements = (int)Math.Pow(2, countCircles);
            string temp;
            arrayWithNewUnfixedRadius = new double[arrayWithRightPart.Length];
            double sum = 0.0;
            for (int i = 1; i < countOfelements; i++)
            {
                temp = leftPatOfArrayInBinary[i - 1];
                for (int k = 0; k < countCircles; k++)
                {
                    if (temp[k] == '1')
                        arrayWithNewUnfixedRadius[i - 1] += x[4 * k + 3];
                }
            }
            //leftPatOfArrayInBinary[]
            int length = countCircles;
            for (int i = 0; i < length; i++)
                sum += Math.Pow(x[4 * i + 3] - TAU, 2);
            arrayWithNewUnfixedRadius[arrayWithRightPart.Length - 1] = sum;
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

                    //r
                    iRow[kk] = g;
                    jCol[kk++] = 4 * g + 3;
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
                //Transition polyhedron
                for (int i = 0; i < elementsInTranspositionPolyhedron; i++)
                {
                    for (int b = 0; b < leftPatOfArrayInBinary[i].Length; ++b)
                    {
                        if (leftPatOfArrayInBinary[i][b] == '1')
                        {
                            iRow[kk] = g;
                            jCol[kk++] = 4 * b + 3;
                        }
                    }
                    g++;
                }
                for (int i = 0; i < countCircles; i++)
                {
                    iRow[kk] = g - 1;
                    jCol[kk++] = 4 * i + 3;
                }
            }
            else
            {
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2  >= 0
                int kk = 0;
                for (int i = 0; i < countCircles; i++)
                {
                    values[kk++] = +2 * (x[_n - 1] - x[4 * i + 3]); // R0'
                    values[kk++] = -2 * x[4 * i]; //X'
                    values[kk++] = -2 * x[4 * i + 1]; //Y'
                    values[kk++] = -2 * x[4 * i + 2]; //Z'
                    values[kk++] = -2 * (x[_n - 1] - x[4 * i + 3]); //r'
                }
                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i] + r[j])^2 >=0
                //  Console.WriteLine("---------------------------------------");
                for (int i = 0; i < countCircles - 1; i++)
                {
                    for (int j = i + 1; j < countCircles; j++)
                    {
                        values[kk++] = +2 * (x[4 * i] - x[4 * j]); //X[i]'
                        values[kk++] = -2 * (x[4 * i] - x[4 * j]); //X[j]'

                        values[kk++] = +2 * (x[4 * i + 1] - x[4 * j + 1]); //Y[i]'
                        values[kk++] = -2 * (x[4 * i + 1] - x[4 * j + 1]); //Y[j]'

                        values[kk++] = +2 * (x[4 * i + 2] - x[4 * j + 2]); //Z[i]'
                        values[kk++] = -2 * (x[4 * i + 2] - x[4 * j + 2]); //Z[j]'

                        values[kk++] = -2 * (x[4 * i + 3] + x[4 * j + 3]); //r[i]'
                        values[kk++] = -2 * (x[4 * i + 3] + x[4 * j + 3]); //r[j]'
                    }
                }
                for (int i = 0; i < elementsInJacLinPolyhedron - countCircles; i++)
                {
                    values[kk++] = 1;
                }
                for (int i = 0; i < countCircles; i++)
                    values[kk++] = 2 * (x[4 * i + 3] - TAU);
            }
            return true;
        }
        public void Dispose()
        {
            _m = 0;
        }
        public override bool eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }
    }
}