using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ScenceLoader : MonoBehaviour
{
    public void StartGame() {
        SceneManager.LoadSceneAsync(1);
        Debug.Log("Proceeding to Next Scene.");
        Debug.LogError("Failed to Proceed into next scene");
    }

    public void QuitGame() {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Quit!");
#else
        Application.Quit();
#endif

    }
}
