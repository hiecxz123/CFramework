using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonUtilityManager
{
    public string ObjectToJson<T>(T obj)
    {
        string json = JsonUtility.ToJson(obj);
        return json;
    }
}
