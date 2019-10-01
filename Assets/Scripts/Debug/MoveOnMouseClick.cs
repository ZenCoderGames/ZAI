using UnityEngine;
using System.Collections;

public class MoveOnMouseClick : MonoBehaviour {
	
	private Ray ray;
	private RaycastHit hit;
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			// Get the point
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast (ray, out hit, 200)) {
                gameObject.transform.position = new Vector3(hit.point.x, 0, hit.point.z);
			}
		}
	}
}
