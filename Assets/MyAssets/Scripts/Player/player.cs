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
	
	// Use this for initialization
	void Start () 
	{
		old_forward = transform.forward;
		animator = GetComponent<Animator>();
		locomotion = new Locomotion(animator);
	}

	void Update()
	{
	}


	void FixedUpdate(){
		float h = Input.GetAxis ("Horizontal");
		bool space_key_down = Input.GetKeyDown("space");
		AnimatorStateInfo anim_state = animator.GetCurrentAnimatorStateInfo(0);

		animator.SetFloat ("Speed", Mathf.Abs (h));

		float vx = rigidbody.velocity.x;
		float vy = rigidbody.velocity.y;
		Vector3 fw = transform.forward;

		if (Mathf.Abs (h) >= 0.2 && h * vx < maxspeed) {
			if (Mathf.Sign (h) != Mathf.Sign (vx) && Mathf.Abs(vx) > 0.1f) {
		    	// プレイヤーが方向転換しているときは弱めの力を与える
				rigidbody.AddForce (h * moveF / 4.0f);
				animator.CrossFade("PlantNTurnRight180",0.001f);
				animator.SetBool ("Turn", true);
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

		if (space_key_down) {
			//animator.SetTrigger("Jump");
			rigidbody.AddForce(new Vector3(0f, 1200.0f,0));
			animator.CrossFade("Jump",0.001f);
		}

		rigidbody.AddForce(new Vector3(0f, -20.0f,0));

		//if (anim_state.IsName("Base Layer.Jump")){
		//}
	}



	void OnGUI()
	{
		Vector3 fw = transform.forward;
		Quaternion rot = transform.rotation;
		Vector3 targetDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		float vx = rigidbody.velocity.x; 
		float vy = rigidbody.velocity.y;
		GUI.Box(new Rect(Screen.width -260, 10 ,250 ,150), "Interaction");
		GUI.Label(new Rect(Screen.width -245,30,250,30),"forward: " + fw);
		GUI.Label(new Rect(Screen.width -245,50,250,30),"vx: " + vx);
		GUI.Label(new Rect(Screen.width -245,70,250,30),"vy: " + vy);
		GUI.Label(new Rect(Screen.width -245,90,250,30),"targetDirection" + targetDirection);
		GUI.Label(new Rect(Screen.width -245,1100,250,30),"Hit Spase key while Stopping : Rest");
		GUI.Label(new Rect(Screen.width -245,130,250,30),"Left Control : Front Camera");
	}
}

