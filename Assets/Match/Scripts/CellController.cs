using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{

    public Action<string> OnCellClicked;

    [SerializeField]
    private Text PieceText;

    [SerializeField]
    private string DefaultDisplayText = "";

    private CellState state;

    void Awake()
    {
        state = new CellState(){
            PlayerPiece = DefaultDisplayText,
            DisplayPiece = DefaultDisplayText
        };
    }

    // Update is called once per frame
    void Update()
    {
        PieceText.text = state.DisplayPiece;
    }

    void OnDestroy()
    {

    }

    public void SetState(CellState state)
    {
        this.state = state;
    }
}

public class CellState
{
    public string PlayerPiece;
    public string DisplayPiece;
}