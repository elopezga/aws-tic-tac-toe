using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnsData : MonoBehaviour
{
    private GameTurnsDataContainer gameTurnData;

    public void SetGameTurnData(GameTurnsDataContainer gameTurnData)
    {
        this.gameTurnData = gameTurnData;
    }

    public string GetGameId()
    {
        return gameTurnData.gameid;
    }
}
