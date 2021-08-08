using MainProject.Model;

namespace PackageProject.Interfaces
{
    internal interface ICone : IObjectHasCircleInStructure, IInternalObject
    {
        double Height { get; set; }

        Point LowerBaseCenter { get; set; }
    }
}
