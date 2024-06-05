using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class FormationsManager : MonoBehaviour
{
    public Chessboard chessboard;

    public List<FormationData> formations;

    public static FormationsManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ScanForFormations()
    {
        var lowerHalf = chessboard.GetLowerHalf();

        foreach (var formation in formations)
        {
            foreach (var square in lowerHalf)
            {
                if (CheckFormationAtPosition(formation, square.GetBoardPosition()))
                {
                    Debug.Log($"Found formation: {formation.formationName} at position {square.GetBoardPosition()}");
                    HighlightFormation(formation, square.GetBoardPosition());
                }
            }
        }
    }

    public bool CheckFormationAtPosition(FormationData formation, Vector2Int startPosition)
    {
        foreach (var pair in formation.positionPiecePairs)
        {
            Vector2Int boardPosition = startPosition + pair.position;
            ChessboardSquare square = chessboard.GetSquareAtPosition(boardPosition);

            if (square == null || square.CurrentFigure == null || square.CurrentFigure.figureType != pair.pieceType)
            {
                return false;
            }
        }
        return true;
    }

    public void HighlightFormation(FormationData formation, Vector2Int startPosition)
    {
        List<Vector2Int> formationPositions = new List<Vector2Int>();

        foreach (var pair in formation.positionPiecePairs)
        {
            Vector2Int boardPosition = startPosition + pair.position;
            ChessboardSquare square = chessboard.GetSquareAtPosition(boardPosition);
            formationPositions.Add(boardPosition);

            if (square != null)
            {
                GameEffectsDatabase.Instance.PlayVFX(ActionType.TURTLE_HIGHLIGHT, square.transform.position, square.transform);
                GameEffectsDatabase.Instance.PlaySFX(ActionType.TURTLE_HIGHLIGHT);

            }
        }

        Vector3 averagePosition = GetCenterPosition(formationPositions);
        DisplayFormationName(formation, averagePosition);
    }

    private Vector3 GetCenterPosition(List<Vector2Int> positions)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        // ZnajdŸ minimalne i maksymalne wartoœci wspó³rzêdnych X i Y
        foreach (var pos in positions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        // Oblicz œrodek obszaru
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        return new Vector3(centerX * chessboard.GetCellSize(), centerY * chessboard.GetCellSize(), 0);
    }


    private void DisplayFormationName(FormationData formationData, Vector3 position)
    {
        var spriteGO = PoolManager.Instance.GetFromPool(ObjectType.SPRITE_ELEMENT);
        var sprite = spriteGO.GetComponent<SpriteRenderer>();

        if(sprite)
        {
            sprite.sprite = formationData.formationIcon;
        }

        spriteGO.transform.position = position;
        spriteGO.transform.SetParent(chessboard.transform, false);

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        var beforeScale = spriteGO.transform.localScale;
        spriteGO.transform.DOScale(sprite.transform.localScale.x * 1.2f, 2.0f);
        sprite.DOFade(0, 2.4f).OnComplete(()=>spriteGO.transform.localScale = beforeScale);

        PoolManager.Instance.ReleaseToPool(ObjectType.SPRITE_ELEMENT, spriteGO, 2.5f);
    }
}