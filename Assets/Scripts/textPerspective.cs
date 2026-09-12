using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class textPerspective : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Assign your TextMeshPro component in the Inspector
    public float startingFontSize = 36f; // The size of the first character
    public float sizeDecrement = 2f; // How much smaller each character will be compared to the previous one

    // Called whenever a value is changed in the Inspector
    void OnValidate()
    {
        if (textMeshPro != null)
        {
            ApplyIncrementalFontSize();
        }
    }

    // Called in both edit and play modes to update the text
    public void ApplyIncrementalFontSize()
    {
        string originalText = textMeshPro.text;
        string newText = "";
        float currentFontSize = startingFontSize;

        // Loop through each character in the original text
        for (int i = 0; i < originalText.Length; i++)
        {
            // Add the character with the size tag
            newText += $"<size={currentFontSize}>{originalText[i]}</size>";
            currentFontSize = Mathf.Max(currentFontSize - sizeDecrement, 1f); // Ensure the font size doesn't go below 1
        }

        //textMeshPro.richText = true;

        // Apply the new text with size tags
        textMeshPro.text = newText;
    }
}

#if UNITY_EDITOR
// Custom editor to update in Scene View and Inspector automatically
[CustomEditor(typeof(textPerspective))]
public class IncrementalTextSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Update the text in Scene View/Inspector when changing properties
        textPerspective script = (textPerspective)target;
        if (GUILayout.Button("Apply Incremental Font Size"))
        {
            script.ApplyIncrementalFontSize();
        }
    }
}
#endif