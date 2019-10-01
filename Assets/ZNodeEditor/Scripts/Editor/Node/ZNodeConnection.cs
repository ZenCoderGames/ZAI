//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeConnection
/// Purpose:  The class that maintains the connection between nodes.
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using WyrmTale;

namespace ZEditor {

	public class ZNodeConnection {
		ZNodeEditor _editor;
	    ZNodeConnector _inConnector, _outConnector;

	    Rect _rect;
	    GUIStyle _style;
	    System.Action<ZNodeConnection> _removeConnectionFunc;

	    public ZNodeConnection(ZNodeEditor editor, ZNodeConnector inConnector, ZNodeConnector outConnector, System.Action<ZNodeConnection> RemoveConnectionFunc) {
			_editor = editor;
			_inConnector = inConnector;
	        _outConnector = outConnector;
	        _removeConnectionFunc = RemoveConnectionFunc;

	        _style = new GUIStyle();
	        _style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
	        _style.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
	        _style.border = new RectOffset(4, 4, 12, 12);
	    }

		public void RefreshSkinParams() {
			if(_inConnector!=null)
				_inConnector.SetupSkinParams();
			if(_outConnector!=null)
				_outConnector.SetupSkinParams();
		}

	    public bool ContainsNode(ZNode node) {
	        return (_inConnector.Node == node) || (_outConnector.Node == node);
	    }

		public bool HasParentNode(ZNode node) {
			return _inConnector.Node == node;
		}

		public ZNode GetChildNode() {
			return _outConnector.Node;
		}

		public void Draw(float zoomLevel) {
			curveFromTo(_inConnector.GetRect(), _outConnector.GetRect(), Color.white, zoomLevel);

			Vector3 pos = (_inConnector.GetRect().center + _outConnector.GetRect().center) * 0.5f; 

			if(!_inConnector.Node.NodeTree.IsPositionInANode(pos)) {
				if (Handles.Button(pos, Quaternion.identity, _editor.SkinItem.connectionCircleSize * zoomLevel,
									_editor.SkinItem.connectionCirclePickSize, Handles.CircleHandleCap)) {
		            if(_removeConnectionFunc!=null)
		                _removeConnectionFunc(this);
		        }
			}
	    }

	    public void ClearLinksToConnectors() {
	        _inConnector.RemoveConnection(this);
	        _outConnector.RemoveConnection(this);
	    }

		void curveFromTo(Rect wr1, Rect wr2, Color color, float zoom) {
			bool isVerticalView = _editor.SkinItem.isVerticalLayout;
			if(isVerticalView) {
		        Handles.DrawBezier(
		            wr1.center,
		            wr2.center,
					wr1.center + Vector2.up * 50f * zoom,
					wr2.center - Vector2.up * 50f * zoom,
		            color, null, 2f);
			}
			else {
				Handles.DrawBezier(
					wr1.center,
					wr2.center,
					wr1.center + Vector2.right * 50f * zoom,
					wr2.center - Vector2.right * 50f * zoom,
					color, null, 2f);
			}
	    }

	    #region SERIALIZE
	    virtual public void Serialize(ref JSON connectionJS) {
			connectionJS["inNodeId"] = _inConnector.Node.NodeTree.GetNodeId(_inConnector.Node);
			connectionJS["outNodeId"] = _outConnector.Node.NodeTree.GetNodeId(_outConnector.Node);
	    }
	    #endregion
	}

}