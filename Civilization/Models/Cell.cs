using System.Collections.Generic;

namespace Civilization.Models {
    class Cell {
        List<Cell> neighbors;
        double desirability, defensibility;
        Dictionary<Civ, double> capitalDistance;
        Civ owner;
    }
}
