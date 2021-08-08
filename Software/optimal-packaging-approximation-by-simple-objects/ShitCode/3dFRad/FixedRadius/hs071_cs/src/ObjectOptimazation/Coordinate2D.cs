using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hs071_cs.ObjectOptimazation
{
  class Coordinate2D:CoordinateBase
  {
    public override double X { get; set; }
    public override double Y { get; set; }

    public Coordinate2D()
    {
      X = Y = 0;
    }
  }
}
