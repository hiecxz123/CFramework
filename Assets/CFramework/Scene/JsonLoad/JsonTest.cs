using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TestJsonData
{
    public int id;
    public string name;
    public string des;
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public float moveSpeed;
    public float atkSpeed;
}

public class JsonTest : MonoBehaviour
{
    private List<TestJsonData> lists = new List<TestJsonData>();

    // Start is called before the first frame update
    private void Start()
    {
        double startTime = DateTime.Now.TimeOfDay.TotalMilliseconds;
        for (int i = 0; i < 1000000; i++)
        {
            TestJsonData testJson = new TestJsonData();
            testJson.id = i;
            testJson.name = "name_" + i;
            testJson.des = "des_" + i;
            testJson.hp = i * 10;
            testJson.mp = testJson.hp / 2;
            testJson.atk = 100;
            testJson.def = 50;
            testJson.moveSpeed = 1;
            testJson.atkSpeed = 1;

            lists.Add(testJson);
        }

        Debug.Log(DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);
        startTime = DateTime.Now.Millisecond;
        string json =
            LitJsonManager.ObjectToJson(lists);
        File.WriteAllText(
            Path.Combine(Application.streamingAssetsPath, "TestData.json"),
            json);
        Debug.Log(DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);
        
        lists.Clear();
        string jsonStr =
            File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "TestData.json"));
        
        startTime = DateTime.Now.Millisecond;
        
        
        lists = LitJsonManager.JsonToObject<List<TestJsonData>>(jsonStr);
        Debug.Log(DateTime.Now.TimeOfDay.TotalMilliseconds - startTime);
    }

    IEnumerator Create()
    {
        for (int i = 0; i < 100000; i++)
        {
        }

        yield return new WaitForFixedUpdate();
    }
}