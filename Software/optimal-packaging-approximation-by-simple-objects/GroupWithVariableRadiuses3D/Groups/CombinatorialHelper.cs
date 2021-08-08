// Decompiled with JetBrains decompiler
// Type: hs071_cs.ObjectOptimazation.CombinatorialHelper
// Assembly: AdapterLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E82B398F-3AFD-42A4-A941-6E0B182418E2
// Assembly location: H:\Dropbox\диплом\CirclesInCircle\CirclesInCircle\AdapterLibrary.dll

using System;

namespace hs071_cs.ObjectOptimazation
{
    public static class CombinatorialHelper
    {
        public static string DecToBase(int num_value, int base_value, int max_bit = 64)
        {
            int num1 = 10;
            char[] chArray = new char[6]
            {
        'A',
        'B',
        'C',
        'D',
        'E',
        'F'
            };
            string str = string.Empty;
            int[] numArray = new int[max_bit];
            while (num_value > 0)
            {
                int num2 = num_value % base_value;
                numArray[--max_bit] = num2;
                num_value /= base_value;
            }
            for (int index = 0; index < numArray.Length; ++index)
            {
                str = numArray[index] < num1 ? str + numArray[index].ToString() : str + chArray[numArray[index] % num1].ToString();
            }

            return str;
        }

        public static int GetOneCountInGroups(int[] elemGroupCount, int groupCount)
        {
            int num1 = 0;
            for (int index1 = 1; index1 < groupCount; ++index1)
            {
                double num2 = Math.Pow(2.0, elemGroupCount[index1]);
                for (int num_value = 1; num_value < num2; ++num_value)
                {
                    foreach (char ch in CombinatorialHelper.DecToBase(num_value, 2, elemGroupCount[index1]))
                    {
                        if (ch == '1')
                        {
                            ++num1;
                        }
                    }
                }
                for (int index2 = 0; index2 < elemGroupCount[index1]; ++index2)
                {
                    ++num1;
                }
            }
            return num1;
        }

        public static double[][] GetRightPartInCombinatornOgr(double[][] elemInGroup)
        {
            double[][] numArray = new double[elemInGroup.Length - 1][];
            for (int index = 0; index < elemInGroup.Length - 1; ++index)
            {
                numArray[index] = new double[elemInGroup[index + 1].Length + 1];
            }

            for (int index1 = 1; index1 < elemInGroup.Length; ++index1)
            {
                for (int index2 = 0; index2 < elemInGroup[index1].Length; ++index2)
                {
                    for (int index3 = 0; index3 <= index2; ++index3)
                    {
                        numArray[index1 - 1][index2] += elemInGroup[index1][index3];
                    }
                }
                for (int index2 = 0; index2 < elemInGroup[index1].Length; ++index2)
                {
                    numArray[index1 - 1][elemInGroup[index1].Length] += Math.Pow(elemInGroup[index1][index2], 2.0);
                }
            }
            return numArray;
        }
    }
}
