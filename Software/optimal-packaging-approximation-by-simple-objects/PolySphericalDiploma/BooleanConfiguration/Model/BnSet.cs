using BooleanConfiguration.Interfaces;

namespace BooleanConfiguration.Model
{
    internal class BnSet : IBnSet
    {
        public int M1 { get; set; }
        public int M2 { get; set; }
        public string MatrixX0 { get; set; }

        public BnSet(int N)
        {
            string localSet = "0000000001111111111111111111111111111111111111111111";
            if (N >= localSet.Length)
            {
                throw new System.Exception($"Value of variables too big! Generated N = {N} You can use {localSet.Length} max.");
            }

            MatrixX0 = localSet.Substring(0, N);
            M1 = MatrixX0.Contains("1").ToString().Length;
            M2 = 5;
        }
    }
}
