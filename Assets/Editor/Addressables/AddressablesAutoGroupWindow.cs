using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.UIElements;

public class AddressablesAutoGroupWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private AddressableAssetSettings setting;
    private string defaultProfileName = "";

    const string aaResForderName = "AddressablesResources";

    public static string aaResForderRemotePath
    {
        get
        {
            return Path.Combine(
                Application.dataPath,
                aaResForderName,
                "Remote");
        }
    }
    public static string aaResForderLocalPath
    {
        get
        {
            return Path.Combine(
                Application.dataPath,
                aaResForderName,
                "Local");
        }
    }

    HashSet<string[]> resKey = new HashSet<string[]>();

    [MenuItem("Addressables/AddressablesAutoGroupWindow")]
    public static void ShowExample()
    {
        AddressablesAutoGroupWindow wnd = GetWindow<AddressablesAutoGroupWindow>();
        wnd.titleContent = new GUIContent("AddressablesAutoGroupWindow");
    }

    public void CreateGUI()
    {

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        if (setting == null)
        {
            setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        }

        root.Q<Button>("InitBtn").clicked += InitAddressables;
        root.Q<Button>("CreateForderBtn").clicked += OnCreateForderBtnClick;
        root.Q<Button>("GroupingBtn").clicked += OnGroupingBtnClick;
        root.Q<Button>("GenerateEnumClassBtn").clicked += OnGenerateEnumClassBtn;

        if (setting == null)
        {
            Debug.LogError("Settings为空！");
            return;
        }
        //获取所有的Profiles名称
        List<string> profileNames = setting.profileSettings.GetAllProfileNames();

        DropdownField dropdownField = rootVisualElement.Q<DropdownField>("ProfileDropdown");
        foreach (var item in profileNames)
        {
            dropdownField.choices.Add(item);
        }
        //设置默认选择索引
        dropdownField.index = 0;
        //设置默认的Profile名称
        defaultProfileName = dropdownField.choices[0];
        dropdownField.RegisterValueChangedCallback(OnProfileDropdownValueChanged);
    }
    private void InitAddressables()
    {
        if (setting == null)
        {
            setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
            if (setting == null)
            {
                AddressableAssetSettingsDefaultObject.Settings
                    = AddressableAssetSettings.Create(
                        AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                        AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true
                        );
                setting = AddressableAssetSettingsDefaultObject.Settings;
            }
        }
        else
        {
            Debug.Log("Addressables已经初始化");
            return;
        }
        setting.BuildRemoteCatalog = true;

        if (setting == null)
        {
            Debug.LogError("Settings为空！");
            return;
        }
        //获取所有的Profiles名称
        List<string> profileNames = setting.profileSettings.GetAllProfileNames();

        DropdownField dropdownField = rootVisualElement.Q<DropdownField>("ProfileDropdown");
        foreach (var item in profileNames)
        {
            dropdownField.choices.Add(item);
        }
        //设置默认选择索引
        dropdownField.index = 0;
        //设置默认的Profile名称
        defaultProfileName = dropdownField.choices[0];
        dropdownField.RegisterValueChangedCallback(OnProfileDropdownValueChanged);
    }
    private void OnProfileDropdownValueChanged(ChangeEvent<string> evt)
    {
        defaultProfileName = evt.newValue;
    }

    #region OnGroupingBtnClick
    private void OnGroupingBtnClick()
    {
        if (Directory.Exists(aaResForderRemotePath))
        {
            string groupName = "Remote";
            DirectoryInfo[] forderInfo = new DirectoryInfo(aaResForderRemotePath).GetDirectories();
            for (int i = 0; i < forderInfo.Length; i++)
            {
                MarkRes(forderInfo[i], groupName);
            }
        }
        else
        {
            Debug.LogError("Remote Forder Not Exist!");
        }
        if (Directory.Exists(aaResForderLocalPath))
        {
            string groupName = "Local";
            DirectoryInfo[] forderInfo = new DirectoryInfo(aaResForderLocalPath).GetDirectories();
            for (int i = 0; i < forderInfo.Length; i++)
            {
                MarkRes(forderInfo[i], groupName);
            }
        }
        else
        {
            Debug.LogError("Local Forder Not Exist!");
            return;
        }
        MarkStatus();
        GenerateEnumClass();

    }
    //根据资源分类Remote和Local
    private void MarkStatus()
    {
        for (int i = 0; i < setting.groups.Count; i++)
        {
            var group = setting.groups[i];
            foreach (var schema in group.Schemas)
            {
                if (schema is UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema)
                {
                    string buildPath = AddressableAssetSettings.kBuildPath;
                    string loadPath = AddressableAssetSettings.kLoadPath;
                    if (group.name.Contains("Local"))
                    {
                        buildPath = AddressableAssetSettings.kLocalBuildPath;
                        loadPath = AddressableAssetSettings.kLocalLoadPath;
                    }
                    else if (group.name.Contains("Remote"))
                    {
                        buildPath = AddressableAssetSettings.kRemoteBuildPath;
                        loadPath = AddressableAssetSettings.kRemoteLoadPath;
                    }
                    var bundleAssetGroupSchema = (schema as UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema);
                    bundleAssetGroupSchema.BuildPath.SetVariableByName(group.Settings, buildPath);
                    bundleAssetGroupSchema.LoadPath.SetVariableByName(group.Settings, loadPath);
                }
            }
        }
    }
    private void MarkRes(DirectoryInfo info, string groupName)
    {
        string subGroupName = groupName + "_" + info.Name;
        //查询当前路径下的所有目录
        DirectoryInfo[] subForders = new DirectoryInfo(info.FullName).GetDirectories();

        var group = setting.FindGroup(subGroupName);
        if (group == null)
        {
            group = setting.CreateGroup(
                subGroupName, false, false, false,
                new List<AddressableAssetGroupSchema>
                {
                    setting.DefaultGroup.Schemas[0],
                    setting.DefaultGroup.Schemas[1]
                }
                );
        }

        //查找当前目录下的所有资源
        FileInfo[] files = info.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension != ".meta")
            {
                string address = Path.GetFileNameWithoutExtension(files[i].FullName);
                string assetPath = "Assets" +
                    files[i].FullName.Replace(Application.dataPath.Replace("/", "\\"), "");
                assetPath.Replace("\\", "/");
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                var entry = setting.CreateOrMoveEntry(guid, group);
                if (entry.address != address)
                {
                    entry.SetAddress(address);
                    //Debug.Log("Label:" + info.Name + " " + "File:" + files[i]);
                    //通过HashSet集合来管理Label，如果HashSet中已经存在该值，则什么也不做
                    string[] labels = subGroupName.Split("_");
                    for (int j = 0; j < labels.Length; j++)
                    {
                        entry.SetLabel(labels[j], true, true);
                    }
                }
            }
        }
        for (int i = 0; i < subForders.Length; i++)
        {
            MarkRes(subForders[i], subGroupName);
        }
    }
    #endregion

    private void OnGenerateEnumClassBtn()
    {
        GenerateEnumClass();
    }
    private void GenerateEnumClass()
    {
        string path = Path.Combine(Application.dataPath, aaResForderName);
        DirectoryInfo[] subForders =
            new DirectoryInfo(path).GetDirectories();
        for (int i = 0; i < subForders.Length; i++)
        {
            GetAddressablesResKey(subForders[i]);
        }

        foreach (var item in resKey)
        {
            string str = "";
            foreach (var r in item)
            {
                str += r.ToString();
            }
            Debug.Log(str);
        }

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("public enum ADDRESSABLES_RESOURCES \n{\n");
        foreach (var item in resKey)
        {
            string itemStr = "";
            for (int i = 0; i < item.Length; i++)
            {
                if (i == 0)
                {
                    itemStr = "    " + item[i];
                }
                else
                {
                    itemStr = itemStr + "_" + item[i];
                }
            }
            stringBuilder.Append(itemStr + ",\n");

        }

        stringBuilder.Append("\n}");
        Debug.Log(stringBuilder.ToString());
        //写入文件

        string scriptPath =
            Path.Combine(Application.dataPath, "CFramework", "Addressables", "AddressablesResEnum.cs");
        FileStream file = new FileStream(scriptPath, FileMode.Create);
        StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
        fileW.Write(stringBuilder);
        fileW.Flush();
        fileW.Close();
        file.Close();
        //刷新资源列表
        AssetDatabase.Refresh();
    }

    void GetAddressablesResKey(DirectoryInfo info, string groupName = "")
    {
        string groupStr = "";
        if (groupName == "")
        {
            groupStr = info.Name;
        }
        else
        {
            groupStr = groupName + "_" + info.Name;
        }
        //查找当前目录下所有文件夹
        DirectoryInfo[] subForders =
            new DirectoryInfo(info.FullName).GetDirectories();
        //如果有文件夹，继续搜索文件夹内的资源
        for (int i = 0; i < subForders.Length; i++)
        {
            GetAddressablesResKey(subForders[i], groupStr);
        }
        //搜索当前文件夹内的资源
        FileInfo[] fileInfo = info.GetFiles();
        for (int i = 0; i < fileInfo.Length; i++)
        {
            if (fileInfo[i].Extension.Contains(".meta"))
            {
                continue;
            }
            string keyStr = groupStr + "_" + Path.GetFileNameWithoutExtension(fileInfo[i].Name);
            string[] keys = keyStr.Split("_");
            resKey.Add(keys);
        }
    }

    private void OnCreateForderBtnClick()
    {
        if (!Directory.Exists(aaResForderRemotePath))
        {
            Directory.CreateDirectory(aaResForderRemotePath);
        }
        if (!Directory.Exists(aaResForderLocalPath))
        {
            Directory.CreateDirectory(aaResForderLocalPath);
        }
        AssetDatabase.Refresh();
    }

}
