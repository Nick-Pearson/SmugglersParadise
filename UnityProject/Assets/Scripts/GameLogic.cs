using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private float ObsticalDensity = 1;
    [SerializeField] private Image Fadeout;

    public enum State { TapToStart, Game, GameOver, Won, EndGame };

    public delegate void OnGameStateChange(State s);
    public static event OnGameStateChange OnStateChange;
    
    private PlayerCharacter mPlayerCharacter;
    private float mDistanceTravelled;
    private State mGameStatus;

    public static float GameDeltaTime { get; private set; }
    public static float GameFixedDeltaTime { get; private set; }
    public float PlayerSpeed { get { return mPlayerCharacter.PlayerSpeed; } }
    public static float ScreenBounds { get; private set; }
    public static float ScreenTop { get { return mScreenHeight + Camera.main.transform.position.y; } }
    public static float ScreenBottom { get { return Camera.main.transform.position.y - mScreenHeight; } }
    public static float ColumnSize { get { return ScreenBounds * 1.6f / (int)GameState.Column.NumColumns; } }
    public static bool Paused { get; private set; }
    public static Planet Origin;
    public static Planet Destination;
    public static float TargetDistance { get; private set; }

    //keep track of the camera offset
    public static float mScreenHeight; //height of half the camera

    //for spawning obsticals
    private const float MIN_OBSTICAL_DISTANCE = 10;
    private float[] mLastSpawnDistance;

    //time until we atuoload the menu
    private const float TIME_UNTIL_LEVEL_LOAD = 1.0f;

    void Awake()
	{
        //setup the camera variables
        float distance = transform.position.z-Camera.main.transform.position.z;
        mScreenHeight = CameraUtils.FrustumHeightAtDistance(distance, Camera.main.fieldOfView) * 0.5f;
        ScreenBounds = mScreenHeight * Camera.main.aspect;

        GameInput.OnTap += HandleOnTap;
		GameInput.OnSwipe += HandleOnSwipe;
		mPlayerCharacter = GetComponentInChildren<PlayerCharacter>();
		mGameStatus = State.TapToStart;
		Paused = false;
        
        //TODO: Fix aribtrary starting values
        Origin = World.Egoras;
        Destination = World.Hellzine;

        TargetDistance = World.getDistance(Origin, Destination);

        //setup spawning parameters
        mLastSpawnDistance = new float[(int)GameState.Column.NumColumns];

        //ensure nothing is spawned until we are out of the atmosphere
        for (int i = 0; i < mLastSpawnDistance.Length; i++)
            mLastSpawnDistance[i] = Origin.AtmosphereSize;
        

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

        //should we spawn some enemies
        for (int c = 0; c < (int)GameState.Column.NumColumns; c++)
        {
            //has it been long enough since the last obstical
            if(mLastSpawnDistance[c] + MIN_OBSTICAL_DISTANCE < mDistanceTravelled)
            {
                if(Random.value <= ObsticalDensity)
                    ObsticalFactory.Dispatch((GameState.Column)c);

                mLastSpawnDistance[c] = mDistanceTravelled;
            }
        }
    }

	void Update()
	{
		GameDeltaTime = Paused ? 0.0f : Time.deltaTime;

		if( mGameStatus == State.Game )
		{
			mDistanceTravelled += PlayerSpeed * GameDeltaTime;
            UIManager.UISystem.ChangeDistanceValue(mDistanceTravelled / TargetDistance);

            if(mDistanceTravelled > TargetDistance)
            {
                mGameStatus = State.Won;
                StartCoroutine(FadeOut(0.5f, 30));
                OnStateChange(mGameStatus);
            }
                
		}
	}

	private void Reset()
	{
		mPlayerCharacter.Reset();
		ObsticalFactory.Reset();
		mDistanceTravelled = 0.0f;
	}

	private void HandleOnTap( Vector3 position )
	{
		switch( mGameStatus )
		{
		case State.Game:
			mPlayerCharacter.Fire();
			break;
        default:
            //do nothing
            break;
        }
	}

    private IEnumerator FadeOut(float fadeoutTime, int iterations = 100)
    {
        UIManager.UISystem.ChangeEndText(Destination.Name);
        
        for(int i = 0; i < iterations; i++)
        {
            Fadeout.color = new Color(0, 0, 0, (float)(i+1) / iterations);
            yield return new WaitForSeconds(fadeoutTime / iterations);
        }

        //pause the game
        Paused = true;
        mGameStatus = State.EndGame;

        GameState.CurrentPlanet = Destination;

        OnStateChange(mGameStatus);

        yield return new WaitForSeconds(TIME_UNTIL_LEVEL_LOAD);
        SceneManager.LoadScene("Menu");
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
