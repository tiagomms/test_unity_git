using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;

public class VoiceCommandManager : MonoBehaviour
{
    public SteamVR_Action_Boolean voiceInputAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VoiceInput");
    public TrashObjectsHandling trashObjectsHandling;
    public ConfidenceLevel wordConfidenceLevel = ConfidenceLevel.Low;

    // TODO: with events
    public AudioClip turnOnClip;
    public AudioClip turnOffClip;
    public AudioClip successClip;

    // private Queue<AudioClip> audioClipQueue = new Queue<AudioClip>();

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    private bool isInMeditationCircle = false;
    private bool isActivated = false;
    private AudioSource voiceInputAudioSource;

    private void Awake()
    {
        voiceInputAudioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Start()
    {
        keywords.Add("go away", () => { HandleGoAwayVoiceInput(); });
        keywords.Add("I am ready", () => { HandleImReadyVoiceInput(); });
        keywords.Add("I'm ready", () => { HandleImReadyVoiceInput(); });

        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray(), wordConfidenceLevel);
    }

    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.IN_MEDITATION_CIRCLE, HandleInMeditationCircle);
        EventManager.StartListening(Global.Shared_Events.OUT_MEDITATION_CIRCLE, HandleOutMeditationCircle);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.IN_MEDITATION_CIRCLE, HandleInMeditationCircle);
        EventManager.StopListening(Global.Shared_Events.OUT_MEDITATION_CIRCLE, HandleOutMeditationCircle);
    }

    private void HandleOutMeditationCircle()
    {
        isInMeditationCircle = false;
    }

    private void HandleInMeditationCircle()
    {
        isInMeditationCircle = true;
    }

    private void KeywordRecognizerOnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction) && keywordRecognizer.IsRunning)
        {
            keywordAction.Invoke();
        }
    }


    private void HandleImReadyVoiceInput()
    {
        if (isInMeditationCircle)
        {
            StartCoroutine(SuccessKeywordRecognizer());
            StartCoroutine(TriggerChangeScene());
        }
        throw new NotImplementedException();
    }

    private IEnumerator TriggerChangeScene()
    {
        yield return new WaitForSecondsRealtime(1f);
        EventManager.TriggerEvent(Global.Shared_Events.CHANGE_SCENE);
    }

    private void HandleGoAwayVoiceInput()
    {
        // trigger transparency
        bool isAnyFading = trashObjectsHandling.TriggerFading();
        if (isAnyFading)
        {
            StartCoroutine(SuccessKeywordRecognizer());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActivated && voiceInputAction.GetState(SteamVR_Input_Sources.RightHand))
        {
            // TODO: make voice input ready sound
            StartCoroutine(StartKeywordRecognizer());
            DebugManager.Info("ON - keywordRecognizer");
        }
        if (voiceInputAction.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            // TODO: make voice input off
            StartCoroutine(StopKeywordRecognizer());
            DebugManager.Info("OFF - keywordRecognizer");
        }
    }
    private IEnumerator StartKeywordRecognizer()
    {
        isActivated = true;
        VoiceManagerPlayClip(turnOnClip);
        yield return new WaitForSecondsRealtime(.5f);
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private IEnumerator StopKeywordRecognizer()
    {
        keywordRecognizer.OnPhraseRecognized -= KeywordRecognizerOnPhraseRecognized;
        keywordRecognizer.Stop();
        VoiceManagerPlayClip(turnOffClip);
        yield return new WaitForSecondsRealtime(.7f);
        isActivated = false;
    }
    private IEnumerator SuccessKeywordRecognizer()
    {
        yield return new WaitForSecondsRealtime(.3f);
        VoiceManagerPlayClip(successClip);
    }

    private void VoiceManagerPlayClip(AudioClip newClip)
    {
        if (voiceInputAudioSource.isPlaying)
        {
            voiceInputAudioSource.Stop();
        }
        voiceInputAudioSource.clip = newClip;
        voiceInputAudioSource.Play();
        DebugManager.Info("Now Playing: " + newClip.name);
    }



    private void OnApplicationQuit()
    {
        if (keywordRecognizer != null && keywordRecognizer.IsRunning)
        {
            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizerOnPhraseRecognized;
            keywordRecognizer.Stop();
        }
    }
}
