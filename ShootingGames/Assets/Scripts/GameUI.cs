using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        
    }

    void OnGameOver()
    {
        //khi bat dau` goi. Fade thi` no' se~ chuyen? dan` tu` mau` trong sang mau` den trong vong` 1s
        StartCoroutine(Fade(Color.clear,Color.black,1));
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from ,Color to,float time) 
    {
        float speed = 1 / time;
        //do. mo` dan` cua? mau`
        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }

    }

    //UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }
}
