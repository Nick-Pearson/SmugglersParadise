using UnityEngine;

public class SpacePhysics : MonoBehaviour {
    [SerializeField] private bool ApplyGravity = true;
    public float Mass = 1;
    public float Thrust = 0;

    private Camera mGameplayCamera;

    private bool mIsPlayer = false;
    private PlayerCharacter mPlayer;

    private float mVelocity;
    private float mGravityScale;
    private float mVirtualHeight;
    private const float GRAVITATIONAL_FORCE = 9.81f;
    private Vector2 DRAG_COEFFICIENT = new Vector2(0.5f, 0.005f);

    public float Velocity
    {
        get
        {
            return mVelocity;
        }
    }
    public float Height
    {
        get { return mVirtualHeight; }
    }

    void Awake()
    {
        mGameplayCamera = Camera.main;
        mPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();

        if (mPlayer.gameObject == gameObject)
            mIsPlayer = true;

        mVelocity = 0;
        mVirtualHeight = 0;
    }

    void FixedUpdate()
    {
        //calculate the thrust we are using
        float appliedThrust = Thrust * GameLogic.GameFixedDeltaTime; //thrust applied in kN

        if (ApplyGravity)
            mGravityScale = 1 - Mathf.Clamp01(Mathf.Pow(mVirtualHeight, 2) / 16900); //x^2 falloff of gravity (not actually acurate but good enough for our game
        else
            mGravityScale = 0;

        //thrust minus gravity
        float netAcceleration = (appliedThrust / Mass) - (GRAVITATIONAL_FORCE * mGravityScale * GameLogic.GameFixedDeltaTime);
        mVelocity += netAcceleration;
        
        //finally apply drag
        mVelocity = (1 - ((DRAG_COEFFICIENT.y + (100 * DRAG_COEFFICIENT.y * mGravityScale)) * GameLogic.GameFixedDeltaTime)) * mVelocity;
    }

    void Update()
    {
        if (!mIsPlayer)
        {
            transform.Translate(0, mVelocity - mPlayer.PlayerSpeed * GameLogic.GameDeltaTime, 0);
            return;
        }

        //update camera position if necassary
        if (mVirtualHeight < 8)
            mGameplayCamera.transform.Translate(0, -mVelocity * GameLogic.GameDeltaTime, 0);

        //basic collision detection on planet
        mVirtualHeight += mVelocity * GameLogic.GameFixedDeltaTime;
        if (mVirtualHeight <= 0.0f)
        {
            mVelocity = 0; //velocity is the exact distance to ground
            mVirtualHeight = 0;
        }
    }
}
