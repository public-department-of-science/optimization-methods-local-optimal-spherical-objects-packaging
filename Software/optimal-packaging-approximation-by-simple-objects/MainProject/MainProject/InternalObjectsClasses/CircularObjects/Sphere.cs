using System;
using MainProject.Enums;
using MainProject.Model;
using PackageProject.Interfaces;

namespace PackageProject.InternalObjectsClasses.CircularObjects
{
    public class Sphere : ISphere
    {
        /// <summary>
        /// x, y, z, R
        /// </summary>
        public int NumberOfVariableValues { get; } = 4;

        /// <summary>
        /// 4/3* Pi* R^3
        /// </summary>
        public double Weight { get; private set; }
        public Enums.ObjectType ObjectType { get; }

        public double Radius { get; private set; }

        public Point Center { get; private set; }

        public Sphere(Point center, double radius, Enums.ObjectType objectType = Enums.ObjectType.Sphere) : this(objectType)
        {
            Center = center ?? new Point();
            Radius = radius;
        }

        public Sphere(double[] data, Enums.ObjectType objectType = Enums.ObjectType.Sphere) : this(objectType)
        {
            Center = new Point(data) ?? new Point();
            Radius = data[3];
        }

        private Sphere(Enums.ObjectType objectType)
        {
            ObjectType = objectType;
            CalculationSphereWeight();
        }

        private void CalculationSphereWeight()
        {
            Weight = ((Func<double>)(() =>
            {
                return 4.0 / 3.0 * Math.PI * Math.Pow(Radius, 3.0);
            })).Invoke();
        }
    }
}
