using MainProject.Interfaces;
using System;
using System.Collections.Generic;

namespace hs071_cs
{
    /// <summary>
    /// Main adaptor for all adaptor classes
    /// </summary>
    public abstract class BaseAdapter : IDisposable
    {
        /// <summary>
        /// amount of objects
        /// </summary>
        public int objectsCont;

        /// <summary>
        /// first coeficient (used in objective function)
        /// </summary>
        protected readonly double K1 = 1;

        /// <summary>
        /// second coeficient (used in objective function)
        /// </summary>
        protected readonly double K2 = 1;

        /// <summary>
        /// amount of variables in vector
        /// </summary>
        public int _n;

        /// <summary>
        /// amount of restrictions
        /// </summary>
        public int _m;

        /// <summary>
        /// amount of non-zero elements in jacobian
        /// </summary>
        public int _nele_jac;

        /// <summary>
        /// Hessian
        /// </summary>
        public int _nele_hess;

        /// <summary>
        /// array with lower bound of variables
        /// </summary>
        public double[] _x_L;

        /// <summary>
        /// array with upper bound of variables
        /// </summary>
        public double[] _x_U;

        /// <summary>
        /// array with lower bound for restrictions
        /// </summary>
        public double[] _g_L;

        /// <summary>
        /// array with upper bound for restrictions
        /// </summary>
        public double[] _g_U;

        /// <summary>
        /// Matrix of coefficients which can "say" prioritet of position some of object in container space
        /// </summary>
        protected double[,] C { get; set; }

        /// <summary>
        /// List with IpOpt coordinates on each iteration
        /// </summary>
        public static List<double[]> AllIteration { get; protected set; }

        /// <summary>
        /// Random instance
        /// </summary>
        protected Random random;

        /// <summary>
        /// All restrictions which you can use
        /// </summary>
        protected ObjectsRestrictions Restrictions { get; set; }

        /// <summary>
        /// Type of container which you use in your task
        /// </summary>
        protected IContainer container;

        public BaseAdapter()
        {
            objectsCont = -1;

            _x_L = null;
            _x_U = null;
            _g_L = null;
            _g_U = null;

            _n = 0;
            _m = 0;
            _nele_jac = 0;
            _nele_hess = 0;

            C = null;
            container = null;

            random = new Random();
            AllIteration = new List<double[]>();
            Restrictions = new ObjectsRestrictions();
        }

        /// <summary>
        /// Evaluation of object function
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="obj_value"></param>
        /// <returns></returns>
        public abstract bool Eval_f(int n, double[] x, bool new_x, out double obj_value);

        /// <summary>
        /// Gradient for object function 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="grad_f"></param>
        /// <returns></returns>
        public abstract bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f);

        /// <summary>
        /// Evaluation of restriction function (compute on each iteration)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="m"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public abstract bool Eval_g(int n, double[] x, bool new_x, int m, double[] g);

        /// <summary>
        /// Jacobian for each restriction function
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="new_x"></param>
        /// <param name="m"></param>
        /// <param name="nele_jac"></param>
        /// <param name="iRow"></param>
        /// <param name="jCol"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public abstract bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values);

        public abstract bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values);

        public void Dispose()
        {
            _m = 0;
        }
    }
}