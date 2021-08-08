namespace BooleanConfiguration.Interfaces
{
    internal interface IBnSet : ISet
    {
        /// <summary>
        /// Sum left bound of Bn => When TypeOfSet = 2
        /// </summary>
        int M1 { get; set; }

        /// <summary>
        /// Sum right bound of Bn => When TypeOfSet = 2
        /// </summary>
        int M2 { get; set; }
    }
}
