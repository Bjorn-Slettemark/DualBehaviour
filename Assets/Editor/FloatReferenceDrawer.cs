using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer
{
    // Define the names of your fields here
    private const string useConstantPropertyName = "UseConstant";
    private const string constantValuePropertyName = "ConstantValue";
    private const string variablePropertyName = "Variable";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, label);

        SerializedProperty useConstantProp = property.FindPropertyRelative(useConstantPropertyName);
        SerializedProperty constantValueProp = property.FindPropertyRelative(constantValuePropertyName);
        SerializedProperty variableProp = property.FindPropertyRelative(variablePropertyName);

        // Calculate rect for configuration button
        Rect buttonRect = position;
        buttonRect.width = EditorGUIUtility.singleLineHeight;

        // Draw button to select constant or variable
        useConstantProp.boolValue = EditorGUI.Toggle(buttonRect, useConstantProp.boolValue);

        // Adjust rect for value field
        Rect valueRect = position;
        valueRect.x += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        valueRect.width -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw the appropriate field based on the current toggle state
        if (useConstantProp.boolValue)
        {
            EditorGUI.PropertyField(valueRect, constantValueProp, GUIContent.none);
        }
        else
        {
            EditorGUI.PropertyField(valueRect, variableProp, GUIContent.none);
        }

        EditorGUI.EndProperty();
    }
}
