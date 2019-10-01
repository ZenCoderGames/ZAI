//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeMultiChild
/// Purpose:  Multi Child nodes can have multiple children
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using WyrmTale;

namespace ZEditor {

	public class ZNodeComposite:ZNode {
        public enum NODE_TYPE { SEQUENCER, SELECTOR, PARALLEL, DYNAMIC_SELECTOR }
        NODE_TYPE nodeType;

        public ZNodeComposite(ZNodeTree nodeTree, Rect wr, NODE_TYPE compositeType):base(BASE_TYPE.COMPOSITE, nodeTree, wr) {
            nodeType = compositeType;

            _imgStyle.normal.background = NodeEditor.SkinItem.GetCompositeNodeImage(nodeType) as Texture2D;
            _inspectorName = nodeType.ToString();
	    }

        #region SERIALIZE
        override public void Serialize(ref JSON nodeJS) {
            base.Serialize(ref nodeJS);

            nodeJS["nodeType"] = nodeType.ToString();
        }
        #endregion

        public static ZNode CreateNode(NODE_TYPE nodeType, ZNodeTree nodeTree, Rect nodeRect, JSON js) {
            ZNode node = null;

            if(nodeType == NODE_TYPE.SEQUENCER)
                node = new ZBTSequencer(nodeTree, nodeRect);

            return node;
        }
	}

}