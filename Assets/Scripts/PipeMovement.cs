using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float destroyPosition = -12f;

    private bool isMoving = false;

    void Update()
    {
        if (!isMoving) return;

        // Gerakkan pipa ke kiri
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Hancurkan jika sudah melewati batas
        if (transform.position.x < destroyPosition)
        {
            Destroy(gameObject);
        }
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}