using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ControllerHintsManager : MonoBehaviour {

    public static ControllerHintsManager instance;
    
    public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
    public SteamVR_Action_Boolean grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
    
    public SteamVR_Action_Boolean voiceInputAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VoiceInput");
    
    public SteamVR_Action_Boolean selectionRayAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SelectionRay");
    public SteamVR_Action_Boolean cancelSelectionAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("CancelSelection");
    
    public SteamVR_Action_Boolean extraMenuAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("ExtraMenu");
    
    public SteamVR_Action_Boolean interactUIAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

	public Hand leftHand;
	public Hand rightHand;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // CreateButtonHints();

        DontDestroyOnLoad(gameObject);
	}

    private void CreateButtonHints()
    {
        // ControllerButtonHints.CreateAndAddButtonInfo(teleportAction, SteamVR_Input_Sources.LeftHand);


        throw new NotImplementedException();
    }

    private void OnHandHoverBegin(Hand hand)
    {
        // ControllerButtonHints.ShowButtonHint(hand, EVRButtonId.k_EButton_Grip);
        // ControllerButtonHints.ShowTextHint(hand, EVRButtonId.k_EButton_Grip, "Move thing");
        ControllerButtonHints.ShowButtonHint(hand, voiceInputAction);
        ControllerButtonHints.ShowTextHint(hand, voiceInputAction, "Say: Go Away");
    }

    private void OnHandHoverEnd(Hand hand)
    {
        // ControllerButtonHints.HideButtonHint(hand, EVRButtonId.k_EButton_Grip);
        // ControllerButtonHints.HideTextHint(hand, EVRButtonId.k_EButton_Grip);
        ControllerButtonHints.HideButtonHint(hand, voiceInputAction);
        ControllerButtonHints.HideTextHint(hand, voiceInputAction);
    }

	private void Start()
	{
		StartCoroutine(ShowHintTest());
	}

    private IEnumerator ShowHintTest()
    {
        yield return new WaitForSeconds(10f);
		DebugManager.Info("showing info");
		OnHandHoverBegin(rightHand);
    }
}
