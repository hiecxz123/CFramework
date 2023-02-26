using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//调用GetInstance()会创建一个空物体并挂载相应脚本
//切换场景会被删除
public class SingletonAutoMono<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T instance;
    public static T GetInstance()
    {
        if(instance==null)
        {
            GameObject obj = new GameObject();
            obj.name = typeof(T).ToString();
            //让单例模式对象过场景不移除

            DontDestroyOnLoad(obj);

            instance = obj.AddComponent<T>();
        }
        return instance;
    }
    
}
