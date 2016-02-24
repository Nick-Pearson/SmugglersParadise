using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private Camera GameplayCamera;
    [SerializeField] private float FireOffset;
    [SerializeField] private Vector2 DragCoefficient;

    //component references
    private Weapon mGun;

    private GameObject[] mEngineEffects = new GameObject[0];

    private float mGameStartTime;

    //place to store references to our addons
    //TODO: fill with default addons for now 
    private Addon baseAddon;
    public Addon BaseAddon { get; private set; }

    //physics variables
    [SerializeField]
    private Vector2 mVelocity;
    private float mGravityScale;
    [SerializeField] private Vector2 mVirtualPosition;
    private const float GRAVITATIONAL_FORCE = 9.81f;

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
    public float PlayerSpeedY
    { //migrated from DifficultyCurve (PlayerSpeed)
        get { return PlayerVelocity.y; }
    }
    public float PlayerSpeedX
    {
        get { return PlayerVelocity.x; }
    }
    public float PlayerSpeed
    {
        get { return PlayerVelocity.magnitude; }
    }
    public Vector2 PlayerVelocity
    {
        get
        {
            return mVelocity;
        }
    }
    public Vector2 PlayerPosition
    {
        get { return mVirtualPosition; }
    }
    public float PlayerMass //total mass of the ship in tons
    {
        get
        {
            return ShipAddonMass + PlayerFuelAmount + ShipCargoMass;
        }
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

    Addon buildBasicShip()
    {
        Addon fd = new Mk1FlightDeck();
        Addon cg = new Mk1Cargo();
        Addon cg2 = new Mk1Cargo();
        Addon em = new Mk1EngineMount();
        Addon e1 = new Mk1Engine();
        Addon e2 = new Mk1Engine();
        Addon em2 = new Mk1EngineMount();
        Addon e3 = new Mk1Engine();
        Addon e4 = new Mk1Engine();

        em.attach(Addon.AttachPosition.Left, e1);
        em.attach(Addon.AttachPosition.Right, e2);

        cg.attach(Addon.AttachPosition.Bottom, em);

        cg2.attach(Addon.AttachPosition.Bottom, cg);

        em2.attach(Addon.AttachPosition.Left, e3);
        em2.attach(Addon.AttachPosition.Right, e4);

        em2.attach(Addon.AttachPosition.Bottom, cg2);

        fd.attach(Addon.AttachPosition.Bottom, em2);

        return fd;
    }

    void Start()
    {
        // Look for the gun
        mGun = GetComponentInChildren<Weapon>();

        mDirection = 0;

        //Setup our roation quarternions
        mZeroRotation = transform.rotation;
        mTargetRotation = mZeroRotation;

        transform.Rotate(new Vector3(0, 0, SHIP_TILT_FACTOR));
        mLeftRotation = transform.rotation;

        transform.Rotate(new Vector3(0, 0, -2 * SHIP_TILT_FACTOR));
        mRightRotation = transform.rotation;

        transform.rotation = mZeroRotation;

        //TODO: Make this dynamic from our savegame
        BaseAddon = buildBasicShip();

        //apply the buffs from our addons
        BaseAddon.ApplyChanges(this);

        // TODO: Fix arbitrary starting values
        PlayerFuelAmount = PlayerMaxFuel;
        PlayerThrustPercentage = 0.0f;
        ShipCargoMass = ShipMaxCargo;

        //signup for game state changes
        GameLogic.OnStateChange += OnGameStateChange;

        //work out what our ship should look like
        GenerateShipGraphics();

        //set initial throttle to 0%
        UIManager.UISystem.ChangeThrottleValue(0);
    }

    void GenerateShipGraphics()
    {
        GameObject baseObject = new GameObject("ShipGraphics");
        baseObject.transform.parent = transform;
        float height = BuildAddonGraphics(0, 0, BaseAddon, baseObject);
        baseObject.transform.Translate(0, height, 0);

        //find all our engine effects
        mEngineEffects = GameObject.FindGameObjectsWithTag("Effect");
    }

    float BuildAddonGraphics(float offsetX, float offsetY, Addon a, GameObject parent)
    {
        if (a == null)
            return 0;

        //cache the properties we will need
        float width = a.getWidth();
        float height = a.getHeight();

        Debug.Log("Found" + a.getName());
        GameObject art = Resources.Load<GameObject>("Player/" + a.getName());
        GameObject instance = Instantiate(art, new Vector3(offsetX, offsetY, 0), Quaternion.identity) as GameObject;
        instance.transform.parent = parent.transform;

        Addon[] attachments = a.getAttachments();

        if (attachments.Length == 0)
            return height;

        float h0 = BuildAddonGraphics(offsetX, offsetY + height, attachments[0], instance);
        float h1 = BuildAddonGraphics(offsetX, offsetY - height, attachments[1], instance);
        float h2 = BuildAddonGraphics(offsetX + width, offsetY, attachments[2], instance);
        float h3 = BuildAddonGraphics(offsetX - width, offsetY, attachments[3], instance);

        //the total height of the unit plus addons
        float normHeight = height + h1;
        //the height of the edge cases extending bellow the addon
        float edgeHeight = h2 > h3 ? h2 : h3;

        //are our edge addons taller than us?
        return normHeight > edgeHeight ? normHeight : edgeHeight;
    }

    void FixedUpdate()
    {
        //calculate the thrust we are using
        float totalThrust = PlayerFuelAmount == 0 ? 0 : PlayerThrustPercentage * MaxThrustForce * GameLogic.GameFixedDeltaTime; //total thrust in kN
        Vector2 appliedThrust = new Vector2(mDirection * Mathf.Sin(SHIP_TILT_FACTOR * Mathf.PI / 180) * totalThrust, Mathf.Cos(SHIP_TILT_FACTOR * (Mathf.PI / 180) * Mathf.Abs(mDirection)) * totalThrust);

        mGravityScale = 1 - Mathf.Clamp01(Mathf.Pow(mVirtualPosition.y, 2) / 16900); //x^2 falloff of gravity

        //thrust minus gravity
        Vector2 netForce = new Vector2(appliedThrust.x, appliedThrust.y - (GRAVITATIONAL_FORCE * mGravityScale * PlayerMass * GameLogic.GameFixedDeltaTime));
        Vector2 netAcceleration = new Vector2(netForce.x / PlayerMass, netForce.y / PlayerMass);

        //basic collision detection on planet
        //Will this move put us bellow the ground
        mVelocity += netAcceleration;
        transform.Translate(mVelocity.x * GameLogic.GameFixedDeltaTime, 0, 0);

        //notify the UI of our new velocity
        UIManager.UISystem.ChangeSpeedValue(mVelocity.y);

        //finally apply drag
        //mVelocity = new Vector2((1 - (DragCoefficient.x * GameLogic.GameFixedDeltaTime)) * mVelocity.x, (1 - ((DragCoefficient.y + (100 * DragCoefficient.y * mGravityScale)) * GameLogic.GameFixedDeltaTime)) * mVelocity.y);
    }

    void Update()
    {
        mRotateTime += GameLogic.GameDeltaTime * ROTATION_SPEED;
        transform.rotation = Quaternion.Slerp(mStartRotation, mTargetRotation, mRotateTime);

        //burn our fuel
        PlayerFuelAmount -= PlayerMaxFuelBurn * PlayerThrustPercentage * GameLogic.GameDeltaTime;
        UIManager.UISystem.ChangeFuelValue(PlayerFuelAmount / PlayerMaxFuel);

        //update camera position if necassary
        if (mVirtualPosition.y < 8)
            GameplayCamera.transform.Translate(0, -mVelocity.y * GameLogic.GameDeltaTime, 0);

        //basic collision detection on planet
        mVirtualPosition += mVelocity * GameLogic.GameFixedDeltaTime;
        if (mVirtualPosition.y <= 0.0f)
        {
            mVelocity.y = 0; //velocity is the exact distance to ground
            mVirtualPosition.y = 0;
        }
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

        if (PlayerFuelAmount == 0)
            val = 0;

        foreach (GameObject go in mEngineEffects)
        {
            go.transform.localScale = new Vector3(go.transform.localScale.x, val, go.transform.localScale.z);
        }
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
