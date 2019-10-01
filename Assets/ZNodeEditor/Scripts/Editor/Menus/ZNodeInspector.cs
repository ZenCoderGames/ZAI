//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeInspector
/// Purpose:  This class handles drawing the property of nodes in the left tab
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace ZEditor {

	public class ZNodeInspector {
	    ZNodeEditor _editor;
	    ZNode _selectedNode;
	    GUIStyle _titleStyle, _textStyle;
	    Rect _rect, _innerRect;

		public ZNodeInspector(ZNodeEditor editor) {
	        _editor  = editor;

	        _editor.OnNodeSelected += OnNodeSelected;

			_titleStyle = new GUIStyle("WhiteLabel");
			_titleStyle.alignment = TextAnchor.MiddleCenter;
			_titleStyle.fontStyle = FontStyle.Bold;

	        _textStyle = new GUIStyle("WhiteLabel");
			_textStyle.alignment = TextAnchor.MiddleLeft;
			_textStyle.fontStyle = FontStyle.Normal;
	    }

	    void OnNodeSelected (ZNode node) {
	        _selectedNode = node;
			GUI.changed = true;
	    }

		public void Draw() {
			if(_selectedNode!=null) {
				GUILayout.BeginVertical("");
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.TextArea("NODE INSPECTOR", _titleStyle);
				GUILayout.Space(-5);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		        if(_selectedNode!=null) {
					EditorGUILayout.BeginVertical(GUILayout.MinHeight(30));
					_selectedNode.DrawInInspector(_textStyle);
					EditorGUILayout.EndVertical();
				}
				GUILayout.EndVertical();
			}
	    }
	}

}