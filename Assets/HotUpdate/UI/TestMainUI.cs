using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestMainUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI targetText;
    // Start is called before the first frame update
    void Start()
    {
        infoText.text = "This is TestMainPanel <color=#00FF00FF>Info</color>";
        targetText = transform.Find("TargetTxt").GetComponent<TextMeshProUGUI>();
        targetText.text = "This is TestMainPanel <color=#00FF00FF>Target</color>";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
