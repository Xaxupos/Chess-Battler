using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class FormationsManager : MonoBehaviour
{
    public Chessboard chessboard;
    public List<FormationData> allGameFormations;
    public List<FigureActiveFormation> figureActiveFormations;

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

        foreach (var formation in allGameFormations)
        {
            foreach (var square in lowerHalf)
            {
                if (CheckFormationAtPosition(formation, square.GetBoardPosition()))
                {
                    Debug.Log($"Found formation: {formation.formationName} at position {square.GetBoardPosition()}");

                    FigureActiveFormation formationToAdd = new FigureActiveFormation
                    {
                        formationData = formation,
                        chessboardSquares = new List<Vector2Int>()
                    };

                    foreach (var pair in formation.positionPiecePairs)
                    {
                        Vector2Int boardPosition = square.GetBoardPosition() + pair.position;
                        formationToAdd.chessboardSquares.Add(boardPosition);
                    }

                    ActivateFormation(formationToAdd);
                }
            }
        }
    }

    public void ActivateFormation(FigureActiveFormation formation)
    {
        foreach(var squarePos in formation.chessboardSquares)
        {
            chessboard.GetSquareAtPosition(squarePos).CurrentFigure.formationController.SetCurrentFormationAndApplyBonus(formation);
        }

        figureActiveFormations.Add(formation);
        HighlightFormation(formation);

    }

    public void ScanActiveFormations()
    {
        for (int i = figureActiveFormations.Count - 1; i >= 0; i--)
        {
            var activeFormation = figureActiveFormations[i];
            if (!IsFormationStillValid(activeFormation))
            {
                Debug.Log($"Formation {activeFormation.formationData.formationName} is no longer valid.");

                foreach (var squarePos in activeFormation.chessboardSquares)
                {
                    if (chessboard.GetSquareAtPosition(squarePos).CurrentFigure == null) continue;

                    chessboard.GetSquareAtPosition(squarePos).CurrentFigure.formationController.ClearCurrentFormationAndRevertBonus();
                }

                figureActiveFormations.RemoveAt(i);
            }
        }
    }

    private bool IsFormationStillValid(FigureActiveFormation formation)
    {
        var startPosition = formation.chessboardSquares[0];
        foreach (var squarePos in formation.chessboardSquares)
        {
            ChessboardSquare square = chessboard.GetSquareAtPosition(squarePos);
            Vector2Int relativePosition = squarePos - startPosition;
            int pieceTypeInt = formation.formationData.GetPieceTypeAtPosition(relativePosition);
            if (square == null || square.CurrentFigure == null ||
                (int)square.CurrentFigure.figureType != pieceTypeInt || pieceTypeInt == -1)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckFormationAtPosition(FormationData formation, Vector2Int startPosition)
    {
        foreach (var pair in formation.positionPiecePairs)
        {
            Vector2Int boardPosition = startPosition + pair.position;
            ChessboardSquare square = chessboard.GetSquareAtPosition(boardPosition);

            if (square == null) return false;
            if (square.CurrentFigure == null) return false;
            if (square.CurrentFigure.figureType != pair.pieceType) return false;
            if (square.CurrentFigure.formationController.IsPartOfFormation()) return false;
        }
        return true;
    }

    public void HighlightFormation(FigureActiveFormation formation)
    {
        List<Vector2Int> formationPositions = formation.chessboardSquares;

        foreach (var position in formationPositions)
        {
            ChessboardSquare square = chessboard.GetSquareAtPosition(position);

            if (square != null)
            {
                GameEffectsDatabase.Instance.PlayVFX(formation.formationData.formationActionType, square.transform.position, square.transform);
                GameEffectsDatabase.Instance.PlaySFX(formation.formationData.formationActionType);
            }
        }

        Vector3 centerPosition = GetCenterPosition(formationPositions);
        DisplayFormationName(formation.formationData, centerPosition);
    }

    private Vector3 GetCenterPosition(List<Vector2Int> positions)
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var pos in positions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

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