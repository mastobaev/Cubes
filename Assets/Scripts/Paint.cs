using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Project/Scripts/Paint")]
public class Paint : MonoBehaviour
{
    #region constants

    const int SKETCH_RADIUS = 10;

    #endregion

    #region factory

    static public Paint instance = null;

    #endregion

    #region types

    public enum TOOLS
    {
        NONE,
        PENCIL,
        ERASER,
        BRUSH,
        SKETCH
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
    int[] m_SketchBuffer;

    TOOLS m_Tool = TOOLS.SKETCH;

    Vector2 m_PrevPencilPos;

    bool m_Changed = true;

    #endregion

    #region engine methods

    void Awake()
    {
        m_Canvas = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        m_Buffer = new Color[m_Canvas.width * m_Canvas.height];
        m_SketchBuffer = new int[m_Buffer.Length];
        FillCanvas(m_DefaultCanvasColor);
        instance = this;
    }

    void OnGUI()
    {
        DrawToolBar();

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), m_Canvas);

        if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
        {
            if (Event.current.type == EventType.MouseDown)
                m_PrevPencilPos = Input.mousePosition;

            switch(m_Tool)
            {
                case TOOLS.PENCIL:
                Pencil(Input.mousePosition);
                break;

                case TOOLS.ERASER:
                Eraser(Input.mousePosition);
                break;

                case TOOLS.BRUSH:
                Brush(Input.mousePosition);
                break;

                case TOOLS.SKETCH:
                Sketch(Input.mousePosition);
                break;
            }

            m_PrevPencilPos = Input.mousePosition;
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
        SetPoint(_Point, m_DefaultCanvasColor, 5, false);
    }

    void Pencil(Vector2 _Point)
    {
        Line(_Point, m_PrevPencilPos, m_Color, false, 1);
    }

    void Brush(Vector2 _Point)
    {
        Line(_Point, m_PrevPencilPos, m_Color, true, 2);
    }

    void Sketch(Vector2 _Point)
    {
        m_SketchBuffer[Point2Index(_Point)] = 1;
        Color c = m_Color;
        c.a = 0.3f;

        foreach (Vector2 point in GetRadiusSketchPoints(_Point, SKETCH_RADIUS))
            Line(point, _Point, c, true, 2);
    }

    void Line(Vector2 _P1, Vector2 _P2, Color _Color, bool _Smooth, int _Radius)
    {
        Vector2 dir = _P2 - _P1;
        dir.Normalize();
        int steps = Mathf.RoundToInt((_P2 - _P1).magnitude);

        for (int i = 0; i <= steps; i++)
            SetPoint(_P1 + i * dir, _Color, _Radius, _Smooth);
    }

    #endregion

    #region service methods

    Vector2[] GetRadiusSketchPoints(Vector2 _Point, int _Radius)
    {
        List<Vector2> indexList = new List<Vector2>();

        for (int i = 0; i < _Radius; i++)
        {
            for (int j = 0; j < _Radius; j++)
            {
                if (i != 0 || j != 0)
                {
                    CheckSketchPoint(_Point, i, j, indexList);
                    CheckSketchPoint(_Point, -i, j, indexList);
                    CheckSketchPoint(_Point, i, -j, indexList);
                    CheckSketchPoint(_Point, -i, -j, indexList);
                }
            }
        }

        return indexList.ToArray();
    }

    void CheckSketchPoint(Vector2 _Start, int _X, int _Y, List<Vector2> _IndexList)
    {
        Vector2 point = _Start + new Vector2(_X, _Y);
        int index = Point2Index(point);
        if (index < 0 || index >= m_SketchBuffer.Length)
            return;
        if (m_SketchBuffer[Point2Index(point)] == 1)
            _IndexList.Add(point);
    }

    int Distance(int _X, int _Y)
    {
        return Mathf.CeilToInt(Mathf.Sqrt(_X * _X + _Y * _Y));
    }

    void SetPoint(Vector2 _Point, Color _Color, int _Radius, bool _Smooth)
    {
        int startBlend = 2;

        for (int i = 0; i < _Radius; i++)
        {
            for (int j = 0; j < _Radius; j++)
            {
                Color c = new Color(_Color.r, _Color.g, _Color.b, _Color.a);
                int currRadius = Distance(i, j);
                int deltaRadius = _Radius - currRadius;

                if (_Smooth && deltaRadius <= startBlend)
                {
                    int offset = startBlend - deltaRadius;
                    int pow = offset * offset + 1;
                    c.a *= Mathf.Pow(0.4f, pow);
                }

                SetPixel(_Point + new Vector2(i, j), c, _Smooth);
                SetPixel(_Point + new Vector2(-i, -j), c, _Smooth);
                SetPixel(_Point + new Vector2(-i, j), c, _Smooth);
                SetPixel(_Point + new Vector2(i, -j), c, _Smooth);
            }
        }

        m_Changed = true;
    }

    int Point2Index(Vector2 _Point)
    {
        int x = Mathf.Clamp((int)_Point.x, 0, Screen.width - 1);
        int y = Mathf.Clamp((int)_Point.y, 0, Screen.height - 1);
        int index = x + m_Canvas.width * y;
        return index;
    }

    void SetPixel(Vector2 _Point, Color _Color, bool _Smooth)
    {
        int index = Point2Index(_Point);
        if (index < 0 || index >= m_Buffer.Length)
            return;

        if (!_Smooth)
        {
            m_Buffer[index] = _Color;
        }
        else
        {
            Color prev = m_Buffer[index];
            Color c = _Color * (1f - _Color.a) + prev * _Color.a;
            c.a = prev.a * (1f - _Color.a) + _Color.a;
            m_Buffer[index] = c;
        }

        if (m_Buffer[index] == m_DefaultCanvasColor)
            m_SketchBuffer[index] = 0;

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

        if (GUI.Button(new Rect(0, 0.6f * Screen.height, 0.1f * Screen.width, 0.15f * Screen.height), "Brush"))
        {
            m_Tool = TOOLS.BRUSH;
        }

        if (GUI.Button(new Rect(0, 0.75f * Screen.height, 0.1f * Screen.width, 0.15f * Screen.height), "Sketch"))
        {
            m_Tool = TOOLS.SKETCH;
        }
    }

    void FillCanvas(Color _Color)
    {
        for (int i = 0; i < m_Canvas.width; i++)
            for (int j = 0; j < m_Canvas.height; j++)
                SetPixel(new Vector2(i, j), _Color, false);

        m_Changed = true;
    }

    #endregion
}
