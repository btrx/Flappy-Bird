using UnityEngine;
using System.Collections.Generic;

public class PipeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnXPosition = 10f;

    [Header("Randomization")]
    [SerializeField] private float minHeight = -2f;
    [SerializeField] private float maxHeight = 2f;
    [SerializeField] private float minGap = 3f;
    [SerializeField] private float maxGap = 4.5f;

    [Header("Difficulty")]
    [SerializeField] private bool increaseDifficulty = true;
    [SerializeField] private float difficultyIncreaseRate = 0.1f;
    [SerializeField] private float minGapLimit = 2.5f;

    private float timer = 0f;
    private bool isSpawning = false;
    private List<GameObject> activePipes = new List<GameObject>();
    private float currentGap;

    void Start()
    {
        currentGap = maxGap;
    }

    void Update()
    {
        if (!isSpawning) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPipe();
            timer = 0f;

            // Increase difficulty over time
            if (increaseDifficulty && currentGap > minGapLimit)
            {
                currentGap -= difficultyIncreaseRate;
            }
        }
    }

    void SpawnPipe()
    {
        // Posisi Y acak
        float randomY = Random.Range(minHeight, maxHeight);
        Vector3 spawnPos = new Vector3(spawnXPosition, randomY, 0);

        // Spawn pipa
        GameObject newPipe = Instantiate(pipePrefab, spawnPos, Quaternion.identity);
        activePipes.Add(newPipe);

        // Start movement
        PipeMovement movement = newPipe.GetComponent<PipeMovement>();
        if (movement != null)
        {
            movement.StartMoving();
        }

        // Atur gap acak antara pipa atas dan bawah
        float gap = Mathf.Max(currentGap, minGapLimit);
        Transform pipeTop = newPipe.transform.Find("PipeTop");
        Transform pipeBottom = newPipe.transform.Find("PipeBottom");

        if (pipeTop != null && pipeBottom != null)
        {
            pipeTop.localPosition = new Vector3(0, gap / 2 + 2.5f, 0);
            pipeBottom.localPosition = new Vector3(0, -gap / 2 - 2.5f, 0);
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        timer = spawnInterval; // Spawn immediately
        currentGap = maxGap; // Reset difficulty
    }

    public void StopSpawning()
    {
        isSpawning = false;

        // Stop all pipes movement
        foreach (GameObject pipe in activePipes)
        {
            if (pipe != null)
            {
                PipeMovement movement = pipe.GetComponent<PipeMovement>();
                if (movement != null)
                {
                    movement.StopMoving();
                }
            }
        }
    }

    public void ClearAllPipes()
    {
        // Hancurkan semua pipa yang ada
        foreach (GameObject pipe in activePipes)
        {
            if (pipe != null)
            {
                Destroy(pipe);
            }
        }
        activePipes.Clear();
    }
}