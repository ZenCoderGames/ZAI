using UnityEngine;
using UnityEditor;
using WyrmTale;
using ZEditor;

public class ZBTNegator:ZNodeDecorator {

    public ZBTNegator(ZNodeTree nodeTree, Rect wr):base(nodeTree, wr, NODE_TYPE.NEGATOR) {
		
	}
}