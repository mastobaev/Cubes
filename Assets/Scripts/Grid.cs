using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Project/Scripts/Grid")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Grid : MonoBehaviour
{
    #region attributes

    /// <summary>
    /// Will update grid each frame in editor mode if true
    /// </summary>
    [SerializeField]
    bool m_DebugGrid = false;

    /// <summary>
    /// The distance between nearby lines
    /// </summary>
    [SerializeField]
    float m_GridLinesDistance = 1f;

    /// <summary>
    /// Grid color
    /// </summary>
    [SerializeField]
    Color m_GridColor = Color.blue;

    /// <summary>
    /// General grid width in the world space
    /// </summary>
    [SerializeField]
    float m_GridSize = 50f;

    #endregion

    #region engine methods

    void Awake()
    {
        CrtGrid();
    }

    void Update()
    {
        //recreate grid mesh each frame in the editor mode
        if (Application.isEditor && m_DebugGrid)
            CrtGrid();
    }

    #endregion

    #region service methods

    void CrtGrid()
    {
        transform.position = Vector3.zero;

        int linesNum = Mathf.Approximately(0f, m_GridLinesDistance) ? 0 : Mathf.RoundToInt(m_GridSize / m_GridLinesDistance);
        float centerOffset = -0.5f * m_GridSize;
        Vector3 origin = new Vector3(centerOffset, 0f, centerOffset);

        renderer.sharedMaterial.color = m_GridColor;
        Mesh gridMesh = new Mesh();
        List<Vector3> vertsList = new List<Vector3>();
        List<int> indicesList = new List<int>();

        for (int i = 0; i < linesNum; i++)
        {
            //x-axis
            float xAxisOffset = i * m_GridLinesDistance;
            Vector3 xv1 = origin + new Vector3(0f, 0f, xAxisOffset);
            Vector3 xv2 = origin + new Vector3(m_GridSize - m_GridLinesDistance, 0f, xAxisOffset);

            //z-axis
            float zAxisOffset = i * m_GridLinesDistance;
            Vector3 zv1 = origin + new Vector3(zAxisOffset, 0f, 0f);
            Vector3 zv2 = origin + new Vector3(zAxisOffset, 0f, m_GridSize - m_GridLinesDistance);

            vertsList.AddRange(new Vector3[] { xv1, xv2, zv1, zv2 });
            indicesList.AddRange(new int[] { i*4 + 0, i*4 + 1, i*4 + 2, i*4 + 3 });
        }

        gridMesh.Clear();
        gridMesh.vertices = vertsList.ToArray();
        gridMesh.SetIndices(indicesList.ToArray(), MeshTopology.Lines, 0);
        gridMesh.uv = new Vector2[vertsList.Count];
        gridMesh.normals = new Vector3[vertsList.Count];
        GetComponent<MeshFilter>().mesh = gridMesh;
    }

    #endregion

}
