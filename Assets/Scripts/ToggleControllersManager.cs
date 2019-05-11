using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;


public class ToggleControllersManager : MonoBehaviour
{
    public Teleport teleportScript;
    public VoiceCommandManager voiceCommandManagerScript;
    public ToggleLightBeam toggleLightBeamScript;
    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.SET_TELEPORT, SetTeleportScript);
        EventManager.StartListening(Global.Shared_Events.SET_VOICECOMMAND, SetVoiceCommandScript);
        EventManager.StartListening(Global.Shared_Events.SET_SELECTION_RAY, SetToggleLightBeamScript);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.SET_TELEPORT, SetTeleportScript);
        EventManager.StopListening(Global.Shared_Events.SET_VOICECOMMAND, SetVoiceCommandScript);
        EventManager.StopListening(Global.Shared_Events.SET_SELECTION_RAY, SetToggleLightBeamScript);
    }

    private void SetTeleportScript()
    {
        if (teleportScript != null)
        {
            teleportScript.enabled = Global.Shared_Controllers.TELEPORT;
        }
    }

    private void SetVoiceCommandScript()
    {
        if (voiceCommandManagerScript != null)
        {
            voiceCommandManagerScript.enabled = Global.Shared_Controllers.VOICECOMMAND;
        }
    }

    private void SetToggleLightBeamScript()
    {
        if (toggleLightBeamScript != null)
        {
            // gameObject of toggleLightBeamScript is the parent of all light beam variations
            // so by setting active / inactive this object, it blocks the light beams
            toggleLightBeamScript.gameObject.SetActive(Global.Shared_Controllers.SELECTION_RAY);
        }
    }
}
