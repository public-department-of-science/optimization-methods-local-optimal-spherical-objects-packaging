// Copyright (C) 2010-2011 Anders Gustafsson and others. All Rights Reserved.
// This code is published under the Eclipse Public License.
//
// Author:  Anders Gustafsson, Cureos AB 2011-09-02

using System;
using System.Runtime.InteropServices;

namespace Cureos.Numerics.Nlp
{
    /// <summary>
    /// Implementation of Hock-Schittkowski problem no. 40 from the CUTE collection.
    /// Adapted from http://www.orfe.princeton.edu/~rvdb/ampl/nlmodels/cute/hs040.mod
    /// Optimal solution x* = { 0.793701; 0.707107; 0.529732; 0.840896 }, f* = -0.25
    /// </summary>
    public class HS040 : IpoptProblem
    {
        #region CONSTRUCTORS

        public HS040(bool useNativeCallbackFunctions, bool useHessianApproximation, bool useIntermediateCallback)
            : base(
            4,  // number of variables
            new[] { NegativeInfinity, NegativeInfinity, NegativeInfinity, NegativeInfinity },   // values of the variable bounds
            new[] { PositiveInfinity, PositiveInfinity, PositiveInfinity, PositiveInfinity },
            3,  // number of constraints 
            new[] { 1.0, 0.0, 0.0 },   // values of the constraint bounds
            new[] { 1.0, 0.0, 0.0 },
            7,  // Number of nonzeros in the Jacobian of the constraints
            10, // Number of nonzeros in the Hessian of the Lagrangian (lower or upper triangual part only)
            useNativeCallbackFunctions,    // Use native callback functions,
            useHessianApproximation, // Use limited-memory Hessian approximation?
            useIntermediateCallback  // Use intermediate callback function?
            )
        {
            NumberIterations = 0;
        }
        
        #endregion

        #region PROPERTIES

        public int NumberIterations { get; private set; }
        
        #endregion

        #region NATIVE CALLBACK FUNCTIONS

        [AllowReversePInvokeCalls]
        public override IpoptBoolType eval_f(int n, double[] x, IpoptBoolType new_x, out double obj_value, IntPtr p_user_data)
        {
            obj_value = -x[0] * x[1] * x[2] * x[3];
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public override IpoptBoolType eval_g(int n, double[] x, IpoptBoolType new_x, int m, double[] g, IntPtr p_user_data)
        {
            g[0] = x[0] * x[0] * x[0] + x[1] * x[1];
            g[1] = x[0] * x[0] * x[3] - x[2];
            g[2] = x[3] * x[3] - x[1];
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public override IpoptBoolType eval_grad_f(int n, double[] x, IpoptBoolType new_x, double[] grad_f, IntPtr p_user_data)
        {
            grad_f[0] = -x[1] * x[2] * x[3];
            grad_f[1] = -x[0] * x[2] * x[3];
            grad_f[2] = -x[0] * x[1] * x[3];
            grad_f[3] = -x[0] * x[1] * x[2];
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public override IpoptBoolType eval_jac_g(int n, double[] x, IpoptBoolType new_x, int m, int nele_jac,
            int[] iRow, int[] jCol, double[] values, IntPtr p_user_data)
        {
            if (values == null)
            {
                /* set the structure of the jacobian */
                /* this particular jacobian is dense */

                iRow[0] = 0;
                jCol[0] = 0;
                iRow[1] = 0;
                jCol[1] = 1;
                iRow[2] = 1;
                jCol[2] = 0;
                iRow[3] = 1;
                jCol[3] = 2;
                iRow[4] = 1;
                jCol[4] = 3;
                iRow[5] = 2;
                jCol[5] = 1;
                iRow[6] = 2;
                jCol[6] = 3;
            }
            else
            {
                /* return the values of the jacobian of the constraints */

                values[0] = 3.0 * x[0] * x[0];  /* 0,0 */
                values[1] = 2.0 * x[1];         /* 0,1 */

                values[2] = 2.0 * x[0] * x[3];  /* 1,0 */
                values[3] = -1.0;               /* 1,2 */
                values[4] = x[0] * x[0];        /* 1,3 */

                values[5] = -1.0;               /* 2,1 */
                values[6] = 2.0 * x[3];         /* 2,3 */
            }

            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public override IpoptBoolType eval_h(int n, double[] x, IpoptBoolType new_x, double obj_factor, int m, double[] lambda,
            IpoptBoolType new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values, IntPtr p_user_data)
        {
            if (values == null)
            {
                /* set the Hessian structure. This is a symmetric matrix, fill the lower left
                 * triangle only. */

                /* the hessian for this problem is actually dense */
                int idx = 0; /* nonzero element counter */
                for (int row = 0; row < 4; row++)
                {
                    for (int col = 0; col <= row; col++)
                    {
                        iRow[idx] = row;
                        jCol[idx] = col;
                        idx++;
                    }
                }

            }
            else
            {
                /* fill the objective portion */
                values[0] = 0.0;                            /* 0,0 */

                values[1] = obj_factor * (-x[2] * x[3]);    /* 1,0 */
                values[2] = 0.0;                            /* 1,1 */

                values[3] = obj_factor * (-x[1] * x[3]);    /* 2,0 */
                values[4] = obj_factor * (-x[0] * x[3]);    /* 2,1 */
                values[5] = 0.0;                            /* 2,2 */

                values[6] = obj_factor * (-x[1] * x[2]);    /* 3,0 */
                values[7] = obj_factor * (-x[0] * x[2]);    /* 3,1 */
                values[8] = obj_factor * (-x[0] * x[1]);    /* 3,2 */
                values[9] = 0.0;                            /* 3,3 */

                /* add the portion for the first constraint */
                values[0] += lambda[0] * (6.0 * x[0]);                      /* 0,0 */

                values[2] += lambda[0] * 2.0;                               /* 1,1 */

                /* add the portion for the second constraint */
                values[0] += lambda[1] * (2.0 * x[3]);                      /* 0,0 */

                values[6] += lambda[1] * (2.0 * x[0]);                      /* 3,0 */

                /* add the portion for the third constraint */
                values[9] += lambda[2] * 2.0;                               /* 3,3 */
            }
            return IpoptBoolType.True;
        }

        [AllowReversePInvokeCalls]
        public override IpoptBoolType intermediate(
            IpoptAlgorithmMode alg_mod, int iter_count, double obj_value, double inf_pr, double inf_du,
            double mu, double d_norm, double regularization_size, double alpha_du, double alpha_pr, int ls_trials, IntPtr p_user_data)
        {
            NumberIterations = iter_count;
            return IpoptBoolType.True;
        }
        
        #endregion

        #region MANAGED CALLBACK FUNCTIONS

        public override bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            obj_value = -x[0] * x[1] * x[2] * x[3];
            return true;
        }

        public override bool eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            g[0] = x[0] * x[0] * x[0] + x[1] * x[1];
            g[1] = x[0] * x[0] * x[3] - x[2];
            g[2] = x[3] * x[3] - x[1];
            return true;
        }

        public override bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            grad_f[0] = -x[1] * x[2] * x[3];
            grad_f[1] = -x[0] * x[2] * x[3];
            grad_f[2] = -x[0] * x[1] * x[3];
            grad_f[3] = -x[0] * x[1] * x[2];
            return true;
        }

        public override bool eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac,
            int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                /* set the structure of the jacobian */
                /* this particular jacobian is dense */

                iRow[0] = 0;
                jCol[0] = 0;
                iRow[1] = 0;
                jCol[1] = 1;
                iRow[2] = 1;
                jCol[2] = 0;
                iRow[3] = 1;
                jCol[3] = 2;
                iRow[4] = 1;
                jCol[4] = 3;
                iRow[5] = 2;
                jCol[5] = 1;
                iRow[6] = 2;
                jCol[6] = 3;
            }
            else
            {
                /* return the values of the jacobian of the constraints */

                values[0] = 3.0 * x[0] * x[0];  /* 0,0 */
                values[1] = 2.0 * x[1];         /* 0,1 */

                values[2] = 2.0 * x[0] * x[3];  /* 1,0 */
                values[3] = -1.0;               /* 1,2 */
                values[4] = x[0] * x[0];        /* 1,3 */

                values[5] = -1.0;               /* 2,1 */
                values[6] = 2.0 * x[3];         /* 2,3 */
            }

            return true;
        }

        public override bool eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda,
            bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                /* set the Hessian structure. This is a symmetric matrix, fill the lower left
                 * triangle only. */

                /* the hessian for this problem is actually dense */
                int idx = 0; /* nonzero element counter */
                for (int row = 0; row < 4; row++)
                {
                    for (int col = 0; col <= row; col++)
                    {
                        iRow[idx] = row;
                        jCol[idx] = col;
                        idx++;
                    }
                }

            }
            else
            {
                /* fill the objective portion */
                values[0] = 0.0;                            /* 0,0 */

                values[1] = obj_factor * (-x[2] * x[3]);    /* 1,0 */
                values[2] = 0.0;                            /* 1,1 */

                values[3] = obj_factor * (-x[1] * x[3]);    /* 2,0 */
                values[4] = obj_factor * (-x[0] * x[3]);    /* 2,1 */
                values[5] = 0.0;                            /* 2,2 */

                values[6] = obj_factor * (-x[1] * x[2]);    /* 3,0 */
                values[7] = obj_factor * (-x[0] * x[2]);    /* 3,1 */
                values[8] = obj_factor * (-x[0] * x[1]);    /* 3,2 */
                values[9] = 0.0;                            /* 3,3 */

                /* add the portion for the first constraint */
                values[0] += lambda[0] * (6.0 * x[0]);                      /* 0,0 */

                values[2] += lambda[0] * 2.0;                               /* 1,1 */

                /* add the portion for the second constraint */
                values[0] += lambda[1] * (2.0 * x[3]);                      /* 0,0 */

                values[6] += lambda[1] * (2.0 * x[0]);                      /* 3,0 */

                /* add the portion for the third constraint */
                values[9] += lambda[2] * 2.0;                               /* 3,3 */
            }
            return true;
        }

        public override bool intermediate(
            IpoptAlgorithmMode alg_mod, int iter_count, double obj_value, double inf_pr, double inf_du,
            double mu, double d_norm, double regularization_size, double alpha_du, double alpha_pr, int ls_trials)
        {
            NumberIterations = iter_count;
            return true;
        }

        #endregion
    }
}
