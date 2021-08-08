/*Main for Fixed Radius 3d-optimization*/
// Message of ipopt Errors: https://www.coin-or.org/Ipopt/documentation/node36.html

using System;
using System.IO;

namespace hs071_cs
{
    public static class Input
    {
        private static readonly Random rnd = new Random();

        // Reading from file while end (x, y, z, r, R)
        public static void ReadFromFile(ref double[] x, ref double[] y, ref double[] z, ref double[] r, ref double R, ref int TotalBallCount, string fileName)
        {
            ReadDataFromFile(ref x, ref y, ref z, ref r, ref R, ref TotalBallCount, fileName);
        }

        private static void ReadDataFromFile(ref double[] x, ref double[] y, ref double[] z, ref double[] r, ref double R, ref int TotalBallCount, string fileName)
        {
            try
            {
                string readPath = @"D:\p134.txt";
                FileInfo fileInfo = new FileInfo(readPath);
                if (fileInfo.Exists)
                {
                    StreamReader sr = new StreamReader(readPath);
                    string allReadedSymbols = "";
                    string[] xyzrString = new string[4];
                    string currentLine = "";
                    int i = 0;
                    while (currentLine != null)
                    {
                        currentLine = sr.ReadLine();
                        if (currentLine != null)
                        {
                            allReadedSymbols += currentLine + ";";
                            i++;
                        }
                    }

                    string[] arrayOfLines = allReadedSymbols.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    TotalBallCount = Convert.ToInt32(arrayOfLines[0].Split(' ')[0].Trim()); // amount balls

                    R = double.Parse(arrayOfLines[1].Split(' ')[0].Replace('.', ',').Trim()); // external radius

                    // check demention
                    if ((i - 1) != (TotalBallCount + 1)) /// (i-2)
                    {
                        throw new Exception("Dimension do not match!");
                    }
                    else
                    {
                        r = new double[TotalBallCount];
                        x = new double[TotalBallCount];
                        y = new double[TotalBallCount];
                        z = new double[TotalBallCount];

                        for (i = 2; i < arrayOfLines.Length; i++) //// !!!!!! arrayOfLines.Length - 2
                        {
                            xyzrString = arrayOfLines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int k = 0; k < xyzrString.Length; k++)
                            {
                                xyzrString[k] = xyzrString[k].Replace('.', ',').Trim();
                            }
                            r[i - 2] = double.Parse(xyzrString[0]);
                            x[i - 2] = double.Parse(xyzrString[1]);
                            y[i - 2] = double.Parse(xyzrString[2]);
                            z[i - 2] = double.Parse(xyzrString[3]);
                        }
                        OutPut.WriteLine("Data has been read!");
                    }
                }
            }
            catch (Exception ex)
            {
                OutPut.WriteLine(string.Format("Data not readed! Error --> {0}", ex.Message));
                OutPut.WriteLine("We generate random coordinate from 0 to 10");
            }
        }
    }
}