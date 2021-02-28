using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    void Start(){
        SoundManager.instance.PlayMusic("puzzle_game_02",0.5f);
    }

    public void GoToGame(){
        SceneManager.LoadScene("GameScene");
    }
}
