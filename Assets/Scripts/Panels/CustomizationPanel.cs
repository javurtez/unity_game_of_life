using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationPanel : MonoBehaviour
{
    public Slider slider;
    public Toggle simulateToggle;

    public Slider widthSlider, heightSlider;
    public TextMeshProUGUI widthText, heightText;

    private bool isStart = true;

    public static Action<float> OnSimulationSpeed;
    public static Action<bool> OnSimulate;
    public static Action OnNext;
    public static Action OnReset;
    public static Action OnRandomize;

    public delegate void CellGrid(int value);
    public static event CellGrid CellHeight;
    public static event CellGrid CellWidth;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.5f);
        if (simulateToggle.isOn)
        {
            OnToggleSimulate(simulateToggle.isOn);
            SetSimulationSpeed(slider.value);
            OnRandomizeGrid();
        }
        Width((int)widthSlider.value);
        Height((int)heightSlider.value);
    }

    public void Height(float value)
    {
        heightText.text = $"Width: {(int)value}";
        CellHeight?.Invoke((int)value);
    }
    public void Width(float value)
    {
        widthText.text = $"Height: {(int)value}";
        CellWidth?.Invoke((int)value);
    }

    // Is called in the slider
    public void SetSimulationSpeed(float value)
    {
        OnSimulationSpeed?.Invoke(value);
    }
    // Is called in the toggle
    public void OnToggleSimulate(bool toggle)
    {
        OnSimulate?.Invoke(toggle);
        if (isStart) isStart = false;
        else return;
        OnRandomizeGrid();
    }
    public void OnNextGeneration()
    {
        OnNext?.Invoke();
    }
    public void OnGenerateGrid()
    {
        if (isStart) isStart = false;
        OnReset?.Invoke();
    }
    public void OnRandomizeGrid()
    {
        if (isStart) isStart = false;
        OnRandomize?.Invoke();
    }
}
