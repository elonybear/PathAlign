using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour {

  public float timeFired;
  public GameObject firePoint;
  public GameObject bulletPrefab;
  public int bulletSpeed = 5;
  void Start () {
    timeFired = -1;
  }

	// Update is called once per frame
	void Update () {

    float axisVal = Input.GetAxis("RightTrigger_P1");

    //Pressed trigger
		if (axisVal != 0 && axisVal != -1 && (timeFired == -1 || Time.time - timeFired > 1)) {
      //Fire weapon
      Fire();
    }
	}

  void Fire () {
    GameObject bullet = Instantiate<GameObject>(bulletPrefab);
    bullet.transform.position = firePoint.transform.position;
    bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(5, 0) * PlayerMovement.S.direction;
    timeFired = Time.time;
  }
}
