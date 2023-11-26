using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(PlayerInputRebindData))]
public class PlayerInputRebindDataEditor : Editor
{
    private SerializedProperty m_actionProperty;
    private SerializedProperty m_bindingIdProperty;

    private GUIContent m_bindingLabel = new GUIContent("Binding");
    private string[] m_bindingOptionValues;
    private GUIContent[] m_bindingOptions;

    private int m_selectedBindingOption;
    protected void OnEnable()
    {
        m_actionProperty = serializedObject.FindProperty("m_action");
        m_bindingIdProperty = serializedObject.FindProperty("m_bindingId");
        RefreshBindingOptions();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        // Binding section.
        EditorGUILayout.LabelField(m_bindingLabel, Styles.boldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(m_actionProperty);

            var newSelectedBinding = EditorGUILayout.Popup(m_bindingLabel, m_selectedBindingOption, m_bindingOptions);
            if (newSelectedBinding != m_selectedBindingOption)
            {
                var bindingId = m_bindingOptionValues[newSelectedBinding];
                m_bindingIdProperty.stringValue = bindingId;
                m_selectedBindingOption = newSelectedBinding;
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            RefreshBindingOptions();
        }
    }

    protected void RefreshBindingOptions()
    {
        var actionReference = (InputActionReference)m_actionProperty.objectReferenceValue;
        var action = actionReference?.action;

        if (action == null)
        {
            m_bindingOptions = new GUIContent[0];
            m_bindingOptionValues = new string[0];
            m_selectedBindingOption = -1;
            return;
        }

        var bindings = action.bindings;
        var bindingCount = bindings.Count;

        m_bindingOptions = new GUIContent[bindingCount];
        m_bindingOptionValues = new string[bindingCount];
        m_selectedBindingOption = -1;

        var currentBindingId = m_bindingIdProperty.stringValue;
        for (var i = 0; i < bindingCount; ++i)
        {
            var binding = bindings[i];
            var bindingId = binding.id.ToString();
            var haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

            // If we don't have a binding groups (control schemes), show the device that if there are, for example,
            // there are two bindings with the display string "A", the user can see that one is for the keyboard
            // and the other for the gamepad.
            var displayOptions =
                InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
            if (!haveBindingGroups)
                displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

            // Create display string.
            var displayString = action.GetBindingDisplayString(i, displayOptions);

            // If binding is part of a composite, include the part name.
            if (binding.isPartOfComposite)
                displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

            // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
            // by instead using a backlash.
            displayString = displayString.Replace('/', '\\');

            // If the binding is part of control schemes, mention them.
            if (haveBindingGroups)
            {
                var asset = action.actionMap?.asset;
                if (asset != null)
                {
                    var controlSchemes = string.Join(", ",
                        binding.groups.Split(InputBinding.Separator)
                            .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                    displayString = $"{displayString} ({controlSchemes})";
                }
            }

            m_bindingOptions[i] = new GUIContent(displayString);
            m_bindingOptionValues[i] = bindingId;

            if (currentBindingId == bindingId)
                m_selectedBindingOption = i;
        }
    }

    private static class Styles
    {
        public static GUIStyle boldLabel = new GUIStyle("MiniBoldLabel");
    }
}
