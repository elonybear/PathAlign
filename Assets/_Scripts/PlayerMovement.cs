using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

  public static PlayerMovement S;

  public float groundSpeed = 4;
  public float jumpSpeed = 8;
  public float groundSpeedMult = 0;

  public bool ___________________;

  public bool grounded;
  Rigidbody2D rigid;
  int groundPhysicsLayerMask;
  int elevatorPhysicsLayerMask;
  Dictionary<string, float> multipliers;
  RaycastHit2D groundHit;
  public bool elevator;
  public float direction;

  void Awake () {
    S = this;
  }

  void Start () {
    rigid = GetComponent<Rigidbody2D>();
    grounded = false;
    groundPhysicsLayerMask = LayerMask.GetMask("Ground");
    elevatorPhysicsLayerMask = LayerMask.GetMask("Elevator");
    elevator = false;
    multipliers = new Dictionary<string, float>();
    direction = 1;
    multipliers["JumpBox"] = 2;
    multipliers["JumpBoxUltra"] = 2.5f;
    multipliers["SpeedBox"] = 1.5f;
    multipliers["SpeedBoxUltra"] = 2f;
  }

 
  void Update () {
    if (elevator) return;

    grounded = CheckIfGrounded();

    if (Input.GetButtonDown("A_P1") && grounded) {
      Vector2 vel = rigid.velocity;
      vel.y = jumpSpeed;
      rigid.velocity = vel;
    } 
  }
 	void FixedUpdate () {
    if (elevator) return;

		Vector2 vel = rigid.velocity;

    if (grounded) {
      GameObject col = groundHit.collider.gameObject;
      if(col.tag == "SpeedBox" || col.tag == "SpeedBoxUltra") {
        groundSpeedMult = multipliers[col.tag] * Mathf.Cos(col.transform.rotation.eulerAngles.z) * .5f;
      } else {
        groundSpeedMult = 0;
      }
    }

    if (!grounded && groundSpeedMult > 0) {
      groundSpeedMult = 0;
    }

    vel.x = Input.GetAxis("LeftJoystickX_P1") * groundSpeed + groundSpeed * groundSpeedMult;

    Vector3 rot = transform.rotation.eulerAngles;

    if (vel.x != 0) {
      rot.y = Mathf.Acos(Mathf.Sign(vel.x)) * 180 / Mathf.PI;
      direction = Mathf.Sign(vel.x);
    }

    rigid.velocity = vel;
    transform.rotation = Quaternion.Euler(0, rot.y, 0);
  }

  bool CheckIfGrounded () {
    groundHit = Physics2D.Raycast(transform.position, Vector2.down, 1.2f, groundPhysicsLayerMask);
    return groundHit;
  }

  void OnCollisionEnter2D (Collision2D coll) {

    if (elevator) return;

    if (grounded && (coll.gameObject.tag == "JumpBox" || coll.gameObject.tag == "JumpBoxUltra")) {
      Vector2 vel = rigid.velocity;
      vel.y = jumpSpeed * multipliers[coll.gameObject.tag];
      rigid.velocity = vel;
    } 

    if (grounded && (coll.gameObject.tag == "SpeedBox" || coll.gameObject.tag == "SpeedBoxUltra")) {
      groundSpeedMult = Mathf.Cos(coll.gameObject.transform.rotation.eulerAngles.z);
    }
  }

  void OnCollisionExit2D (Collision2D coll) {

    if (elevator) return;

    if ((coll.gameObject.tag == "SpeedBox" || coll.gameObject.tag == "SpeedBoxUltra")) {
      groundSpeedMult = 0;
    }
  }
}
