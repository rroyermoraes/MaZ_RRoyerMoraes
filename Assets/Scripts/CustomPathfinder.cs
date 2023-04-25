using System.Collections.Generic;
using System;
using UnityEngine;

namespace Custom_Model
{
    public class RNode
    {
        public Position pos;
        public MazeCell cell;
        public int rValue=0;
        public RNode parent;
        public List<RNode> neighbours;
        public RNode(Position pos, MazeCell cell)
        {
            this.pos = pos;
            this.cell = cell;
            neighbours = new List<RNode>();
        }
    }
    public class RoyerPathfinder
    {        
        private MazeCell[,] maze;
        private uint mazeWidht = 0, mazeHeight = 0;

        private HashSet<RNode> rNodeSet;
        private RNode startNode, endNode;


        public RoyerPathfinder(MazeCell[,] nMaze, uint width, uint height)
        {

            this.mazeWidht = width;
            this.mazeHeight = height;
            this.maze = nMaze;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {                   

                    MazeCell node = nMaze[i, j];

                    if (node.HasFlag(MazeCell.start))//Set the starting position
                    {
                        Position mazePosition;
                        mazePosition.x = i;
                        mazePosition.y = j;
                        startNode = new RNode(mazePosition, node);
                    }
                    if (node.HasFlag(MazeCell.end))//Set the end position
                    {
                        Position mazePosition;
                        mazePosition.x = i;
                        mazePosition.y = j;
                        startNode = new RNode(mazePosition, node);
                    }
                }
            }
        }
        private List<RNode> GetNeighbours(RNode node)
        {
            return null;
        }

        public List<Position> FindPath(RNode current, RNode end)
        {
            return null;
        }

        List<Position> RetracePath(RNode start, RNode end)
        {
            return null;
        }
    }
}
