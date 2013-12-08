using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization.Models {
    class DistanceCalculator {
        public enum CalculationType {
            CT_PYTHAGORIAN, //bardzo słaby ale szybki - odległość w linii prostej
            CT_PYTHAGORIAN2, //słaby ale bardzo szybki - odległość bez brania pod uwagę przeszkód
            CT_ASTAR //bardzo silny ale wolny - odległość z uwzględnieniem przeszkód
        };

        public static double calculateDistance(Cell from, Cell to, CalculationType calcType) {
            switch (calcType) {
                case CalculationType.CT_ASTAR:
                    AStarCalculator calculator = new AStarCalculator(from, to);
                    return calculator.calculate();
                case CalculationType.CT_PYTHAGORIAN2:
                    int diag = Math.Min(Math.Abs(from.getX() - to.getX()), Math.Abs(from.getY() - to.getY()));
                    int vorh = Math.Max(Math.Abs(from.getX() - to.getX()), Math.Abs(from.getY() - to.getY()))-diag;
                    return diag * 141 + vorh * 100;
                case CalculationType.CT_PYTHAGORIAN:
                default: //niech pitagoras będzie defaultowy (mimo że najsłabszy)
                    return Math.Sqrt((from.getX() - to.getX()) * (from.getX() - to.getX()) + (from.getY() - to.getY()) * (from.getY() - to.getY()));
            }
        }
    }

    class AStarCalculator {
        class AStarNode {
            public Cell cell;
            public AStarNode parentNode;
            public int f, g;
            public AStarNode(Cell theCell, AStarNode pNode, AStarCalculator calculator) {
                cell = theCell;
                parentNode = pNode;
                if (pNode == null) {//parent
                    f = 0;
                    g = 0;
                }
                else {
                    g = pNode.g + calculator.costToReachNeighbour(pNode.cell, theCell);
                    f = g + calculator.heuristicDel(cell, calculator.targetCell);
                }
            }
        }
        AStarNode startingNode;
        Cell targetCell;
        AStarNode targetNode;
        List<AStarNode> openList;
        List<AStarNode> closedList;
        public delegate int HeuristicDel(Cell from, Cell to);
        HeuristicDel heuristicDel;
        //parametry
        const bool debug = true;
        const int waterCost = 5;
        const int stopValue = 20000; //tak sobie eksperymentalnie dobrałem

        public AStarCalculator(Cell from, Cell to) {
            startingNode = new AStarNode(from, null, this);
            targetCell = to;
            targetNode = null;
            openList = new List<AStarNode>();
            closedList = new List<AStarNode>();
            heuristicDel = new HeuristicDel(manhattanHeuristic); //tu można zmienić metodę H
        }

        public int calculate() {
            //sprawdzenie czy jest w ogóle sens to liczyć
            int diag = Math.Min(Math.Abs(startingNode.cell.getX() - targetCell.getX()), Math.Abs(startingNode.cell.getY() - targetCell.getY()));
            int vorh = Math.Max(Math.Abs(startingNode.cell.getX() - targetCell.getX()), Math.Abs(startingNode.cell.getY() - targetCell.getY())) - diag;
            if (diag * 141 + vorh * 100 > stopValue)
                return stopValue;//stop value zostanie przekroczone na 100 %

            insertToOpenList(startingNode);
            while (openList.Count != 0 && targetNode == null && openList.First().f<stopValue)
                processNode(openList.First());
            if (targetNode == null)
                if (openList.First().f > stopValue)
                    return stopValue;
                else {
                    Console.WriteLine("DistanceCalculator: Nie znaleziono ścieżki!"); //raczej niemożliwa sytuacja, ale nich będzie jakby się coś miało zmienić
                    return 999999;
                }
            else {
                if (debug)
                    ;//TODO zrób coś ze ścieżką - wyswietl ja, wypisz, cokolwiek
                return targetNode.g;
            }

        }
        private void insertToOpenList(AStarNode node) {
            foreach (AStarNode aNode in openList) {
                if (aNode.f > node.f) {
                    int index = openList.IndexOf(aNode);
                    openList.Insert(index, node);
                    return;
                }
                openList.Add(node);
            }
        }

        private void processNode(AStarNode node) {
            if (node.cell == targetCell) {
                targetNode = node;
                return;
            }
            openList.Remove(node);
            closedList.Add(node);
            foreach (Cell theCell in node.cell.getNeighbours()) {
                bool isClosed = false;
                foreach (AStarNode aNode in closedList) {
                    if (aNode.cell == theCell) {
                        isClosed = true;
                        break;
                    }
                }
                if (isClosed)
                    continue;
                AStarNode nodeInOpen = null;
                foreach (AStarNode aNode in openList) {
                    if (aNode.cell == theCell) {
                        nodeInOpen = aNode;
                        break;
                    }
                }
                if (nodeInOpen == null)
                    insertToOpenList(new AStarNode(theCell, startingNode, this));
                else { //node jest już na liście open
                    if (nodeInOpen.g > node.g + costToReachNeighbour(node.cell, theCell)) {
                        openList.Remove(nodeInOpen);
                        insertToOpenList(new AStarNode(theCell, startingNode, this));
                    }
                }
            }
        }

        public int manhattanHeuristic(Cell from, Cell to){
            return 100*(Math.Abs(from.getX()-to.getX())+Math.Abs(from.getY()-to.getY()));
        }

        public int costToReachNeighbour(Cell from, Cell to) {
            int cost = 100;
            if (to.getX() != from.getX() && to.getY() != from.getY()) //kratka znajduje się po skosie
                cost = 141; // sqrt(2)
            if (!to.isReachable())
                cost *= waterCost; //woda/góry itp zwiększają odległość od stolicy
            return cost;
        }
    }
}
