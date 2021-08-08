using Cureos.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace hs071_cs
{
    class FixedAndVariableRadiusAdaptor : BaseAdaptor, IDisposable
    {
        public double[] X { get; } //массив х (входят все координаты и радиусы)
        public int VariableBALLSCount { get; private set; }
        private readonly int ballsCount; // количество шаров
        private readonly double[] sortedRadiusForVariableBalls;
        private double[] unsortedRadius;
        public readonly double[] allRadiusIncludingStartedForVariableProblem;

        private readonly int elementsInTranspositionPolyhedron; // 2^circleCount
        private readonly int elementsInJacLinPolyhedron; // OneCounter
        private readonly double[] arrayWithRightPart;
        private readonly string[] leftPatOfArrayInBinary;
        private double K1 = 1;
        private readonly double TAU = 0;
        public List<double[]> AllIteration { get; set; }

        public FixedAndVariableRadiusAdaptor(Data data, double[] radiusesWichWeMustGetAfterSolutionOfProblem)
        {
            AllIteration = new List<double[]>();
            VariableBALLSCount = data.VariableBALLSCount;
            ballsCount = data.ballCount; // set ball count
            _n = data.ballCount * 4 + 1;  // количество шаров переменного радиуса в векторе external

            sortedRadiusForVariableBalls = new double[data.VariableBALLSCount];
            unsortedRadius = new double[ballsCount];
            allRadiusIncludingStartedForVariableProblem = new double[ballsCount];

            for (int i = 0; i < ballsCount; i++)
            {
                allRadiusIncludingStartedForVariableProblem[i] = data.ball[i].R;
            }

            // записываем и потом отсортируем радиусы для метода с перестан. многоранником
            for (int i = 0; i < data.VariableBALLSCount; ++i)
            {
                sortedRadiusForVariableBalls[i] = radiusesWichWeMustGetAfterSolutionOfProblem[i];
            }
            double sumR = allRadiusIncludingStartedForVariableProblem.Sum();

            sortedRadiusForVariableBalls = sortedRadiusForVariableBalls.OrderBy(a => a).ToArray();

            RightAndLeftPartOfArrayForMethodUnfixedRadius(out leftPatOfArrayInBinary, out arrayWithRightPart, data, radiusesWichWeMustGetAfterSolutionOfProblem);
            X = new double[_n];
            try
            {
                if (data.ball != null && data != null && data.R >= 0)
                {
                    for (int i = 0; i < ballsCount; i++)
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

            /*    Ограничения переменных для переменного радиуса
            * *************************************************************************************/

            _x_L = new double[_n];
            _x_U = new double[_n];
            var diametrSum = 2 * sortedRadiusForVariableBalls.Sum();
            for (int i = 0; i < data.VariableBALLSCount; i++)
            {
                _x_L[4 * i] = _x_L[4 * i + 1] = _x_L[4 * i + 2] = -K1 * diametrSum + sortedRadiusForVariableBalls[i];
                _x_U[4 * i] = _x_U[4 * i + 1] = _x_U[4 * i + 2] = K1 * diametrSum - sortedRadiusForVariableBalls[i];
                _x_L[4 * i + 3] = sortedRadiusForVariableBalls[0];
                _x_U[4 * i + 3] = sortedRadiusForVariableBalls[data.VariableBALLSCount - 1];
            }

            // 
            for (int i = data.VariableBALLSCount; i < ballsCount; i++)
            {
                _x_L[4 * i] = _x_L[4 * i + 1] = _x_L[4 * i + 2] = -K1 * diametrSum + allRadiusIncludingStartedForVariableProblem[i];
                _x_U[4 * i] = _x_U[4 * i + 1] = _x_U[4 * i + 2] = K1 * diametrSum - allRadiusIncludingStartedForVariableProblem[i];
                _x_L[4 * i + 3] = allRadiusIncludingStartedForVariableProblem[i];
                _x_U[4 * i + 3] = allRadiusIncludingStartedForVariableProblem[i];
            }

            _x_L[_n - 1] = allRadiusIncludingStartedForVariableProblem[ballsCount - 1]; // От максимального радиуса
            _x_U[_n - 1] = sumR; // до суммы радиусов


            /*    Огрaничения
             **************************************************************************************/
            _nele_jac = 0;
            _m = 0;
            
            // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            _nele_jac += 5 * ballsCount; // x, y, z , r, R
            _m += ballsCount;

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]+r[j])^2 >=0
            _nele_jac += 2 * 4 * ballsCount * (ballsCount - 1) / 2;  // 2 * 3 два ограничения по 3 ненулевых частных производных
            _m += ballsCount * (ballsCount - 1) / 2;

            // перестановочный многогранник
            elementsInTranspositionPolyhedron = (int)Math.Pow(2, data.VariableBALLSCount);
            elementsInJacLinPolyhedron = OneCounter(leftPatOfArrayInBinary);
            _nele_jac += elementsInJacLinPolyhedron;
            _m += elementsInTranspositionPolyhedron;

            _g_L = new double[_m];
            _g_U = new double[_m];
            int op = 0;
            for (int j = 0; j < ballsCount; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }
            // Constraint for "Формирование условий попарного непересечания объектов"
            for (int i = 0; i < ballsCount - 1; i++)
            {
                for (int j = i + 1; j < ballsCount; j++)
                {
                    _g_L[op] = 0;
                    _g_U[op++] = Ipopt.PositiveInfinity;
                }
            }
            // Constraint for "перестановочного многоранника"
            // В правую часть помещаем константы, т.е. от констрант до +inf
            for (int i = 0; i < elementsInTranspositionPolyhedron; i++)
            {
                _g_L[op] = arrayWithRightPart[i];
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
            for (int i = 0; i < VariableBALLSCount; ++i)
                res++;
            return res;
        }

        private void RightAndLeftPartOfArrayForMethodUnfixedRadius(out string[] leftPart, out double[] rightPart, Data balls, double[] constantRadius)
        {
            int countOfelements = (int)Math.Pow(2, balls.VariableBALLSCount);
            double sum = 0;
            int ballCount = balls.VariableBALLSCount;
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
                        rightPart[i - 1] += constantRadius[k];
                }
            }
            foreach (var radius in constantRadius)
            {
                sum += Math.Pow(radius - TAU, 2);
            }
            rightPart[countOfelements - 1] = sum;
        }

        private void AddNewIteration(object element)
        {
            AllIteration.Add((double[])element);
        }

        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
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

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            for (var i = 0; i < _n - 1; i++)
            {
                grad_f[i] = 0;
            }
            grad_f[_n - 1] = 1;
            return true;
        }

        public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int kk = 0;
            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            // from 0 to count-1
            for (int i = 0; i < ballsCount; i++)
            {
                g[kk++] = Math.Pow((x[_n - 1] - x[4 * i + 3]), 2) //(R-r)^2
                         - x[4 * i] * x[4 * i]            // x^2
                         - x[4 * i + 1] * x[4 * i + 1]    // y^2
                         - x[4 * i + 2] * x[4 * i + 2];   // z^2
            };
            // kk = count
            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]+r[j])^2 >=0
            // from count to count*(count-1)/2 - 1
            for (int i = 0; i < ballsCount - 1; i++)
            {
                for (int j = i + 1; j < ballsCount; j++)
                {
                    g[kk++] = Math.Pow((x[4 * i] - x[4 * j]), 2.0)
                                  + Math.Pow((x[4 * i + 1] - x[4 * j + 1]), 2.0)
                                  + Math.Pow((x[4 * i + 2] - x[4 * j + 2]), 2.0)
                                  - Math.Pow((x[4 * i + 3] + x[4 * j + 3]), 2.0);
                }
            }
            double[] a = null;

            NewRadius(out a, x);
            for (int i = 0; i < elementsInTranspositionPolyhedron; i++)
            {
                g[kk++] = a[i];
            }
            return true;
        }

        // Transition polyhedron
        private void NewRadius(out double[] arrayWithNewUnfixedRadius, double[] x)
        {
            int countOfelements = (int)Math.Pow(2, VariableBALLSCount);
            string temp;
            arrayWithNewUnfixedRadius = new double[arrayWithRightPart.Length];
            double sum = 0.0;
            for (int i = 1; i < countOfelements; i++)
            {
                temp = leftPatOfArrayInBinary[i - 1];
                for (int k = 0; k < VariableBALLSCount; k++)
                {
                    if (temp[k] == '1')
                        arrayWithNewUnfixedRadius[i - 1] += x[4 * k + 3];
                }
            }
            //leftPatOfArrayInBinary[]
            for (int i = 0; i < VariableBALLSCount; i++)
                sum += Math.Pow(x[4 * i + 3] - TAU, 2);
            arrayWithNewUnfixedRadius[arrayWithRightPart.Length - 1] = sum;
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
                for (int i = 0; i < ballsCount - 1; ++i)
                {
                    for (int j = i + 1; j < ballsCount; ++j)
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
                for (int i = 0; i < VariableBALLSCount; i++)
                {
                    iRow[kk] = g - 1;
                    jCol[kk++] = 4 * i + 3;
                }
            }
            else
            {
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2  >= 0
                int kk = 0;
                for (int i = 0; i < ballsCount; i++)
                {
                    values[kk++] = 2 * (x[_n - 1] - x[4 * i + 3]); // R0'
                    values[kk++] = -2 * x[4 * i]; //X'
                    values[kk++] = -2 * x[4 * i + 1]; //Y'
                    values[kk++] = -2 * x[4 * i + 2]; //Z'
                    values[kk++] = -2 * (x[_n - 1] - x[4 * i + 3]); //r'
                }
                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i] + r[j])^2 >=0
                //  Console.WriteLine("---------------------------------------");
                for (int i = 0; i < ballsCount - 1; i++)
                {
                    for (int j = i + 1; j < ballsCount; j++)
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
                for (int i = 0; i < elementsInJacLinPolyhedron - VariableBALLSCount; i++)
                {
                    values[kk++] = 1;
                }
                for (int i = 0; i < VariableBALLSCount; i++)
                    values[kk++] = 2 * (x[4 * i + 3] - TAU);
            }
            return true;
        }

        public void Dispose()
        {
            _m = 0;
        }

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }
    }
}