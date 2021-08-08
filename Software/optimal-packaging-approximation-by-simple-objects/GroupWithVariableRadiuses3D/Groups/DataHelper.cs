using System;
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

        internal void RandomizeCoordinate(ref Balls[] balls, double[] xIter, double[] yIter, double[] zIter, int ballsCount)
        {
            Console.WriteLine("Указать тип рондомизации координат (X,Y,Z)");
            Console.WriteLine("0 --> те, что и были");
            Console.WriteLine("1 --> Рандом(0,1) для всех координат");
            Console.WriteLine("2 --> Рандом для Х");
            Console.WriteLine("3 --> Рнадом для Y");
            Console.WriteLine("4 --> Рандом для Z");

            switch (Console.ReadLine())
            {
                case "0":
                    SetCoordinatesThatWeHadBefore(balls, xIter, yIter, zIter);
                    break;
                case "1":
                    RandomizeAllCoordinates(balls, xIter, yIter, zIter);
                    break;
                case "2":
                    RandomizeXCoordinate(balls, xIter, yIter, zIter);
                    break;
                case "3":
                    RandomizeYCoordinate(balls, xIter, yIter, zIter);
                    break;
                case "4":
                    RandomizeZCoordinate(balls, xIter, yIter, zIter);
                    break;
                default:
                    throw new Exception("Incorrect value");
            }
        }

        private void RandomizeZCoordinate(Balls[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i];
                balls[i].Coordinate.Y = yIter[i];
                balls[i].Coordinate.Z = Random.NextDouble() * zIter[i];
            }
        }

        private void RandomizeYCoordinate(Balls[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i];
                balls[i].Coordinate.Y = Random.NextDouble() * yIter[i];
                balls[i].Coordinate.Z = zIter[i];
            }
        }

        private void RandomizeXCoordinate(Balls[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = Random.NextDouble() * xIter[i];
                balls[i].Coordinate.Y = yIter[i];
                balls[i].Coordinate.Z = zIter[i];
            }
        }

        private void RandomizeAllCoordinates(Balls[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = Random.NextDouble() * xIter[i];
                balls[i].Coordinate.Y = Random.NextDouble() * yIter[i];
                balls[i].Coordinate.Z = Random.NextDouble() * zIter[i];
            }
        }

        private void SetCoordinatesThatWeHadBefore(Balls[] balls, double[] xIter, double[] yIter, double[] zIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Coordinate.X = xIter[i];
                balls[i].Coordinate.Y = yIter[i];
                balls[i].Coordinate.Z = zIter[i];
            }
        }

        internal void RandomizeRadiuses(ref Balls[] balls, double[] rIter, int ballsCount)
        {
            Console.WriteLine("Указать тип рондомизации радиуса");
            Console.WriteLine("0 --> те, что и были");
            Console.WriteLine("1 --> Radius * Рандом(0,1) для всех");
            Console.WriteLine("2 --> Рандомные 0 - 20");

            switch (Console.ReadLine())
            {
                case "0":
                    SetRadiusesThatWeHadBefore(balls, rIter);
                    break;
                case "1":
                    RandomizeRadiuses(balls, rIter);
                    break;
                case "2":
                    RandRadiuses(balls, rIter);
                    break;
                default:
                    throw new Exception("Incorrect value");
            }
        }

        private void RandRadiuses(Balls[] balls, double[] rIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Radius = Random.Next(0, 20);
            }
        }

        private void RandomizeRadiuses(Balls[] balls, double[] rIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Radius = Random.NextDouble() * rIter[i];
            }
        }

        private void SetRadiusesThatWeHadBefore(Balls[] balls, double[] rIter)
        {
            for (int i = 0; i < balls.Length; i++)
            {
                balls[i].Radius = rIter[i];
            }
        }

        internal void SetGroups(Balls[] balls, ref int varRadiuses)
        {
            Console.WriteLine("Укажите кол. элементов в группе 3-15");
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
                Console.WriteLine(ex.Message);
                inGroupElements = 7;
            }

            if (inGroupElements > balls.Length)
            {
                inGroupElements = balls.Length;
            }

            int withFixedRad = balls.Length % inGroupElements;
            int elementsCounter = 0;
            int groupNumber = 1;

            for (int i = 0; i < balls.Length - withFixedRad; i++)
            {
                if (elementsCounter == inGroupElements)
                {
                    ++groupNumber;
                    elementsCounter = 0;
                }

                balls[i].Group = groupNumber;
                ++elementsCounter;
                ++varRadiuses;
            }
        }

        #endregion
    }
}
