using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        //set the game state to null so it resets a new game
        GameState.PlayerAddons = null;

        SceneManager.LoadScene("Menu");
    }
}
