using System;
using System.Diagnostics;
using Cureos.Numerics;

namespace hs071_cs
{
    public class SolverHelper
    {
        #region Solving region

        public Data SolveTheProblem(Adapter dataAdapter, Data data)
        {
            Data result = new Data(data.Container);

            using (Ipopt ipoptSolver = new Ipopt(dataAdapter._n, dataAdapter._x_L, dataAdapter._x_U, dataAdapter._m, dataAdapter._g_L, dataAdapter._g_U,
                dataAdapter._nele_jac, dataAdapter._nele_hess, dataAdapter.Eval_f, dataAdapter.Eval_g, dataAdapter.Eval_grad_f, dataAdapter.Eval_jac_g, dataAdapter.Eval_h))
            {
                TaskConfiguration(ipoptSolver);

                Stopwatch taskTime = new Stopwatch();

                taskTime.Start();
                double[] x = data.DataToArray();
                IpoptReturnCode t = ipoptSolver.SolveProblem(x, out double resultVector, null, null, null, null);
                taskTime.Stop();
                result = data.ArrayToData(x);
                result.Iterations = BaseAdapter.AllIteration;
                result.status = t;
                result.SpendedTime = taskTime;

            }

            return result;
        }

        private static void TaskConfiguration(Ipopt problem)
        {
            problem.AddOption("tol", 1e-4);
            problem.AddOption("mu_strategy", "adaptive");
            problem.AddOption("hessian_approximation", "limited-memory");
            problem.AddOption("max_iter", 10000);
            problem.AddOption("print_level", 3); // 0 <= value <= 12, default is 5
        }

        #endregion

        #region Measurement methods soon be refactored

        private static double Density(Data data)
        {
            return 0.0;
        }

        private static string GetElapsedTime(Stopwatch Watch)
        {
            TimeSpan ts = Watch.Elapsed;
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        }

        #endregion
    }
}
