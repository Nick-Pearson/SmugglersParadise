using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour 
{
	[SerializeField] private Camera GameplayCamera;
    [SerializeField] private RelativeObject PlanetGraphics;
	[SerializeField] private float FireOffset;
    [SerializeField] private float RotationSpeed;

	private Weapon mGun;

	private float mStartY;
    private int mDirection; // 0 for ahead, 1 for right, -1 for left

    private float mGameStartTime;

    private Quaternion mStartRotation;
    private Quaternion mTargetRotation;
    private Quaternion mZeroRotation;
    private Quaternion mLeftRotation;
    private Quaternion mRightRotation;
    [SerializeField] private float mRotateTime = 0;

    private const int SHIP_TILT_FACTOR = 10; //tilt when turning in degrees

	public Weapon Weapon { get { return mGun; } }
	//public int Column { get; private set; }

	void Start() 
	{
        //make start position absolute
		mStartY = transform.position.y; 

		// Look for the gun
		mGun = GetComponentInChildren<Weapon>();
        
        mDirection = 0;

        //Setup our roation quarternions
        mZeroRotation = transform.rotation;
        mTargetRotation = mZeroRotation;

        transform.Rotate(new Vector3(0, 0, SHIP_TILT_FACTOR));
        mLeftRotation = transform.rotation;

        transform.Rotate(new Vector3(0, 0,-2 * SHIP_TILT_FACTOR));
        mRightRotation = transform.rotation;

        transform.rotation = mZeroRotation;

        //Ensure the planet doesnt start moving without us
        PlanetGraphics.Move = false;

        //signup for game state changes
        GameLogic.OnStateChange += OnGameStateChange;
    }

    private void OnGameStateChange(GameLogic.State s)
    {
        if(s == GameLogic.State.Game)
        {
            mGameStartTime = Time.time;
            StartCoroutine(LaunchShip());
        }
    }

    private IEnumerator LaunchShip()
    {
        float timeRequired = 6.0f / GameLogic.PlayerSpeed;

        while(Time.time - mGameStartTime < timeRequired)
        {
            transform.Translate(new Vector3(0, GameLogic.PlayerSpeed * GameLogic.GameDeltaTime, 0));
            yield return new WaitForEndOfFrame();
        }
        PlanetGraphics.Move = true;
    }

    void Update()
	{
		Vector3 position = transform.position;
		if( mDirection != 0 )
        {
            position.x += GameLogic.GameDeltaTime * GameLogic.PlayerSpeed * mDirection * Mathf.Sin(SHIP_TILT_FACTOR * Mathf.PI / 90);
            transform.position = position;
		}

        mRotateTime += GameLogic.GameDeltaTime * RotationSpeed;
        transform.rotation = Quaternion.Slerp(mStartRotation, mTargetRotation, mRotateTime);
	}

	public void Reset()
	{
		Vector3 position = new Vector3( 0.0f, mStartY, 0.0f );
		transform.position = position;
	}

	public void Fire()
	{
		if( mGun != null )
		{
			Vector3 position = transform.position;
			position.y += FireOffset;
			mGun.Fire( position );
		}
	}

	public void MoveLeft()
    {
        mDirection--;

        if (mDirection < -1)
            mDirection = -1;

        UpdateRotationTarget();
    }

	public void MoveRight()
	{
        mDirection++;

        if (mDirection > 1)
            mDirection = 1;

        UpdateRotationTarget();
    }

    private void UpdateRotationTarget()
    {
        //store our current rotation as a base
        //(keeps rotations smooth when someone switches half way through one)
        mStartRotation = transform.rotation;

        mRotateTime = 0;

        switch(mDirection)
        {
            case -1:
                mTargetRotation = mLeftRotation;
                break;
            case 0:
                mTargetRotation = mZeroRotation;
                break;
            case 1:
                mTargetRotation = mRightRotation;
                break;
        }
    }
}
