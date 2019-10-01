//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeLeaf
/// Purpose:  Leaf nodes dont have children
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using WyrmTale;

namespace ZEditor {

	public class ZNodeAction:ZNode {
        ZBTActionManager.NODE_TYPE nodeType;

        public ZNodeAction(ZNodeTree nodeTree, Rect wr, ZBTActionManager.NODE_TYPE actionType):base(BASE_TYPE.ACTION, nodeTree, wr) {
            nodeType = actionType;

            _imgStyle.normal.background = NodeEditor.SkinItem.GetActionNodeImage(nodeType) as Texture2D;
            _inspectorName = nodeType.ToString();
	    }

	    protected override void CreateOutConnector() {}

        #region SERIALIZE
        override public void Serialize(ref JSON nodeJS) {
            base.Serialize(ref nodeJS);

            nodeJS["nodeType"] = nodeType.ToString();
        }
        #endregion
	}

}