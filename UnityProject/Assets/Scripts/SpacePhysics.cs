﻿using UnityEngine;

public class SpacePhysics : MonoBehaviour {
    [SerializeField] private bool ApplyGravity = true;
    [SerializeField] private bool RecievesDamage = false;

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

        //basic collision detection on planet
        mVirtualHeight += mVelocity * GameLogic.GameFixedDeltaTime;
        if (mVirtualHeight <= 0.0f)
        {
            mVelocity = 0; //velocity is the exact distance to ground
            mVirtualHeight = 0;
        }
    }

    //sign up for collision events
    void OnTriggerEnter2D(Collider2D collision)
    {
        //did we collide with something that should cause us damage
        if (collision.tag == "Enemy")
        {
            if (RecievesDamage)
                SendMessage("ApplyDamage", collision.GetComponent<SpacePhysics>().Mass / 100);

            mVelocity -= ((Velocity - collision.GetComponent<SpacePhysics>().Velocity) * collision.GetComponent<SpacePhysics>().Mass) / Mass;
            ObsticalFactory.Return(collision.gameObject);
        } else if(collision.tag == "Bullet" && gameObject.tag == "Enemy") {
            //hide the bullet (we don't destory it)
            collision.GetComponent<MeshRenderer>().enabled = false;
            collision.GetComponent<BoxCollider2D>().enabled = false;


            if (RecievesDamage)
                SendMessage("ApplyDamage", GameState.BULLET_DAMAGE);
        }

    }
}
