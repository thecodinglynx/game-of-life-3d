using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour 
{
    public GameObject cube;

    public int xCubes;
    public int yCubes;
    public int zCubes;

    [Range(0.0f, 1.0f)]
    public double lifeProbability;
   
    private GameObject[,,] matrix;

    private float scale = 0.9f;
    private Vector3 scaleVector;

    private Dictionary<String, ISetup> initialSetups = new Dictionary<String, ISetup>();

    void Start ()
    {
        initialSetups.Add("flipper", new Flipper());

        ISetup setup;
        initialSetups.TryGetValue("flipper", out setup);

        // bool[,,] firstGeneration = setup.getSetup(xCubes, yCubes, zCubes);
        bool[,,] firstGeneration = createProbabilisticFirstGeneration();
            
        initialSetup(firstGeneration);

        InvokeRepeating("nextGeneration", 1.0f, 1.0f);
    }

    private void initialSetup(bool[,,] firstGeneration)
    {
        matrix = new GameObject[xCubes, yCubes, zCubes];
        scaleVector = new Vector3(scale, scale, scale);

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                for (int k = 0; k < matrix.GetLength(2); k++)
                {
                    GameObject cubeClone = Instantiate(
                        cube, 
                        new Vector3(i, j, k), 
                        Quaternion.identity
                    );

                    cubeClone.transform.localScale = scaleVector;
                    Renderer renderer = cubeClone.GetComponent<Renderer>();

                    if (firstGeneration[i, j, k])
                    {
                        renderer.enabled = true;
                    }
                    else
                    {
                        renderer.enabled = false;
                    }

                    matrix[i, j, k] = cubeClone;
                }
            }
        }
    }

    private int currentNeighbours(int x, int y, int z)
    {
        int neighbours = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    int curX = x+i;
                    int curY = y+j;
                    int curZ = z+k;

                    if (x == curX && y == curY && z == curZ)
                    {
                        // this is the cube under test and since the
                        // current state of the cube is ignored, we 
                        // continue at this point
                        continue;
                    }

                    if (curX >= 0 && curX < xCubes
                        && curY >= 0 && curY < yCubes
                        && curZ >= 0 && curZ < zCubes)
                    {
                        if ( matrix[curX, curY, curZ].GetComponent<Renderer>().enabled )
                        {
                            neighbours++;
                        }
                    }
                }
            }
        }

        return neighbours;
    }

    private void nextGeneration()
    {
        bool[,,] newArr = new bool[xCubes, yCubes, zCubes];

        // calculate the next generation
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                for (int k = 0; k < matrix.GetLength(2); k++)
                {
                    int neighbours = currentNeighbours(i, j, k);

                    GameObject curCube = matrix[i, j, k];
                    Renderer renderer = curCube.GetComponent<Renderer>();

                    if (renderer.enabled)
                    {
                        // rule 1 - underpopulation, you die
                        if (neighbours <= 1)
                        {
                            newArr[i, j, k] = false;
                            continue;
                        }

                        // rule 2 - you survive
                        if (neighbours == 2 || neighbours == 3)
                        {
                            newArr[i, j, k] = true;
                            continue;
                        }

                        // rule 3 - overpopulation, you die
                        if (neighbours > 3)
                        {
                            newArr[i, j, k] = false;
                            continue;
                        }
                    }
                    else
                    {
                        // rule 4 - you are born
                        if (neighbours == 3)
                        {
                            newArr[i, j, k] = true;
                        }
                    }
                }
            }
        }

        // update the visibility of all cubes
        for (int i = 0; i < newArr.GetLength(0); i++)
        {
            for (int j = 0; j < newArr.GetLength(1); j++)
            {
                for (int k = 0; k < newArr.GetLength(2); k++)
                {
                    GameObject curCube = matrix[i, j, k];
                    Renderer renderer = curCube.GetComponent<Renderer>();

                    renderer.enabled = newArr[i, j, k];
                }
            }
        }
    }

    private bool[,,] stableSquare()
    {
        bool[,,] arr = new bool[xCubes, yCubes, zCubes];

        arr[5,5,0] = true;
        arr[6,5,0] = true;
        arr[5,6,0] = true;
        arr[6,6,0] = true;

        return arr;
    }

    private bool[,,] all()
    {
        bool[,,] arr = new bool[xCubes, yCubes, zCubes];

        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                for (int k = 0; k < arr.GetLength(2); k++)
                {
                    arr[i, j, k] = true;
                }
            }
        }

        return arr;
    }

    private bool[,,] createProbabilisticFirstGeneration()
    {
        bool[,,] arr = new bool[xCubes, yCubes, zCubes];

        System.Random random = new System.Random();

        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                for (int k = 0; k < arr.GetLength(2); k++)
                {
                    if (random.NextDouble() >= (1 - lifeProbability))
                    {
                        arr[i, j, k] = true;
                    }
                }
            }
        }

        return arr;
    }
}

