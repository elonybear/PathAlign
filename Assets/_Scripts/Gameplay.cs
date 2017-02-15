using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour {

  static public Gameplay S;

  static int lives = 3;

  void Awake () {
    S = this;
  }

	public void Restart () {
    lives--;
    if (lives > 0) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    } else {

    }
  }

  public void LoadNextScene () {
    print("Loading next scene");
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }

  public void TutorialClick () {
    SceneManager.LoadScene("Tutorial");
  }

  public void CampaignClick () {
    SceneManager.LoadScene("Level1");
  }
}
