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
        //ͬ�����ص�LoadAsset<T>�Ѿ�����
        //��ȡ�첽������ɵ��¼���Ҫ
        AsyncOperationHandle<GameObject> handle= assetReference.LoadAssetAsync<GameObject>();
        handle.Completed += HandleCompleted;
        handle.Destroyed += HandleDestoryed;
    }

    private void HandleDestoryed(AsyncOperationHandle obj)
    {
        
    }

    private void HandleCompleted(AsyncOperationHandle<GameObject> obj)
    {
        //�ж��Ƿ���سɹ�
        if(obj.Status==AsyncOperationStatus.Succeeded)
        {
            //�ɹ���ʵ��������
            Instantiate(obj.Result);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
