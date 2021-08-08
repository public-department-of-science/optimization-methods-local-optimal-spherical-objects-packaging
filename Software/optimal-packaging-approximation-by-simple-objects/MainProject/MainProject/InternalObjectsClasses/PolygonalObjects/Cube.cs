using MainProject.Enums;
using PackageProject.Interfaces.InternalObjects.PolygonalObjects;
using System;
using MainProject.Model;

namespace PackageProject.InternalObjectsClasses.PolygonalObjects
{
    internal class Cube : ICube
    {
        public double FaceOfTheCube { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int NumberOfVariableValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Point[] Points { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Weight => throw new NotImplementedException();

        public Enums.ObjectType ObjectType => throw new NotImplementedException();
    }
}
