namespace hs071_cs
{
    public abstract class BaseAdaptor
    {
        public int _n;
        public int _m;
        public int _nele_jac;
        public int _nele_hess;
        public double[] _x_L;
        public double[] _x_U;
        public double[] _g_L;
        public double[] _g_U;

        public BaseAdaptor()
        {
            _n = 0;
            _m = 0;
            _nele_jac = 0;
            _nele_hess = 0;
        }
        public abstract bool eval_f(int n, double[] x, bool new_x, out double obj_value);
        public abstract bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f);
        public abstract bool eval_g(int n, double[] x, bool new_x, int m, double[] g);
        public abstract bool eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values);
        public abstract bool eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values);
    }
}