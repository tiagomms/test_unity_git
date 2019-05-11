using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
public class TriggerChangeScene : MonoBehaviour {
	private SteamVR_LoadLevel steamVR_LoadLevel;

	private void Awake() {
		steamVR_LoadLevel = GetComponent<SteamVR_LoadLevel>();
	}
    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.CHANGE_SCENE, HandleChangeScene);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.CHANGE_SCENE, HandleChangeScene);
    }

    private void HandleChangeScene()
    {
        StartCoroutine(TriggerNextScene());
    }

    private IEnumerator TriggerNextScene()
    {
        yield return new WaitForEndOfFrame();
        steamVR_LoadLevel.Trigger();
    }
}
