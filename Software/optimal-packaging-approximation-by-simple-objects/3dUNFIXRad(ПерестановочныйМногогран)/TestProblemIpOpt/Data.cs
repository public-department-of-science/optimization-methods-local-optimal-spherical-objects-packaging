namespace hs071_cs
{
    public enum ObjectType
    {
        Undefined = 0, // type not undefined
        ExternalObject = 1, // external ball (include balls less radius)
        ProhibitionZone = 2, // prohibition zona which not moving in internal region
        SimpleObject = 3, // usual ball
        CompositeObject = 4 //a few ball united in one composite object
    }

    public class Ball
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double R { get; set; }
        public double Weight { get; set; }
        public ObjectType ObjectType { get; set; }

        public Ball()
        {
            ObjectType = 0;
        }
    }

    public class Data
    {
        public int ballCount;
        public int holeCount;
        public Ball[] ball;
        public double[,] C; // матрица связей
        public double R;

        private Data()
        {
        }
        private Data(int circleCount)
        {
            ball = new Ball[circleCount];
            for (int i = 0; i < ball.Length; i++)
            {
                ball[i] = new Ball();
            }
            C = new double[circleCount, circleCount];
        }
        public Data(double[] x, double[] y, double[] z, double[] r, double R, int ballCount,int holeCount, ObjectType[] type = null, double[] Weight = null, double[,] C = null) : this(ballCount)
        {
            this.R = R;
            this.ballCount = ballCount;
            this.holeCount = holeCount;
            for (int i = 0; i < ball.Length; i++)
            {
                ball[i].X = x[i];
                ball[i].Y = y[i];
                ball[i].Z = z[i];
                ball[i].R = r[i];
                ball[i].ObjectType = (type != null) ? type[i] : (ObjectType)(-1);
                ball[i].Weight = (Weight != null) ? Weight[i] : 0;
            }
            this.C = C ?? null;
        }
    }
}