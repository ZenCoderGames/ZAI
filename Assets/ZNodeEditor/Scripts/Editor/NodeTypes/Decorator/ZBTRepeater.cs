using UnityEngine;
using UnityEditor;
using WyrmTale;
using ZEditor;

public class ZBTRepeater:ZNodeDecorator {

    public ZBTRepeater(ZNodeTree nodeTree, Rect wr):base(nodeTree, wr, NODE_TYPE.REPEATER) {
		
	}
}