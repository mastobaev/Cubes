using UnityEngine;
using System.Collections;

[AddComponentMenu("Project/Scripts/Paint")]
public class Paint : MonoBehaviour
{
    #region factory

    static public Paint instance = null;

    #endregion

    #region types

    public enum TOOLS
    {
        NONE,
        PENCIL,
        ERASER
    }

    #endregion

    #region properties

    public TOOLS Tool
    {
        set
        {
            m_Tool = value;
        }
    }

    #endregion

    #region attributes

    [SerializeField]
    Color m_DefaultCanvasColor;

    [SerializeField]
    Color m_Color;

    Texture2D m_Canvas;
    Color[] m_Buffer;

    TOOLS m_Tool = TOOLS.PENCIL;

    Vector2 m_PrevPencilPos;

    bool m_Changed = true;

    #endregion

    #region engine methods

    void Awake()
    {
        m_Canvas = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        m_Buffer = new Color[m_Canvas.width * m_Canvas.height];
        FillCanvas(m_DefaultCanvasColor);
        instance = this;
    }

    void OnGUI()
    {
        DrawToolBar();

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_Canvas);

        if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
        {
            switch(m_Tool)
            {
                case TOOLS.PENCIL:
                Pencil(Input.mousePosition);
                break;

                case TOOLS.ERASER:
                Eraser(Input.mousePosition);
                break;
            }
        }
    }

    void LateUpdate()
    {
        if (m_Changed)
        {
            m_Canvas.SetPixels(m_Buffer);
            m_Canvas.Apply();
            m_Changed = false;
        }
    }

    #endregion

    #region tools

    void Clear()
    {
        FillCanvas(m_DefaultCanvasColor);
    }

    void Eraser(Vector2 _Point)
    {
        SetPixel(_Point, m_DefaultCanvasColor, 5);
    }

    void Pencil(Vector2 _Point)
    {
        if (Event.current.type == EventType.MouseDown)
            m_PrevPencilPos = _Point;

        SetPixel(_Point, m_Color);
        Line(_Point, m_PrevPencilPos, m_Color);
        m_PrevPencilPos = _Point;
    }

    void Line(Vector2 _P1, Vector2 _P2, Color _Color)
    {
        Vector2 dir = _P2 - _P1;
        dir.Normalize();
        int steps = Mathf.RoundToInt((_P2 - _P1).magnitude);

        for (int i = 0; i < steps; i++)
        {
            SetPixel(_P1 + i * dir, _Color);
        }
    }

    #endregion

    #region service methods

    void SetPixel(Vector2 _Point, Color _Color, int _Radius = 2)
    {
        m_Buffer[Mathf.Clamp((int)_Point.x, 0, Screen.width - 1) + m_Canvas.width * Mathf.Clamp((int)_Point.y, 0, Screen.height - 1)] = _Color;

        for (int i = 0; i < _Radius; i++)
        {
            for (int j = 0; j < _Radius; j++)
            {
                SetPixel(_Point + new Vector2(i, j), _Color, 0);
                SetPixel(_Point + new Vector2(-i, -j), _Color, 0);
                SetPixel(_Point + new Vector2(-i, j), _Color, 0);
                SetPixel(_Point + new Vector2(i, -j), _Color, 0);
            }
        }

        m_Changed = true;
    }

    void DrawToolBar()
    {
        if (GUI.Button(new Rect(0, 0.15f * Screen.height, 0.1f * Screen.width, 0.15f * Screen.height), "Pencil"))
        {
            m_Tool = TOOLS.PENCIL;
        }

        if (GUI.Button(new Rect(0, 0.3f * Screen.height, 0.1f * Screen.width, 0.15f * Screen.height), "Eraser"))
        {
            m_Tool = TOOLS.ERASER;
        }

        if (GUI.Button(new Rect(0, 0.45f * Screen.height, 0.1f * Screen.width, 0.15f * Screen.height), "Clear"))
        {
            m_Tool = TOOLS.NONE;
            Clear();
        }
    }

    void FillCanvas(Color _Color)
    {
        for (int i = 0; i < m_Canvas.width; i++)
            for (int j = 0; j < m_Canvas.height; j++)
                m_Buffer[i + j*m_Canvas.width] = _Color;

        m_Changed = true;
    }

    #endregion
}
