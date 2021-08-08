using System;

namespace hs071_cs
{
    /// <summary>
    /// Classic adaptor with spheres, combined objects
    /// </summary>
    public class Adapter : BaseAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        private static Data adaptorLocalData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public Adapter(Data data)
        {
            adaptorLocalData = data ?? throw new NullReferenceException("Data can't be null!!");
            container = data.Container;
            Restrictions.CalculationAmountOfVariablesForWholeTask(data, ref _n);

            _x_L = new double[_n];
            _x_U = new double[_n];
            C = data.C ?? null;

            Restrictions.CalculationFlourAndCeilValuesRangeForVariablesVector(data, ref objectsCont, _x_L, _x_U, out int systemVariables);

            #region Restrictions

            // 1) (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
            Restrictions.CalculationKeepingObjectsIntoContainerRestriction(data: data, amountOfNonZeroElementInFirstDerivatives: ref _nele_jac, restrictions: ref _m, systemVariables: systemVariables, objectsCount: objectsCont);
            // 2) (x[i] - x[j]) ^ 2 + (y[i] - y[j]) ^ 2 + (z[i] - z[j]) ^ 2 - (r[i] + r[j]) ^ 2 >= 0
            Restrictions.CalculationAmount_Not_IntersectionObjectsRestriction(data, ref _nele_jac, ref _m, out int amountOfElementThoseMustNotIntersect, out int _nele_jacAmountOfElementThoseMustNotIntersect);
            // 3) Intersection restriction for combinedObjects
            Restrictions.CalculationAmountOfIntersectionCombinedObjectsRestriction(data, ref _nele_jac, ref _m);

            _g_L = new double[_m];
            _g_U = new double[_m];

            Restrictions.CalculationFlourAndCeilValuesForAllRestrictions_g(data, _g_L, _g_U, objectsCont);
            
            #endregion
        }

        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            obj_value = container.EvalFunction(x, _n);
            AddNewIteration(x);
            return true;
        }

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            container.EvalFunctionGrad(x, grad_f, _n);
            container.AdditionalCriteriaFunctionGrad(x, grad_f, _n);
            return true;
        }

        public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            Restrictions.Evaluation_g(adaptorLocalData.ArrayToData(x), n, x, new_x, m, g);
            return true;
        }

        public override bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            Restrictions.Evaluation_jacobian_g((x is null) ? adaptorLocalData : adaptorLocalData.ArrayToData(x), n, x, new_x, m, nele_jac, iRow, jCol, values);
            return true;
        }

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        private void AddNewIteration(double[] element)
        {
            AllIteration.Add(element);
        }
    }
}
