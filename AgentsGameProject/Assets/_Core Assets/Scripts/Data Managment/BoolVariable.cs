using UnityEngine;


[CreateAssetMenu]
public class BoolVariable : ScriptableObject
{
    public bool Toggle;

    public void SetValue(bool toggle)
    {
        Toggle = toggle;
    }

    public void SetValue(BoolVariable toggle)
    {
        Toggle = toggle.Toggle;
    }
}
