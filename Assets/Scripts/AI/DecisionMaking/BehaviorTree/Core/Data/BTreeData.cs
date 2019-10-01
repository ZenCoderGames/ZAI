using UnityEngine;
using WyrmTale;
using System.Collections.Generic;

namespace ZAI {
    
    public class BTreeData {
        public string Id { get { return _id; } }
        string _id;
        public BTNodeData Root { get { return _btRootData; } }
        BTNodeData _btRootData;

        public BTreeData(JSON js) {
            _id = js.ToString("name");

            List<BTNodeData> listOfNodes = new List<BTNodeData>();
            JSON[] nodeList = js.ToArray<JSON>("bTree");
            for(int i=0; i<nodeList.Length; ++i) {
                BTNodeData nodeData = BTNodeData.CreateNodeData(nodeList[i]);
                listOfNodes.Add(nodeData);
            }

            JSON[] connectionList = js.ToArray<JSON>("connections");
            for(int i=0; i<connectionList.Length; ++i) {
                int inNodeId = connectionList[i].ToInt("inNodeId");
                int outNodeId = connectionList[i].ToInt("outNodeId");
                BTNodeData inNode = listOfNodes[inNodeId];
                BTNodeData outNode = listOfNodes[outNodeId];
                if(inNode.type == BTNodeData.TYPE.ROOT || inNode.type == BTNodeData.TYPE.DECORATOR) {
                    (inNode as BTDecoratorNodeData).AddChild(outNode);
                    if(inNode.type == BTNodeData.TYPE.ROOT) {
                        _btRootData = outNode;
                    }
                }
                else if(inNode.type == BTNodeData.TYPE.COMPOSITE) {
                    (inNode as BTCompositeNodeData).AddChild(outNode);
                }
            }
        }
    }

}
