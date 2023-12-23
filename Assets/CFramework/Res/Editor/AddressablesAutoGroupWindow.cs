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

        //��ʼ����Դ·��
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

        root.Q<Button>("CreateForderBtn").clicked += OnCreateForderBtnClick;
        root.Q<Button>("GroupingBtn").clicked += OnGroupingBtnClick;
    }

    private void OnGroupingBtnClick()
    {
        if (Directory.Exists(aaResForderRemotePath))
        {
            string[] files = Directory.GetFiles(aaResForderRemotePath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i]);
            }
        }
        else
        {
            Debug.LogError("Remote Forder Not Exist!");
        }
        if (Directory.Exists(aaResForderLocalPath))
        {

            //string[] files = Directory.GetFiles(aaResForderLocalPath, "*", SearchOption.AllDirectories);

            string groupName = "Local";
            DirectoryInfo[] forderInfo = new DirectoryInfo(aaResForderLocalPath).GetDirectories();
            for (int i = 0; i < forderInfo.Length; i++)
            {
                //Debug.Log(forderInfo[i].FullName);
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
        //��ѯ��ǰ·���µ�����Ŀ¼
        DirectoryInfo[] subForders = new DirectoryInfo(info.FullName).GetDirectories();
        Debug.Log(subGroupName);
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
        //���ҵ�ǰĿ¼�µ�������Դ
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
