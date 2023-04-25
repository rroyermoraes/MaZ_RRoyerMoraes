using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using A_Star_Model;
using DephtFirstSearch_Model;
using DephtSearch_Model;
using UnityEngine.UI;
using TMPro;

public class MazRenderer : MonoBehaviour
{
    
    [SerializeField]
    public uint width=10;
    [SerializeField]
    public uint height=10;
    [SerializeField]
    private Transform wandPrefab = null;
    [SerializeField]
    private Transform startPrefab = null;
    //[SerializeField]
    //private Transform pathPrefab = null;
    [SerializeField]
    private Transform endPrefab = null;

    private A_Star a_sAgent;
    private DephtLimitedSearch dlsAgent;
    private DephtFirstSearch dfsAgent;
    private MazeCell[,] maze;
    private Transform startPos;
    private IEnumerator tracePathA;
    private IEnumerator tracePathB;
    private IEnumerator tracePathC;

    private uint dlsDephtFactor = 1;
    public TrailRenderer trailerA;
    public TrailRenderer trailerB;
    public TrailRenderer trailerC;
    public LineRenderer lineA;

    public RectTransform DLSErrorPanel;
    public TextMeshProUGUI logConsole;
    public float moveSpeed = 0.5f;

    
    private void DrawMaze(MazeCell[,] maze)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                MazeCell node = maze[i, j];

                Vector3 position = new Vector3(-width / 2 + i, 0, -height / 2 + j);
                if (node.HasFlag(MazeCell.start))//START POSITION
                {
                    Transform startP = Instantiate(startPrefab, transform);
                    startP.position = position + new Vector3(0, -0.1f, 0);
                    startPos = startP;
                    trailerA.Clear();
                    trailerB.Clear();
                    trailerC.Clear();
                }
                if (node.HasFlag(MazeCell.end))//END POSITION
                {                    
                    Transform endP = Instantiate(endPrefab, transform);
                    endP.position = position + new Vector3(0, -0.1f, 0);
                }
                if (node.HasFlag(MazeCell.north))//GENERTE NORH WALLS
                {
                    Transform topWand = Instantiate(wandPrefab, transform);
                    topWand.position = position + new Vector3(0, 0, 0.5f);
                }
                if (node.HasFlag(MazeCell.west))//GENERATE WEST WALLS
                {
                    Transform leftWand = Instantiate(wandPrefab, transform);
                    leftWand.position = position + new Vector3(-0.5f, 0, 0);
                    leftWand.eulerAngles = new Vector3(0, 90, 0);
                }
                if (i == width - 1)//GENERATE MISSING EAST WALLS
                {
                    if (node.HasFlag(MazeCell.east))
                    {
                        Transform rightWand = Instantiate(wandPrefab, transform);
                        rightWand.position = position + new Vector3(0.5f, 0, 0);
                        rightWand.eulerAngles = new Vector3(0, 90, 0);
                    }
                }
                if (j == 0)//GENERATE MISSING SOUTH WALLS
                {
                    if (node.HasFlag(MazeCell.south))
                    {
                        Transform downWand = Instantiate(wandPrefab, transform);
                        downWand.position = position + new Vector3(0, 0, -0.5f);
                    }
                }
            }
        }
        logConsole.text = "A " + width + " by " + height + " maze was generated.";
    }

    public void SliderW(float f)
    {
        width = (uint)f;
    }
    public void SliderH(float f)
    {
        height = (uint)f;
    }
    public void SliderDLS(float f)
    {
        dlsDephtFactor = (uint)f;
    }
    void FilterPath(List<Position> path)
    {
        List<Position> newPath= new List<Position>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//GENERATE NEW MAZE
        {
            GenerateNewMaze();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            A_StarGO();
            DLS_GO();
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }
    public void A_StarGO()
    {
        if (maze != null)
        {
            List<Position> pathA = new List<Position>();
            trailerA.transform.position = startPos.position;
            trailerA.Clear();
            pathA = a_sAgent.FindPath(a_sAgent.NodeGrid[a_sAgent.StartPos.x, a_sAgent.StartPos.y], a_sAgent.NodeGrid[a_sAgent.EndPos.x, a_sAgent.EndPos.y]);
            if (tracePathA != null)
            {
                StopCoroutine(tracePathA);
            }
            tracePathA = TracePath(moveSpeed, pathA, trailerA);
            StartCoroutine(tracePathA);
            logConsole.text = "The A* agent solved the maze with " + a_sAgent.GetLastRunSteps() + " checks, and found a solution " + pathA.Count + " steps long.";
        }
    }
    public void DLS_GO()
    {
        if (maze != null)
        {
            List<Position> pathB = new List<Position>();
            trailerB.transform.position = startPos.position;
            trailerB.Clear();
            pathB = dlsAgent.FindPath(dlsAgent.NodeGrid[dlsAgent.StartPos.x, dlsAgent.StartPos.y], dlsAgent.NodeGrid[dlsAgent.EndPos.x, dlsAgent.EndPos.y], dlsDephtFactor);
            if (tracePathB != null)
            {
                StopCoroutine(tracePathB);
            }
            if (pathB != null){
                tracePathB = TracePath(moveSpeed, pathB, trailerB);
                StartCoroutine(tracePathB);
                logConsole.text = "The DLS agent solved the maze with " + dlsAgent.GetLastRunSteps() + " checks, and found a solution " + pathB.Count + " steps long. With a search depht of: "+ dlsDephtFactor;
            }
            else
            {
                //DLSErrorPanel.gameObject.SetActive(true);
                logConsole.text = "The DLS agent could not solve the maze with " + dlsAgent.GetLastRunSteps() + " checks. With a search depht of: " + dlsDephtFactor;
            }
        }       
    }

    public void DFS_GO()
    {
        if (maze != null)
        {
            List<Position> pathC = new List<Position>();
            trailerC.transform.position = startPos.position;
            trailerC.Clear();
            pathC = dfsAgent.FindPath(dfsAgent.NodeGrid[dfsAgent.StartPos.x, dfsAgent.StartPos.y], dfsAgent.NodeGrid[dfsAgent.EndPos.x, dfsAgent.EndPos.y]);
            if (tracePathC != null)
            {
                StopCoroutine(tracePathC);
            }
            if (pathC != null)
            {
                tracePathC = TracePath(moveSpeed, pathC, trailerC);
                StartCoroutine(tracePathC);
                logConsole.text = "The DFS agent solved the maze with " + dfsAgent.GetLastRunSteps() + " checks, and found a solution " + pathC.Count + " steps long.";
            }
        }
    }
    public void ClearAgents()
    {
        trailerB.transform.position = startPos.position;
        trailerB.Clear();
        trailerA.transform.position = startPos.position;
        trailerA.Clear();
        trailerC.transform.position = startPos.position;
        trailerC.Clear();
        if (tracePathA != null)
        {
            StopCoroutine(tracePathA);
        }
        if (tracePathB != null)
        {
            StopCoroutine(tracePathB);
        }
        if (tracePathC != null)
        {
            StopCoroutine(tracePathC);
        }
    }
    public void GenerateNewMaze()
    {
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        maze = MazeGenerator.Generate(width, height);
        DrawMaze(maze);
        a_sAgent = new A_Star(maze, width, height);
        dlsAgent = new DephtLimitedSearch(maze, width, height);
        dfsAgent = new DephtFirstSearch(maze, width, height);
    }
    private IEnumerator TracePath(float waitTime, List<Position> path,TrailRenderer trailer)
    {            
        foreach (Position n in path)
        {            
            //yield return new WaitForSeconds(waitTime);
            Vector3 targetPosition = new Vector3(-width / 2 + n.x, -0.1f, -height / 2 + n.y);
            trailer.emitting = true;
            float elapsedTime = 0;
            
            while (elapsedTime <= waitTime)
            {
                trailer.transform.position = Vector3.MoveTowards(trailer.transform.position, targetPosition,elapsedTime/waitTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            trailer.transform.position = targetPosition;

        }
        
    }
}
