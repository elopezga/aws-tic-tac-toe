using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
public static class JsonHelper
{
    public static T[] Deserialize<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static T[] DeserializeFromServer<T>(string json)
    {
        string wrappedJson = "{\"Items\":" + json + "}";
        return Deserialize<T>(wrappedJson);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}