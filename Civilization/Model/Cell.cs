using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization.Model {
    class Cell {
        List<Cell> neighbors;
        double desirability, defensibility;
        Dictionary<Civ, double> capitalDistance;
        Civ owner;
    }
}
