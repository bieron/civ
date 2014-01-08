using System;
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

        public void CalculateNewOwner(Random random = null)
        {
            if (random == null)
                random = MainModel.Instance.Random;

            foreach (Cell neighbour in neighbors)
            {
                if (neighbour.Owner != null)
                    if (owner == null)
                    {
                        if (!reachable)
                        {
                            if (random.Next(255) <= desirability + 10)
                            {
                                newOwner = neighbour.Owner;
                                return;
                            }
                        }
                        else if (random.Next(255) <= desirability + 5)
                        {
                            newOwner = neighbour.Owner;
                            return;
                        }
                    }
                    else
                    {
                        if (neighbour.Owner != owner)
                        {
                            double thisStrength = owner.Strength * (Math.Max(((MainModel.Instance.GameBoard.Width + MainModel.Instance.GameBoard.Height)/10 - DistanceToCapitalOf(owner) ), 35)/ (MainModel.Instance.GameBoard.Width + MainModel.Instance.GameBoard.Height));
                            double neighbourStrength = neighbour.Owner.Strength * (Math.Max(((MainModel.Instance.GameBoard.Width + MainModel.Instance.GameBoard.Height)/10 - DistanceToCapitalOf(neighbour.Owner)), 35) / (MainModel.Instance.GameBoard.Width + MainModel.Instance.GameBoard.Height));
                            if (reachable)
                            {
                                double db = random.NextDouble();
                                if (db <= 0.05 * (neighbourStrength / (thisStrength + neighbourStrength)))
                                {
                                    if (db == 0)
                                        System.Console.WriteLine(db);
                                    newOwner = neighbour.Owner;
                                    return;
                                }
                                else
                                {
                                    
                                }
                            }
                            else
                            {
                                if (random.NextDouble() <= 0.01 * (neighbourStrength / (thisStrength + neighbourStrength)))
                                {
                                    newOwner = neighbour.Owner;
                                    return;
                                }
                            }
                        }
                    }
            }
            newOwner = owner;
        }

        public void ChangeOwner()
        {
            if (owner == newOwner)
            {
                newOwner = null;
                return;
            }
            if (owner != null)
                owner.lostCell(this);
            if (newOwner != null)
                newOwner.gainedCell(this);
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
            if (cell == null)
                return (MainModel.Instance.GameBoard.Width + MainModel.Instance.GameBoard.Height);
            //tu można zmienić metodę liczenia dystansu
            return DistanceCalculator.calculateDistance(this, cell, DistanceCalculator.CalculationType.CT_PYTHAGORIAN);
        }

        public void SetUnreachable()
        {
            reachable = false;
        }
    }
}