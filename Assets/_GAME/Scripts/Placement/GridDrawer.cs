using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Vector2 gridOrigin = Vector2.zero;
    public Material lineMaterial;

    private void Start()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        for (int x = 0; x <= width; x++)
        {
            DrawLine(
                new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y, 0),
                new Vector3(gridOrigin.x + x * cellSize, gridOrigin.y + height * cellSize, 0)
            );
        }

        for (int y = 0; y <= height; y++)
        {
            DrawLine(
                new Vector3(gridOrigin.x, gridOrigin.y + y * cellSize, 0),
                new Vector3(gridOrigin.x + width * cellSize, gridOrigin.y + y * cellSize, 0)
            );
        }
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("GridLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.03f;
        lr.endWidth = 0.03f;
        lr.sortingLayerName = "Ground";
        lr.sortingOrder = 2;
    }
}
