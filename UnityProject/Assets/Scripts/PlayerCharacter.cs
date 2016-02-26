using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float FireOffset;

    //component references
    private Weapon mGun;
    private SpacePhysics mPhysics;

    private float mGameStartTime;

    //Rotation variables
    private int mDirection; // 0 for ahead, 1 for right, -1 for left
    private Quaternion mStartRotation;
    private Quaternion mTargetRotation;
    private Quaternion mZeroRotation;
    private Quaternion mLeftRotation;
    private Quaternion mRightRotation;
    private float mRotateTime = 0; //timer for smooth rotation
    private const float ROTATION_SPEED = 4; //speed that we turn
    private const int SHIP_TILT_FACTOR = 5; //tilt when turning in degrees

    public Weapon Weapon { get { return mGun; } }

    //INFO Properties                  (i.e. Readonly Properties to get data)
    public SpacePhysics Physics
    {
        get { return mPhysics; }
    }
    public float PlayerSpeed
    { //migrated from DifficultyCurve (PlayerSpeed)
        get { return Physics.Velocity; }
    }
    private float mFuelAmount;
    public float PlayerFuelAmount
    {
        get { return mFuelAmount; }
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
        // Look for the gun
        mGun = GetComponentInChildren<Weapon>();
        mPhysics = GetComponent<SpacePhysics>();

        mDirection = 0;

        //Setup our roation quarternions
        mZeroRotation = transform.rotation;
        mTargetRotation = mZeroRotation;

        transform.Rotate(new Vector3(0, 0, SHIP_TILT_FACTOR));
        mLeftRotation = transform.rotation;

        transform.Rotate(new Vector3(0, 0, -2 * SHIP_TILT_FACTOR));
        mRightRotation = transform.rotation;

        transform.rotation = mZeroRotation;

        //apply the buffs from our addons
        GetComponent<ShipGraphics>().BaseAddon.ApplyChanges(this);

        // TODO: Fix arbitrary starting values
        PlayerFuelAmount = PlayerMaxFuel;
        PlayerThrustPercentage = 0.0f;
        ShipCargoMass = ShipMaxCargo;
        Physics.Mass = ShipAddonMass + PlayerFuelAmount;

        //signup for game state changes
        GameLogic.OnStateChange += OnGameStateChange;

        //set initial throttle to 0%
        UIManager.UISystem.ChangeThrottleValue(0);
    }

    void Update()
    {
        mRotateTime += GameLogic.GameDeltaTime * ROTATION_SPEED;
        transform.rotation = Quaternion.Slerp(mStartRotation, mTargetRotation, mRotateTime);

        //burn our fuel
        PlayerFuelAmount -= PlayerMaxFuelBurn * PlayerThrustPercentage * GameLogic.GameDeltaTime;
        UIManager.UISystem.ChangeFuelValue(PlayerFuelAmount / PlayerMaxFuel);

        //tell the phsyics how much thrust we have
        if(PlayerFuelAmount <= 0)
        {
            mPhysics.Thrust = 0;
            //TODO: do we need this arbitrary 20 anymore?
            UIManager.UISystem.ChangeThrottleValue(PlayerThrustPercentage * 20);
        }
        else
        {
            mPhysics.Thrust = PlayerThrustPercentage * MaxThrustForce;
        }

        Physics.Mass = ShipAddonMass + PlayerFuelAmount;
    }

    private void OnGameStateChange(GameLogic.State s)
    {
        if (s == GameLogic.State.Game)
        {
            mGameStartTime = Time.time;
        }
    }


    public void Reset()
    {
        Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
        transform.position = position;
    }

    public void Fire()
    {
        if (mGun != null)
        {
            Vector3 position = transform.position;
            position.y += FireOffset;
            mGun.Fire(position);
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

    public void UpdateThrottle(float val)
    {
        PlayerThrustPercentage = val;
    }

    private void UpdateRotationTarget()
    {
        //store our current rotation as a base
        //(keeps rotations smooth when someone switches half way through one)
        mStartRotation = transform.rotation;

        mRotateTime = 0;

        switch (mDirection)
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
