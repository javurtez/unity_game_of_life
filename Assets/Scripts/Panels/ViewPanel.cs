using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ViewPanel : MonoBehaviour
{
    public TextMeshProUGUI generationText;
    public TextMeshProUGUI cellCountText;

    public void OnEnable()
    {
        GridObject.OnGenerationChange += OnUpdateGeneration;
        GridObject.OnCellCountChange += OnUpdateCellCount;
    }
    public void OnDisable()
    {
        GridObject.OnGenerationChange -= OnUpdateGeneration;
        GridObject.OnCellCountChange -= OnUpdateCellCount;
    }

    private void OnUpdateGeneration(int generation)
    {
        generationText.text = $"Generation: {generation}";
    }
    private void OnUpdateCellCount(int count)
    {
        cellCountText.text = $"Cell Count: {count}";
    }
}