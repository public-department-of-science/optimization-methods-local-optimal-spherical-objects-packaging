using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hs071_cs.ObjectOptimazation
{
  abstract class OdzBase
  {
    public abstract double rL { get; set; }
    public abstract double rU { get; set; }
    public abstract double xL { get; set; }
    public abstract double xU { get; set; }
    public abstract double yL { get; set; }
    public abstract double yU { get; set; }
  }
}
