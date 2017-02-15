using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

  Rigidbody2D rigid;
  public float timeStart = -1;

  void Start () {
    rigid = GetComponent<Rigidbody2D>();
  }

  void FixedUpdate () {
    if (timeStart != -1 && Time.time - timeStart > 3) {
      Gameplay.S.LoadNextScene();
    }
  }

	void OnCollisionEnter2D (Collision2D other) {
  
    if (other.gameObject.tag == "Player") {
      PlayerMovement.S.elevator = true;

      rigid.velocity = new Vector2(0, 5);
      timeStart = Time.time;
    }
  }
}
