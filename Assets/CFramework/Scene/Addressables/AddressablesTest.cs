using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesTest : MonoBehaviour
{
    public AssetReference assetReference;
    public AssetReferenceAtlasedSprite assetReferenceAtlasedSprite;
    public AssetReferenceGameObject assetReferenceGameObject;
    public AssetReferenceSprite assetReferenceSprite;
    public AssetReferenceTexture assetReferenceTexture;
    public AssetReferenceT<AudioClip> assetReferenceAudioClip;
    public AssetReferenceT<TextAsset> assetReferenceTextAsset;
    // Start is called before the first frame update
    void Start()
    {
        //同步加载的LoadAsset<T>已经弃用
        //获取异步加载完成的事件需要
        AsyncOperationHandle<GameObject> handle= assetReference.LoadAssetAsync<GameObject>();
        handle.Completed += HandleCompleted;
        handle.Destroyed += HandleDestoryed;
    }

    private void HandleDestoryed(AsyncOperationHandle obj)
    {
        
    }

    private void HandleCompleted(AsyncOperationHandle<GameObject> obj)
    {
        //判断是否加载成功
        if(obj.Status==AsyncOperationStatus.Succeeded)
        {
            //成功后实例化对象
            Instantiate(obj.Result);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
