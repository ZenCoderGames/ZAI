//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeRoot
/// Purpose:  Root nodes are the starting point of every tree
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;

namespace ZEditor {

	public class ZNodeRoot:ZNode {
	    
        public ZNodeRoot(ZNodeTree nodeTree, Rect wr):base(BASE_TYPE.ROOT, nodeTree, wr) {
            _imgStyle.normal.background = NodeEditor.SkinItem.GetBaseNodeImage(BaseType) as Texture2D;
            _inspectorName = BaseType.ToString();
		}

	    protected override void CreateInConnector() {
	    }
	}

}