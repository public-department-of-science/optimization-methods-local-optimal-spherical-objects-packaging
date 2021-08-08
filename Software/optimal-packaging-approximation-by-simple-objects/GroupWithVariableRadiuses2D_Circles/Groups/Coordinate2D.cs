// Decompiled with JetBrains decompiler
// Type: hs071_cs.ObjectOptimazation.Coordinate2D
// Assembly: AdapterLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E82B398F-3AFD-42A4-A941-6E0B182418E2
// Assembly location: H:\Dropbox\диплом\CirclesInCircle\CirclesInCircle\AdapterLibrary.dll

namespace hs071_cs.ObjectOptimazation
{
    internal class Coordinate2D : CoordinateBase
    {
        public override double X { get; set; }

        public override double Y { get; set; }

        public Coordinate2D()
        {
             X =  Y = 0.0;
        }
    }
}
