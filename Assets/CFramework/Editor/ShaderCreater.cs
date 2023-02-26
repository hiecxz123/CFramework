using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using UnityEditor.ProjectWindowCallback;

public class ShaderCreater : Editor
{
    public static string shaderTemplatePath = "Assets/CFramework/Editor/Templates/ShaderTemplates/URPTemplate.txt";


    [MenuItem("Assets/Create/Shader/URPTemplate", priority =0)]
    public static void CreateShader()
    {
        //获取选择路径
        string locationPath = GetSelectedPathOrFallBack();
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            CreateInstance<CreateScriptAssetAction>(),
            locationPath+"/New URPShader.shader",
            null,
            shaderTemplatePath);
    }
    private static string GetSelectedPathOrFallBack()
    {
        string path = "Assets";
        //这个的作用就是获取你鼠标选中的文件夹对象
        // Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }

        return path;
    }
    class CreateScriptAssetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //创建资源
            UnityEngine.Object obj = CreateAssetFromTemplate(pathName, resourceFile);
            //高亮显示该资源
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
        internal static UnityEngine.Object CreateAssetFromTemplate(string pathName, string resourceFile)
        {
            //获取要创建的资源的绝对路径
            string fullName = Path.GetFullPath(pathName);
            //读取本地模板文件
            StreamReader reader = new StreamReader(resourceFile);
            string content = reader.ReadToEnd();
            reader.Close();

            //获取资源的文件名
            // string fileName = Path.GetFileNameWithoutExtension(pahtName);
            //替换默认的文件名
            content = content.Replace("#TIME", System.DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss dddd"));
            string[] tempStrs = pathName.Split('/');
            string fileName = tempStrs[tempStrs.Length - 1].Split('.')[0];
            content = content.Replace("#URPBaseShader", fileName);
            //写入新文件
            StreamWriter writer = new StreamWriter(fullName, false, System.Text.Encoding.UTF8);
            writer.Write(content);
            writer.Close();

            //刷新本地资源
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();

            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }
    }
}


