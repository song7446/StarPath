using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetLoader : EditorWindow
{
    private UnityWebRequest _request;

    private const string SaveDir = "Assets/Json/Dialogue";
    private const string SavePath = SaveDir + "/dialogue.json";

    [MenuItem("Tools/Google Sheet/Load Dialogue & Save JSON")]
    public static void Open()
    {
        GetWindow<GoogleSheetLoader>("Google Sheet Loader");
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet → Dialogue JSON", EditorStyles.boldLabel);

        EditorGUILayout.TextArea(
            GoogleSheetURL.DialogueURL,
            GUILayout.Height(60)
        );

        GUILayout.Space(10);

        if (_request == null)
        {
            if (GUILayout.Button("Load CSV & Save JSON"))
            {
                StartRequest();
            }
        }
        else
        {
            GUILayout.Label("Loading...");
        }
    }

    private void StartRequest()
    {
        _request = UnityWebRequest.Get(GoogleSheetURL.DialogueURL);
        _request.SendWebRequest();

        EditorApplication.update += OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (!_request.isDone)
            return;

        EditorApplication.update -= OnEditorUpdate;

        if (_request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ CSV Load Failed: {_request.error}");
        }
        else
        {
            ProcessCSV(_request.downloadHandler.text);
        }

        _request.Dispose();
        _request = null;
    }

    private void ProcessCSV(string csv)
    {
        // 🔴 BOM 제거 (중요)
        csv = csv.Replace("\uFEFF", "");

        var lines = csv.Split('\n');
        var list = new List<DialogueRow>();

        for (int i = 1; i < lines.Length; i++) // 헤더 스킵
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var cols = line.Split(',');

            if (cols.Length < 4)
                continue;

            list.Add(new DialogueRow
            {
                id = cols[0].Trim(),
                speaker = cols[1].Trim(),
                textKo = cols[2].Trim(),
                textEn = cols[3].Trim()
            });
        }

        Debug.Log($"✅ Parsed Dialogue Count: {list.Count}");

        SaveJson(new DialogueDatabase
        {
            dialogues = list.ToArray()
        });
    }


    private void SaveJson(DialogueDatabase db)
    {
        if (!Directory.Exists(SaveDir))
            Directory.CreateDirectory(SaveDir);

        string json = JsonUtility.ToJson(db, true);
        File.WriteAllText(SavePath, json);

        AssetDatabase.Refresh();

        Debug.Log($"✅ Dialogue JSON saved: {SavePath}");
    }
}
