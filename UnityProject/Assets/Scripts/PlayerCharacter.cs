using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private float FireOffset;

    //component references
    private Weapon mGun;
    private SpacePhysics mPhysics;

    //columning variables
    private float mTargetPositon;
    private float mStartPosition;
    private GameState.Column mColumn = GameState.Column.Two;
    private float mColumnTime = 0; //timer for smooth movement

    //Rotation variables
    private Quaternion mStartRotation;
    private Quaternion mTargetRotation;
    private Quaternion mZeroRotation;
    private Quaternion mLeftRotation;
    private Quaternion mRightRotation;
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
    [HideInInspector] public float MaxThrustForce = 0; //how much thrust can we apply in kilo Newtons (??)
    [HideInInspector] public float ShipAddonMass = 0; //mass of addon elements
    [HideInInspector] public float PlayerMaxFuel = 0; //max capacity of fuel tanks in tons
    [HideInInspector] public float PlayerMaxFuelBurn = 0; //tons of fuel burnt per second at 100% power
    [HideInInspector] public float ShipMaxCargo = 0; //max capacity of the cargo hold
    [HideInInspector] public float ShipHandling = 75; //time it takes the ship to move across a column

    //CARGO Properties                  (i.e. Properties exposed to be modified by cargo)
    public float ShipCargoMass { get; set; }

    void Start()
    {
        // Look for the gun
        mGun = GetComponentInChildren<Weapon>();
        mPhysics = GetComponent<SpacePhysics>();

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
        PlayerFuelAmount = GameState.PlayerFuel;
        PlayerThrustPercentage = 0.0f;
        ShipCargoMass = GameState.PlayerCargo.GetAmount();
        Physics.Mass = ShipAddonMass + PlayerFuelAmount;

        //set initial throttle to 0%
        UIManager.UISystem.ChangeThrottleValue(0);
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(mStartRotation, mTargetRotation, mColumnTime);

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

        //move ourselves to the right column
        mColumnTime += GameLogic.GameDeltaTime * (Mathf.Clamp(mPhysics.Velocity,0, 100) / ShipHandling);
        Vector3 pos = transform.position;
        pos.x = Mathf.SmoothStep(mStartPosition, mTargetPositon, mColumnTime);
        transform.position = pos;

        if(mColumnTime < 0.5f)
        { //rotate towards the middle
            transform.rotation = Quaternion.Slerp(mStartRotation, mTargetRotation, mColumnTime * 2);
        }
        else
        { //move back to normal
            transform.rotation = Quaternion.Slerp(mTargetRotation, mZeroRotation, (mColumnTime * 2) - 1);
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
        if(mColumn != GameState.Column.One)
        {
            mColumn--;
            mStartRotation = transform.rotation;
            mTargetRotation = mLeftRotation;
            mStartPosition = transform.position.x;
            mTargetPositon -= GameLogic.ColumnSize;
            mColumnTime = 0;
        }
    }

    public void MoveRight()
    {
        if (mColumn != GameState.Column.Three)
        {
            mColumn++;
            mStartRotation = transform.rotation;
            mTargetRotation = mRightRotation;
            mStartPosition = transform.position.x;
            mTargetPositon += GameLogic.ColumnSize;
            mColumnTime = 0;
        }
    }

    public void UpdateThrottle(float val)
    {
        PlayerThrustPercentage = val;

        if (val > 0)
            GetComponent<AudioSource>().mute = false;
        else
            GetComponent<AudioSource>().mute = true;

    }
}
