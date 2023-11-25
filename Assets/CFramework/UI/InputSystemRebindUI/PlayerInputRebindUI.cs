using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputRebindData))]
public class PlayerInputRebindUI : MonoBehaviour
{
    public PlayerInputRebindData m_playerInputRebindData;

    public InputBinding.DisplayStringOptions m_displayStringOptions;
    public TMP_Text m_actionNameTmpText;
    public Button m_rebindBtn;
    public TMP_Text m_keyName;
    public Button m_resetBtn;

    private InputActionRebindingExtensions.RebindingOperation m_rebindingOperation;
    private void Start()
    {
        //���½���
        UpdateBindingDisplay();
        m_rebindBtn.onClick.AddListener(RebingingKeyBtnOnClick);
        m_resetBtn.onClick.AddListener(ResetBtnOnClick);

    }
    /// <summary>
    /// ������ť���°�
    /// </summary>
    void RebingingKeyBtnOnClick()
    {
        //�󶨿�ʼǰ����Ҫ��ActionMap�л�������Map�У������޷��󶨣��ᱨ��
        StartInteractiveRebind();
    }

    void StartInteractiveRebind()
    {
        //����Inspector��������õ�bindingId������bingding
        if (!FindActionAndBingding(out var action, out var bindingIndex))
            return;
        //�жϰ��Ƿ��ǻ�����룬��WASD��Vector2���룬����ǻ�����룬�Ի��������Ҫ��������
        if (action.bindings[bindingIndex].isComposite)
        {
            var firstPartIndex = bindingIndex + 1;
            if (firstPartIndex < action.bindings.Count
                && action.bindings[firstPartIndex].isPartOfComposite)
                PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
        }
        else
        {
            PerformInteractiveRebind(action, bindingIndex);
        }

    }
    /// <summary>
    /// ��ʼ�԰��������°�
    /// </summary>
    /// <param name="action">PlayerInputReference��Ӧ�Ķ���</param>
    /// <param name="bindingIndex">������</param>
    /// <param name="allCompositeParts">�Ƿ��ǻ�����룬��wasd</param>
    void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        m_rebindingOperation?.Cancel();
        void CleanUp()
        {
            //�ͷ��ڴ�
            m_rebindingOperation?.Dispose();
            m_rebindingOperation = null;
        }
        m_rebindingOperation = m_playerInputRebindData.m_action.action.PerformInteractiveRebinding(bindingIndex)
            //��ϣ��������������
            //.WithControlsExcluding("Mouse")
            //������������ļ��
            //.OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
            {
                //���½���
                UpdateBindingDisplay();
                //ȡ����ʱ��Ҫ�ͷ��ڴ�
                CleanUp();
            })
            //���ʱִ�е��߼�
            .OnComplete(operation =>
            {
                //���½���
                UpdateBindingDisplay();
                //�����ڴ�
                CleanUp();
                //�ж��Ƿ��ǻ������
                if (allCompositeParts)
                {
                    var nextBindingIndex = bindingIndex + 1;
                    if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                        //����ǻ�����룬��Ҫ�ٴε��ð󶨣����еݹ����
                        PerformInteractiveRebind(action, nextBindingIndex, true);
                }
                
            });
        //ƴ��չʾ��ʾ�ַ�
        var partName = default(string);
        //����ǻ�����룬ƴ�ӵ�������İ����ַ�
        if (action.bindings[bindingIndex].isPartOfComposite)
            partName = $"Binding '{action.bindings[bindingIndex].name}'. ";
        if (m_keyName != null)
        {
            var text = !string.IsNullOrEmpty(m_rebindingOperation.expectedControlType)
                ? $"{partName}Waiting for {m_rebindingOperation.expectedControlType} input..."
                : $"{partName}Waiting for input...";
            m_keyName.text = text;
        }
        //����Start��������
        m_rebindingOperation.Start();
    }

    bool FindActionAndBingding(out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;
        //�ж�playerInputReference�Ƿ�ֵ
        action = m_playerInputRebindData?.m_action;

        if (action == null)
            return false;
        //�ж��Ƿ�ѡ����bindingId
        if (string.IsNullOrEmpty(m_playerInputRebindData.m_bindingId))
            return false;

        var bindingId = new Guid(m_playerInputRebindData.m_bindingId);
        //ͨ��bindingId����Guid����ȡbingingIndex
        bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
        if (bindingIndex == -1)
        {
            Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
            return false;
        }
        return true;
    }

    void UpdateBindingDisplay()
    {
        //��ȡ��������
        m_actionNameTmpText.text = m_playerInputRebindData.m_action.name.Split('/')[1] + ":";
        //��ȡ����ɶ��İ�������
        var displayString = string.Empty;
        var deviceLayoutName = default(string);
        var controlPath = default(string);
        var action = m_playerInputRebindData.m_action?.action;
        if (action != null)
        {
            var bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_playerInputRebindData.m_bindingId);
            if (bindingIndex != -1)
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, m_displayStringOptions);
        }
        if (m_keyName != null)
            m_keyName.text = displayString;

    }

    private void ResetBtnOnClick()
    {
        if (!FindActionAndBingding(out var action, out var bindingIndex))
            return;

        if (action.bindings[bindingIndex].isComposite)
        {
            // It's a composite. Remove overrides from part bindings.
            for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                action.RemoveBindingOverride(i);
        }
        else
        {
            action.RemoveBindingOverride(bindingIndex);
        }
        UpdateBindingDisplay();
    }

}
