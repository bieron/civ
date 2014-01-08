using System.Windows.Controls;
using System.Drawing;
using System;
using System.Threading;

namespace Civilization.Models
{
    internal class Board
    {
        private Cell[][] cells;
        private readonly int height;
        private readonly int width;
        private Thread ULThread;
        private Thread URThread;
        private Thread LLThread;
        private Thread LRThread;
        private bool useThreads;
        private bool bFirstTick;

        public bool UseThreads
        {
            get { return useThreads; }
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

        public Board(string mapTitle)
        {
            Bitmap terrainBmp = new Bitmap(@"..\..\Resources\" + mapTitle + @"\terrain.bmp");
            height = terrainBmp.Height;
            width = terrainBmp.Width;
            CreateCells();
            InitializeNeighbours();
            InitializeCells();
            useThreads = true;
            bFirstTick = true;
            if(useThreads)
                InitializeThreads();
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

        private void InitializeThreads()
        {
            ThreadStart ULThreadStart = delegate { ThreadDetermineNewOwner(0, width / 2, 0, height / 2); };
            ULThread = new Thread(ULThreadStart);
            ULThread.Name = "ULThread";
            ULThread.Start();
            ThreadStart URThreadStart = delegate { ThreadDetermineNewOwner(0, width / 2, height / 2, height); };
            URThread = new Thread(URThreadStart);
            URThread.Name = "URThread";
            URThread.Start();
            ThreadStart LLThreadStart = delegate { ThreadDetermineNewOwner(width / 2, width, 0, height / 2); };
            LLThread = new Thread(LLThreadStart);
            LLThread.Name = "LLThread";
            LLThread.Start();
            ThreadStart LRThreadStart = delegate { ThreadDetermineNewOwner(width / 2, width, height / 2, height); };
            LRThread = new Thread(LRThreadStart);
            LRThread.Name = "LRThread";
            LRThread.Start();
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
            Bitmap landBMP = new Bitmap(@"..\..\Resources\EgyptMap\land.bmp");
            for (int i = 0; i < landBMP.Width; i++)
                for (int j = 0; j < landBMP.Height; j++)
                    if (landBMP.GetPixel(i, j).R == 0)
                        cells[i][j].SetUnreachable();
        }

        private void DetermineDesirability()
        {
            Bitmap desBMP = new Bitmap(@"..\..\Resources\EgyptMap\desirability.bmp");
            for (int i = 0; i < desBMP.Width; i++)
                for (int j = 0; j < desBMP.Height; j++)
                    cells[i][j].Desirability = desBMP.GetPixel(i, j).R;
        }

        public void Tick()
        {
            //DetermineNewOwnerForRangeCells(0, width / 2, 0, height / 2);
            //DetermineNewOwnerForRangeCells(0, width / 2, height / 2, height);
            //DetermineNewOwnerForRangeCells(width / 2, width, 0, height / 2);
            //DetermineNewOwnerForRangeCells(width / 2, width, height / 2, height);
            if (bFirstTick)
            {
                ResumeThreads();
                bFirstTick = false;
            }
            if (useThreads)
            {
                while (ULThread.ThreadState != ThreadState.Suspended || URThread.ThreadState != ThreadState.Suspended || LLThread.ThreadState != ThreadState.Suspended || LRThread.ThreadState != ThreadState.Suspended)
                    Thread.Sleep(0);
            }
            else
                DetermineNewOwnerForAllCells();
            ChangeOwnerForAllCells();
            
            UpdateCivs();
            ResumeThreads();
            MainModel.Instance.endOfTick();
        }

        private void UpdateCivs()
        {
            foreach (Civ civ in MainModel.Instance.Civilizations)
                civ.endOfTick();
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

        private void ThreadDetermineNewOwner(int fromX, int toX, int fromY, int toY)
        {
            Random random = new Random();
            while (true)
            {
                Thread.CurrentThread.Suspend();
                DetermineNewOwnerForRangeCells(fromX, toX, fromY, toY, random);
            }
        }

        public void ResumeThreads()
        {
            if (!useThreads)
                return;
            ULThread.Resume();
            URThread.Resume();
            LLThread.Resume();
            LRThread.Resume();
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

        public Cell pickRandomCapital(Civ empire, int dist = 0)
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