using MainProject.Model;

namespace MainProject.Interfaces
{
    public interface IParallelogramContainer : IContainer
    {
        Point[] ParallelogramPoints { get; set; }
    }
}
