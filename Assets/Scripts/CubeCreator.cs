/*
 *                Cube gizmo vertex indices map
 * 
 *              
 *                 (6)_____________________(7)
 *                 /.                      /|
 *                / .                     / |
 *              (2)_____________________(3) |
 *               |  .                    |  |
 *               |  .                    |  |
 *               |  .                    |  |
 *               |  .                    |  |
 *               |  .                    |  |
 *               | (4)...................|_(5)
 *               | .                     | /
 *               |.                      |/
 *              (0)_____________________(1)
 * 
 * 
 */

using UnityEngine;
using System.Collections;

[AddComponentMenu("Project/Scripts/CubeCreator")]
public class CubeCreator : MonoBehaviour
{
    #region types

    enum CheckPointMap
    {
        P0,
        P1,
        NONE
    }

    #endregion

    #region constants

    const int FLOOR_COLLIDER_TAG = 0;

    #endregion

    #region attributes

    GameObject m_Cube;

    Vector2 m_P0;
    Vector2 m_P1;
    CheckPointMap m_CurrAction = CheckPointMap.NONE;

    #endregion

    #region engine methods

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 0.15f * Screen.width, 0.15f * Screen.height), "Mastrubate cube"))
        {
            m_CurrAction = CheckPointMap.P0;
        }

        if (Event.current.type == EventType.mouseUp)
        {
            switch (m_CurrAction)
            {
                case CheckPointMap.P0:
                    m_P0 = Input.mousePosition;
                    m_CurrAction = CheckPointMap.P1;
                    break;
                case CheckPointMap.P1:
                    m_P1 = Input.mousePosition;
                    m_CurrAction = CheckPointMap.NONE;
                    CrtCube(m_P0, m_P1);
                    break;
            }
        }
    }

    #endregion

    #region service methods

    /// <summary>
    /// Createcube gizmo according to the two point on the screen projection
    /// </summary>
    /// <param name="_P0">Screen projection point for the (0) vertex of the cube gizmo</param>
    /// <param name="_P1">Screen projection point for the (1) vertex of the cube gizmo</param>
    void CrtCube(Vector2 _P0, Vector2 _P1)
    {
        try
        {
            Vector3 pos = PointToFloorPlane(_P0);
            Vector3 v0 = PointToFloorPlane(_P0);
            Vector3 v1 = PointToFloorPlane(_P1);

            float cubeSize = (v1 - v0).magnitude;
            v0 -= pos;
            v1 -= pos;

            float angle = Vector3.Angle(Vector3.right, v1) * (Vector3.Dot(-Vector3.back, v1) > 0 ? 1 : -1);

            v1 = cubeSize * Vector3.right;

            Vector3 v2 = cubeSize * Vector3.up;
            Vector3 v3 = cubeSize * (Vector3.up + Vector3.right);
            Vector3 v4 = cubeSize * -Vector3.back;
            Vector3 v5 = cubeSize * (-Vector3.back + Vector3.right);
            Vector3 v6 = cubeSize * (-Vector3.back + Vector3.up);
            Vector3 v7 = cubeSize * (-Vector3.back + Vector3.right + Vector3.up);

            GameObject cube = Utilities.CrtWireframe(new Vector3[] { v0, v1, v1, v3, v3, v2, v2, v0, v4, v5, v5, v7, v7, v6, v6, v4, v0, v4, v1, v5, v2, v6, v3, v7 });
            cube.transform.Translate(pos);
            cube.transform.Rotate(Vector3.up, -angle);

            if (m_Cube != null)
                Destroy(m_Cube);

            m_Cube = cube;
        }
        catch (System.Exception e)
        {
            Debug.LogError("CrtCube exeption: " + e.Message);
        }
    }

    /// <summary>
    /// Returns world space point for the _P
    /// </summary>
    /// <param name="_P">Screen projection point on the floor plane</param>
    /// <returns></returns>
    Vector3 PointToFloorPlane(Vector2 _P)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(_P.x, _P.y, 0f));
        RaycastHit hit;
        Vector3 v;
        if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, 1 << FLOOR_COLLIDER_TAG))
            v = hit.point;
        else
            throw new System.Exception("Can't cast v0");

        return v;
    }

    #endregion
}
