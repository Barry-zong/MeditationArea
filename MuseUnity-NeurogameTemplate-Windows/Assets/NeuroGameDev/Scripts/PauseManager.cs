using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class PauseManager : MonoBehaviour
{
    public enum PauseState
    {
        PAUSED,
        UNPAUSED
    }

    public PauseState currentPauseState;
    public RectTransform pauseMenuPanel;

    // This is so you can access inputs from the PlayerInput class in the Player Armature.
    // Your workflow will obviously vary depending on your game design.
    private StarterAssetsInputs starterAssetsInputs;

    // This is where I added the Pause action in the Input Actions Template.
    // Read the Starter Pack documentation in Assets > StarterAssets > Starter Assets_Documentation_v1.1.pdf
    [SerializeField] private InputActionReference inputActions;

    void Start()
    {
        starterAssetsInputs = FindFirstObjectByType<StarterAssetsInputs>();
        Unpause();
    }

    private void OnEnable()
    {
        inputActions.action.performed += TogglePause;
        inputActions.action.Enable();
    }

    private void OnDisable()
    {
        inputActions.action.performed -= TogglePause;
        inputActions.action.Disable();
    }

    public void Pause()
    {
        currentPauseState = PauseState.PAUSED;

        pauseMenuPanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
        starterAssetsInputs.cursorInputForLook = false;

        Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        currentPauseState = PauseState.UNPAUSED;

        pauseMenuPanel.gameObject.SetActive(false);
        starterAssetsInputs.cursorInputForLook = true;
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (currentPauseState == PauseState.PAUSED)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    public void TogglePause(bool pause)
    {
        if (pause)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    public void OnClickUnpause()
    {
        Unpause();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}