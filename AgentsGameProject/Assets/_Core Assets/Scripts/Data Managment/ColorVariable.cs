using UnityEngine;

[CreateAssetMenu]
public class ColorVariable : ScriptableObject
{
    public Color ColorValue;

    public void SetColor(Color color)
    {
        ColorValue = color;
    }

    public void SetColor(ColorVariable color)
    {
        ColorValue = color.ColorValue;
    }
}
