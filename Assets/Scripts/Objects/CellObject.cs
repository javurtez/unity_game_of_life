using UnityEngine;

public class CellObject : MonoBehaviour
{
    public bool tempAlive;
    public Vector2 cellPosition = Vector2.zero;

    private bool isAlive;
    private SpriteRenderer spriteRenderer;

    public bool IsAlive => isAlive;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseUp()
    {
        SetAlive(!isAlive);
    }

    public void SetAlive(bool value)
    {
        isAlive = value;

        spriteRenderer.color = isAlive ? Color.black : Color.white;
    }
}
