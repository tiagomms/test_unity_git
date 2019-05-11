using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpiritsController : MonoBehaviour {
	// private static int IT_NBR = 0;
	private Dictionary<string, LightSpirit> lightSpiritsDict = new Dictionary<string, LightSpirit>();
	public enum LSAnimations {
		STILL = -1,
		IDLE = 0,
		TALKING = 1,
		FLYING = 2
	}

	public class LightSpirit {
		public GameObject lsGameObject;
		public LightSpiritAnimationManager lsAnimatorManager;
	}
	// Use this for initialization
	public string GetLSAnimationName(int action)
    {
        return Enum.GetName(typeof(LSAnimations), action);
    }
	private void Awake()
	{
		// generate random seed everytime
		UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

		GameObject[] allLightSpirits = GameObject.FindGameObjectsWithTag("LightSpirit");
		foreach (GameObject obj in allLightSpirits)
		{
			LightSpirit newLightSpirit = new LightSpirit();
			newLightSpirit.lsGameObject = obj;
			newLightSpirit.lsAnimatorManager = obj.GetComponent<LightSpiritAnimationManager>();
			lightSpiritsDict.Add(obj.name, newLightSpirit);
		}
	}

	public void SetActionToAllLightSpirits(int actionValue) {
		foreach(KeyValuePair<string, LightSpirit> item in lightSpiritsDict)
		{
            item.Value.lsAnimatorManager.SetActionByValue(actionValue);
		}
	}
	
	public void SetActionToLightSpirit(string lsName, int actionValue) {
		LightSpirit currentLS = lightSpiritsDict[lsName];
		if (currentLS != null) {
			currentLS.lsAnimatorManager.SetActionByValue(actionValue);
		}
	}

	public void SetLockedAnimToLightSpirit(string lsName, bool lockedAnim = false) {
		LightSpirit currentLS = lightSpiritsDict[lsName];
		if (currentLS != null) {
			currentLS.lsAnimatorManager.IsLockedAnimOn = lockedAnim;
		}
	}

	private void Start()
	{
		SetActionToAllLightSpirits((int)LSAnimations.IDLE);
		SetActionToLightSpirit("Angel White", (int)LSAnimations.TALKING);
        SetLockedAnimToLightSpirit("Angel White", true);
		
		StartCoroutine(TriggerRandomLightSpiritAnimations());
	}

    private IEnumerator TriggerRandomLightSpiritAnimations()
    {
        foreach (LightSpirit ls in RandomValues(lightSpiritsDict).Take(lightSpiritsDict.Count))
        {
			float randProb = UnityEngine.Random.value;
			int action = 0;
			if (randProb < 0.01f) {
				action = (int)LSAnimations.STILL;
			} else if (randProb < 0.85f) {
				action = (int)LSAnimations.IDLE;
			} else {
				action = (int)LSAnimations.FLYING;
			}

			SetActionToLightSpirit(ls.lsGameObject.name, action);
            // DebugManager.Info("IT Nbr: " + IT_NBR + " - " + ls.lsGameObject.name + " - Action: " + GetLSAnimationName(action));
        }
		yield return new WaitForSeconds(5f);
		while (true) {
			// IT_NBR++;
			yield return TriggerRandomLightSpiritAnimations();
		}
    }

    public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
    {
        System.Random rand = new System.Random();
        List<TValue> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;
        while (true)
        {
            yield return values[rand.Next(size)];
        }
    }
}
