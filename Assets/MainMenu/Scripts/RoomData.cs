using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    private RoomsDataContainer roomData;

    public void SetRoomData(RoomsDataContainer roomData)
    {
        this.roomData = roomData;
    }

    public string GetRoomId()
    {
        return roomData.id;
    }
}
