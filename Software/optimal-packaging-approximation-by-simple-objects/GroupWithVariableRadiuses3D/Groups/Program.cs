using System;
using System.Diagnostics;
using System.Linq;
using Cureos.Numerics;
using Groups;
using hs071_cs.ObjectOptimazation;

namespace hs071_cs
{
    public class Program
    {
        private static Random _rnd = new Random();
        private static int ballN;
        private static int _countVarR = 0; // количество кругов с переменным радиусом

        public static void Main()
        {
            //Timer tmr = new Timer(Tick, null, 1000, 1000);
            int ballsCount = 10; // количество кругов
            const double maxRandRadius = 30; // максимальный радиус кругов r = 1..maxRandRadius

            #region Инициализация и обявление переменных
            double[] rSortSum = null; // отсортированный массив радиусов, для ограничений
            int[] groups = new int[ballsCount];
            DataHelper dataHelper = new DataHelper();

            double[] xNach = new double[ballsCount];
            double[] yNach = new double[ballsCount];
            double[] zNach = new double[ballsCount];
            double[] rNach = new double[ballsCount];
            double RNach = 0.0;

            double[] rIter = new double[ballsCount];
            double[] xIter = new double[ballsCount];
            double[] yIter = new double[ballsCount];
            double[] zIter = new double[ballsCount];

            double[] rBest = new double[ballsCount];
            double[] xBest = new double[ballsCount];
            double[] yBest = new double[ballsCount];
            double[] zBest = new double[ballsCount];
            double RIter = 0.0;
            Stopwatch fullTaskTime = new Stopwatch();
            #endregion

            fullTaskTime.Start();
            ballN = ballsCount; // для использования вне Main (количество кругов)

            Console.WriteLine("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");

            string type = "";
            switch (type = Console.ReadLine())
            {
                case "1":
                    break;
                case "2":
                    break;
                default: return;
            }

            if (type == "1")
            {
                Input.ReadFromFile(ref xNach, ref yNach, ref zNach, ref rNach, ref RNach, ref ballsCount, "");
            }

            if (type == "2")
            {
                /* Генерирования случайными числами начальных радиусов
                 * *********************************************************************************/
                Console.WriteLine("~~~ Генерирования случайными числами начальных радиусов ~~~");
                Stopwatch stopWatch = new Stopwatch();
                rNach = rRandomGenerate(maxRandRadius, ballsCount);
                //rNach.OrderBy(a => a).ToArray();
                rSortSum = raSumGenerate(rNach); // отсортированные радиусы r[0]; r[0] + r[1]; ...
                                                 // генерируем начальные точки x,y,r,R
                xyRRandomGenerateAvg(ballsCount, ref rNach, ref xNach, ref yNach, ref zNach, ref RNach);
                Console.WriteLine("\n\t~~~ Генерируем точки с которых будем считать ~~~");
                for (int i = 0; i < ballsCount; ++i)
                {
                    xIter[i] = xNach[i];
                    yIter[i] = yNach[i];
                    zIter[i] = zNach[i];
                    rIter[i] = rNach[i];
                }
                RIter = RNach;
            }

            Console.WriteLine("=== Начальные значения ===");
            ShowData(xNach, yNach, zNach, rNach, RNach);
            Console.WriteLine("=== ================== ===");

            /* Решаем с фиксированными радиусами - начальная точка сгенерирована случайно
             * *********************************************************************************/
            Console.WriteLine("\n\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("\t ~~~ Решаем с фиксированными радиусами ~~~");
            Console.WriteLine("\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            #region FixedRadiuses

            double[] xyzFixR = new double[3 * ballsCount + 1];

            for (int i = 0; i < ballsCount; ++i)
            {
                xyzFixR[3 * i] = xNach[i];
                xyzFixR[3 * i + 1] = yNach[i];
                xyzFixR[3 * i + 2] = zNach[i];
            }

            xyzFixR[3 * ballsCount] = RNach;

            Stopwatch fixRTaskTime = new Stopwatch();
            Data startPointData = new Data(xNach, yNach, zNach, rNach, RNach, ballsCount, 0, TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
            OutPut.SaveToFile(startPointData, $"StartPoint");

            using (FixedRadius3dAdaptor adaptor = new FixedRadius3dAdaptor(startPointData))
            {
                fixRTaskTime.Start();
                RunTask(adaptor, xyzFixR, out xIter, out yIter, out zIter, ballsCount);
                fixRTaskTime.Stop();
                RIter = xyzFixR[3 * ballsCount];
                RNach = xyzFixR[3 * ballsCount];
            }

            startPointData = new Data(xIter, yIter, zIter, rNach, RIter, ballsCount, 0, TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
            OutPut.SaveToFile(startPointData, $"FixedRad{RNach}");

            Console.WriteLine("Выполенение задачи RunTime: " + getElapsedTime(fixRTaskTime));

            Console.WriteLine("=== Результат расчётов ===");
            ShowData(xIter, yIter, zIter, rNach, RIter);
            Console.WriteLine("=== ================== ===");

            double norma = Norma(xNach, xIter, yNach, yIter, zNach, zIter, rNach, rIter);

            Console.WriteLine($"Norma {norma}");

            #endregion

            Console.WriteLine("\n\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("\t\t ~~           Решаем с группами           ~~");
            Console.WriteLine("\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Balls[] balls = new Balls[ballsCount];

            for (int i = 0; i < ballsCount; ++i)
            {
                balls[i] = new Balls
                {
                    Group = 0
                };

                balls[i].Odz.xL = xIter[i] - maxRandRadius;
                balls[i].Odz.xU = xIter[i] + maxRandRadius;
                balls[i].Odz.yL = yIter[i] - maxRandRadius;
                balls[i].Odz.yU = yIter[i] + maxRandRadius;
                balls[i].Odz.zL = zIter[i] - maxRandRadius;
                balls[i].Odz.zU = zIter[i] + maxRandRadius;
                balls[i].Odz.rL = 0;
                balls[i].Odz.rU = rIter.Sum();
                balls[i].Radius = rIter[i] * new Random().NextDouble();
            }

            dataHelper.RandomizeCoordinate(ref balls, xIter, yIter, zIter, ballsCount);
            dataHelper.RandomizeRadiuses(ref balls, rIter, ballsCount);
            dataHelper.SetGroups(balls, ref _countVarR);

            IpoptReturnCode status;
            double[] radius = rNach.OrderBy(a => a).ToArray();

            Stopwatch varRTaskTime = new Stopwatch();
            using (VariableRadiusAdapter vr = new VariableRadiusAdapter(balls, radius))
            {
                varRTaskTime.Start();
                status = RunTask(vr, balls, out xIter, out yIter, out zIter, out rIter, out RIter);
                varRTaskTime.Stop();
            }
#if DEBUG
            ShowData(xIter, yIter, zIter, rIter, RIter);
#endif
            Data optionalPoint = new Data(xIter, yIter, zIter, rIter, RIter, ballsCount, holeCount: 0, taskClassification: TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
            OutPut.SaveToFile(optionalPoint, "VariableRadius"); // запись результата в файл

            Console.WriteLine("Выполенение задачи RunTime: " + getElapsedTime(varRTaskTime));

            fullTaskTime.Stop();
            Console.WriteLine("Выполенение всей задачи RunTime: " + getElapsedTime(fullTaskTime));
            Console.WriteLine("\n\n\n{0} ========= Press <RETURN> to exit... ========= ", Environment.NewLine);
            Console.ReadLine();
        }

        private static double Norma(double[] xNach, double[] xIter, double[] yNach, double[] yIter, double[] zNach, double[] zIter, double[] rNach, double[] rIter)
        {
            double norma = 0.0;
            for (int i = 0; i < xNach.Length; i++)
            {
                norma += Math.Pow(xNach[i] - xIter[i], 2);
                norma += Math.Pow(yNach[i] - yIter[i], 2);
                norma += Math.Pow(zNach[i] - zIter[i], 2);
            }

            return norma;
        }

        private static IpoptReturnCode RunTask(VariableRadiusAdapter op, Balls[] c, out double[] NewX, out double[] NewY, out double[] NewZ, out double[] NewR, out double CF)
        {
            Stopwatch timer = new Stopwatch();

            IpoptReturnCode status;
            double[] x = new double[op._n];
            timer.Start();
            /* allocate space for the initial point and set the values */

            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
            {
                /* Set some options.  The following ones are only examples,
                   they might not be suitable for your problem. */

                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-3);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("output_file", op.ToString() + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt");
                //problem.AddOption("file_print_level", 7); // 0..12
                problem.AddOption("file_print_level", 0);
                problem.AddOption("max_iter", 4000);
                problem.AddOption("print_level", 3); // 0<= value <= 12, default value is 5

                for (int i = 0; i < ballN; ++i)
                {
                    x[3 * i] = c[i].Coordinate.X;
                    x[3 * i + 1] = c[i].Coordinate.Y;
                    x[3 * i + 2] = c[i].Coordinate.Z;
                    if (i < _countVarR)
                    {
                        x[3 * ballN + i] = c[i].Radius;
                    }
                }
                status = problem.SolveProblem(x, out double obj, null, null, null, null);
                // CF = obj;
            }
            timer.Stop();
            //SaveToFile("d:\\out.txt", r, x, 10);
            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];
            NewR = new double[ballN];
            CF = x[x.Length - 1];
            for (int i = 0; i < ballN; ++i)
            {
                //Console.WriteLine("x[{0}]=    {1}", i, x[2 * i]);
                //Console.WriteLine("x[{0}]=    {1}", i, x[2 * i + 1]);
                //if(i<_countVarR) Console.WriteLine("x[{0}]=    {1}", i, x[2 * _count + i]);
                NewX[i] = x[3 * i];
                NewY[i] = x[3 * i + 1];
                NewZ[i] = x[3 * i + 2];
                NewR[i] = (i < _countVarR) ? x[3 * ballN + i]: c[i].Radius;
            }

            Console.WriteLine("{0}{0}Optimization return status: {1}{0}{0}", Environment.NewLine, status);
            return status;
        }

        private static void RunTask(FixedRadius3dAdaptor op, double[] xyz, out double[] NewX, out double[] NewY, out double[] NewZ, int ballN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.Eval_f, op.Eval_g, op.Eval_grad_f, op.Eval_jac_g, op.Eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-2);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 3000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                /* solve the problem */
                status = problem.SolveProblem(xyz, out double obj, null, null, null, null);
            }
            taskWatch.Stop();
            OutPut.ReturnCodeMessage("\nOptimization return status: " + status);

            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];

            for (int i = 0; i < ballN; i++)
            {
                NewX[i] = xyz[3 * i];
                NewY[i] = xyz[3 * i + 1];
                NewZ[i] = xyz[3 * i + 2];
            }
            OutPut.WriteLine("RunTime: " + OutPut.getElapsedTime(taskWatch));
        }

        private static string TimeToString(Stopwatch Watch)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = Watch.Elapsed;
            // Format and display the TimeSpan value.
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        /// <summary>
        /// Форматирует результат конвертирования времени запуска программы 
        /// </summary>
        /// <param name="Watch">объект Stopwatch</param>
        /// <returns>Строка- время чч:мм:сс.мс</returns>
        private static string getElapsedTime(Stopwatch Watch)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = Watch.Elapsed;
            // Format and display the TimeSpan value.

            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        /// <summary>
        /// Отобразить входные данные
        /// </summary>
        /// <param name="data">Данные передаваемы в решатель: переменные</param>
        /// <param name="selector">0 - в формате Карташова (по умолчанию)</param>
        /// <param name="selector">1 - в формате XYrR </param>
        private static void ShowData(double[] data, int selector = 0)
        {
            switch (selector)
            {
                case 1:
                    int cicleCount = (data.Length - 1) / 3;
                    Console.WriteLine("~~~~~~~~~~~ Координата Х ~~~~~~~~~~~");
                    for (int i = 0; i < cicleCount; ++i)
                    {
                        Console.WriteLine(" x[{0}]= {1}", i + 1, data[3 * i]);
                    }

                    Console.WriteLine("~~~~~~~~~~~ Координата Y ~~~~~~~~~~~");
                    for (int i = 0; i < cicleCount; ++i)
                    {
                        Console.WriteLine(" y[{0}]= {1}", i + 1, data[23 * i + 1]);
                    }

                    Console.WriteLine("~~~~~~~~~~~ Radius ~~~~~~~~~~~");
                    for (int i = 0; i < cicleCount; ++i)
                    {
                        Console.WriteLine(" r[{0}]= {1}", i + 1, data[3 * cicleCount + i]);
                    }

                    Console.WriteLine("\n R = {0}", data[4 * cicleCount]);
                    break;
                default:
                    for (int i = 0; i < data.Length; ++i)
                    {
                        Console.WriteLine(" x[{0}]= {1}", i + 1, data[i]);
                    }

                    break;
            }
        }

        /// <summary>
        /// Вывод значений в Формате Карташова
        /// </summary>
        /// <param name="data">Данные</param>
        private static void ShowData(double[] dataX, double[] dataY, double[] dataZ, double[] radius, double R)
        {
            int cicleCount = radius.Length;
            int iterCount = 1;
            for (int i = 0; i < cicleCount; ++i)
            {
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, dataX[i]);
                Console.WriteLine(" y[{0}]=  {1}", iterCount++, dataY[i]);
                Console.WriteLine(" z[{0}]=  {1}", iterCount++, dataZ[i]);
                Console.WriteLine(" r[{0}]=  {1}", iterCount++, radius[i]);
            }

            Console.WriteLine(" R_External[{0}]=  {1}", iterCount++, R);
        }

        /// <summary>
        /// Генератор начальных r - радиусов внутренних кругов
        /// в диапазоне от -max(r[i]) до max(r[i])
        /// </summary>
        public static double[] rRandomGenerate(double maxRandRadius, int cCount)
        {
            double[] arrR = new double[cCount];
            maxRandRadius--;
            for (int i = 0; i < cCount; ++i)
            {
                //arrR[i] = 1 + _rnd.NextDouble() * maxRandRadius; // 1..maxRadius
                arrR[i] = 2 + Math.Round(_rnd.NextDouble() * maxRandRadius); // 1..maxRadius
            }
            //arrR[i] = 1 + Math.Round(_rnd.NextDouble() * maxRandRadius); // 1..maxRadius
            //arrR[i] = Math.Pow(1 + i, 0.5);

            return arrR;
        }

        /// <summary>
        /// Генератор начальных x and y and R
        /// в диапазоне от -max(r[i]) до max(r[i])
        /// </summary>
        public static void xyRRandomGenerateAvg(int cCount, ref double[] r, ref double[] x, ref double[] y, ref double[] z, ref double R)
        {
            x = new double[cCount];
            y = new double[cCount];
            z = new double[cCount];
            double avgCircle = r.Average();
            double maxCircle = r.Max();
            // генеририруем R из массивов Х и У
            double maxX = 0;
            double maxY = 0;
            double maxZ = 0;
            double maxR = 0;
            double maxRXYZ = 0;
            for (int i = 0; i < cCount; ++i)
            {
                x[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);
                y[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);
                z[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);

                maxX = Math.Max(Math.Abs(x[i] + r[i]), Math.Abs(x[i] - r[i]));
                maxY = Math.Max(Math.Abs(y[i] + r[i]), Math.Abs(y[i] - r[i]));
                maxZ = Math.Max(Math.Abs(z[i] + r[i]), Math.Abs(z[i] - r[i]));
                maxR = Math.Max(Math.Max(maxX, maxY), maxZ);
                maxRXYZ = Math.Max(maxRXYZ, maxR);
            }
            R = maxRXYZ; // sumR;
        }

        /// <summary>
        /// Формирование массива радиусов для ограничений и задач оптимизаций
        /// r[i] = sum[1..i]{r[k]}
        /// </summary>
        public static double[] raSumGenerate(double[] radius)
        {
            int cCount = radius.Length;
            double[] arrSumR = new double[cCount];
            radius = radius.OrderBy(a => a).ToArray();
            for (int i = 0; i < cCount; ++i)
            {
                for (int k = 0; k <= i; ++k)
                {
                    arrSumR[i] += radius[k];
                }
            }

            return arrSumR;
        }
    }
}
