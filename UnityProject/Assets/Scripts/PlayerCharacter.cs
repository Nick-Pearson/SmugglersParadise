using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour 
{
	[SerializeField] private Camera GameplayCamera;
    [SerializeField] private RelativeObject PlanetGraphics;
	[SerializeField] private float FireOffset;

    //component references
	private Weapon mGun;
    private Rigidbody2D mPhysics;

	private float mStartY;

    private float mGameStartTime;

    //fill with default addons for now
    private Ship addonManager = new Ship(new Mk1FlightDeck(), new Mk1Cargo(), new RearEngineMount(new Mk1Engine(), new Mk1Engine()));

    //Rotation variables
    private int mDirection; // 0 for ahead, 1 for right, -1 for left
    private Quaternion mStartRotation;
    private Quaternion mTargetRotation;
    private Quaternion mZeroRotation;
    private Quaternion mLeftRotation;
    private Quaternion mRightRotation;
    private float mRotateTime = 0; //timer for smooth rotation
    private const float ROTATION_SPEED = 4; //speed that we turn
    private const int SHIP_TILT_FACTOR = 10; //tilt when turning in degrees

	public Weapon Weapon { get { return mGun; } }

    //INFO Properties                  (i.e. Readonly Properties to get data)
    public float PlayerSpeedY { //migrated from DifficultyCurve (PlayerSpeed)
        get { return PlayerVelocity.y; }
    } 
    public float PlayerSpeedX
    {
        get { return PlayerVelocity.x; }
    }
    public float PlayerSpeed
    {
        get { return PlayerVelocity.magnitude;  }
    }
    public Vector2 PlayerVelocity {
        get {
            return mPhysics.velocity;
        }
    }
    public float PlayerMass //total mass of the ship in tons
    {
        get
        {
            return ShipAddonMass + PlayerFuelAmount + ShipCargoMass;
        }
    }
    private float mFuelAmount;
    public float PlayerFuelAmount {
        get { return mFuelAmount;  }
        private set
        {
            if (value < 0)
                mFuelAmount = 0;
            else
                mFuelAmount = value;
        }
    } //amount of fuel left in tons


    //INPUT Properties                  (i.e. Properties exposed to be modified by input scripts)
    public float PlayerThrustPercentage { get; set; } //how much power are we applying


    //ADDON Properties                  (i.e. Properties exposed to be modified by addons)
    public float MaxThrustForce { get; set; } //how much thrust can we apply in kilo Newtons (??)

    public float ShipAddonMass { get; set; } //mass of addon elements

    public float PlayerMaxFuel { get; set; } //max capacity of fuel tanks in tons
    public float PlayerMaxFuelBurn { get; set; } //tons of fuel burnt per second at 100% power

    public float ShipMaxCargo { get; set; }

    //CARGO Properties                  (i.e. Properties exposed to be modified by cargo)
    public float ShipCargoMass { get; set; }

    void Start() 
	{
        //make start position absolute
		mStartY = transform.position.y; 

		// Look for the gun
		mGun = GetComponentInChildren<Weapon>();
        mPhysics = GetComponent<Rigidbody2D>();
        
        mDirection = 0;

        //Setup our roation quarternions
        mZeroRotation = transform.rotation;
        mTargetRotation = mZeroRotation;

        transform.Rotate(new Vector3(0, 0, SHIP_TILT_FACTOR));
        mLeftRotation = transform.rotation;

        transform.Rotate(new Vector3(0, 0,-2 * SHIP_TILT_FACTOR));
        mRightRotation = transform.rotation;

        transform.rotation = mZeroRotation;

        //apply the buffs from our addons
        addonManager.ApplyChanges(this);

        //FIXME arbitrary starting values
        PlayerFuelAmount = PlayerMaxFuel;
        PlayerThrustPercentage = 1.0f;
        ShipCargoMass = ShipMaxCargo;

        mPhysics.mass = PlayerMass;


        //Ensure the planet doesnt start moving without us
        PlanetGraphics.Move = false;

        //signup for game state changes
        GameLogic.OnStateChange += OnGameStateChange;
    }

    void FixedUpdate()
    {
        //calculate the thrust we are using
        float totalThrust = PlayerFuelAmount == 0 ? 0 :PlayerThrustPercentage * MaxThrustForce * Time.fixedDeltaTime; //total thrust in kN
        Vector2 appliedThrust = new Vector2(mDirection * Mathf.Sin(SHIP_TILT_FACTOR) * totalThrust, Mathf.Cos(SHIP_TILT_FACTOR * Mathf.Abs(mDirection)) * totalThrust);
        
        //update the physics component
        mPhysics.AddForce(appliedThrust);
        mPhysics.mass = PlayerMass;
        mPhysics.gravityScale = 1-Mathf.Clamp01(Mathf.Pow(transform.position.y - mStartY,2)/16900); //x^2 falloff of gravity
    }

    void Update()
    {
        mRotateTime += GameLogic.GameDeltaTime * ROTATION_SPEED;
        transform.rotation = Quaternion.Slerp(mStartRotation, mTargetRotation, mRotateTime);

        //burn our fuel
        PlayerFuelAmount -= PlayerMaxFuelBurn * PlayerThrustPercentage * GameLogic.GameDeltaTime;
    }

    private void OnGameStateChange(GameLogic.State s)
    {
        if(s == GameLogic.State.Game)
        {
            mGameStartTime = Time.time;
        }
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
