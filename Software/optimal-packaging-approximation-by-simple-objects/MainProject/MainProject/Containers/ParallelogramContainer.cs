using MainProject.Interfaces;
using MainProject.Model;

namespace MainProject.Containers
{
    public class ParallelogramContainer : IParallelogramContainer
    {
        public Point[] ParallelogramPoints
        {
            get => ParallelogramPoints ?? null;
            set => ParallelogramPoints = value ?? new Point[4];
        }

        public int AmountOfVariables { get; private set; }

        public ParallelogramContainer()
        {
            int pointAmount = 4;
            AmountOfVariables = 4; // A1, A2, A3, A4
            ParallelogramPoints = new Point[pointAmount];
        }

        public double AdditionalCriteriaFunction(double[] x, int _n)
        {
            throw new System.NotImplementedException();
        }

        public void AdditionalCriteriaFunctionGrad(double[] x, double[] grad_f, int _n)
        {
            throw new System.NotImplementedException();
        }

        public double EvalFunction(double[] x, int _n)
        {
            throw new System.NotImplementedException();
        }

        public void EvalFunctionGrad(double[] x, double[] grad_f, int _n)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// fuck this method! Skip it... 
        /// </summary>
        public bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        public void Eval_g(int n, double[] x, double[] g, ref int kk, double[] radius)
        {
            throw new System.NotImplementedException();
        }

        public void Eval_jac_g(int n, double[] x, ref int kk, ref int g, double[] radius, int[] iRow, int[] jCol, double[] values, int countObjects)
        {
            throw new System.NotImplementedException();
        }
    }
}
