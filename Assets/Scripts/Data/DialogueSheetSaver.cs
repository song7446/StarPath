using System.Collections.Generic;
using SongLib;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

public class DialogueSheetSaver : ISheetJsonParser
{ 
    public string Parse(string csv)
    {
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

        return JsonUtility.ToJson(
            new DialogueDatabase { dialogues = list.ToArray() },
            true
        );
    }
}
