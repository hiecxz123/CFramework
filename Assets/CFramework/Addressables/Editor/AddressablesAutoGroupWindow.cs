using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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

    string localBuildPath = "";
    string localLoadPath = "";
    string remoteBuildPath = "";
    string remoteLoadPath = "";

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

        if (setting == null)
        {
            setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        }
        //获取所有的Profiles名称
        List<string> profileNames = setting.profileSettings.GetAllProfileNames();


        DropdownField dropdownField = root.Q<DropdownField>("ProfileDropdown");
        foreach (var item in profileNames)
        {
            dropdownField.choices.Add(item);
        }
        //设置默认选择索引
        dropdownField.index = 0;
        //设置默认的Profile名称
        defaultProfileName = dropdownField.choices[0];
        dropdownField.RegisterValueChangedCallback(OnProfileDropdownValueChanged);

        root.Q<Button>("CreateForderBtn").clicked += OnCreateForderBtnClick;
        root.Q<Button>("GroupingBtn").clicked += OnGroupingBtnClick;
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
        }
    }

    public void MarkRes(DirectoryInfo info, string groupName)
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
            //group.Settings.activeProfileId = setting.activeProfileId;

            if (groupName.Contains("Remote"))
            {

            }

        }
        //Debug.Log(group.Settings..SetVariableByName(setting.g));
        group.Settings.RemoteCatalogBuildPath.GetValue(group.Settings);

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
                Debug.Log(assetPath);
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

    public void OnCreateForderBtnClick()
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
