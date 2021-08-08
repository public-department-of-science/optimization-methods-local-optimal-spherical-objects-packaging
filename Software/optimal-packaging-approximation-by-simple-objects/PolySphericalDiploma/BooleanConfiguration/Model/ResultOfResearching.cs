using BooleanConfiguration.IO;
using Cureos.Numerics;
using System.Collections.Generic;
using System.Diagnostics;

namespace BooleanConfiguration.Model
{
    public class ResultOfResearching
    {
        /// <summary>
        /// Key => labda;
        /// Value => optional point with time
        /// </summary>
        private Dictionary<double, KeyValuePair<KeyValuePair<double[], Stopwatch>, double>> Result { get; set; }

        private Dictionary<double, IpoptReturnCode> ipoptReturnCode;

        private Dictionary<double, double[]> StartPoint;

        public ResultOfResearching()
        {
            Result = new Dictionary<double, KeyValuePair<KeyValuePair<double[], Stopwatch>, double>>();
            ipoptReturnCode = new Dictionary<double, IpoptReturnCode>();
            StartPoint = new Dictionary<double, double[]>();
        }

        public void AddNewResult(double lambda, KeyValuePair<double[], Stopwatch> keyValues, double functionValue, IpoptReturnCode ipoptReturnCode, double[] startPoint)
        {
            if (Result.ContainsKey(lambda))
            {
                do
                {
                    lambda += 0.01;
                }
                while (Result.ContainsKey(lambda));
            }

            double[] temp = new double[keyValues.Key.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = keyValues.Key[i];
            }

            Result.Add(lambda, new KeyValuePair<KeyValuePair<double[], Stopwatch>, double>(keyValues, functionValue));
            this.ipoptReturnCode.Add(lambda, ipoptReturnCode);
            StartPoint.Add(lambda, startPoint);
        }

        public void ShowAllResults()
        {
            int i = 0;
            foreach (KeyValuePair<double, KeyValuePair<KeyValuePair<double[], Stopwatch>, double>> item in Result)
            {
                Output.ConsolePrint($"Task status {ipoptReturnCode.ToString()} : Lambda = {item.Key}, FunctValue = {item.Value.Value}, Array {item.Value.Key}, Time = {item.Value.Value}");
                ++i;
            }
        }

        public KeyValuePair<KeyValuePair<double[], Stopwatch>, double> GetResultById(double lambda)
        {
            if (Result.ContainsKey(lambda))
            {
                return Result[lambda];
            }
            else
            {
                return new KeyValuePair<KeyValuePair<double[], Stopwatch>, double>();
            }
        }

        public IpoptReturnCode GetTaskStatusById(double lambda)
        {
            if (ipoptReturnCode.ContainsKey(lambda))
            {
                return ipoptReturnCode[lambda];
            }
            else
            {
                return IpoptReturnCode.Internal_Error;
            }
        }

        public double[] GetStartPointById(double lambda)
        {
            if (StartPoint.ContainsKey(lambda))
            {
                return StartPoint[lambda];
            }
            else
            {
                return null;
            }
        }
    }
}
