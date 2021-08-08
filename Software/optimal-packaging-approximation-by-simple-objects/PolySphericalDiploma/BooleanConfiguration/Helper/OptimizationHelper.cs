using BooleanConfiguration.Model;
using System;
using System.Linq;

namespace BooleanConfiguration.Helper
{
    internal class OptimizationHelper
    {
        private static readonly Random random;

        static OptimizationHelper()
        {
            random = new Random();
        }

        public void RandomizeMatrix(double[][] matrix, int leftBound, int rightBound)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                double[] row = matrix[i];

                for (int j = i; j < row.Length; j++)
                {
                    matrix[i][j] = random.Next(leftBound, rightBound); //matrix[j][i] = // + random.Next(0, 10)) / 2;
                }
            }
        }

        public static void GettingArrayWithLabda(double[][] matrixA, ref double[] labdaArray)
        {
            labdaArray = new double[matrixA.Length];
            for (int i = 0; i < matrixA.Length; i++)
            {
                // lambdaInThisLine = Sum of line - a[i][j]
                labdaArray[i] = matrixA[i].Sum(x => x) - matrixA[i][i];
            }

            labdaArray = labdaArray.OrderBy(x => x).ToArray();
        }

        public int GerIntegerValueInlcudingUpperBound(int v1, int v2)
        {
            return random.Next(v1, v2 + 1);
        }

        public void RandomizeMatrixC(double[] maxtrixC, int leftBound, int rightBound)
        {
            for (int i = 0; i < maxtrixC.Length; i++)
            {
                maxtrixC[i] = random.Next(leftBound, rightBound);
            }
        }

        public static double[] GettingVariablesVector(Data data)
        {
            double[] randomStartValues = new double[data.N];
            for (int i = 0; i < randomStartValues.Length; i++)
            {
                randomStartValues[i] = random.NextDouble();
            }

            return randomStartValues;
        }

        public void RandomizeMatrixNxNSize(double[][] maxtrixA, int leftBound, int rightBound)
        {
            RandomizeMatrix(maxtrixA, leftBound, rightBound);
        }

        public void RandomizeMatrixX1(double[] matrixX1)
        {
            for (int j = 0; j < matrixX1.Length; j++)
            {
                matrixX1[j] = random.NextDouble();
            }
        }
    }
}
