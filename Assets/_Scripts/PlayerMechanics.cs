using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMechanics : MonoBehaviour {

  public int health = 100;
  public GameObject body;
  public GameObject barrel;
  public GameObject endpoint;
  public Color collide;
  public Text healthText;

  void OnTriggerEnter2D(Collider2D other) {
    if (other.gameObject.layer == LayerMask.NameToLayer("TurretBullet")) {
      StartCoroutine(Flasher());
      health -= 10;
      healthText.text = health.ToString();
      if (health == 0) {
        Gameplay.S.Restart();
      }
    }

    if (other.gameObject.layer == LayerMask.NameToLayer("JumpPowerup")) {
      Trailblazer.S.EnablePower("Jump", 2);
      Trailblazer.S.hasJump = true;
      Destroy(other.gameObject);
    }

    if (other.gameObject.layer == LayerMask.NameToLayer("AmmoBox")) {
      Trailblazer.S.IncreaseCapacity("Standard", 5);
      Destroy(other.gameObject);
    }

    print("Collided with " + other.gameObject.name);

    if (other.gameObject.layer == LayerMask.NameToLayer("SpeedPowerup")) {
      Trailblazer.S.EnablePower("Speed", 10);
      Trailblazer.S.hasSpeed = true;
      Destroy(other.gameObject);
    }
  }

  IEnumerator Flasher () {
    Renderer bodyR = body.GetComponent<Renderer>();
    Renderer barrelR = barrel.GetComponent<Renderer>();
    Renderer endpointR = endpoint.GetComponent<Renderer>();
    Color normal = bodyR.material.color;
    for (int i = 0; i < 5; i++) {
      bodyR.material.color = collide;
      barrelR.material.color = collide;
      endpointR.material.color = collide;
      yield return new WaitForSeconds(.1f);
      bodyR.material.color = normal;
      barrelR.material.color = normal;
      endpointR.material.color = normal;
      yield return new WaitForSeconds(.1f);
    }
  }
}
