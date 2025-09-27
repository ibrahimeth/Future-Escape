using UnityEngine;

public class PauseButtonHandler : MonoBehaviour
{
    // Bu fonksiyonları Inspector'dan OnClick olaylarına atayabilirsiniz
    
    public void ResumeGame()
    {
        if (GamePageManager.Instance != null)
        {
            GamePageManager.Instance.ResumeGame();
        }
    }

    public void GoToStartPage()
    {
        if (GamePageManager.Instance != null)
        {
            GamePageManager.Instance.GoToStartPage();
        }
    }
}
