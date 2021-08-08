using System.Collections.Generic;
using System.Diagnostics;
using Cureos.Numerics;
using MainProject.Containers;
using MainProject.Helpers;
using MainProject.Interfaces;
using MainProject.Interfaces.InternalObjects.CircularObjects;
using MainProject.InternalObjectsClasses.CircularObjects;
using PackageProject.Interfaces;
using PackageProject.InternalObjectsClasses.CircularObjects;

namespace hs071_cs
{
    public class Data
    {
        public Dimension dimension = new Dimension();

        public List<double[]> Iterations { get; set; }
        public List<IInternalObject> Objects { get; }
        public double[,] C { get; } // матрица связей

        public Stopwatch SpendedTime { get; set; }

        public IpoptReturnCode status;
        public IContainer Container { get; set; }

        public Data(IContainer container)
        {
            Objects = new List<IInternalObject>();
            Container = container ?? new CircularContainer(0.0, new MainProject.Model.Point());
            Iterations = new List<double[]>();
            C = null;
        }

        /// <summary>
        /// Return double array with all system variables
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double[] DataToArray()
        {
            int varCount = 0;
            foreach (IInternalObject item in Objects)
            {
                varCount += item.NumberOfVariableValues;
            }

            double[] dataToArray = new double[varCount + Container.AmountOfVariables];
            varCount = 0;

            foreach (IInternalObject @object in Objects)
            {
                if (@object is ISphere)
                {
                    Sphere sphere = (Sphere)@object;
                    dataToArray[varCount++] = sphere.Center.X;
                    dataToArray[varCount++] = sphere.Center.Y;
                    dataToArray[varCount++] = sphere.Center.Z;
                    dataToArray[varCount++] = sphere.Radius;
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
                            dataToArray[varCount++] = sphere.Center.X;
                            dataToArray[varCount++] = sphere.Center.Y;
                            dataToArray[varCount++] = sphere.Center.Z;
                            dataToArray[varCount++] = sphere.Radius;
                            continue;
                        }
                    }
                }
            }

            for (; varCount < dataToArray.Length; ++varCount)
            {
                if (Container is CircularContainer)
                {
                    dataToArray[varCount] = ((CircularContainer)Container).Radius;
                }
            }

            return dataToArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public Data ArrayToData(double[] x)
        {
            if (x is null)
            {
                return new Data(null);
            }

            int xCount = 0;
            Data deserializedArrayToData = new Data(null);

            foreach (IInternalObject @object in Objects)
            {
                IInternalObject tempObject = null;

                if (@object is ISphere)
                {
                    double[] sphereData = new double[((Sphere)@object).NumberOfVariableValues];
                    for (int i = 0; i < ((Sphere)@object).NumberOfVariableValues; ++i, ++xCount)
                    {
                        sphereData[i] = x[xCount];
                    }
                    tempObject = new Sphere(sphereData);
                    deserializedArrayToData.Objects.Add(tempObject);
                    continue;
                }

                if (@object is ICombinedObject)
                {
                    CombinedObject combinedObject = new CombinedObject();
                    foreach (IInternalObject item in ((CombinedObject)@object).InternalInCombineObjects)
                    {
                        if (item is ISphere)
                        {
                            double[] sphereData = new double[((Sphere)item).NumberOfVariableValues];
                            for (int i = 0; i < ((Sphere)item).NumberOfVariableValues; ++i, ++xCount)
                            {
                                sphereData[i] = x[xCount];
                            }
                            tempObject = new Sphere(sphereData);
                            combinedObject.InternalInCombineObjects.Add(tempObject);
                            continue;
                        }
                    }
                    tempObject = combinedObject;
                    deserializedArrayToData.Objects.Add(tempObject);
                    continue;
                }
            }

            for (; xCount < x.Length; ++xCount)
            {
                if (Container is IСircularContainer)
                {
                    deserializedArrayToData.Container = new CircularContainer(x[xCount], new MainProject.Model.Point());
                }
            }

            return deserializedArrayToData;
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Didn't emplemented.";
        }
    }
}
