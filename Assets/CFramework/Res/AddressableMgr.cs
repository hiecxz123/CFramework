using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableMgr : Singleton<AddressableMgr>
{
    //使用AsyncOperationHandle的父类来记录
    public Dictionary<string, IEnumerator> resDic = new Dictionary<string, IEnumerator>();
    //异步加载资源
    public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        //由于存在同名 不同类型资源的区分加载，将名字和类型作为Key
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //如果加载过资源
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<T>)resDic[keyName];
            //判断异步加载是否结束
            if (handle.IsDone)
            {
                //加载完成且加载成功，未加载完成和未加载成功的资源不会记入到resDic中
                callback(handle);

            }
            //还未加载完成，绑定加载完成的逻辑
            else
            {
                handle.Completed += callback;
            }
            return;
        }
        //如果没有加载过该资源

        //直接进行异步加载 并且记录
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += (obj) =>
        {
            //判断是否加载成功，成功的话执行回调
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                callback(obj);
            }
            else
            {
                Debug.LogWarning(keyName + "Load Addressable Resources Failed!");
                //该逻辑会在下一帧执行，在此之前，keyName已经加入了resDic，保险起见，在加一层判断
                if (resDic.ContainsKey(keyName))
                {
                    resDic.Remove(keyName);
                }
            }
        };
        resDic.Add(keyName, handle);
    }

    //释放资源
    public void Release<T>(string name)
    {
        string keyName = name + "_" + typeof(T).Name;
        if (resDic.ContainsKey(keyName))
        {
            AsyncOperationHandle<T> handle = (AsyncOperationHandle<T>)resDic[keyName];
            Addressables.Release(handle);
            resDic.Remove(keyName);
        }
    }

    public void LoadAssetsAsync<T>(Addressables.MergeMode mode, Action<T> callback, params string[] keys)
    {
        //callback为参数是T类型的对象，而不是AsyncOperationHandle
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
        {
            keyName += key + "_";
        }
        keyName += typeof(T).Name;
        AsyncOperationHandle<IList<T>> handle;
        //是否已经加载了
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
            if (handle.IsDone)
            {
                //如果已经创建了，则将Result传入callback作为参数
                foreach (T item in handle.Result)
                {
                    callback(item);
                }
            }
            else
            {
                handle.Completed += (obj) =>
                {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (T item in handle.Result)
                        {
                            callback(item);
                        }
                    }
                };
            }
            return;
        }

        handle = Addressables.LoadAssetsAsync<T>(list, callback, mode);
        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Resources Load Failed! " + keyName);
                if (resDic.ContainsKey(keyName))
                {
                    resDic.Remove(keyName);
                }
            }
        };
        resDic.Add(keyName, handle);
    }
    
    /// <summary>
    /// 不使用Action<T>回调，使用complate事件来执行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <param name="callback"></param>
    /// <param name="keys"></param>
    public void LoadAssetsAsync<T>(Addressables.MergeMode mode, Action<AsyncOperationHandle<IList<T>>> callback, params string[] keys)
    {
        //callback为参数是T类型的对象，而不是AsyncOperationHandle
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
        {
            keyName += key + "_";
        }
        keyName += typeof(T).Name;
        AsyncOperationHandle<IList<T>> handle;
        //是否已经加载了
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
            if (handle.IsDone)
            {
                //如果已经创建了，则将Result传入handle作为参数
                callback(handle);
            }
            else
            {
                handle.Completed += (obj) =>
                {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        callback(handle);
                    }
                };
            }
            return;
        }

        handle = Addressables.LoadAssetsAsync<T>(
            list,
            (obj) =>
            {

            },
            mode);
        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                callback(obj);
            }
            else
            {
                Debug.LogError("Resources Load Failed! " + keyName);
                if (resDic.ContainsKey(keyName))
                {
                    resDic.Remove(keyName);
                }
            }
        };
        resDic.Add(keyName, handle);
    }

    public void Release<T>(params string[] keys)
    {
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
        {
            keyName += key + "_";
        }
        keyName += typeof(T).Name;

        AsyncOperationHandle<IList<T>> handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
        Addressables.Release(handle);
        resDic.Remove(keyName);
    }

    public void Clear()
    {
        resDic.Clear();
        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}
