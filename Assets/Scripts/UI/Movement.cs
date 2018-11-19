using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {
	
	public ArrayList selected;
	private Ray ray;
	private RaycastHit hit;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(1)) {
			// Get the point
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast (ray, out hit, 200)) {
			  // Move all selected to the position
			  foreach (GameObject go in selected) {
                    go.transform.position = new Vector3(hit.point.x, 0, hit.point.y);
			  }
			}
		}
	}
	
	void setSelected(ArrayList selArray) {
		selected = selArray;
	}
}
