using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour 
{
    [SerializeField] private float WaitTime = 1.0f;

	public enum State { TapToStart, Game, GameOver };

    public delegate void OnGameStateChange(State s);
    public static event OnGameStateChange OnStateChange;

	private List<GameObject> mActiveObsticals;
	private PlayerCharacter mPlayerCharacter;
    private float mGameOverTime;
    private float mDistanceTravelled;
	private State mGameStatus;

	public static float GameDeltaTime { get; private set; }
    public static float GameFixedDeltaTime { get; private set; }
	public float PlayerSpeed { get { return mPlayerCharacter.PlayerSpeed; } }
	public static float ScreenBounds { get; private set; }
	public static float ScreenHeight { get; private set; }
	public static bool Paused { get; private set; }
    public static Planet Origin;
    public static Planet Destination;

    void Awake()
	{
		float distance = transform.position.z - Camera.main.transform.position.z;
		ScreenHeight = CameraUtils.FrustumHeightAtDistance( distance, Camera.main.fieldOfView );
		ScreenBounds = ScreenHeight * Camera.main.aspect * 0.5f;

		GameInput.OnTap += HandleOnTap;
		GameInput.OnSwipe += HandleOnSwipe;
		mActiveObsticals = new List<GameObject>();
		mPlayerCharacter = GetComponentInChildren<PlayerCharacter>();
		mGameStatus = State.TapToStart;
        mGameOverTime = Time.timeSinceLevelLoad;
		Paused = false;

        //TODO: Fix aribtrary starting values
        Origin = World.Egoras;
        Destination = World.Hellzine;

        StartCoroutine(StartSequence());
	}

    IEnumerator StartSequence()
    {
        for(int i = 3; i > 0; i--)
        {
            UIManager.UISystem.ChangeStartText(i + "");
            yield return new WaitForSeconds(1.0f);
        }

        UIManager.UISystem.ChangeStartText("");
        Paused = false;
        mGameStatus = State.Game;
        OnStateChange(mGameStatus);
    }

    void FixedUpdate()
    {
        GameFixedDeltaTime = Paused ? 0.0f : Time.fixedDeltaTime;
    }

	void Update()
	{
		GameDeltaTime = Paused ? 0.0f : Time.deltaTime;

		if( mGameStatus == State.Game )
		{
			mDistanceTravelled += PlayerSpeed * GameDeltaTime;

			// Update the position of each active enemy, keep a track of enemies which have gone off screen 
			List<GameObject> oldEnemys = new List<GameObject>(); 
			for( int count = 0; count < mActiveObsticals.Count; count++ )
			{
				if( mActiveObsticals[count].transform.position.y < ScreenHeight * -0.5f )
				{
					ObsticalFactory.Return( mActiveObsticals[count] );
					oldEnemys.Add( mActiveObsticals[count] );
				}
			}

			for( int count = 0; count < oldEnemys.Count; count++ )
			{
				mActiveObsticals.Remove( oldEnemys[count] );
			}
		}
	}

	private void Reset()
	{
		mPlayerCharacter.Reset();
		ObsticalFactory.Reset();
		mActiveObsticals.Clear();
		mDistanceTravelled = 0.0f;
	}

	private void HandleOnTap( Vector3 position )
	{
		switch( mGameStatus )
		{
		case State.TapToStart:
            //Do Nothing
            break;
		case State.Game:
			mPlayerCharacter.Fire();
			break;
		case State.GameOver:
            if (Time.timeSinceLevelLoad - mGameOverTime > WaitTime)
            { 
			    Reset();
			    UIManager.UISystem.ChangeStartText("Tap to Start");
			    mGameStatus = State.TapToStart;
            }
            OnStateChange(mGameStatus);
            break;
		}
	}

	
	private void HandleOnSwipe( GameInput.Direction direction )
	{
		if( mGameStatus == State.Game )
		{
			switch( direction )
			{
			case GameInput.Direction.Left:
				mPlayerCharacter.MoveLeft();
				break;
			case GameInput.Direction.Right:
				mPlayerCharacter.MoveRight();
				break;
			}
		}
	}
}
