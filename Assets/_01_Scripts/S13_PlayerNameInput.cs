using UnityEngine;
using TMPro;

public class S13_PlayerNameInput : MonoBehaviour
{
    // ── Keys ──────────────────────────────────────────────────────
    const string KEY_NAME = "PlayerName";
    const string KEY_HIGHSCORE = "HighScore";
    const string KEY_VOLUME = "MasterVolume";

    // ── Static Properties (Access these from any script) ──────────
    public static string PlayerName { get; private set; } = "Player";
    public static int HighScore { get; private set; } = 0;
    public static float MasterVolume { get; private set; } = 1.0f;

    [Header("Panels")]
    [SerializeField] private GameObject namePanel;
    [SerializeField] private GameObject[] menuObjects;

    [Header("References")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameDisplay;

    private void Start()
    {
        LoadAllData();
        
        if (namePanel != null) namePanel.SetActive(false);
        if (nameDisplay != null) nameDisplay.text = PlayerName;
    }

    // ── Loading Logic ─────────────────────────────────────────────
    private void LoadAllData()
    {
        PlayerName = PlayerPrefs.GetString(KEY_NAME, "Player");
        HighScore = PlayerPrefs.GetInt(KEY_HIGHSCORE, 0);
        
        Debug.Log($"Loaded: {PlayerName} | HighScore: {HighScore}");
    }

    // ── Saving Logic ──────────────────────────────────────────────
    public void OnNameSubmitted(string value)
    {
        // If the value passed is empty, try to grab it directly from the input box as a backup
        if (string.IsNullOrEmpty(value) && nameInput != null)
        {
            value = nameInput.text;
        }

        string trimmed = value.Trim();
        
        // If it's STILL empty, don't allow it to be blank
        if (string.IsNullOrEmpty(trimmed)) 
        {
            trimmed = "Player"; 
        }

        PlayerName = trimmed;
        PlayerPrefs.SetString(KEY_NAME, PlayerName);
        PlayerPrefs.Save();

        if (nameDisplay != null) nameDisplay.text = PlayerName;
        
        Debug.Log($"[S13] Name Submitted & Saved: {PlayerName}");
        HideNamePanel();
    }

    public static void SaveHighScore(int newScore)
    {
        if (newScore > HighScore)
        {
            HighScore = newScore;
            PlayerPrefs.SetInt(KEY_HIGHSCORE, HighScore);
            PlayerPrefs.Save();
        }
    }

    public static void SaveVolume(float vol)
    {
        MasterVolume = Mathf.Clamp01(vol);
        PlayerPrefs.SetFloat(KEY_VOLUME, MasterVolume);
        PlayerPrefs.Save();
    }

    // ── Panel Management ──────────────────────────────────────────
    public void ShowNamePanel()
    {
        SetMenuVisible(false);
        if (nameInput != null) nameInput.text = PlayerName;
        if (namePanel != null) namePanel.SetActive(true);
    }

    public void HideNamePanel()
    {
        if (namePanel != null) namePanel.SetActive(false);
        SetMenuVisible(true);
    }

    private void SetMenuVisible(bool visible)
    {
        foreach (var obj in menuObjects)
            if (obj != null) obj.SetActive(visible);
    }
}