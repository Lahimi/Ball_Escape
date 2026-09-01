using System.Collections;
using UnityEngine;
using TMPro;

public class S11_GameTimer : MonoBehaviour
{
    public static S11_GameTimer Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI penaltyText;
    [SerializeField] private RectTransform   penaltyRect;

    [Header("Settings")]
    public float penaltySeconds    = 10f;   // how much time is added per catch
    public float popupFloatAmount  = 80f;   // how far the +10 floats upward (units)
    public float popupDuration     = 1.5f;  // seconds the popup takes to fade out

    // ── State ────────────────────────────────────────────────────
    public  float ElapsedTime  { get; private set; }
    public  bool  IsRunning    { get; private set; }

    private Vector2 penaltyStartPos;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (penaltyRect != null)
            penaltyStartPos = penaltyRect.anchoredPosition;

        // Hide penalty text on start
        SetPenaltyAlpha(0f);

        StartTimer();
    }

    private void Update()
    {
        if (!IsRunning) return;

        ElapsedTime += Time.deltaTime;
        UpdateTimerDisplay();
    }

    // ── Public API ───────────────────────────────────────────────

    public void StartTimer()
    {
        IsRunning = true;
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        ElapsedTime = 0f;
        UpdateTimerDisplay();
    }

    // Call this when an enemy catches the player
    public void AddPenalty()
    {
        ElapsedTime += penaltySeconds;
        StopAllCoroutines();
        StartCoroutine(ShowPenaltyPopup());
    }

    // ── Display ──────────────────────────────────────────────────

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int   minutes  = (int)(ElapsedTime / 60f);
        int   seconds  = (int)(ElapsedTime % 60f);
        float fraction = ElapsedTime % 1f;
        int   tenths   = (int)(fraction * 10f);

        timerText.text = $"{minutes:00}:{seconds:00}.{tenths}";
    }

    // ── Penalty popup animation ──────────────────────────────────

    private IEnumerator ShowPenaltyPopup()
    {
        if (penaltyText == null || penaltyRect == null) yield break;

        penaltyText.text = $"+{penaltySeconds:0}s";

        // Reset position and make visible
        penaltyRect.anchoredPosition = penaltyStartPos;
        SetPenaltyAlpha(1f);

        float elapsed = 0f;
        while (elapsed < popupDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popupDuration;

            // Float upward
            penaltyRect.anchoredPosition = penaltyStartPos + Vector2.up * (popupFloatAmount * t);

            // Fade out in the second half
            float alpha = t < 0.4f ? 1f : Mathf.Lerp(1f, 0f, (t - 0.4f) / 0.6f);
            SetPenaltyAlpha(alpha);

            yield return null;
        }

        SetPenaltyAlpha(0f);
        penaltyRect.anchoredPosition = penaltyStartPos;
    }

    private void SetPenaltyAlpha(float alpha)
    {
        if (penaltyText == null) return;
        var c   = penaltyText.color;
        c.a     = alpha;
        penaltyText.color = c;
    }

    public float GetFinalTime()
    {
        // Return the current time variable you are using in this script
        // (Usually called 'currentTime' or 'elapsedTime')
        return ElapsedTime; 
    }

}
