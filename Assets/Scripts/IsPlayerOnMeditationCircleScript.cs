using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerOnMeditationCircleScript : MonoBehaviour {
	
    private List<string> handsOnMeditationCircle = new List<string>();

    public bool IsPlayerSittingOnMeditationCircle() {
        return handsOnMeditationCircle.Count > 0;
    }

    private void OnTriggerEnter(Collider other) {
        GameObject obj = other.gameObject;

        if (obj.tag == "Player") { // Players hands
            if (!handsOnMeditationCircle.Contains(obj.name)) {
                handsOnMeditationCircle.Add(obj.name);
                EventManager.TriggerEvent(Global.Shared_Events.IN_MEDITATION_CIRCLE);
                // DebugManager.Info("ENTER GameObject name: " + obj.name);
            }
            // DebugManager.Info("Is Player Sitting On Meditation Circle: " + IsPlayerSittingOnMeditationCircle());
        }
	}
	private void OnTriggerExit(Collider other)
	{
        GameObject obj = other.gameObject;
        if (obj.tag == "Player") {
            if (handsOnMeditationCircle.Contains(obj.name)) {
                handsOnMeditationCircle.Remove(obj.name);
                // DebugManager.Info("EXIT GameObject name: " + obj.name);
                if (!IsPlayerSittingOnMeditationCircle()) {
                    EventManager.TriggerEvent(Global.Shared_Events.OUT_MEDITATION_CIRCLE);
                }
            }
            // DebugManager.Info("Is Player Sitting On Meditation Circle: " + IsPlayerSittingOnMeditationCircle());
        }
	}
}
