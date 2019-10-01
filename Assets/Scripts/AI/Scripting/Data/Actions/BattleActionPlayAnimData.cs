using WyrmTale;
using System.Collections.Generic;
using UnityEngine;

namespace ZAI {

    public class BattleActionPlayAnimData : BattleActionData {
        public string animName;
        public string[] animList;
        public float speed, startPercent, blendTime;
        public int layer;

        public BattleActionPlayAnimData(JSON actionJS):base(actionJS)
        {
            JSON parms = actionJS.ToJSON("nodeParams");
            animName = parms.ToString("animName");
            animList = parms.ToArray<string>("animList");
            speed = parms.ToFloat("speed", 1);
            startPercent = parms.ToFloat("startPercent")/100.0f;
            blendTime = parms.ToFloat ("blendTime");
            layer = parms.ToInt ("layer");
        }
    }

}
