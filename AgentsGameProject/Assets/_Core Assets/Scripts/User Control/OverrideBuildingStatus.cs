using UnityEngine;
using UnityEngine.UI;


public class OverrideBuildingStatus : MonoBehaviour
{
    SelectObject selectedObject;

    Toggle toggleElement;

    DisplayBoolVariable dispBool;

    void Awake()
    {
        selectedObject = GameObject.Find("UserControls").GetComponent<SelectObject>();
        toggleElement = this.GetComponent<Toggle>();
        dispBool = this.GetComponent<DisplayBoolVariable>();
    }

    public void OverrideBuildingStatusToggle()
    {
        dispBool.ToggleToDisplay.SetValue(toggleElement.isOn);
        selectedObject.SelectedObject.GetComponent<GenericBuilding>().UserOverrideBuildingActive = toggleElement.isOn;
    }
}
