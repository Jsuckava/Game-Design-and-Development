using UnityEngine;
using UnityEngine.SceneManagement;
public class Scenes : MonoBehaviour
{
    public void StartGame(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void QuitGame()
    {   
        Application.Quit();
    }
}