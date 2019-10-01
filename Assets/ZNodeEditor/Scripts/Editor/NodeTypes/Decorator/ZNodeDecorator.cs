//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeSingleChild
/// Purpose:  Single child nodes can only have a single child connection.
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using WyrmTale;

namespace ZEditor {

	public class ZNodeDecorator:ZNode {
        public enum NODE_TYPE { NEGATOR, REPEATER }
        NODE_TYPE nodeType;

        public ZNodeDecorator(ZNodeTree nodeTree, Rect wr, NODE_TYPE decoratorType):base(BASE_TYPE.DECORATOR, nodeTree, wr) {
            nodeType = decoratorType; 

            _imgStyle.normal.background = NodeEditor.SkinItem.GetDecoratorNodeImage(nodeType) as Texture2D;
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

            if(nodeType == NODE_TYPE.REPEATER)
                node = new ZBTRepeater(nodeTree, nodeRect);

            return node;
        }
	}
}