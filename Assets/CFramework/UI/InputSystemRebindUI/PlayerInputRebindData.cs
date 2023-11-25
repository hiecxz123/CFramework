using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInputRebindUI))]
public class PlayerInputRebindData : MonoBehaviour
{
    public InputActionReference m_action;
    public string m_bindingId;
}
