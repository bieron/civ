using SharpDX;
using System;
using System.Collections.Generic;
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

        private List<Civ> deadCivilizations;
        private List<Civ> civilizations;
        private Board gameBoard;
        private Random random;

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
            gameBoard = new Board("EgyptMap");
            civilizations = new List<Civ>();
            deadCivilizations = new List<Civ>();
            random = new System.Random();
            //tests
            civilizations.Add(new Civ(gameBoard.Cells[39][21], "Greek Empire", new Color(0, 204, 255)));
            civilizations.Add(new Civ(gameBoard.Cells[117][181], "Egyptian Empire", new Color(255, 255, 0)));
            civilizations.Add(new Civ(gameBoard.Cells[221][137], "Jewish Empire", new Color(153, 77, 204)));
            civilizations.Add(new Civ(gameBoard.Cells[264][583], "Some African Empire", new Color(104, 0, 0)));
            civilizations.Add(new Civ(gameBoard.Cells[230][39], "Persian Empire", new Color(0, 51, 255)));
            civilizations.Add(new Civ(gameBoard.Cells[169][353], "Kings of the Desert Empire", new Color(32, 128, 32)));
            civilizations.Add(new Civ(gameBoard.Cells[409][522], "Arabian Empire", new Color(32, 255, 32)));
            civilizations.Add(new Civ(gameBoard.Cells[417][109], "Mongol Empire", new Color(139, 101, 8)));
            civilizations.Add(new Civ(gameBoard.Cells[161][80], "Cyprus Empire", new Color(238, 130, 238)));
            civilizations.Add(new Civ(gameBoard.Cells[12][157], "Moors Empire", new Color(238, 173, 14)));
        }

        public void KillCivilization(Civ empire)
        {
            deadCivilizations.Add(empire);
        }

        public void AddCivilization(Civ empire)
        {
            civilizations.Add(empire);
        }

        public void endOfTick()
        {
            foreach (Civ civ in deadCivilizations)
                civilizations.Remove(civ);
            deadCivilizations.Clear();
        }
    }
}