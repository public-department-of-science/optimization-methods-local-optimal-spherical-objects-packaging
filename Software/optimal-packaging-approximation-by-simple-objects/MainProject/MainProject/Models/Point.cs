namespace MainProject.Model
{
    public class Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(double[] ArrayWithPointData)
        {
            if (ArrayWithPointData.Length < 3 || ArrayWithPointData.Length > 4)
            {
                throw new System.Exception($"Cast array to Point error {nameof(ArrayWithPointData)}");
            }

            X = ArrayWithPointData[0];
            Y = ArrayWithPointData[1];
            Z = ArrayWithPointData[2];
        }

        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
    }
}
