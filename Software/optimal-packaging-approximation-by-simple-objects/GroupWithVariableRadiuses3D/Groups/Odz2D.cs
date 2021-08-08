// Decompiled with JetBrains decompiler
// Type: hs071_cs.ObjectOptimazation.Odz2D
// Assembly: AdapterLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E82B398F-3AFD-42A4-A941-6E0B182418E2
// Assembly location: H:\Dropbox\диплом\CirclesInCircle\CirclesInCircle\AdapterLibrary.dll

namespace hs071_cs.ObjectOptimazation
{
    internal class Odz2D : OdzBase
    {
        public override double rL { get; set; }

        public override double rU { get; set; }

        public override double xL { get; set; }

        public override double xU { get; set; }

        public override double yL { get; set; }

        public override double yU { get; set; }

        public override double zL { get; set; }

        public override double zU { get; set; }

        public Odz2D()
        {
            rL = rU = xL = xU = yL = yU = zL = zU = 0.0;
        }
    }
}
