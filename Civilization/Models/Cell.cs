using System.Collections.Generic;

namespace Civilization.Models {
    class Cell {
        List<Cell> neighbors;
        double desirability, defensibility;
        Dictionary<Civ, double> capitalDistance;
        Civ owner;
        Civ newOwner;
        int xCoord, yCoord; //będzie to potrzebne m.in. do stolic
        bool reachable; //przyda się do odległości od stolicy
        static double waterCost = 5.0;

        public Cell(int x, int y, bool reachable) {
            owner = null;
            capitalDistance = new Dictionary<Civ, double>();
            neighbors = new List<Cell>();
            xCoord = x;
            yCoord = y;
            this.reachable = reachable;
        }

        public void calculateNewOwner() {
            //TransitionAlgorithm wchodzi do gry
        }

        public void changeOwner() {
            owner = newOwner;
        }

        public void addNeighbour(Cell neighbour) {
            neighbors.Add(neighbour);
        }

        public List<Cell> getNeighbours() {
            return neighbors;
        }

        public int getX() {
            return xCoord;
        }

        public int getY() {
            return yCoord;
        }

        public bool isReachable() {
            return reachable;
        }

        public double distanceToCapitalOf(Civ empire) {
            if (!capitalDistance.ContainsKey(empire))
                addCapital(empire);  /*chyba najlepiej będzie jednak liczyć te odległości run-time
                                      *w takim wypadku nigdy nie policzymy odległości kratki ze szkocji do Kairu
                                      *(który w sumie nie był stolicą starożytnego Egiptu :P)
                                      *a i tak raz policzona odległość będzie się zapisywać*/
            double distance;
            if (capitalDistance.TryGetValue(empire, out distance))
                return distance;
            else {
                throw new System.Exception("Nie udało się uzyskać wartości ze słownika");
            }
        }

        public void addCapital(Civ empire) {
            double dist = distanceTo(empire.getCapital());
            capitalDistance.Add(empire, dist);
        }

        public void removeCapital(Civ empire) {
            capitalDistance.Remove(empire);
        }

        public double distanceTo(Cell cell) {
            //tu można zmienić metodę liczenia dystansu
            return DistanceCalculator.calculateDistance(this, cell, DistanceCalculator.CalculationType.CT_ASTAR);
        }
    }
}
