using System.Windows.Controls;
using System.Drawing;
namespace Civilization.Models {
    class Board {
        Cell[][] cells;
        int height, width;
        Bitmap terrainBMP;
        public Board(string mapTitle) {
            terrainBMP = new Bitmap(@"..\..\Resources\" + mapTitle + @"\terrain.bmp");
            height = terrainBMP.Height;
            width = terrainBMP.Width;
            createCells();
            initializeNeighbours();
            initializeCells();
        }

        private void createCells() {
            //nie jestem pewien czy tak się inicjalizuje tablice 2D :P
            cells = new Cell[width][];
            for (int i = 0; i < width; i++)
            {
                cells[i]=new Cell[height];
            }

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    cells[i][j]=new Cell(i, j);
                }
            }
        }

        private void initializeNeighbours() {
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    if (i - 1 >= 0 && i + 1 < width && j - 1 >= 0 && j + 1 < height) { //komórki niegraniczne
                        cells[i][j].addNeighbour(cells[i - 1][j - 1]); //NW
                        cells[i][j].addNeighbour(cells[i - 1][j]); //W
                        cells[i][j].addNeighbour(cells[i - 1][j + 1]); //SW
                        cells[i][j].addNeighbour(cells[i][j - 1]); //N
                        cells[i][j].addNeighbour(cells[i][j + 1]); //S
                        cells[i][j].addNeighbour(cells[i + 1][j - 1]); //NE
                        cells[i][j].addNeighbour(cells[i + 1][j]); //E
                        cells[i][j].addNeighbour(cells[i + 1][j + 1]); //SE
                    }
                    else { // komórki graniczne
                        if (i - 1 >= 0) {
                            if (j - 1 >= 0) { //NW
                                cells[i][j].addNeighbour(cells[i - 1][j - 1]);
                            }
                            cells[i][j].addNeighbour(cells[i - 1][j]); //W
                            if (j + 1 < height) { //SW
                                cells[i][j].addNeighbour(cells[i - 1][j + 1]);
                            }
                        }
                        if (j - 1 >= 0) { //N
                            cells[i][j].addNeighbour(cells[i][j - 1]);
                        }
                        if (j + 1 < height) { //S
                            cells[i][j].addNeighbour(cells[i][j + 1]);
                        }
                        if (i + 1 < width) {
                            if (j - 1 >= 0) { //NE
                                cells[i][j].addNeighbour(cells[i + 1][j - 1]);
                            }
                            cells[i][j].addNeighbour(cells[i + 1][j]); //E
                            if (j + 1 < height) { //SE
                                cells[i][j].addNeighbour(cells[i + 1][j + 1]);
                            }
                        }
                    }
                }
            }
        }

        private void initializeCells() {
            Bitmap landBMP = new Bitmap(@"..\..\Resources\EgyptMap\land.bmp");
            for (int i = 0; i < landBMP.Width; i++)
                for (int j = 0; j < landBMP.Height; j++)
                    if (landBMP.GetPixel(i, j).R == 0)
                        cells[i][j].setUnreachable();
            Bitmap desBMP = new Bitmap(@"..\..\Resources\EgyptMap\desirability.bmp");
            for (int i = 0; i < landBMP.Width; i++)
                for (int j = 0; j < landBMP.Height; j++)
                    cells[i][j].setDesirability(desBMP.GetPixel(i, j).R);
        }

        public void tick()
        {
            //System.Console.WriteLine("Tick!");
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    cells[i][j].calculateNewOwner();
                }
            }
            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    cells[i][j].changeOwner();
                }
            }
        }

        public Cell[][] getCells() {
            return cells;
        }

        public int getWidth() {
            return width;
        }

        public int getHeight() {
            return height;
        }
    }
}
