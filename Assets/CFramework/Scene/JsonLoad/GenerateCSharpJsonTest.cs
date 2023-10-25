using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestData
{
    public int id;
    public string name;
    public DateTime dateTime;

}

public class GenerateCSharpJsonTest : MonoBehaviour
{
    [SerializeField]
    //public List<Sheet1> lists = new List<Sheet1>();
    // Start is called before the first frame update
    void Start()
    {
        //string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "TestCharacter.json"));
        //Debug.Log(json);
        //lists= LitJsonManager.JsonToObject<List<Sheet1>>(json);
        //Debug.Log(lists[2].Name);
        //Debug.Log(lists[1].Name);
        //Debug.Log(lists[0].Name);
        //Debug.Log(lists[2].HP);
        //TestData testData = new TestData();
        //testData.id = 1;
        //testData.name = "skdljf";
        //testData.dateTime = DateTime.Now;
        //string json= LitJsonManager.ObjectToJson<TestData>(testData);
        //File.WriteAllText(
        //    Path.Combine(Application.streamingAssetsPath,
        //    "TestData.json"),json
        //    );

        TestData testData1 = new TestData();

        string jsonStr = File.ReadAllText(Path.Combine(Application.streamingAssetsPath,
            "TestData.json"));

        testData1 = LitJsonManager.JsonToObject<TestData>(jsonStr);

        Debug.Log(testData1.dateTime);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
