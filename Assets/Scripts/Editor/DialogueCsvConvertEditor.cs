using System.IO;
using SongLib;
using UnityEditor;
using UnityEngine;

public static class DialogueCsvConvertEditor
{
    [MenuItem("Tools/CSV To Json/Dialogue")]
    public static void ConvertDialogueData()
    {
        string csvPath = "Assets/Sheets/DialogueData.csv";
        string jsonPath = "Assets/Json/DialogueData.json";

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

    [MenuItem("Tools/CSV To Json/Constellation")]
    public static void ConvertConstellationData()
    {
        string csvPath = "Assets/Sheets/ConstellationData.csv";
        string jsonPath = "Assets/Json/ConstellationData.json";

        if (!File.Exists(csvPath))
        {
            Debug.LogError("❌ CSV file not found");
            return;
        }

        // 1️⃣ CSV 읽기
        string csv = File.ReadAllText(csvPath);

        // 2️⃣ 파서 생성
        ISheetJsonParser parser = new ConstellationSheetSaver();

        // 3️⃣ CSV → JSON
        string json = parser.Parse(csv);

        // 4️⃣ JSON 저장 (라이브러리 유틸 사용)
        SheetJsonSaver.Save(json, jsonPath);
    }
}