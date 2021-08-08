using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    public class Class1
    {
        [Cudafy]
        public void add(int a, int b, int[] c)
        {
            c[0] = a + b;
        }

        [Cudafy]
        public void sub(int a, int b, int[] c)
        {
            c[0] = a - b;
        }

        public static void Execute()
        {
            CudafyModule km = CudafyTranslator.Cudafy(eArchitecture.OpenCL);
            GPGPU gpu = CudafyHost.GetDevice(CudafyModes.Target);

            gpu.LoadModule(km);

            int c;
            int[] dev_c = gpu.Allocate<int>();
            gpu.Launch().add(2, 7, dev_c);
            gpu.CopyFromDevice(dev_c, out c);

            Console.WriteLine("2+7 = {0}", c);
            gpu.Launch().sub(2, 7, dev_c);
            gpu.CopyFromDevice(dev_c, out c);

            Console.WriteLine("2- 7 = {0}", c);
            gpu.Free(dev_c);
        }
    }
}
