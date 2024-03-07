using UnityEngine;

[CreateAssetMenu(fileName = "New FloatVariable", menuName = "Variables/FloatVariable")]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public float Value;

    public void SetValue(float value)
    {
        Value = value;
    }

    public void ApplyChange(float amount)
    {
        Value += amount;
    }
}
