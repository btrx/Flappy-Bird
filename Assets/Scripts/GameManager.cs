using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Game References")]
    [SerializeField] private BirdController bird;
    [SerializeField] private PipeSpawner pipeSpawner;

    [Header("Audio")]
    [SerializeField] private AudioClip scoreSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Events")]
    public UnityEvent OnGameStart;
    public UnityEvent OnGameOver;
    public UnityEvent OnScoreChanged;

    private AudioSource audioSource;
    private int score = 0;
    private int highScore = 0;
    private bool gameStarted = false;
    private bool isGameOver = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Start()
    {
        // Initialize game state
        ResetGame();
        ShowStartScreen();
    }

    void ShowStartScreen()
    {
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        infoPanel.SetActive(false);

        // Update instruction text based on platform
        if (Application.isMobilePlatform)
        {
            instructionText.text = "Tap to Start";
        }
        else
        {
            instructionText.text = "Click or Press Space to Start";
        }
    }

    public void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;
        isGameOver = false;
        startPanel.SetActive(false);

        // Start spawning pipes
        pipeSpawner.StartSpawning();

        // Invoke event
        OnGameStart?.Invoke();
    }

    public void AddScore()
    {
        if (!gameStarted || isGameOver) return;

        score++;
        UpdateScoreUI();

        // Play score sound
        if (scoreSound && audioSource)
        {
            audioSource.PlayOneShot(scoreSound);
        }

        // Invoke event
        OnScoreChanged?.Invoke();
    }

    void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameStarted = false;

        // Stop spawning
        pipeSpawner.StopSpawning();

        // Play game over sound
        if (gameOverSound && audioSource)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Update high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        // Show game over panel after short delay
        Invoke(nameof(ShowGameOverPanel), 0.5f);

        // Invoke event
        OnGameOver?.Invoke();
    }

    void ShowGameOverPanel()
    {
        finalScoreText.text = "Score: " + score;
        highScoreText.text = "Best: " + highScore;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ResetGame()
    {
        score = 0;
        gameStarted = false;
        isGameOver = false;
        UpdateScoreUI();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowInfo()
    {
        infoPanel.SetActive(true);
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
