using PackageProject.Interfaces;
using System.Collections.ObjectModel;

namespace MainProject.Interfaces.InternalObjects.CircularObjects
{
    public interface ICombinedObject
    {
        ObservableCollection<IInternalObject> InternalInCombineObjects { get; }

        double[][] ArrayWithDistances { get; }

        int AmountOfElementsInTheDistanceArray { get; }

        void ComputeDistanceWithObjects();
    }
}
