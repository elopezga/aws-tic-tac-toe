using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Action OnPiecePlaced;

    [SerializeField]
    private RowController bottomRow;
    [SerializeField]
    private RowController middleRow;
    [SerializeField]
    private RowController topRow;

    private GridState state;

    void Start()
    {
        bottomRow.OnPiecePlaced += OnPiecePlaced;
        middleRow.OnPiecePlaced += OnPiecePlaced;
        topRow.OnPiecePlaced += OnPiecePlaced;
    }

    void OnDestroy()
    {
        bottomRow.OnPiecePlaced -= OnPiecePlaced;
        middleRow.OnPiecePlaced -= OnPiecePlaced;
        topRow.OnPiecePlaced -= OnPiecePlaced;
    }

    public void DisablePlacingPiece()
    {
        bottomRow.DisablePlacingPiece();
        middleRow.DisablePlacingPiece();
        topRow.DisablePlacingPiece();
    }

    public void EnablePlacingPiece()
    {
        bottomRow.EnablePlacingPiece();
        middleRow.EnablePlacingPiece();
        topRow.EnablePlacingPiece();
    }

    public void SetState(GridState state)
    {
        this.state = state;

        bottomRow.SetState(new RowState(){
            leftCell = state.bottomRow[0],
            middleCell = state.bottomRow[1],
            rightCell = state.bottomRow[2],
            currentTurnId = state.currentTurnId,
            currentTurnPiece = state.currentTurnPiece
        });

        middleRow.SetState(new RowState(){
            leftCell = state.middleRow[0],
            middleCell = state.middleRow[1],
            rightCell = state.middleRow[2],
            currentTurnId = state.currentTurnId,
            currentTurnPiece = state.currentTurnPiece
        });

        topRow.SetState(new RowState(){
            leftCell = state.topRow[0],
            middleCell = state.topRow[1],
            rightCell = state.topRow[2],
            currentTurnId = state.currentTurnId,
            currentTurnPiece = state.currentTurnPiece
        });
    }
}

public class GridState
{
    public CellStatePayload[] bottomRow;
    public CellStatePayload[] middleRow;
    public CellStatePayload[] topRow;
    public string currentTurnId;
    public string currentTurnPiece;
}