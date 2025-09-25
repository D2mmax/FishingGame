using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private InputSystem_Actions input;
    private bool gameStarted = false;
    public bool GameStarted => gameStarted;

    // Optional: keep a list of caught fish
    private List<GameObject> caughtFish = new List<GameObject>();

    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Click.performed += OnClick;
    }

    void OnDisable()
    {
        input.Player.Click.performed -= OnClick;
        input.Disable();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (gameStarted) return;

        Vector2 screenPos = input.Player.Point.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null && hit.gameObject == gameObject)
        {
            gameStarted = true;
            Debug.Log("Fishing Started!");
        }
    }

    public void ResetGame()
    {
        gameStarted = false; // wait for next click
    }

    // --- NEW METHOD ---
    public void CatchFish(GameObject fish)
    {
        if (!caughtFish.Contains(fish))
        {
            caughtFish.Add(fish);
            Debug.Log("Caught a fish! Total: " + caughtFish.Count);
        }
    }
}
