using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public static class DialogueSheetImporter
{
    private static readonly string SheetUrl = GoogleSheet.GoogleSheetURL;

    [MenuItem("Tools/Dialogue/Import From Google Sheet")]
    public static void ImportDialogueSheet()
    {
        string savePath = "Assets/Data/dialogue.json";

        try
        {
            using WebClient client = new WebClient();
            string csv = client.DownloadString(SheetUrl);
            string json = CsvToJson(csv);
            File.WriteAllText(savePath, json);
            Debug.Log($"✅ Dialogue data imported successfully → {savePath}");
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Failed to import Google Sheet: " + e.Message);
        }
    }

    private static string CsvToJson(string csv)
    {
        var lines = csv.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2) return "{}";

        string[] headers = ParseCsvLine(lines[0]);
        var json = new System.Text.StringBuilder();
        json.Append("{\"entries\":[");

        int count = 0;
        for (int i = 1; i < lines.Length; i++)
        {
            var values = ParseCsvLine(lines[i]);
            if (values.Length < headers.Length) continue;

            if (count > 0) json.Append(",");

            json.Append("{");
            for (int j = 0; j < headers.Length; j++)
            {
                string key = headers[j].Trim();
                string value = values[j].Trim().Replace("\"", "\\\"");
                json.Append($"\"{key}\":\"{value}\"");
                if (j < headers.Length - 1) json.Append(",");
            }
            json.Append("}");
            count++;
        }

        json.Append("]}");
        return json.ToString();
    }

    private static string[] ParseCsvLine(string line)
    {
        var values = new List<string>();
        bool inQuotes = false;
        var value = new System.Text.StringBuilder();

        foreach (char c in line)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                values.Add(value.ToString());
                value.Clear();
            }
            else
            {
                value.Append(c);
            }
        }

        values.Add(value.ToString());
        return values.ToArray();
    }

}