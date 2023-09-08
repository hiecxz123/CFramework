using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class GenerateCSharpJsonTest : MonoBehaviour
{
    [SerializeField]
    public List<Sheet1> lists = new List<Sheet1>();
    // Start is called before the first frame update
    void Start()
    {
        string json = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "TestCharacter.json"));
        Debug.Log(json);
        lists= LitJsonManager.JsonToObject<List<Sheet1>>(json);
        Debug.Log(lists[2].Name);
        Debug.Log(lists[1].Name);
        Debug.Log(lists[0].Name);
        Debug.Log(lists[2].HP);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
