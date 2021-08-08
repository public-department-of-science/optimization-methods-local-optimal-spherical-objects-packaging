using static MainProject.Enums.Enums;

namespace PackageProject.Interfaces
{
    public interface IInternalObject
    {
        /// <summary>
        /// Amount of variables for one internal object (for fix radius sphere it's 3)
        /// </summary>
        int NumberOfVariableValues { get; }

        /// <summary>
        /// value of Object Weight 
        /// </summary>
        double Weight { get; }

        /// <summary>
        /// Type of object selected according to ObjectType enum
        /// </summary>
        ObjectType ObjectType { get; }
    }
}
