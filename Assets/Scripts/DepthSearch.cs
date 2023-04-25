using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DephtSearch_Model
{
    public class Node
    {
        public Position pos;
        public MazeCell cell;
        public Node parent;
        public List<Node> branches;
        public int depth=0;
        public Node(Position pos, MazeCell cell)
        {
            this.pos = pos;
            this.cell = cell;
        }
    }
    public class DephtLimitedSearch
    {
        private uint mazeWidht = 0, mazeHeight = 0;

        private Node[,] nodeGrid;
        private Position startPos, endPos;
        private ulong lastRunSteps=0;

        public Node[,] NodeGrid { get { return nodeGrid; } }
        public Position StartPos { get { return startPos; } }
        public Position EndPos { get { return endPos; } }

        public DephtLimitedSearch(MazeCell[,] nMaze, uint width, uint height)
        {

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
                neighbours.Add(new Node(NodeGrid[node.pos.x - 1, node.pos.y].pos, NodeGrid[node.pos.x - 1, node.pos.y].cell));
            }

            //south check
            if (!(node.pos.y == 0) && !node.cell.HasFlag(MazeCell.south))
            {                
                neighbours.Add(new Node(NodeGrid[node.pos.x, node.pos.y-1].pos, NodeGrid[node.pos.x, node.pos.y-1].cell));                
            }

            //east check
            if (!(node.pos.x - 1 == mazeWidht) && !node.cell.HasFlag(MazeCell.east))
            {
                neighbours.Add(new Node(NodeGrid[node.pos.x + 1, node.pos.y].pos, NodeGrid[node.pos.x + 1, node.pos.y].cell));
            }

            //north check
            if (!(node.pos.y - 1 == mazeHeight) && !node.cell.HasFlag(MazeCell.north))
            {
                neighbours.Add(new Node(NodeGrid[node.pos.x, node.pos.y+1].pos, NodeGrid[node.pos.x, node.pos.y+1].cell));
            }

            return neighbours;
        }
        public ulong GetLastRunSteps()
        {
            return lastRunSteps;
        }
        public List<Position> FindPath(Node start, Node end,uint maxDepth)
        {
            lastRunSteps = 0;
            
            Stack<Node> nodes = new Stack<Node>();
            nodes.Push(start);
            while (nodes.Count > 0  )
            {
                Node n = nodes.Pop();
                if (n.depth < maxDepth){                    
                    lastRunSteps++;
                    if (n.pos.Equals(end.pos))
                    {
                        return RetracePath(n, start);
                    }
                    else
                    {
                        List<Node> neighbours = GetNeighbours(n);
                        foreach (Node nei in neighbours)
                        {
                            nei.parent = n;
                            nei.depth = n.depth + 1;
                            nodes.Push(nei);
                        }
                    }
                }
            }
            return null;            
        }


        public List<Position> RetracePath(Node end,Node start)
        {
            /*Wallks the maze backwards to get the path*/
            List<Position> path = new List<Position>();
            Node currentNode = end;
            while (!currentNode.pos.Equals(start.pos))
            {                
                path.Add(currentNode.pos);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }
    }
}

