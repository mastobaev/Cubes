using UnityEngine;
using System.Collections;

[AddComponentMenu("Project/Scripts/AreaMenu")]
public class AreaMenu : MonoBehaviour
{
    #region engine methods

    void Awake()
    {
        Application.LoadLevelAdditive("Area1");
    }

    void OnGUI()
    {
        /*
        if (GUI.Button(new Rect(Screen.width - 0.1f * Screen.width, 0f, 0.1f * Screen.width, 0.15f * Screen.height), "Area1"))
        {
            Application.LoadLevelAdditive("Area1");
        }
         * */
    }

    #endregion
}
