using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ── 1 & 2. SERIALIZABLE DATA CLASSES ──────────────────────────────
[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float time;
}

[Serializable]
public class LeaderboardData
{
    // JsonUtility requires a wrapper class to handle lists/arrays
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class S14_LeaderboardUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject[] menuObjects;

    [Header("Leaderboard UI References")]
    [SerializeField] private Transform entryContainer; 
    [SerializeField] private GameObject entryTemplate; 

    // Helper to get the save path
    private string FilePath => Application.persistentDataPath + "/leaderboard.json";

    private void Start()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        // Ensure the template itself is hidden in the UI
        if (entryTemplate != null)
            entryTemplate.SetActive(false);
    }

    // ── 3. SAVE LOGIC (Call this when Room 5 is cleared) ──────────

    public void AddEntryAndSave(string name, float finalTime)
    {
        LeaderboardData data = LoadData();

        data.entries.Add(new LeaderboardEntry { playerName = name, time = finalTime });

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(FilePath, json);
        Debug.Log($"[S14] Entry Saved! Path: {FilePath}");
    }

    private LeaderboardData LoadData()
    {

        if (!File.Exists(FilePath))
        {
            Debug.Log("[S14] No save file found, creating fresh data.");
            return new LeaderboardData();
        }

        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<LeaderboardData>(json);
    }

    // ── 4. POPULATE UI (Read, Sort, and Display) ──────────────────
    public void PopulateEntries()
    {
        // 1. Cleanup existing rows
        foreach (Transform child in entryContainer)
        {
            if (child.gameObject != entryTemplate) Destroy(child.gameObject);
        }

        // 2. Load and Sort Data
        LeaderboardData data = LoadData();
        // Sort by time (ascending) so the fastest is first
        var sortedList = data.entries.OrderBy(e => e.time).ToList();

        // 3. Instantiate and Inject
        for (int i = 0; i < sortedList.Count; i++)
        {
            if (entryTemplate == null) break;

            GameObject newRow = Instantiate(entryTemplate, entryContainer);
            newRow.SetActive(true);

            var entry = sortedList[i];

            // Use GetComponentsInChildren to find ALL text components in this row
            TMP_Text[] allTextComponents = newRow.GetComponentsInChildren<TMP_Text>();

            foreach (TMP_Text txt in allTextComponents)
            {
                // Check the name of the GameObject the text is attached to
                if (txt.gameObject.name == "TXT_Rank") 
                    txt.text = (i + 1).ToString();
                
                else if (txt.gameObject.name == "TXT_EntryName") 
                    txt.text = entry.playerName;
                
                else if (txt.gameObject.name == "TXT_EntryTime") 
                    txt.text = FormatTime(entry.time);
            }
        }
    }

    private string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60);
        int seconds = Mathf.FloorToInt(t % 60);
        int fraction = Mathf.FloorToInt((t * 100) % 100);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }

    // ── Navigation ────────────────────────────────────────────────
    public void ShowLeaderboard()
    {
        SetMenuVisible(false);
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            PopulateEntries(); // Fill the list as soon as it opens
        }
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        SetMenuVisible(true);
    }

    private void SetMenuVisible(bool visible)
    {
        foreach (var obj in menuObjects)
            if (obj != null) obj.SetActive(visible);
    }

    private void Awake()
    {
        // Add this so the leaderboard is still there when you finish Room 5
        DontDestroyOnLoad(gameObject);
    }
}