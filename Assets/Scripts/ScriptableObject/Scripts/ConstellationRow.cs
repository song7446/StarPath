[System.Serializable]
public class ConstellationRow
{
    public string Id;
    public string Name_Kr;
    public string Explain_Kr;
    public string Name_En;
    public string Explain_En;
}

[System.Serializable]
public class ConstellationDatabase
{
    public ConstellationRow[] Constellations;
}