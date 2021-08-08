using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hs071_cs.ObjectOptimazation
{
  class Odz2D:OdzBase
  {
    public override double rL { get; set; }
    public override double rU { get; set; }
    public override double xL { get; set; }
    public override double xU { get; set; }
    public override double yL { get; set; }
    public override double yU { get; set; }

    public Odz2D()
    {
      rL = rU = xL = xU = yL = yU = 0;
    }
  }
}
