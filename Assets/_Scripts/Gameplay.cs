using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour {

  static public Gameplay S;

  void Awake () {
    S = this;
  }

	public void Restart () {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  public void LoadNextScene () {
    print("Loading next scene");
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }
}
