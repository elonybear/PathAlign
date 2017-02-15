using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

  public GameObject player;
  public GameObject trailblazer;
  public float smoothTime = .5f;
  public bool ______________________;
  public Camera cam;
  public float startTime;
  public float duration = 1;
  public float nextOrthSize;
  public float prevDistance;

  Vector3 velocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
    startTime = -1;
    nextOrthSize = cam.orthographicSize;

    float pX = player.transform.position.x;
    float pY = player.transform.position.y;
    float tX = trailblazer.transform.position.x;
    float tY = trailblazer.transform.position.y;

    prevDistance = Mathf.Sqrt(Mathf.Pow(pX - tX, 2) + Mathf.Pow(pY - tY, 2));
	}
	
	// Update is called once per frame
	void FixedUpdate () {

    if (PlayerMovement.S.elevator) return;

		float averageX = (player.transform.position.x + trailblazer.transform.position.x) / 2;
    float averageY = (player.transform.position.y + trailblazer.transform.position.y) / 2;
    float diffX = Mathf.Abs(player.transform.position.x - trailblazer.transform.position.x);
    float diffY = Mathf.Abs(player.transform.position.y - trailblazer.transform.position.y);

    float pX = player.transform.position.x;
    float pY = player.transform.position.y;
    float tX = trailblazer.transform.position.x;
    float tY = trailblazer.transform.position.y;

    float nextDistance = Mathf.Sqrt(Mathf.Pow(pX - tX, 2) + Mathf.Pow(pY - tY, 2));

    Vector3 endLoc = new Vector3(averageX, averageY, transform.position.z);

    transform.position = Vector3.SmoothDamp(transform.position, endLoc, ref velocity, smoothTime, 5f, Time.deltaTime); 

    float height = cam.orthographicSize * 2;
    float width = height * cam.aspect;

    if (diffY > height - 6 && nextDistance > prevDistance) {
      float newHeight = height + 2;
      nextOrthSize = newHeight / 2;
    }

    if (diffX > width - 6 && nextDistance > prevDistance) {
      float newWidth = width + 2;
      float newHeight = newWidth / cam.aspect;
      nextOrthSize = newHeight / 2;
    }

    if (diffY < height - 6 && nextDistance < prevDistance) {
      float newHeight = height - 2;
      nextOrthSize = newHeight / 2;
    }

    if (diffX < width - 6 && nextDistance < prevDistance) {
      float newWidth = width - 2;
      float newHeight = newWidth / cam.aspect;
      nextOrthSize = newHeight / 2;
    }

    if (nextOrthSize < 10) nextOrthSize = 10;

    prevDistance = nextDistance;

    cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, nextOrthSize, .05f);
	}
}
