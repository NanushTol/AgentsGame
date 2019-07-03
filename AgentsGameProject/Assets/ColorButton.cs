
using UnityEngine;


public class ColorButton : MonoBehaviour
{
    public GameObject ColorPanel;
    bool panelActive;

    public void ColorButtonClick()
    {
        panelActive = ColorPanel.activeInHierarchy;
        panelActive = !panelActive;
        ColorPanel.SetActive(panelActive);
    }
}
