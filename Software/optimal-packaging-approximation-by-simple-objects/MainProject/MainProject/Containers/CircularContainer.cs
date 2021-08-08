using MainProject.Interfaces;
using MainProject.Model;

namespace MainProject.Containers
{
    public class CircularContainer : IСircularContainer
    {
        public double Radius { get; private set; }

        public int AmountOfVariables { get; private set; }
        public Point CenterOfTheContainer { get; private set; }

        public CircularContainer(double radius, Point centerOfContainer)
        {
            Radius = radius;
            AmountOfVariables = 1; // Radius
            CenterOfTheContainer = centerOfContainer ?? new Point(0, 0, 0);
        }

        #region Interface implementation methods

        public double EvalFunction(double[] x, int _n)
        {
            return x[_n - 1];
        }

        public void EvalFunctionGrad(double[] x, double[] grad_f, int _n)
        {
            grad_f[_n - 1] = 1;
        }

        public double AdditionalCriteriaFunction(double[] x, int _n)
        {
            return 0.0;
        }

        public void AdditionalCriteriaFunctionGrad(double[] x, double[] grad_f, int _n)
        {
            grad_f[_n - 1] += 0.0;
        }

        #endregion
    }
}
