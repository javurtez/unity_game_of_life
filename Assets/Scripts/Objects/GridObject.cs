using System.Collections;
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

    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(30, 30);

    private GameObject gridParent;

    private IEnumerator simulateIEnum;

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

    private void Awake()
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
        yield return new WaitForSeconds(simulationSpeed);
        while (true)
        {
            NextGeneration();
            yield return new WaitForSeconds(simulationSpeed);
        }
    }

    public void NextGeneration()
    {
        if (gridParent == null) return;

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

        if (cellCount == 0) return;
        currentGenerationCount++;

        OnGenerationChange?.Invoke(currentGenerationCount);
        OnCellCountChange?.Invoke(cellCount);
    }

    public void GenerateGrid()
    {
        if (this.gridParent != null)
        {
            Destroy(this.gridParent);
        }

        Transform gridParent = new GameObject("Grid Parent").transform;

        Vector2 cellSize = new Vector2(cellObject.GetComponent<SpriteRenderer>().bounds.size.x, cellObject.GetComponent<SpriteRenderer>().bounds.size.y);

        cellObjectArray = new CellObject[(int)gridSize.x, (int)gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                CellObject cell = Instantiate(cellObject, new Vector3(-(cellSize.x * gridSize.x / 2) + cellSize.x * x, -(cellSize.y * gridSize.y / 2) + cellSize.y * y, 0), Quaternion.identity);
                cell.transform.SetParent(gridParent);
                cell.cellPosition = new Vector2(x, y);
                cell.name = $"Cell_Object {x}_{y}";

                cellObjectArray[x, y] = cell;
            }
        }

        this.gridParent = gridParent.gameObject;
        gridParent.position = transform.position;

        currentGenerationCount = 0;
        OnGenerationChange?.Invoke(currentGenerationCount);
        OnCellCountChange?.Invoke(0);
    }

    public void RandomizeGrid()
    {
        if (gridParent == null)
        {
            GenerateGrid();
            RandomizeGrid();
            return;
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
