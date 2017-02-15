using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour {

  static public Gameplay S;

  public static int lives = 3;

  static string lastLevel = "MainMenu";

  void Awake () {
    S = this;
  }

	public void Restart () {
    lives--;
    if (lives > 0) {
      lastLevel = SceneManager.GetActiveScene().name;
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    } else {
      SceneManager.LoadScene("GameOver");
    }
  }

  public void LoadNextScene () {
    //lastLevel = SceneManager.GetSceneAt(SceneManager.GetActiveScene().buildIndex + 1).name;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void TutorialClick () {
    lastLevel = "Tutorial";
    SceneManager.LoadScene("Tutorial");
  }

  public void CampaignClick () {
    lastLevel = "Level1";
    SceneManager.LoadScene("Level1");
  }

  public void MainMenuClick () {
    lastLevel = "MainMenu";
    SceneManager.LoadScene("MainMenu");
  }

  public void TryAgainClick () {
    SceneManager.LoadScene(lastLevel);
  }
}
