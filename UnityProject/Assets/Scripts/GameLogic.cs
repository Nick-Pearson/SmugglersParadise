﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private float ObsticalDensity = 1;
    [SerializeField] private Image Fadeout;

    public enum State { TapToStart, Game, GameOver, Won, EndGame };
    
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
        GameInput.OnHold += HandleOnHold;
		mPlayerCharacter = GetComponentInChildren<PlayerCharacter>();
		mGameStatus = State.TapToStart;
		Paused = false;

        TargetDistance = World.getDistance(GameState.CurrentPlanet, GameState.Destination);

        //setup spawning parameters
        mLastSpawnDistance = new float[(int)GameState.Column.NumColumns];

        //ensure nothing is spawned until we are out of the atmosphere
        for (int i = 0; i < mLastSpawnDistance.Length; i++)
            mLastSpawnDistance[i] = GameState.CurrentPlanet.AtmosphereSize;
        

        StartCoroutine(StartSequence());
	}

    private void HandleOnHold(bool release)
    {
        if (release)
            UIManager.UISystem.ChangeThrottleValue(20);
        else
            UIManager.UISystem.ChangeThrottleValue(0);
    }

    IEnumerator StartSequence()
    {
        GetComponent<GameInput>().enabled = false;
        for(int i = 3; i > 0; i--)
        {
            UIManager.UISystem.ChangeStartText(i + "");
            yield return new WaitForSeconds(1.0f);
        }

        UIManager.UISystem.ChangeStartText("");
        Paused = false;
        mGameStatus = State.Game;
        GetComponent<GameInput>().enabled = true;
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
                GameState.CurrentPlanet = GameState.Destination;
                UIManager.UISystem.ChangeEndText(GameState.Destination.Name);
                StartCoroutine(FadeOut(0.5f, 30));
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

    public void GameOver()
    {
        UIManager.UISystem.ChangeEndText("Game Over");
        StartCoroutine(FadeOut(0.5f, 30));
    }

    private IEnumerator FadeOut(float fadeoutTime, int iterations = 100)
    {
        MusicManager.ChangeMusic();
        
        for (int i = 0; i < iterations; i++)
        {
            Fadeout.color = new Color(0, 0, 0, (float)(i+1) / iterations);
            yield return new WaitForSeconds(fadeoutTime / iterations);
        }

        //pause the game
        Paused = true;
        mGameStatus = State.EndGame;

        //store the players values
        //update the game state with our fuel and damage
        GameState.PlayerFuel = mPlayerCharacter.PlayerFuelAmount;
        GameState.PlayerDamage = 1 - ((float)mPlayerCharacter.GetComponent<DamageReciever>().CurrentHealth / mPlayerCharacter.GetComponent<DamageReciever>().MaxHealth);

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
