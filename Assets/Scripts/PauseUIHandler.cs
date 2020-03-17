using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUIHandler : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private Text livesText;

    bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        isPaused = false;
        livesText.text = gameManager.GetNumberOfLives().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Show pause screen
            isPaused = !isPaused;
            pauseScreen.SetActive(isPaused);
            if (isPaused)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }

    public void ReloadGame()
    {
        if (gameManager != null)
            Destroy(gameManager.gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    public void MainMenu()
    {
        if (gameManager != null)
            Destroy(gameManager.gameObject);
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}
