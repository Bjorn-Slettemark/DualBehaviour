[System.Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;
    public FloatVariable Variable;

    public FloatReference(float value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }

    public void SetValue(float value)
    {
        if (UseConstant)
        {
            ConstantValue = value;
        }
        else
        {
            Variable.SetValue(value);
        }
    }

    public void ApplyChange(float amount)
    {
        if (UseConstant)
        {
            ConstantValue += amount;
        }
        else
        {
            Variable.ApplyChange(amount);
        }
    }
}
