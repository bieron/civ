using SharpDX;

namespace Civilization.Models
{
    public class Civ
    {
        private Cell capital;
        private string name;
        private Color color;
        private int landCells;
        private long landCellsdt;
        private double strength;
        private double landDesirability;
        private int splitCount;

        public int SplitCount
        {
            get { return splitCount; }
            set { splitCount = value; }
        }

        public string Name
        {
            get { return name; }
        }

        public double Strength
        {
            get { return strength; }
        }

        public int LandCells
        {
            get { return landCells; }
        }

        public long LandCellsdt
        {
            get { return landCellsdt; }
            set { landCellsdt = value; }
        }

        public void GainedCell(Cell cell)
        {
            if (cell.IsReachable)
            {
                landCells++;
                landDesirability += cell.Desirability;
            }
        }

        public void LostCapital()
        {
            capital = MainModel.Instance.GameBoard.PickBestCapitalFor(this);
            if (capital == null)
            {
                System.Console.WriteLine(name + " was destroyed!");
                MainModel.Instance.KillCivilization(this);
            }
        }

        public void LostCell(Cell cell)
        {
            if (cell.IsReachable)
            {
                landCells--;
                landDesirability -= cell.Desirability;
            }
        }

        public void EndOfTick()
        {
            landCellsdt += landCells;
            if (capital.Owner != this)
                LostCapital();
            CalculateStrength();
            if (MainModel.Instance.DoSplits)
            {
                long minSplitThresholdLand = 50000;
                long minSplitThresholdStrength = 1500000;
                long maxSplitThresholdStrength = 5000000;
                if (Strength > minSplitThresholdStrength && landCells > minSplitThresholdLand)
                {
                    double CollapseProbability = (strength - minSplitThresholdStrength) / (maxSplitThresholdStrength - minSplitThresholdStrength);
                    if (MainModel.Instance.Random.NextDouble() < CollapseProbability)
                        MainModel.Instance.SplitCivilization(this);
                }
            }
        }

        public Cell Capital
        {
            get { return capital; }
        }

        public Color Color
        {
            get { return color; }
        }

        public Civ(Cell capital, string name, Color color)
        {
            this.capital = capital;
            this.name = name;
            this.color = color;
            splitCount = 0;
        }

        public Civ(Civ other)
        {
            capital = other.capital;
            name = other.name;
            color = other.color;
        }

        public void SettleCiv()
        {
            this.capital.Owner = this;
            GainedCell(capital);
        }

        public void CalculateStrength()
        {
            strength = landCells * (landDesirability/landCells) + 0.1 * landCellsdt;
            strength /= (splitCount+1)*(splitCount+1);
        }
    }
}