using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Parser
{
    internal class Program
    {
        public static void Method(List<double> vs, int start, int end)
        {
            double tmp = vs[start];
            for (int i = start; i < end; i++)
            {
                vs[i] = vs[i + 1];
            }
            vs[end] = tmp;
        }

        private static void Main(string[] args)
        {

            try
            {
                string path = @"D:\DeskTop\spheres in domains\sphere\r[i]=i\ZHXF16\p.txt";
                List<double> nums = new List<double>();

                using (StreamReader sr = new StreamReader(path))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] seg = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string s in seg)
                        {
                            double num = double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
                            nums.Add(num);
                        }
                    }
                }

                string writePath = @"D:\DeskTop\spheres in domains\sphere\r[i]=i\ZHXF16\p1.txt";

                int countOfSphers = (nums.Count - 1) / 4;

                string firstLine = countOfSphers.ToString() + ' ' + (nums[nums.Count - 1]);

                nums.Remove(nums[nums.Count - 1]);

                for (int i = 0; i < nums.Count; i += 4)
                {
                    Method(nums, i, i + 3);
                }

                using (StreamWriter sw = new StreamWriter(writePath, false, Encoding.Default))
                {
                    sw.WriteLine(firstLine);
                    int k = 0;
                    int b = 0;
                    for (int i = 0; i < countOfSphers; i++)
                    {
                        k = b;
                        string tempString = "";
                        for (int j = k; j < (k + 4); j++)
                        {
                            tempString += nums[j] + " ";
                            b++;
                        }
                        tempString.TrimEnd();
                        sw.WriteLine(tempString);
                    }
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
