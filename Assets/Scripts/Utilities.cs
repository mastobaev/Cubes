using UnityEngine;
using System.Collections;

public static class Utilities
{
    #region public methods

    public static GameObject CrtWireframe(Vector3[] _Vertices)
    {
        GameObject obj = new GameObject("wireframe");
        obj.AddComponent<MeshRenderer>();
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.vertices = _Vertices;
        int[] indices = new int[_Vertices.Length];
        for (int i = 0; i < indices.Length; i++)
            indices[i] = i;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        mesh.uv = new Vector2[_Vertices.Length];
        mesh.normals = new Vector3[_Vertices.Length];
        mf.mesh = mesh;
        return obj;
    }

    #endregion
}
