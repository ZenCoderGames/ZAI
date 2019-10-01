//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeSubTree
/// Purpose:  Subtree nodes are just links to a different tree
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using WyrmTale;

namespace ZEditor {

	public class ZNodeSubTree:ZNode {
		public string treePath;

        public ZNodeSubTree(ZNodeTree nodeTree, Rect wr, JSON js):base(BASE_TYPE.SUBTREE, nodeTree, wr) {
            _imgStyle.normal.background = NodeEditor.SkinItem.GetBaseNodeImage(BaseType) as Texture2D;
            _inspectorName = BaseType.ToString();

            treePath = "";

			if(js!=null && js.Contains("nodeParams")) {
				JSON paramsJS = js.ToJSON("nodeParams");
				treePath = paramsJS.ToString("treePath");
			}
		}

		protected override void CreateOutConnector() {}

		override public void Serialize(ref JSON nodeJS) {
			base.Serialize(ref nodeJS);

			JSON paramsJS = new JSON();
			paramsJS["treePath"] = treePath;
			nodeJS["nodeParams"] = paramsJS;
		}

		override public void DrawInInspector(GUIStyle guiStyle) {
			base.DrawInInspector(guiStyle);

			GUILayout.BeginHorizontal("");
			GUILayout.TextArea("Tree Path:", guiStyle);
			treePath = GUILayout.TextField(treePath, EditorStyles.textField);
			GUILayout.EndHorizontal();

			if(GUILayout.Button("Show Subtree")) {
				NodeEditor.GoToOrCreateTree(treePath);
			}
		}
	}

}