using System;
/*
 * alternative way of sharing state (global) between game objects
 */

public static class Global
{
    public static class Shared_Events {
        static public string CHANGE_SCENE = "CHANGE_SCENE";
        static public string IN_MEDITATION_CIRCLE = "IN_MEDITATION_CIRCLE";
        static public string OUT_MEDITATION_CIRCLE = "OUT_MEDITATION_CIRCLE";

        // CONTROLLERS
        static public string SET_TELEPORT = "SET_TELEPORT";
        static public string SET_VOICECOMMAND = "SET_VOICECOMMAND";
        static public string SET_SELECTION_RAY = "SET_SELECTION_RAY";

        static public string TURN_ON_VOICE_INPUT = "TURN ON";
        static public string TURN_OFF_VOICE_INPUT = "TURN OFF";
        static public string SUCCESSFUL_VOICE_INPUT = "SUCCESSFUL";
    }

    public static class Shared_Controllers {
        static public bool TELEPORT = true;
        static public bool VOICECOMMAND = true;
        static public bool SELECTION_RAY = true;
        static public bool GRAB = true;
    }

    public enum ConsciousnessLevel {
        FULLY = 0, BECOMING = 1, NOT = 2
    }

    public static ConsciousnessLevel ConsciousLevel = ConsciousnessLevel.FULLY;
    public static string GetConsciousnessLevelString() {
        return Enum.GetName(typeof(ConsciousnessLevel), ConsciousLevel);
    }

    public static class Level1_Events {

    }
    public static class Level2_Events {
        // level events
        static public string RESET_BALL = "RESET_BALL";
        static public string THROW_BALL = "THROW";
        static public string TURN_AROUND = "TURN_AROUND";
        public static string START_TURN_AROUND = "START_TURN_AROUND";

        static public int score = 0;
    }
    public static class Level3_Events {

    }
    public static class Level4_Events {

    }
    public static class Level5_Events {

    }


}
