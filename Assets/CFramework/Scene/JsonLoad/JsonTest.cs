using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

public class TestJsonDatas
{
    public List<TestJsonData> lists = new List<TestJsonData>();
}

public class JsonTest : MonoBehaviour
{
    
       
    // Start is called before the first frame update
    private void Start()
    {
        Stopwatch sw = new Stopwatch();
        TestJsonDatas testJsonDatas = new TestJsonDatas();
        #region 创建100万json数据
        //sw.Start();
        //for (int i = 0; i < 1000000; i++)
        //{
        //    TestJsonData testJson = new TestJsonData();
        //    testJson.id = i;
        //    testJson.name = "name_" + i;
        //    testJson.des = "des_" + i;
        //    testJson.hp = i * 10;
        //    testJson.mp = testJson.hp / 2;
        //    testJson.atk = 100;
        //    testJson.def = 50;
        //    testJson.moveSpeed = 1;
        //    testJson.atkSpeed = 1;

        //    testJsonDatas.lists.Add(testJson);
        //}
        //sw.Stop();
        //UnityEngine.Debug.Log(string.Format("create data total:{0} ms", sw.ElapsedMilliseconds));
        #endregion

        #region 6秒 写入文本
        //sw.Reset();
        //sw.Start();
        //string json =
        //    LitJsonManager.ObjectToJson(testJsonDatas);
        //File.WriteAllText(
        //    Path.Combine(Application.streamingAssetsPath, "TestData.json"),
        //    json);
        //sw.Stop();
        //UnityEngine.Debug.Log(string.Format("write text total:{0} ms", sw.ElapsedMilliseconds));
        //testJsonDatas.lists.Clear();
        #endregion

        #region 0.3秒 读取100万json数据字符
        sw.Reset();
        sw.Start();
        string jsonStr =
            File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "TestData.json"));
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("read text total:{0} ms", sw.ElapsedMilliseconds));
        #endregion

        #region 16秒 100万条json 直接序列化
        //sw.Reset();
        //sw.Start();
        //testJsonDatas = LitJsonManager.JsonToObject<TestJsonDatas>(jsonStr);
        //sw.Stop();
        //UnityEngine.Debug.Log(string.Format("total:{0} ms", sw.ElapsedMilliseconds));
        #endregion

        #region 35秒 100万条json JsonReader获取到指定字段
        sw.Reset();
        testJsonDatas.lists.Clear();
        sw.Start();
        JsonReader reader = new JsonReader(jsonStr);
        while(reader.Read())
        {
            if (reader.Value == null)
                continue;
            if(reader.Value.ToString()=="name_900000")
            {
                sw.Stop();
                UnityEngine.Debug.Log(string.Format("find name total:{0} ms", sw.ElapsedMilliseconds));
                break;
            }
        }
        //testJsonDatas.lists = JsonMapper.ToObject<List<TestJsonData>>(reader);
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("not find total:{0} ms", sw.ElapsedMilliseconds));
        #endregion

        #region 16秒 100万条json JsonData序列化
        //sw.Reset();
        //sw.Start();
        //JsonData jsonData = JsonMapper.ToObject(jsonStr);
        //sw.Stop();
        //UnityEngine.Debug.Log(string.Format("total:{0} ms", sw.ElapsedMilliseconds));
        #endregion



    }

}