// Decompiled with JetBrains decompiler
// Type: hs071_cs.ObjectOptimazation.Circle2D
// Assembly: AdapterLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E82B398F-3AFD-42A4-A941-6E0B182418E2
// Assembly location: H:\Dropbox\диплом\CirclesInCircle\CirclesInCircle\AdapterLibrary.dll

namespace hs071_cs.ObjectOptimazation
{
    public class Circle : CircleBase
    {
        public CoordinateBase Coordinate { get; set; }

        public OdzBase Odz { get; set; }

        public Circle()
        {
             Coordinate = new Coordinate2D();
             Odz = new Odz2D();
        }
    }
}
