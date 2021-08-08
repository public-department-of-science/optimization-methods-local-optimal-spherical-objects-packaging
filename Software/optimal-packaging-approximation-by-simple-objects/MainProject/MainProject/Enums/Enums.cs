namespace MainProject.Enums
{
    public class Enums
    {
        public enum ObjectType
        {
            Undefined = 0, // type not undefined
            Circle = 1,
            CombinedObject = 2, //a few ball united in one composite object
            Sphere = 3,
            SphereWithVariableRadius = 4,
            Cone = 5,
            Cylender = 6,
            Cube = 7,
            Parallepiped = 8,
            Prism = 9
        }

        //    ProhibitionZone = 1, // prohibition zone which not moving
        public enum TaskClassification
        {
            FixedRadiusTask = 0, // task type
            VariableRadius = 1 // task type
        }

        #region 

        public enum SelectBallsWithVariableRadiusScheme
        {
            Random = 0,
            EvenBalls = 1,
            OddBalls = 2
        }

        #endregion
    }
}
