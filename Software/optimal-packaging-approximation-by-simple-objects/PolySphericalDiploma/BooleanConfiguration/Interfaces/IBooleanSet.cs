namespace BooleanConfiguration.Interfaces
{
    public interface IBooleanSet : ISet
    {
        /// <summary>
        /// Sum of 1 When TypeOfSet = 1
        /// </summary>
        int M { get; set; }
    }
}
