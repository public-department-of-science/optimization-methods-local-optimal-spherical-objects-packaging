using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hs071_cs.ObjectOptimazation
{
  public static class CombinatorialHelper
  {
    public static string DecToBase(int num_value, int base_value, int max_bit = 64)
    {
      var dec_base = 10;
      var hexchars = new[] { 'A', 'B', 'C', 'D', 'E', 'F' };
      var result = string.Empty;
      var result_array = new int[max_bit];

      for (/* nothing */; num_value > 0; num_value /= base_value)
      {
        int i = num_value % base_value;
        result_array[--max_bit] = i;
      }

      for (int i = 0; i < result_array.Length; i++)
      {
        if (result_array[i] >= dec_base)
        {
          result += hexchars[(int)result_array[i] % dec_base].ToString();
        }
        else
        {
          result += result_array[i].ToString();
        }
      }

      //result = result.TrimStart(new char[] {'0'});
      return result;
    }
    public static int GetOneCountInGroups(int[] elemGroupCount, int groupCount)
    {
      int oneCount = 0;
      // перебираем группы
      for (int i = 1; i < groupCount; ++i)
      {
        var countLineal = Math.Pow(2, elemGroupCount[i]);
        // перебирам всевозможные комбинации элементов внутри группы
        for (int a = 1; a < countLineal; ++a)
        {
          var bitStr = DecToBase(a, 2, elemGroupCount[i]);
          for (int b = 0; b < bitStr.Length; ++b)
          {
            if (bitStr[b] == '1')
            {
              oneCount++;
            }
          }
        }
        // для sum(r[i]^2)
        for (int eg = 0; eg < elemGroupCount[i]; ++eg)
          oneCount++;
      }
      return oneCount;
    }

    /// <summary>
    /// Вычисляет правую часть линейных ограничений 
    /// </summary>
    /// <returns>{1,2,3} => {1,3,5,14}</returns>
    public static double[][] GetRightPartInCombinatornOgr(double[][] elemInGroup)
    {
      var result = new double[elemInGroup.Length-1][]; // -1 нулевая группа
      for (int i = 0; i < elemInGroup.Length-1; ++i)
        result[i] = new double[elemInGroup[i+1].Length+1]; //+1 для квадратов
      // перебираем группы
      for (int i = 1; i < elemInGroup.Length; ++i)
      {
        for (int j = 0; j < elemInGroup[i].Length; ++j)
          for (int k = 0; k <= j; ++k)
            result[i-1][j] += elemInGroup[i][k];
        for (int j = 0; j < elemInGroup[i].Length; ++j)
          result[i-1][elemInGroup[i].Length] += Math.Pow(elemInGroup[i][j], 2);
      }

      return result;
    }
  }
}
