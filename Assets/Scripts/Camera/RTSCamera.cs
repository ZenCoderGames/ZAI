using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour {
	
	public float speed;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Horizontal")<0) {
			Camera.main.transform.Translate(Vector3.left*Time.deltaTime*speed);
		}
		else if (Input.GetAxis("Horizontal")>0) {
			Camera.main.transform.Translate(Vector3.right*Time.deltaTime*speed);
		}
		if (Input.GetAxis("Vertical")<0) {
			Camera.main.transform.Translate(Vector3.back*Time.deltaTime*speed, Space.World);
		}
		else if (Input.GetAxis("Vertical")>0) {
			Camera.main.transform.Translate(Vector3.forward*Time.deltaTime*speed, Space.World);
		}
		
		Camera.main.transform.Translate(Vector3.down*Time.deltaTime*Input.GetAxis("Mouse ScrollWheel"), Space.World);
	}
}
