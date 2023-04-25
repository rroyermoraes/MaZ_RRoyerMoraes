using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFit : MonoBehaviour
{
    public MazRenderer maze;
    public float adjust = 0.6f;

    private Camera c;

    private void Awake()
    {
        c = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        c.orthographicSize = (maze.height + maze.width) / 2 * 0.6f;
    }
}
