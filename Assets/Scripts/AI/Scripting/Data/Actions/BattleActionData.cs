using WyrmTale;

namespace ZAI {
    
    public class BattleActionData {
        public string id;
        public BattleActionTypes.TYPE type;
        public int frame;
        public int framesToRun;
        public bool pause;
        public bool debug;
        public bool playTerminateLogic;

        public BattleActionData(JSON actionJS) {
            frame = actionJS.ToInt("frame");
            framesToRun = actionJS.ToInt("numFrames");
            id = actionJS.ToString("id");
            type = (BattleActionTypes.TYPE)System.Enum.Parse(typeof(BattleActionTypes.TYPE), actionJS.ToString("type"));
            debug = actionJS.ToBoolean("debug");
            pause = actionJS.ToBoolean("pause");
            playTerminateLogic = actionJS.ToBoolean("resetOnEnd", true);
        }
    }

}