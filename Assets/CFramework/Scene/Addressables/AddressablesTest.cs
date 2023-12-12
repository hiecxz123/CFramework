using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressablesTest : MonoBehaviour
{
    [AssetReferenceUILabelRestriction("Player")]
    public AssetReference assetReference;
    public AssetReferenceAtlasedSprite assetReferenceAtlasedSprite;
    public AssetReferenceGameObject assetReferenceGameObject;
    public AssetReferenceSprite assetReferenceSprite;
    public AssetReferenceTexture assetReferenceTexture;
    public AssetReferenceT<AudioClip> assetReferenceAudioClip;
    public AssetReferenceT<TextAsset> assetReferenceTextAsset;
    public AssetReferenceT<Material> assetReferenceMaterial;

    public AssetReference sceneReference;

    public List<GameObject> objLists = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //ͬ�����ص�LoadAsset<T>�Ѿ�����
        //��ȡ�첽������ɵ��¼���Ҫ
        //AsyncOperationHandle<GameObject> handle= assetReference.LoadAssetAsync<GameObject>();
        //handle.Completed += HandleCompleted;
        //handle.Destroyed += HandleDestoryed;

        //��������
        //sceneReference.LoadSceneAsync().Completed += (handle) => {
        //};

        //assetReference.LoadAssetAsync<GameObject>().Completed += HandleCompleted;
        //������ɺ�ֱ��ʵ����
        //assetReference.InstantiateAsync();

        //��̬������Դ
        //AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Capsule");
        //handle.Completed += (handle) => {
        //    if(handle.Status==AsyncOperationStatus.Succeeded)
        //    {
        //        Instantiate(handle.Result);
        //    }
        //};
        //Addressables.LoadAssetAsync<GameObject>("Capsule").Completed += (handle) => {
        //    if (handle.Status == AsyncOperationStatus.Succeeded)
        //    {
        //        Instantiate(handle.Result);
        //    }
        //    Addressables.Release(handle);
        //};

        //1����������2�����ط�ʽ��single(����)additive(����)��3���Ƿ񼤻4���������ȼ�
        //Addressables.LoadSceneAsync("TestScene", LoadSceneMode.Single, true, 100);
        //�ֶ�����
        //Addressables.LoadSceneAsync("TestScene", LoadSceneMode.Single, false, 100).Completed +=
        //    (handle) => {
        //        //�첽����
        //        handle.Result.ActivateAsync();
        //    };
        //AddressableMgr.GetInstance().LoadAssetAsync<GameObject>("Capsule", (obj) =>
        //{
        //    GameObject o = Instantiate(obj.Result);
        //    o.transform.position = Vector3.up;
        //    Debug.Log("1");
        //});
        //AddressableMgr.GetInstance().LoadAssetAsync<GameObject>("Capsule", (obj) =>
        //{
        //    GameObject o = Instantiate(obj.Result);
        //    o.transform.position = Vector3.down;
        //    Debug.Log("2");
        //});
        //AddressableMgr.GetInstance().LoadAssetAsync<GameObject>("Capsule", (obj) =>
        //{
        //    GameObject o = Instantiate(obj.Result);
        //    o.transform.position = Vector3.left;
        //    Debug.Log("3");
        //});
        //AddressableMgr.GetInstance().LoadAssetAsync<GameObject>("Capsule", (obj) =>
        //{
        //    GameObject o = Instantiate(obj.Result);
        //    o.transform.position = Vector3.right;
        //    Debug.Log("4");
        //    AddressableMgr.GetInstance().Release<GameObject>("Capsule");
        //});

        //��̬���ض����Դ
        //1����Դ�����ǩ����
        //2�����ؽ�����Ļص���
        //3��true��ʾ����ʧ�ܻὫ�Ѿ����ص���Դ���������ͷŵ�;false��ʾ�ֶ������ͷ�
        //Addressables.LoadAssetsAsync<GameObject>("Cube", (obj) =>
        //{
        //    Debug.Log(obj.name);
        //},true);
        //AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>("Cube", (obj) =>
        // {
        //     Debug.Log(obj.name);
        // }, true);
        //handle.Completed += (obj) =>
        //{
        //    foreach (var item in obj.Result)
        //    {
        //        Debug.Log(item.name);
        //    }
        //    Addressables.Release(obj);
        //};

        //��������̬����
        //1�������б�
        //2���ص�
        //3������ģʽ
        //4������ʧ��ʱ�Ƿ��Զ��ͷ���Դ
        //List<string> strs = new List<string>() {"Cube","Item" ,"default"};
        //Addressables.LoadAssetsAsync<GameObject>(
        //    strs,
        //    (obj) => {
        //        Debug.Log(obj.name);
        //    },
        //    //Intersection:�����ཻ���������Ʊ�ǩΪstrs�е������ֶε���Դ�ᱻ����
        //    //Union:�����ϲ���ֻҪ��Դ�����ƻ��ǩ��strs�е��ֶΣ��ͻᱻ����
        //    //None��UseFirst:ʹ��strs�еĵ�һ����������ɸѡ
        //    Addressables.MergeMode.None,true);

        //AddressableMgr
        AddressableMgr.GetInstance().LoadAssetsAsync<UnityEngine.Object>(
            Addressables.MergeMode.Union,
            (obj) =>
            {
                Debug.Log("1:" + obj.name);
            },
            "Cube", "Item", "default");
        AddressableMgr.GetInstance().LoadAssetsAsync<UnityEngine.Object>(
            Addressables.MergeMode.Union,
            (obj) =>
            {
                Debug.Log("2:" + obj.name);
            },
            "Cube", "Item", "default");
    }

    private void ComplateLoad(AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log("Load");
    }

    private void HandleCompleted(AsyncOperationHandle<GameObject> obj)
    {
        //�ж��Ƿ���سɹ�
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            //�ɹ���ʵ��������
            GameObject o = Instantiate(obj.Result);
            //assetReference.ReleaseAsset();
            assetReferenceMaterial.LoadAssetAsync().Completed += (handle) =>
            {
                o.GetComponent<Renderer>().material = handle.Result;
                assetReferenceMaterial.ReleaseAsset();
                print(assetReferenceMaterial);
                print(handle.Result);
            };

        }

    }
    private void HandleDestoryed(AsyncOperationHandle obj)
    {

    }


    // Update is called once per frame
    void Update()
    {

    }
}
