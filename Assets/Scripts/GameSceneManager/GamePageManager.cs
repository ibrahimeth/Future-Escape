using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamePageManager : MonoBehaviour
{
  [SerializeField] public AudioSource backgroundAudio;

  public static GamePageManager Instance { get; private set; }

  private GameObject pausePopup;

  void Awake()
  {
    if (backgroundAudio != null && !backgroundAudio.isPlaying)
    {
      backgroundAudio.Play();
    }
    // Singleton pattern implementation
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  void Start()
  {
    FindPausePopup();
  }

  private void FindPausePopup()
  {
    // Find the PausePopup in the current scene's canvas
    Canvas canvas = FindFirstObjectByType<Canvas>();
    if (canvas != null)
    {
      Transform pausePopupTransform = canvas.transform.Find("PausePopup");
      if (pausePopupTransform != null)
      {
        pausePopup = pausePopupTransform.gameObject;
      }
    }
  }

  public void OnPauseGame(InputValue value)
  {
    if (value.isPressed)
    {
      PausePage(Time.timeScale == 0);
    }
  }

  public void PausePage(bool isGamePuased)
  {
    if (!isGamePuased)
    {
      Time.timeScale = 0f; // Pause the game

      // Find pause popup if not already found
      if (pausePopup == null)
      {
        FindPausePopup();
      }

      // Show pause popup
      if (pausePopup != null)
      {
        pausePopup.SetActive(true);
      }
    }
    else
    {
      Time.timeScale = 1f; // Resume the game

      // Hide pause popup
      if (pausePopup != null)
      {
        pausePopup.SetActive(false);
      }
    }
  }

  // Pause popup buton fonksiyonları
  public void ResumeGame()
  {
    PausePage(true); // Oyunu devam ettir (unpause)
  }

  public void GoToStartPage()
  {
    Time.timeScale = 1f; // Resume time before changing scene
    SceneManager.LoadScene("StartPage");
  }

}