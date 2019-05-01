using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Action OnPiecePlaced;
    public Action OnStateUpdate;

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

        bottomRow.OnStateUpdate += UpdateState;
        middleRow.OnStateUpdate += UpdateState;
        topRow.OnStateUpdate += UpdateState;
    }

    void OnDestroy()
    {
        bottomRow.OnPiecePlaced -= OnPiecePlaced;
        middleRow.OnPiecePlaced -= OnPiecePlaced;
        topRow.OnPiecePlaced -= OnPiecePlaced;

        bottomRow.OnStateUpdate -= UpdateState;
        middleRow.OnStateUpdate -= UpdateState;
        topRow.OnStateUpdate -= UpdateState;
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

    public string GetStateSerialized()
    {
        CellStatePayload[] bottomRow = new CellStatePayload[] {
            new CellStatePayload(){
                ownerid = this.bottomRow.state.leftCell.ownerid,
                piece = this.bottomRow.state.leftCell.piece
            },
            new CellStatePayload(){
                ownerid = this.bottomRow.state.middleCell.ownerid,
                piece = this.bottomRow.state.middleCell.piece
            },
            new CellStatePayload(){
                ownerid = this.bottomRow.state.rightCell.ownerid,
                piece = this.bottomRow.state.rightCell.piece
            }
        };

        CellStatePayload[] middleRow = new CellStatePayload[]{
            new CellStatePayload(){
                ownerid = this.middleRow.state.leftCell.ownerid,
                piece = this.middleRow.state.leftCell.piece
            },
            new CellStatePayload(){
                ownerid = this.middleRow.state.middleCell.ownerid,
                piece = this.middleRow.state.middleCell.piece
            },
            new CellStatePayload(){
                ownerid = this.middleRow.state.rightCell.ownerid,
                piece = this.middleRow.state.rightCell.piece
            }
        };

        CellStatePayload[] topRow = new CellStatePayload[]{
            new CellStatePayload(){
                ownerid = this.topRow.state.leftCell.ownerid,
                piece = this.topRow.state.leftCell.piece
            },
            new CellStatePayload(){
                ownerid = this.topRow.state.middleCell.ownerid,
                piece = this.topRow.state.middleCell.piece
            },
            new CellStatePayload(){
                ownerid = this.topRow.state.rightCell.ownerid,
                piece = this.topRow.state.rightCell.piece
            }
        };

        GameStatePayload payload = new GameStatePayload()
        {
            bottomRow = bottomRow,
            middleRow = middleRow,
            topRow = topRow
        };
        return JsonUtility.ToJson(payload);
    }

    private void UpdateState()
    {
        state.bottomRow = new CellStatePayload[]{bottomRow.state.leftCell, bottomRow.state.middleCell, bottomRow.state.rightCell};
        state.middleRow = new CellStatePayload[]{middleRow.state.leftCell, middleRow.state.middleCell, middleRow.state.rightCell};
        state.topRow = new CellStatePayload[]{topRow.state.leftCell, topRow.state.middleCell, topRow.state.rightCell};

        // TODO: update currentTurn info here or at server

        OnStateUpdate.Invoke();
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
