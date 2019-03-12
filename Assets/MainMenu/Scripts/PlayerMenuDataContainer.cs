using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerMenuDataContainer
{
    public PlayerMenuGamesDataContainer[] games;
    public string[] rooms;

    public bool Equals(PlayerMenuDataContainer other)
    {
        bool isRoomsEqual = rooms.SequenceEqual(other.rooms);
        bool isGamesEqual = true;
        
        Debug.Log("Games count " + games.Count());
        Debug.Log("Other games count " + other.games.Count());
        if (games.Count() != other.games.Count())
        {
            isGamesEqual = false;
            Debug.Log(string.Format("Rooms equal: {0} Games equal: {1}", isRoomsEqual, isGamesEqual));
            return (isRoomsEqual && isGamesEqual);
        }
        else
        {
            //Debug.Log(JsonUtility.ToJson(other));
            // They don't arrive in the same order
            string otherGamesSerialized = JsonUtility.ToJson(other);
            Debug.Log(otherGamesSerialized);
            foreach(PlayerMenuGamesDataContainer game in games)
            {
                string gameSerialized = JsonUtility.ToJson(game);
                Debug.Log(gameSerialized);
                isGamesEqual = otherGamesSerialized.Contains(gameSerialized);
                if (!isGamesEqual)
                {
                    Debug.Log(string.Format("Rooms equal: {0} Games equal: {1}", isRoomsEqual, isGamesEqual));
                    return (isRoomsEqual && isGamesEqual);
                }
            }
            /* for(int i=0; i<games.Count(); i+=1)
            {
                isGamesEqual = games[i].Equals(other.games[i]);
                if (!isGamesEqual)
                {
                    Debug.Log(string.Format("Rooms equal: {0} Games equal: {1}", isRoomsEqual, isGamesEqual));
                    return (isRoomsEqual && isGamesEqual);
                }
            } */
        }

        Debug.Log(string.Format("Rooms equal: {0} Games equal: {1}", isRoomsEqual, isGamesEqual));
        return (isRoomsEqual && isGamesEqual);
    }
}

[Serializable]
public class PlayerMenuGamesDataContainer
{
    public string gameid;
    public string turn;

    public bool Equals(PlayerMenuGamesDataContainer other)
    {
        bool isGameIdEqual = (gameid == other.gameid);
        bool isTurnEqual = (turn == other.turn);

        return (isGameIdEqual && isTurnEqual);
    }
}