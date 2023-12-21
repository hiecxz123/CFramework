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

        #region ��̬������Դ
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
        #endregion

        #region ��������
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
        #endregion

        #region ��̬���ض����Դ
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
        #endregion

        #region ����������
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
        #endregion

        #region AddressableMgr����
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

        #region Զ�˼���
        //Զ�˼���
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

        #region IResourceLocation����
        //ͨ��IResourceLocation������Դ
        //AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync("Cube", typeof(GameObject));
        //handle.Completed += (obj) =>
        //{
        //    if(obj.Status==AsyncOperationStatus.Succeeded)
        //    {
        //        foreach (var item in obj.Result)
        //        {
        //            //���ö�λ��Ϣ������Դ����ȥ������Դ
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

        #region Э�̼���
        //StartCoroutine(LoadAsset());
        #endregion

        #region async��await����
        //WebGL��֧�ִ��ַ�ʽ
        //Load();
        //Loads();
        #endregion

        #region �ֶ�����Ŀ¼
        //��Ҫ�ر��Զ�����
        //�Զ��������Ŀ¼�Ƿ��и��£�������Ŀ¼API
        //Addressables.UpdateCatalogs().Completed += (obj) =>
        //{
        //    Addressables.Release(obj);
        //};
        ////3.��ȡĿ¼�б��ڸ���Ŀ¼(trueΪ�Զ��ͷ���Դ��Ĭ��Ϊtrue�������Զ��ͷ�obj)
        //Addressables.CheckForCatalogUpdates(true).Completed += (obj) =>
        //{
        //    if(obj.Result.Count>0)
        //    {
        //        //����Ŀ¼�б����Ŀ¼(trueΪ�Զ��ͷ���Դ��Ĭ��Ϊtrue�������Զ��ͷ�handle)
        //        Addressables.UpdateCatalogs(obj.Result,true).Completed += (handle) =>
        //        {
        //            //���������ϣ��ǵ��ͷ���Դ
        //            //Addressables.Release(handle);
        //            //Addressables.Release(obj);
        //        };
        //    }
        //};



        #endregion

        #region Ԥ����

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

        #region AddressablesMgr���ü�������
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
        //1.���Ȼ�ȡ���ذ��Ĵ�С
        //���Դ���Դ������ǩ�����������ߵ����
        AsyncOperationHandle<long> handleSize = Addressables.GetDownloadSizeAsync(new List<string> { "Cube", "Sphere", "SD" });
        yield return handleSize;
        //2.Ԥ����
        if (handleSize.Result > 0)
        {
            AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(
                new List<string> { "Cube", "Sphere", "SD" },
                Addressables.MergeMode.Union
                );
            while(!handle.IsDone)
            {
                //3.���ؽ���
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
        Debug.Log("�첽����");
        handle = Addressables.LoadAssetAsync<GameObject>("Cube");
        //�ȴ��������
        await handle.Task;

        Instantiate(handle.Result);
    }

    async void Loads()
    {
        AsyncOperationHandle<GameObject> handle1;
        AsyncOperationHandle<GameObject> handle2;
        Debug.Log("�첽����������");
        handle1 = Addressables.LoadAssetAsync<GameObject>("Cube");
        handle2 = Addressables.LoadAssetAsync<GameObject>("Player");
        //�ȴ������������
        await Task.WhenAll(handle1.Task, handle2.Task);

        Instantiate(handle1.Result);
        Instantiate(handle2.Result);
    }

    AsyncOperationHandle<GameObject> handle;
    IEnumerator LoadAsset()
    {
        Debug.Log("Э�̿���");
        handle = Addressables.LoadAssetAsync<GameObject>("Cube");
        //һ����û�м��سɹ���yield return;yield return handle��˼Ϊ�ȴ�handle���
        //if (!handle.IsDone)
        //    yield return handle;

        //��ס���߳�ǿ�Ƶȴ�
        handle.WaitForCompletion();

        while (!handle.IsDone)
        {
            DownloadStatus info = handle.GetDownloadStatus();
            //��ȡ���ؽ���
            Debug.Log(info.Percent);
            //��ȡ�����ֽ���
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


    
}
