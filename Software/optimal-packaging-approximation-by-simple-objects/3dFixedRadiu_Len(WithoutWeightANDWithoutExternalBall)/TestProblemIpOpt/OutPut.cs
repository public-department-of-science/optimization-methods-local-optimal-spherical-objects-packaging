/*Main for Fixed Radius 3d-optimization*/
// Message of ipopt Errors: https://www.coin-or.org/Ipopt/documentation/node36.html

using System;
using System.Diagnostics;
using System.IO;

namespace hs071_cs
{
    public static class OutPut
    {
        #region Console Interface
        public static void Write(string str = "")
        {
            Console.Write(str);
        }
        public static void WriteLine(string str = "") // writeLine For array with index
        {
            Console.WriteLine("{0}", str);
        }
        public static void WriteLine(params string[] a)
        {
            foreach (string item in a)
            {
                Write(item + " ");
            }
        }
        public static void WriteLine(string str, int index, double value) // writeLine For array with index
        {
            Console.WriteLine(str + "[{0}] = {1}", index, value);
        }
        public static void ErrorMessage(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Black;
        }
        public static void ReturnCodeMessage(string str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(str);
            Console.ForegroundColor = ConsoleColor.Black;
        }
        public static void ShowData(Data data)
        {
            int cicleCount = data.circleCount;
            int iterCount = 1;
            WriteLine("\n~~~~~~~~~~~ Формат Карташова ~~~~~~~~~~~");
            for (int i = 0; i < cicleCount; i++)
            {
                WriteLine("x", iterCount++, data.ball[i].X);
                WriteLine("x", iterCount++, data.ball[i].Y);
                WriteLine("x", iterCount++, data.ball[i].Z);
            }

            for (int i = 0; i < cicleCount; i++)
                WriteLine("x", iterCount++, data.ball[i].R);
            WriteLine("x", iterCount++, data.R);
        }
        public static void SaveToFile(Data data, string fileName)
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
                    sw.Write(data.R.ToString().Replace(',', '.'));
                    sw.WriteLine();
                    for (int i = 0; i < data.circleCount; i++)
                    {
                        sw.Write(data.ball[i].X.ToString().Replace(',', '.') + " ");
                        sw.Write(data.ball[i].Y.ToString().Replace(',', '.') + " ");
                        sw.Write(data.ball[i].Z.ToString().Replace(',', '.') + " ");
                        sw.Write(data.ball[i].R.ToString().Replace(',', '.') + " ");
                        if (data.ball[i].Weight != 0)
                            sw.Write(data.ball[i].Weight.ToString().Replace(',', '.'));
                        sw.WriteLine();
                    }
                    sw.Close();
                    WriteLine("All Data has been written to " + writePath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage("Writing Error!! --> " + ex.Message);
            }
        }
        public static void SaveToC(double[,] C, int circleCount, string fileName)
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
                    sw.Write(circleCount);
                    sw.WriteLine();

                    for (int i = 0; i < circleCount; i++)
                    {
                        for (int j = 0; j < circleCount; j++)
                        {
                            if (j == (circleCount - 1))
                                sw.Write(C[i, j]);
                            else
                                sw.Write(C[i, j] + " ");
                        }
                        if (i != circleCount - 1)
                            sw.WriteLine();
                    }
                    sw.Close();
                    WriteLine("All Data has been written to " + writePath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage("Writing Error!! --> " + ex.Message);
            }
        }

        public static string TimeToString(Stopwatch Watch)
        {
            TimeSpan ts = Watch.Elapsed;
            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }
        // Форматирует результат конвертирования времени запуска программы 
        public static string getElapsedTime(Stopwatch Watch)
        {
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = Watch.Elapsed;
            // Format and display the TimeSpan value.

            return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }
        #endregion
        #region Form Interface
        private static void WriteToTextBox(string str)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}