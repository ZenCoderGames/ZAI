using UnityEngine;
using UnityEditor;
using WyrmTale;
using ZEditor;

public class ZBTDynamicSelector:ZNodeComposite {

    public ZBTDynamicSelector(ZNodeTree nodeTree, Rect wr):base(nodeTree, wr, NODE_TYPE.DYNAMIC_SELECTOR) {
		
	}
}