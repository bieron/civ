using SharpDX;

namespace Civilization.Models
{
    internal class Civ
    {
        private Cell capital;
        private string name;
        private Color color;

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
            this.capital.Owner = this;
        }
    }
}