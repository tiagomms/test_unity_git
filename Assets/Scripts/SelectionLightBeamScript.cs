using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SelectionLightBeamScript : MonoBehaviour
{

    public Hand rightHand;
    public SteamVR_Input_Sources thisInputSource = SteamVR_Input_Sources.RightHand;

    public SteamVR_Action_Boolean selectionRayAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SelectionRay");
    public SteamVR_Action_Boolean cancelSelectionAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("CancelSelection");

    public TrashObjectsHandling trashObjectHandling;

    //public GameObject parentHand;
    public GameObject lineRendererObject;
    public GameObject surroundingPSObject;
    public GameObject startingPointPSObject;

    // variables to hold data
    public float maxLRWidth = 2.0f;
    public float maxLRLength = 10.0f;
    public float maxParticleSurBeamLifetime = 5.0f;
    public float minParticleSurBeamLifetime = 1.0f;
    public float maxSpeedParticle = 3.0f;

    public float startingSpeedParticle = 0.0f;
    public float startingMaxSurroundingPSLifetime = 1.0f;
    public float startingLRLength = 0.2f;
    public float startingLRWidth = 1.0f;

    // time it takes to skip level
    public float timeStarting = 5.0f;
    public float timeProgressing = 10.0f;
    public float timeTurn_Off = 2.0f;

    // current times
    public float currentTimeStarting = 0.0f;
    public float currentTimeProgressing = 0.0f;
    public float currentTimeTurn_Off = 0.0f;

    private float currentLRLength = 0.0f;
    // will not be used

    public enum SelectionRayMode
    {
        STARTING,
        PROGRESSING,
        MAX_REACH,
        TURN_OFF,
        DISABLED
    }

    public SelectionRayMode currentMode = SelectionRayMode.DISABLED;

    private LineRenderer lineRendererBeam;
    private Vector3 finalLRPosition;

    private ParticleSystem surroundingPSBeam;
    private ParticleSystem.MainModule surroundingPSBeamMain;

    private ParticleSystem startingPointPSBeam;
    private ParticleSystem.MainModule startingPointPSBeamMain;

    public bool isRayActivated = false;
    public bool isHittingObjects = false;
    private int trashLayer;
    // private int 
    private float rayRadius = 0.2f;
    private RaycastHit[] hitObjects;

    //public float 
    // TODO: test voice input with selection ray at the same time
    // private void OnEnable()
    // {
    //     EventManager.StartListening(Global.Shared_Events.TURN_ON_VOICE_INPUT, HandleTurnOnVoiceInput);
    //     EventManager.StartListening(Global.Shared_Events.TURN_OFF_VOICE_INPUT, HandleTurnOffVoiceInput);
    // }
    // private void OnDisable()
    // {
    //     EventManager.StopListening(Global.Shared_Events.TURN_ON_VOICE_INPUT, HandleTurnOnVoiceInput);
    //     EventManager.StopListening(Global.Shared_Events.TURN_OFF_VOICE_INPUT, HandleTurnOffVoiceInput);
    // }

    // private void HandleTurnOnVoiceInput()
    // {
    // }

    // private void HandleTurnOffVoiceInput()
    // {
    // }

    // Use this for initialization
    void Start()
    {
        lineRendererBeam = lineRendererObject.GetComponent<LineRenderer>();
        finalLRPosition = lineRendererBeam.GetPosition(1);

        surroundingPSBeam = surroundingPSObject.GetComponent<ParticleSystem>();
        surroundingPSBeamMain = surroundingPSBeam.main;

        startingPointPSBeam = startingPointPSObject.GetComponent<ParticleSystem>();
        startingPointPSBeamMain = startingPointPSBeam.main;

        trashLayer = LayerMask.GetMask("trash");
        disableSelectionRay();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionRayAction.GetStateDown(thisInputSource))
        {
            // enable things to start Ray
            lineRendererObject.SetActive(true);
            surroundingPSObject.SetActive(true);
            currentLRLength = maxLRLength;
            surroundingPSBeamMain.startSpeed = maxSpeedParticle;

            finalLRPosition.z = maxLRLength;
            lineRendererBeam.SetPosition(1, finalLRPosition);
            isRayActivated = true;
            isHittingObjects = true;
        }
        if (selectionRayAction.GetState(thisInputSource))
        {
            // TODO: ray animation - starting, progressing, max reach

            // haptics
            rightHand.TriggerHapticPulse(750);
        }
        if (selectionRayAction.GetStateUp(thisInputSource))
        {
            // TODO: animation turnOff
            StartCoroutine(TurnOffRayAnimation());
        }
        if (isRayActivated) {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            float maxDistance = maxLRLength;
            if (Physics.Raycast(ray, out hit, maxLRLength, ~trashLayer)) {
                maxDistance = hit.distance;
            }

            // animation - line length and surrounding beam distance
            finalLRPosition.z = maxDistance;
            lineRendererBeam.SetPosition(1, finalLRPosition);
            surroundingPSBeamMain.startSpeed = maxDistance / maxParticleSurBeamLifetime;
            
            // hit trash objects
            if (isHittingObjects) {
                StartCoroutine(HitTrashObjects(ray, maxDistance));
            }
        }

        if (cancelSelectionAction.GetStateDown(thisInputSource) && trashObjectHandling.anyObjectSelected()) {
            // since there is no hits, it will cancel all selected objects back to normal            
            trashObjectHandling.TriggerSelection();
            // haptics
            rightHand.TriggerHapticPulse(0.5f, 20.0f, 25.0f);
        }
    }

    private IEnumerator HitTrashObjects(Ray ray, float maxDistance)
    {
        isHittingObjects = false;
        hitObjects = Physics.SphereCastAll(ray, rayRadius, maxDistance, trashLayer);
        if (hitObjects.Length != 0) {
            foreach (RaycastHit obj in hitObjects)
            {
                trashObjectHandling.HitObject(obj.transform.name);
            }
            trashObjectHandling.TriggerSelection();
        }
        yield return new WaitForSeconds(.5f);
        isHittingObjects = true;
    }

    private IEnumerator TurnOffRayAnimation()
    {
        disableSelectionRay();
        throw new NotImplementedException();
    }

    public void disableSelectionRay()
    {
        finalLRPosition.z = startingLRLength;
        lineRendererBeam.SetPosition(1, finalLRPosition);

        surroundingPSBeamMain.startSpeed = startingSpeedParticle;
        lineRendererObject.SetActive(false);
        surroundingPSObject.SetActive(false);
        isRayActivated = false;
        isHittingObjects = false;
    }
}
