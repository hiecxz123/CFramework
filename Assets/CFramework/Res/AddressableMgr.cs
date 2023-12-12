using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableMgr : Singleton<AddressableMgr>
{
    //ʹ��AsyncOperationHandle�ĸ�������¼
    public Dictionary<string, IEnumerator> resDic = new Dictionary<string, IEnumerator>();
    //�첽������Դ
    public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callback)
    {
        //���ڴ���ͬ�� ��ͬ������Դ�����ּ��أ������ֺ�������ΪKey
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //������ع���Դ
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<T>)resDic[keyName];
            //�ж��첽�����Ƿ����
            if (handle.IsDone)
            {
                //��������Ҽ��سɹ���δ������ɺ�δ���سɹ�����Դ������뵽resDic��
                callback(handle);

            }
            //��δ������ɣ��󶨼�����ɵ��߼�
            else
            {
                handle.Completed += callback;
            }
            return;
        }
        //���û�м��ع�����Դ

        //ֱ�ӽ����첽���� ���Ҽ�¼
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += (obj) =>
        {
            //�ж��Ƿ���سɹ����ɹ��Ļ�ִ�лص�
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                callback(obj);
            }
            else
            {
                Debug.LogWarning(keyName + "Load Addressable Resources Failed!");
                //���߼�������һִ֡�У��ڴ�֮ǰ��keyName�Ѿ�������resDic������������ڼ�һ���ж�
                if (resDic.ContainsKey(keyName))
                {
                    resDic.Remove(keyName);
                }
            }
        };
        resDic.Add(keyName, handle);
    }

    //�ͷ���Դ
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
        //callbackΪ������T���͵Ķ��󣬶�����AsyncOperationHandle
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
        {
            keyName += key + "_";
        }
        keyName += typeof(T).Name;
        AsyncOperationHandle<IList<T>> handle;
        //�Ƿ��Ѿ�������
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
            if (handle.IsDone)
            {
                //����Ѿ������ˣ���Result����callback��Ϊ����
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
    /// ��ʹ��Action<T>�ص���ʹ��complate�¼���ִ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mode"></param>
    /// <param name="callback"></param>
    /// <param name="keys"></param>
    public void LoadAssetsAsync<T>(Addressables.MergeMode mode, Action<AsyncOperationHandle<IList<T>>> callback, params string[] keys)
    {
        //callbackΪ������T���͵Ķ��󣬶�����AsyncOperationHandle
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
        {
            keyName += key + "_";
        }
        keyName += typeof(T).Name;
        AsyncOperationHandle<IList<T>> handle;
        //�Ƿ��Ѿ�������
        if (resDic.ContainsKey(keyName))
        {
            handle = (AsyncOperationHandle<IList<T>>)resDic[keyName];
            if (handle.IsDone)
            {
                //����Ѿ������ˣ���Result����handle��Ϊ����
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
