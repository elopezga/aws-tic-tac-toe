using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowController : MonoBehaviour
{
    public Action OnPiecePlaced;
    public Action OnStateUpdate;

    [SerializeField]
    private CellController LeftCell;

    [SerializeField]
    private CellController MiddleCell;

    [SerializeField]
    private CellController RightCell;

    public RowState state;

    void Start()
    {
        LeftCell.OnPiecePlaced += OnPiecePlaced;
        MiddleCell.OnPiecePlaced += OnPiecePlaced;
        RightCell.OnPiecePlaced += OnPiecePlaced;

        LeftCell.OnPiecePlaced += UpdateState;
        MiddleCell.OnPiecePlaced += UpdateState;
        RightCell.OnPiecePlaced += UpdateState;
    }

    void OnDestroy()
    {
        LeftCell.OnPiecePlaced -= OnPiecePlaced;
        MiddleCell.OnPiecePlaced -= OnPiecePlaced;
        RightCell.OnPiecePlaced -= OnPiecePlaced;

        LeftCell.OnPiecePlaced -= OnPiecePlaced;
        MiddleCell.OnPiecePlaced -= OnPiecePlaced;
        RightCell.OnPiecePlaced -= OnPiecePlaced;
    }

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

    private void UpdateState()
    {
        state.leftCell.ownerid = LeftCell.state.OwnerId;
        state.leftCell.piece = LeftCell.state.PlayerPiece;

        state.middleCell.ownerid = MiddleCell.state.OwnerId;
        state.middleCell.piece = MiddleCell.state.PlayerPiece;

        state.rightCell.ownerid = RightCell.state.OwnerId;
        state.rightCell.piece = RightCell.state.PlayerPiece;

        OnStateUpdate.Invoke();
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