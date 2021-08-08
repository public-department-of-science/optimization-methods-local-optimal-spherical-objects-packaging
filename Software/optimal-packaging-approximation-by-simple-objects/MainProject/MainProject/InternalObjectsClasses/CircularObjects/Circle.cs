using MainProject.Interfaces.InternalObjects.CircularObjects;
using System;
using MainProject.Model;
using static MainProject.Enums.Enums;

namespace MainProject.InternalObjectsClasses.CircularObjects
{
    public class Circle : ICircle
    {
        /// <summary>
        /// x, y, R
        /// </summary>
        public int NumberOfVariableValues { get; private set; }

        /// <summary>
        /// Square
        /// </summary>
        public double Weight { get; private set; }

        public double Radius { get; private set; }

        /// <summary>
        /// Center of circle
        /// </summary>
        public Point Center { get; private set; }

        public ObjectType ObjectType { get; private set; }

        public Circle(Point centerPoint, double radius)
        {
            Center = centerPoint ?? new Point();
            Radius = radius;
            ObjectType = ObjectType.Circle;
            NumberOfVariableValues = 3;

            Weight = ((Func<double>)(() =>
            {
                return Math.PI * Math.Pow(radius, 2.0);
            })).Invoke();
        }
    }
}
