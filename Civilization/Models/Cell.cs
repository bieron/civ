using System.Collections.Generic;

namespace Civilization.Models
{
    internal class Cell
    {
        private List<Cell> neighbors;
        private double desirability, defensibility;
        private Dictionary<Civ, double> capitalDistance;
        private Civ owner;
        private Civ newOwner;
        private int xCoord, yCoord; //będzie to potrzebne m.in. do stolic
        private bool reachable; //przyda się do odległości od stolicy
        private static double waterCost = 5.0;

        public List<Cell> Neighbors
        {
            get { return neighbors; }
        }

        public double Desirability
        {
            get { return desirability; }
            set { desirability = value; }
        }

        public Civ Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public Civ NewOwner
        {
            get { return newOwner; }
            set { newOwner = value; }
        }

        public bool IsReachable
        {
            get { return reachable; }
            set { reachable = value; }
        }

        public int X
        {
            get { return xCoord; }
            set { xCoord = value; }
        }

        public int Y
        {
            get { return yCoord; }
            set { yCoord = value; }
        }

        public Cell(int x, int y)
        {
            owner = null;
            capitalDistance = new Dictionary<Civ, double>();
            neighbors = new List<Cell>();
            xCoord = x;
            yCoord = y;
            reachable = true;
        }

        public void CalculateNewOwner()
        {
            if (!reachable)
                newOwner = null;
            //to tylko testy
            foreach (Cell neighbour in neighbors)
            {
                if (neighbour.Owner != null && owner == null)
                    if (MainModel.Instance.Random.Next(255) <= desirability + 5)
                    {
                        newOwner = neighbour.Owner;
                        return;
                    }
            }
            newOwner = owner;
        }

        public void ChangeOwner()
        {
            owner = newOwner;
            newOwner = null;
        }

        public void AddNeighbour(Cell neighbour)
        {
            neighbors.Add(neighbour);
        }

        public double DistanceToCapitalOf(Civ empire)
        {
            if (!capitalDistance.ContainsKey(empire))
                AddCapital(empire); /*chyba najlepiej będzie jednak liczyć te odległości run-time
                                      *w takim wypadku nigdy nie policzymy odległości kratki ze szkocji do Kairu
                                      *(który w sumie nie był stolicą starożytnego Egiptu :P)
                                      *a i tak raz policzona odległość będzie się zapisywać*/
            double distance;
            if (capitalDistance.TryGetValue(empire, out distance))
                return distance;
            else
            {
                throw new System.Exception("Nie udało się uzyskać wartości ze słownika");
            }
        }

        public void AddCapital(Civ empire)
        {
            double dist = DistanceTo(empire.Capital);
            capitalDistance.Add(empire, dist);
        }

        public void RemoveCapital(Civ empire)
        {
            capitalDistance.Remove(empire);
        }

        public double DistanceTo(Cell cell)
        {
            //tu można zmienić metodę liczenia dystansu
            return DistanceCalculator.calculateDistance(this, cell, DistanceCalculator.CalculationType.CT_PYTHAGORIAN2);
        }

        public void SetUnreachable()
        {
            reachable = false;
        }
    }
}