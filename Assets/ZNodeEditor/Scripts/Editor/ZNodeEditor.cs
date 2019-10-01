//////////////////////////////////////////////////////////////////////////////////////////////////
/// Class: 	  ZNodeEditor
/// Purpose:  The base editor class that initiates the inspector, the node editor windows and tabs
/// Author:   Srinavin Nair
//////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using WyrmTale;

namespace ZEditor {
	
	public class ZNodeEditor:EditorWindow {
		Vector2 _nodeScrollView = Vector2.zero;

		ZNodeCreator _nodeCreator;
	    ZNodeInspector _inspector;

		ZNode _selectedNode;
		ZNodeConnector _selectedConnector;

		List<ZNodeTree> _nodeTrees;
		ZNodeTree _currentNodeTree;
		GUIStyle _xStyle, _plusStyle, _zoomInStyle, _zoomOutStyle;
		GUIStyle _leftArrowInactiveStyle, _leftArrowActiveStyle, _rightArrowInactiveStyle, _rightArrowActiveStyle;
		float _currentTabStartX;

	    public event System.Action<ZNode> OnNodeSelected;

		float _minZoom = 0.35f;
		float _maxZoom = 1.0f;
		float _currentZoom = 0.75f;

		ZNodeEditorSkin _editorSkin;

		public ZNodeEditorSkin.ZNodeEditorSkinItem SkinItem { get { return _editorSkinItem; } }
		ZNodeEditorSkin.ZNodeEditorSkinItem _editorSkinItem;

		public static string EDITOR_SKIN_PATH = "Assets/ZNodeEditor/ZNodeEditorSkin.asset";

		[MenuItem("ZTools/ZNodeEditor/Create Skin Definition")]
		static void CreateView() {
			ZEditorUtils.CreateScriptableObject<ZNodeEditorSkin>(EDITOR_SKIN_PATH);
		}

	    [MenuItem("ZTools/ZNodeEditor/Open Editor")]
	    static void Init() {
			ZNodeEditor window = (ZNodeEditor)EditorWindow.GetWindow<ZNodeEditor> ("ZNodeEditor", true, typeof(SceneView));
			window.Show();

			window.Initialize();
	    }

		public void Initialize() {
			_editorSkin = EditorGUIUtility.Load(EDITOR_SKIN_PATH) as ZNodeEditorSkin;
			if(_editorSkin==null) {
				CreateView();
				_editorSkin = EditorGUIUtility.Load(EDITOR_SKIN_PATH) as ZNodeEditorSkin;
			}
			_editorSkinItem = _editorSkin.GetDefaultSkinItem();
			_editorSkin.OnModified += OnSkinModified;

			_inspector = new ZNodeInspector(this);
			_nodeCreator = new ZNodeCreator(this);

			_nodeTrees = new List<ZNodeTree>();

			_currentTabStartX = 0;

			SetupSkinParams();
		}

		void OnDestroy() {
			if(_editorSkin!=null)
				_editorSkin.OnModified -= OnSkinModified;
		}

		void OnSkinModified() {
			SetupSkinParams();
		}

		void SetupSkinParams() {
			_xStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.x);
			_plusStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.plus);
			_leftArrowInactiveStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.leftArrowInactive);
			_leftArrowActiveStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.leftArrowActive);
			_rightArrowInactiveStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.rightArrowInactive);
			_rightArrowActiveStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.rightArrowActive);
			_zoomInStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.zoomIn);
			_zoomOutStyle = ZEditorUtils.CreateGUIStyle(_editorSkinItem.zoomOut);

			_minZoom = _editorSkinItem.minZoom;
			_maxZoom = _editorSkinItem.maxZoom;
			_currentZoom = _editorSkinItem.startZoom;

			if(_currentNodeTree!=null) {
				_currentNodeTree.RefreshSkin();
				if(_editorSkinItem.autoLayoutAsDefault)
					_currentNodeTree.AutoLayout();
			}
		}

		void CreateNewTree() {
			ResetBeforeTabSwitch();
			_currentNodeTree = new ZNodeTree(this);
			_nodeTrees.Add(_currentNodeTree);
			_nodeScrollView.Set(0,0);
		}

	    void OnGUI() {
			if(_inspector==null)
				Initialize();

			// Top Menu
			GUILayout.BeginArea(new Rect(5,0,SkinItem.leftPanelWidth-10,80));
	        DrawTopMenu();
			GUILayout.EndArea();

			// Left Panel
			GUILayout.BeginArea(new Rect(0,75,SkinItem.leftPanelWidth,SkinItem.leftPanelHeight));
			if(_currentNodeTree!=null) {
				_inspector.Draw();
				_nodeCreator.Draw();
			}
			GUILayout.EndArea();

			// Main Panel
			GUI.Box(new Rect(SkinItem.leftPanelWidth-2,0,position.width-SkinItem.leftPanelWidth+2,position.height), "");

			// Tab Panel
			GUILayout.BeginArea(new Rect(SkinItem.leftPanelWidth,0,SkinItem.mainPanelWidth,SkinItem.tabHeight));
			GUILayout.BeginArea(new Rect(SkinItem.tabHeight,0,position.width-(SkinItem.leftPanelWidth+SkinItem.tabHeight*2),SkinItem.tabHeight));
			DrawTabs();
			GUILayout.EndArea();
			DrawTabArrows();
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(SkinItem.leftPanelWidth,SkinItem.tabHeight,SkinItem.mainPanelWidth * _currentZoom,SkinItem.mainPanelHeight * _currentZoom));
			DrawBackground();

			if(_currentNodeTree!=null) {
				_currentNodeTree.zoomLevel = _currentZoom;

				_nodeScrollView = GUI.BeginScrollView (
					new Rect (0, 0, position.width - SkinItem.leftPanelWidth, position.height - SkinItem.tabHeight),
					_nodeScrollView,
					new Rect (0, 0, SkinItem.mainPanelWidth * _currentZoom, SkinItem.mainPanelHeight * _currentZoom)
				);

				_currentNodeTree.Update(_nodeScrollView);

				GUI.EndScrollView ();

			}
			GUILayout.EndArea();

			if(_currentNodeTree!=null) {
				if(_currentZoom<_maxZoom) {
					Rect zoomInNodeRect = new Rect(SkinItem.leftPanelWidth, SkinItem.tabHeight + 5, SkinItem.tabHeight*1.5f, SkinItem.tabHeight*1.5f);
					if(GUI.RepeatButton(zoomInNodeRect, "", _zoomInStyle)) {
						_currentZoom += 0.005f;
						if(_currentZoom>_maxZoom)
							_currentZoom = _maxZoom;
						GUI.changed = true;
					}
				}

				if(_currentZoom>_minZoom) {
					Rect zoomOutNodeRect = new Rect(SkinItem.leftPanelWidth, position.height - (SkinItem.tabHeight + 25), SkinItem.tabHeight*1.5f, SkinItem.tabHeight*1.5f);
					if(GUI.RepeatButton(zoomOutNodeRect, "", _zoomOutStyle)) {
						_currentZoom -= 0.005f;
						if(_currentZoom<_minZoom)
							_currentZoom = _minZoom;
						GUI.changed = true;
					}
				}
			}

			if (GUI.changed) Repaint();
	    }

		void OnInspectorUpdate() {
			Repaint();
		}

	    #region NODES
		public void CreateBaseNode(ZNode.BASE_TYPE type) {
			_currentNodeTree.CreateBaseNode(type, _nodeScrollView.x + (position.width - SkinItem.leftPanelWidth)/2, _nodeScrollView.y + (position.height - SkinItem.tabHeight)/2, null);
		}
			
        public void CreateCompositeNode(ZNodeComposite.NODE_TYPE nodeType) {
            _currentNodeTree.CreateCompositeNode(nodeType, _nodeScrollView.x + (position.width - SkinItem.leftPanelWidth)/2, _nodeScrollView.y + (position.height - SkinItem.tabHeight)/2, null);
        }

        public void CreateDecoratorNode(ZNodeDecorator.NODE_TYPE nodeType) {
            _currentNodeTree.CreateDecoratorNode(nodeType, _nodeScrollView.x + (position.width - SkinItem.leftPanelWidth)/2, _nodeScrollView.y + (position.height - SkinItem.tabHeight)/2, null);
        }

        public void CreateActionNode(ZBTActionManager.NODE_TYPE actionType) {
            _currentNodeTree.CreateActionNode(actionType, _nodeScrollView.x + (position.width - SkinItem.leftPanelWidth)/2, _nodeScrollView.y + (position.height - SkinItem.tabHeight)/2, null);
		}

		public void RemoveNode(ZNode node) {
			_currentNodeTree.RemoveNode(node); 

			if(_selectedNode==node)
				DeselectNode(_selectedNode);
		}

		public void SelectNode(ZNode node) {
			_selectedNode = node;
			if(OnNodeSelected!=null)
				OnNodeSelected(node);
			GUI.changed = true;
		}

		public void DeselectNode(ZNode node) {
			_selectedNode = null;
			if(OnNodeSelected!=null)
				OnNodeSelected(null);
			GUI.changed = true;
		}

		public void DragNodeCompleted() {
			if(_editorSkinItem.autoLayoutAsDefault)
				_currentNodeTree.AutoLayout();
		}
	    #endregion

		#region CONNECTIONS
		public void SetStartConnection(ZNodeConnector inConnector) {
			_selectedConnector = inConnector;
		}

		public bool HasSelectedConnector() {
			return _selectedConnector!=null;
		}

		public void SetEndConnection(ZNodeConnector outConnector) {
			if(_selectedConnector!=null && _selectedConnector!=outConnector) {
				if(_selectedConnector.CanTakeConnection())
					_currentNodeTree.CreateConnection(_selectedConnector, outConnector);
			}
			_selectedConnector = null;
		}
		#endregion

		#region TABS
		void DrawTabs() {
			float startX = _currentTabStartX;
			if(_nodeTrees!=null) {
				float tabWidth = 100;
				int i=0;
				for(; i<_nodeTrees.Count; ++i) {
					Rect tabRect = new Rect(startX + tabWidth * i, 0, tabWidth, SkinItem.tabHeight);
					if(_currentNodeTree == _nodeTrees[i]) {
						GUI.Box(tabRect, _nodeTrees[i].GetName());

						Rect deleteNodeRect = new Rect(startX + tabWidth * i, 0, 10, 10);
						if(GUI.Button(deleteNodeRect, "", _xStyle)) {
							DeleteTab(i);
							break;
						}
					}
					else if(GUI.Button(tabRect, _nodeTrees[i].GetName())) {
						SwitchTab(_nodeTrees[i]);
					}
				}

				Rect newNodeRect = new Rect(startX + tabWidth * i, 0, SkinItem.tabHeight, SkinItem.tabHeight);
				if(GUI.Button(newNodeRect, "", _plusStyle)) {
					CreateNewTree();
				}
			}
		}

		void DrawTabArrows() {
			float tabWidth = 100;
			bool showGreenLeftArrow = _currentTabStartX < 0;
			// Left Arrow
			Rect leftArrowRect = new Rect(0, 0, SkinItem.tabHeight, SkinItem.tabHeight);
			if(GUI.Button(leftArrowRect, "", showGreenLeftArrow?_leftArrowActiveStyle:_leftArrowInactiveStyle)) {
				if(showGreenLeftArrow) {
					_currentTabStartX += tabWidth/2;
					if(_currentTabStartX > 0)
						_currentTabStartX = 0;
				}
			}

			bool isPastWindowTabSize = _currentTabStartX + tabWidth * _nodeTrees.Count + SkinItem.tabHeight > position.width-SkinItem.leftPanelWidth-SkinItem.tabHeight*2;
			// Right Arrow
			Rect rightArrowRect = new Rect(position.width-SkinItem.leftPanelWidth-SkinItem.tabHeight, 0, SkinItem.tabHeight, SkinItem.tabHeight);
			if(GUI.Button(rightArrowRect, "", isPastWindowTabSize?_rightArrowActiveStyle:_rightArrowInactiveStyle)) {
				if(isPastWindowTabSize) {
					_currentTabStartX -= tabWidth/2;
				}
			}
		}

		void DeleteTab(int tabIdx) {
			_nodeTrees.RemoveAt(tabIdx);
			if(tabIdx<_nodeTrees.Count) {
				SwitchTab(_nodeTrees[tabIdx]);
			}
			else if(tabIdx>0) {
				SwitchTab(_nodeTrees[tabIdx-1]);
			}
			else {
				_currentNodeTree = null;
			}
		}

		void ResetBeforeTabSwitch() {
			if(_selectedNode!=null)
				DeselectNode(_selectedNode);
			if(_selectedConnector!=null)
				_selectedConnector = null;
			if(_currentNodeTree!=null)
				_currentNodeTree.ResetNodes();
		}

		void SwitchTab(ZNodeTree newTree) {
			ResetBeforeTabSwitch();
			_currentNodeTree = newTree;
			_nodeScrollView = _currentNodeTree.GetScrollView();
		}

		public void GoToOrCreateTree(string treePath) {
			for(int i=0; i<_nodeTrees.Count; ++i) {
				if(_nodeTrees[i].GetFilePath()==treePath) {
					SwitchTab(_nodeTrees[i]);
					return;
				}
			}

			LoadTree(treePath);
		}
		#endregion

	    #region TOP_MENU
	    void DrawTopMenu() {
			GUILayout.BeginVertical();
			GUILayout.Space(5);
			// File Menu Options
	        GUILayout.BeginHorizontal();
			if(_currentNodeTree==null) {
				if(GUILayout.Button("New")) {
					CreateNewTree();
				}
			}
	        if(GUILayout.Button("Load")) {
				Load();
	        }
			if(_currentNodeTree!=null) {
		        if(GUILayout.Button("Save As")) {
					Save();
		        }
				if(!string.IsNullOrEmpty(_currentNodeTree.GetFilePath()) && GUILayout.Button("Save")) {
					Save(_currentNodeTree.GetFilePath());
		        }
		        if(GUILayout.Button("Reset")) {
					_currentNodeTree.Reset();
		        }
			}
			GUILayout.EndHorizontal();
			if(_currentNodeTree!=null) {
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Auto Layout")) {
					_currentNodeTree.AutoLayout();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.Space(10);
			//EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			if(_currentNodeTree!=null) {
				// Name
				GUILayout.BeginHorizontal();
				GUILayout.Label("Tree Name:", GUILayout.ExpandWidth(true));
				_currentNodeTree.SetName(GUILayout.TextField(_currentNodeTree.GetName(), GUILayout.Width(120)));
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
	    }
	    #endregion

	    #region GRID
		void DrawBackground() {
			DrawGrid(20, 0.2f, Color.gray);
			DrawGrid(100, 0.4f, Color.gray);
		}

	    Vector2 offset = Vector2.zero;
	    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
	        int widthDivs = Mathf.CeilToInt(SkinItem.mainPanelWidth / gridSpacing);
	        int heightDivs = Mathf.CeilToInt(SkinItem.mainPanelHeight / gridSpacing);

	        Handles.BeginGUI();
	        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

	        offset += Vector2.zero * 0.5f;
	        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

	        for (int i = 0; i < widthDivs; i++) {
	            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, SkinItem.mainPanelHeight, 0f) + newOffset);
	        }

	        for (int j = 0; j < heightDivs; j++) {
	            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(SkinItem.mainPanelWidth, gridSpacing * j, 0f) + newOffset);
	        }

	        Handles.color = Color.white;
	        Handles.EndGUI();
	    }
	    #endregion

		#region LOADING
		void Load(string filePath="") {
			string path = "";
			if(string.IsNullOrEmpty(filePath)) {
				path = EditorUtility.OpenFilePanel("Load a node file", Application.dataPath, "txt,json");
			}
			else {
				path = filePath;		
			}
			if (path.Length != 0) {
				string clippedPath = path.Replace(string.Concat(Application.dataPath,"/"),"");
				LoadTree(clippedPath);
			}
		}

		void LoadTree(string pathInProject) {
			string finalPath = string.Concat("Assets/", pathInProject);
			TextAsset textAsset = (TextAsset) AssetDatabase.LoadAssetAtPath(finalPath,typeof(TextAsset));
			if(textAsset!=null) {
				JSON js = new JSON();
				js.serialized = textAsset.text;

				CreateNewTree();
				_currentNodeTree.Load(js, pathInProject);
			}
		}
		#endregion

		#region SAVING
		void Save(string filePath="") {
			JSON treeJS = _currentNodeTree.Save();

			if(!string.IsNullOrEmpty(filePath)) {
				System.IO.File.WriteAllText(string.Concat(Application.dataPath,"/",filePath), treeJS.serializedCompact);
				AssetDatabase.Refresh();
			}
			else {
				string path = EditorUtility.SaveFilePanel("Save file", Application.dataPath, "test", "txt");
				if (path.Length != 0) {
					System.IO.File.WriteAllText(path, treeJS.serializedCompact);

					AssetDatabase.Refresh();

					_currentNodeTree.SetFilePath(path.Replace(string.Concat(Application.dataPath,"/"),""));
				}
			}
		}
		#endregion
	}
}