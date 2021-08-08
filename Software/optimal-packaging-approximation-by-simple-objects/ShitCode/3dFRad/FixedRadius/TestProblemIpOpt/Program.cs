/*
        Main for Fixed Radius 3d-optimization
 */

// Сообщения об ошибках ipopt: https://www.coin-or.org/Ipopt/documentation/node36.html
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.IO;
using Cureos.Numerics;

namespace hs071_cs
{
    public class Program
    {
        private static Random _rnd = new Random();
        private static int circleN;

        public static void Main()
        {
            #region Инициализация и обявление переменных
            int circlesCount = 0; // кол. шаров
            double maxRandRadius = 0; // максимальный радиус кругов r = 1..maxRandRadius

            try
            {
                Console.Write("BallsCount = ");
                circlesCount = Convert.ToInt32(Console.ReadLine()); // количество кругов
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Input Error --> {0}", ex.Message);
                Console.ForegroundColor = ConsoleColor.Black;
                circlesCount = 1;
            }

            circleN = circlesCount; // для использования вне Main (количество кругов)
            double[] rSortSum = null; // отсортированный массив радиусов, для ограничений
            var groups = new int[circlesCount];

            var xNach = new double[circlesCount];
            var yNach = new double[circlesCount];
            var zNach = new double[circlesCount];
            var rIter = new double[circlesCount];
            var xIter = new double[circlesCount];
            var yIter = new double[circlesCount];
            var zIter = new double[circlesCount];

            var rNach = new double[circlesCount];
            double RNach = 0;

            #region Reading Data
            Console.WriteLine("Select input method \n 1 --> Read from File \n 2 --> Random generate");
            int keyCode = 0;
            try
            {
                Console.Write("Input 1 or 2 ==>");
                keyCode = int.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Input Error --> {0}", ex.Message);
                Console.ForegroundColor = ConsoleColor.Black;
                keyCode = 2;
            }

            switch (keyCode)
            {
                case 1:
                    ReadFromFile(out xNach, out yNach, out zNach, out rNach, out RNach, "ChangedCoordinate");
                    rSortSum = raSumGenerate(rNach); // отсортированные радиусы r[0]; r[0] + r[1];
                    break;
                case 2:
                    try
                    {
                        Console.Write("MaxRadius = ");
                        maxRandRadius = Convert.ToInt32(Console.ReadLine()); // количество кругов
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error -> {0}", ex.Message);
                        Console.ForegroundColor = ConsoleColor.Black;
                        maxRandRadius = 0;
                    }
                    rNach = rRandomGenerate(maxRandRadius, circlesCount); //"~~~ Генерирования случайными числами начальных радиусов ~~~
                    rSortSum = raSumGenerate(rNach); // отсортированные радиусы r[0]; r[0] + r[1];
                    xyzRRandomGenerateAvg(circlesCount, rNach, out xNach, out yNach, out zNach, out RNach); // генерируем начальные точки x,y,r,R
                    break;
                default:
                    break;
            }
            #endregion

            for (int i = 0; i < circlesCount; i++)
            {
                xIter[i] = xNach[i];
                yIter[i] = yNach[i];
                zIter[i] = zNach[i];
                rIter[i] = rNach[i];
            }
            double RIter = RNach;
            Console.WriteLine("=== Начальные значения ===");
            #endregion

            Stopwatch fullTaskTime = new Stopwatch();
            fullTaskTime.Start();

            ShowData(xNach, yNach, zNach, rNach, RNach);
            SaveToFile(xNach, yNach, zNach, rNach, RNach, "StartPoint");
            Console.WriteLine("=== ================== ===");

            /* Решаем с фиксированными радиусами - начальная точка сгенерирована случайно
             * *********************************************************************************/
            Console.WriteLine("\n\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("\t\t ~~~ Solve problem ~~~");
            Console.WriteLine("\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            #region Формирование xyzR массива  
            double[] xyzFixR = new double[3 * circlesCount + 1];

            int countXYZR = 0,
                countCoordinate = 0;
            while (countCoordinate < circlesCount)
            {
                xyzFixR[countXYZR] = xNach[countCoordinate];
                xyzFixR[++countXYZR] = yNach[countCoordinate];
                xyzFixR[++countXYZR] = zNach[countCoordinate];
                countCoordinate++;
                countXYZR++;
            }

            xyzFixR[3 * circlesCount] = RNach;
            #endregion
            Stopwatch fixRTaskTime = new Stopwatch();
            fixRTaskTime.Start();// Start time
            using (var adaptor = new FixedRadius3dAdaptor(rNach, xNach, yNach, zNach, RNach))
            {
                RunTask(adaptor, xyzFixR, out xIter, out yIter, out zIter);
                RNach = RIter = xyzFixR[3 * circlesCount];
                rNach = adaptor.radius;
            }
            fixRTaskTime.Stop(); // Stop time
            Console.WriteLine("Выполенение задачи RunTime: " + getElapsedTime(fixRTaskTime));

            Console.WriteLine("=== Результат расчётов ===");
            ShowData(xIter, yIter, zIter, rNach, RIter);
            SaveToFile(xIter, yIter, zIter, rNach, RIter, "Coordinate"); // запись результата в файл

            fullTaskTime.Stop();
            Console.WriteLine("Выполенение всей задачи RunTime: " + getElapsedTime(fullTaskTime));
            Console.WriteLine("Quality of solution = {0}", Density(rNach, RNach));
            Console.WriteLine("========= Press <RETURN> to exit... ========= ");
            Console.ReadLine();
        }

        private static string TimeToString(Stopwatch Watch)
        {
            TimeSpan ts = Watch.Elapsed;
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        static void RunTask(FixedRadius3dAdaptor op, double[] xyz, out double[] NewX, out double[] NewY, out double[] NewZ)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-4);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("output_file", op.ToString() + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + ".txt");
                problem.AddOption("file_print_level", 3);//1-12
                problem.AddOption("max_iter", 10000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                /* solve the problem */
                double obj;
                status = problem.SolveProblem(xyz, out obj, null, null, null, null);
            }
            taskWatch.Stop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0}{0}Optimization return status: {1}{0}{0}", Environment.NewLine, status);
            Console.ForegroundColor = ConsoleColor.Black;
            NewX = new double[circleN];
            NewY = new double[circleN];
            NewZ = new double[circleN];

            int countXYZR = 0,
            countCoordinate = 0;
            while (countCoordinate < circleN)
            {
                NewX[countCoordinate] = xyz[countXYZR];
                NewY[countCoordinate] = xyz[++countXYZR];
                NewZ[countCoordinate] = xyz[++countXYZR];

                countCoordinate++;
                countXYZR++;
            }
            Console.WriteLine("RunTime: " + getElapsedTime(taskWatch));
        }

        // Форматирует результат конвертирования времени запуска программы 
        static string getElapsedTime(Stopwatch Watch)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = Watch.Elapsed;
            // Format and display the TimeSpan value.

            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        static void ShowData(double[] dataX, double[] dataY, double[] dataZ, double[] radius, double R)
        {
            int cicleCount = radius.Length;
            int iterCount = 1;
            Console.WriteLine("~~~~~~~~~~~ ~~~~~~~~~~~");
            for (int i = 0; i < cicleCount; ++i)
            {
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, dataX[i]);
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, dataY[i]);
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, dataZ[i]);
            }
            for (int i = 0; i < cicleCount; ++i)
                Console.WriteLine(" x[{0}]=  {1}", iterCount++, radius[i]);
            Console.WriteLine(" x[{0}]=  {1}", iterCount++, R);
        }

        static double[] rRandomGenerate(double maxRandRadius, int cCount)
        {
            var arrR = new double[cCount];
            maxRandRadius--;
            for (int i = 0; i < cCount; ++i)
            {//arrR[i] = 1 + _rnd.NextDouble() * maxRandRadius; // 1..maxRadius
                arrR[i] = 2 + Math.Round(_rnd.NextDouble() * maxRandRadius); // 1..maxRadius
                Thread.Sleep(14);
            }
            return arrR;
        }

        // Генератор начальных r - радиусов внутренних кругов
        // r[i] = r[i] + rnd()*min{r}
        static double[] rRandomGenerate(double[] radius)
        {
            int cCount = radius.Length;
            double minR = radius[0];
            double maxR = radius[0];
            for (int i = 1; i < cCount; ++i)
            {
                if (minR > radius[i])
                    minR = radius[i];
                if (maxR < radius[i])
                    maxR = radius[i];
            }
            var arrR = new double[cCount];
            for (int i = 0; i < cCount; ++i)
            {
                //arrR[i] = radiuses[i] + (_rnd.NextDouble() - 0.5) * minR;
                arrR[i] = radius[i] + 0.5 * (_rnd.NextDouble() - 0.5) * (maxR - minR);
                if (arrR[i] < minR) arrR[i] = minR;
                if (arrR[i] > maxR) arrR[i] = maxR;
            }
            return arrR;
        }

        static void SaveToFile(double[] x, double[] y, double[] z, double[] r, double R, string fileName)
        {
            try
            {
                string writePath = @"D:\" + fileName + ".txt";
                FileInfo fi = new FileInfo(writePath);
                if (fi.Exists)
                {
                    fi.Delete();
                }
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    sw.Write(R.ToString().Replace(',', '.'));
                    sw.WriteLine();
                    for (int i = 0; i < x.Length; i++)
                    {
                        sw.Write(x[i].ToString().Replace(',', '.') + " ");
                        sw.Write(y[i].ToString().Replace(',', '.') + " ");
                        sw.Write(z[i].ToString().Replace(',', '.') + " ");
                        sw.Write(r[i].ToString().Replace(',', '.'));
                        sw.WriteLine();
                    }
                    sw.Close();
                    Console.WriteLine("All Data has been written to {0}!", writePath);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Writing Error!! --> {0}", ex.Message);
                Console.ForegroundColor = ConsoleColor.Black;
            }
        }

        static void ReadFromFile(out double[] x, out double[] y, out double[] z, out double[] r, out double R, string fileName)
        {
            x = new double[circleN];
            y = new double[circleN];
            z = new double[circleN];
            r = new double[circleN];
            R = 0;
            try
            {
                string readPath = @"D:\" + fileName + ".txt";
                FileInfo fi1 = new FileInfo(readPath);
                if (fi1.Exists)
                {
                    StreamReader sr = new StreamReader(readPath);
                    string[] s = new string[circleN + 1];
                    string[] temp = new string[4];
                    string str = "";
                    int i = 0;

                    while (str != null)
                    {
                        str = sr.ReadLine();
                        if (str != null)
                        {
                            s[i] = str;
                            i++;
                        }
                    }
                    // check demention
                    if (i > (circleN + 1) || i < (circleN + 1))
                    {
                        throw new Exception("Dimension do not match!");
                    }
                    else
                    {
                        R = Convert.ToDouble(s[0].Split(' ')[3].Replace('.', ',').Trim()); // external radius
                        for (i = 1; i < s.Length; i++)
                        {
                            temp = s[i].Split(' ');
                            for (int k = 0; k < temp.Length; k++)
                            {
                                temp[k] = temp[k].Replace('.', ',').Trim();
                            }
                            x[i - 1] = Convert.ToDouble(temp[0]);
                            y[i - 1] = Convert.ToDouble(temp[1]);
                            z[i - 1] = Convert.ToDouble(temp[2]);
                            r[i - 1] = Convert.ToDouble(temp[3]);
                        }
                        Console.WriteLine("Data has been read!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Data not readed! Error --> {0}", ex.Message);
                Console.WriteLine("We generate random coordinate from 0 to 30");
                Console.ForegroundColor = ConsoleColor.Black;

                r = rRandomGenerate(20, circleN); //"~~~ Генерирования случайными числами начальных радиусов ~~~
                xyzRRandomGenerateAvg(circleN, r, out x, out y, out z, out R); // генерируем начальные точки x,y,r,R
                return;
            }

        }

        // Генератор начальных x and y and R
        // в диапазоне от -max(r[i]) до max(r[i])
        static void xyzRRandomGenerateAvg(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R)
        {
            x = new double[cCount];
            y = new double[cCount];
            z = new double[cCount];

            var avgCircle = r.Average();
            var maxCircle = r.Max();
            // генеририруем R из массивов Х и У
            double maxX = 0;
            double maxY = 0;
            double maxZ = 0;
            double maxR = 0;
            double maxRXYZ = 0;
            for (int i = 0; i < cCount; i++)
            {
                x[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);
                y[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);
                z[i] = 10 * avgCircle * (_rnd.NextDouble() - 0.5);

                maxX = Math.Max(Math.Abs(x[i] + r[i]), Math.Abs(x[i] - r[i]));
                maxY = Math.Max(Math.Abs(y[i] + r[i]), Math.Abs(y[i] - r[i]));
                maxZ = Math.Max(Math.Abs(z[i] + r[i]), Math.Abs(z[i] - r[i]));

                maxR = Math.Max(maxX, maxY);
                maxR = Math.Max(maxR, maxZ);

                maxRXYZ = Math.Max(maxRXYZ, maxR);
            }
            R = maxRXYZ; // sumR;
        }

        // Формирование массива радиусов для ограничений и задач оптимизаций
        // r[i] = sum[1..i]{r[k]}
        static double[] raSumGenerate(double[] radius)
        {
            int cCount = radius.Length;
            var arrSumR = new double[cCount];
            radius = radius.OrderBy(a => a).ToArray();
            for (int i = 0; i < cCount; ++i)
                for (int k = 0; k <= i; ++k)
                    arrSumR[i] += radius[k];
            return arrSumR;
        }

        //объемное заполнение внешнего шара
        private static double Density(double[] r, double R)
        {
            double totalCapacity = (4.0 / 3.0) * Math.PI * Math.Pow(R, 3.0),
                   realCapacity = 0;

            for (int i = 0; i < r.Length; i++)
            {
                realCapacity += (4.0 / 3.0) * Math.PI * Math.Pow(r[i], 3.0);
            }
            return realCapacity / totalCapacity;
        }
    }
}