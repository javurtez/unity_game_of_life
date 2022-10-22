using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridObject cellGrid;

    public void OnEnable()
    {
        CustomizationPanel.OnSimulationSpeed += SimulationChange;
        CustomizationPanel.OnSimulate += SimulationChange;

        CustomizationPanel.OnNext += cellGrid.NextGeneration;
        CustomizationPanel.OnReset += cellGrid.GenerateGrid;
        CustomizationPanel.OnRandomize += cellGrid.RandomizeGrid;

        CustomizationPanel.CellWidth += Width;
        CustomizationPanel.CellHeight += Height;
    }
    public void OnDisable()
    {
        CustomizationPanel.OnSimulationSpeed -= SimulationChange;
        CustomizationPanel.OnSimulate -= SimulationChange;

        CustomizationPanel.OnNext -= cellGrid.NextGeneration;
        CustomizationPanel.OnReset -= cellGrid.GenerateGrid;
        CustomizationPanel.OnRandomize -= cellGrid.RandomizeGrid;

        CustomizationPanel.CellWidth -= Width;
        CustomizationPanel.CellHeight -= Height;
    }

    private void Height(int height)
    {
        cellGrid.GridSize = new Vector2Int(cellGrid.GridSize.x, height);
    }
    private void Width(int width)
    {
        cellGrid.GridSize = new Vector2Int(width, cellGrid.GridSize.y);
    }

    private void SimulationChange(float speed)
    {
        cellGrid.SimulationSpeed = speed;
    }
    private void SimulationChange(bool simulate)
    {
        cellGrid.Simulate(simulate);
    }
}
