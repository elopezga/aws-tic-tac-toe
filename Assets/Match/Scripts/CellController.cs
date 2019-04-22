using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{

    public Action OnPiecePlaced;

    [SerializeField]
    private Text PieceText;

    [SerializeField]
    private string DefaultDisplayText = "";

    private CellState state;

    private Button button;

    private bool placePieceDisabled = true;

    void Awake()
    {
        state = new CellState(){
            PlayerPiece = DefaultDisplayText,
            DisplayPiece = DefaultDisplayText
        };

        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        PieceText.text = state.DisplayPiece;
    }

    public void AttemptToPlacePiece()
    {
        if (placePieceDisabled)
        {
            Debug.Log("Placing piece is disabled");
        }
        else
        {
            if (OpenCell())
            {
                Debug.Log("Place the piece " + state.CurrentTurnPiece);
                PlacePiece();
            }
            else
            {
                Debug.Log("Piece already taken either by you or the opponent");
            }
        }
    }

    public void DisablePlacingPiece()
    {
        placePieceDisabled = true;
    }

    public void EnablePlacingPiece()
    {
        placePieceDisabled = false;
    }

    public void SetState(CellState state)
    {
        this.state = state;
    }

    private void PlacePiece()
    {
        state.DisplayPiece = state.CurrentTurnPiece;
        state.PlayerPiece = state.CurrentTurnPiece;
        state.OwnerId = state.CurrentTurnId;
        OnPiecePlaced.Invoke();
    }

    private bool AlreadyOwnedByYou()
    {
        return state.OwnerId == state.CurrentTurnId;
    }

    private bool OpenCell()
    {
        return string.IsNullOrEmpty(state.OwnerId);
    }
}

public class CellState
{
    public string PlayerPiece;
    public string DisplayPiece;
    public string OwnerId;
    public string CurrentTurnId;
    public string CurrentTurnPiece;
}