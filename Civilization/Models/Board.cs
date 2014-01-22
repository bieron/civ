using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;
using System;
using System.Threading;

namespace Civilization.Models
{
    public class Board
    {
        private Cell[][] cells;
        private readonly int height;
        private readonly int width;
        private bool useThreads;
        private bool bFirstTick;
        private TaskBoardProcessor taskBoardProcessor;
        private string mapTitle;

        public bool UseThreads
        {
            get { return useThreads; }
        }

        public string MapTitle
        {
            get { return mapTitle; }
        }

        public Cell[][] Cells
        {
            get { return cells; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }

        public Board(string _mapTitle)
        {
            mapTitle = _mapTitle;
            Bitmap terrainBmp = new Bitmap(@"..\..\Resources\" + mapTitle + @"\terrain.bmp");
            height = terrainBmp.Height;
            width = terrainBmp.Width;
            CreateCells();
            InitializeNeighbours();
            InitializeCells();
            useThreads = false;
            bFirstTick = true;

            if (useThreads)
            {
                taskBoardProcessor = TaskBoardProcessor.CreateNTasksForBoard(4, height, width);
            }
            else
            {
                System.Console.WriteLine("SINGLE-THREAD MODE! Toggle useThreads in Board class constructor to use multi-threading");
            }
        }

        public void Reset()
        {
            foreach (Cell[] cellA in cells)
            {
                foreach (Cell cell in cellA)
                {
                    cell.Clear();
                }
            }
            bFirstTick = true;
        }

        private void CreateCells()
        {
            cells = new Cell[width][];
            for (int i = 0; i < width; i++)
            {
                cells[i] = new Cell[height];
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i][j] = new Cell(i, j);
                }
            }
        }

        private void InitializeNeighbours()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (IsInnerCell(i, j))
                    {
                        AddNeighborsForInnerCell(i, j);
                    }
                    else
                    {
                        AddNeighborsForBorderCell(i, j);
                    }
                }
            }
        }

        private bool IsInnerCell(int col, int row)
        {
            return col - 1 >= 0 && col + 1 < width && row - 1 >= 0 && row + 1 < height;
        }

        private void AddNeighborsForInnerCell(int col, int row)
        {
            cells[col][row].AddNeighbour(cells[col - 1][row - 1]); //NW
            cells[col][row].AddNeighbour(cells[col - 1][row]); //W
            cells[col][row].AddNeighbour(cells[col - 1][row + 1]); //SW
            cells[col][row].AddNeighbour(cells[col][row - 1]); //N
            cells[col][row].AddNeighbour(cells[col][row + 1]); //S
            cells[col][row].AddNeighbour(cells[col + 1][row - 1]); //NE
            cells[col][row].AddNeighbour(cells[col + 1][row]); //E
            cells[col][row].AddNeighbour(cells[col + 1][row + 1]); //SE
        }

        private void AddNeighborsForBorderCell(int col, int row)
        {
            if (col - 1 >= 0)
            {
                if (row - 1 >= 0)
                {
                    cells[col][row].AddNeighbour(cells[col - 1][row - 1]); //NW
                }
                cells[col][row].AddNeighbour(cells[col - 1][row]); // W
                if (row + 1 < height)
                {
                    cells[col][row].AddNeighbour(cells[col - 1][row + 1]); //SW
                }
            }

            if (row - 1 >= 0)
            {
                cells[col][row].AddNeighbour(cells[col][row - 1]); //N
            }
            if (row + 1 < height)
            {
                cells[col][row].AddNeighbour(cells[col][row + 1]); //S
            }

            if (col + 1 < width)
            {
                if (row - 1 >= 0)
                {
                    cells[col][row].AddNeighbour(cells[col + 1][row - 1]); //NE
                }
                cells[col][row].AddNeighbour(cells[col + 1][row]); //E
                if (row + 1 < height)
                {
                    cells[col][row].AddNeighbour(cells[col + 1][row + 1]); //SE
                }
            }
        }

        private void InitializeCells()
        {
            DetermineReachability();
            DetermineDesirability();
        }

        private void DetermineReachability()
        {
            Bitmap landBMP = new Bitmap(@"..\..\Resources\" + mapTitle + @"\land.bmp");
            for (int i = 0; i < landBMP.Width; i++)
                for (int j = 0; j < landBMP.Height; j++)
                    if (landBMP.GetPixel(i, j).R == 0)
                        cells[i][j].SetUnreachable();
        }

        private void DetermineDesirability()
        {
            Bitmap desBMP = new Bitmap(@"..\..\Resources\" + mapTitle + @"\desirability.bmp");
            for (int i = 0; i < desBMP.Width; i++)
                for (int j = 0; j < desBMP.Height; j++)
                    cells[i][j].Desirability = desBMP.GetPixel(i, j).R;
        }

        private void DetermineDefensibility()
        {
            Bitmap defBMP = new Bitmap(@"..\..\Resources\" + mapTitle + @"\defensibility.bmp");
            for (int i = 0; i < defBMP.Width; i++)
                for (int j = 0; j < defBMP.Height; j++)
                    cells[i][j].Defensibility = defBMP.GetPixel(i, j).R;
        }

        public void Tick()
        {
            if (bFirstTick)
            {
                bFirstTick = false;
                if(useThreads)
                    taskBoardProcessor.StartProcessing(DetermineNewOwnerForRangeCells);
            }
            if (useThreads)
                taskBoardProcessor.WaitForEnd();
            else
                DetermineNewOwnerForAllCells();
            ChangeOwnerForAllCells();
            
            UpdateCivs();
            MainModel.Instance.EndOfTick();
            if (useThreads)
            {
                taskBoardProcessor.StartProcessing(DetermineNewOwnerForRangeCells);
            }
        }

        private void UpdateCivs()
        {
            foreach (Civ civ in MainModel.Instance.Civilizations)
                civ.EndOfTick();
        }

        private void DetermineNewOwnerForAllCells()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i][j].CalculateNewOwner();
                }
            }
        }

        private void DetermineNewOwnerForRangeCells(int fromX, int toX, int fromY, int toY, Random random = null)
        {
            if (random == null)
                random = MainModel.Instance.Random;
            for (int i = fromX; i < toX; i++)
            {
                for (int j = fromY; j < toY; j++)
                {
                    cells[i][j].CalculateNewOwner(random);
                }
            }
        }
        private void ChangeOwnerForAllCells()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i][j].ChangeOwner();
                }
            }
        }

        public Cell PickBestCapitalFor(Civ empire)
        {
            Cell bestCell = null;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if(cells[i][j].IsReachable&&cells[i][j].Owner==empire&&(bestCell==null||cells[i][j].Desirability>bestCell.Desirability))
                        bestCell = cells[i][j];
                }
            }
            return bestCell; 
        }

        public Cell PickRandomCapital(Civ empire, int dist = 0)
        {
            Cell newCapital;
            Random rand = MainModel.Instance.Random;
            bool isFarEnough = false;
            do
            {
                int x = rand.Next(width);
                int y = rand.Next(height);
                newCapital = cells[x][y];
                isFarEnough = dist != 0 && DistanceCalculator.calculateDistance(empire.Capital, newCapital, DistanceCalculator.CalculationType.CT_PYTHAGORIAN) > dist;
            } while (!newCapital.IsReachable || newCapital.Owner != empire || !isFarEnough || newCapital == empire.Capital);
            return newCapital;
        }

        public bool IsInBounds(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                return false;
            return true;
        }
    }
}