using MainProject.Model;

namespace MainProject.Interfaces
{
    public interface IСircularContainer : IContainer
    {
        double Radius { get; }

        Point CenterOfTheContainer { get; }
    }
}
