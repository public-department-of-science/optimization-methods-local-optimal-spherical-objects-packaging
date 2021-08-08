using BooleanConfiguration.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BooleanConfiguration.IO
{
    internal static class Output
    {
        public static void ConsolePrint(string text, bool needWriteLine = false)
        {
            Console.Write(text);

            if (needWriteLine)
            {
                ConsolePrintWriteLine();
            }
        }

        public static void ConsolePrintWriteLine()
        {
            Console.WriteLine();
        }

        public static void PrintToConsole(Print ConsolePrint)
        {
            ConsolePrint("Select set type:", needPrintNewLine: true);
            ConsolePrint("1 - > Bn (0101101011...00111010101", needPrintNewLine: true);
            ConsolePrint("2 - > Bn(m) 00000 (n-m of 0)_111111....111111... (m of 1)", needPrintNewLine: true);
            ConsolePrint("3 - > Bn(m1, m2) 00000 (n-m1 of 0)_111111....111111... (m2 of 1)", needPrintNewLine: true);
            Console.Write("Choose type of set: ");
        }

        public static void SaveToFile(ResultOfResearching res, double[] lambda, Data data)
        {
            string writePath = AppDomain.CurrentDomain.BaseDirectory + res.ToString() + ".txt";

            try
            {

                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    double mostEffectiveSolution = 0.0;
                    double[] mostEffectivePoint = null;
                    sw.WriteLine($"Результат");

                    sw.WriteLine($"Матрица А");
                    for (int i = 0; i < data.MatrixA.Length; i++)
                    {
                        for (int j = 0; j < data.MatrixA[i].Length; j++)
                        {
                            sw.Write(data.MatrixA[i][j] + " ");
                        }
                        sw.WriteLine();
                    }

                    if (data.ConstraintsMatrix != null)
                    {
                        sw.WriteLine($"Количество ограничений {data.Constraints}");
                        sw.WriteLine($"Матрица ограничений");

                        for (int i = 0; i < data.ConstraintsMatrix.Length; i++)
                        {
                            for (int j = 0; j < data.ConstraintsMatrix[i].Length; j++)
                            {
                                sw.Write(data.ConstraintsMatrix[i][j] + " ");
                            }
                            sw.WriteLine();
                        }
                        sw.Write("\n");
                    }

                    if (data.Ovipuckelije)
                    {
                        sw.WriteLine($"Овыпукление функции использовалось: {data.Ovipuckelije}");
                        sw.Write("\n");
                    }

                    sw.WriteLine($"Количество итераций цикла: {lambda.Length}");

                    foreach (double item in lambda)
                    {
                        KeyValuePair<KeyValuePair<double[], Stopwatch>, double> result = res.GetResultById(item);
                        Cureos.Numerics.IpoptReturnCode taskStatus = res.GetTaskStatusById(item);

                        sw.WriteLine($"Статус по текущей задаче = {taskStatus.ToString()} ");
                        string text = data.Ovipuckelije ? "Лямбда-значение" : "Итерация №";
                        sw.WriteLine($" {text}= {item.ToString()}");

                        if (result.Value < mostEffectiveSolution)
                        {
                            mostEffectiveSolution = result.Value;
                            mostEffectivePoint = result.Key.Key;
                        }

                        sw.WriteLine($"Значение функции в локально-оптимальной точке = {result.Value.ToString("0")} Время поиска решения = {result.Key.Value.Elapsed.ToString()}");

                        sw.WriteLine($"Значение для стартовой точки: ");
                        double[] t = res.GetStartPointById(item);
                        for (int i = 0; i < t.Length; i++)
                        {
                            sw.Write($"{t[i].ToString("0.00")}" + " ");
                        }
                        sw.WriteLine();

                        sw.WriteLine($"Значение для локально-оптимальной точки: ");

                        for (int i = 0; i < result.Key.Key.Length; i++)
                        {
                            double item1 = result.Key.Key[i];
                            //sw.Write($" X[{i + 1}] = " + Math.Round(item1).ToString() + ";");
                            // sw.Write($" X[{i + 1}] = " + item1.ToString("0.00") + ";");
                            sw.Write($"{ Math.Round(item1)}");//.ToString("0")}"+ " ");
                        }

                        sw.WriteLine();
                        sw.WriteLine();
                    }

                    string optionalPoint = "";
                    for (int i = 0; i < mostEffectivePoint.Length; i++)
                    {
                        optionalPoint += $"{ Math.Round(mostEffectivePoint[i])}";
                    }

                    sw.WriteLine($"Наилучшее значение функции: F = {mostEffectiveSolution.ToString("0")}, Точка: {optionalPoint}");

                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                ConsolePrint(ex.Message);
            }
        }
    }
}
