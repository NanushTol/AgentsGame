using UnityEngine;
using UnityEngine.UI;

public class DisplayBoolVariable : MonoBehaviour
{

    public BoolVariable ToggleToDisplay;

    Toggle toggleElement;

    SelectObject selectedObject;

    private void Awake()
    {
        toggleElement = this.GetComponent<Toggle>();
    }

    public void DisplayToggle()
    {
        toggleElement.isOn = ToggleToDisplay.Toggle;
    }

    public void ChangeToggle()
    {
        ToggleToDisplay.SetValue(toggleElement.isOn);
    }
}
