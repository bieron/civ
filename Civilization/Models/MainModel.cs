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
            random = new System.Random();
            //tests
            civilizations.Add(new Civ(gameBoard.Cells[39][21], "Greek Empire", new Color(0, 204, 255)));
            civilizations.Add(new Civ(gameBoard.Cells[117][181], "Egyptian Empire", new Color(255, 204, 0)));
            civilizations.Add(new Civ(gameBoard.Cells[221][137], "Jewish Empire", new Color(153, 77, 204)));
            civilizations.Add(new Civ(gameBoard.Cells[264][583], "Some African Empire", new Color(104, 0, 0)));
        }

        public void AddCivilization(Civ empire)
        {
            civilizations.Add(empire);
        }
    }
}