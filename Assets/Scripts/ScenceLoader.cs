using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class ScenceLoader : MonoBehaviour
{
    public void StartGame() {
        SceneManager.LoadSceneAsync(1);
        Debug.Log("Proceeding to Next Scene.");
    }
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Quit!");
#else
        Application.Quit();
#endif
    
    }
    public void GoBack() {
        SceneManager.LoadSceneAsync(2);
    }
}
