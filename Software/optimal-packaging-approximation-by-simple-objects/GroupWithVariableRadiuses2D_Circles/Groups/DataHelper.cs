using System;
using System.Linq;
using hs071_cs;
using hs071_cs.ObjectOptimazation;

namespace Groups
{
    public class DataHelper
    {
        private static readonly Random Random;

        static DataHelper()
        {
            Random = new Random();
        }

        #region CoordinatesRegions

        internal void RandomizeCoordinate(ref Circle[] balls, double[] xIter, double[] yIter, double[] zIter, int ballsCount)
        {
            Program.Print("********************************************************");
            Program.Print("----Указать тип рондомизации координат (X,Y)----".ToUpper());
            Program.Print("-- 1 --> те, что и были");
            Program.Print("-- 2 --> Рандом(0,1) для всех координат");
            Program.Print("-- 3 --> Рандом на интервале (K,P) для всех координат целыми числами");
            Program.Print("________________________________");
            Program.Print("-- 4 --> Рандом для Х на (0,1)");
            Program.Print("-- 5 --> Рандом для Х на (K,P) * x[i]");
            Program.Print("-- 6 --> Совершенно рандомные координаты для Х на интервале (K,P)");
            Program.Print("________________________________");
            Program.Print("-- 7 --> Рнадом для Y на (0,1)");
            Program.Print("-- 8 --> Рнадом для Y на (K,P) * y[i]");
            Program.Print("-- 9 --> Совершенно рандомные координаты для Х на интервале (K,P)");

            switch (Console.ReadLine())
            {
                case "1":
                    SetCoordinatesThatWeHadBefore(balls, xIter, yIter, zIter);
                    break;
                case "2":
                    RandomizeAllCoordinatesOne0_1Interval(balls, xIter, yIter, zIter);
                    break;
                case "3":
                    RandomizeAllCordinatesOnCustomInterval(balls, xIter, yIter, zIter);
                    break;
                case "4":
                    RandomizeXCoordinateOn0_1Interval(balls, xIter, yIter, zIter);
                    break;
                case "5":
                    RandomizeXCoordinateOnCustomIntervalMultOnPreviousOptionalYCoordinate(balls, xIter, yIter, zIter);
                    break;
                case "6":
                    RandomizeAllXCoordinateWithRandomValues(balls, xIter, yIter, zIter);
                    break;
                case "7":
                    RandomizeYCoordinateOn0_1Interval(balls, xIter, yIter, zIter);
                    break;
                case "8":
                    RandomizeYCoordinateOnCustomIntervalMultOnPreviousOptionalYCoordinate(balls, xIter, yIter, zIter);
                    break;
                case "9":
                    RandomizeAllYCoordinateWithRandomValues(balls, xIter, yIter, zIter);
                    break;
                default:
                    throw new Exception("Incorrect value");
            }
            Program.Print("********************************************************");
        }

        private void RandomizeAllYCoordinateWithRandomValues(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            SetLeftAndRightBounds(out int leftBound, out int rightBound);
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i];
                balls[i].Coordinate.Y = Random.Next(leftBound, rightBound);
            }
        }

        private void RandomizeAllXCoordinateWithRandomValues(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            SetLeftAndRightBounds(out int leftBound, out int rightBound);
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = Random.Next(leftBound, rightBound);
                balls[i].Coordinate.Y = yIter[i];
            }
        }

        private void RandomizeYCoordinateOnCustomIntervalMultOnPreviousOptionalYCoordinate(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            SetLeftAndRightBounds(out int leftBound, out int rightBound);
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i];
                balls[i].Coordinate.Y = Random.Next(leftBound, rightBound) * yIter[i];
            }
        }

        private void RandomizeXCoordinateOnCustomIntervalMultOnPreviousOptionalYCoordinate(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            SetLeftAndRightBounds(out int leftBound, out int rightBound);
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = Random.Next(leftBound, rightBound) * xIter[i];
                balls[i].Coordinate.Y = yIter[i];
            }
        }

        private void RandomizeAllCordinatesOnCustomInterval(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            SetLeftAndRightBounds(out int leftBound, out int rightBound);
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i] * Random.Next(leftBound, rightBound);
                balls[i].Coordinate.Y = yIter[i] * Random.Next(leftBound, rightBound);
            }
        }

        private void RandomizeYCoordinateOn0_1Interval(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i];
                balls[i].Coordinate.Y = Random.NextDouble() * yIter[i];
            }
        }

        private void RandomizeXCoordinateOn0_1Interval(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = Random.NextDouble() * xIter[i];
                balls[i].Coordinate.Y = yIter[i];
            }
        }

        private void RandomizeAllCoordinatesOne0_1Interval(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = Random.NextDouble() * xIter[i];
                balls[i].Coordinate.Y = Random.NextDouble() * yIter[i];
            }
        }

        private void SetCoordinatesThatWeHadBefore(Circle[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i];
                balls[i].Coordinate.Y = yIter[i];
            }
        }

        internal void RandomizeRadiuses(ref Circle[] balls, double[] rIter, int ballsCount)
        {
            Program.Print("********************************************************");
            Program.Print("--- Указать тип рондомизации радиуса----".ToUpper());
            Program.Print("-- 1 --> Оставить те, что и были");
            Program.Print("-- 2 --> Radius * Рандом(0,1) для всех");
            Program.Print("-- 3 --> Рандомные на интервале (K,P)");
            Program.Print("-- 4 --> \"Центроид по всем раиусам\" + Рoзмах по всем радиусам * {Rnd(0,1) - 0.5}");
            Program.Print("-- 5 --> \"Центроид по группе\" + Рoзмах по группе * {Rnd(0,1) - 0.5}");
            Program.Print("-- 6--> Все радиусы равны \"Центроиду по всей системе!\"");
            Program.Print("-- 7 --> Все радиусы равны \"Центроиду по каждой греппе свой!\"");

            switch (Console.ReadLine())
            {
                case "1":
                    SetRadiusesThatWeHadBefore(balls, rIter);
                    break;
                case "2":
                    RandomizeRadiusesOn0_1Interval(balls, rIter);
                    break;
                case "3":
                    RandRadiuses(balls, rIter);
                    break;
                case "4":
                    RandRadiusesRangeMultRndMinus05OnFullSystem(balls, rIter);
                    break;
                case "5":
                    RandRadiusesRangeMultRndMinus05OnEachGroupBased(balls, rIter);
                    break;
                case "6":
                    RadiusesEqualCentroidOnFullSystem(balls, rIter);
                    break;
                case "7":
                    RadiusesEqualCentroidBasedOnEachGroup(balls, rIter);
                    break;
                default:
                    throw new Exception("Incorrect value");
            }
            Program.Print("********************************************************");
        }

        private void RandRadiusesRangeMultRndMinus05OnEachGroupBased(Circle[] balls, double[] rIter)
        {
            foreach (IGrouping<int, Circle> item in balls.GroupBy(x => x.Group))
            {
                double maxRadius = item.Max(y => y.Radius);
                double minRadius = item.Min(y => y.Radius);

                double centroid = (maxRadius + minRadius) / 2;

                double range = maxRadius - minRadius;

                for (int i = 0; i < balls.Length; i++)
                {
                    if (NotFixedGroup(balls, i) && item.FirstOrDefault().Group != 0)
                    {
                        balls[i].Radius = centroid + range * (Random.NextDouble() - 0.5);
                    }
                }
            }
        }

        private void RadiusesEqualCentroidBasedOnEachGroup(Circle[] balls, double[] rIter)
        {
            foreach (IGrouping<int, Circle> item in balls.GroupBy(x => x.Group))
            {
                double maxRadius = item.Max(y => y.Radius);
                double minRadius = item.Min(y => y.Radius);

                double centroid = (maxRadius + minRadius) / 2;

                for (int i = 0; i < balls.Length; i++)
                {
                    if (NotFixedGroup(balls, i) && item.FirstOrDefault().Group != 0)
                    {
                        balls[i].Radius = centroid;
                    }
                }
            }
        }

        private void RadiusesEqualCentroidOnFullSystem(Circle[] balls, double[] rIter)
        {
            double centroid = (rIter[rIter.Length - 1] + rIter[0]) / 2;

            for (int i = 0; i < balls.Length; i++)
            {
                if (NotFixedGroup(balls, i))
                {
                    balls[i].Radius = centroid;
                }
            }
        }

        private void RandRadiusesRangeMultRndMinus05OnFullSystem(Circle[] balls, double[] rIter)
        {
            double centroid = (rIter[rIter.Length - 1] + rIter[0]) / 2;
            double range = (rIter[rIter.Length - 1] - rIter[0]) / 2;

            for (int i = 0; i < balls.Length; i++)
            {
                if (NotFixedGroup(balls, i))
                {
                    balls[i].Radius = centroid + range * (Random.NextDouble() - 0.5);
                }
            }
        }

        private void RandRadiuses(Circle[] balls, double[] rIter)
        {
            SetLeftAndRightBounds(out int leftBound, out int rightBound);

            if (leftBound < 0)
            {
                leftBound = 0;
            }

            for (int i = 0; i < balls.Length; i++)
            {
                if (NotFixedGroup(balls, i))
                {
                    balls[i].Radius = Random.Next(leftBound, rightBound);
                }
            }
        }

        private static bool NotFixedGroup(Circle[] balls, int i)
        {
            return balls[i].Group != 0;
        }

        private static void SetLeftAndRightBounds(out int leftBound, out int rightBound)
        {
            leftBound = 0;
            rightBound = 20;
            try
            {
                Program.Print("Enter Left bound:");
                leftBound = int.Parse(Console.ReadLine());
                Program.Print("Enter rigth bound:");
                rightBound = int.Parse(Console.ReadLine());
                if (leftBound > rightBound)
                {
                    int temp = leftBound;
                    leftBound = rightBound;
                    rightBound = temp;
                }
            }
            catch (Exception)
            {
                Program.Print($"some parsing error occurred Left{leftBound}; RightBound {rightBound}");
            }
        }

        private void RandomizeRadiusesOn0_1Interval(Circle[] balls, double[] rIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                if (NotFixedGroup(balls, i))
                {
                    balls[i].Radius = Random.NextDouble() * rIter[i];
                }
            }
        }

        private void SetRadiusesThatWeHadBefore(Circle[] balls, double[] rIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Radius = rIter[i];
            }
        }

        internal void SetGroups(ref Circle[] circles, ref int varRadiuses)
        {
            Program.Print("********************************************************");
            Program.Print("In which way would you like to set group numbers?");
            Program.Print(" --1 --> Без изменений (как указаны выше)");
            Program.Print(" --2 --> Нарезать по порядку по N штук");
            Program.Print(" --3 --> Все фиксированные");

            switch (Console.ReadLine())
            {
                case "1":
                    break;
                case "2":
                    SliceCircles(circles, ref varRadiuses);
                    break;
                case "3":
                    SetAllCirclesLikeFixed(circles);
                    break;
                default:
                    break;
            }

            ShowHowToCirlesDistributedByGroups(circles);
        }

        private void ShowHowToCirlesDistributedByGroups(Circle[] circles)
        {
            foreach (IGrouping<int, Circle> item in circles.GroupBy(x => x.Group))
            {
                Program.Print($"В группе #{item.FirstOrDefault().Group} {item.Count()} кругов");
            }
        }

        private void SliceCircles(Circle[] circles, ref int varRadiuses)
        {
            varRadiuses = 0;
            Program.Print("Укажите кол. элементов в группе 3-15".ToUpper());
            int inGroupElements = 0;

            try
            {
                inGroupElements = int.Parse(Console.ReadLine());

                if (inGroupElements < 3 || inGroupElements > 15)
                {
                    inGroupElements = 7;
                }
            }
            catch (Exception ex)
            {
                Program.Print(ex.Message);
                inGroupElements = 7;
            }

            Program.Print("********************************************************");

            if (inGroupElements > circles.Length)
            {
                inGroupElements = circles.Length;
            }

            int withFixedRad = circles.Length % inGroupElements;
            int elementsCounter = 0;
            int groupNumber = 1;

            for (int i = 0; i < circles.Length - withFixedRad; i++)
            {
                if (elementsCounter == inGroupElements)
                {
                    ++groupNumber;
                    elementsCounter = 0;
                }

                circles[i].Group = groupNumber;
                ++elementsCounter;
                ++varRadiuses;
            }
        }

        private void SetAllCirclesLikeFixed(Circle[] circles)
        {
            foreach (Circle item in circles)
            {
                item.Group = 0;
            }
        }

        #endregion
    }
}
