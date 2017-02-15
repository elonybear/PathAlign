using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trailblazer : MonoBehaviour {

  public static Trailblazer S;

  public GameObject trailCubePrefab;
  public GameObject solidCubePrefab;
  public GameObject structureParentPrefab;
  public float speed = 1;
  public int blockCapacity;

  public bool _____________________;

  Rigidbody2D rigid;
  float timeStart = 0;
  Vector3 direction;
  public List<GameObject> trail;
  public Stack<GameObject> structures;
  bool change = true;
  public bool obstacle;
  public int groundPhysicsLayerMask;
  public int elevatorPhysicsLayerMask;
  public int maxCubes = 4;
  public bool incrementRight;
  public bool incrementLeft;

  void Awake () {
    S = this;
  }

  void Start () {
    rigid = GetComponent<Rigidbody2D>();
    timeStart = Time.time;
    trail = new List<GameObject>();
    direction = Vector2.up;
    groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    elevatorPhysicsLayerMask = LayerMask.GetMask("Elevator");
    obstacle = false;
    incrementLeft = false;
    incrementRight = false;
    structures = new Stack<GameObject>();
  }

	// Update is called once per frame
	void Update () {

    if (PlayerMovement.S.elevator) return;

    Debug.DrawRay(transform.position, direction * .6f, Color.red);

    obstacle = CheckForObstacle();

    if (Input.GetButtonDown("X_P2")) {
      SolidifyTrail();
    }

    if (Input.GetButtonDown("Y_P2")) {
      DestroyStructure();
    }

    if (Input.GetAxis("LeftJoystickY_P2") == -1 && direction.y == 0) {
      direction = Vector3.up;
      change = true;
    }

    if (Input.GetAxis("LeftJoystickY_P2") == 1 && direction.y == 0) {
      direction = Vector3.down;
      change = true;
    }

    if (Input.GetAxis("LeftJoystickX_P2") == 1 && direction.x == 0) {
      direction = Vector3.right;
      change = true;
    }

    if (Input.GetAxis("LeftJoystickX_P2") == -1 && direction.x == 0) {
      direction = Vector3.left;
      change = true;
    }

    if (Input.GetAxis("LeftTrigger_P2") != 0 && Input.GetAxis("LeftTrigger_P2") != -1 && !incrementLeft && maxCubes > 0) {
      incrementLeft = true;
      maxCubes -= 1;
    }

    if (Input.GetAxis("RightTrigger_P2") != 0 && Input.GetAxis("RightTrigger_P2") != -1 && !incrementRight && maxCubes < 10) {
      incrementRight = true;
      maxCubes += 1;
    }


    if ((Input.GetAxis("LeftTrigger_P2") == 0 || Input.GetAxis("LeftTrigger_P2") == -1) && incrementLeft) {
      incrementLeft = false;
    }

    if ((Input.GetAxis("RightTrigger_P2") == 0 || Input.GetAxis("RightTrigger_P2") == -1) && incrementRight) {
      incrementRight = false;
    }

    if (change) {
      timeStart = Time.time - speed;
      change = false;
    }

    if (Time.time - timeStart >= speed) {
      //Move();
      //CreateTrailCube();
    }
	}

  void Move () {

    if (obstacle) {
      Destroy(gameObject);
      Gameplay.S.Restart();
      return;
    }

    transform.position += direction;
    timeStart = Time.time;
  }

  void CreateTrailCube () {
    //Creating new trail cube
    GameObject nextTrailCube = Instantiate<GameObject>(trailCubePrefab);
    nextTrailCube.transform.position = transform.position - direction;
    trail.Add(nextTrailCube);

    while (trail.ToArray().Length > maxCubes) {
      CutEnd();
    }
  }

  void SolidifyTrail () {

    GameObject parent = Instantiate<GameObject>(structureParentPrefab);
    parent.GetComponent<StructureParent>().size = trail.ToArray().Length;

    foreach (GameObject trailCube in trail) {
      if (blockCapacity == 0) return;
      GameObject solidCube = Instantiate<GameObject>(solidCubePrefab);
      solidCube.transform.position = trailCube.transform.position;
      solidCube.transform.parent = parent.transform;
      Destroy(trailCube);
      blockCapacity--;
    }

    if (maxCubes > blockCapacity) {
      maxCubes = blockCapacity;
    }

    structures.Push(parent);

    trail.Clear();
  }

  void ClearTrail () {
    foreach(GameObject trailCube in trail) {
      Destroy(trailCube);
    }

    trail.Clear();
  }

  bool CheckForObstacle () {
    Vector2 checkLoc = transform.position;
    return Physics2D.Raycast(checkLoc, direction, 1, groundPhysicsLayerMask)
      || Physics2D.Raycast(checkLoc, direction, 1, elevatorPhysicsLayerMask);
  }

  void CutEnd () {
    GameObject trailCube = trail[0];
    RemoveTrailCube(trailCube);
    Destroy(trailCube);
  }

  void DestroyStructure () {
    if (structures.Peek() != null) {
      blockCapacity += structures.Peek().GetComponent<StructureParent>().size;
      Destroy(structures.Peek());
      structures.Pop();
      if (maxCubes == 0) maxCubes = 4;
    }
  }

  /* ============ public interface =========== */

  public void RemoveTrailCube (GameObject trailCube) {
    trail.Remove(trailCube);
  }
}
