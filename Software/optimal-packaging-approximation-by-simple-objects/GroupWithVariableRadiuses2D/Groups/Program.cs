using Cureos.Numerics;
using hs071_cs.ObjectOptimazation;
using System;
using System.Diagnostics;
using System.Linq;

namespace hs071_cs
{
    public class ProgramScriptCycleProblemSept2017
    {
        private static readonly int _flagProcess = 1;
        private static Random _rnd = new Random();
        private static int circleN;
        private static int _countVarR; // количество кругов с переменным радиусом

        public static void Main()
        {
            //Timer tmr = new Timer(Tick, null, 1000, 1000);
            const int circlesCount = 10; // количество кругов
            const double maxRandRadius = 30; // максимальный радиус кругов r = 1..maxRandRadius
            #region Инициализация и обявление переменных
            double[] rSortSum = null; // отсортированный массив радиусов, для ограничений
            int[] groups = new int[circlesCount];

            double[] xNach = new double[circlesCount];
            double[] yNach = new double[circlesCount];
            double RNach;

            double[] rIter = new double[circlesCount];
            double[] xIter = new double[circlesCount];
            double[] yIter = new double[circlesCount];

            double[] rBest = new double[circlesCount];
            double[] xBest = new double[circlesCount];
            double[] yBest = new double[circlesCount];
            double RBest = maxRandRadius * circlesCount;

            Stopwatch fullTaskTime = new Stopwatch();
            #endregion
            fullTaskTime.Start();
            circleN = circlesCount; // для использования вне Main (количество кругов)
                                    /* Генерирования случайными числами начальных радиусов
                                     * *********************************************************************************/
            Console.WriteLine("~~~ Генерирования случайными числами начальных радиусов ~~~");
            Stopwatch stopWatch = new Stopwatch();
            double[] rNach = new double[circlesCount];
            rNach = rRandomGenerate(maxRandRadius, circlesCount);
            //rNach.OrderBy(a => a).ToArray();
            rSortSum = raSumGenerate(rNach); // отсортированные радиусы r[0]; r[0] + r[1]; ...
                                             // генерируем начальные точки x,y,r,R
            xyRRandomGenerateAvg(circlesCount, rNach, out xNach, out yNach, out RNach);
            Console.WriteLine("\n\t~~~ Генерируем точки с которых будем считать ~~~");
            for (int i = 0; i < circlesCount; ++i)
            {
                xIter[i] = xNach[i];
                yIter[i] = yNach[i];
                rIter[i] = rNach[i];
            }
            double RIter = RNach;
            Console.WriteLine("=== Начальные значения ===");
            ShowData(xNach, yNach, rNach, RNach);
            Console.WriteLine("=== ================== ===");

            /* Решаем с фиксированными радиусами - начальная точка сгенерирована случайно
             * *********************************************************************************/
            Console.WriteLine("\n\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("\t ~~~ Решаем с фиксированными радиусами ~~~");
            Console.WriteLine("\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            double[] xyFixR = new double[2 * circlesCount + 1];
            for (int i = 0; i < circlesCount; ++i)
            {
                xyFixR[2 * i] = xNach[i];
                xyFixR[2 * i + 1] = yNach[i];
            }
            xyFixR[2 * circlesCount] = RNach;
            Stopwatch fixRTaskTime = new Stopwatch();
            fixRTaskTime.Start();
            //using (var op7 = new OptimalPoints7(rNach, xNach, yNach, RNach))
            //{
            //    RunTask(op7, xyFixR, out xIter, out yIter);
            //    // Сохраняем результат рачсётов с фиксированными радиусами
            //    //for (int i = 0; i < circlesCount; ++i)
            //    //{
            //    //  // rNach - не изменились
            //    //  xIter[i] = xNach[i];
            //    //  yIter[i] = yNach[i];
            //    //}
            //    RNach = RIter = xyFixR[2 * circlesCount];
            //}
            fixRTaskTime.Stop();
            Console.WriteLine("Выполенение задачи RunTime: " + getElapsedTime(fixRTaskTime));

            Console.WriteLine("=== Результат расчётов ===");
            ShowData(xIter, yIter, rIter, RIter);
            Console.WriteLine("=== ================== ===");

            Console.WriteLine("\n\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("\t\t ~~           Решаем с группами           ~~");
            Console.WriteLine("\t\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Circle2D[] Circles = new Circle2D[circlesCount];
            for (int i = 0; i < circlesCount; ++i)
            {
                Circles[i] = new Circle2D();
                Circles[i].Group = 0;
                Circles[i].Odz.xL = xIter[i] - maxRandRadius;
                Circles[i].Odz.xU = xIter[i] + maxRandRadius;
                Circles[i].Odz.yL = yIter[i] - maxRandRadius;
                Circles[i].Odz.yU = yIter[i] + maxRandRadius;
                Circles[i].Odz.rL = -rIter.Sum();
                Circles[i].Odz.rU = rIter.Sum();
                Circles[i].Coordinate.X = xIter[i];
                Circles[i].Coordinate.Y = yIter[i];
                Circles[i].Radius = rNach[i];
            }

            IpoptReturnCode status;
            double[] radius = rNach.OrderBy(a => a).ToArray();
            _countVarR = 0;
            for (int i = 0; i < circlesCount; ++i)
            {
                //20фикс + 10п;
                //if (Circles[i].Radius < radius[10])
                //{
                //    Circles[i].Group = 0;
                //}

                if (i < 7)
                {
                    Circles[i].Group = 1;
                }

                if (i >= 7)
                {
                    Circles[i].Group = 2;
                }
            }
            Stopwatch varRTaskTime = new Stopwatch();
            varRTaskTime.Start();
            using (VariableRadiusPolySpheraAdapter vr = new VariableRadiusPolySpheraAdapter(Circles, radius))
            {
                status = RunTask(vr, Circles, out xIter, out yIter, out rIter, out RIter);
            }
#if DEBUG
            ShowData(xIter, yIter, radius, RIter);
#endif
            varRTaskTime.Stop();
            Console.WriteLine("Выполенение задачи RunTime: " + getElapsedTime(varRTaskTime));

            fullTaskTime.Stop();
            Console.WriteLine("Выполенение всей задачи RunTime: " + getElapsedTime(fullTaskTime));
            Console.WriteLine("\n\n\n{0} ========= Press <RETURN> to exit... ========= ", Environment.NewLine);
            Console.ReadLine();
        }

        private static IpoptReturnCode RunTask(VariableRadiusPolySpheraAdapter op, Circle2D[] c, out double[] NewX, out double[] NewY, out double[] NewR, out double CF)
        {
            Stopwatch timer = new Stopwatch();
            IpoptReturnCode status;
            double[] x = new double[op._n];
            timer.Reset();
            timer.Start();
            /* allocate space for the initial point and set the values */

            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
            {
                /* Set some options.  The following ones are only examples,
                   they might not be suitable for your problem. */

                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-7);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("output_file", op.ToString() + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt");
                problem.AddOption("print_frequency_iter", 20);
                //problem.AddOption("file_print_level", 7); // 0..12
                problem.AddOption("file_print_level", 8);
                problem.AddOption("max_iter", 100000);
                problem.AddOption("print_level", 0); // 0<= value <= 12, default value is 5
#if INTERMEDIATE
        problem.SetIntermediateCallback(p.intermediate);
#endif
                /* solve the problem */
                //Input: Starting point; Output: Optimal solution
                for (int i = 0; i < circleN; ++i)
                {
                    x[2 * i] = c[i].Coordinate.X;
                    x[2 * i + 1] = c[i].Coordinate.Y;
                    if (i < _countVarR)
                    {
                        x[2 * circleN + i] = c[i].Radius;
                    }
                }
                double obj;
                status = problem.SolveProblem(x, out obj, null, null, null, null);
            }
            timer.Stop();
            Console.WriteLine("{0}{0}Optimization return status: {1}{0}{0}", Environment.NewLine, status);
            //SaveToFile("d:\\out.txt", r, x, 10);
            NewX = new double[circleN];
            NewY = new double[circleN];
            NewR = new double[circleN];
            CF = x[x.Length - 1];
            for (int i = 0; i < circleN; ++i)
            {
                //Console.WriteLine("x[{0}]=    {1}", i, x[2 * i]);
                //Console.WriteLine("x[{0}]=    {1}", i, x[2 * i + 1]);
                //if(i<_countVarR) Console.WriteLine("x[{0}]=    {1}", i, x[2 * _count + i]);
                NewX[i] = x[2 * i];
                NewY[i] = x[2 * i + 1];
                NewR[i] = (i >= _countVarR) ? c[i].Radius : x[2 * circleN + i];
            }
            for (int i = 0; i < x.Length; ++i)
            {
                Console.WriteLine("x[{0}]=    {1}", i, x[i]);
            }

            Console.WriteLine("===> RunTime: " + TimeToString(timer));
            return status;
        }
        private static string TimeToString(Stopwatch Watch)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = Watch.Elapsed;
            // Format and display the TimeSpan value.
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

//        private static void RunTask(OptimalPoints7 op, double[] xy, out double[] NewX, out double[] NewY)
//        {
//            Stopwatch taskWatch = new Stopwatch();
//            IpoptReturnCode status;
//            taskWatch.Start();
//            /* allocate space for the initial point and set the values */
//            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
//            {
//                /* Set some options.  The following ones are only examples,
//                   they might not be suitable for your problem. */
//                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
//                problem.AddOption("tol", 1e-7);
//                problem.AddOption("mu_strategy", "adaptive");
//                problem.AddOption("hessian_approximation", "limited-memory");
//                problem.AddOption("output_file", op.ToString() + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt");
//                //problem.AddOption("print_frequency_iter", 20);
//                //problem.AddOption("file_print_level", 7); // 0..12
//                problem.AddOption("file_print_level", 1);
//                problem.AddOption("max_iter", 100000);
//                problem.AddOption("print_level", 1); // 0<= value <= 12, default value is 5
//#if INTERMEDIATE
//                problem.SetIntermediateCallback(p.intermediate);
//#endif
//                /* solve the problem */
//                double obj;
//                status = problem.SolveProblem(xy, out obj, null, null, null, null);
//            }
//            taskWatch.Stop();
//            Console.WriteLine("{0}{0}Optimization return status: {1}{0}{0}", Environment.NewLine, status);
//            //SaveToFile("d:\\out.txt", r, x, 10);
//            NewX = new double[circleN];
//            NewY = new double[circleN];
//            for (int i = 0; i < circleN; ++i)
//            {
//                //Console.WriteLine("x[{0}]=  {1}", i, x[i]);
//                NewX[i] = xy[2 * i];
//                NewY[i] = xy[2 * i + 1];
//            }
//            //ShowData(x); // форматный вывод результата
//            Console.WriteLine("RunTime: " + getElapsedTime(taskWatch));
//        }

//        private static void RunTask(OptimalPoints4 op, double[] x, out double[] NewX)
//        {
//            Stopwatch stopWatch = new Stopwatch();
//            IpoptReturnCode status;

//            stopWatch.Reset();
//            stopWatch.Start();
//            /* allocate space for the initial point and set the values */
//            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
//            {
//                /* Set some options.  The following ones are only examples,
//                   they might not be suitable for your problem. */
//                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
//                problem.AddOption("tol", 1e-7);
//                problem.AddOption("mu_strategy", "adaptive");
//                problem.AddOption("hessian_approximation", "limited-memory");
//                problem.AddOption("output_file", op.ToString() + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt");
//                //problem.AddOption("print_frequency_iter", 20);
//                //problem.AddOption("file_print_level", 7); // 0..12
//                problem.AddOption("file_print_level", 1);
//                problem.AddOption("max_iter", 100000);
//                problem.AddOption("print_level", 1); // 0<= value <= 12, default value is 5
//#if INTERMEDIATE
//                problem.SetIntermediateCallback(p.intermediate);
//#endif
//                /* solve the problem */
//                double obj;
//                status = problem.SolveProblem(x, out obj, null, null, null, null);
//            }
//            stopWatch.Stop();
//            Console.WriteLine("{0}{0}Optimization return status: {1}{0}{0}", Environment.NewLine, status);
//            //SaveToFile("d:\\out.txt", r, x, 10);
//            NewX = new double[x.Length];
//            for (int i = 0; i < x.Length; ++i)
//            {
//                //Console.WriteLine("x[{0}]=  {1}", i, x[i]);
//                NewX[i] = x[i];
//            }
//            //ShowData(x); // форматный вывод результата
//            Console.WriteLine("RunTime: " + getElapsedTime(stopWatch));
//        }

//        private static void RunTask(OptimalPoints7 op, double[] x, out double[] NewX)
//        {
//            Stopwatch stopWatch = new Stopwatch();
//            IpoptReturnCode status;
//            stopWatch.Start();
//            /* allocate space for the initial point and set the values */
//            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
//            {
//                /* Set some options.  The following ones are only examples,
//                   they might not be suitable for your problem. */
//                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
//                problem.AddOption("tol", 1e-7);
//                problem.AddOption("mu_strategy", "adaptive");
//                problem.AddOption("hessian_approximation", "limited-memory");
//                problem.AddOption("output_file", op.ToString() + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt");
//                //problem.AddOption("print_frequency_iter", 20);
//                //problem.AddOption("file_print_level", 7); // 0..12
//                problem.AddOption("file_print_level", 1);
//                problem.AddOption("max_iter", 100000);
//                problem.AddOption("print_level", 1); // 0<= value <= 12, default value is 5
//#if INTERMEDIATE
//                problem.SetIntermediateCallback(p.intermediate);
//#endif
//                /* solve the problem */
//                double obj;
//                status = problem.SolveProblem(x, out obj, null, null, null, null);
//            }
//            stopWatch.Stop();
//            Console.WriteLine("{0}{0}Optimization return status: {1}{0}{0}", Environment.NewLine, status);
//            //SaveToFile("d:\\out.txt", r, x, 10);
//            NewX = new double[x.Length];
//            for (int i = 0; i < x.Length; ++i)
//            {
//                //Console.WriteLine("x[{0}]=  {1}", i, x[i]);
//                NewX[i] = x[i];
//            }
//            Console.WriteLine("RunTime: " + getElapsedTime(stopWatch));
//        }

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

            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
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
                        Console.WriteLine(" x[{0}]= {1}", i + 1, data[2 * i]);
                    }

                    Console.WriteLine("~~~~~~~~~~~ Координата Y ~~~~~~~~~~~");
                    for (int i = 0; i < cicleCount; ++i)
                    {
                        Console.WriteLine(" y[{0}]= {1}", i + 1, data[2 * i + 1]);
                    }

                    Console.WriteLine("~~~~~~~~~~~ Radius ~~~~~~~~~~~");
                    for (int i = 0; i < cicleCount; ++i)
                    {
                        Console.WriteLine(" r[{0}]= {1}", i + 1, data[2 * cicleCount + i]);
                    }

                    Console.WriteLine("\n R = {0}", data[3 * cicleCount]);
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
        private static void ShowData(double[] dataX, double[] dataY, double[] radius, double R)
        {
            int cicleCount = radius.Length;
            int iterCount = 1;
            Console.WriteLine("~~~~~~~~~~~ Формат Карташова ~~~~~~~~~~~");
            for (int i = 0; i < cicleCount; ++i)
            {
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, dataX[i]);
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, dataY[i]);
            }
            for (int i = 0; i < cicleCount; ++i)
            {
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, radius[i]);
            }

            Console.WriteLine(" x[{0}]=  {1}", iterCount++, R);
        }

        /// <summary>
        /// Для задачи с фиксированным радиусами
        /// </summary>
        /// <param name="data">Данные передаваемы в решатель: переменные</param>
        /// <param name="radius">Константные радиусы</param>
        private static void ShowData(double[] data, double[] radius)
        {
            int cicleCount = radius.Length;
            Console.WriteLine("~~~~~~~~~~~ Координата Х ~~~~~~~~~~~");
            for (int i = 0; i < cicleCount; ++i)
            {
                Console.WriteLine(" x[{0}]= {1}", i + 1, data[2 * i]);
            }

            Console.WriteLine("~~~~~~~~~~~ Координата Y ~~~~~~~~~~~");
            for (int i = 0; i < cicleCount; ++i)
            {
                Console.WriteLine(" y[{0}]= {1}", i + 1, data[2 * i + 1]);
            }

            Console.WriteLine("~~~~~~~~~~~ Radius ~~~~~~~~~~~");
            for (int i = 0; i < cicleCount; ++i)
            {
                Console.WriteLine(" r[{0}]= {1}", i + 1, radius[i]);
            }

            Console.WriteLine("\n R = {0}", data[2 * cicleCount]);
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
        /// Генератор начальных r - радиусов внутренних кругов
        /// r[i] = r[i] + rnd()*min{r}
        /// </summary>
        public static double[] rRandomGenerate(double[] radiuses)
        {
            int cCount = radiuses.Length;
            double minR = radiuses[0];
            double maxR = radiuses[0];
            for (int i = 1; i < cCount; ++i)
            {
                if (minR > radiuses[i])
                {
                    minR = radiuses[i];
                }

                if (maxR < radiuses[i])
                {
                    maxR = radiuses[i];
                }
            }
            double[] arrR = new double[cCount];
            for (int i = 0; i < cCount; ++i)
            {
                //arrR[i] = radiuses[i] + (_rnd.NextDouble() - 0.5) * minR;
                arrR[i] = radiuses[i] + 0.5 * (_rnd.NextDouble() - 0.5) * (maxR - minR);
                if (arrR[i] < minR)
                {
                    arrR[i] = minR;
                }

                if (arrR[i] > maxR)
                {
                    arrR[i] = maxR;
                }
            }


            return arrR;
        }
        /// <summary>
        /// Генератор начальных x and y and R
        /// в диапазоне от -max(r[i]) до max(r[i])
        /// </summary>
        public static void xyRRandomGenerate(int cCount, double[] r, out double[] x, out double[] y, out double R)
        {
            x = new double[cCount];
            y = new double[cCount];
            // генеририруем R из массивов Х и У
            double maxX = 0;
            double maxY = 0;
            double maxR = 0;
            double maxRXY = 0;
            for (int i = 0; i < cCount; ++i)
            {
                x[i] = r[cCount - 1] * (_rnd.NextDouble() - 0.5);
                y[i] = r[cCount - 1] * (_rnd.NextDouble() - 0.5);

                maxX = Math.Max(Math.Abs(x[i] + r[i]), Math.Abs(x[i] - r[i]));
                maxY = Math.Max(Math.Abs(y[i] + r[i]), Math.Abs(y[i] - r[i]));
                maxR = Math.Max(maxX, maxY);
                maxRXY = Math.Max(maxRXY, maxR);
            }
            R = maxRXY; // sumR;
        }
        /// <summary>
        /// Генератор начальных x and y and R
        /// в диапазоне от -max(r[i]) до max(r[i])
        /// </summary>
        public static void xyRRandomGenerateAvg(int cCount, double[] r, out double[] x, out double[] y, out double R)
        {
            x = new double[cCount];
            y = new double[cCount];
            double avgCircle = r.Average();
            double maxCircle = r.Max();
            // генеририруем R из массивов Х и У
            double maxX = 0;
            double maxY = 0;
            double maxR = 0;
            double maxRXY = 0;
            for (int i = 0; i < cCount; ++i)
            {
                x[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);
                y[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);

                maxX = Math.Max(Math.Abs(x[i] + r[i]), Math.Abs(x[i] - r[i]));
                maxY = Math.Max(Math.Abs(y[i] + r[i]), Math.Abs(y[i] - r[i]));
                maxR = Math.Max(maxX, maxY);
                maxRXY = Math.Max(maxRXY, maxR);
            }
            R = maxRXY; // sumR;
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
