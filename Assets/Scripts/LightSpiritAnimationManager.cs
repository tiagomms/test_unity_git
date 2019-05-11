using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpiritAnimationManager : MonoBehaviour {
	private Animator anim;
    private bool isLockedAnimOn = false;

    public bool IsLockedAnimOn
    {
        get
        {
            return isLockedAnimOn;
        }

        set
        {
            isLockedAnimOn = value;
        }
    }

    private void Awake()
	{
		anim = gameObject.GetComponent<Animator>();
	}

	public void SetActionByValue(int actionValue) {
		if (!IsLockedAnimOn) {
        	anim.SetInteger("Action", actionValue);
		}
    }
}
