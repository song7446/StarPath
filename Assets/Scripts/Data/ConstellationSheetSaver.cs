using System.Collections.Generic;
using SongLib;
using UnityEngine;

public class ConstellationSheetSaver : ISheetJsonParser
{ 
    public string Parse(string csv)
    {
        csv = csv.Replace("\uFEFF", "");

        var lines = csv.Split('\n');
        var list = new List<ConstellationRow>();

        for (int i = 1; i < lines.Length; i++) // 헤더 스킵
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var cols = line.Split(',');

            if (cols.Length < 4)
                continue;

            list.Add(new ConstellationRow
            {
                Id = cols[0].Trim(),
                Name_Kr = cols[1].Trim(),
                Explain_Kr = cols[2].Trim(),
                Name_En = cols[3].Trim(),
                Explain_En = cols[4].Trim()
            });
        }

        Debug.Log($"✅ Parsed Constellation Count: {list.Count}");

        return JsonUtility.ToJson(
            new ConstellationDatabase() { Constellations = list.ToArray() },
            true
        );
    }
}