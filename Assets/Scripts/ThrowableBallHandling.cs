using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableBallHandling : MonoBehaviour
{

    // gameobject - ball
    public GameObject spawnLocation;
    private Rigidbody tBrb;

    // Use this for initialization
    private void Awake()
    {
        tBrb = gameObject.GetComponent<Rigidbody>();
    }
    void Start()
    {
        HandleResetBall();
    }

    private void OnEnable()
    {
        EventManager.StartListening(Global.Level2_Events.RESET_BALL, HandleResetBall);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Level2_Events.RESET_BALL, HandleResetBall);
    }

    private void HandleResetBall()
    {
        tBrb.useGravity = false;
        tBrb.isKinematic = true;
        gameObject.transform.position = spawnLocation.transform.position;
    }

    public void HandleThrow()
    {
        tBrb.isKinematic = false;
        tBrb.useGravity = true;

        Global.Level2_Events.score++;
    }
    // animation - floating??? / rotating on itself? / any needed?
}
