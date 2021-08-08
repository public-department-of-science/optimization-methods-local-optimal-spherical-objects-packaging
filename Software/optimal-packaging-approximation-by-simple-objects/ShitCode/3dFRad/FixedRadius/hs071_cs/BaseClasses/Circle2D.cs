using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hs071_cs.ObjectOptimazation
{
  class Circle2D: CircleBase
  {
    public CoordinateBase Coordinate { get; set; }
    public OdzBase Odz { get; set; }

    public Circle2D()
    {
      Coordinate = new Coordinate2D();
      Odz = new Odz2D();
    }

  }
}
