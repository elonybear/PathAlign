﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trailblazer : MonoBehaviour {

  public struct CubeType {

    public CubeType (int capacity_in, GameObject trailPrefab_in, GameObject solidPrefab_in) {
      capacity = capacity_in;
      structures = new Stack<GameObject>();
      trail = new List<GameObject>();
      trailPrefab = trailPrefab_in;
      solidPrefab = solidPrefab_in;
      maxCubes = 4 > capacity ? capacity : 4;
    }

    public int capacity;
    public Stack<GameObject> structures;
    public List<GameObject> trail;
    public GameObject trailPrefab;
    public GameObject solidPrefab;
    public int maxCubes;
  }

  public static Trailblazer S;

  //Standard
  public GameObject trailStandardCubePrefab;
  public GameObject solidStandardCubePrefab;

  //Jump
  public GameObject trailJumpCubePrefab;
  public GameObject solidJumpCubePrefab;

  //Ultra jump
  public GameObject trailUltraJumpCubePrefab;
  public GameObject solidUltraJumpCubePrefab;

  //Speed
  public GameObject trailSpeedCubePrefab;
  public GameObject solidSpeedCubePrefab;

  //Ultra speed
  public GameObject trailUltraSpeedCubePrefab;
  public GameObject solidUltraSpeedCubePrefab;

  //Parent of solid blocks
  public GameObject structureParentPrefab;

  public Text capacity;
  public Text trailLength;
  public Text blockType;

  public float speed = 1;
  public int standardCapacity;
  public int jumpCapacity;
  public int ultraJumpCapacity;
  public int speedCapacity = 10;
  public int ultraSpeedCapacity;
  public Color collide;

  public bool _____________________;

  Rigidbody2D rigid;
  float timeStart = 0;
  Vector3 direction;
  bool change = true;
  public bool obstacle;
  public int groundPhysicsLayerMask;
  public int elevatorPhysicsLayerMask;
  public bool incrementRight;
  public bool incrementLeft;
  public bool hasJump = false;
  public bool hasUltraJump = false;
  public bool hasSpeed = false;
  public bool hasUltraSpeed = false;
  public int currentBlockIndex = 0;
  public float timeHit = -1;
  public string currentBlockType = "Standard";

  Dictionary<string, CubeType> cubeTypes;
  public Dictionary<string, int> powersToIndex;
  public bool [] availableBlocks = new bool[5]{true, true, true, false, false};

  public List<string> cubeTypeStrings;

  void Awake () {
    S = this;
  }

  void Start () {
    rigid = GetComponent<Rigidbody2D>();
    timeStart = Time.time;
    direction = Vector2.up;
    groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    elevatorPhysicsLayerMask = LayerMask.GetMask("Elevator");
    obstacle = false;
    incrementLeft = false;
    incrementRight = false;

    cubeTypes = new Dictionary<string, CubeType>();
    cubeTypeStrings = new List<string>(new string[5]{"Standard", "Jump", "Speed", "Ultra Jump", "Ultra Speed"});

    cubeTypes["Standard"] = new CubeType(standardCapacity, trailStandardCubePrefab, solidStandardCubePrefab);
    cubeTypes["Jump"] = new CubeType(jumpCapacity, trailJumpCubePrefab, solidJumpCubePrefab);
    cubeTypes["Ultra Jump"] = new CubeType(ultraJumpCapacity, trailUltraJumpCubePrefab, solidUltraJumpCubePrefab);
    cubeTypes["Speed"] = new CubeType(speedCapacity, trailSpeedCubePrefab, solidSpeedCubePrefab);
    cubeTypes["Ultra Speed"] = new CubeType(speedCapacity, trailUltraSpeedCubePrefab, solidUltraSpeedCubePrefab);

    powersToIndex = new Dictionary<string, int>();
    int index = 0;
    foreach (string type in cubeTypeStrings) {
      powersToIndex[type] = index++;
    }
  }

	// Update is called once per frame
	void Update () {

    if (timeHit != -1 && Time.time - timeHit > 1f) {
      timeHit = -1;
    }
    
    if (PlayerMovement.S.elevator || timeHit != -1) return;

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

    if (Input.GetAxis("LeftTrigger_P2") != 0 && Input.GetAxis("LeftTrigger_P2") != -1 && !incrementLeft && cubeTypes[currentBlockType].maxCubes > 1) {
      incrementLeft = true;
      CubeType c = cubeTypes[currentBlockType];
      c.maxCubes--;
      cubeTypes[currentBlockType] = c;
    }

    if (Input.GetAxis("RightTrigger_P2") != 0 && Input.GetAxis("RightTrigger_P2") != -1 
      && !incrementRight && cubeTypes[currentBlockType].maxCubes < cubeTypes[currentBlockType].capacity && cubeTypes[currentBlockType].maxCubes < 10) {
      incrementRight = true;
      CubeType c = cubeTypes[currentBlockType];
      c.maxCubes++;
      cubeTypes[currentBlockType] = c;
    }


    if (Input.GetButtonDown("DPadUp_P2")) {

      if (!Gameplay.powered) {
        if (currentBlockIndex == 0) {
          int prevCurrentBlockIndex = currentBlockIndex;

          currentBlockIndex = availableBlocks.Length - 1;
          while (!availableBlocks[currentBlockIndex]) {
            currentBlockIndex--;
          }

          if (prevCurrentBlockIndex != currentBlockIndex)
            ClearTrail();
        } else if (availableBlocks[currentBlockIndex - 1]) {
          ClearTrail();
          currentBlockIndex -= 1;
        }
      } else {
        if (currentBlockIndex == 0) currentBlockIndex = availableBlocks.Length - 1;
        else currentBlockIndex--;
      }

      currentBlockType = cubeTypeStrings[currentBlockIndex];
    }


    if (Input.GetButtonDown("DPadDown_P2")) {

      if (!Gameplay.powered) {
        if (currentBlockIndex == availableBlocks.Length - 1 || !availableBlocks[currentBlockIndex + 1]) {
          ClearTrail();
          currentBlockIndex = 0;
        } else if (availableBlocks[currentBlockIndex + 1]) {
          ClearTrail();
          currentBlockIndex += 1;
        }
      } else {
        if (currentBlockIndex == availableBlocks.Length - 1) currentBlockIndex = 0;
        else currentBlockIndex++;
      }

      currentBlockType = cubeTypeStrings[currentBlockIndex];
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

    if (cubeTypes[currentBlockType].maxCubes > cubeTypes[currentBlockType].capacity) {
      CubeType c = cubeTypes[currentBlockType];
      c.maxCubes = cubeTypes[currentBlockType].capacity;
      cubeTypes[currentBlockType] = c;
    } else if (cubeTypes[currentBlockType].maxCubes == 0 && cubeTypes[currentBlockType].maxCubes < cubeTypes[currentBlockType].capacity) {
      CubeType c = cubeTypes[currentBlockType];
      c.maxCubes = 4 < cubeTypes[currentBlockType].capacity ? 4 : cubeTypes[currentBlockType].capacity;
      cubeTypes[currentBlockType] = c;
    }

    if (Time.time - timeStart >= speed) {
      Move();
      CreateTrailCube();
    }

    if (!Gameplay.powered) {
      blockType.text = currentBlockType;
      capacity.text = cubeTypes[currentBlockType].capacity.ToString();
      trailLength.text = cubeTypes[currentBlockType].maxCubes.ToString();
    } else {
      blockType.text = currentBlockType;
      capacity.text = "INF";
      trailLength.text = "INF";
    }
	}

  void Move () {

    if (obstacle && !Gameplay.invincible) {
      Destroy(gameObject);
      Gameplay.S.Restart();
      return;
    }

    if (obstacle && Gameplay.invincible) {
      direction = -direction;
    }

    transform.position += direction;
    timeStart = Time.time;
  }

  void CreateTrailCube () {
    //Creating new trail cube
    GameObject nextTrailCube = Instantiate<GameObject>(cubeTypes[currentBlockType].trailPrefab);
    nextTrailCube.transform.position = transform.position - direction;
    nextTrailCube.GetComponent<TrailCube>().type = currentBlockType;
    cubeTypes[currentBlockType].trail.Add(nextTrailCube);

    if (!Gameplay.powered) {
      while (cubeTypes[currentBlockType].trail.ToArray().Length > cubeTypes[currentBlockType].maxCubes) {
        CutEnd();
      }
    }
  }

  void SolidifyTrail () {

    GameObject parent = Instantiate<GameObject>(structureParentPrefab);
    parent.GetComponent<StructureParent>().size = cubeTypes[currentBlockType].trail.ToArray().Length;

    foreach (GameObject trailCube in cubeTypes[currentBlockType].trail) {
      if (standardCapacity == 0) return;
      GameObject solidCube = Instantiate<GameObject>(cubeTypes[currentBlockType].solidPrefab);
      solidCube.transform.position = trailCube.transform.position;
      solidCube.transform.localEulerAngles = new Vector3(0, 0, Mathf.Acos(direction.x) * 180 / Mathf.PI);
      solidCube.transform.parent = parent.transform;
      Destroy(trailCube);
      CubeType c = cubeTypes[currentBlockType];
      if (!Gameplay.powered) {
        c.capacity--;
      }
      cubeTypes[currentBlockType] = c;
    }

    if (cubeTypes[currentBlockType].maxCubes > cubeTypes[currentBlockType].capacity) {
      CubeType c = cubeTypes[currentBlockType];
      c.maxCubes = cubeTypes[currentBlockType].capacity;
      cubeTypes[currentBlockType] = c;
    }

    cubeTypes[currentBlockType].structures.Push(parent);

    cubeTypes[currentBlockType].trail.Clear();
  }

  void ClearTrail () {
    foreach(GameObject trailCube in cubeTypes[currentBlockType].trail) {
      Destroy(trailCube);
    }

    cubeTypes[currentBlockType].trail.Clear();
  }

  bool CheckForObstacle () {
    Vector2 checkLoc = transform.position;
    return Physics2D.Raycast(checkLoc, direction, 1, groundPhysicsLayerMask)
      || Physics2D.Raycast(checkLoc, direction, 1, elevatorPhysicsLayerMask);
  }

  void CutEnd () {
    GameObject trailCube = cubeTypes[currentBlockType].trail[0];
    RemoveTrailCube(trailCube, currentBlockType);
    Destroy(trailCube);
  }

  void DestroyStructure () {
    if (cubeTypes[currentBlockType].structures.Peek() != null) {
      if (!Gameplay.powered) {
        CubeType c = cubeTypes[currentBlockType];
        c.capacity += cubeTypes[currentBlockType].structures.Peek().GetComponent<StructureParent>().size;
        cubeTypes[currentBlockType] = c;
      }
      Destroy(cubeTypes[currentBlockType].structures.Peek());
      cubeTypes[currentBlockType].structures.Pop();
      if (cubeTypes[currentBlockType].maxCubes == 0 && !Gameplay.powered) {
        CubeType c = cubeTypes[currentBlockType];
        c.maxCubes = 4 < cubeTypes[currentBlockType].capacity ? 4 : cubeTypes[currentBlockType].capacity;
        cubeTypes[currentBlockType] = c;
      }
    }
  }

  void OnTriggerEnter2D (Collider2D other) {
    if (other.gameObject.layer == LayerMask.NameToLayer("TurretBullet")) {
      timeHit = Time.time;
      StartCoroutine(Flasher());
    }

    if (other.gameObject.layer == LayerMask.NameToLayer("BottomlessPit")) {
      Gameplay.S.Restart();
    }
  }

  IEnumerator Flasher () {
    Renderer body = GetComponent<Renderer>();
    Color normal = body.material.color;
    for (int i = 0; i < 5; i++) {
      body.material.color = collide;
      yield return new WaitForSeconds(.1f);
      body.material.color = normal;
      yield return new WaitForSeconds(.1f);
    }
  }

  /* ============ public interface =========== */

  public void RemoveTrailCube (GameObject trailCube, string type) {
    cubeTypes[type].trail.Remove(trailCube);
  }

  public void EnablePower (string power, int capacity) {
    if (Gameplay.powered) return;
    availableBlocks[powersToIndex[power]] = true;
    CubeType c = cubeTypes[power];
    c.capacity = capacity;
    c.maxCubes = capacity;
    cubeTypes[power] = c;
  }

  public void IncreaseCapacity (string power, int capacity) {
    if (Gameplay.powered) return;
    CubeType c = cubeTypes[power];
    c.capacity += capacity;
    cubeTypes[power] = c;
  }
}
