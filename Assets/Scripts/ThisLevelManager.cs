using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisLevelManager : MonoBehaviour {
	public enum ThisLevelNbr
	{
		L1, L2A, L2B, L3, L4, L5, L6
	}
	public ThisLevelNbr currentLevel = ThisLevelNbr.L1;
	private void Awake()
	{
		if (currentLevel == ThisLevelNbr.L1) {
			Global.Shared_Controllers.TELEPORT = false;
			Global.Shared_Controllers.VOICECOMMAND = false;
			Global.Shared_Controllers.SELECTION_RAY = false;
		} else if (currentLevel == ThisLevelNbr.L3 || currentLevel == ThisLevelNbr.L5) {
			Global.Shared_Controllers.TELEPORT = false;
			Global.Shared_Controllers.VOICECOMMAND = true;
			Global.Shared_Controllers.SELECTION_RAY = false;
		} else { // 2A, 2B, 4, 6
			Global.Shared_Controllers.TELEPORT = true;
			Global.Shared_Controllers.VOICECOMMAND = true;
			Global.Shared_Controllers.SELECTION_RAY = true;
		}

		// toggle level scripts
		switch (currentLevel) {
			case ThisLevelNbr.L1:
				Global.ConsciousLevel = Global.ConsciousnessLevel.FULLY;
				break;
			case ThisLevelNbr.L2A:
				Global.ConsciousLevel = Global.ConsciousnessLevel.NOT;
				break;
			case ThisLevelNbr.L2B:
				Global.ConsciousLevel = Global.ConsciousnessLevel.NOT;
				break;
			case ThisLevelNbr.L3:
				break;
			case ThisLevelNbr.L4:
				Global.ConsciousLevel = Global.ConsciousnessLevel.BECOMING;
				break;
			case ThisLevelNbr.L5:
				break;
			case ThisLevelNbr.L6:
				Global.ConsciousLevel = Global.ConsciousnessLevel.FULLY;
				break;
			default:
				break;
		}
	}

	// Use this for initialization
	void Start () {
		EventManager.TriggerEvent(Global.Shared_Events.SET_TELEPORT);
		EventManager.TriggerEvent(Global.Shared_Events.SET_VOICECOMMAND);
		EventManager.TriggerEvent(Global.Shared_Events.SET_SELECTION_RAY);
	}
}
