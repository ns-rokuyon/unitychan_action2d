using UnityEngine;
using System.Collections;

using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class player : MonoBehaviour {
	
	protected Animator animator;

	public Vector3 moveF = new Vector3(200f, 0, 0);
	public float maxspeed = 20f;

	private float speed = 0;
	private float direction = 0;
	private Locomotion locomotion = null;
	private Vector3 moveDirection= Vector3.zero;
	private Vector3 old_forward;
	private Vector3 turn_slide_formard;
	private int anim_turn_id;
	private bool on_ground = true;
	private float dist_to_ground;	// 地面までの距離
	private Vector3 dist_checksphere_center = new Vector3(0,0.2f,0);	// 地面との当たり判定に使う球の中心までの距離
	private float anim_speed_default;	// デフォルトのアニメーションの再生スピード
	private float jump_start_y ;		// ジャンプ開始時のy座標

	private const float CHECKSPHERE_RADIUS = 0.1f;	// 地面との当たり判定に使う球の半径

	// Use this for initialization
	void Start () 
	{
		old_forward = transform.forward;
		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
		anim_speed_default = animator.speed;
		dist_to_ground = GetComponent<CapsuleCollider>().height;
	}

	void Update()
	{
	}


	void FixedUpdate(){
		float h = Input.GetAxis ("Horizontal");
		bool space_key_down = Input.GetKeyDown("space");
		bool z_key_down = Input.GetKeyDown("z");
		AnimatorStateInfo anim_state = animator.GetCurrentAnimatorStateInfo(0);

		animator.SetFloat ("Speed", Mathf.Abs (h));

		float vx = rigidbody.velocity.x;
		float vy = rigidbody.velocity.y;
		Vector3 fw = transform.forward;

		if (vy < 0 && isGrounded()){
			// 着地したときの処理
			animator.SetBool("OnGround",true);
			animator.speed = anim_speed_default;
		}

		if (Mathf.Abs (h) >= 0.2 && h * vx < maxspeed) {
			if (Mathf.Sign (h) != Mathf.Sign (vx) && Mathf.Abs(vx) > 0.1f) {
		    	// プレイヤーが方向転換しているときは弱めの力を与える
				rigidbody.AddForce (h * moveF / 4.0f);
				if (isGrounded()){
					animator.CrossFade("PlantNTurnRight180",0.001f);
					animator.SetBool ("Turn", true);
				}
			} else {
				// 加速させる
				rigidbody.AddForce (h * moveF);
				animator.SetBool ("Turn", false);
			}
		} else {
			animator.SetBool("Turn", false);
		}



		if (Mathf.Abs(h) < 0.2 && Mathf.Abs(vx) > 0.2f) {
			// キー入力が無かったらブレーキをかける
			if (Mathf.Sign(fw.x) == Mathf.Sign(vx)) {
				rigidbody.AddForce(fw * -20.0f);
			} else {
				rigidbody.AddForce(fw * 20.0f);
			}
		}

		if (Mathf.Abs (vx) > maxspeed) {
			rigidbody.velocity = new Vector3(Mathf.Sign(vx)* maxspeed, vy);
		}

		
		if (h > 0 && fw.x < 0) {
			// 左を向いていて右に入力されているとき
			transform.rotation = Quaternion.LookRotation (new Vector3 (h, 0, 0.8f));
		} else if (h < 0 && fw.x > 0) {
			// 右を向いていて左に入力されているとき
			transform.rotation = Quaternion.LookRotation (new Vector3 (h, 0, -0.8f));
		} else {
			float newz_fw = fw.z;
			if ( newz_fw > 0 ) {
				newz_fw -= 0.3f;
				if (newz_fw < 0) {
					newz_fw = 0;
				}
			}
			else {
				newz_fw += 0.3f;
				if (newz_fw > 0) {
					newz_fw = 0;
				}
			}
			transform.rotation = Quaternion.LookRotation (new Vector3 (fw.x, 0, newz_fw));
		}

		if (space_key_down && isGrounded()) {
			// ジャンプ
			jump_start_y = transform.position.y;
			rigidbody.AddForce(new Vector3(0f, 1200.0f,0));
			animator.CrossFade("Jump",0.001f);
			animator.SetBool("OnGround", false);
		}

		if (z_key_down && !animator.GetBool("Turn") && isGrounded()) {
			// スライディング
			animator.CrossFade("Sliding", 0.001f);
		}

		// 重力
		rigidbody.AddForce(new Vector3(0f, -26.0f,0));

		//if (anim_state.IsName("Base Layer.Jump")){
		//}
	}

	void OnJumpAnimEnd(){
		Debug.Log("on jump anim end");
		animator.speed = 0.2f;
	}

	private bool isGrounded(){
		return Physics.CheckSphere(transform.position - dist_checksphere_center,  CHECKSPHERE_RADIUS);
	}

	void OnGUI()
	{
		Vector3 fw = transform.forward;
		Quaternion rot = transform.rotation;
		Vector3 targetDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		float vx = rigidbody.velocity.x; 
		float vy = rigidbody.velocity.y;
		CapsuleCollider cc = GetComponent<CapsuleCollider>();
		GUI.Box(new Rect(Screen.width -260, 10 ,250 ,200), "Interaction");
		GUI.Label(new Rect(Screen.width -245,30,250,30),"forward: " + fw);
		GUI.Label(new Rect(Screen.width -245,50,250,30),"vx: " + vx);
		GUI.Label(new Rect(Screen.width -245,70,250,30),"vy: " + vy);
		GUI.Label(new Rect(Screen.width -245,90,250,30),"targetDirection: " + targetDirection);
		GUI.Label(new Rect(Screen.width -245,110,250,30),"on_ground: " + isGrounded());
		GUI.Label(new Rect(Screen.width -245,130,250,30),"dist_to_ground: " + dist_to_ground);
		GUI.Label(new Rect(Screen.width -245,150,250,30),"(x,y,z): " + transform.position);
		GUI.Label(new Rect(Screen.width -245,170,250,30),"capsule_center: " + cc.bounds.center);
		GUI.Label(new Rect(Screen.width -245,190,250,30),"capsule_height: " + cc.height);
	}
}

/*
 Unitychan Animation 変更箇所

unitychan_JUMP00
	start: 10 - end: 56
	Root Transform Position (Y) : off
	Events:
		* 0.8付近 : OnJumpAnimEnd()


DefaultAvatar_PlantNTurn180
	start:151 - end: 194.7
	Root Transform Rotation : off
	Root Transform Position (Y) : on
	Root Transform Position (XZ) : off
*/ 