using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneManagerForAct : MonoBehaviour
{
    public static int playerHealth;
    public static string playerName;

    public TMP_Text description;
    public TMP_InputField inputField;
    public TMP_Text healthname;
    public VideoPlayer VideoPlayer;
    public VideoClip damage;
    public VideoClip heal;
    public VideoClip standby;
    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            healthname.text = "Name: " + playerName + "\nHealth: "+ playerHealth;
        }

    }
    public void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            playerName = inputField.text;
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            healthname.text = "Name: " + playerName + "\nHealth: " + playerHealth;
        }
    }
    public void NextScene()
    {
        SceneManager.LoadSceneAsync(5);
    }
    //public void NextScene2() { 
    //    SceneManager.LoadSceneAsync(6);
    //}

    public void SwitchScene(int scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }
    public void SelectHealth(int health)
    {
        playerHealth = health;
        Debug.Log(health);
        description.text = "You have selected " + playerHealth + " health.";
    }
    public void AddHealth() {
        playerHealth += 10;
        VideoPlayer.isLooping = false;
        VideoPlayer.clip = heal;
        VideoPlayer.loopPointReached += StandbyMode;
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
    public void DecreaseHealth()
    {
        playerHealth -= 10;
        VideoPlayer.isLooping = false;
        VideoPlayer.clip = damage;
        VideoPlayer.loopPointReached += StandbyMode;

    }
    private void StandbyMode(VideoPlayer source) {
        source.clip = standby;
        source.isLooping = true;
    }
}
    