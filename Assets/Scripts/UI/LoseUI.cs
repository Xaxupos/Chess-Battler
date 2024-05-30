using UnityEngine.SceneManagement;

public class LoseUI : GameUI
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}