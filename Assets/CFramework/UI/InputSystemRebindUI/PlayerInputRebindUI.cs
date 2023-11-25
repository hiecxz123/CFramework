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
        //更新界面
        UpdateBindingDisplay();
        m_rebindBtn.onClick.AddListener(RebingingKeyBtnOnClick);
        m_resetBtn.onClick.AddListener(ResetBtnOnClick);

    }
    /// <summary>
    /// 单个按钮重新绑定
    /// </summary>
    void RebingingKeyBtnOnClick()
    {
        //绑定开始前，需要将ActionMap切换到其他Map中，否则无法绑定，会报错
        StartInteractiveRebind();
    }

    void StartInteractiveRebind()
    {
        //根据Inspector面版上设置的bindingId来查找bingding
        if (!FindActionAndBingding(out var action, out var bindingIndex))
            return;
        //判断绑定是否是混合输入，如WASD的Vector2输入，如果是混合输入，对混合输入需要单独处理
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
    /// 开始对按键的重新绑定
    /// </summary>
    /// <param name="action">PlayerInputReference对应的动作</param>
    /// <param name="bindingIndex">绑定索引</param>
    /// <param name="allCompositeParts">是否是混合输入，如wasd</param>
    void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
    {
        m_rebindingOperation?.Cancel();
        void CleanUp()
        {
            //释放内存
            m_rebindingOperation?.Dispose();
            m_rebindingOperation = null;
        }
        m_rebindingOperation = m_playerInputRebindData.m_action.action.PerformInteractiveRebinding(bindingIndex)
            //不希望接收鼠标的输入
            //.WithControlsExcluding("Mouse")
            //监听其它输入的间隔
            //.OnMatchWaitForAnother(0.1f)
            .OnCancel(operation =>
            {
                //更新界面
                UpdateBindingDisplay();
                //取消绑定时需要释放内存
                CleanUp();
            })
            //完成时执行的逻辑
            .OnComplete(operation =>
            {
                //更新界面
                UpdateBindingDisplay();
                //清理内存
                CleanUp();
                //判断是否是混合输入
                if (allCompositeParts)
                {
                    var nextBindingIndex = bindingIndex + 1;
                    if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                        //如果是混合输入，需要再次调用绑定，进行递归调用
                        PerformInteractiveRebind(action, nextBindingIndex, true);
                }
                
            });
        //拼接展示显示字符
        var partName = default(string);
        //如果是混合输入，拼接单个输入的按键字符
        if (action.bindings[bindingIndex].isPartOfComposite)
            partName = $"Binding '{action.bindings[bindingIndex].name}'. ";
        if (m_keyName != null)
        {
            var text = !string.IsNullOrEmpty(m_rebindingOperation.expectedControlType)
                ? $"{partName}Waiting for {m_rebindingOperation.expectedControlType} input..."
                : $"{partName}Waiting for input...";
            m_keyName.text = text;
        }
        //调用Start来启动绑定
        m_rebindingOperation.Start();
    }

    bool FindActionAndBingding(out InputAction action, out int bindingIndex)
    {
        bindingIndex = -1;
        //判断playerInputReference是否赋值
        action = m_playerInputRebindData?.m_action;

        if (action == null)
            return false;
        //判断是否选择了bindingId
        if (string.IsNullOrEmpty(m_playerInputRebindData.m_bindingId))
            return false;

        var bindingId = new Guid(m_playerInputRebindData.m_bindingId);
        //通过bindingId构建Guid，获取bingingIndex
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
        //获取动作名称
        m_actionNameTmpText.text = m_playerInputRebindData.m_action.name.Split('/')[1] + ":";
        //获取人类可读的按键名称
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
