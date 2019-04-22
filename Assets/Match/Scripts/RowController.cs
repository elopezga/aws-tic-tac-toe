using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowController : MonoBehaviour
{
    [SerializeField]
    private CellController LeftCell;

    [SerializeField]
    private CellController MiddleCell;

    [SerializeField]
    private CellController RightCell;

    private RowState state;

    public void DisablePlacingPiece()
    {
        LeftCell.DisablePlacingPiece();
        MiddleCell.DisablePlacingPiece();
        RightCell.DisablePlacingPiece();
    }

    public void EnablePlacingPiece()
    {
        LeftCell.EnablePlacingPiece();
        MiddleCell.EnablePlacingPiece();
        RightCell.EnablePlacingPiece();
    }

    public void SetState(RowState state)
    {
        this.state = state;

        LeftCell.SetState(new CellState(){
            PlayerPiece = state.leftCell.piece,
            DisplayPiece = state.leftCell.piece,
            OwnerId = state.leftCell.ownerid,
            CurrentTurnId = state.currentTurnId,
            CurrentTurnPiece = state.currentTurnPiece
        });

        MiddleCell.SetState(new CellState(){
            PlayerPiece = state.middleCell.piece,
            DisplayPiece = state.middleCell.piece,
            OwnerId = state.middleCell.ownerid,
            CurrentTurnId = state.currentTurnId,
            CurrentTurnPiece = state.currentTurnPiece
        });

        RightCell.SetState(new CellState(){
            PlayerPiece = state.rightCell.piece,
            DisplayPiece = state.rightCell.piece,
            OwnerId = state.rightCell.ownerid,
            CurrentTurnId = state.currentTurnId,
            CurrentTurnPiece = state.currentTurnPiece
        });
    }
}


public class RowState
{
    public CellStatePayload leftCell;
    public CellStatePayload middleCell;
    public CellStatePayload rightCell;
    public string currentTurnId;
    public string currentTurnPiece;
}