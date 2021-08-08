using System;
using System.Collections.Generic;
using System.Linq;
using Cureos.Numerics;
using MainProject.Containers;
using MainProject.Interfaces.InternalObjects.CircularObjects;
using MainProject.InternalObjectsClasses.CircularObjects;
using PackageProject.Interfaces;
using PackageProject.InternalObjectsClasses.CircularObjects;

namespace hs071_cs
{
    public class ObjectsRestrictions //: IRestrictions
    {
        #region Calculation amount of restrictions

        public void CalculationAmount_Not_IntersectionObjectsRestriction(Data data, ref int _nele_jac, ref int _m, out int amountOfElementThoseMustNotIntersect, out int _nele_jacAmountOfElementThoseMustNotIntersect)
        {
            amountOfElementThoseMustNotIntersect = 0;
            _nele_jacAmountOfElementThoseMustNotIntersect = 0;
            for (int i = 0; i < data.Objects.Count - 1; i++)
            {
                if (!((data.Objects[i] is ISphere) || (data.Objects[i] is ICombinedObject)))
                {
                    throw new Exception($"Program didn't configured for using polypoint objects. You can usr only spheres or other combined objects.{data.Objects[i].GetType().ToString()}");
                }

                List<IInternalObject> firstInternalObject = new List<IInternalObject>();
                if (data.Objects[i] is CombinedObject)
                {
                    firstInternalObject.AddRange(((CombinedObject)data.Objects[i]).InternalInCombineObjects);
                }
                else
                {
                    firstInternalObject.Add(data.Objects[i]);
                }

                for (int j = i + 1; j < data.Objects.Count; j++)
                {
                    if (!((data.Objects[j] is ISphere) || (data.Objects[j] is ICombinedObject)))
                    {
                        throw new Exception($"Program didn't configure for using polypoint objects. Only spheres or combined object.{data.Objects[j].GetType().ToString()}");
                    }

                    List<IInternalObject> secondInternalObject = new List<IInternalObject>();
                    if (data.Objects[j] is CombinedObject)
                    {
                        secondInternalObject.AddRange(((CombinedObject)data.Objects[j]).InternalInCombineObjects);
                    }
                    else
                    {
                        secondInternalObject.Add(data.Objects[j]);
                    }

                    // Cycle for not intersection restriction
                    for (int k = 0; k < firstInternalObject.Count; k++)
                    {
                        for (int z = 0; z < secondInternalObject.Count; z++)
                        {
                            _nele_jacAmountOfElementThoseMustNotIntersect += (((Sphere)secondInternalObject[z]).NumberOfVariableValues - 1) * 2; // *2 because in one time get elem for two objects
                            ++amountOfElementThoseMustNotIntersect;
                        }
                    }
                }
            }

            _nele_jac += _nele_jacAmountOfElementThoseMustNotIntersect;
            _m += amountOfElementThoseMustNotIntersect;
        }

        public void CalculationAmountOfIntersectionCombinedObjectsRestriction(Data data, ref int _nele_jac, ref int _m)
        {
            foreach (IInternalObject item in data.Objects)
            {
                CombinedObject combinedObject = item as CombinedObject;
                if (combinedObject is null)
                {
                    continue;
                }
                _m += combinedObject.AmountOfElementsInTheDistanceArray;
                _nele_jac += 2 * 3 * combinedObject.AmountOfElementsInTheDistanceArray;
            }
        }

        public void CalculationAmountOfVariablesForWholeTask(Data data, ref int _n)
        {
            foreach (IInternalObject @object in data.Objects)
            {
                _n += @object.NumberOfVariableValues;
            }
            _n += data.Container.AmountOfVariables; // container variables
        }

        #endregion

        #region Calculation amount of non-zero elements and amount of restrictions

        public void CalculationKeepingObjectsIntoContainerRestriction(Data data, ref int amountOfNonZeroElementInFirstDerivatives, ref int restrictions, int systemVariables, int objectsCount)
        {
            foreach (IInternalObject @object in data.Objects)
            {
                if (@object is ISphere)
                {
                    Sphere sphere = (Sphere)@object;

                    if (sphere.ObjectType == MainProject.Enums.Enums.ObjectType.Sphere)
                    {
                        amountOfNonZeroElementInFirstDerivatives += @object.NumberOfVariableValues - 1;
                    }
                    else
                    {
                        amountOfNonZeroElementInFirstDerivatives += @object.NumberOfVariableValues;
                    }

                    // container var
                    amountOfNonZeroElementInFirstDerivatives += data.Container.AmountOfVariables;

                    ++restrictions;
                    continue;
                }

                if (@object is ICombinedObject)
                {
                    CombinedObject combinedObject = new CombinedObject();
                    foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                    {
                        if (item is ISphere)
                        {
                            Sphere sphere = (Sphere)item;

                            if (sphere.ObjectType == MainProject.Enums.Enums.ObjectType.Sphere)
                            {
                                amountOfNonZeroElementInFirstDerivatives += @object.NumberOfVariableValues - 1;
                            }
                            else
                            {
                                amountOfNonZeroElementInFirstDerivatives += @object.NumberOfVariableValues;
                            }

                            // container var
                            amountOfNonZeroElementInFirstDerivatives += data.Container.AmountOfVariables;

                            ++restrictions;
                            continue;
                        }
                    }
                    continue;
                }
            }
        }

        #endregion

        #region Block Calculation Flour And Ceil Values

        public void CalculationFlourAndCeilValuesRangeForVariablesVector(Data data, ref int countObjects, double[] _x_L, double[] _x_U, out int systemVariables)
        {
            countObjects = 0;
            systemVariables = 0; // amount of variables(not fixed values) in system
            List<Sphere> listWithSpheres = new List<Sphere>();
            Sphere sphere = null;

            foreach (IInternalObject item in data.Objects)
            {
                if (item is ICombinedObject)
                {
                    foreach (IInternalObject item1 in ((CombinedObject)item).InternalInCombineObjects)
                    {
                        int varInOneInternalObject = 0;
                        for (; varInOneInternalObject < item1.NumberOfVariableValues; ++varInOneInternalObject, ++systemVariables)
                        {
                            sphere = (Sphere)item1;
                            listWithSpheres.Add(sphere);
                            if (varInOneInternalObject == 3) // TODO: hardcode
                            {
                                _x_L[systemVariables] = sphere.Radius;
                                _x_U[systemVariables] = sphere.Radius;
                            }
                            else
                            {
                                _x_L[systemVariables] = Ipopt.NegativeInfinity;
                                _x_U[systemVariables] = Ipopt.PositiveInfinity;
                            }
                        }
                        ++countObjects;
                    }
                }
                else
                {
                    int varInInternalObject = 0;
                    for (; varInInternalObject < item.NumberOfVariableValues; ++varInInternalObject, ++systemVariables)
                    {
                        sphere = (Sphere)item;
                        listWithSpheres.Add(sphere);
                        if (varInInternalObject == 3) // TODO: fix hardcode
                        {
                            _x_L[systemVariables] = sphere.Radius;
                            _x_U[systemVariables] = sphere.Radius;
                        }
                        else
                        {
                            _x_L[systemVariables] = Ipopt.NegativeInfinity;
                            _x_U[systemVariables] = Ipopt.PositiveInfinity;
                        }
                    }
                    ++countObjects;
                }
            }

            for (int j = 0; j < data.Container.AmountOfVariables; ++j, ++systemVariables) // TODO: hardcode for container var
            {
                _x_L[systemVariables] = listWithSpheres.Max(x => x.Radius);
                _x_U[systemVariables] = listWithSpheres.Sum(x => x.Radius);
            }
        }

        public void CalculationFlourAndCeilValuesForAllRestrictions_g(Data data, double[] _g_L, double[] _g_U, int objectsCont)
        {
            int op = 0;

            #region Keeping objects into container

            for (int j = 0; j < objectsCont; j++) // радиусы от 0 до MAX
            {
                _g_L[op] = 0;// Ipopt.NegativeInfinity;
                _g_U[op++] = Ipopt.PositiveInfinity;
            }

            #endregion

            #region Not intersection Objects

            for (int i = 0; i < data.Objects.Count - 1; i++)
            {
                if (!((data.Objects[i] is ISphere) || (data.Objects[i] is ICombinedObject)))
                {
                    throw new Exception($"Program didn't configure for using polypoint objects. Only spheres or combined object.{data.Objects[i].GetType().ToString()}");
                }

                List<IInternalObject> firstInternalObject = new List<IInternalObject>();
                if (data.Objects[i] is CombinedObject)
                {
                    firstInternalObject.AddRange(((CombinedObject)data.Objects[i]).InternalInCombineObjects);
                }
                else
                {
                    firstInternalObject.Add(data.Objects[i]);
                }

                for (int j = i + 1; j < data.Objects.Count; j++)
                {
                    if (!((data.Objects[j] is ISphere) || (data.Objects[j] is ICombinedObject)))
                    {
                        throw new Exception($"Program didn't configure for using polypoint objects. Only spheres or combined objects.{data.Objects[j].GetType().ToString()}");
                    }

                    List<IInternalObject> secondInternalObject = new List<IInternalObject>();
                    if (data.Objects[j] is CombinedObject)
                    {
                        secondInternalObject.AddRange(((CombinedObject)data.Objects[j]).InternalInCombineObjects);
                    }
                    else
                    {
                        secondInternalObject.Add(data.Objects[j]);
                    }

                    // Cycle for not intersection restriction
                    for (int k = 0; k < firstInternalObject.Count; k++)
                    {
                        Sphere first = (Sphere)firstInternalObject[k];
                        for (int z = 0; z < secondInternalObject.Count; z++)
                        {
                            Sphere second = (Sphere)secondInternalObject[z];

                            _g_L[op] = 0; // Math.Pow(first.Radius - second.Radius, 2.0);
                            _g_U[op++] = Ipopt.PositiveInfinity;
                        }
                    }
                }
            }

            #endregion

            #region Combined objects intersections

            foreach (IInternalObject item in data.Objects)
            {
                if (item as CombinedObject is null)
                {
                    continue;
                }

                double[][] arrayWithDistances = ((CombinedObject)item).ArrayWithDistances;
                for (int i = 0; i < arrayWithDistances.Length; i++)
                {
                    for (int j = 0; j < arrayWithDistances[i].Length; j++)
                    {
                        _g_L[op] = arrayWithDistances[i][j];
                        _g_U[op++] = arrayWithDistances[i][j];
                    }
                }
            }
            #endregion
        }

        #endregion

        #region Eval

        public void Evaluation_g(Data data, int n, double[] x, bool new_x, int m, double[] restrictions)
        {
            int gCount = 0;
            // (R-r[i])^2 - x[i]^2 - y[i]^2 - z^2 >= 0
            foreach (IInternalObject @object in data.Objects)
            {
                if (@object is ISphere)
                {
                    restrictions[gCount++] = EquationKeepingSphereInTheContainer((CircularContainer)data.Container, (Sphere)@object);
                    continue;
                }

                if (@object is ICombinedObject)
                {
                    foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                    {
                        restrictions[gCount++] = EquationKeepingSphereInTheContainer((CircularContainer)data.Container, (Sphere)item);
                    }
                    continue;
                }
            }

            // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
            for (int i = 0; i < data.Objects.Count - 1; i++)
            {
                List<IInternalObject> firstInternalObject = new List<IInternalObject>();
                if (data.Objects[i] is CombinedObject)
                {
                    firstInternalObject.AddRange(((CombinedObject)data.Objects[i]).InternalInCombineObjects);
                }
                else
                {
                    firstInternalObject.Add(data.Objects[i]);
                }

                for (int j = i + 1; j < data.Objects.Count; j++)
                {
                    List<IInternalObject> secondInternalObject = new List<IInternalObject>();
                    if (data.Objects[j] is CombinedObject)
                    {
                        secondInternalObject.AddRange(((CombinedObject)data.Objects[j]).InternalInCombineObjects);
                    }
                    else
                    {
                        secondInternalObject.Add(data.Objects[j]);
                    }

                    // Cycle for not intersection restriction
                    for (int k = 0; k < firstInternalObject.Count; k++)
                    {
                        Sphere first = (Sphere)firstInternalObject[k];
                        for (int z = 0; z < secondInternalObject.Count; z++)
                        {
                            Sphere second = (Sphere)secondInternalObject[z];

                            restrictions[gCount++] = EquationNotIntersactionTwoSpheres(first, second);
                            continue;
                        }
                    }
                }
            }

            // objects intersection in combined object 
            foreach (IInternalObject item in data.Objects)
            {
                CombinedObject combinedObject = item as CombinedObject;
                if (combinedObject is null)
                {
                    continue;
                }

                double[][] arrayWithDistances = combinedObject.ArrayWithDistances;
                for (int i = 0; i < arrayWithDistances.Length; i++)
                {
                    for (int j = 0; j < arrayWithDistances[i].Length; j++)
                    {
                        restrictions[gCount++] = arrayWithDistances[i][j];
                    }
                }
            }
        }

        public void Evaluation_jacobian_g(Data data, int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int kk = 0, g = 0;
                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                // позиции R, Х и У, Z
                foreach (IInternalObject @object in data.Objects)
                {
                    if (@object is ISphere)
                    {
                        kk = ElementsPositionCalculationForKeepingObjectsInArea_Jacobian_G(n, iRow, jCol, kk, g);
                        ++g;
                        continue;
                    }

                    if (@object is ICombinedObject)
                    {
                        foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                        {
                            kk = ElementsPositionCalculationForKeepingObjectsInArea_Jacobian_G(n, iRow, jCol, kk, g);
                            ++g;
                        }
                        continue;
                    }
                }

                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
                for (int i = 0; i < data.Objects.Count - 1; i++)
                {
                    List<IInternalObject> firstInternalObject = new List<IInternalObject>();
                    if (data.Objects[i] is CombinedObject)
                    {
                        firstInternalObject.AddRange(((CombinedObject)data.Objects[i]).InternalInCombineObjects);
                    }
                    else
                    {
                        firstInternalObject.Add(data.Objects[i]);
                    }

                    for (int j = i + 1; j < data.Objects.Count; j++)
                    {
                        List<IInternalObject> secondInternalObject = new List<IInternalObject>();
                        if (data.Objects[j] is CombinedObject)
                        {
                            secondInternalObject.AddRange(((CombinedObject)data.Objects[j]).InternalInCombineObjects);
                        }
                        else
                        {
                            secondInternalObject.Add(data.Objects[j]);
                        }

                        // Cycle for not intersection restriction
                        for (int k = 0; k < firstInternalObject.Count; k++)
                        {
                            for (int z = 0; z < secondInternalObject.Count; z++)
                            {
                                kk = ElementsPositionCalculationForNotIntersectionObjects_Jacobian_G(iRow, jCol, kk, g, k, z);
                                ++g;
                            }
                        }
                    }
                }

                // попарное пересечение 
                foreach (IInternalObject item in data.Objects)
                {
                    CombinedObject combinedObject = item as CombinedObject;
                    if (combinedObject is null)
                    {
                        continue;
                    }

                    double[][] arrayWithDistances = combinedObject.ArrayWithDistances;
                    for (int i = 0; i < arrayWithDistances.Length; i++)
                    {
                        for (int j = 0; j < arrayWithDistances[i].Length; j++)
                        {
                            kk = ElementsPositionCalculationForNotIntersectionObjects_Jacobian_G(iRow, jCol, kk, g, i, j);
                            ++g;
                        }
                    }
                }
            }
            else
            {
                int kk = 0;

                // (R-r[i])^2 - x[i]^2 - y[i]^2 - z[i]^2 >= 0
                foreach (IInternalObject @object in data.Objects)
                {
                    if (@object is ISphere)
                    {
                        Sphere sphere = (Sphere)@object;
                        kk = ValuesCalculationKeepingInArea_Jacobian_G(n, x, values, kk, sphere);
                        continue;
                    }

                    if (@object is ICombinedObject)
                    {
                        foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                        {
                            Sphere sphere = (Sphere)item;
                            kk = ValuesCalculationKeepingInArea_Jacobian_G(n, x, values, kk, sphere);
                            continue;
                        }
                    }
                }

                // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
                for (int i = 0; i < data.Objects.Count - 1; i++)
                {
                    List<IInternalObject> firstInternalObject = new List<IInternalObject>();
                    if (data.Objects[i] is CombinedObject)
                    {
                        firstInternalObject.AddRange(((CombinedObject)data.Objects[i]).InternalInCombineObjects);
                    }
                    else
                    {
                        firstInternalObject.Add(data.Objects[i]);
                    }

                    for (int j = i + 1; j < data.Objects.Count; j++)
                    {
                        List<IInternalObject> secondInternalObject = new List<IInternalObject>();
                        if (data.Objects[j] is CombinedObject)
                        {
                            secondInternalObject.AddRange(((CombinedObject)data.Objects[j]).InternalInCombineObjects);
                        }
                        else
                        {
                            secondInternalObject.Add(data.Objects[j]);
                        }

                        // Cycle for not intersection restriction
                        for (int k = 0; k < firstInternalObject.Count; k++)
                        {
                            Sphere first = (Sphere)firstInternalObject[k];
                            for (int z = 0; z < secondInternalObject.Count; z++)
                            {
                                Sphere second = (Sphere)secondInternalObject[z];

                                kk = ValuesCalculationObjectsNotIntersection_Jacobian_G(values, kk, first, second);
                                continue;
                            }
                        }
                    }
                }

                //попарное пересечение комб объетов
                foreach (IInternalObject item in data.Objects)
                {
                    CombinedObject combinedObject = item as CombinedObject;
                    if (combinedObject is null)
                    {
                        continue;
                    }

                    double[][] arrayWithDistances = combinedObject.ArrayWithDistances;
                    for (int i = 0; i < arrayWithDistances.Length; i++)
                    {
                        for (int j = 0; j < arrayWithDistances[i].Length; j++)
                        {
                            kk = ValuesCalculationObjectsIntersectionInCombinedObjects_Jacobian_G(values, kk);
                            continue;
                        }
                    }
                }
            }
        }

        #endregion

        #region Restriction helper methods

        private static int ValuesCalculationObjectsIntersectionInCombinedObjects_Jacobian_G(double[] values, int kk)
        {
            values[kk++] = 2.0;//* (first.Center.X - second.Center.X); //X[i]'
            values[kk++] = -2.0;// * (first.Center.X - second.Center.X); //X[j]'

            values[kk++] = 2.0;// * (first.Center.Y - second.Center.Y); //Y[i]'
            values[kk++] = -2.0;// * (first.Center.Y - second.Center.Y); //Y[j]'

            values[kk++] = 2.0;// * (first.Center.Z - second.Center.Z); //Z[i]'
            values[kk++] = -2.0;// * (first.Center.Z - second.Center.Z); //Z[j]'
            return kk;
        }

        private static int ValuesCalculationObjectsNotIntersection_Jacobian_G(double[] values, int kk, Sphere first, Sphere second)
        {
            values[kk++] = 2.0 * (first.Center.X - second.Center.X); //X[i]'
            values[kk++] = -2.0 * (first.Center.X - second.Center.X); //X[j]'

            values[kk++] = 2.0 * (first.Center.Y - second.Center.Y); //Y[i]'
            values[kk++] = -2.0 * (first.Center.Y - second.Center.Y); //Y[j]'

            values[kk++] = 2.0 * (first.Center.Z - second.Center.Z); //Z[i]'
            values[kk++] = -2.0 * (first.Center.Z - second.Center.Z); //Z[j]'
            return kk;
        }

        private static int ValuesCalculationKeepingInArea_Jacobian_G(int n, double[] x, double[] values, int kk, Sphere sphere)
        {
            values[kk] = 2.0 * (x[n - 1] - sphere.Radius); // R0'
            kk++;
            values[kk] = -2.0 * sphere.Center.X; //X'
            kk++;
            values[kk] = -2.0 * sphere.Center.Y; //Y'
            kk++;
            values[kk] = -2.0 * sphere.Center.Z; //Z'
            kk++;

            return kk;
        }

        private static int ElementsPositionCalculationForNotIntersectionObjects_Jacobian_G(int[] iRow, int[] jCol, int kk, int g, int k, int z)
        {
            // -------  X[k], X[z] ------- 
            iRow[kk] = g;
            jCol[kk++] = 0;// 4 * k;
            iRow[kk] = g;
            jCol[kk++] = 1;//4 * z;

            // -------  Y[k], Y[z] ------- 
            iRow[kk] = g; ;
            jCol[kk++] = 2;//4 * k + 1;
            iRow[kk] = g;
            jCol[kk++] = 3;// 4 * z + 1;

            // -------  Z[k], Z[z] ------- 
            iRow[kk] = g;
            jCol[kk++] = 4;// 4 * k + 2;
            iRow[kk] = g;
            jCol[kk++] = 5;// 4 * z + 2;

            return kk;
        }

        private static int ElementsPositionCalculationForKeepingObjectsInArea_Jacobian_G(int n, int[] iRow, int[] jCol, int kk, int g)
        {
            //R0 -> внешний шар 
            iRow[kk] = g;
            jCol[kk++] = n - 1;

            //X
            iRow[kk] = g;
            jCol[kk++] = 4 * g;

            //Y
            iRow[kk] = g;
            jCol[kk++] = 4 * g + 1;

            //Z
            iRow[kk] = g;
            jCol[kk++] = 4 * g + 2;

            return kk;
        }

        private static double EquationNotIntersactionTwoSpheres(Sphere first, Sphere second)
        {
            return Math.Pow(first.Center.X - second.Center.X, 2.0) + Math.Pow(first.Center.Y - second.Center.Y, 2.0) + Math.Pow(first.Center.Z - second.Center.Z, 2.0)
                            - Math.Pow(first.Radius + second.Radius, 2.0);
        }

        private static double EquationKeepingSphereInTheContainer(CircularContainer container, Sphere @object)
        {
            return Math.Pow(container.Radius - @object.Radius, 2.0)
                - Math.Pow(@object.Center.X, 2.0) - Math.Pow(@object.Center.Y, 2.0) - Math.Pow(@object.Center.Z, 2.0);
        }

        #endregion
    }
}
