using System;

namespace MainProject.Helpers
{
    /// <summary>
    /// Entity instance for concrete dimention
    /// </summary>
    public class Dimension
    {
        public DimensionInstance EntityOfProblem { get; private set; }
        public void CreateInstance(UInt16 dimension)
        {
            EntityOfProblem = DimensionInstance.getInstance(dimension);
        }
    }
}