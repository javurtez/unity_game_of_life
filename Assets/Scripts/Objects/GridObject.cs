using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField]
    private int currentGenerationCount;
    [SerializeField]
    private float simulationSpeed = 1f;

    [SerializeField]
    private CellObject cellObject;
    private CellObject[,] cellObjectArray;

    private List<CellObject> cellPoolObject = new List<CellObject>();

    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(30, 30);

    private GameObject gridParent;

    private IEnumerator simulateIEnum;

    public bool IsSameSize => cellObjectArray.GetLength(0) == gridSize.x && cellObjectArray.GetLength(1) == gridSize.y;
    public float SimulationSpeed
    {
        get => simulationSpeed;
        set
        {
            simulationSpeed = value;
        }
    }
    public Vector2Int GridSize
    {
        get => gridSize;
        set
        {
            gridSize = value;
        }
    }

    public delegate void CellDataUpdate(int value);
    public static event CellDataUpdate OnGenerationChange;
    public static event CellDataUpdate OnCellCountChange;

    private void Start()
    {
        simulateIEnum = Simulate();
    }

    public void Simulate(bool isSimulate)
    {
        if (isSimulate)
        {
            StartCoroutine(simulateIEnum);
        }
        else
        {
            StopCoroutine(simulateIEnum);
        }
    }
    private IEnumerator Simulate()
    {
        while (true)
        {
            yield return new WaitForSeconds(simulationSpeed);
            NextGeneration();
        }
    }

    public void NextGeneration()
    {
        if (gridParent == null)
        {
            Debug.Log("Randomize Grid First!");
            return;
        }

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (cellObjectArray[x, y].IsAlive)
                {
                    cellObjectArray[x, y].tempAlive = GetCellNeighborCount(x, y) == 2 || GetCellNeighborCount(x, y) == 3;
                }
                else
                {
                    cellObjectArray[x, y].tempAlive = GetCellNeighborCount(x, y) == 3;
                }
            }
        }

        int cellCount = 0;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                cellObjectArray[x, y].SetAlive(cellObjectArray[x, y].tempAlive);
                if (cellObjectArray[x, y].IsAlive)
                {
                    cellCount++;
                }
            }
        }

        if (cellCount == 0 && currentGenerationCount == 0)
        {
            Debug.Log("Randomize Grid First!");
            return;
        }
        currentGenerationCount++;

        OnGenerationChange?.Invoke(currentGenerationCount);
        OnCellCountChange?.Invoke(cellCount);
    }

    public void GenerateGrid()
    {
        if (gridParent != null)
        {
            // Add to Pool Object
            for (int x = 0; x < cellObjectArray.GetLength(0); x++)
            {
                for (int y = 0; y < cellObjectArray.GetLength(1); y++)
                {
                    try
                    {
                        CellObject cell = cellObjectArray[x, y];
                        cell.SetAlive(false);
                        if (!IsSameSize)
                        {
                            cell.gameObject.SetActive(false);
                            cell.transform.SetParent(transform);
                            cellPoolObject.Add(cell);
                        }
                    }
                    catch { }
                }
            }

            if (IsSameSize)
            {
                Debug.Log("Same Grid Size! Hide Cells!");
                return;
            }
        }
        else
        {
            Transform cellParent = new GameObject("Grid Parent").transform;
            gridParent = cellParent.gameObject;
            gridParent.transform.position = transform.position;
        }

        Vector2 cellSize = new Vector2(cellObject.GetComponent<SpriteRenderer>().bounds.size.x, cellObject.GetComponent<SpriteRenderer>().bounds.size.y);

        cellObjectArray = new CellObject[(int)gridSize.x, (int)gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // This project version is using Unity 2020.3 LTS
                // Built-in object pooling is only available in Unity 2021
                CellObject cell;
                if (cellPoolObject.Count == 0)
                {
                    // Create cells if object pool list is empty
                    cell = Instantiate(cellObject, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    // Get cells from the Pool Object
                    cell = cellPoolObject[cellPoolObject.Count - 1];
                    cell.gameObject.SetActive(true);
                    cellPoolObject.Remove(cell);
                }

                cell.transform.SetParent(gridParent.transform);
                cell.transform.localPosition = new Vector3(-(cellSize.x * gridSize.x / 2) + cellSize.x * x, -(cellSize.y * gridSize.y / 2) + cellSize.y * y, 0);
                cell.cellPosition = new Vector2(x, y);
                cell.name = $"Cell_Object {x}_{y}";

                cellObjectArray[x, y] = cell;
            }
        }

        currentGenerationCount = 0;
        OnGenerationChange?.Invoke(currentGenerationCount);
        OnCellCountChange?.Invoke(0);
    }

    public void RandomizeGrid()
    {
        if (gridParent == null || !IsSameSize)
        {
            Debug.Log("Generating Grid... Randomize Grid...");
            GenerateGrid();
        }

        int cellCount = 0;
        currentGenerationCount = 0;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                cellObjectArray[x, y].SetAlive(Random.Range(0, 2) == 0);
                if (cellObjectArray[x, y].IsAlive)
                {
                    cellCount++;
                }
            }
        }

        OnGenerationChange?.Invoke(currentGenerationCount);
        OnCellCountChange?.Invoke(cellCount);
    }

    private int GetCellNeighborCount(int x, int y)
    {
        int neighbors = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                try
                {
                    if (cellObjectArray[x + i, y + j].IsAlive && cellObjectArray[x + i, y + j] != cellObjectArray[x, y])
                    {
                        neighbors++;
                    }
                }
                catch { }
            }
        }

        return neighbors;
    }
}
