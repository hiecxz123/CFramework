using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIBaseScriptsCreater : Editor
{
    static string UIBaseTemplatePath = "Assets/CFramework/Editor/Templates/CSharpTemplates/UIBaseTemplate.txt";
    
    public static string TransformFindStr = "#目标成员变量#=transform.Find(\"#路径#\").GetComponent<#组件名称#>();";
    /// <summary>
    /// UI组件查找工具命名标准：_xxx，如：_Img，_Sli，_Tog
    /// </summary>
    [MenuItem("GameObject/CreateUIBaseScript",false,-100)]
    public static void UIComponentFinder()
    {
        GameObject item = Selection.activeGameObject;
        string className = item.name;
        string scriptPath = Application.dataPath + "/AutoUIScripts";

        //创建自动生成的脚本文件
        if (!Directory.Exists(scriptPath))
        {
            Directory.CreateDirectory(scriptPath);
        }
        scriptPath = scriptPath + "/" + className + ".cs";
        //判断文件是否已经存在
        
        string contentStr = "";
        string variableStr = "";
        List<string> variableStrList = new List<string>();
        List<string> variableTypeList = new List<string>();
        List<string> transPathStrList = new List<string>();
        Transform[] trans = item.GetComponentsInChildren<Transform>();
        //TODO: 如何获取单个物体多个组件
        for (int i = 0; i < trans.Length; i++)
        {
            string[] str = trans[i].name.Split('_');
            if (str.Length < 2)
            {
                continue;
            }
            string componentName = "Transform";
            switch(str[1].ToLower())
            {
                case "img":
                    componentName = "Image";
                    break;
                case "raw":
                    componentName = "RawImage";
                    break;
                case "sli":
                    componentName = "Slider";
                    break;
                case "txt":
                    componentName = "Text";
                    break;
                case "btn":
                    componentName = "Button";
                    break;
            }
            //获取变量名
            string variableName = "";
            for(int j=0;j<str.Length-1;j++)
            {
                variableName += str[j];
            }
            variableStrList.Add("m_"+ variableName);
            if(componentName!="")
            {
                //获取变量类型名称
                variableTypeList.Add(componentName);
            }

            //获取Transform路径
            transPathStrList.Add(GetTransformPath(trans[i], item.transform));
        }
        //初始化变量字符串
        for(int i=0;i<variableStrList.Count;i++)
        {
            variableStr += "public "+variableTypeList[i] +" "+ variableStrList[i]+";\n";

        }
        //初始化查找字符串
        for(int i=0;i<transPathStrList.Count;i++)
        {
            string transformFindStr= TransformFindStr.Replace("#目标成员变量#", variableStrList[i]);
            transformFindStr=transformFindStr.Replace("#路径#", transPathStrList[i]);
            transformFindStr = transformFindStr.Replace("#组件名称#", variableTypeList[i]);
            contentStr += transformFindStr+"\n";
        }
        //最终写入到文件中的字符串
        string classInfo = File.ReadAllText(UIBaseTemplatePath, System.Text.Encoding.UTF8);
        classInfo = classInfo.Replace("#时间#", System.DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss dddd"));
        classInfo = classInfo.Replace("#类名#", className);
        classInfo = classInfo.Replace("#成员变量#", variableStr);
        classInfo = classInfo.Replace("#查找方法#", contentStr);
        //如果脚本存在，将相应自动生成的代码替换掉
        if (File.Exists(scriptPath))
        {
            string originalCode= File.ReadAllText(scriptPath);
            
            Debug.Log(originalCode);
            classInfo= SubUIBaseScriptsExist(classInfo, originalCode);
            Debug.Log(classInfo);
            Debug.Log("存在相同名称的类:" + scriptPath + ",已覆盖");
            
        }
        //写入文件
        FileStream file = new FileStream(scriptPath, FileMode.Create);
        StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
        fileW.Write(classInfo);
        fileW.Flush();
        fileW.Close();
        file.Close();
        //刷新资源列表
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 如果UIBase子类已经存在，只修改自动生成的代码，其余不变
    /// </summary>
    static string SubUIBaseScriptsExist(string newCode,string originalCode)
    {
        string originalCodeTemp= StringHelper.MidStrEx(originalCode, "#region AutoUIBase", "#endregion");
        string newCodeTemp= StringHelper.MidStrEx(newCode, "#region AutoUIBase", "#endregion");
        return originalCode.Replace(originalCodeTemp, newCodeTemp);
        
    }

    /// <summary>
    /// 获取Transform路径
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <param name="root">根物体</param>
    /// <returns></returns>
    static string GetTransformPath(Transform target,Transform root)
    {
        string pathStr = "";
        if(target.parent==root)
        {
            pathStr = target.name+"/";
        }
        else
        {
            while (target!= root)
            {
                pathStr = target.name + "/" + pathStr;
                target = target.parent;

            }
        }
        //最后会多一个“/”将它去掉
        pathStr= pathStr.Substring(0,pathStr.Length-1);
        return pathStr;
    }
}
