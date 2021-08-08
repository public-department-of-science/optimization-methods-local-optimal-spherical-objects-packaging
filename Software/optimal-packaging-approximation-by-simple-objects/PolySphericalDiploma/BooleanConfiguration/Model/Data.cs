
using BooleanConfiguration.Helper;
using BooleanConfiguration.Interfaces;
using System;
using static BooleanConfiguration.Helper.Enums;

namespace BooleanConfiguration.Model
{
    /// <summary>
    /// Class with optimization parameters
    /// </summary>
    public class Data
    {
        public bool Ovipuckelije { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public double[][] MatrixA { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double[][] ConstraintsMatrix { get; private set; }

        /// <summary>
        /// Dimension of space, variables amount
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int[] Lambda { get; private set; }

        public double MainLambda { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double[] MatrixCOrRightPart { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public double[] MatrixX1 { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TypeOfSet SetType { get; set; }

        /// <summary>
        /// Class with all restriction/values culculations
        /// </summary>
        private readonly OptimizationHelper OptimizationHelper;

        public ISet Set { get; set; }

        public double[] g_L { get; set; }

        public double[] g_U { get; set; }

        public double[] x_L { get; set; }

        public double[] x_U { get; set; }

        public int Constraints { get; set; } // restrictions amount

        public Data(TypeOfSet typeOfSet, Print outStream)
        {
            OptimizationHelper = new OptimizationHelper();

            outStream("N = ");
            int.TryParse(Console.ReadLine(), out int N); // OptimizationHelper.GerIntegerValueInlcudingUpperBound(leftBound, rightBound);

            this.N = N;

            outStream("Set data range:", needPrintNewLine: true);

            outStream("Left bound = ");
            int.TryParse(Console.ReadLine(), out int leftBound);

            outStream("Right bound = ");
            int.TryParse(Console.ReadLine(), out int rightBound);

            outStream("How many constraints = ");
            int.TryParse(Console.ReadLine(), out int constraints);

            Set = SelectSet(typeOfSet);
            SetType = typeOfSet;

            // arrays allocation
            Lambda = new int[N];

            x_L = new double[N];
            x_U = new double[N];
            MatrixCOrRightPart = new double[N];
            MatrixX1 = new double[N];
            MatrixA = new double[N][];
            AllocateArrayMemory(MatrixA, N);


            if (constraints > 0)
            {
                outStream("CBQP - Solving constraint binary quadratic problem", needPrintNewLine: true);
                ConstraintsMatrix = new double[constraints][];
                Constraints = RestrictionHelper.SetM(constraints);
                AllocateArrayMemory(ConstraintsMatrix, N);
                g_L = new double[constraints];
                g_U = new double[constraints];
                OptimizationHelper.RandomizeMatrixNxNSize(ConstraintsMatrix, leftBound, rightBound); // N*N
                RestrictionHelper.SetRestrictionsBounds(g_L, g_U, ConstraintsMatrix, Set.MatrixX0, MatrixX1, N);
            }
            else
            {
                outStream("UBQP - Solving unconstraint binary quadratic problem", needPrintNewLine: true);
            }

            // Setting random values in the certain range
            OptimizationHelper.RandomizeMatrixNxNSize(MatrixA, leftBound, rightBound); // N*N
            OptimizationHelper.RandomizeMatrixC(MatrixCOrRightPart, leftBound, rightBound); // N*1
            OptimizationHelper.RandomizeMatrixX1(MatrixX1); // N*1

            RestrictionHelper.SetXBounds(x_L, x_U);
        }

        private ISet SelectSet(TypeOfSet typeOfSet)
        {
            ISet currentSet = null;

            switch (typeOfSet)
            {
                case TypeOfSet.BooleanSet:
                    currentSet = new BooleanSet(N);
                    break;
                case TypeOfSet.BnSet:
                    currentSet = new BnSet(N);
                    break;
                case TypeOfSet.SphericalLocatedBnSet:
                    currentSet = new SphericalLocatedBnSet(N);
                    break;
                default:
                    break;
            }
            return currentSet;
        }

        private void AllocateArrayMemory(double[][] array, int n)
        {
            if (n > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new double[n];
                }
            }
        }
    }
}
