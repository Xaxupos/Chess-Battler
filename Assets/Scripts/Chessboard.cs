using System.Collections.Generic;
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
    public Vector2Int InitGridSize { get; private set; }

    private void Awake()
    {
        InitGridSize = new Vector2Int(gridXSize, gridYSize);
        GenerateGrid();
        CenterGrid();
    }

    public bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridXSize && position.y >= 0 && position.y < gridYSize;
    }

    public List<ChessboardSquare> GetUpperHalf()
    {
        List<ChessboardSquare> upperHalf = new List<ChessboardSquare>();

        for (int x = 0; x < gridXSize; x++)
        {
            for (int y = gridYSize / 2; y < gridYSize; y++)
            {
                upperHalf.Add(ChessboardGrid[x, y]);
            }
        }

        return upperHalf;
    }

    public List<ChessboardSquare> GetLowerHalf()
    {
        List<ChessboardSquare> lowerHalf = new List<ChessboardSquare>();

        for (int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize / 2; y++)
            {
                lowerHalf.Add(ChessboardGrid[x, y]);
            }
        }

        return lowerHalf;
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

    private void GenerateGrid()
    {
        ChessboardGrid = new ChessboardSquare[gridXSize, gridYSize];

        for (int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                var squarePrefab = (x + y) % 2 == 0 ? whiteSquarePrefab : blackSquarePrefab;
                var square = Instantiate(squarePrefab, position, Quaternion.identity);

                square.Chessboard = this;
                square.transform.SetParent(transform);
                square.transform.localPosition = position;
                square.SetBoardPosition(new Vector2Int(x, y));
                ChessboardGrid[x, y] = square;
            }
        }
    }

    private void CenterGrid()
    {
        // Oblicz po³owê rozmiaru planszy w obu wymiarach
        float halfGridSizeX = ((gridXSize - 1) * cellSize) / 2.0f;
        float halfGridSizeY = ((gridYSize - 1) * cellSize) / 2.0f;

        // Utwórz wektor reprezentuj¹cy œrodek planszy
        Vector3 gridCenter = new Vector3(-halfGridSizeX, -halfGridSizeY, 0);

        // Ustaw pozycjê planszy
        transform.position = gridCenter;
    }

    public void ResizeGrid(int newX, int newY)
    {
        for (int x = 0; x < gridXSize; x++)
        {
            for (int y = 0; y < gridYSize; y++)
            {
                if (ChessboardGrid[x, y] != null)
                {
                    if (ChessboardGrid[x,y].CurrentFigure != null && ChessboardGrid[x,y].CurrentFigure.figureType == ChessFigureType.KING)
                    {
                        ChessboardGrid[x, y].CurrentFigure.transform.parent = null;
                    }
                    Destroy(ChessboardGrid[x, y].gameObject);
                    ChessboardGrid[x, y] = null;
                }
            }
        }

        gridXSize = newX;
        gridYSize = newY;

        GenerateGrid();
        CenterGrid();
    }
}