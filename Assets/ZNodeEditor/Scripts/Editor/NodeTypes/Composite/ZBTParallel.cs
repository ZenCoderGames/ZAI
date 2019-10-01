using UnityEngine;
using UnityEditor;
using WyrmTale;
using ZEditor;

public class ZBTParallel:ZNodeComposite {

    public ZBTParallel(ZNodeTree nodeTree, Rect wr):base(nodeTree, wr, NODE_TYPE.PARALLEL) {
		
	}
}