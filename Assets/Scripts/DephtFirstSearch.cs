using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DephtFirstSearch_Model
{
    public class Node
    {
        public Position pos;
        public MazeCell cell;
        public Node parent;
        public List<Node> branches;
        public bool visited;
        public Node(Position pos, MazeCell cell)
        {
            this.pos = pos;
            this.cell = cell;
            this.visited = false;
        }
    }
    public class DephtFirstSearch
    {
        private uint mazeWidht = 0, mazeHeight = 0;

        private Node[,] nodeGrid;
        private Position startPos, endPos;
        private List<Position> pathFound;
        private uint lastRunSteps = 0;
        public Node[,] NodeGrid { get { return nodeGrid; } }
        public Position StartPos { get { return startPos; } }
        public Position EndPos { get { return endPos; } }

        public DephtFirstSearch(MazeCell[,] nMaze, uint width, uint height)
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
        public List<Node> GetNeighbours(Node node)
        {
            /*This will check the mazecell walls and also if the cell is a boundary one, to get the possible movable neighbouring possitions, it probably could be improved but I havent found a way*/

            List<Node> neighbours = new List<Node>();

            //west check
            if (!(node.pos.x == 0) && !node.cell.HasFlag(MazeCell.west))
            {
                if (!nodeGrid[node.pos.x - 1, node.pos.y].visited)
                {
                    neighbours.Add(NodeGrid[node.pos.x - 1, node.pos.y]);
                }                
            }           

            //south check
            if (!(node.pos.y == 0) && !node.cell.HasFlag(MazeCell.south))
            {
                if (!nodeGrid[node.pos.x, node.pos.y - 1].visited)
                {
                    neighbours.Add(NodeGrid[node.pos.x, node.pos.y - 1]);
                }
            }

            //east check
            if (!(node.pos.x - 1 == mazeWidht) && !node.cell.HasFlag(MazeCell.east))
            {
                if (!nodeGrid[node.pos.x + 1, node.pos.y].visited)
                {
                    neighbours.Add(NodeGrid[node.pos.x + 1, node.pos.y]);
                }
            }

            //north check
            if (!(node.pos.y - 1 == mazeHeight) && !node.cell.HasFlag(MazeCell.north))
            {
                if (!nodeGrid[node.pos.x, node.pos.y+1].visited)
                {
                    neighbours.Add(NodeGrid[node.pos.x, node.pos.y + 1]);
                }
            }

            return neighbours;
        }
        public uint GetLastRunSteps()
        {
            return lastRunSteps;
        }
        public List<Position> FindPath(Node start, Node end)
        {
            lastRunSteps = 0;
            DFS(start, end);
            return pathFound;
        }
        public void DFS(Node current, Node end)
        {
            current.visited = true;
            lastRunSteps++;
            List<Node> neighbours = GetNeighbours(current);            
            foreach (Node n in neighbours)
            {
                n.parent = current;
                if (n == end)
                {
                    pathFound = RetracePath(n);
                }
                else
                {
                    DFS(n, end);
                }
            }
        }
        public List<Position> RetracePath(Node end)
        {
            /*Wallks the maze backwards to get the path*/
            List<Position> path = new List<Position>();
            Node currentNode = end;
            while (currentNode.parent != null)
            {
                path.Add(currentNode.pos);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }
    }
}

