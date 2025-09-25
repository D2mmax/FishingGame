using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingHook : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public GameManager gameManager;
    public Transform fishAnchorPoint; // Empty GameObject on hook

    [Header("Hook Settings")]
    public float dropSpeed = 5f;
    public float reelSpeed = 8f;
    public float maxDepth = -20f;
    public float maxWeight = 2f;
    public float fishStackOffsetY = 0.5f; // Vertical spacing between stacked fish

    private InputSystem_Actions input;
    private bool isFishing = false;
    private bool isReeling = false;
    private Vector3 startPos;
    private float currentWeight = 0f;

    private List<GameObject> attachedFish = new List<GameObject>();

    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!gameManager.GameStarted) return;

        if (!isFishing)
        {
            isFishing = true;
            isReeling = false;
        }

        if (isFishing && !isReeling)
        {
            DropHook();

            // Always move attached fish while dropped
            UpdateAttachedFishPositions();

            if (Released())
                isReeling = true;
        }
        else if (isReeling)
        {
            ReelUp();
            UpdateAttachedFishPositions();
        }

        FollowCamera();
    }

    void DropHook()
    {
        if (transform.position.y > maxDepth)
            transform.Translate(Vector2.down * dropSpeed * Time.deltaTime);

        if (Held())
        {
            Vector2 screenPos = input.Player.Point.ReadValue<Vector2>();
            Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);

            Vector3 newPos = transform.position;
            newPos.x = worldPos.x;

            float halfWidth = cam.orthographicSize * cam.aspect;
            float leftBound = cam.transform.position.x - halfWidth;
            float rightBound = cam.transform.position.x + halfWidth;
            newPos.x = Mathf.Clamp(newPos.x, leftBound, rightBound);

            transform.position = newPos;
        }
    }

    void ReelUp()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPos, reelSpeed * Time.deltaTime);

        if (transform.position == startPos)
        {
            // "Catch" all fish
            foreach (var fish in attachedFish)
            {
                if (fish != null)
                {
                    gameManager.CatchFish(fish);
                    Destroy(fish); // optional: remove from scene
                }
            }

            attachedFish.Clear();
            isFishing = false;
            isReeling = false;
            currentWeight = 0f;

            gameManager.ResetGame();
        }
    }

    void FollowCamera()
    {
        Vector3 camPos = cam.transform.position;
        camPos.y = transform.position.y;
        cam.transform.position = camPos;
    }

    // Input helpers
    bool Held() => input.Player.Click.IsPressed();
    bool Released() => input.Player.Click.WasReleasedThisFrame();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish") && !attachedFish.Contains(collision.gameObject))
        {
            attachedFish.Add(collision.gameObject);

            // Disable movement while attached
            var movement = collision.GetComponent<FishMovement>();
            if (movement != null)
                movement.enabled = false;

            // Immediately snap fish to anchor position
            UpdateAttachedFishPositions();
        }
    }

    // Moves all attached fish to their stacked positions above the anchor
    private void UpdateAttachedFishPositions()
    {
        for (int i = 0; i < attachedFish.Count; i++)
        {
            GameObject fish = attachedFish[i];
            if (fish != null)
            {
                Vector3 targetPos = fishAnchorPoint.position + Vector3.up * fishStackOffsetY * i;
                fish.transform.position = targetPos;
            }
        }
    }
}
