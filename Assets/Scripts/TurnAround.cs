using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TurnAround : MonoBehaviour {
    
    public SteamVR_Action_Boolean cancelSelectionAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("CancelSelection");
	
	public bool hasTurnAroundStarted = false;
	public float yInitialRotation = 0f;
	public float yRotationThreshold = 90f;

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level2_Events.START_TURN_AROUND, HandleStartTurnAround);
        EventManager.StartListening(Global.Level2_Events.TURN_AROUND, HandleTurnAround);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Global.Level2_Events.START_TURN_AROUND, HandleStartTurnAround);
        EventManager.StopListening(Global.Level2_Events.TURN_AROUND, HandleTurnAround);
    }
    private void HandleStartTurnAround()
    {
        hasTurnAroundStarted = true;
		yInitialRotation = gameObject.transform.eulerAngles.y;
    }

    private void HandleTurnAround()
    {
		hasTurnAroundStarted = false;
		PlayerPrefs.SetInt("NBR_BALLS", Global.Level2_Events.score);
        PlayerPrefs.SetFloat("LEVEL2_ROTATION", gameObject.transform.eulerAngles.y);
		EventManager.TriggerEvent(Global.Shared_Events.CHANGE_SCENE);
        // throw new NotImplementedException("Missing change scene");
    }

	
	// Update is called once per frame
	void Update () {
		if (cancelSelectionAction.GetStateDown(SteamVR_Input_Sources.Any)) { // delete this else part once narrator is completed
            EventManager.TriggerEvent(Global.Level2_Events.START_TURN_AROUND);
        }
		if (hasTurnAroundStarted) {
			if (Mathf.Abs(gameObject.transform.eulerAngles.y - yInitialRotation) > yRotationThreshold) {
				EventManager.TriggerEvent(Global.Level2_Events.TURN_AROUND);
			}
		}
	}
}
