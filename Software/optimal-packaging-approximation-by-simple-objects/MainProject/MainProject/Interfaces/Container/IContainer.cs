namespace MainProject.Interfaces
{
    /// <summary>
    /// This intarface include methods for calculation parameters for external container
    /// </summary>
    /// <remarks>
    /// We must culculate value for "main" container because all composed objects should be into
    /// This interface descride all calculation only for External CONTAINER! (not other)
    /// </remarks>
    public interface IContainer
    {
        /// <summary>
        /// Variable values for vontainer
        /// </summary>
        int AmountOfVariables { get; }

        /// <summary>
        /// In this method you should culculate value for objective ("target") function and reproduce your logic
        /// </summary>
        /// <remarks>
        /// Usually we searching MIN of Objective function, but can and MAX (multiply on "-1" last got value)
        /// </remarks>
        /// <param name="x">Vector of values from IPopt (on each iteration new values)</param>
        /// <param name="n">amount of variables in task</param>
        /// <returns>"Double Value" calcucated for objective value;
        /// obj_value reproduce main indicator for IPOPT when to stop searching optimum
        /// if you have addictional parameters (for example: min -> weight of each of compose objects)
        /// please implement this in  AdditionalCriteria(double[] x) method
        /// </returns>
        double EvalFunction(double[] x, int _n);

        /// <summary>
        /// Gradient "f"
        /// </summary>
        /// <param name="x">Vector of values from IPopt (on each iteration new values)</param>
        /// <param name="grad_f">vector with gradient of EvalFunction</param>
        /// <param name="_n">amount of variables in task</param>
        /// <returns>Gradient "f"</returns>
        void EvalFunctionGrad(double[] x, double[] grad_f, int _n);

        /// <summary>
        /// Optional method if you have multi Criteria task
        /// </summary>
        /// <param name="x">Vector of values from IPopt (on each iteration new values)</param>
        /// <param name="_n"></param>
        /// <returns>Objective Valuse</returns>
        double AdditionalCriteriaFunction(double[] x, int _n);

        /// <summary>
        /// Grdient of additional creteria of "f"
        /// </summary>
        /// <param name="x">Vector of values from IPopt (on each iteration new values)</param>
        /// <param name="grad_f">vector with gradient of EvalFunction</param>
        /// <param name="_n"></param>
        /// <returns>Gradient addititonal creteria of "f"</returns>
        void AdditionalCriteriaFunctionGrad(double[] x, double[] grad_f, int _n);

        ///// <summary>
        ///// Evaluation for restrictions
        ///// </summary>
        ///// <param name="n">amount of variables</param>
        ///// <param name="x">Vector of values from IPopt (on each iteration new values)</param>
        ///// <param name="g">vector with restrictions</param>
        //void Eval_g(int n, double[] x, double[] g, ref int kk, double[] radius);

        ///// <summary>
        ///// Restriction Jacobian
        ///// </summary>
        ///// <param name="n">Amount of objects</param>
        ///// <param name="x">Vector of values from IPopt (on each iteration new values)</param>
        ///// <param name="values"></param>
        //void Eval_jac_g(int n, double[] x, ref int kk, ref int g, double[] radius, int[] iRow, int[] jCol, double[] values, int countObjects);

        ///// <summary>
        ///// Optional method (you can return false and skip them)
        ///// </summary>
        ///// <param name="n"></param>
        ///// <param name="x"></param>
        ///// <param name="new_x"></param>
        ///// <param name="obj_factor"></param>
        ///// <param name="m"></param>
        ///// <param name="lambda"></param>
        ///// <param name="new_lambda"></param>
        ///// <param name="nele_hess"></param>
        ///// <param name="iRow"></param>
        ///// <param name="jCol"></param>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values);
    }
}
