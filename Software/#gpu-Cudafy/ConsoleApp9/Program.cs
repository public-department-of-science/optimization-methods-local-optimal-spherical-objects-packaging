using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp9
{
    [MemoryDiagnoser]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.Net461)]
    [SimpleJob(runtimeMoniker: RuntimeMoniker.Net462)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.Net47)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.Net471)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.Net472)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.Net48)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.NetCoreApp20)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.NetCoreApp21)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.NetCoreApp22)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.NetCoreApp30)]
    [SimpleJob(runtimeMoniker: RuntimeMoniker.NetCoreApp31)]
    //[SimpleJob(runtimeMoniker: RuntimeMoniker.NetCoreApp50)]
    public class Program
    {
        private static readonly object obj = new object();
        private static readonly int List_size = 1_000_000;

        public static string str = "jhjjkakjkj asdjasjkasjkkj jajiiieie hjjjjs jjjjjvnnvbb " +
            "kkdsfjsddsfosdofsdjkjlxjlk ll ldsldl lorotiiyi ollgl l" +
            "lliiiyiykfkjgjll;;km;''lkdskfksji49030" +
            "sdflksdkdsk kdsfk lfloo4959 kfdl l;df 599olfdl dfk 9fd9r999  odof fl df9994o fdo 9fd9 o4]" +
            "b hhshsh uuwuui iiahh kkakshh hgh gfgfg gasjj gjjgj";

        class StructTest
        {
         //   public string MyProperty { get; set; }
           // public int Integer { get; set; }
        }

        [Benchmark]
        public void StructToTest()
        {
            for (int i = 0; i < 10; i++)
            {

            StructTest test = new StructTest();
            }
          //  test.Integer = 12;
          //  test.MyProperty = "sa";
           // Console.WriteLine(test.Integer + test.MyProperty);
          //  Method( test);
           // Console.WriteLine(test.Integer + test.MyProperty);
        }

        static void Main()
        {
            // BenchmarkRunner.Run<Program>();
            Class1.Execute();
        }
        static void Method(ref StructTest test)
        {
          //  test.Integer = 555;
            //test.MyProperty = "777";

        }

        // [Benchmark]
        public void StringSortToBenchmark()
        {
            var t = SortedString(str);
        }

        // [Benchmark]
        public void StringBuilderSortToBenchmark()
        {
            var t = SortedStringBuilder(str);
        }

        //[Benchmark]
        public void For()
        {
            List<int> vs = new List<int>(List_size);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < vs.Count; j++)
                {
                    var t = vs[j];
                }
            }
        }

       // [Benchmark]
        public void ForEach()
        {
            List<int> vs = new List<int>(List_size);
            for (int i = 0; i < 10; i++)
            {
                foreach (var item in vs)
                {
                    var t = item;
                }
            }
        }

        public static string SortedStringBuilder(string unsortedString)
        {
            List<char> list = new List<char>();
            foreach (var digit in unsortedString)
            {
                list.Add(digit);
            }

            lock (obj)
            {
                list.Sort();
            }

            StringBuilder builder = new StringBuilder(capacity: list.Count);
            foreach (var digit in list)
            {
                builder.Append(digit);
            }

            return builder.ToString();
        }

        public static string SortedString(string unsortedString)
        {
            List<char> list = new List<char>();
            foreach (var digit in unsortedString)
            {
                list.Add(digit);
            }

            list.Sort();

            var str = "";
            foreach (var digit in list)
            {
                str += digit;
            }

            return str;
        }
    }


}
