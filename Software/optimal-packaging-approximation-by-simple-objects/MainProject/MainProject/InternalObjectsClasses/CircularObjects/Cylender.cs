using MainProject.Enums;
using PackageProject.Interfaces;
using System;
using MainProject.Model;

namespace PackageProject.InternalObjectsClasses.CircularObjects
{
    internal class Cylender : ICylinder
    {
        public Point LowerBaseCenter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Point UpperBaseCenter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Height { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Radius { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int NumberOfVariableValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Weight => throw new NotImplementedException();

        public Enums.ObjectType ObjectType => throw new NotImplementedException();
    }
}
