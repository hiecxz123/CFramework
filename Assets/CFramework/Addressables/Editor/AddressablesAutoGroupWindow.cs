using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    string aaResForderRemotePath = "";
    string aaResForderLocalPath = "";

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

        //初始化资源路径
        aaResForderRemotePath =
            Path.Combine(
                Application.dataPath,
                rootVisualElement.Q<TextField>("ResForderTxtField").text,
                "Remote");
        aaResForderLocalPath =
            Path.Combine(
                Application.dataPath,
                rootVisualElement.Q<TextField>("ResForderTxtField").text,
                "Local");


        root.Q<Button>("InitBtn").clicked += InitAddressables;
        root.Q<Button>("CreateForderBtn").clicked += OnCreateForderBtnClick;
        root.Q<Button>("GroupingBtn").clicked += OnGroupingBtnClick;

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
                //Debug.Log(assetPath);
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                var entry = setting.CreateOrMoveEntry(guid, group);
                if (entry.address != address)
                {
                    entry.SetAddress(address);
                }
            }
        }
        for (int i = 0; i < subForders.Length; i++)
        {
            MarkRes(subForders[i], subGroupName);
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
