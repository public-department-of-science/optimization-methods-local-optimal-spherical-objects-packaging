using System.Collections.Generic;
using MainProject.Containers;
using MainProject.InternalObjectsClasses.CircularObjects;
using MainProject.Model;
using PackageProject.InternalObjectsClasses.CircularObjects;

namespace hs071_cs
{
    #region Delegates

    /// <summary>
    /// Console print delegate
    /// </summary>
    /// <param name="message">Text message</param>
    public delegate void PrintTextDel(string message);

    /// <summary>
    /// Delegat-helper for output error messages to stream
    /// </summary>
    /// <param name="message">Text message</param>
    public delegate void PrintErrorMessageDel(string message);

    #endregion

    internal class Program
    {
        public static PrintTextDel Print = new PrintTextDel(OutPut.Write);

        public static void Main()
        {
            #region Reading Data => will be there one method Output.ReadDataType

            //Print("\nSelect input method \n 1 --> Read from File \n 2 --> Random generate");
            //Input.ChooseTypeReadingData(out int[] amountOfObjectsInEachComplexObject, out int TotalBallCount, out xNach, out yNach, out zNach, out rNach, out RNach, out double maxRandRadius, out rSortSum);
            //Print("\nChoose  type of external container \n 1 --> Circular container \n 2 --> Parallelogram container\nSelect-->");
            //Input.ChooseTypeOfContainer(out IContainer container, RNach);

            #endregion

            SolverHelper solverHelper = new SolverHelper();

            #region will be deleted soon, now it's like imitation of data input

            CombinedObject list1 = new CombinedObject();
            list1.InternalInCombineObjects.Add(new Sphere(new Point(), 2));
            list1.InternalInCombineObjects.Add(new Sphere(new Point(3, 2, -1), 28));
            list1.InternalInCombineObjects.Add(new Sphere(new Point(3, 2, -1), 8));

            List<CombinedObject> combinedObjects = new List<CombinedObject>()
            {
                list1
            };

            Data data1232 = new Data(null)
            {
                Container = new CircularContainer(13, new Point())
            };

            // data1232.Objects.AddRange(combinedObjects);
            data1232.Objects.Add(new Sphere(new Point(-50.2, 10.8, -3.9), 5));
            data1232.Objects.Add(new Sphere(new Point(29.22, -22, 2.92), 8));
            data1232.Objects.Add(new Sphere(new Point(1, 2, 14), 10));


            #endregion

            Adapter ad = new Adapter(data1232);
            try
            {
                OutPut.SaveResultToFileStartPoint(data1232);
                Data optionalPoint = solverHelper.SolveTheProblem(dataAdapter: ad, data: data1232);
                OutPut.SaveResultToFile(optionalPoint);
                OutPut.SaveToFileAllIteration(optionalPoint);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
