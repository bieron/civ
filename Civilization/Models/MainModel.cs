using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization.Models
{
    //nie wiedziałem jak to nazwać, można to zmienić
    class MainModel
    {
        List<Civ> civilizations;
        Board gameBoard;
        Random random;
        static MainModel instance = new MainModel(); 
        protected MainModel() {
            gameBoard = new Board("EgyptMap");
            civilizations = new List<Civ>();
            random = new System.Random();
            //tests
            civilizations.Add(new Civ(gameBoard.getCells()[39][21], "Greek Empire", new Color(0, 204, 255)));
            civilizations.Add(new Civ(gameBoard.getCells()[117][181], "Egyptian Empire", new Color(255, 204, 0)));
            civilizations.Add(new Civ(gameBoard.getCells()[221][137], "Jewish Empire", new Color(153, 77, 204)));
            civilizations.Add(new Civ(gameBoard.getCells()[264][583], "Some African Empire", new Color(204, 0, 0)));
        }

        public static MainModel getInstance() {
            return instance;
        }

        public Random getRandom() {
            return random;
        }

        public Board getBoard() {
            return gameBoard;
        }

        public List<Civ> getCivList() {
            return civilizations;
        }

        public void addCivilization(Civ empire) {
            civilizations.Add(empire);
        }
    }
}
