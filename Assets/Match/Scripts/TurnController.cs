using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    [SerializeField]
    private Text Turn;
    
    [SerializeField]
    private Text Piece;

    private TurnState state;

    // Start is called before the first frame update
    void Start()
    {
        state = new TurnState()
        {
            CurrentTurn = "",
            CurrentTurnPiece = ""
        };
    }

    // Update is called once per frame
    void Update()
    {
        Turn.text = state.CurrentTurn;
        Piece.text = state.CurrentTurnPiece;
    }

    public void SetState(TurnState state)
    {
        this.state = state;
    }

    public void SetOpponentTurn()
    {
        this.state = new TurnState()
        {
            CurrentTurn = "Opponent Turn",
            CurrentTurnPiece = (this.state.CurrentTurnPiece == "X") ? "O" : "X"
        };
    }
}

public class TurnState
{
    public string CurrentTurn;
    public string CurrentTurnPiece;
}