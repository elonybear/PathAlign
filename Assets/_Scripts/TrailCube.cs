using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailCube : MonoBehaviour {

  public float timeFactor = 20;
  public bool ___________________;
  public Renderer rend;
  public bool solid;
  public string type;


  void Start () {
    rend = GetComponent<Renderer>();
    solid = false;

    Color temp = rend.material.color;
    temp.a = .75f;
    rend.material.color = temp;
  }

  void FixedUpdate () {
    if (solid) return;

    Color temp = rend.material.color;
    temp.a -= Time.deltaTime / timeFactor;
    rend.material.color = temp;

    if (rend.material.color.a <= 0) {
      Destroy(gameObject);
      Trailblazer.S.RemoveTrailCube(gameObject, type);
    }
  }
}
