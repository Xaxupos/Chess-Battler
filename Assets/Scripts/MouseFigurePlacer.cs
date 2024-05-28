using UnityEngine;

public class MouseFigurePlacer : MonoBehaviour
{
    public ChessSide side;
    [SerializeField] private ChessFigure pawnPrefab;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("hit");
                ChessboardSquare chessSquare = hit.collider.GetComponent<ChessboardSquare>();
                if (chessSquare != null && chessSquare.IsEmpty())
                {
                    Debug.Log("ciach");
                    SpawnPawn(chessSquare);
                }
            }
        }
    }

    private void SpawnPawn(ChessboardSquare square)
    {
        ChessFigure pawn = Instantiate(pawnPrefab, square.transform.position, Quaternion.identity);

        pawn.InitChessFigure(square, side);
    }
}
