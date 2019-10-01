//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeCreator
/// Purpose:  This class handles drawing the bt node types in the left tab
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ZEditor {

	public class ZNodeCreator {
		ZNodeEditor _editor;
		ZNode _selectedNode;
		GUIStyle _titleStyle, _textStyle, _searchStyle;
		Rect _rect, _innerRect;
		Vector2 _nodeScrollView = Vector2.zero;
		string _searchString;

        ZNodeComposite.NODE_TYPE[] _compositeNodeList;
        ZNodeDecorator.NODE_TYPE[] _decoratorNodeList;
        ZBTActionManager.NODE_TYPE[] _actionNodeList;

		public ZNodeCreator(ZNodeEditor editor) {
			_editor  = editor;

			_editor.OnNodeSelected += OnNodeSelected;

			_titleStyle = new GUIStyle("WhiteLabel");
			_titleStyle.alignment = TextAnchor.MiddleCenter;
			_titleStyle.fontStyle = FontStyle.Bold;

			_textStyle = new GUIStyle("WhiteLabel");
			_textStyle.alignment = TextAnchor.MiddleLeft;
			_textStyle.fontStyle = FontStyle.Normal;

			_searchStyle = ZEditorUtils.CreateGUIStyle(_editor.SkinItem.searchIcon);
			_searchString = "";

            _compositeNodeList = (ZNodeComposite.NODE_TYPE[])System.Enum.GetValues(typeof(ZNodeComposite.NODE_TYPE));
            _decoratorNodeList = (ZNodeDecorator.NODE_TYPE[])System.Enum.GetValues(typeof(ZNodeDecorator.NODE_TYPE));
            _actionNodeList = (ZBTActionManager.NODE_TYPE[])System.Enum.GetValues(typeof(ZBTActionManager.NODE_TYPE));
		}

		void OnNodeSelected (ZNode node) {
			_selectedNode = node;
			GUI.changed = true;
		}

		public void Draw() {
			if(_selectedNode==null) {
				GUILayout.BeginVertical("");
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.TextArea("NODE CREATOR", _titleStyle);
				GUILayout.Space(-5);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.BeginHorizontal();
				_searchString = GUILayout.TextField(_searchString, GUILayout.ExpandWidth(true));
				GUILayout.Box("", _searchStyle, GUILayout.Width(18), GUILayout.Height(18));
				GUILayout.Space(5);
				GUILayout.EndHorizontal();
				_nodeScrollView = GUI.BeginScrollView (
					new Rect (0, 55, _editor.SkinItem.leftPanelWidth, _editor.position.height - 90),
					_nodeScrollView,
                    new Rect (0, 52, _editor.SkinItem.leftPanelWidth, (_actionNodeList.Length + 1) * 30)
				);
				EditorGUILayout.BeginVertical(GUILayout.MinHeight(30));
                for(int i=0; i<_compositeNodeList.Length; ++i) {
                    if(!IsInValidSearch(_compositeNodeList[i].ToString())) {
                        continue;
                    }
                    if(GUILayout.Button(_compositeNodeList[i].ToString())) {
                        _editor.CreateCompositeNode(_compositeNodeList[i]);
                    }
                }
                for(int i=0; i<_decoratorNodeList.Length; ++i) {
                    if(!IsInValidSearch(_decoratorNodeList[i].ToString())) {
                        continue;
                    }
                    if(GUILayout.Button(_decoratorNodeList[i].ToString())) {
                        _editor.CreateDecoratorNode(_decoratorNodeList[i]);
                    }
                }
                for(int i=0; i<_actionNodeList.Length; ++i) {
                    if(!IsInValidSearch(_actionNodeList[i].ToString())) {
						continue;
					}
                    if(GUILayout.Button(_actionNodeList[i].ToString())) {
                        _editor.CreateActionNode(_actionNodeList[i]);
					}
				}
                string subtreeBtn = ZNode.BASE_TYPE.SUBTREE.ToString();
				if(IsInValidSearch(subtreeBtn)) {
					if(GUILayout.Button(subtreeBtn)) {
						_editor.CreateBaseNode(ZNode.BASE_TYPE.SUBTREE);
					}
				}
				EditorGUILayout.EndVertical();
				GUI.EndScrollView();
				GUILayout.EndVertical();
			}
		}

		bool IsInValidSearch(string btnName) {
			if(string.IsNullOrEmpty(_searchString))
				return true;
			
			return btnName.ToLower().Contains(_searchString.ToLower());
		}
	}

}