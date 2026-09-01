using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S20_NegativeFlicker : MonoBehaviour
{
    private Image uiImage;

    [Header("Sprites")]
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private Sprite negativeSprite;

    [Header("Settings")]
    [SerializeField] private float flashInterval = 0.05f;

    void Awake()
    {
        uiImage = GetComponent<Image>();
    }

    void OnEnable()
    {
        StartCoroutine(NegativeFlashRoutine());
    }

    IEnumerator NegativeFlashRoutine()
    {
        bool isOriginal = true;

        while (true)
        {
            // Swap the sprite based on the current state
            uiImage.sprite = isOriginal ? negativeSprite : originalSprite;
            
            // Flip the state for the next loop
            isOriginal = !isOriginal;

            // Randomized wait time for a "glitchy" feel
            yield return new WaitForSeconds(Random.Range(flashInterval, flashInterval * 2f));
        }
    }
}