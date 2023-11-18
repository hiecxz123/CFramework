using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
[AddComponentMenu("UI/MyImage", 11)]
public class MyImageUI : Image
{
    List<UIVertex> uIVertices = new List<UIVertex>();
    VertexHelper[] vertexs;
    protected override void Start()
    {
        
    }
    protected override void OnPopulateMesh(VertexHelper m)
    {
        base.OnPopulateMesh(m);
        m.GetUIVertexStream(uIVertices);
        UIVertex u = new UIVertex();
        
        u = uIVertices[1];
        Debug.Log(u.position);
        u.position.x -= 100;
        Debug.Log(u.position);
        m.SetUIVertex(u, 1);


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
