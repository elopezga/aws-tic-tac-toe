using System;

[Serializable]
public class GameInfoDataContainer
{
    public string currentTurn;
    public string[] players;
    public GameStateDataContainer gameState;
}

[Serializable]
public class GameStateDataContainer
{
    public GameStateCellDataContainer[] topRow;
    public GameStateCellDataContainer[] middleRow;
    public GameStateCellDataContainer[] bottomRow;
}



[Serializable]
public class GameStateCellDataContainer
{
    public string ownerid;
    public string piece;
}