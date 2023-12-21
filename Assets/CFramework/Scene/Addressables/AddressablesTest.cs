using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
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

        #region 动态加载资源
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
        #endregion

        #region 场景加载
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
        #endregion

        #region 动态加载多个资源
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
        #endregion

        #region 多条件加载
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
        #endregion

        #region AddressableMgr加载
        //////AddressableMgr
        //AddressableMgr.GetInstance().LoadAssetsAsync<UnityEngine.Object>(
        //    Addressables.MergeMode.Union,
        //    (obj) =>
        //    {
        //        Debug.Log("1:" + obj.name);
        //        Instantiate(obj, Vector3.up * 2f, Quaternion.identity);

        //    },
        //    "Cube", "Item", "default");
        //AddressableMgr.GetInstance().LoadAssetsAsync<UnityEngine.Object>(
        //    Addressables.MergeMode.Union,
        //    (obj) =>
        //    {
        //        Debug.Log("2:" + obj.name);
        //        Instantiate(obj);
        //    },
        //    "Cube", "Item", "default");
        #endregion

        #region 远端加载
        //远端加载
        //AddressableMgr.GetInstance().LoadAssetAsync<GameObject>("Player1", (obj) =>
        //{
        //    GameObject o = Instantiate(obj.Result);
        //    o.transform.position = Vector3.right;
        //    Debug.Log("4");
        //});
        //AddressableMgr.GetInstance().LoadAssetAsync<GameObject>("Cube", (obj) =>
        //{
        //    GameObject o = Instantiate(obj.Result);
        //    o.transform.position = Vector3.left;
        //    Debug.Log("4");
        //});
        #endregion

        #region IResourceLocation加载
        //通过IResourceLocation加载资源
        //AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync("Cube", typeof(GameObject));
        //handle.Completed += (obj) =>
        //{
        //    if(obj.Status==AsyncOperationStatus.Succeeded)
        //    {
        //        foreach (var item in obj.Result)
        //        {
        //            //利用定位信息加载资源，再去加载资源
        //            Debug.Log(item.PrimaryKey);
        //            Addressables.LoadAssetAsync<GameObject>(item).Completed += (obj) =>
        //            {
        //                Instantiate(obj.Result);
        //            };
        //        }
        //    }
        //    else
        //    {
        //        Addressables.Release(handle);
        //    }
        //};
        //AsyncOperationHandle<IList<IResourceLocation>> handle = 
        //    Addressables.LoadResourceLocationsAsync(new List<string> {"Player"},Addressables.MergeMode.Union, typeof(GameObject));
        //handle.Completed += (obj) =>
        //{
        //    if (obj.Status == AsyncOperationStatus.Succeeded)
        //    {
        //        foreach (var item in obj.Result)
        //        {
        //            Debug.Log(item.PrimaryKey);
        //            Debug.Log(item.InternalId);
        //            Debug.Log(item.ResourceType.Name);
        //            Addressables.LoadAssetAsync<UnityEngine.Object>(item).Completed += (obj) =>
        //            {

        //            };
        //        }
        //    }
        //    else
        //    {
        //        Addressables.Release(handle);
        //    }
        //};
        #endregion

        #region 协程加载
        //StartCoroutine(LoadAsset());
        #endregion

        #region async和await加载
        //WebGL不支持此种方式
        //Load();
        //Loads();
        #endregion

        #region 手动更新目录
        //需要关闭自动更新
        //自动检测所有目录是否有更新，并更新目录API
        //Addressables.UpdateCatalogs().Completed += (obj) =>
        //{
        //    Addressables.Release(obj);
        //};
        ////3.获取目录列表，在更新目录(true为自动释放资源，默认为true，这里自动释放obj)
        //Addressables.CheckForCatalogUpdates(true).Completed += (obj) =>
        //{
        //    if(obj.Result.Count>0)
        //    {
        //        //根据目录列表更新目录(true为自动释放资源，默认为true，这里自动释放handle)
        //        Addressables.UpdateCatalogs(obj.Result,true).Completed += (handle) =>
        //        {
        //            //如果更新完毕，记得释放资源
        //            //Addressables.Release(handle);
        //            //Addressables.Release(obj);
        //        };
        //    }
        //};



        #endregion

        #region 预加载

        #endregion

    }
    private List<AsyncOperationHandle<GameObject>> list = new List<AsyncOperationHandle<GameObject>>();
    // Update is called once per frame
    void Update()   
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Cube");
            handle.Completed += (obj) =>
            {
                Instantiate(obj.Result);
            };
            list.Add(handle);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (list.Count > 0)
            {
                Addressables.Release(list[0]);
                list.RemoveAt(0);
            }
        }

        #region AddressablesMgr引用计数测试
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    AddressableMgr.GetInstance().LoadAssetAsync<GameObject>("Cube", (obj) =>
        //    {
        //        Instantiate(obj.Result);
        //    });

        //}

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    AddressableMgr.GetInstance().Release<GameObject>("Cube");
        //}
        #endregion
    }
    IEnumerator PreLoadAssets()
    {
        //1.首先获取下载包的大小
        //可以传资源名、标签名、或者两者的组合
        AsyncOperationHandle<long> handleSize = Addressables.GetDownloadSizeAsync(new List<string> { "Cube", "Sphere", "SD" });
        yield return handleSize;
        //2.预加载
        if (handleSize.Result > 0)
        {
            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(
                new List<string> { "Cube", "Sphere", "SD" },
                Addressables.MergeMode.Union
                );
            while(!handle.IsDone)
            {
                //3.加载进度
                DownloadStatus info = handle.GetDownloadStatus();
                Debug.Log(info.Percent);
                Debug.Log(info.DownloadedBytes + "/" + info.TotalBytes);
                yield return 0;
            }
            Addressables.Release(handle);
        }
    }
    async void Load()
    {
        AsyncOperationHandle<GameObject> handle;
        Debug.Log("异步函数");
        handle = Addressables.LoadAssetAsync<GameObject>("Cube");
        //等待任务完成
        await handle.Task;

        Instantiate(handle.Result);
    }

    async void Loads()
    {
        AsyncOperationHandle<GameObject> handle1;
        AsyncOperationHandle<GameObject> handle2;
        Debug.Log("异步函数多任务");
        handle1 = Addressables.LoadAssetAsync<GameObject>("Cube");
        handle2 = Addressables.LoadAssetAsync<GameObject>("Player");
        //等待所有任务完成
        await Task.WhenAll(handle1.Task, handle2.Task);

        Instantiate(handle1.Result);
        Instantiate(handle2.Result);
    }

    AsyncOperationHandle<GameObject> handle;
    IEnumerator LoadAsset()
    {
        Debug.Log("协程开启");
        handle = Addressables.LoadAssetAsync<GameObject>("Cube");
        //一定是没有加载成功再yield return;yield return handle意思为等待handle完成
        //if (!handle.IsDone)
        //    yield return handle;

        //卡住主线程强制等待
        handle.WaitForCompletion();

        while (!handle.IsDone)
        {
            DownloadStatus info = handle.GetDownloadStatus();
            //获取加载进度
            Debug.Log(info.Percent);
            //获取包总字节数
            Debug.Log(info.DownloadedBytes + "/" + info.TotalBytes);
            yield return 0;
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(handle.Result);
        }
        else
        {
            Addressables.Release(handle);
        }
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


    
}
