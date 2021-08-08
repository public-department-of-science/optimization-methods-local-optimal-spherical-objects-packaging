/* Main for Fixed Radius 3d-optimization */
// Message of ipopt Errors: https://www.coin-or.org/Ipopt/documentation/node36.html

using System;
using Cureos.Numerics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

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
        public static void Main()
        {
            #region declaration of variables and setting StartPoint

            #region delegates exemplars
            var Print = new PrintTextDel(OutPut.Write); // Write To Console some string
            var SaveToFile = new WriteResultToFileDel(OutPut.SaveToFile); //  Writing result
            var Show = new ShowResultDel(OutPut.ShowData); // print data of Optimization Problem

            #endregion

            #region Reading Data and set start point
            //Start Point
            Print("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");
            int amountOfInternalBall = 2;
            Data[] startPointData = new Data[amountOfInternalBall];
            Print("\n=== Начальные значения ===");
            for (int i = 0; i < amountOfInternalBall; i++)
            {
                CreateStartPoint(out startPointData[i], amountOfInternalBall);
                OutPut.ReturnCodeMessage($"\nGeneration ==> {i} -- { startPointData[i].ballCount}");
                Show(startPointData[i]);
            }
            //SaveToFile(startPointData[0], "StartPointWithHoles");
            #endregion

            #endregion

            #region Solving Problem and Time measurement
            Stopwatch fullTaskTime = new Stopwatch();
            fullTaskTime.Start();
            Print("\n=== ================== ===");
            /* Solving with fixed radius -- Start point random generated or reading from file
             * *********************************************************/
            Print("\n\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\t\t ~~~ Solve problem ~~~\n\t ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            #region Fixed radius
            Stopwatch TimeForFixedRadiusTask = new Stopwatch();
            TimeForFixedRadiusTask.Start();// Start time

            Data[] optionalPoint = new Data[amountOfInternalBall];
            //Task[] fixedRadiusTasks = new Task[amountOfInternalBall]; // ARRAY TASKS
            //fixedRadiusTasks[0] = Task.Factory.StartNew(() => // CREATE + START TASK
            //{
            //Parallel.For(0, amountOfInternalBall, i =>
            //{

            for (int i = 0; i < amountOfInternalBall; i++)
            {
                int z = i;
                SolveProblemWithFixedRadius(Print, startPointData[z], out optionalPoint[z]);
                OutPut.ReturnCodeMessage($"\nGeneration ==> {z}");
                Show(optionalPoint[z]);
                //});
            }
            //    //// Print($"\nQuality of solution = { optionalPoint[i].R}");
            //fixedRadiusTasks[1] = Task.Factory.StartNew(() => // CREATE + START TASK
            //{
            //    SolveProblemWithFixedRadius(Print, startPointData[1], out optionalPoint[1]);
            //    OutPut.ReturnCodeMessage($"\nGeneration ==> {1}");
            //    Show(optionalPoint[1]);
            //    //// Print($"\nQuality of solution = { optionalPoint[i].R}");
            //});
            //Task.WaitAll(fixedRadiusTasks); // WAIT ALL TASKS
            TimeForFixedRadiusTask.Stop(); // Stop time
            Print("\nВыполенение задачи RunTime: " + OutPut.getElapsedTime(TimeForFixedRadiusTask) + "\n === Результат расчётов === ");

            //OutPut.SaveToFileAllIteration(optionalPoint[0], IpoptIterationData, "IterationFixedRadius", true);
            //SaveToFile(optionalPoint[0], "CoordinateWithHoles"); // запись результата в файл
            #endregion

            #region VARIABLE RADIUS
            Stopwatch TimeForVariableRadiusTask = new Stopwatch();
            TimeForVariableRadiusTask.Start();// Start time
            Data[] optionalPointWithUnfixedRadius = new Data[amountOfInternalBall];
            // Task[] variableRadiusTasks = new Task[amountOfInternalBall]; // ARRAY TASKS
            for (int i = 0; i < amountOfInternalBall; i++)
            {
                //  variableRadiusTasks[i] = Task.Run(() => // CREATE + START TASK
                //  {
                SolveProblemWithVariableRadius(startPointData[i], optionalPoint[i], out optionalPointWithUnfixedRadius[i]);
                OutPut.ReturnCodeMessage($"\nGeneration ==> {i}");
                Show(optionalPointWithUnfixedRadius[i]);
                // Print($"\nQuality of solution = { optionalPoint[i].R}");
                // });
            }
            // Task.WaitAll(variableRadiusTasks); //   WAIT
            TimeForFixedRadiusTask.Stop(); // Stop time
            Print("\nВыполенение задачи RunTime: " + OutPut.getElapsedTime(TimeForFixedRadiusTask) + "\n=== Результат расчётов ===");
            #endregion

            #region Balls Packing which contains inner balls 
            Print("\nTrying to improve solution");
            Data ballsWhichContainsInnerBallsStartPoint;
            Data ballsWhichContainsInnerBalls;
            // предвнешние шары (упаковка с фикс радиусом)
            PackingExternalBallsForEachGroups(Print, SaveToFile, amountOfInternalBall, startPointData, optionalPointWithUnfixedRadius, out ballsWhichContainsInnerBallsStartPoint, out ballsWhichContainsInnerBalls);
            //
            #region Union All Ball in one object for writing to file
            Data dataAboutAllBallWhichPackedInBall;
            UnionAllInformationAboutThatProblem(amountOfInternalBall, optionalPointWithUnfixedRadius, ballsWhichContainsInnerBalls, out dataAboutAllBallWhichPackedInBall);
            SaveToFile(dataAboutAllBallWhichPackedInBall, "CoordinateWithHoles100"); // запись результата в файл
            #endregion
            Print("\n\nUNFIXED_RADIUS");
            //
            fullTaskTime.Stop();
            Print("\nВыполенение всей задачи RunTime: " + OutPut.getElapsedTime(fullTaskTime));
            Print("\n========= Press <Enter> to exit... ========= \n");
            Console.ReadLine();
            #endregion

            #endregion
        }

        private static void CreateStartPoint(out Data startPointData, int amountOfBall)
        {
            int TotalBallCount;
            int holesCount;
            double RNach = 0;
            double[] xNach = null; double[] yNach = null; double[] zNach = null; double[] rNach = null;

            Input.ChooseTypeReadingData(out TotalBallCount, out holesCount, out xNach, out yNach, out zNach, out rNach, out RNach, out double maxRandRadius, out double[] rSortSum);
            startPointData = new Data(xNach, yNach, zNach, rNach, RNach, TotalBallCount, holesCount, TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);
        }

        private static void UnionAllInformationAboutThatProblem(int amountOfInternalBall, Data[] optionalPointWithUnfixedRadius,
            Data ballsWhichContainsInnerBalls, out Data dataAboutAllBallWhichPackedInBall)
        {
            int AmountOfAllBalls = amountOfInternalBall;
            for (int i = 0; i < amountOfInternalBall; i++)
            {
                AmountOfAllBalls += optionalPointWithUnfixedRadius[i].ballCount;
            }
            Array.Sort(optionalPointWithUnfixedRadius);
            double[] cordinateX = new double[AmountOfAllBalls]; double[] cordinateY = new double[AmountOfAllBalls];
            double[] cordinateZ = new double[AmountOfAllBalls]; double[] radius = new double[AmountOfAllBalls];

            int countForAllBalls = 0;
            // НУjНО ЗАДАТЬ СМЕЩЕНИЕ 
            for (int i = 0; i < amountOfInternalBall; i++) // cycle at each group
            {
                for (int j = 0; j < optionalPointWithUnfixedRadius[i].ball.Length; j++)
                {
                    cordinateX[countForAllBalls] = optionalPointWithUnfixedRadius[i].ball[j].X + ballsWhichContainsInnerBalls.ball[i].X;
                    cordinateY[countForAllBalls] = optionalPointWithUnfixedRadius[i].ball[j].Y + ballsWhichContainsInnerBalls.ball[i].Y;
                    cordinateZ[countForAllBalls] = optionalPointWithUnfixedRadius[i].ball[j].Z + ballsWhichContainsInnerBalls.ball[i].Z;
                    radius[countForAllBalls] = optionalPointWithUnfixedRadius[i].ball[j].R;
                    ++countForAllBalls;
                }
            }
            // дописываем внешние радиусы для каждой из групп в общие массивы 
            for (int i = 0; i < amountOfInternalBall; i++)
            {
                cordinateX[countForAllBalls] = ballsWhichContainsInnerBalls.ball[i].X;
                cordinateY[countForAllBalls] = ballsWhichContainsInnerBalls.ball[i].Y;
                cordinateZ[countForAllBalls] = ballsWhichContainsInnerBalls.ball[i].Z;
                radius[countForAllBalls] = optionalPointWithUnfixedRadius[i].R;
                ++countForAllBalls;
            }
            //double[] weightForBall = new double[AmountOfAllBalls];
            dataAboutAllBallWhichPackedInBall = new Data(cordinateX, cordinateY, cordinateZ, radius, ballsWhichContainsInnerBalls.R, ballCount: AmountOfAllBalls, holeCount: 0,
                    taskClassification: TaskClassification.FixedRadiusTask, type: null, Weight: null, C: null);

            CalculateWeight(dataAboutAllBallWhichPackedInBall, out double[] weightForBall);
        }

        private static void PackingExternalBallsForEachGroups(PrintTextDel Print, WriteResultToFileDel SaveToFile, int amountOfInternalBall, Data[] startPointData,
            Data[] optionalPointWithUnfixedRadius, out Data ballsWhichContainsInnerBallsStartPoint, out Data ballsWhichContainsInnerBalls)
        {
            #region Matrix of Bond
            double[,] lengthBond1;
            Print("Matrix of bond:\n");
            Input.Ball = amountOfInternalBall;
            Input.SetC(out lengthBond1);
            OutPut.SaveToC(lengthBond1, amountOfInternalBall, "MatrixC");
            #endregion

            double[] xIter = new double[amountOfInternalBall]; double[] yIter = new double[amountOfInternalBall];
            double[] zIter = new double[amountOfInternalBall]; double[] rIter = new double[amountOfInternalBall];
            double RIter = 0;

            double[] xNach = new double[amountOfInternalBall]; double[] yNach = new double[amountOfInternalBall];
            double[] zNach = new double[amountOfInternalBall]; double[] rNach = new double[amountOfInternalBall];
            double RNach = 0;

            for (int j = 0; j < rNach.Length; j++)
            {
                rNach[j] = optionalPointWithUnfixedRadius[j].R;
            }
            Input.Ball = amountOfInternalBall;
            new XYZRGenerateDel(Input.XyzRRandomGenerateAvg)(amountOfInternalBall, rNach, out xNach, out yNach, out zNach, out RNach); // генерируем начальные точки x,y,r,R

            ballsWhichContainsInnerBallsStartPoint = new Data(xNach, yNach, zNach, rNach, RNach, ballCount: amountOfInternalBall,
                holeCount: 0, taskClassification: TaskClassification.FixedRadiusTask, type: null, Weight: null, C: lengthBond1);

            StartFindingLocalSolutionWithFixedRadius(ballsWhichContainsInnerBallsStartPoint, out xIter, out yIter, out zIter, out rIter, out RIter);

            ballsWhichContainsInnerBalls = new Data(xIter, yIter, zIter, rNach, RIter, ballCount: amountOfInternalBall, holeCount: 0,
                   taskClassification: TaskClassification.FixedRadiusTask, type: null, Weight: rNach, C: lengthBond1);

            SaveToFile(ballsWhichContainsInnerBalls, "CoordinateWithHoles100"); // запись результата в файл
        }

        private static void SolveProblemWithVariableRadius(Data startPointData, Data optionalPoint, out Data optionalPointWithUnfixedRadius)
        {
            double[] xIter = new double[startPointData.ballCount]; double[] yIter = new double[startPointData.ballCount];
            double[] zIter = new double[startPointData.ballCount]; double[] rIter = new double[startPointData.ballCount];
            double RIter = 0;
            RandomizeStartPoint(optionalPoint, out double[] constantRadius);
            ObjectType[] objectType = new ObjectType[startPointData.ballCount];
            SetObjectTypeForBallsWithUnfixedRadius(objectType);
            CalculateWeight(startPointData, out double[] weight);
            rIter = StartFindingLocalSolutionWithVariableRadius(optionalPoint, out xIter, out yIter, out zIter, rIter, out RIter, constantRadius);
            optionalPointWithUnfixedRadius = new Data(xIter, yIter, zIter, rIter, RIter, ballCount: startPointData.ballCount,
                holeCount: startPointData.holeCount, taskClassification: TaskClassification.VariableRadius, type: objectType, Weight: weight, C: startPointData.C);
        }

        private static double[] StartFindingLocalSolutionWithVariableRadius(Data optionalPoint, out double[] xIter, out double[] yIter, out double[] zIter, double[] rIter, out double RIter, double[] constantRadius)
        {
            using (var adaptor = new UnFixedRadius3dAdaptor(optionalPoint, constantRadius))
            {
                double[] xyzUnFixR;
                Xyzr_R_external(out xyzUnFixR, optionalPoint.X, optionalPoint.Y, optionalPoint.Z, rIter, optionalPoint.R, optionalPoint.ballCount);
                RunTask(adaptor, xyzUnFixR, out xIter, out yIter, out zIter, out rIter, optionalPoint.ballCount);
                RIter = xyzUnFixR[4 * optionalPoint.ballCount];
                //       IpoptIterationData = adaptor.AllIteration;
            }

            return rIter;
        }

        private static void SolveProblemWithFixedRadius(PrintTextDel Print, Data startPointData, out Data optionalPoint)
        {
            double[] xIter = new double[startPointData.ballCount]; double[] yIter = new double[startPointData.ballCount];
            double[] zIter = new double[startPointData.ballCount]; double[] rIter = new double[startPointData.ballCount];
            double RIter = 0;
            #region Matrix of Bond
            double[,] lengthBond;
            Print("Matrix of bond:\n");
            Input.Ball = startPointData.ballCount;
            Input.SetC(out lengthBond);
            // OutPut.SaveToC(lengthBond, startPointData.ballCount, "MatrixC");
            #endregion

            #region Set Object Type; Set Weight Object; 
            CreateArrayWithObjectType(startPointData, out ObjectType[] objectType);
            CalculateWeight(startPointData, out double[] weight);
            #endregion

            startPointData.C = lengthBond;
            StartFindingLocalSolutionWithFixedRadius(startPointData, out xIter, out yIter, out zIter, out rIter, out RIter);
            optionalPoint = new Data(xIter, yIter, zIter, rIter, RIter, ballCount: startPointData.ballCount, holeCount: startPointData.holeCount,
                taskClassification: TaskClassification.FixedRadiusTask, type: objectType, Weight: weight, C: startPointData.C);
        }

        private static void StartFindingLocalSolutionWithFixedRadius(Data startPointData, out double[] xIter, out double[] yIter, out double[] zIter, out double[] rIter, out double RIter)
        {
            using (var adaptor = new FixedRadius3dAdaptor(startPointData))
            {
                #region Формирование xyzR массива  
                double[] xyzFixR;
                XyzFixR(out xyzFixR, startPointData.X, startPointData.Y, startPointData.Z, startPointData.R, startPointData.ballCount);
                #endregion

                RunTask(adaptor, xyzFixR, out xIter, out yIter, out zIter, startPointData.ballCount);
                RIter = xyzFixR[3 * startPointData.ballCount];
                // IpoptIterationData = adaptor.AllIteration;
                rIter = adaptor.radius;
            }
        }

        private static void SetObjectTypeForBallsWithUnfixedRadius(ObjectType[] objectType)
        {
            Parallel.For(0, objectType.Length, i =>
             {
                 objectType[i] = ObjectType.VariiableRadiusBall;
             });
        }

        private static void RandomizeStartPoint(Data data, out double[] constantRadius)
        {
            Random random = new Random();
            constantRadius = new double[data.ballCount];
            for (int i = 0; i < data.ballCount; i++)
            {
                constantRadius[i] = data.ball[i].R;
                data.ball[i].R *= random.NextDouble();
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
            for (int i = 0; i < ballCount; i++)
            {
                xyzrFixR[j] = xNach[i];
                xyzrFixR[++j] = yNach[i];
                xyzrFixR[++j] = zNach[i];
                xyzrFixR[++j] = Math.Abs(xNach[i]); //rNach[i] * random.NextDouble(); //rNach[i];
                j++;
            }
            xyzrFixR[4 * ballCount] = RNach;
        }

        static void RunTask(FixedRadius3dAdaptor op, double[] xyz, out double[] NewX, out double[] NewY, out double[] NewZ, int ballN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.Eval_f, op.Eval_g, op.Eval_grad_f, op.Eval_jac_g, op.Eval_h))
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
                taskWatch.Stop();
            }

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

        static void RunTask(UnFixedRadius3dAdaptor op, double[] xyzr, out double[] NewX, out double[] NewY, out double[] NewZ, out double[] NewRadiuses, int ballN)
        {
            Stopwatch taskWatch = new Stopwatch();
            IpoptReturnCode status;
            taskWatch.Start();
            using (Ipopt problem = new Ipopt(op._n, op._x_L, op._x_U, op._m, op._g_L, op._g_U, op._nele_jac, op._nele_hess, op.Eval_f, op.Eval_g, op.Eval_grad_f, op.Eval_jac_g, op.Eval_h))
            {
                // https://www.coin-or.org/Ipopt/documentation/node41.html#opt:print_options_documentation
                problem.AddOption("tol", 1e-6);
                problem.AddOption("mu_strategy", "adaptive");
                problem.AddOption("hessian_approximation", "limited-memory");
                problem.AddOption("max_iter", 3000);
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