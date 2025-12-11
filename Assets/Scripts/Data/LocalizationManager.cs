using UnityEngine;

public enum Language
{
    Korean,
    English,
    Japanese
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public Language currentLanguage = Language.Korean;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string GetLocalizedText(DialogueEntry entry)
    {
        return currentLanguage switch
        {
            Language.Korean => entry.Text_ko,
            Language.English => entry.Text_en,
            Language.Japanese => entry.Text_jp,
            _ => entry.Text_ko
        };
    }
}