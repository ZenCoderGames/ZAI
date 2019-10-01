using UnityEngine;
using UnityEditor;
using WyrmTale;
using ZEditor;

public class ZBTActionScale:ZNodeAction {
    public string scaleAmountX, scaleAmountY, scaleAmountZ;
    public string scaleSpeed;

    public ZBTActionScale(ZNodeTree nodeTree, Rect wr, JSON js):base(nodeTree, wr, ZBTActionManager.NODE_TYPE.SCALE) {
        scaleSpeed = "1";
        scaleAmountX = "0";
        scaleAmountY = "0";
        scaleAmountZ = "0";

        if(js!=null && js.Contains("nodeParams")) {
            JSON paramsJS = js.ToJSON("nodeParams");
            float[] scaleValues = paramsJS.ToArray<float>("scaleAmount");
            Vector3 scaleAmount = new Vector3(scaleValues[0], scaleValues[1], scaleValues[2]);
            scaleAmountX = scaleAmount.x.ToString();
            scaleAmountY = scaleAmount.y.ToString();
            scaleAmountZ = scaleAmount.z.ToString();
            scaleSpeed = paramsJS.ToFloat("scaleSpeed").ToString();
        }
	}

    override public void Serialize(ref JSON nodeJS) {
        base.Serialize(ref nodeJS);

        JSON paramsJS = new JSON();
        float[] scaleAmount = new float[3]{ float.Parse(scaleAmountX), float.Parse(scaleAmountY), float.Parse(scaleAmountZ) };
        paramsJS["scaleAmount"] = scaleAmount;
        paramsJS["scaleSpeed"] = float.Parse(scaleSpeed);

        nodeJS["nodeParams"] = paramsJS;
    }

    override public void DrawInInspector(GUIStyle guiStyle) {
        base.DrawInInspector(guiStyle);

        GUILayout.BeginHorizontal("");
        GUILayout.TextArea("x:", EditorStyles.label);
        scaleAmountX = GUILayout.TextField(scaleAmountX, EditorStyles.textField);
        GUILayout.TextArea("y:", EditorStyles.label);
        scaleAmountY = GUILayout.TextField(scaleAmountY, EditorStyles.textField);
        GUILayout.TextArea("z:", EditorStyles.label);
        scaleAmountZ = GUILayout.TextField(scaleAmountZ, EditorStyles.textField);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("");
        GUILayout.TextArea("speed:", guiStyle);
        scaleSpeed = GUILayout.TextField(scaleSpeed, EditorStyles.textField);
        GUILayout.EndHorizontal();
    }
}