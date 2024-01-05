using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//可寻址资源信息
public class AddressablesInfo
{
    //记录 异步操作句柄
    public AsyncOperationHandle handle;
    //记录 引用计数
    public uint count;
    //第一次使用时，引用计数默认+1
    public AddressablesInfo(AsyncOperationHandle handle)
    {
        this.handle = handle;
        count += 1;
    }
}
public class AddressableMgr : Singleton<AddressableMgr>
{
    //使用AsyncOperationHandle的父类来记录
    public Dictionary<string, AddressablesInfo> resDic = new Dictionary<string, AddressablesInfo>();
    //异步加载资源
    public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        //由于存在同名 不同类型资源的区分加载，将名字和类型作为Key
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //如果加载过资源
        if (resDic.ContainsKey(keyName))
        {
            handle = resDic[keyName].handle.Convert<T>();
            //要使用资源了，引用计数+1
            resDic[keyName].count += 1;
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
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(keyName, info);
    }

    //释放资源
    public void Release<T>(string name)
    {
        string keyName = name + "_" + typeof(T).Name;
        if (resDic.ContainsKey(keyName))
        {
            //释放时引用计数-1
            resDic[keyName].count -= 1;
            //如果引用计数为0，才真正的释放
            if (resDic[keyName].count <= 0)
            {
                //取出对象，移除资源，并且从字典里面移除
                AsyncOperationHandle<T> handle = resDic[keyName].handle.Convert<T>();
                Addressables.Release(handle);
                resDic.Remove(keyName);
            }

        }
    }
    public void LoadAssetsAsync<T>(Addressables.MergeMode mode, Action<T> callback, ADDRESSABLES_RESOURCES enumKey)
    {
        string[] keys = enumKey.ToString().Split("_");
        LoadAssetsAsync<T>(mode,callback,keys);
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
            handle = resDic[keyName].handle.Convert<IList<T>>();
            //使用资源，引用计数+1
            resDic[keyName].count += 1;
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
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(keyName, info);
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
            resDic[keyName].count += 1;
            handle = resDic[keyName].handle.Convert<IList<T>>();
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
        AddressablesInfo info = new AddressablesInfo(handle);
        resDic.Add(keyName, info);
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
        if (resDic.ContainsKey(keyName))
        {
            resDic[keyName].count -= 1;
            if (resDic[keyName].count == 0)
            {
                AsyncOperationHandle<IList<T>> handle = resDic[keyName].handle.Convert<IList<T>>();
                Addressables.Release(handle);
                resDic.Remove(keyName);
            }
        }
    }

    public void Clear()
    {
        foreach (var item in resDic.Values)
        {
            Addressables.Release(item.handle);
        }
        resDic.Clear();
        AssetBundle.UnloadAllAssetBundles(true);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}
