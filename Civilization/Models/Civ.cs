﻿using SharpDX;

namespace Civilization.Models
{
    internal class Civ
    {
        private Cell capital;
        private string name;
        private Color color;
        private int landCells;
        private long landCellsdt;

        public int LandCells
        {
            get { return landCells; }
        }

        public long LandCellsdt
        {
            get { return landCellsdt; }
        }

        public void gainedLandCell()
        {
            landCells++;
        }

        public void lostCapital()
        {
            capital = MainModel.Instance.GameBoard.pickBestCapitalFor(this);
            if (capital == null)
            {
                System.Console.WriteLine(name + " was destroyed!");
                MainModel.Instance.KillCivilization(this);
            }
        }

        public void lostLandCell()
        {
            landCells--;
        }

        public void endOfTick()
        {
            landCellsdt += landCells;
            if (capital.Owner != this)
                lostCapital();
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
            this.capital.Owner = this;
        }
    }
}