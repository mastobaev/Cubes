using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class RenderQueueSetter : MonoBehaviour
{
    [SerializeField]
    int m_Queue = 1;

    [SerializeField]
    int[] m_Queues;

    protected void Start()
    {
        if (!renderer || !renderer.sharedMaterial || m_Queues == null)
            return;
        renderer.sharedMaterial.renderQueue = m_Queue;
        for (int i = 0; i < m_Queues.Length && i < renderer.sharedMaterials.Length; i++)
            renderer.sharedMaterials[i].renderQueue = m_Queues[i];
    }

}
