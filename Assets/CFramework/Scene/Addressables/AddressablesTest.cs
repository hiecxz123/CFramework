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
        //同步加载的LoadAsset<T>已经弃用
        //获取异步加载完成的事件需要
        //AsyncOperationHandle<GameObject> handle= assetReference.LoadAssetAsync<GameObject>();
        //handle.Completed += HandleCompleted;
        //handle.Destroyed += HandleDestoryed;

        //场景加载
        //sceneReference.LoadSceneAsync().Completed += (handle) => {
        //};

        //assetReference.LoadAssetAsync<GameObject>().Completed += HandleCompleted;
        //加载完成后直接实例化
        //assetReference.InstantiateAsync();

        //动态加载资源
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

        //1、场景名；2、加载方式：single(单个)additive(叠加)；3、是否激活；4、加载优先级
        //Addressables.LoadSceneAsync("TestScene", LoadSceneMode.Single, true, 100);
        //手动激活
        //Addressables.LoadSceneAsync("TestScene", LoadSceneMode.Single, false, 100).Completed +=
        //    (handle) => {
        //        //异步激活
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

        //动态加载多个资源
        //1、资源名或标签名；
        //2、加载结束后的回调；
        //3、true表示加载失败会将已经加载的资源和依赖都释放掉;false表示手动管理释放
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

        //多条件动态加载
        //1、条件列表
        //2、回调
        //3、条件模式
        //4、加载失败时是否自动释放资源
        //List<string> strs = new List<string>() {"Cube","Item" ,"default"};
        //Addressables.LoadAssetsAsync<GameObject>(
        //    strs,
        //    (obj) => {
        //        Debug.Log(obj.name);
        //    },
        //    //Intersection:条件相交，满足名称标签为strs中的所有字段的资源会被加载
        //    //Union:条件合并，只要资源的名称或标签有strs中的字段，就会被加载
        //    //None和UseFirst:使用strs中的第一个条件进行筛选
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
        //判断是否加载成功
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            //成功后实例化对象
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
