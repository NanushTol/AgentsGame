using UnityEngine;


[CreateAssetMenu]
public class StringVariable : ScriptableObject
{
    public string Value;

    public void SetValue(string value)
    {
        Value = value;
    }

    public void SetValue(StringVariable value)
    {
        Value = value.Value;
    }

    public void AddCharacters(string amount)
    {
        Value += amount;
    }

    public void AddCharacters(StringVariable amount)
    {
        Value += amount.Value;
    }
}
