using UnityEditor;
using UnityEngine;
using TMPro;

public class TMPFontEditorTool : EditorWindow
{
    public TMP_FontAsset newFont;

    [MenuItem("Tools/Replace TMP Font in Scene")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontEditorTool>("Replace TMP Font");
    }

    void OnGUI()
    {
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts in Scene"))
        {
            if (newFont == null)
            {
                Debug.LogWarning("Assign a TMP Font Asset first.");
                return;
            }

            ReplaceFontsInScene(newFont);
        }
    }

    static void ReplaceFontsInScene(TMP_FontAsset font)
    {
        TMP_Text[] texts = GameObject.FindObjectsOfType<TMP_Text>(true);
        foreach (var text in texts)
        {
            Undo.RecordObject(text, "Change TMP Font");
            text.font = font;
            EditorUtility.SetDirty(text);
        }

        Debug.Log($"Replaced fonts on {texts.Length} TMP_Text components.");
    }
}
