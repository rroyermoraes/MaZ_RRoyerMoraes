using System;
using System.Collections.Generic;

[Flags]
public enum MazeCell //Using a flag system to create the maze walls and mark the maze start and end
{
    west = 1,       //0000 0001
    east = 2,       //0000 0010
    north = 4,      //0000 0100
    south = 8,      //0000 1000

    start = 32,     //0010 0000
    end = 64,       //0100 0000
    marked = 128,   //1000 0000 
}

public struct Position
{
    public int x,y;
}

public struct Neighbour
{
    public Position position;
    public MazeCell sharedWall;
}
public static class MazeGenerator
{
    private static MazeCell GetOpposingWall(MazeCell wall)
    {
        switch (wall)
        {
            case MazeCell.east:
                return MazeCell.west;
            case MazeCell.west:
                return MazeCell.east;
            case MazeCell.north:
                return MazeCell.south;
            case MazeCell.south:
                return MazeCell.north;
            default:
                return MazeCell.west;
        }
    }
    private static MazeCell[,] RecursiveBacktracker(MazeCell[,] maze,uint widht,uint height)
    {
        Stack<Position> posStack = new Stack<Position>();
        Random rnd = new System.Random();
        Position pos = new Position { x = 0, y = rnd.Next(0, (int)height)};

        maze[pos.x, pos.y] |= MazeCell.marked;
        maze[pos.x, pos.y] |= MazeCell.start;
        posStack.Push(pos);

        while (posStack.Count > 0)
        {
            Position currentPos = posStack.Pop();
            List<Neighbour> neighbours = GetNeighbours(currentPos, maze, widht, height,true);

            if (neighbours.Count > 0)
            {
                posStack.Push(currentPos);
                int randIndex = rnd.Next(0, neighbours.Count);
                Neighbour randNeighbour = neighbours[randIndex];
                Position nPosition = randNeighbour.position;
                maze[currentPos.x, currentPos.y] &= ~randNeighbour.sharedWall;
                maze[nPosition.x,nPosition.y] &= ~GetOpposingWall(randNeighbour.sharedWall);
                maze[nPosition.x, nPosition.y] |= MazeCell.marked;
                posStack.Push(nPosition);
            }                        
        }
        maze[widht - 1, rnd.Next(0, (int)height)] |= MazeCell.end;
        maze = SwissCheese(maze, widht, height,98);
        return maze;
    }
    private static MazeCell[,] SwissCheese(MazeCell[,] maze, uint widht, uint height,uint roleRatio)
    {
        Random rnd = new System.Random();
        if(widht > 4 && height > 4) { 
        for (int i = 1; i < widht - 2; i+=2)
        {
            for (int j = 1; j < height - 2; j+=2)
            {
                int randChance = rnd.Next(0, 100);
                if (randChance < roleRatio)
                {
                    Position p = new Position { x = i, y = j };
                    List<Neighbour> neighbours = new List<Neighbour>();
                    neighbours.Add(new Neighbour { position = new Position { x = p.x, y = p.y + 1 }, sharedWall = MazeCell.north });
                    neighbours.Add(new Neighbour { position = new Position { x = p.x, y = p.y - 1 }, sharedWall = MazeCell.south });
                    neighbours.Add(new Neighbour { position = new Position { x = p.x + 1, y = p.y }, sharedWall = MazeCell.east });
                    neighbours.Add(new Neighbour { position = new Position { x = p.x - 1, y = p.y }, sharedWall = MazeCell.west });

                    if (neighbours.Count > 0)
                    {
                        int randIndex = rnd.Next(0, neighbours.Count);
                        Neighbour randNeighbour = neighbours[randIndex];
                        Position nPosition = randNeighbour.position;
                        maze[p.x, p.y] &= ~randNeighbour.sharedWall;
                        maze[nPosition.x, nPosition.y] &= ~GetOpposingWall(randNeighbour.sharedWall);
                    }
                }
            }
        }
        }
        return maze;
        /*int randIndex = rnd.Next(0, neighbours.Count);
    Neighbour randNeighbour = neighbours[randIndex];
    Position nPosition = randNeighbour.position;
    maze[current.x, current.y] &= ~randNeighbour.sharedWall;
    maze[nPosition.x, nPosition.y] &= ~GetOpposingWall(randNeighbour.sharedWall);*/
    }
    private static List<Neighbour> GetNeighbours(Position pos, MazeCell[,] maze, uint widht, uint height,bool unvisitedOnly)
    {
        List<Neighbour> ngbList = new List<Neighbour>();
        //WEST
        if (pos.x > 0)
        {
            if (!maze[pos.x - 1, pos.y].HasFlag(MazeCell.marked))
            {
                ngbList.Add(new Neighbour {
                    position = new Position { x =pos.x-1,y=pos.y},
                    sharedWall = MazeCell.west
                });
            }
        }
        //SOUTH
        if (pos.y > 0)
        {
            if (!maze[pos.x, pos.y-1].HasFlag(MazeCell.marked))
            {
                ngbList.Add(new Neighbour
                {
                    position = new Position { x = pos.x, y = pos.y-1 },
                    sharedWall = MazeCell.south
                });
            }
        }
        //NORTH
        if (pos.y < height-1)
        {
            if (!maze[pos.x, pos.y + 1].HasFlag(MazeCell.marked))
            {
                ngbList.Add(new Neighbour
                {
                    position = new Position { x = pos.x, y = pos.y + 1 },
                    sharedWall = MazeCell.north
                });
            }
        }
        //EAST
        if (pos.x < widht - 1)
        {
            if (!maze[pos.x+1, pos.y].HasFlag(MazeCell.marked))
            {
                ngbList.Add(new Neighbour
                {
                    position = new Position { x = pos.x+1, y = pos.y },
                    sharedWall = MazeCell.east
                });
            }
        }
        return ngbList;
    }
    public static MazeCell[,] Generate(uint width, uint height)
    {
        MazeCell[,] maze = new MazeCell[width, height];
        MazeCell init = MazeCell.west | MazeCell.east | MazeCell.north | MazeCell.south;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                maze[i, j] = init;
            }
        }
        return RecursiveBacktracker(maze,width,height);
    }
}
