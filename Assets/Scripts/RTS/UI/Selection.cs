using UnityEngine;
using System.Collections;

public class Selection : MonoBehaviour {

	private bool mouseClicked = false;
	private float mouseX = 0;
	private float mouseY = 0;
	private Rect currentRect;
	
	public ArrayList selected;
	
	// Use this for initialization
	void Start () {
		selected = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			foreach (GameObject go in selected) {
				(go.GetComponent<Renderer>()).material.color = Color.white;
			}
			selected.Clear();
			mouseClicked = true;
			mouseX = Input.mousePosition.x;
			mouseY = Input.mousePosition.y;
		}
		if (Input.GetMouseButtonUp(0)) {
			mouseClicked = false;
			findObjectsInSelection();
			gameObject.BroadcastMessage("setSelected", selected);
		}
	}
	
	void findObjectsInSelection() {
		GameObject[] selectableGO = GameObject.FindGameObjectsWithTag("Selectable");
		foreach (GameObject go in selectableGO) {
			Vector3 pos = Camera.main.WorldToScreenPoint(go.transform.position);
			Vector2 checkPos = new Vector2(pos.x, Screen.height-pos.y);
			if (currentRect.Contains(checkPos)) {
				selected.Add(go);
				(go.GetComponent<Renderer>()).material.color = Color.red;
			}
		}
	}
	
	void OnGUI() {
		if (mouseClicked) {
			currentRect = new Rect(mouseX, Screen.height - mouseY, Input.mousePosition.x-mouseX, mouseY-Input.mousePosition.y);
			GUI.Box(currentRect, "");	
		}
	}
}
