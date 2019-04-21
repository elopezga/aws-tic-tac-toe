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
}


public class RowState
{
    
}