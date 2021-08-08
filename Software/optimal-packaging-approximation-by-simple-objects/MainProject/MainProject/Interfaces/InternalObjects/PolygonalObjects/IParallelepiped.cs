namespace PackageProject.Interfaces.InternalObjects.PolygonalObjects
{
    internal interface IParallelepiped : IPolygonalObject, IInternalObject
    {
        double Height { get; set; }
    }
}
