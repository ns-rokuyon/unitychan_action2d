using UnityEngine;
using System.Collections;

public class player_camera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find ("unitychan");
		//transform.LookAt (player.transform);
		transform.position = new Vector3 (player.transform.position.x, player.transform.position.y + 10, transform.position.z);
	}
}
