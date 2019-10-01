//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNode
/// Purpose:  The base class for all nodes in the Node Editor. Has both core and btree types.
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using WyrmTale;

namespace ZEditor {

	public class ZNode {
		Rect _windowRect, _scaledWindowRect;

		public ZNodeEditor NodeEditor { get { return _nodeTree.NodeEditor; } }
		public ZNodeTree NodeTree { get { return _nodeTree; } }
		ZNodeTree _nodeTree;

	    List<ZNodeConnector> _connectorList;
	    ZNodeConnector _inConnector, _outConnector;
	    public ZNodeConnector InConnector { get { return _inConnector; } }
	    public ZNodeConnector OutConnector { get { return _outConnector; } }

		protected GUIStyle _defaultStyle, _selectedStyle, _xButtonStyle, _imgStyle;
		bool _isDragging, _isSelected;
	    Vector2 _dragStartPosition;

		List<ZNode> _children;

		public string Name { get; set; }
		protected string _inspectorName;

		public enum BASE_TYPE { ROOT, DECORATOR, COMPOSITE, ACTION, SUBTREE }
	    public BASE_TYPE BaseType;

        public ZNode(BASE_TYPE type, ZNodeTree nodeTree, Rect wr) {
	        BaseType = type;
			_nodeTree = nodeTree;
			_windowRect = wr;
			Name = "";

			SetupSkinParams();

	        _connectorList = new List<ZNodeConnector>();
	        CreateInConnector();
	        CreateOutConnector();
		}

		public void SetupSkinParams() {
			_defaultStyle = new GUIStyle();
			_defaultStyle.normal.background = NodeEditor.SkinItem.defaultNode as Texture2D;
			_defaultStyle.border = new RectOffset(4, 4, 12, 12);
			_defaultStyle.alignment = TextAnchor.MiddleCenter;
			_defaultStyle.fontStyle = FontStyle.Bold;

			_selectedStyle = new GUIStyle();
			_selectedStyle.normal.background = NodeEditor.SkinItem.selectedNode as Texture2D;
			_selectedStyle.border = new RectOffset(4, 4, 12, 12);
			_selectedStyle.alignment = TextAnchor.MiddleCenter;
			_selectedStyle.fontStyle = FontStyle.Bold;

			_xButtonStyle = new GUIStyle();
			_xButtonStyle.normal.background = NodeEditor.SkinItem.xButton as Texture2D;

			_imgStyle = new GUIStyle();
			/*if(BTNodeType == ZBTNodeManager.BT_NODE_TYPE.NONE) {
				_imgStyle.normal.background = NodeEditor.SkinItem.GetCoreNodeImage(CoreType) as Texture2D;
				_inspectorName = CoreType.ToString();
			}
			else {
				_imgStyle.normal.background = NodeEditor.SkinItem.GetBTNodeImage(BTNodeType) as Texture2D;
				_inspectorName = NodeEditor.SkinItem.GetBTNodeInspectorName(BTNodeType);
			}*/
		}

		public void SetupSkinNodeSize(float width, float height) {
			_windowRect.Set(_windowRect.x, _windowRect.y, width, height);
		}

		public void SetPosition(float x, float y) {
			_windowRect.Set(x, y, _windowRect.width, _windowRect.height);
		}

		public void AssignChildren(List<ZNode> children) {
			_children = children;
		}

		public List<ZNode> GetChildren() {
			return _children;
		}

	    protected virtual void CreateInConnector() {
	        _inConnector = new ZNodeConnector(this, ZNodeConnector.TYPE.IN);
	        _connectorList.Add(_inConnector);
	    }

	    protected virtual void CreateOutConnector() {
	        _outConnector = new ZNodeConnector(this, ZNodeConnector.TYPE.OUT);
	        _connectorList.Add(_outConnector);
	    }

		public void Draw() {
			float zoom = _nodeTree.zoomLevel;
			_scaledWindowRect = _windowRect.ScaleSizeBy(_nodeTree.zoomLevel);

	        for(int i=0; i<_connectorList.Count; ++i) {
	            _connectorList[i].Draw();
	        }

			bool isSelected = _isSelected || _nodeTree.IsNodeInSelectedGroup(this);
			GUI.Box(_scaledWindowRect, "", isSelected?_selectedStyle:_defaultStyle);

			float imgPct = NodeEditor.SkinItem.nodeImgSize;
			float imgStartX = _scaledWindowRect.width * (1-imgPct)/2;
			float imgStartY = _scaledWindowRect.height * (1-imgPct)/2;
			Rect imgRect = new Rect(_scaledWindowRect.x + imgStartX, _scaledWindowRect.y + imgStartY, _scaledWindowRect.width * imgPct, _scaledWindowRect.height * imgPct);
			if(!NodeEditor.SkinItem.isVerticalLayout)
				GUIUtility.RotateAroundPivot(-90, imgRect.center);
			GUI.Box(imgRect, "", _imgStyle);
			if(!NodeEditor.SkinItem.isVerticalLayout)
				GUIUtility.RotateAroundPivot(90, imgRect.center);

			if(_isSelected && BaseType != BASE_TYPE.ROOT) {
				Rect deleteNodeRect = new Rect(_scaledWindowRect.x, _scaledWindowRect.y, _nodeTree.NodeEditor.SkinItem.xIconSize * zoom, _nodeTree.NodeEditor.SkinItem.xIconSize * zoom);
				if(GUI.Button(deleteNodeRect, "", _xButtonStyle)) {
					DeleteNode();
				}
			}
		}

		virtual public void DrawInInspector(GUIStyle guiStyle) {
			GUILayout.Space(10);
			GUILayout.BeginHorizontal("");
			GUILayout.TextArea("Node Id:", guiStyle);
			Name = GUILayout.TextField(Name, EditorStyles.textField);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal("");
			GUILayout.TextArea("Node Type:", guiStyle);
			GUILayout.TextArea(_inspectorName, EditorStyles.textArea);
			GUILayout.EndHorizontal();
		}

		public Rect GetOriginalRect() {
			return _windowRect;
		}

		public Rect GetRect() {
			return _scaledWindowRect;
		}

		Rect ClampToScreen(Rect r) {
			r.x = Mathf.Clamp(r.x,0,_nodeTree.NodeEditor.SkinItem.mainPanelWidth-r.width); 
			r.y = Mathf.Clamp(r.y,0,_nodeTree.NodeEditor.SkinItem.mainPanelHeight-r.height); 
			return r; 
		}

		public bool OverflowEdgeCheck(Vector2 delta) {
			if(_scaledWindowRect.x+delta.x<0)
				return true;

			if(_scaledWindowRect.x+delta.x>_nodeTree.NodeEditor.SkinItem.mainPanelWidth-_scaledWindowRect.width)
				return true;

			if(_scaledWindowRect.y+delta.y<0)
				return true;

			if(_scaledWindowRect.y+delta.y>_nodeTree.NodeEditor.SkinItem.mainPanelHeight-_scaledWindowRect.height)
				return true;

			return false;
		}

	    public void ProcessEvents(Event e) {
	        for(int i=0; i<_connectorList.Count; ++i) {
	            _connectorList[i].ProcessEvents(e);
	        }

			switch (e.type) {
	            case EventType.MouseDown:
	                if(_isSelected) {
						NodeEditor.DeselectNode(this);
	                }
					_isSelected = false;
					_isDragging = false;
	    			if (e.button == 0) {
						if (_scaledWindowRect.Contains(e.mousePosition)) {
	                        if(_outConnector==null || !_outConnector.IsSelected) {
	                            _isDragging = true;
	                            GUI.changed = true;
	                            _dragStartPosition = e.mousePosition;
								if(!_nodeTree.IsNodeInSelectedGroup(this)) {
									_nodeTree.ClearSelectedGroup();
								}
	                        }
							e.Use();
	    				}
	    			}
	    			break;

	    		case EventType.MouseUp:
	                if(_isDragging) {
	                    if (e.button == 0) {
							if (_scaledWindowRect.Contains(e.mousePosition)) {
	                            if(Vector2.Distance(_dragStartPosition, e.mousePosition)<10) {
	                                _isSelected = true;
									NodeTree.DeselectOtherNodes(this);
									NodeEditor.SelectNode(this);
	                            }
								e.Use();
								NodeEditor.DragNodeCompleted();
	                        }
	                    }
	                }
	                _isDragging = false;
	    			break;

	    		case EventType.MouseDrag:
	    		    if (e.button == 0 && _isDragging) {
						if(_nodeTree.IsNodeInSelectedGroup(this))
							_nodeTree.DragSelectedNodes(e.delta);
						else
							Drag(e.delta);
						e.Use();
	    			}
	    			break;
			}
		}

		public void Reset() {
			_isSelected = false;
			_isDragging = false;
		}

	    void DeleteNode() {
	        NodeEditor.RemoveNode(this);
	    }

		public void Drag(Vector2 delta) {
			_windowRect.position += delta * 1/_nodeTree.zoomLevel;
			_windowRect = ClampToScreen(_windowRect);
		}

	    #region SERIALIZE
	    virtual public void Serialize(ref JSON nodeJS) {
			nodeJS["id"] = Name;
	        nodeJS["baseType"] = BaseType.ToString();
	        nodeJS["posX"] = _windowRect.position.x;
	        nodeJS["posY"] = _windowRect.position.y;
	    }
	    #endregion
	}

}