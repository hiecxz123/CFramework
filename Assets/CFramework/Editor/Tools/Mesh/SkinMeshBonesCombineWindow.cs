using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkinMeshBonesCombineWindow : EditorWindow
{
    public Transform baseTrans;
    public List<Transform> targetMeshs = new List<Transform>();
    private List<string> addedBones = new List<string>();
    private string addedBoneslabelStr;
    [MenuItem("Art/Skinned Mesh Bones Combine")]
    public static void OpenSkinMeshBonesCombineWindow()
    {
        var window = GetWindow<SkinMeshBonesCombineWindow>();
        window.titleContent = new GUIContent("Skin Updater");
    }
    private void OnGUI()
    {
        baseTrans = EditorGUILayout.ObjectField("Base Skin", baseTrans, typeof(Transform), true) as Transform;
        ScriptableObject baseSO = this;
        SerializedObject so = new SerializedObject(baseSO);
        SerializedProperty stringsProperty = so.FindProperty("targetSkins");
        EditorGUILayout.PropertyField(stringsProperty, new GUIContent("Merged Skin"), true); // True means show children
        so.ApplyModifiedProperties();
        bool isBasePrefab = false;
        if(baseTrans!=null)
        {
            var prefabtype = PrefabUtility.GetPrefabAssetType(baseTrans.transform);
            if (!(prefabtype == PrefabAssetType.NotAPrefab))
            {
                isBasePrefab = true;
            }
        }
        if (isBasePrefab)
        {
            GUILayout.Label("the baseSkin is Prefab!");
        }
        bool isTargetPrefab = false;
        foreach (var targetSkin in targetMeshs)
        {
            var prefabtype = PrefabUtility.GetPrefabAssetType(targetSkin.transform);
            if (!(prefabtype == PrefabAssetType.NotAPrefab))
            {
                isTargetPrefab = true;
            }
        }
        if (isTargetPrefab)
        {
            GUILayout.Label("the targetSkins has Prefab!");
        }
        if(!isBasePrefab&&!isTargetPrefab)
        {
            if (GUILayout.Button("Combine SkinMeshBones"))
            {
                CombineSkinMeshBones();
            }
            EditorGUILayout.SelectableLabel(addedBoneslabelStr, GUILayout.Height(500));
        }
    }

    void CombineSkinMeshBones()
    {
        Transform[] targetSkinBones;
        Transform[] baseSkinBones;
        SkinnedMeshRenderer[] targetSkimMeshs;
        SkinnedMeshRenderer baseSkinMesh;
        for (int r = 0; r < targetMeshs.Count; r++)
        {
            targetMeshs[r].position = baseTrans.position;
            targetMeshs[r].rotation = baseTrans.rotation;
            targetSkimMeshs = targetMeshs[r].GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var m_DonorSkinnedMeshRender in targetSkimMeshs)
            {
                targetSkinBones = m_DonorSkinnedMeshRender.bones;
                baseSkinMesh = baseTrans.GetComponentInChildren<SkinnedMeshRenderer>();
                baseSkinBones = baseSkinMesh.bones;
                for (int i = 0; i < targetSkinBones.Length; i++)
                {
                    if (targetSkinBones[i] == null)
                        continue;
                    string boneName = targetSkinBones[i].name;
                    bool found = false;
                    for (int j = 0; j < baseSkinBones.Length; j++)
                    {
                        if (baseSkinBones[j] != null)
                        {
                            if (baseSkinBones[j].name == boneName)
                            {
                                targetSkinBones[i] = baseSkinBones[j];
                                found = true;
                            }
                        }
                    }
                    if (!found)
                    {
                        string boneParent = targetSkinBones[i].transform.parent.name;
                        for (int j = 0; j < baseSkinBones.Length; j++)
                        {
                            if (baseSkinBones[j] != null)
                            {
                                if (baseSkinBones[j].name == boneParent)
                                {
                                    bool alreadyadd = false;
                                    foreach (var addedBone in addedBones)
                                    {
                                        if (addedBone.ToString() == targetSkinBones[i].name)
                                        {
                                            alreadyadd = true;
                                        }
                                    }
                                    if (!alreadyadd)
                                    {
                                        targetSkinBones[i].transform.parent = baseSkinBones[j];
                                        addedBones.Add(targetSkinBones[i].name);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                m_DonorSkinnedMeshRender.bones = targetSkinBones;
                m_DonorSkinnedMeshRender.rootBone = baseSkinMesh.rootBone;
                m_DonorSkinnedMeshRender.transform.parent = baseSkinMesh.transform.parent;
            }
        }
        foreach (var addedBone in addedBones)
        {
            addedBoneslabelStr += System.Environment.NewLine + "bone:" + addedBone;
        }
    }
}
