using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HuatuoAutoToolsEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Huatuo/HuatuoAutoTools")]
    public static void ShowExample()
    {
        HuatuoAutoToolsEditor wnd = GetWindow<HuatuoAutoToolsEditor>();
        wnd.titleContent = new GUIContent("HuatuoAutoToolsEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        
    }
}
