//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZBTNodeManager
/// Purpose:  This is the only class that needs to be edited when creating new nodes apart from
/// 		  the new nodes class itself.
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
using WyrmTale;
using ZEditor;

public static class ZBTActionManager {

    public enum NODE_TYPE {
       SCALE 
    }

	// Edit this function when creating new nodes
    public static ZNode CreateNode(NODE_TYPE actionNodeType, ZNodeTree nodeTree, Rect nodeRect, JSON js) {
		ZNode node = null;

        if(actionNodeType == NODE_TYPE.SCALE)
            node = new ZBTActionScale(nodeTree, nodeRect, js);
		
		return node;
	}
}
