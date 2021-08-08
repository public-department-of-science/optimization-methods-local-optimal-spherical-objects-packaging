/* Main for Fixed Radius 3d-optimization */
// Message of ipopt Errors: https://www.coin-or.org/Ipopt/documentation/node36.html

using Cureos.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
    public delegate void ReadResultFromFileDel(ref double[] xNach, ref double[] yNach, ref double[] zNach, ref double[] rNach,
        ref double RNach, ref int TotalBallCount, ref int holesCount, string filePath);
    public delegate double[] RadiusSumGenerateDel(double[] radius);
    public delegate double[] RadiusRandomGenerate(double maxRandRadius, int cCount);
    public delegate void XYZRGenerateDel(int cCount, double[] r, out double[] x, out double[] y, out double[] z, out double R);
    #endregion

    public class Program
    {
        public static int VariableBALLS { get; private set; } = 10;

        public static void Main()
        {
            #region declaration of variables
            #region delegates exemplars
            PrintTextDel Print = new PrintTextDel(OutPut.Write); // Write To Console some string
            WriteResultToFileDel SaveToFile = new WriteResultToFileDel(OutPut.SaveToFile); //  Writing result
            ShowResultDel Show = new ShowResultDel(OutPut.ShowData); // return StatusCode of Optimization Problem

            #endregion
            //Start Point
            double[] rSortSum; // отсортированный массив радиусов, для ограничений
            double RNach = 0;
            double[] xNach; double[] yNach; double[] zNach; double[] rNach;
            List<double[]> IpoptIterationData = new List<double[]>();

            #region Reading Data
            Print("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");
            Input.ChooseTypeReadingData(out int TotalBallCount, out int holesCount, out xNach, out yNach, out zNach, out rNach, out RNach, out double maxRandRadius, out rSortSum);
            #endregion
            double[] xIter = new double[TotalBallCount]; double[] yIter = new double[TotalBallCount];
            double[] zIter = new double[TotalBallCount]; double[] rIter = new double[TotalBallCount];
            #region Формирование xyzR массива  
            double[] xyzFixR;
            XyzFixR(out xyzFixR, xNach, yNach, zNach, RNach, TotalBallCount);
            #endregion
            for (int i = 0; i < TotalBallCount; i++)
            {
                xIter[i] = xNach[i];
                yIter[i] = yNach[i];
                zIter[i] = zNach[i];
                rIter[i] = rNach[i];
            }
            double RIter = RNach;

            #region Matrix of Bond
            double[,] lengthBond;
            Print("Matrix of bond:\n");
            Input.SetC(out lengthBond);
            OutPut.SaveToC(lengthBond, TotalBallCount, "MatrixC");
            #endregion
            Data startPointData = new Data(xNach, yNach, zNach, rNach, RNach, TotalBallCount, VariableBALLS, holesCount, TaskClassification.FixedRadiusTask, type: null, Weight: null, C: lengthBond);
            #region Set Object Type; Set Weight Object; 
            CreateArrayWithObjectType(startPointData, out ObjectType[] objectType);
            CalculateWeight(startPointData, out double[] weight);
            #endregion

            Print("\n=== Начальные значения ===");
            #endregion
            #region Solving Problem and Time measurement
            CheckAmountOFBallsAndPropertyVariableBall(TotalBallCount);
            Stopwatch fullTaskTime = new Stopwatch();
            fullTaskTime.Start();

            Show(startPointData);
            SaveToFile(startPointData, "StartPointWithHoles");
            Print("\n=== ================== ===");

            /* Solving with fixed radius -- Start point random generated or reading from file
             * *********************************************************/
            Print("\n\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Print("\n\t\t ~~~ Solve problem ~~~");
            Print("\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            // unfixed radius
            SetObjectTypeForBallsWithUnfixedRadius(objectType);
            RandomizeStartPoint(startPointData, out double[] radiusesWichWeMustGetAfterSolutionOfProblem);
            using (FixedAndVariableRadiusAdaptor adaptor = new FixedAndVariableRadiusAdaptor(startPointData, radiusesWichWeMustGetAfterSolutionOfProblem))
            {
                double[] xyzUnFixR;
                Xyzr_R_external(out xyzUnFixR, xIter, yIter, zIter, rIter, RIter, TotalBallCount);
                RunTask(adaptor, xyzUnFixR, out xIter, out yIter, out zIter, out rIter, TotalBallCount);
                RIter = xyzUnFixR[4 * TotalBallCount];
                IpoptIterationData = adaptor.AllIteration;
                rNach = rIter;
            }
            Data optionalPointWithFixedAndVariableRadius = new Data(xIter, yIter, zIter, rNach, RIter, ballCount: TotalBallCount, VariableBALLS: VariableBALLS, holeCount: holesCount, taskClassification: TaskClassification.VariableRadius, type: objectType, Weight: weight, C: lengthBond);
            SaveToFile(optionalPointWithFixedAndVariableRadius, "CoordinateWithHoles");

            OutPut.SaveToFileAllIteration(optionalPointWithFixedAndVariableRadius, IpoptIterationData, "IterationUnFixedRadius", true);
            Print("\n\nUNFIXED_RADIUS");
            Show(optionalPointWithFixedAndVariableRadius);

            //
            fullTaskTime.Stop();
            Print("\nВыполенение всей задачи RunTime: " + OutPut.getElapsedTime(fullTaskTime));
            Print("\nQuality of solution = " + Density(rNach, RIter));
            Print("\n========= Press <RETURN> to exit... ========= \n");
            Console.ReadLine();
            #endregion
        }

        private static void CheckAmountOFBallsAndPropertyVariableBall(int totalBallCount)
        {
            if (totalBallCount < VariableBALLS)
            {
                VariableBALLS = totalBallCount;
            }
        }

        private static void SetObjectTypeForBallsWithUnfixedRadius(ObjectType[] objectType)
        {
            Parallel.For(0, VariableBALLS, i =>
             {
                 objectType[i] = ObjectType.VariableRadiusBall;
             });
        }

        private static void RandomizeStartPoint(Data data, out double[] radiusesWichWeMustGetAfterSolutionOfProblem)
        {
            Random random = new Random();
            radiusesWichWeMustGetAfterSolutionOfProblem = new double[data.VariableBALLSCount];
            for (int i = 0; i < data.VariableBALLSCount; i++)
            {
                radiusesWichWeMustGetAfterSolutionOfProblem[i] = data.ball[i].R;
                data.ball[i].R *= random.NextDouble();
                data.ball[i].ObjectType = ObjectType.VariableRadiusBall;
            };
        }

        private static void CalculateWeight(Data data, out double[] weight)
        {
            weight = new double[data.ball.Length];
            for (int i = 0; i < weight.Length; i++)
            {
                data.ball[i].Weight = weight[i] = (4.0 / 3.0) * Math.PI * Math.Pow(data.ball[i].R, 3);
            }
            Array.Sort(weight);
        }

        private static void CreateArrayWithObjectType(Data data, out ObjectType[] objectType)
        {
            objectType = new ObjectType[data.ball.Length];
            for (int i = 0, j = 0; i < data.ball.Length; i++)
            {
                if (i <= j)
                {
                    for (; j < data.holeCount; ++j, ++i)
                    {
                        data.ball[j].ObjectType = objectType[j] = (ObjectType)2;
                    }
                }
                data.ball[i].ObjectType = objectType[i] = (ObjectType)3;
            }
        }

        private static void XyzFixR(out double[] xyzFixR, double[] xNach, double[] yNach, double[] zNach, double RNach, int ballCount)
        {
            xyzFixR = new double[3 * ballCount + 1];

            int j = 0;
            for (int i = 0; i < ballCount; i++)
            {
                xyzFixR[j] = xNach[i];
                xyzFixR[++j] = yNach[i];
                xyzFixR[++j] = zNach[i];
                j++;
            }
            xyzFixR[3 * ballCount] = RNach;
        }

        private static void Xyzr_R_external(out double[] xyzrFixR, double[] xNach, double[] yNach, double[] zNach, double[] rNach, double RNach, int ballCount)
        {
            xyzrFixR = new double[4 * ballCount + 1];

            Random random = new Random();
            int j = 0;
            //int h = 1;
            double centroid = (rNach[rNach.Length - 1] + rNach[0]) / 2;// (rNach[rNach.Length - 1] / rNach.Length) / 2;
            double range = (rNach[rNach.Length - 1] + rNach[0]) / 2;
            for (int i = 0; i < VariableBALLS; i++)
            {
                xyzrFixR[j] = xNach[i];
                xyzrFixR[++j] = yNach[i];
                xyzrFixR[++j] = zNach[i];
                xyzrFixR[++j] = rNach[i] * random.NextDouble();//centroid + range * (random.NextDouble() - 0.5); //rNach[i] * random.NextDouble(); // // Math.Abs(xNach[i]); // //rNach[i];
                                                               // xyzrFixR[++j] = centroid + range * (random.NextDouble() - 0.5); //rNach[i] * random.NextDouble(); // // Math.Abs(xNach[i]); // //rNach[i];

                j++;
            }
            xyzrFixR[4 * ballCount] = RNach;
        }

        private static void RunTask(FixedRadius3dAdaptor op, double[] xyz, out double[] NewX, out double[] NewY, out double[] NewZ, int ballN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.Eval_f, op.Eval_g, op.Eval_grad_f, op.Eval_jac_g, op.Eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-10);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 10000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                /* solve the problem */
                double obj;
                status = problem.SolveProblem(xyz, out obj, null, null, null, null);
            }
            taskWatch.Stop();
            new PrintResultCodeDel(OutPut.ReturnCodeMessage)("\nOptimization return status: " + status);

            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];

            for (int i = 0; i < ballN; i++)
            {
                NewX[i] = xyz[3 * i];
                NewY[i] = xyz[3 * i + 1];
                NewZ[i] = xyz[3 * i + 2];
            }
            new PrintTextDel(OutPut.WriteLine)("RunTime: " + OutPut.getElapsedTime(taskWatch));
        }

        private static void RunTask(UnFixedRadius3dAdaptor op, double[] xyzr, out double[] NewX, out double[] NewY, out double[] NewZ, out double[] NewRadiuses, int ballN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.Eval_f, op.Eval_g, op.Eval_grad_f, op.Eval_jac_g, op.Eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-10);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 5000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                /* solve the problem */
                double obj;
                status = problem.SolveProblem(xyzr, out obj, null, null, null, null);
            }
            taskWatch.Stop();
            new PrintResultCodeDel(OutPut.ReturnCodeMessage)("\nOptimization return status: " + status);

            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];
            NewRadiuses = new double[ballN];

            for (int i = 0; i < ballN; i++)
            {
                NewX[i] = xyzr[4 * i];
                NewY[i] = xyzr[4 * i + 1];
                NewZ[i] = xyzr[4 * i + 2];
                NewRadiuses[i] = xyzr[4 * i + 3];
            }
            new PrintTextDel(OutPut.WriteLine)("RunTime: " + OutPut.getElapsedTime(taskWatch));
        }

        private static void RunTask(FixedAndVariableRadiusAdaptor op, double[] xyzr, out double[] NewX, out double[] NewY, out double[] NewZ, out double[] NewRadiuses, int ballN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.Eval_f, op.Eval_g, op.Eval_grad_f, op.Eval_jac_g, op.Eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-3);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 10000);
                problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                /* solve the problem */
                double obj;
                status = problem.SolveProblem(xyzr, out obj, null, null, null, null);
            }
            taskWatch.Stop();
            new PrintResultCodeDel(OutPut.ReturnCodeMessage)("\nOptimization return status: " + status);

            NewX = new double[ballN];
            NewY = new double[ballN];
            NewZ = new double[ballN];
            NewRadiuses = new double[ballN];

            for (int i = 0; i < ballN; i++)
            {
                NewX[i] = xyzr[4 * i];
                NewY[i] = xyzr[4 * i + 1];
                NewZ[i] = xyzr[4 * i + 2];
                NewRadiuses[i] = xyzr[4 * i + 3];
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

        private static double Unbalance(Data data)
        {
            double Unbalance = 0.0;
            for (int i = 0; i < data.ball.Length; i++)
            {
                Unbalance += data.ball[i].Weight * data.ball[i].X
                    + data.ball[i].Weight * data.ball[i].Y
                    + data.ball[i].Weight * data.ball[i].Z;
            }
            return Unbalance;
        }
    }
}