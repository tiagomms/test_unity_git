using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2B_BallLandslideAnimation : MonoBehaviour {

	// ball spawner class - object pool inside
	public class BallSpawner : MonoBehaviour
	{
		public static BallSpawner current;

		public GameObject pooledBall; //the prefab of the object in the object pool
		public int ballsAmount = 20; //the number of objects you want in the object pool
		public List<GameObject> pooledBalls; //the object pool
		public static int ballPoolNum = 0; //a number used to cycle through the pooled objects

		private float cooldown;
		private float cooldownLength = 0.5f;

		private float yBallOnFloorPosition = 0.16f; // below this a ball is on the floor
		private int checkingBallPoolCounter = 0; //a number used to check if all pooled objects are resting

		void Awake()
		{
			current = this; //makes it so the functions in ObjectPool can be accessed easily anywhere
		}

		void Start()
		{
			//Create Bullet Pool
			pooledBalls = new List<GameObject>();
			for (int i = 0; i < ballsAmount; i++)
			{
				GameObject obj = Instantiate(pooledBall);
				obj.SetActive(false);
				pooledBalls.Add(obj);
			}
		}

		public GameObject GetPooledBall()
		{
			checkingBallPoolCounter = 0;
			ballPoolNum++;
			if (ballPoolNum > (ballsAmount - 1))
			{
				ballPoolNum = 0;
			}
			while (poolBallsAreAlive(pooledBalls[ballPoolNum]))
			{
				ballPoolNum++;
				if (ballPoolNum > (ballsAmount - 1))
				{
					ballPoolNum = 0;
				}
				checkingBallPoolCounter++;
			}
			//if all balls are active create a new one
			if (checkingBallPoolCounter == ballsAmount)
			{
				//create a new ball and add it to the Pooled Ball List
				GameObject obj = Instantiate(pooledBall);
				pooledBalls.Add(obj);
				ballsAmount++;
				ballPoolNum = ballsAmount - 1;
			}
			DebugManager.Info("GetPooledBall returned ball number: " + ballPoolNum);

			return pooledBalls[ballPoolNum];
		}
		private bool poolBallsAreAlive(GameObject currentBall)
		{
			return checkingBallPoolCounter < ballsAmount && currentBall.activeInHierarchy && currentBall.transform.position.y >= yBallOnFloorPosition;
		}
		// Update is called once per frame
		void Update()
		{
			cooldown -= Time.deltaTime;
			if (cooldown <= 0)
			{
				cooldown = cooldownLength;
				SpawnBall();
			}
		}

		void SpawnBall()
		{
			GameObject selectedBall = BallSpawner.current.GetPooledBall();
			// change this bit here
			selectedBall.transform.position = transform.position;

			Rigidbody selectedRigidbody = selectedBall.GetComponent<Rigidbody>();
			selectedRigidbody.velocity = Vector3.zero;
			selectedRigidbody.angularVelocity = Vector3.zero;
			selectedBall.SetActive(true);
		}
	}
	// object pool - 50 balls
	public int objectPoolSize = 10;
	// // nbr of throws
	// private int nbrOfThrows = 0;

	// cube that is resetting throwable balls on top to their spawn location
	public GameObject stopThrowableBallsCube;
	
	// Use this for initialization
	void Start () {
		if (stopThrowableBallsCube != null) {
			stopThrowableBallsCube.SetActive(true);
		}
	}
	
	// public void IncrementNbrOfThrows() { nbrOfThrows++; }

	// Update is called once per frame
	void Update () {
		
	}
}

