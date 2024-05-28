using UnityEngine;
using VInspector;

public class ChessFigureSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private ChessFigureType figureToSpawn;
    [SerializeField] private ChessSide sideToSpawn;

    [Space(10)]
    [SerializeField] private SerializedDictionary<ChessFigureType, ChessFigure> figureMap;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ChessboardSquare chessSquare = hit.collider.GetComponent<ChessboardSquare>();
                if (chessSquare != null && chessSquare.IsEmpty())
                {
                    SpawnPawn(chessSquare);
                }
            }
        }
    }

    private void SpawnPawn(ChessboardSquare square)
    {
        ChessFigure pawn = Instantiate(figureMap[figureToSpawn], square.transform.position, Quaternion.identity);
        pawn.InitChessFigure(square, sideToSpawn);
    }
}
