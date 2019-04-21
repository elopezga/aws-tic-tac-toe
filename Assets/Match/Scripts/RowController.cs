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

    // Start is called before the first frame update
    void Start()
    {
        /* CellState state = new CellState()
        {
            PlayerPiece = "",
            DisplayPiece = ""
        };

        CellController cellController = new CellController();
        cellController.SetState(new CellState(){
            PlayerPiece = "",
            DisplayPiece = ""
        }); */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetState(RowState state)
    {
        this.state = state;

        LeftCell.SetState(new CellState(){
            PlayerPiece = state.leftCell.piece,
            DisplayPiece = state.leftCell.piece
        });

        MiddleCell.SetState(new CellState(){
            PlayerPiece = state.middleCell.piece,
            DisplayPiece = state.middleCell.piece
        });

        RightCell.SetState(new CellState(){
            PlayerPiece = state.rightCell.piece,
            DisplayPiece = state.rightCell.piece
        });
    }
}


public class RowState
{
    public CellStatePayload leftCell;
    public CellStatePayload middleCell;
    public CellStatePayload rightCell;
}