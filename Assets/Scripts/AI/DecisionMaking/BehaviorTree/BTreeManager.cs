/**********************************************************************************
 * Class:   BTreeManager
 * ********************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using WyrmTale;

namespace ZAI {

    public class BTreeManager : MonoBehaviour {

        static BTreeManager _instance;
        public static BTreeManager Instance { 
            get { 
                if(_instance==null) {
                    GameObject obj = new GameObject("BTree Manager");
                    BTreeManager bTreeManager = obj.AddComponent<BTreeManager>();
                    return bTreeManager;
                }
                else {
                    return _instance; 
                }
            }
        }

        public bool IsInitialized { get { return _isInitialized; } }
        bool _isInitialized;
        public event System.Action OnInitialized;

        public string containerPath;

        Dictionary<string, BTreeData> _dictOfBTreeData;

        void Awake() {
            _instance = this;
        }

        void OnDestroy() {
            _instance = null;
        }

        void Start() {
            Init();
        }

        void Init() {
            LoadData();
            if(OnInitialized!=null) {
                OnInitialized();
            }
            _isInitialized = true;
        }

        #region DATA
        void LoadData() {
            TextAsset fileAsset = Resources.Load(containerPath) as TextAsset;
            JSON js = new JSON();
            js.serialized = fileAsset.text;

            _dictOfBTreeData = new Dictionary<string, BTreeData>();
            string[] treeList = js.ToArray<string>("bTrees");
            for(int i=0; i<treeList.Length; ++i) {
                LoadBTree(treeList[i]);
            }
        }

        void LoadBTree(string treeFilePath) {
            TextAsset fileAsset = Resources.Load(treeFilePath) as TextAsset;
            if(fileAsset!=null) {
                JSON js = new JSON();
                js.serialized = fileAsset.text;

                BTreeData bTreeData = new BTreeData(js);
                if(!_dictOfBTreeData.ContainsKey(bTreeData.Id)) {
                    _dictOfBTreeData.Add(bTreeData.Id, bTreeData);
                }
                else {
                    Debug.Log("Error: Duplicate Btree id: " + bTreeData.Id + " in file: " + treeFilePath);
                }
            }
            else {
                Debug.Log("Error: Invalid Btree path: " + treeFilePath);
            }
        }

        public BTreeData GetBTreeData(string id) {
            if(_dictOfBTreeData.ContainsKey(id)) {
                return _dictOfBTreeData[id];
            }

            Debug.Log("Error: Invalid Btree Id: " + id);
            return null;
        }
        #endregion
    }

}
