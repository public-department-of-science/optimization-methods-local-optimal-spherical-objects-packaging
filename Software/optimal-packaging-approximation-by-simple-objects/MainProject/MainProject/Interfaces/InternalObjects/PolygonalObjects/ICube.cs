namespace PackageProject.Interfaces.InternalObjects.PolygonalObjects
{
    internal interface ICube : IInternalObject, IPolygonalObject
    {
        /// <summary>
        /// must be equal for all similar
        /// </summary>
        double FaceOfTheCube { get; set; }
    }
}
