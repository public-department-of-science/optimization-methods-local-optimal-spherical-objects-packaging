using System;
using System.Linq;

namespace BooleanConfiguration.Helper
{
    public static class IPOPTHelper
    {
        public static double CalculateFunctionObjValue(double[][] MatrixOfCoef, double[] x)
        {
            double value = 0.0;

            for (int i = 0; i < MatrixOfCoef.Length; i++)
            {
                for (int j = 0; j < MatrixOfCoef[i].Length; j++)
                {
                    if (i == j)
                    {
                        value += MatrixOfCoef[i][j] * Math.Pow(x[j], 2);
                    }
                    else
                    {
                        value += MatrixOfCoef[i][j] * x[i] * x[j];
                    }
                }
            }

            return value;
        }

        public static void CulculateFunctionGrad(double[][] matrixOfCoef, double[] x, double[] grad_f)
        {
            for (int i = 0; i < matrixOfCoef.Length; i++)
            {
                for (int j = 0; j < matrixOfCoef[i].Length; j++)
                {
                    if (i == j)
                    {
                        grad_f[i] += matrixOfCoef[i][j] * 2 * x[j];
                    }
                    else
                    {
                        grad_f[i] += matrixOfCoef[i][j] * x[i];
                    }
                }
            }
        }

        public static void CalculateEvalRestrictions(double[] x, double[] g, double[][] constraintsMatrix, double[] matrixX0)
        {
            if (constraintsMatrix != null)
            {
                for (int i = 0; i < constraintsMatrix.Length; i++)
                {
                    for (int j = 0; j < constraintsMatrix[i].Length; j++)
                    {
                        g[i] += constraintsMatrix[i][j] * x[j];
                    }
                }

                double[] resriction = new double[1];
                resriction[0] = x.Sum(y => Math.Pow(y - 0.5, 2) - Math.Sqrt(g.Length) / 2);
            }
        }

        public static void CalculateEvalJacRestrictions(double[] x, double[] values, double[][] constraintsMatrix, double[] matrixX0)
        {
            int valuesCount = 0;

            if (constraintsMatrix != null)
            {
                for (int i = 0; i < constraintsMatrix.Length; i++)
                {
                    for (int j = 0; j < constraintsMatrix[i].Length; j++)
                    {
                        values[valuesCount++] = constraintsMatrix[i][j];
                    }
                }

                double[] value = new double[x.Length];

                for (int i = 0; i < x.Length; i++)
                {
                    value[i] = 2 * (x[i] - 0.5);
                }
            }
        }

        public static void SettingNonZeroElementsInJacPosition(int[] iRow, int[] jCol, double[][] constraintsMatrix)
        {
            int lineCount = 0;

            if (constraintsMatrix != null)
            {
                for (int i = 0; i < constraintsMatrix.Length; i++)
                {
                    for (int j = 0; j < constraintsMatrix[i].Length; j++)
                    {
                        iRow[lineCount] = i;
                        jCol[lineCount] = j;
                        ++lineCount;
                    }
                }

                int[] row = new int[constraintsMatrix[0].Length];
                int[] col = new int[constraintsMatrix[0].Length];

                for (int i = 0; i < constraintsMatrix[0].Length; i++)
                {
                    row[i] = i;
                    col[i] = i;
                }
            }
        }
    }
}
