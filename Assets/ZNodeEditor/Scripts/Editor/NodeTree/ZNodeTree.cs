//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeTree
/// Purpose:  This class represents everything in a Node Editor tab and the tree representation of nodes.
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using WyrmTale;

namespace ZEditor {

	public class ZNodeTree {
		public ZNodeEditor NodeEditor { get { return _nodeEditor; } }
		ZNodeEditor _nodeEditor;

		string _name, _filePath;
		List<ZNode> _nodes, _selectedNodes;
		ZNode _rootNode;
		List<ZNodeConnection> _connections;
		Vector2 _scrollView;

		public float zoomLevel = 0.75f;

		public ZNodeTree(ZNodeEditor editor) {
			_nodeEditor = editor;
			_name = "New Tree";
			_filePath = "";
			_nodes = new List<ZNode>();
			_selectedNodes = new List<ZNode>();
			_connections = new List<ZNodeConnection>();

			CreateRoot(_nodeEditor.SkinItem.rootPositionX, _nodeEditor.SkinItem.rootPositionY);
		}

		public void RefreshSkin() {
			for(int i=0; i<_nodes.Count; ++i) {
				_nodes[i].SetupSkinParams();
				_nodes[i].SetupSkinNodeSize(_nodeEditor.SkinItem.nodeWidth, _nodeEditor.SkinItem.nodeHeight);
			}
			for(int i=0; i<_connections.Count; ++i) {
				_connections[i].RefreshSkinParams();
			}
		}

		public void Update(Vector2 scrollView) {
			_scrollView = scrollView;
			DrawConnections();
			DrawNodes();
			ProcessEvents(Event.current);
		}

		#region NODES
		void DrawNodes() {
			for(int i=0; i<_nodes.Count; ++i) {
				_nodes[i].Draw();
			}
		}

		ZNode CreateRoot(float posX, float posY) {
			_rootNode = new ZNodeRoot(this, new Rect(posX, posY, _nodeEditor.SkinItem.nodeWidth, _nodeEditor.SkinItem.nodeHeight));
			_nodes.Add(_rootNode);
			return _rootNode;
		}

		public ZNode CreateBaseNode(ZNode.BASE_TYPE type, float posX, float posY, JSON js) {
			Rect nodeRect = new Rect(posX, posY, _nodeEditor.SkinItem.nodeWidth, _nodeEditor.SkinItem.nodeHeight);
			ZNode node = null;
			if(type == ZNode.BASE_TYPE.SUBTREE)
				node = new ZNodeSubTree(this, nodeRect, js);
			_nodes.Add(node);
			return node;
		}

        public ZNode CreateCompositeNode(ZNodeComposite.NODE_TYPE nodeType, float posX, float posY, JSON js) {
            Rect nodeRect = new Rect(posX, posY, _nodeEditor.SkinItem.nodeWidth, _nodeEditor.SkinItem.nodeHeight);
            ZNode node = ZNodeComposite.CreateNode(nodeType, this, nodeRect, js);
            if(node!=null)
                _nodes.Add(node);
            return node;
        }

        public ZNode CreateDecoratorNode(ZNodeDecorator.NODE_TYPE nodeType, float posX, float posY, JSON js) {
            Rect nodeRect = new Rect(posX, posY, _nodeEditor.SkinItem.nodeWidth, _nodeEditor.SkinItem.nodeHeight);
            ZNode node = ZNodeDecorator.CreateNode(nodeType, this, nodeRect, js);
            if(node!=null)
                _nodes.Add(node);
            return node;
        }

        public ZNode CreateActionNode(ZBTActionManager.NODE_TYPE actionType, float posX, float posY, JSON js) {
			Rect nodeRect = new Rect(posX, posY, _nodeEditor.SkinItem.nodeWidth, _nodeEditor.SkinItem.nodeHeight);
            ZNode node = ZBTActionManager.CreateNode(actionType, this, nodeRect, js);
            if(node!=null)
			    _nodes.Add(node);
			return node;
		}

		public void RemoveNode(ZNode node) {
			_nodes.Remove(node);
			for(int i=0; i<_connections.Count; ++i) {
				if(_connections[i].ContainsNode(node)) {
					RemoveConnection(_connections[i]);
					i--;
				}
			}
		}

		public ZNode GetNode(int id) {
			for(int i=0; i<_nodes.Count; ++i) {
				if(i==id)
					return _nodes[i];
			}

			Debug.Log(string.Concat("Unable to find NodeId: ", id.ToString()));
			return null;
		}

		public int GetNodeId(ZNode node) {
			for(int i=0; i<_nodes.Count; ++i) {
				if(_nodes[i]==node)
					return i;
			}

			return -1;
		}
		#endregion

		#region EVENTS
		bool _isDragging;
		Vector2 _dragStartPos;
		private void ProcessEvents(Event e) {
			if(_nodes!=null) {
				for(int i=_nodes.Count-1; i>=0; --i) {
					_nodes[i].ProcessEvents(e);
				}
			}

			switch (e.type) {
			case EventType.MouseDown:
				_isDragging = true;
				_dragStartPos = e.mousePosition;
				ClearSelectedGroup();
				break;

			case EventType.MouseUp:
				_isDragging = false;
	 	
				float startX = _dragStartPos.x;
				float startY = _dragStartPos.y;
				float selWidth = e.mousePosition.x - _dragStartPos.x;
				float selHeight = e.mousePosition.y - _dragStartPos.y;
				Rect selectionRect = new Rect(startX, startY, selWidth, selHeight);
				for(int i=0; i<_nodes.Count; ++i) {
					if(selectionRect.Contains(_nodes[i].GetRect().center, true)) {
						_selectedNodes.Add(_nodes[i]);
					}
				}
				break;
			}

			if (_isDragging) {
				DragForSelection(e.mousePosition);
			}
		}
		#endregion

		#region NODE_SELECTION
		public void DragForSelection(Vector2 currentPos) {
			float dragX = currentPos.x - _dragStartPos.x;
			float dragY = currentPos.y - _dragStartPos.y;
			GUI.Box(new Rect(_dragStartPos.x, _dragStartPos.y, dragX, dragY), ""); 
			GUI.changed = true;
		}

		public bool IsNodeInSelectedGroup(ZNode node) {
			return _selectedNodes.Contains(node);
		}

		public void ClearSelectedGroup() {
			_selectedNodes.Clear();
		}

		public void DragSelectedNodes(Vector2 delta) {
			bool hasAnyNodeReachedEdge = false;
			for(int i=0; i<_selectedNodes.Count; ++i) {
				if(_selectedNodes[i].OverflowEdgeCheck(delta))
					return;
			}

			for(int i=0; i<_selectedNodes.Count; ++i) {
				_selectedNodes[i].Drag(delta);
			}
		}

		public void DeselectOtherNodes(ZNode node) {
			for(int i=0; i<_nodes.Count; ++i) {
				if(_nodes[i]!=node) {
					_nodes[i].Reset();
				}
			}
		}
		#endregion

		#region CONNECTIONS
		public void CreateConnection(ZNodeConnector inConnector, ZNodeConnector outConnector) {
			ZNodeConnection newConnection = new ZNodeConnection(_nodeEditor, inConnector, outConnector, RemoveConnection);
			_connections.Add(newConnection);

			// Can only have one outgoing connection for a root/single child connector
			if(inConnector.Node.BaseType == ZNode.BASE_TYPE.ROOT || inConnector.Node.BaseType == ZNode.BASE_TYPE.DECORATOR) {
				inConnector.CheckToRemoveExistingConnection();
			}
			inConnector.AddConnection(newConnection);

			// Can only have one incoming node for an out connector
			outConnector.CheckToRemoveExistingConnection();
			outConnector.AddConnection(newConnection);

			if(_nodeEditor.SkinItem.autoLayoutAsDefault)
				AutoLayout();
		}

		void DrawConnections() {
			for(int i=0; i<_connections.Count; ++i) {
				_connections[i].Draw(zoomLevel);
			}
		}

		public void RemoveConnection(ZNodeConnection connection) {
			connection.ClearLinksToConnectors();
			_connections.Remove(connection);

			if(_nodeEditor.SkinItem.autoLayoutAsDefault)
				AutoLayout();
		}
		#endregion

		#region SERIALIZATION
		public void Load(JSON js, string filePath) {
			ClearAll();

			SetFilePath(filePath);
			_name = js.ToString("name");

			JSON[] nodeListJS = js.ToArray<JSON>("bTree");
			for(int i=0; i<nodeListJS.Length; ++i) {
				float posX = nodeListJS[i].ToFloat("posX");
				float posY = nodeListJS[i].ToFloat("posY");
                ZNode.BASE_TYPE baseType = ZEditorUtils.ParseEnum<ZNode.BASE_TYPE>(nodeListJS[i].ToString("baseType"));
				ZNode newNode = null;
                if(baseType == ZNode.BASE_TYPE.ROOT) {
                    newNode = CreateRoot(posX, posY);
                }
                else if(baseType == ZNode.BASE_TYPE.SUBTREE) {
                    newNode = CreateBaseNode(baseType, posX, posY, nodeListJS[i]);
                }
                else if(baseType == ZNode.BASE_TYPE.COMPOSITE) {
                    ZNodeComposite.NODE_TYPE nodeType = ZEditorUtils.ParseEnum<ZNodeComposite.NODE_TYPE>(nodeListJS[i].ToString("nodeType"));
                    newNode = CreateCompositeNode(nodeType, posX, posY, nodeListJS[i]);
                }
                else if(baseType == ZNode.BASE_TYPE.DECORATOR) {
                    ZNodeDecorator.NODE_TYPE nodeType = ZEditorUtils.ParseEnum<ZNodeDecorator.NODE_TYPE>(nodeListJS[i].ToString("nodeType"));
                    newNode = CreateDecoratorNode(nodeType, posX, posY, nodeListJS[i]);
                }
                else if(baseType == ZNode.BASE_TYPE.ACTION) {
                    ZBTActionManager.NODE_TYPE nodeType = ZEditorUtils.ParseEnum<ZBTActionManager.NODE_TYPE>(nodeListJS[i].ToString("nodeType"));
                    newNode = CreateActionNode(nodeType, posX, posY, nodeListJS[i]);
				}
				newNode.Name = nodeListJS[i].ToString("id");
			}

			JSON[] connectionListJS = js.ToArray<JSON>("connections");
			for(int i=0; i<connectionListJS.Length; ++i) {
				int inId = connectionListJS[i].ToInt("inNodeId");
				int outId = connectionListJS[i].ToInt("outNodeId");
				ZNodeConnector inConnector = GetNode(inId).OutConnector;
				ZNodeConnector outConnector = GetNode(outId).InConnector;
				if(inConnector!=null && outConnector!=null) {
					CreateConnection(inConnector, outConnector);
				}
				else {
					Debug.Log("Unable to find in or out connector for node: " + inId.ToString() + " " + outId.ToString());
				}
			}
		}

		public JSON Save() {
			JSON treeJS = new JSON();

			treeJS["name"] = _name;

			JSON[] nodeListJS = new JSON[_nodes.Count];
			for(int i=0; i<_nodes.Count; ++i) {
				JSON newNode = new JSON();
				_nodes[i].Serialize(ref newNode);
				nodeListJS[i] = newNode;
			}
			treeJS["bTree"] = nodeListJS;

			JSON[] connectionsListJS = new JSON[_connections.Count];
			for(int i=0; i<_connections.Count; ++i) {
				JSON newConnection = new JSON();
				_connections[i].Serialize(ref newConnection);
				connectionsListJS[i] = newConnection;
			}
			treeJS["connections"] = connectionsListJS;

			return treeJS;
		}
		#endregion

		void ClearAll() {
			_nodes.Clear();
			_connections.Clear();
		}

		public void Reset() {
			ClearAll();
			CreateRoot(_nodeEditor.SkinItem.rootPositionX, _nodeEditor.SkinItem.rootPositionY);
		}

		public void ResetNodes() {
			for(int i=0; i<_nodes.Count; ++i) {
				_nodes[i].Reset();
			}
		}

		// Converts the nodes into a linked list representation of the tree
		void CacheChildren() {
			for(int i=0; i<_nodes.Count; ++i) {
				_nodes[i].AssignChildren(GetChildren(_nodes[i]));
			}
		}

		#region AUTO_LAYOUT
		public void AutoLayout() {
			CacheChildren();

			if(_nodeEditor.SkinItem.isVerticalLayout)
				DoAutoArrangeVertical(_rootNode);
			else
				DoAutoArrangeHorizontal(_rootNode);
		}

		void DoAutoArrangeVertical(ZNode parentNode) {
			float parentX = parentNode.GetOriginalRect().x;
			float parentY = parentNode.GetOriginalRect().y;
			List<ZNode> children = parentNode.GetChildren();
			int childCount = children.Count;
			// Case 1: no children
			if(childCount==0) {
				return;
			}
			// Case 2: one child
			if(childCount<=1) {
				children[0].SetPosition(parentX, parentY + _nodeEditor.SkinItem.autoLayoutHeight);
				DoAutoArrangeVertical(children[0]);
				return;
			}

			// Case 3: multiple children
			bool isEvenTree = (childCount%2==0);
			int mid = childCount/2;
			if(!isEvenTree) {
				// middle child of this tree stays under parent position
				children[mid].SetPosition(parentX, parentY + _nodeEditor.SkinItem.autoLayoutHeight);
				DoAutoArrangeVertical(children[mid]);
			}

			// process left children
			int i = mid-1;
			while(i>=0) {
				float startX = parentX;
				if(i<mid-1)
					startX = children[i+1].GetOriginalRect().x;
				
				float x = startX - _nodeEditor.SkinItem.autoLayoutWidth;
				// Check my rightmost child
				float rightIndex = 0;
				UpdateRightOffsetCount(children[i], ref rightIndex);
				x -= rightIndex;

				if((isEvenTree && i<mid-1) || (!isEvenTree)) {
					// Check my right neighbors left most child
					ZNode rightNbr = children[i+1];
					float rightNbrLeftIndex = 0;
					UpdateLeftOffsetCount(rightNbr, ref rightNbrLeftIndex);
					x -= rightNbrLeftIndex;
				}

				children[i].SetPosition(x, parentY + _nodeEditor.SkinItem.autoLayoutHeight);
				DoAutoArrangeVertical(children[i]);

				i--;
			}

			// process right children
			i = isEvenTree?mid:mid+1;
			while(i<children.Count) {
				float startX = parentX;
				if((isEvenTree && i>mid) || (!isEvenTree && i>mid+1))
					startX = children[i-1].GetOriginalRect().x;

				float x = startX + _nodeEditor.SkinItem.autoLayoutWidth;

				// Shift to the right based on left most child (recursive check)
				float leftIndex = 0;
				UpdateLeftOffsetCount(children[i], ref leftIndex);
				x += leftIndex;

				if((isEvenTree && i>mid) || (!isEvenTree)) {
					// Check left neighbor's rightmost child for another right shift for spacing
					ZNode leftNbr = children[i-1];
					float leftNbrLeftIndex = 0;
					UpdateRightOffsetCount(leftNbr, ref leftNbrLeftIndex);
					x += leftNbrLeftIndex;
				}

				children[i].SetPosition(x, parentY + _nodeEditor.SkinItem.autoLayoutHeight);
				DoAutoArrangeVertical(children[i]);

				i++;
			}
		}

		void DoAutoArrangeHorizontal(ZNode parentNode) {
			float parentX = parentNode.GetOriginalRect().x;
			float parentY = parentNode.GetOriginalRect().y;
			List<ZNode> children = parentNode.GetChildren();
			int childCount = children.Count;
			// Case 1: no children
			if(childCount==0) {
				return;
			}
			// Case 2: one child
			if(childCount<=1) {
				children[0].SetPosition(parentX + _nodeEditor.SkinItem.autoLayoutHeight, parentY);
				DoAutoArrangeHorizontal(children[0]);
				return;
			}

			// Case 3: multiple children
			bool isEvenTree = (childCount%2==0);
			int mid = childCount/2;
			if(!isEvenTree) {
				// middle child of this tree stays under parent position
				children[mid].SetPosition(parentX + _nodeEditor.SkinItem.autoLayoutHeight, parentY);
				DoAutoArrangeHorizontal(children[mid]);
			}

			// process left children
			int i = mid-1;
			while(i>=0) {
				float startY = parentY;
				if(i<mid-1)
					startY = children[i+1].GetOriginalRect().y;

				float y = startY - _nodeEditor.SkinItem.autoLayoutWidth;
				// Check my rightmost child
				float rightIndex = 0;
				UpdateRightOffsetCount(children[i], ref rightIndex);
				y -= rightIndex;

				if((isEvenTree && i<mid-1) || (!isEvenTree)) {
					// Check my right neighbors left most child
					ZNode rightNbr = children[i+1];
					float rightNbrLeftIndex = 0;
					UpdateLeftOffsetCount(rightNbr, ref rightNbrLeftIndex);
					y -= rightNbrLeftIndex;
				}

				children[i].SetPosition(parentX + _nodeEditor.SkinItem.autoLayoutHeight, y);
				DoAutoArrangeHorizontal(children[i]);

				i--;
			}

			// process right children
			i = isEvenTree?mid:mid+1;
			while(i<children.Count) {
				float startY = parentY;
				if((isEvenTree && i>mid) || (!isEvenTree && i>mid+1))
					startY = children[i-1].GetOriginalRect().y;

				float y = startY + _nodeEditor.SkinItem.autoLayoutWidth;

				// Shift to the right based on left most child (recursive check)
				float leftIndex = 0;
				UpdateLeftOffsetCount(children[i], ref leftIndex);
				y += leftIndex;

				if((isEvenTree && i>mid) || (!isEvenTree)) {
					// Check left neighbor's rightmost child for another right shift for spacing
					ZNode leftNbr = children[i-1];
					float leftNbrLeftIndex = 0;
					UpdateRightOffsetCount(leftNbr, ref leftNbrLeftIndex);
					y += leftNbrLeftIndex;
				}

				children[i].SetPosition(parentX + _nodeEditor.SkinItem.autoLayoutHeight, y);
				DoAutoArrangeHorizontal(children[i]);

				i++;
			}
		}

		List<ZNode> GetChildren(ZNode parentNode) {
			List<ZNode> children = new List<ZNode>();
			for(int i=0; i<_connections.Count; ++i) {
				if(_connections[i].HasParentNode(parentNode)) {
					children.Add(_connections[i].GetChildNode());
				}
			}
			if(_nodeEditor.SkinItem.isVerticalLayout)
				children.Sort(SortByX);
			else
				children.Sort(SortByY);
			
			return children;
		}

		int SortByX(ZNode n1, ZNode n2) {
			return n1.GetOriginalRect().x.CompareTo(n2.GetOriginalRect().x);
		}

		int SortByY(ZNode n1, ZNode n2) {
			return n1.GetOriginalRect().y.CompareTo(n2.GetOriginalRect().y);
		}

		void UpdateLeftOffsetCount(ZNode leftChildNode, ref float leftIndex) {
			List<ZNode> children = leftChildNode.GetChildren();
			int childCount = children.Count;
			if(childCount>0) {
				if(childCount>1) {
					leftIndex += (childCount/2) * _nodeEditor.SkinItem.autoLayoutWidth;
				}
						
				UpdateRightOffsetCount(children[0], ref leftIndex);
				UpdateLeftOffsetCount(children[0], ref leftIndex);
			}
		}

		void UpdateRightOffsetCount(ZNode rightChildNode, ref float rightIndex) {
			List<ZNode> children = rightChildNode.GetChildren();
			int childCount = children.Count;
			if(childCount>0) {
				if(childCount>1) {
					rightIndex += (childCount/2) * _nodeEditor.SkinItem.autoLayoutWidth;
				}

				UpdateLeftOffsetCount(children[children.Count-1], ref rightIndex);
				UpdateRightOffsetCount(children[children.Count-1], ref rightIndex);
			}
		}
		#endregion

		#region NAME
		public string GetName() {
			return _name;
		}

		public void SetName(string name) {
			_name = name;
		}
		#endregion

		#region FILE_PATH
		public string GetFilePath() {
			return _filePath;
		}

		public void SetFilePath(string filePath) {
			_filePath = filePath;
		}
		#endregion

		#region HELPERS
		public Vector2 GetScrollView() {
			return _scrollView;
		}

		public bool IsPositionInANode(Vector3 pos) {
			for(int i=0; i<_nodes.Count; ++i) {
				if(_nodes[i].GetRect().Contains(pos))
					return true;
			}

			return false;
		}
		#endregion
	}

}