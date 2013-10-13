using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RenderQueueSetter : MonoBehaviour
{
    [SerializeField]
    int m_Queue = 1;

    protected void Start()
    {
        renderer.sharedMaterial.renderQueue = m_Queue;
    }

}
