namespace MainProject.Helpers
{
    // bad solution, late be something else
    public class IpoptConfig
    {
        public double Precision { get; set; }

        public string Strategy { get; set; }

        public string HessianSettings { get; set; }

        public int MaxIteration { get; set; }

        public int PrintInfoToConsoleLevel { get; set; }

        public int PrintInfoToFileLevel { get; set; }
    }
}
