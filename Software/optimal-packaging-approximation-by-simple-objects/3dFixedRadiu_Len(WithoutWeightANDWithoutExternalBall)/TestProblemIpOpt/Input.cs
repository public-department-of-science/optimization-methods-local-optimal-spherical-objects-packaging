/*Main for Fixed Radius 3d-optimization*/
// Message of ipopt Errors: https://www.coin-or.org/Ipopt/documentation/node36.html

using System;
using System.Linq;
using System.Threading;
using System.IO;

namespace hs071_cs
{
    public static class Input
    {
        private static Random rnd = new Random();
        private static int circle = 0;
        static PrintErrorMessageDel PrintError = new PrintErrorMessageDel(OutPut.ErrorMessage); // Print error and ErrorMessage

        // Methods (Properties) for access to private field circle
        public static int Circle { get => circle; set => circle = value; }

        // Generate Value for Extern Ball which include others less balls
        public static double[] RadiusRandomGenerate(double maxRandRadius, int cCount)
        {
            return ExternalRadius(ref maxRandRadius, cCount);
        }
        private static double[] ExternalRadius(ref double maxRandRadius, int cCount)
        {
            var arrR = new double[cCount];
            maxRandRadius--;
            for (int i = 0; i < cCount; ++i)
            {//arrR[i] = 1 + _rnd.NextDouble() * maxRandRadius; // 1..maxRadius
                arrR[i] = 2 + Math.Round(rnd.NextDouble() * maxRandRadius); // 1..maxRadius
                Thread.Sleep(14);
            }
            return arrR;
        }

        //генерирование произвольной матрицы связей
        public static void SetC(out double[,] matrixC)
        {
            matrixC = RandGenerateBondMatrix();
        }
        private static double[,] RandGenerateBondMatrix()
        {
            double[,] matrixC = new double[circle, circle];
            for (int i = 0; i < circle; i++)
            {
                for (int j = i; j < circle; j++)
                {
                    if (i == j)
                        continue;
                    matrixC[i, j] = rnd.Next(0, circle);
                    matrixC[j, i] = matrixC[i, j];
                }
            }
            for (int i = 0; i < circle; i++)
            {
                for (int j = 0; j < circle; j++)
                {
                    OutPut.Write(matrixC[i, j] + " ");
                }
                OutPut.WriteLine();
            }
            return matrixC;
        }

        // установка кол размещаемых объектов
        public static void SetCirclesCount(out int circlesCount)
        {
            circlesCount = CirclesCount();
        }
        private static int CirclesCount()
        {
            int circlesCount;
            try
            {
                OutPut.Write("CircleCount = ");
                circlesCount = Convert.ToInt32(Console.ReadLine()); // количество кругов
                if (circlesCount <= 0)
                    throw new Exception("Incorrect Value !!");
            }
            catch (Exception ex)
            {
                PrintError("\nInput Error --> " + ex.Message);
                circlesCount = 1;
            }
            return circlesCount;
        }

        //Type Reading data
        public static void ChooseTypeReadingData(out double[] xNach, out double[] yNach, out double[] zNach, out double[] rNach, out double RNach, out double maxRandRadius, out double[] rSortSum)
        {
            xNach = new double[circle];
            yNach = new double[circle];
            zNach = new double[circle];
            rNach = new double[circle];
            RNach = 0;
            maxRandRadius = 0;
            rSortSum = new double[circle];

            TypeOFReadingData(ref xNach, ref yNach, ref zNach, ref rNach, ref RNach, ref maxRandRadius, ref rSortSum);
        }
        private static void TypeOFReadingData(ref double[] xNach, ref double[] yNach, ref double[] zNach, ref double[] rNach, ref double RNach, ref double maxRandRadius, ref double[] rSortSum)
        {
            int keyCode;
            try
            {
                OutPut.Write("\nInput 1 or 2 ==>");
                keyCode = int.Parse(Console.ReadLine());
            }
            catch (Exception ex)
            {
                PrintError("\nInput Error --> {0}" + ex.Message);
                keyCode = 2;
            }
            switch (keyCode)
            {
                case 1:
                    new ReadResultFromFileDel(ReadFromFile)(out xNach, out yNach, out zNach, out rNach, out RNach, "ChangedCoordinate");
                    rSortSum = new RadiusSumGenerateDel(Input.raSumGenerate)(rNach); // отсортированные радиусы r[0]; r[0] + r[1];
                    break;

                case 2:
                    try
                    {
                        OutPut.Write("\nMaxRadius = ");
                        maxRandRadius = Convert.ToInt32(Console.ReadLine()); // количество кругов
                        if (maxRandRadius < 0)
                            throw new Exception("Value less 0!!");
                    }
                    catch (Exception ex)
                    {
                        PrintError("\nError -> {0}" + ex.Message);
                        maxRandRadius = 0;
                    }
                    rNach = new RadiusRandomGenerate(RadiusRandomGenerate)(maxRandRadius, circle); //"~~~ Генерирования случайными числами начальных радиусов ~~~
                    rSortSum = new RadiusSumGenerateDel(raSumGenerate)(rNach); // отсортированные радиусы r[0]; r[0] + r[1];
                    new XYZRGenerateDel(Input.xyzRRandomGenerateAvg)(circle, rNach, out xNach, out yNach, out zNach, out RNach); // генерируем начальные точки x,y,r,R
                    break;

                default:
                    break;
            }
        }

        // Генератор начальных r - радиусов внутренних кругов
        // r[i] = r[i] + rnd()*min{r}
        private static double[] rRandomGenerate(double[] radius)
        {
            return GenerateRadius(radius);
        }
        private static double[] GenerateRadius(double[] radius)
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
                arrR[i] = radius[i] + 0.5 * (rnd.NextDouble() - 0.5) * (maxR - minR);
                if (arrR[i] < minR) arrR[i] = minR;
                if (arrR[i] > maxR) arrR[i] = maxR;
            }
            return arrR;
        }

        // Reading from file whule end (x, y, z, r, R)
        public static void ReadFromFile(out double[] x, out double[] y, out double[] z, out double[] r, out double R, string fileName)
        {
            x = new double[Circle];
            y = new double[Circle];
            z = new double[Circle];
            r = new double[Circle];
            R = 0;
            ReadDataFromFile(ref x, ref y, ref z, ref r, ref R, fileName);
        }
        private static void ReadDataFromFile(ref double[] x, ref double[] y, ref double[] z, ref double[] r, ref double R, string fileName)
        {
            try
            {
                string readPath = @"D:\" + fileName + ".txt";
                FileInfo fi1 = new FileInfo(readPath);
                if (fi1.Exists)
                {
                    StreamReader sr = new StreamReader(readPath);
                    string[] s = new string[Circle + 1];
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
                    if (i > (Circle + 1) || i < (Circle + 1))
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

                r = RadiusRandomGenerate(20, Circle); //"~~~ Генерирования случайными числами начальных радиусов ~~~
                xyzRRandomGenerateAvg(Circle, r, out x, out y, out z, out R); // генерируем начальные точки x,y,r,R
                return;
            }
        }

        // Генератор начальных x and y and R
        // в диапазоне от -max(r[i]) до max(r[i])
        public static void xyzRRandomGenerateAvg(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R)
        {
            XYZRRandGenerateAVG(cCount, r, out x, out y, out z, out R);
        }
        private static void XYZRRandGenerateAVG(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R)
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
                x[i] = 10 * avgCircle * (rnd.NextDouble() - 0.5);
                y[i] = 10 * avgCircle * (rnd.NextDouble() - 0.5);
                z[i] = 10 * avgCircle * (rnd.NextDouble() - 0.5);

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
        public static double[] raSumGenerate(double[] radius)
        {
            return SummGenerate(ref radius);
        }
        private static double[] SummGenerate(ref double[] radius)
        {
            int cCount = radius.Length;
            var arrSumR = new double[cCount];
            radius = radius.OrderBy(a => a).ToArray();
            for (int i = 0; i < cCount; ++i)
                for (int k = 0; k <= i; ++k)
                    arrSumR[i] += radius[k];
            return arrSumR;
        }
    }
}