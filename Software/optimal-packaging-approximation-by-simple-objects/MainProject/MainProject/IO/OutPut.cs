/*Main for Fixed Radius 3d-optimization*/
// Message of ipopt Errors: https://www.coin-or.org/Ipopt/documentation/node36.html

using System;
using System.IO;
using MainProject.Containers;
using MainProject.Interfaces.InternalObjects.CircularObjects;
using MainProject.InternalObjectsClasses.CircularObjects;
using PackageProject.Interfaces;
using PackageProject.InternalObjectsClasses.CircularObjects;

namespace hs071_cs
{
    public static class OutPut
    {
        #region Console Interface
        public static void Write(string str = "")
        {
            Console.Write(str);
        }

        public static void WriteLine(string str = "") // Console writeLine For array with index
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
        public static void WriteLine(string str, int index, double value) // Console writeLine For array with index
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

        public static void SaveResultToFile(Data optionalPoint)
        {
            try
            {
                string writePath = @"D:\" + "Coordinate" + ".txt"; // optionalPoint.ToString() + ".txt";
                FileInfo fi = new FileInfo(writePath);
                if (fi.Exists)
                {
                    fi.Delete();
                }
                using (StreamWriter sw = new StreamWriter(writePath))
                {
                    if (optionalPoint.Container is CircularContainer)
                    {
                        sw.Write(((CircularContainer)optionalPoint.Container).Radius.ToString().Replace(',', '.') + " "); // R
                    }
                    //                    sw.Write(optionalPoint.Objects.Count.ToString().Replace(',', '.')); // ballCount
                    //                  sw.Write(" ");

                    foreach (IInternalObject @object in optionalPoint.Objects)
                    {
                        sw.WriteLine();
                        if (@object is ISphere)
                        {
                            Sphere sphere = (Sphere)@object;
                            sw.Write(sphere.Center.X.ToString().Replace(',', '.') + " "); // X
                            sw.Write(sphere.Center.Y.ToString().Replace(',', '.') + " "); // Y
                            sw.Write(sphere.Center.Z.ToString().Replace(',', '.') + " "); // Z
                            sw.Write(sphere.Radius.ToString().Replace(',', '.') + " "); // R
                            continue;
                        }

                        if (@object is ICombinedObject)
                        {
                            CombinedObject combinedObject = new CombinedObject();
                            foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                            {
                                if (item is ISphere)
                                {
                                    Sphere sphere = (Sphere)item;
                                    sw.Write(sphere.Center.X.ToString().Replace(',', '.') + " "); // X
                                    sw.Write(sphere.Center.Y.ToString().Replace(',', '.') + " "); // Y
                                    sw.Write(sphere.Center.Z.ToString().Replace(',', '.') + " "); // Z
                                    sw.Write(sphere.Radius.ToString().Replace(',', '.') + " "); // R
                                    continue;
                                }
                            }
                        }
                    }

                    sw.WriteLine();

                    sw.Close();
                    WriteLine("Optional point has been written to " + writePath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage("Writing Error!! --> " + ex.Message);
            }
        }

        public static void SaveResultToFileStartPoint(Data startPoint)
        {
            try
            {
                string writePath = @"D:\" + "StartPoint" + ".txt"; // optionalPoint.ToString() + ".txt";
                FileInfo fi = new FileInfo(writePath);
                if (fi.Exists)
                {
                    fi.Delete();
                }
                using (StreamWriter sw = new StreamWriter(writePath))
                {
                    if (startPoint.Container is CircularContainer)
                    {
                        sw.Write(((CircularContainer)startPoint.Container).Radius.ToString().Replace(',', '.') + " "); // R
                    }
                    //                    sw.Write(optionalPoint.Objects.Count.ToString().Replace(',', '.')); // ballCount
                    //                  sw.Write(" ");

                    foreach (IInternalObject @object in startPoint.Objects)
                    {
                        sw.WriteLine();
                        if (@object is ISphere)
                        {
                            Sphere sphere = (Sphere)@object;
                            sw.Write(sphere.Center.X.ToString().Replace(',', '.') + " "); // X
                            sw.Write(sphere.Center.Y.ToString().Replace(',', '.') + " "); // Y
                            sw.Write(sphere.Center.Z.ToString().Replace(',', '.') + " "); // Z
                            sw.Write(sphere.Radius.ToString().Replace(',', '.') + " "); // R
                            continue;
                        }

                        if (@object is ICombinedObject)
                        {
                            CombinedObject combinedObject = new CombinedObject();
                            foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                            {
                                if (item is ISphere)
                                {
                                    Sphere sphere = (Sphere)item;
                                    sw.Write(sphere.Center.X.ToString().Replace(',', '.') + " "); // X
                                    sw.Write(sphere.Center.Y.ToString().Replace(',', '.') + " "); // Y
                                    sw.Write(sphere.Center.Z.ToString().Replace(',', '.') + " "); // Z
                                    sw.Write(sphere.Radius.ToString().Replace(',', '.') + " "); // R
                                    continue;
                                }
                            }
                        }
                    }

                    sw.WriteLine();

                    sw.Close();
                    WriteLine("Optional point has been written to " + writePath);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage("Writing Error!! --> " + ex.Message);
            }
        }


        //public static void ShowData(Data data)
        //{
        //    int cicleCount = data.objectsCount;
        //    int iterCount = 1;
        //    WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~");
        //    for (int i = 0; i < cicleCount; i++)
        //    {
        //        WriteLine("X =", iterCount++, data.objects[i].X);
        //        WriteLine("Y =", iterCount++, data.objects[i].Y);
        //        WriteLine("Z =", iterCount++, data.objects[i].X);
        //        WriteLine("R =", iterCount++, data.objects[i].R);
        //        WriteLine("____");
        //    }
        //    WriteLine("R_External = " + data.R.ToString());
        //}

        public static void SaveToFileAllIteration(Data data, bool appandFlag = true)
        {
            try
            {
                string writePath = @"D:\AllIteration.txt";
                FileInfo fi = new FileInfo(writePath);
                if (fi.Exists && appandFlag)
                {
                    fi.Delete();
                }
                using (StreamWriter sw = new StreamWriter(writePath, appandFlag, System.Text.Encoding.Default))
                {
                    sw.Write(data.Objects.Count.ToString()); // ballCount
                    sw.Write(" ");
                    sw.Write(0.ToString()); // holeCount
                    sw.Write(" ");
                    sw.Write(data.Iterations.Count); // iteration Count
                    sw.WriteLine();
                    int counterXYZ = 0, // счетчик x,y,z
                        counterR = 0; // счетчик r

                    for (int k = 0; k < data.Iterations.Count; k++)
                    {
                        counterXYZ = counterR = 0;
                        for (int i = 0; i < data.Iterations[k].Length; i++)
                        {
                            sw.Write(data.Iterations[k][i].ToString().Replace(',', '.') + " ");

                            ++counterXYZ;
                            if (counterXYZ == 3)
                            {
                                sw.Write(data.Iterations[k][i].ToString().Replace(',', '.') + " "); // R
                            }

                            counterXYZ = 0;
                        }
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

        //public static void SaveToFile(Data data, string fileName)
        //{
        //    try
        //    {
        //        string writePath = @"D:\" + fileName + ".txt";
        //        FileInfo fi = new FileInfo(writePath);
        //        if (fi.Exists)
        //        {
        //            fi.Delete();
        //        }
        //        using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
        //        {
        //            sw.Write(data.objectsCount.ToString().Replace(',', '.')); // ballCount
        //            sw.Write(" ");
        //            sw.Write(data.holesCount.ToString().Replace(',', '.')); // holeCount
        //            sw.Write(" ");
        //            sw.Write(data.R.ToString().Replace(',', '.')); // R external
        //            sw.WriteLine();
        //            for (int i = 0; i < data.objectsCount; i++)
        //            {
        //                sw.Write(data.objects[i].X.ToString().Replace(',', '.') + " ");
        //                sw.Write(data.objects[i].Y.ToString().Replace(',', '.') + " ");
        //                sw.Write(data.objects[i].Z.ToString().Replace(',', '.') + " ");
        //                sw.Write(data.objects[i].R.ToString().Replace(',', '.') + " ");
        //                if (data.objects[i].Weight != 0)
        //                    sw.Write(data.objects[i].Weight.ToString().Replace(',', '.'));
        //                sw.WriteLine();
        //            }
        //            sw.Close();
        //            WriteLine("All Data has been written to " + writePath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage("Writing Error!! --> " + ex.Message);
        //    }
        //}

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
                            {
                                sw.Write(C[i, j]);
                            }
                            else
                            {
                                sw.Write(C[i, j] + " ");
                            }
                        }
                        if (i != circleCount - 1)
                        {
                            sw.WriteLine();
                        }
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
        #endregion

        #region Form Interface
        #endregion
    }
}