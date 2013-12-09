using SharpDX;
namespace Civilization.Models {
    class Civ {
        Cell capital;
        string name;
        Color color;

        public Civ(Cell capital, string name, Color color) {
            this.capital = capital;
            this.name = name;
            this.color = color;
            capital.setOwner(this);
        }

        public Cell getCapital() {
            return capital;
        }

        public Color getColor() {
            return color;
        }
    }
}
