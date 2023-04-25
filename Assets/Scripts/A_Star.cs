using System.Collections.Generic;
using System;

namespace A_Star_Model
{
    public class Node
    {
        public Position pos;
        public MazeCell cell;
        public int gCost;
        public int hCost;
        public Node parent;
        
        public int fCost { get { return gCost + hCost; } }
        public Node(Position pos, MazeCell cell)
        {
            this.pos = pos;
            this.cell = cell;
        }
    }
    public class A_Star
    {
        //private MazeCell[,] maze;
        private uint mazeWidht = 0, mazeHeight = 0;

        private Node[,] nodeGrid;
        private Position startPos, endPos;

        private uint lastRunSteps = 0; 

        public Node[,] NodeGrid { get { return nodeGrid; } }
        public Position StartPos { get { return startPos; } }
        public Position EndPos { get { return endPos; } }

        public A_Star(MazeCell[,] nMaze, uint width, uint height)
        {
            /*A-Star constructor*/

            mazeWidht = width;
            mazeHeight = height;
            nodeGrid = new Node[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Position mazePosition;
                    mazePosition.x = i;
                    mazePosition.y = j;
                    NodeGrid[i, j] = new Node(mazePosition, nMaze[i, j]);

                    MazeCell node = nMaze[i, j];

                    if (node.HasFlag(MazeCell.start))//Set the starting position
                    {
                        startPos.x = i;
                        startPos.y = j;
                    }
                    if (node.HasFlag(MazeCell.end))//Set the end position
                    {
                        endPos.x = i;
                        endPos.y = j;
                    }
                }
            }

        }
        public uint GetLastRunSteps()
        {
            return lastRunSteps;
        }
        public List<Node> GetNeighbours(Node node)
        {
            /*This will check the mazecell walls and also if the cell is a boundary one, to get the possible movable neighbouring possitions, it probably could be improved but I havent found a way*/

            List<Node> neighbours = new List<Node>();

            //west check
            if (!(node.pos.x == 0) && !node.cell.HasFlag(MazeCell.west))
            {
                neighbours.Add(NodeGrid[node.pos.x - 1, node.pos.y]);
            }

            //east check
            if (!(node.pos.x - 1 == mazeWidht) && !node.cell.HasFlag(MazeCell.east))
            {
                neighbours.Add(NodeGrid[node.pos.x + 1, node.pos.y]);
            }

            //south check
            if (!(node.pos.y == 0) && !node.cell.HasFlag(MazeCell.south))
            {
                neighbours.Add(NodeGrid[node.pos.x, node.pos.y - 1]);
            }

            //north check
            if (!(node.pos.y - 1 == mazeHeight) && !node.cell.HasFlag(MazeCell.north))
            {
                neighbours.Add(NodeGrid[node.pos.x, node.pos.y + 1]);
            }


            return neighbours;
        }

        public List<Position> FindPath(Node start, Node end)
        {
            lastRunSteps = 0;
            List<Node> openNodes = new List<Node>();
            HashSet<Node> closedNodes = new HashSet<Node>();
            openNodes.Add(start);
            while (openNodes.Count > 0)
            {
                Node curentNode = openNodes[0];
                for (int i = 1; i < openNodes.Count; i++)
                {
                    if (openNodes[i].fCost < curentNode.fCost || openNodes[i].fCost == curentNode.fCost && openNodes[i].hCost < curentNode.hCost)
                    {
                        curentNode = openNodes[i];
                    }
                }
                openNodes.Remove(curentNode);
                closedNodes.Add(curentNode);
                if (curentNode == end)
                {
                    return RetracePath(start, end);
                }
                foreach (Node neighbour in GetNeighbours(curentNode))
                {
                    if (closedNodes.Contains(neighbour))
                    {
                        continue;
                    }
                    int newMovementCostToNeighbour = curentNode.gCost + GetDistance(curentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openNodes.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, end);
                        neighbour.parent = curentNode;
                        lastRunSteps++;
                        if (!openNodes.Contains(neighbour))
                        {
                            openNodes.Add(neighbour);
                        }
                    }
                }
            }
            return null;
        }

        List<Position> RetracePath(Node start, Node end)
        {
            /*Wallks the maze backwards to get the path*/
            List<Position> path = new List<Position>();
            Node currentNode = end;
            while (currentNode != start)
            {
                path.Add(currentNode.pos);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }
        int GetDistance(Node nodeA, Node nodeB)
        {
            /*This is the main heuristic factor, as it is the main path cost definer. Since I can't move diagonaly in this given maze setup I had to use a simple vertical and horizontal absolute distance sum.*/
            return ((int)(System.MathF.Abs(nodeB.pos.x - nodeA.pos.x) + System.MathF.Abs(nodeB.pos.y - nodeA.pos.y) * 10));
        }
    }
}
