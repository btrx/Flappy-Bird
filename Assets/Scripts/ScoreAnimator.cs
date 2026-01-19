using UnityEngine;
using TMPro;

public class ScoreAnimator : MonoBehaviour
{
    [SerializeField] private float punchScale = 1.2f;
    [SerializeField] private float animationDuration = 0.2f;

    private TextMeshProUGUI scoreText;
    private Vector3 originalScale;
    private float animationTimer = 0f;
    private bool isAnimating = false;

    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        originalScale = transform.localScale;

        // Subscribe to score change event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged.AddListener(AnimateScore);
        }
    }

    void Update()
    {
        if (!isAnimating) return;

        animationTimer += Time.deltaTime;
        float progress = animationTimer / animationDuration;

        if (progress >= 1f)
        {
            transform.localScale = originalScale;
            isAnimating = false;
            return;
        }

        // Punch scale animation (scale up then down)
        float scale = progress < 0.5f
            ? Mathf.Lerp(1f, punchScale, progress * 2f)
            : Mathf.Lerp(punchScale, 1f, (progress - 0.5f) * 2f);

        transform.localScale = originalScale * scale;
    }

    void AnimateScore()
    {
        animationTimer = 0f;
        isAnimating = true;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged.RemoveListener(AnimateScore);
        }
    }
}