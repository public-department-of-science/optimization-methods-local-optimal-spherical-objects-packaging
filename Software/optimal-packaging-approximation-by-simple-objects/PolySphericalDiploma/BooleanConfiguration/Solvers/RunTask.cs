using BooleanConfiguration.Helper;
using BooleanConfiguration.IO;
using BooleanConfiguration.Model;
using Cureos.Numerics;
using hs071_cs.Adapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BooleanConfiguration.Solvers
{
    public class RunTask
    {
        public static double[] LamdaArray;

        public ResultOfResearching SolveTheProblem(Data data)
        {
            try
            {
                IPOPTAdapter dataAdapter = new IPOPTAdapter(data);
                ResultOfResearching resultOfResearching = new ResultOfResearching();

                int howManyIterationsToRun = 0;
                //lamdaArray => lambda for each line
                //mainLambda => lamdaArray.MaxFromLambdaArray();
                if (data.Ovipuckelije)
                {
                    OptimizationHelper.GettingArrayWithLabda(data.MatrixA, ref LamdaArray);
                    data.MainLambda = LamdaArray.Max();
                    howManyIterationsToRun = LamdaArray.Length;
                }
                else
                {
                    Console.WriteLine("Ввудите количество итераций цикла:");
                    int.TryParse(Console.ReadLine(), out howManyIterationsToRun);
                    LamdaArray = new double[howManyIterationsToRun];
                }

                for (int i = 0; i < howManyIterationsToRun; i++)
                {
                    using (Ipopt ipoptSolver = new Ipopt(dataAdapter._n, dataAdapter._x_L, dataAdapter._x_U, dataAdapter._m, dataAdapter._g_L, dataAdapter._g_U,
                        dataAdapter._nele_jac, dataAdapter._nele_hess, dataAdapter.Eval_f, dataAdapter.Eval_g, dataAdapter.Eval_grad_f, dataAdapter.Eval_jac_g, dataAdapter.Eval_h))
                    {
                        Stopwatch taskTime = new Stopwatch();

                        ipoptSolver.AddOption("tol", 1e-2);
                        ipoptSolver.AddOption("mu_strategy", "adaptive");
                        ipoptSolver.AddOption("hessian_approximation", "limited-memory");
                        ipoptSolver.AddOption("max_iter", 3000);
                        ipoptSolver.AddOption("print_level", 3); // 0 <= value <= 12, default is 5

                        taskTime.Start();
                        double[] x = OptimizationHelper.GettingVariablesVector(data);

                        double[] startPoint = new double[x.Length];

                        for (int j = 0; j < x.Length; j++)
                        {
                            startPoint[j] = x[j];
                        }

                        for (int j = 0; j < x.Length; j++)
                        {
                            x[j] = Math.Round(x[j]);
                        }

                        IpoptReturnCode ipoptOperationStatusCode = ipoptSolver.SolveProblem(x, out double resultValue, null, null, null, null);
                        taskTime.Stop();
                        resultOfResearching.AddNewResult(data.Ovipuckelije ? LamdaArray[i] : i,
                            new KeyValuePair<double[], Stopwatch>(x, taskTime), GetFunctionValue(data.MatrixA, x),
                            ipoptOperationStatusCode,
                            startPoint);

                        if (!data.Ovipuckelije)
                        {
                            LamdaArray[i] = i;
                        }
                        // taskTime; => spent time
                    }
                }

                return resultOfResearching;
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Output.ConsolePrint(ex.Message);
                Console.BackgroundColor = ConsoleColor.White;
                return null;
            }
        }

        private double GetFunctionValue(double[][] matrixA, double[] x)
        {
            double val = 0.0;
            for (int i = 0; i < matrixA.Length; i++)
            {
                for (int j = 0; j < matrixA[i].Length; j++)
                {
                    if (i == j)
                    {
                        val += matrixA[i][j] * x[j] * x[j];
                    }
                    else
                    {
                        val += matrixA[i][j] * x[i] * x[j];
                    }
                }
            }
            return val;
        }
    }
}
