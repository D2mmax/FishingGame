using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;          // Movement speed
    public float moveDistance = 3f;   // How far to move left/right from start

    private Vector3 startPosition;
    private int direction = 1;        // 1 = right, -1 = left

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Move the fish
        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        // Check if we've reached the bounds
        if (transform.position.x > startPosition.x + moveDistance)
        {
            direction = -1;
            FlipFish();
        }
        else if (transform.position.x < startPosition.x - moveDistance)
        {
            direction = 1;
            FlipFish();
        }
    }

    void FlipFish()
    {
        // Flip the sprite horizontally
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
