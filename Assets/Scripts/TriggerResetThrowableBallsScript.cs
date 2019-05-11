using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerResetThrowableBallsScript : MonoBehaviour {

	private void OnTriggerEnter(Collider other) {
		GameObject obj = other.gameObject;
		if (obj.tag == "ThrowableBall") {
			EventManager.TriggerEvent(Global.Level2_Events.RESET_BALL);
		}
	}
}
