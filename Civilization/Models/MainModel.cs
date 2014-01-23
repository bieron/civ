using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms.VisualStyles;

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
        private Civ[] predefinedCivilizations;
        private List<Civ> newCivilizations;
        private List<Civ> deadCivilizations;
        private List<Civ> civilizations;
        private Board gameBoard;
        private Random random;
        private int drawSpeed;
        private long ticksCount;
        private long last100TicksMilliseconds;
        private Stopwatch sw;
        private bool doSplits=true;
        private Mp3Player musicPlayer;

        private string selectedBG;

        public string SelectedBG
        {
            get { return selectedBG; }
            set { selectedBG = value; }
        }

        public bool DoSplits
        {
            get { return doSplits; }
            set { doSplits = value; }
        }

        public int DrawSpeed
        {
            get { return drawSpeed; }
        }

        public long TicksCount
        {
            get { return ticksCount; }
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

        public int AliveCivilizationsCount
        {
            get { return civilizations.Count + newCivilizations.Count - deadCivilizations.Count; }
        }

        public delegate void TickEventHandler(object sender, EventArgs args);
        public event TickEventHandler TickEvent;

        private void Tick()
        {
            if(TickEvent != null)
                TickEvent(this, EventArgs.Empty);
        }

        protected MainModel()
        {
            
            System.Console.WriteLine("MainModel begin");
            gameBoard = new Board("EgyptMap");
            musicPlayer = new Mp3Player(@"..\..\Resources\Music\Sample\intro.mp3", @"..\..\Resources\Music\Sample\music.mp3");
            civilizations = new List<Civ>();
            deadCivilizations = new List<Civ>();
            newCivilizations = new List<Civ>();
            random = new System.Random();
            drawSpeed = 1;
            ticksCount = 0;
            colors = new ColorDistributor();
            //tests
            setDefaultCivs();

            sw = new Stopwatch();
            sw.Start();
        }

        public void setDefaultCivs()
        {
            if(gameBoard.MapTitle=="EgyptMap")
                predefinedCivilizations = new Civ[]
                {
                    new Civ(gameBoard.Cells[39][21], "Greek Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[117][181], "Egyptian Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[221][137], "Jewish Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[264][583], "Some African Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[230][39], "Persian Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[169][353], "Kings of the Desert Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[409][522], "Arabian Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[417][109], "Mongol Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[161][80], "Cyprus Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[12][157], "Moors Empire", colors.GetNextColor())
                };
            else if(gameBoard.MapTitle=="EgyptMapSmall")
                predefinedCivilizations = new Civ[]
                {
                    new Civ(gameBoard.Cells[26][10], "Greek Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[74][101], "Egyptian Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[132][85], "Jewish Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[153][341], "Some African Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[135][22], "Persian Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[87][217], "Kings of the Desert Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[243][308], "Arabian Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[233][55], "Mongol Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[109][45], "Cyprus Empire", colors.GetNextColor()),
                    new Civ(gameBoard.Cells[8][104], "Moors Empire", colors.GetNextColor())
                };
        }

        public void Start(int civsCount)
        {
            for (int i = 0; i < civsCount; ++i)
            {
                Civ newCiv = new Civ(predefinedCivilizations[i]);
                civilizations.Add(newCiv);
                newCiv.SettleCiv();
            }
        }

        public void Reset()
        {
            ticksCount = 0;
            colors.AddColors(predefinedCivilizations.Select(p => p.Color));
            civilizations.Clear();
            newCivilizations.Clear();
            deadCivilizations.Clear();

            gameBoard.Reset();
        }

        public void KillCivilization(Civ empire)
        {
            deadCivilizations.Add(empire);
            colors.AddColor(empire.Color);
        }

        public void EndOfTick()
        {
            foreach (Civ civ in newCivilizations)
                civilizations.Add(civ);
            newCivilizations.Clear();
            foreach (Civ civ in deadCivilizations)
                civilizations.Remove(civ);
            deadCivilizations.Clear();
            //Debug.WriteLine(civilizations[1].Strength);
            ticksCount++;
            if (ticksCount % 100 == 0)
            {
                sw.Stop();
                last100TicksMilliseconds = sw.ElapsedMilliseconds;
                Trace.WriteLine("TickEvent " + ticksCount + ": Last 100 ticks: " + sw.ElapsedTicks / Stopwatch.Frequency + "s. Last 100 average: " + last100TicksMilliseconds / 100 + "ms/tick");
                sw = new Stopwatch();
                sw.Start();
            }
            Tick();
        }

        public void SplitCivilization(Civ empire)
        {
            Trace.WriteLine("Civilization split!");
            Civ newEmpire = new Civ(gameBoard.PickRandomCapital(empire, 75), empire.Name+random.Next(255), colors.GetNextColor());
            newEmpire.SettleCiv();
            newEmpire.LandCellsdt = (empire.LandCellsdt / 10) * 7;
            //newEmpire.LandCellsdt = (long)empire.Strength * 7;
            empire.LandCellsdt = (empire.LandCellsdt / 10) * 2;
            empire.LostCell(newEmpire.Capital);
            empire.SplitCount++;
            newEmpire.CalculateStrength();
            empire.CalculateStrength();
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