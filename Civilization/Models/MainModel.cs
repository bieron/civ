using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization.Models
{
    //nie wiedziałem jak to nazwać, można to zmienić
    internal class MainModel
    {
        private static MainModel instance;

        public static MainModel Instance
        {
            get { return instance ?? (instance = new MainModel()); }
        }

        private ColorDistributor colors;
        private List<Civ> newCivilizations;
        private List<Civ> deadCivilizations;
        private List<Civ> civilizations;
        private Board gameBoard;
        private Random random;
        private int drawSpeed;
        private long tickCount;
        private long last100TicksMilliseconds;
        private Stopwatch sw;

        public int DrawSpeed
        {
            get { return drawSpeed; }
        }

        public Board GameBoard
        {
            get { return gameBoard; }
        }

        public Random Random
        {
            get { return random; }
        }

        public List<Civ> Civilizations
        {
            get { return civilizations; }
        }

        protected MainModel()
        {
            System.Console.WriteLine("MainModel begin");
            gameBoard = new Board("EgyptMap");
            civilizations = new List<Civ>();
            deadCivilizations = new List<Civ>();
            newCivilizations = new List<Civ>();
            random = new System.Random();
            drawSpeed = 1;
            tickCount = 0;
            colors = new ColorDistributor();
            //tests
            civilizations.Add(new Civ(gameBoard.Cells[39][21], "Greek Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[117][181], "Egyptian Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[221][137], "Jewish Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[264][583], "Some African Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[230][39], "Persian Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[169][353], "Kings of the Desert Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[409][522], "Arabian Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[417][109], "Mongol Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[161][80], "Cyprus Empire", colors.GetNextColor()));
            civilizations.Add(new Civ(gameBoard.Cells[12][157], "Moors Empire", colors.GetNextColor()));

            sw=new Stopwatch();
            sw.Start();
        }

        public void KillCivilization(Civ empire)
        {
            deadCivilizations.Add(empire);
            colors.AddColor(empire.Color);
        }

        public void endOfTick()
        {
            foreach (Civ civ in newCivilizations)
                civilizations.Add(civ);
            newCivilizations.Clear();
            foreach (Civ civ in deadCivilizations)
                civilizations.Remove(civ);
            deadCivilizations.Clear();
            Debug.WriteLine(civilizations[1].Strength);
            tickCount++;
            if (tickCount % 100 == 0)
            {
                sw.Stop();
                last100TicksMilliseconds = sw.ElapsedMilliseconds;
                Trace.WriteLine("Tick "+tickCount+": Last 100 ticks: " + sw.ElapsedTicks / Stopwatch.Frequency + "s. Last 100 average: " + last100TicksMilliseconds / 100 + "ms/tick");
                sw = new Stopwatch();
                sw.Start();
            }
        }

        public void SplitCivilization(Civ empire)
        {
            Trace.WriteLine("Civilization split!");
            Civ newEmpire = new Civ(gameBoard.pickRandomCapital(empire, 75), empire.Name+random.Next(255), colors.GetNextColor());
            newEmpire.LandCellsdt = (empire.LandCellsdt / 10) * 7;
            empire.LandCellsdt = (empire.LandCellsdt / 10) * 2;
            empire.lostCell(newEmpire.Capital);
            newEmpire.calculateStrength();
            empire.calculateStrength();
            for(int i=-10;i<=10;i++)
                for (int j = -10; j <=10; j++)
                {
                    if (gameBoard.IsInBounds(newEmpire.Capital.X + i, newEmpire.Capital.Y + j))
                    {
                        if (random.Next(0, 20) > Math.Abs(i) + Math.Abs(j))
                        {
                            gameBoard.Cells[newEmpire.Capital.X + i][newEmpire.Capital.Y + j].NewOwner = newEmpire;
                            gameBoard.Cells[newEmpire.Capital.X + i][newEmpire.Capital.Y + j].ChangeOwner();
                        }
                    }
                }
            newCivilizations.Add(newEmpire);
        }
    }
}