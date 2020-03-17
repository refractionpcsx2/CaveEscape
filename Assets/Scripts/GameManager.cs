using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int lives;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        lives = 3;
    }

    public void TakeLife()
    {
        lives -= 1;

        if(lives == 0)
        {
            GameOver();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public int GetNumberOfLives()
    {
        return lives;
    }

    public void Win()
    {
        SceneManager.LoadScene(4);
    }

    private void GameOver()
    {
        SceneManager.LoadScene(3);
    }
}
