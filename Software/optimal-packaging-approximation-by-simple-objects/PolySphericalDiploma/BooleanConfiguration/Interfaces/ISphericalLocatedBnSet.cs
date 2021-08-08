namespace BooleanConfiguration.Interfaces
{
    internal interface ISphericalLocatedBnSet : ISet
    {
        /// <summary>
        /// Sum left bound of Bn => When TypeOfSet = 2
        /// </summary>
        int M1 { get; set; }
    }
}
