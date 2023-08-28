
using LitJson;
using UnityEngine;

public class LitJsonManager
{
    public static string ObjectToJson<T>(T obj)
    {
        return JsonMapper.ToJson(obj);
    }

    public static T JsonToObject<T>(string json)
    {
        return JsonMapper.ToObject<T>(json);
    }
}
