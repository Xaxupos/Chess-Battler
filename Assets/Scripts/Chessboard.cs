using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChessboardSquare whiteSquarePrefab;
    [SerializeField] private ChessboardSquare blackSquarePrefab;

    [Header("Settings")]
    [SerializeField] private int gridXSize = 8;
    [SerializeField] private int gridYSize = 8;
    [SerializeField] private float cellSize = 1.0f;

    public ChessboardSquare[,] ChessboardGrid { get; private set; }

    private void Awake()
    {
        ChessboardGrid = new ChessboardSquare[gridXSize, gridYSize];

        GenerateGrid();
        CenterGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                var squarePrefab = (x + y) % 2 == 0 ? whiteSquarePrefab : blackSquarePrefab;
                var square = Instantiate(squarePrefab, position, Quaternion.identity);

                square.Chessboard = this;
                square.transform.SetParent(transform);
                square.SetBoardPosition(new Vector2Int(x, y));
                ChessboardGrid[x, y] = square;
            }
        }
    }

    private void CenterGrid()
    {
        Vector3 gridCenter = new Vector3((gridXSize-1 * cellSize) / 2, (gridYSize-1 * cellSize) / 2, 0);
        transform.position = -gridCenter;
    }

    public bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridXSize && position.y >= 0 && position.y < gridYSize;
    }

    public ChessboardSquare GetSquareAtPosition(Vector2Int position)
    {
        if (IsWithinBounds(position))
        {
            return ChessboardGrid[position.x, position.y];
        }
        return null;
    }

    public Vector2Int GetGridSize()
    {
        return new Vector2Int(gridXSize, gridYSize);
    }
}