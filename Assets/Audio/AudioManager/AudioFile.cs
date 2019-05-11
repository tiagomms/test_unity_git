using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AudioFile
{

    public string audioName;
    public AudioClip audioClip;


    [Range(0f,1f)]
    public float volume;

	[Range(0, 255)]
	public int priority = 128;

    [HideInInspector]
    public AudioSource source;

    public bool isLooping;
    public bool playOnAwake;

}

