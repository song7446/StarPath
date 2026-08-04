using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CharacterExtractor : EditorWindow
{
    private string jsonPath = "Assets/Json/dialogue.json";
    private string outputPath = "Assets/Json/dialogue_characters.txt";

    [MenuItem("Tools/Dialogue/Extract Characters")]
    public static void ShowWindow()
    {
        GetWindow<CharacterExtractor>(
            "Character Extractor"
        );
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Character Extractor",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        jsonPath = EditorGUILayout.TextField(
            "JSON Path",
            jsonPath
        );

        outputPath = EditorGUILayout.TextField(
            "Output Path",
            outputPath
        );

        EditorGUILayout.Space();

        if (GUILayout.Button(
            "Extract Characters",
            GUILayout.Height(35)))
        {
            ExtractCharacters();
        }
    }

    private void ExtractCharacters()
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogError(
                $"JSON 파일을 찾을 수 없습니다: {jsonPath}"
            );
            return;
        }

        string json = File.ReadAllText(
            jsonPath,
            Encoding.UTF8
        );

        HashSet<char> characters =
            new HashSet<char>();

        foreach (char character in json)
        {
            characters.Add(character);
        }

        List<char> sortedCharacters =
            new List<char>(characters);

        sortedCharacters.Sort();

        StringBuilder result =
            new StringBuilder();

        foreach (char character in sortedCharacters)
        {
            result.Append(character);
        }

        File.WriteAllText(
            outputPath,
            result.ToString(),
            new UTF8Encoding(false)
        );

        AssetDatabase.Refresh();

        Debug.Log(
            $"<color=green>" +
            $"문자 추출 완료!</color>\n" +
            $"고유 문자 수: {characters.Count}\n" +
            $"저장 위치: {outputPath}"
        );
    }
}