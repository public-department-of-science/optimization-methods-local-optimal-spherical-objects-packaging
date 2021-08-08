using System;

namespace BooleanConfiguration.Helper
{
    public class RestrictionHelper
    {
        public static void SetXBounds(double[] x_L, double[] x_U)
        {
            for (int i = 0; i < x_L.Length; i++)
            {
                x_L[i] = 0;
                x_U[i] = 1;
            }
        }

        public static int SetM(int N)
        {
            return N;
        }

        public static void SetRestrictionsBounds(double[] g_L, double[] g_U, double[][] restrictionsMatrix, string matrixX0, double[] matrixX1, int n)
        {
            if (restrictionsMatrix == null)
            {
                return;
            }

            for (int i = 0; i < g_L.Length; i++)
            {
                // Cx0 > Cx1 ==  p0 is upper bount else it's lower bound
                double p0 = MultiplicationLineByColumn(restrictionsMatrix[i], matrixX0); // column must be
                double p1 = MultiplicationLineByColumn(restrictionsMatrix[i], matrixX1); // column

                if (p0 >= p1)
                {
                    g_L[i] = p1;
                    g_U[i] = p0;
                }
                else
                {
                    g_U[i] = p1;
                    g_L[i] = p0;
                }
            }
        }

        private static double MultiplicationLineByColumn(double[] row, double[] matrixColumn)
        {
            double value = 0.0;

            for (int i = 0; i < row.Length; i++)
            {
                value += row[i] * matrixColumn[i];
            }

            return value;
        }

        private static double MultiplicationLineByColumn(double[] row, string matrixX0)
        {
            double[] matrix0Column = GettingOneDemensionArrayFromString(row.Length, matrixX0);

            return MultiplicationLineByColumn(row, matrix0Column);
        }

        public static double[] GettingOneDemensionArrayFromString(int N, string matrixX0)
        {
            double[] matrix0Column = new double[N];

            try
            {
                for (int i = 0; i < matrix0Column.Length; i++)
                {
                    matrix0Column[i] = double.Parse(matrixX0[i].ToString());
                }
            }
            catch (FormatException)
            {
            }
            catch (Exception)
            {
            }

            return matrix0Column;
        }

        private static double[][] Multiplication(double[][] restrictionsMatrix, string matrixX0String)
        {
            double[] matrix0Array = new double[restrictionsMatrix.Length];

            for (int i = 0; i < matrix0Array.Length; i++)
            {
                matrix0Array[i] = double.Parse(matrixX0String[i].ToString());
            }

            return Multiplication(restrictionsMatrix, matrix0Array);
        }

        private static double[][] Multiplication(double[][] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new Exception("Матрицы нельзя перемножить");
            }

            double[][] r = new double[a.Length][];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new double[b.Length];
            }

            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    for (int k = 0; k < b.Length; k++)
                    {
                        r[i][j] += a[i][k] * b[k];
                    }
                }
            }
            return r;
        }
    }
}
