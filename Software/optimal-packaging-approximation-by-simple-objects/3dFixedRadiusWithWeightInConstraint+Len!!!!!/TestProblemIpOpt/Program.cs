/* Main for Fixed Radius 3d-optimization */
// Message of ipopt Errors: https://www.coin-or.org/Ipopt/documentation/node36.html

using System;
using Cureos.Numerics;
using System.Diagnostics;

namespace hs071_cs
{
    #region Delegates
    // outPut Delegates
    public delegate void PrintTextDel(string message); // делегат для вывода информации
    public delegate void PrintErrorMessageDel(string message); // делегат для вывода информации об ошибке
    public delegate void ShowResultDel(Data data); // делегат для вывода информации об ошибке
    public delegate void WriteResultToFileDel(Data data, string fileName); // делегат для вывода информации в файл
    public delegate void PrintResultCodeDel(string codeResult); // делегат для вывода информации о решении

    // Input Delegates
    public delegate void ReadResultFromFileDel(out double[] xNach, out double[] yNach, out double[] zNach, out double[] rNach, out double RNach, string filePath);
    public delegate double[] RadiusSumGenerateDel(double[] radius);
    public delegate double[] RadiusRandomGenerate(double maxRandRadius, int cCount);
    public delegate void XYZRGenerateDel(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R);
    #endregion

    public class Program
    {
        public static void Main()
        {
            #region declaration of variables

            #region delegates exemplars
            var Print = new PrintTextDel(OutPut.Write); // Write To Console some string
            var SaveToFile = new WriteResultToFileDel(OutPut.SaveToFile); //  Writing result
            var Show = new ShowResultDel(OutPut.ShowData); // return StatusCode of Optimization Problem
            #endregion

            // input CircleCount
            int circlesCount = 0; // кол. шаров
            Input.SetCirclesCount(out circlesCount);
            Input.Circle = circlesCount; // для использования вне Main (количество кругов)
            double[] rSortSum; // отсортированный массив радиусов, для ограничений
            double RNach = 0;

            //Start Point
            var xNach = new double[circlesCount]; var yNach = new double[circlesCount]; var zNach = new double[circlesCount]; var rNach = new double[circlesCount];
            // Optional Point
            var xIter = new double[circlesCount]; var yIter = new double[circlesCount]; var zIter = new double[circlesCount]; var rIter = new double[circlesCount];
            #region Reading Data
            Print("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");
            Input.ChooseTypeReadingData(out xNach, out yNach, out zNach, out rNach, out RNach, out double maxRandRadius, out rSortSum);
            #endregion
            #region Формирование xyzR массива  
            double[] xyzFixR;
            XyzFixR(out xyzFixR, xNach, yNach, zNach, RNach, circlesCount);
            #endregion
            #region Set weight, C (Bond matrix) and Object Type
            ObjectType[] objectType; // all objects simple ball
            double[,] C;
            Print("Matrix of bond:\n");
            Input.SetC(out C);
            OutPut.SaveToC(C,circlesCount,"MatrixC");
            double[] weight;
            CalculateWeightAndSetObjectType(circlesCount, rNach, out objectType, out weight);
            #endregion

            for (int i = 0; i < circlesCount; i++)
            {
                xIter[i] = xNach[i];
                yIter[i] = yNach[i];
                zIter[i] = zNach[i];
                rIter[i] = rNach[i];
            }
            double RIter = RNach;

            Data startPointData = new Data(xNach, yNach, zNach, rNach, RNach, circlesCount, objectType, weight,C);
            Print("\n=== Начальные значения ===");
            #endregion
            #region Solving Problem and Time measurement
            Stopwatch fullTaskTime = new Stopwatch();
            fullTaskTime.Start();

            Show(startPointData);
            SaveToFile(startPointData, "StartPointWithWeight");
            Print("\n=== ================== ===");

            /* Solving with fixed radius -- Start point random generated or reading from file
             * *********************************************************/
            Print("\n\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Print("\n\t\t ~~~ Solve problem ~~~");
            Print("\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            Stopwatch fixRTaskTime = new Stopwatch();
            fixRTaskTime.Start();// Start time

            using (var adaptor = new FixedRadius3dAdaptor(startPointData))
            {
                RunTask(adaptor, xyzFixR, out xIter, out yIter, out zIter, circlesCount);
                RNach = RIter = xyzFixR[3 * circlesCount];
                rNach = adaptor.radius;
            }

            fixRTaskTime.Stop(); // Stop time
            Print("\nВыполенение задачи RunTime: " + OutPut.getElapsedTime(fixRTaskTime));

            Print("\n=== Результат расчётов ===");
            Data newData = new Data(xIter, yIter, zIter, rNach, RIter, circlesCount, objectType, weight);
            Show(newData);
            SaveToFile(newData, "CoordinateWithWeight"); // запись результата в файл

            fullTaskTime.Stop();
            Print("\nВыполенение всей задачи RunTime: " + OutPut.getElapsedTime(fullTaskTime));
            Print("\nQuality of solution = " + Density(rNach, RNach));
            Print("\n========= Press <RETURN> to exit... ========= \n");
            Console.ReadLine();
            #endregion
        }

        private static void CalculateWeightAndSetObjectType(int circlesCount, double[] rNach, out ObjectType[] objectType, out double[] weight)
        {
            objectType = new ObjectType[circlesCount];
            weight = new double[circlesCount];
            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] = (4.0 / 3.0) * Math.PI * Math.Pow(rNach[i], 3);
            }
            Array.Sort(weight);
            for (int i = 0; i < objectType.Length; i++)
            {
                objectType[i] = (ObjectType)3;
            }
        }
        private static void XyzFixR( out double [] xyzFixR,double[] xNach, double[] yNach, double[] zNach, double RNach ,int circlesCount)
        {
            xyzFixR = new double[3 * circlesCount + 1];

            int j = 0;
            for (int i = 0; i < circlesCount; i++)
            {
                xyzFixR[j] = xNach[i];
                xyzFixR[++j] = yNach[i];
                xyzFixR[++j] = zNach[i];
                j++;
            }
            xyzFixR[3 * circlesCount] = RNach;
        }
        static void RunTask(FixedRadius3dAdaptor op, double[] xyz, out double[] NewX, out double[] NewY, out double[] NewZ, int circleN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.eval_f, op.eval_g, op.eval_grad_f, op.eval_jac_g, op.eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-2);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 3000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                /* solve the problem */
                double obj;
                status = problem.SolveProblem(xyz, out obj, null, null, null, null);
            }
            taskWatch.Stop();
            new PrintResultCodeDel(OutPut.ReturnCodeMessage)("\nOptimization return status: " + status);

            NewX = new double[circleN];
            NewY = new double[circleN];
            NewZ = new double[circleN];

            for (int i = 0; i < circleN; i++)
            {
                NewX[i] = xyz[3 * i];
                NewY[i] = xyz[3 * i + 1];
                NewZ[i] = xyz[3 * i + 2];
            }
            new PrintTextDel(OutPut.WriteLine)("RunTime: " + OutPut.getElapsedTime(taskWatch));
        }
        //объемное заполнение внешнего шара
        private static double Density(double[] r, double R)
        {
            double totalCapacity = (4.0 / 3.0) * Math.PI * Math.Pow(R, 3.0),
                   realCapacity = 0;

            for (int i = 0; i < r.Length; i++)
            {
                realCapacity += (4.0 / 3.0) * Math.PI * Math.Pow(r[i], 3.0);
            }
            return realCapacity / totalCapacity;
        }
    }
}