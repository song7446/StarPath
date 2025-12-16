using System.IO;
using SongLib;
using UnityEditor;
using UnityEngine;

public static class DialogueCsvConvertEditor
{
    [MenuItem("Tools/Dialogue/Convert CSV To JSON")]
    public static void Convert()
    {
        string csvPath = "Assets/Sheets/dialogue.csv";
        string jsonPath = "Assets/Json/Dialogue/dialogue.json";

        if (!File.Exists(csvPath))
        {
            Debug.LogError("❌ CSV file not found");
            return;
        }

        // 1️⃣ CSV 읽기
        string csv = File.ReadAllText(csvPath);

        // 2️⃣ 파서 생성
        ISheetJsonParser parser = new DialogueSheetSaver();

        // 3️⃣ CSV → JSON
        string json = parser.Parse(csv);

        // 4️⃣ JSON 저장 (라이브러리 유틸 사용)
        SheetJsonSaver.Save(json, jsonPath);
    }
}