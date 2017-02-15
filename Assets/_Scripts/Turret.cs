using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

  public GameObject player;
  public GameObject trailblazer;
  public GameObject turretBulletPrefab;
  public GameObject firePoint;
  public float aggroRange = 6;
  public float fireTime = -1;
  public bool hit = false;
  public float timeHit = -1;
  public float rotatingDist;
  public bool restart = false;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player");
    trailblazer = GameObject.Find("Trailblazer");
	}
	
	// Update is called once per frame
	void Update () {

    if (timeHit != -1 && Time.time - timeHit > 2) {
      hit = false;
      restart = true;
      rotatingDist = 0;
      timeHit = -1;
    }

    if (hit) {
      Vector3 rot = transform.rotation.eulerAngles;
      Vector3 angles = new Vector3(rot.x + (Time.deltaTime * 2) * rotatingDist, rot.y, rot.z);
      if (angles.x > 65) angles.x = 65;
      transform.localEulerAngles = angles;
      return;
    }

    if (restart) {
      Vector3 rot = transform.rotation.eulerAngles;
      Vector3 angles = new Vector3(rot.x - (Time.deltaTime * 2) * 65, rot.y, rot.z);
      if (angles.x < 0) {
        angles.x = 0;
        restart = false;
      }
      transform.localEulerAngles = angles;
      return;
    }

    float pDistance = Vector3.Distance(transform.position, player.transform.position);
    float tDistance = Vector3.Distance(transform.position, trailblazer.transform.position);
    

    GameObject target = pDistance < tDistance ? player : trailblazer;
    float targetDistance = pDistance < tDistance ? pDistance : tDistance;

		transform.LookAt(target.transform);

    if (targetDistance < aggroRange && (fireTime == -1 || Time.time - fireTime > 1.5f)) {
      Fire();
    }
	}

  void Fire () {
    GameObject bullet = Instantiate<GameObject>(turretBulletPrefab);
    bullet.transform.position = firePoint.transform.position;
    bullet.GetComponent<Rigidbody2D>().velocity = transform.forward * 5 * Mathf.Sign(transform.rotation.eulerAngles.y);
    fireTime = Time.time;
  }

  public void NotifyHit () {
    hit = true;
    timeHit = Time.time;
    rotatingDist = Mathf.Abs(transform.rotation.eulerAngles.x - 65);
  }
}
