//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeConnector
/// Purpose:  This class that represents the start/end connection points on a node
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ZEditor {

	public class ZNodeConnector {
	    public ZNode Node { get { return _parentNode; } }
	    ZNode _parentNode;
	    List<ZNodeConnection> _connections;

	    public enum TYPE { IN, OUT }
	    TYPE _type;
	    public bool IsTypeIn { get { return _type == TYPE.IN; } }
	    public bool IsTypeOut { get { return _type == TYPE.OUT; } }
		Rect _rect;
	    GUIStyle _defaultStyle, _selectedStyle, _filledStyle;

	    public bool IsSelected { get { return _isSelected; } }
	    bool _isSelected, _canBeConnectedTo;

	    public ZNodeConnector(ZNode node, TYPE type) {
			_type = type;
			_parentNode = node;

			SetupSkinParams();

	        _connections = new List<ZNodeConnection>();
		}

		public void SetupSkinParams() {
			_rect = new Rect(0, 0, _parentNode.NodeEditor.SkinItem.inConnectorSize, _parentNode.NodeEditor.SkinItem.inConnectorSize);
			Texture buttonStyleType = _parentNode.NodeEditor.SkinItem.defaultInConnector;
			Texture selectedButtonStyleType = _parentNode.NodeEditor.SkinItem.selectedInConnector;
			Texture filledStyleType = _parentNode.NodeEditor.SkinItem.filledInConnector;
			if(_type == TYPE.OUT) {
				buttonStyleType = _parentNode.NodeEditor.SkinItem.defaultOutConnector;
				selectedButtonStyleType = _parentNode.NodeEditor.SkinItem.selectedOutConnector;
				filledStyleType = _parentNode.NodeEditor.SkinItem.filledOutConnector;
				_rect = new Rect(0, 0, _parentNode.NodeEditor.SkinItem.outConnectorSize, _parentNode.NodeEditor.SkinItem.outConnectorSize);
			}

			_defaultStyle = ZEditorUtils.CreateGUIStyle(buttonStyleType);
			_selectedStyle = ZEditorUtils.CreateGUIStyle(selectedButtonStyleType);
			_filledStyle = ZEditorUtils.CreateGUIStyle(filledStyleType);
		}

	    #region CONNECTIONS
	    public bool CanTakeConnection() {
	        if(_parentNode.BaseType == ZNode.BASE_TYPE.ACTION)
	            return false;
	        else
	            return true;
	    }

		public void CheckToRemoveExistingConnection() {
			if(_connections.Count>0) {
				for(int i=0; i<_connections.Count; ++i) {
					_parentNode.NodeTree.RemoveConnection(_connections[i]);
				}
				_connections.Clear();
			}
		}

	    public void AddConnection(ZNodeConnection connection) {
	        _connections.Add(connection);
	    }

	    public void RemoveConnection(ZNodeConnection connection) {
	        _connections.Remove(connection);
	    }

		bool HasConnection() {
			return _connections.Count>0;
		}
	    #endregion

		public Rect GetRect() {
			float zoom = _parentNode.NodeTree.zoomLevel;
			Rect scaledRect = _rect.ScaleSizeBy(zoom);
			Rect parentRect = _parentNode.GetRect();
			float heightOffset = -_parentNode.NodeEditor.SkinItem.connectorHeightOffset * zoom;
			float xPos = 0;
			float yPos = 0;

			bool isVerticalView = _parentNode.NodeEditor.SkinItem.isVerticalLayout;
			if(isVerticalView) {
				xPos = parentRect.x + parentRect.width/2 - scaledRect.width/2;
				if(_type == TYPE.OUT)
					yPos = parentRect.y + (parentRect.height - (scaledRect.height/2 * 1/zoom) - heightOffset);
				else
					yPos = parentRect.y - (scaledRect.height/2 * zoom) + heightOffset/2;
			}
			else {
				yPos = parentRect.y + parentRect.height/2 - scaledRect.height/2;
				if(_type == TYPE.OUT)
					xPos = parentRect.x + (parentRect.width - (scaledRect.width/2 * 1/zoom) - heightOffset);
				else
					xPos = parentRect.x - (scaledRect.width/2 * zoom) + heightOffset/2;
			}

			return new Rect(new Vector2(xPos, yPos), scaledRect.size);
		}

		public void Draw() {
			GUIStyle style = _defaultStyle;
			if(_isSelected || _canBeConnectedTo)
				style = _selectedStyle;
			if(HasConnection())
				style = _filledStyle;

			GUI.Box(GetRect(), "", style);
		}

	    public void ProcessEvents(Event e) {
	        Rect currentRect = GetRect();

	        switch (e.type) {
	            case EventType.MouseDown:
	                if(IsTypeOut) {
	                    if (e.button == 0) {
	                        if (currentRect.Contains(e.mousePosition)) {
	                            _isSelected = true;
	                            _parentNode.NodeEditor.SetStartConnection(this);
								e.Use();
	                        }
							GUI.changed = true;
	                    }
	                }
	                break;

	            case EventType.MouseUp:
	                if(IsTypeIn) {
	                    if (e.button == 0) {
	                        if (currentRect.Contains(e.mousePosition)) {
								_parentNode.NodeEditor.SetEndConnection(this);
	                        }
	                    }
	                }
	                _isSelected = false;
	                _canBeConnectedTo = false;
	                break;

	            case EventType.MouseDrag:
	                if(IsTypeIn) {
	                    if (e.button == 0) {
	                        if (currentRect.Contains(e.mousePosition)) {
	                            if(_parentNode.NodeEditor.HasSelectedConnector()) {
	                                _canBeConnectedTo = true;
								}
	                        }
	                        else {
	                            _canBeConnectedTo = false;
							}
	                    }
	                }
	                break;
	        }

	        if(_isSelected) {
				bool isVerticalView = _parentNode.NodeEditor.SkinItem.isVerticalLayout;
				if(isVerticalView) {
		            Handles.DrawBezier(
		                currentRect.center,
		                e.mousePosition,
		                currentRect.center + Vector2.up * 50f,
		                e.mousePosition - Vector2.up * 50f,
		                Color.white, null, 2f);
				}
				else {
					Handles.DrawBezier(
						currentRect.center,
						e.mousePosition,
						currentRect.center + Vector2.right * 50f,
						e.mousePosition - Vector2.right * 50f,
						Color.white, null, 2f);
				}
	            GUI.changed = true;
	        }
	    }
	}

}