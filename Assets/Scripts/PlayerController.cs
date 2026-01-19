using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class BirdController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Boundaries")]
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private float minHeight = -5f;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private bool isAlive = true;
    private bool gameStarted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();

        // Get Jump action dari Input Actions
        jumpAction = playerInput.actions["Jump"];
    }

    void OnEnable()
    {
        // Subscribe ke event saat jump action dipanggil
        jumpAction.performed += OnJump;
    }

    void OnDisable()
    {
        // Unsubscribe untuk mencegah memory leak
        jumpAction.performed -= OnJump;
    }

    void Start()
    {
        // Freeze bird sampai game dimulai
        rb.simulated = false;
    }

    void Update()
    {
        if (!isAlive || !gameStarted) return;

        // Cek batas area
        CheckBoundaries();

        // Rotasi bird berdasarkan velocity
        RotateBird();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Jika game belum mulai, start game
        if (!gameStarted)
        {
            StartGame();
            return;
        }

        // Jika sudah mati, jangan bisa jump
        if (!isAlive) return;

        Jump();
    }

    void StartGame()
    {
        gameStarted = true;
        rb.simulated = true;
        GameManager.Instance.StartGame();
        Jump(); // Jump pertama untuk mulai
    }

    void Jump()
    {
        // Reset velocity dan tambahkan force ke atas
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Play sound effect
        if (audioSource && jumpSound)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    void CheckBoundaries()
    {
        Vector3 pos = transform.position;

        // Cek batas atas dan bawah
        if (pos.y > maxHeight || pos.y < minHeight)
        {
            GameOver();
        }
    }

    void RotateBird()
    {
        // Rotasi bird berdasarkan kecepatan vertikal
        // Naik = rotasi ke atas, turun = rotasi ke bawah
        float targetRotation = Mathf.Lerp(-30, 90, (-rb.linearVelocity.y + 10f) / 20f);
        float currentRotation = transform.rotation.eulerAngles.z;

        // Smooth rotation
        float newRotation = Mathf.LerpAngle(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            GameOver();
        }
        else if (collision.CompareTag("ScoreZone"))
        {
            // Tambah skor
            GameManager.Instance.AddScore();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Tabrakan dengan apapun = game over
        GameOver();
    }

    void GameOver()
    {
        if (!isAlive) return;

        isAlive = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // Disable input
        playerInput.enabled = false;

        GameManager.Instance.GameOver();
    }

    public void ResetBird()
    {
        isAlive = true;
        gameStarted = false;
        transform.position = new Vector3(-3, 0, 0);
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // Re-enable input
        playerInput.enabled = true;
    }
}