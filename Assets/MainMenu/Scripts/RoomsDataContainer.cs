using System;


[Serializable]
public class RoomsDataContainer
{
    public string id;
    public string[] players;
    public bool isFull;
    public RoomCreatedDataContainer created;
}

[Serializable]
public class RoomCreatedDataContainer
{
    public ulong _seconds;
    public ulong _nanseconds;
}