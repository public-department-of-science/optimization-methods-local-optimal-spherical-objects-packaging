using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.Helpers
{
    /// <summary>
    /// Settings of dimention for optimization problem; Create <Single> instance for one Thread 
    /// </summary>
    public class DimensionInstance
    {
        private static DimensionInstance instance;

        private static object syncRoot = new Object();
        public UInt16 SettedDimension { get; private set; }

        DimensionInstance(UInt16 settedDimension)
        {
            SettedDimension = settedDimension;
        }

        public static DimensionInstance getInstance(UInt16 settedDimension)
        {
            if (instance == null)
                lock (syncRoot)
                {
                    if (settedDimension > 1 && settedDimension < 4)
                        instance = new DimensionInstance(settedDimension);
                    else
                        instance = new DimensionInstance(2);
                }
            return instance;
        }
    }
}