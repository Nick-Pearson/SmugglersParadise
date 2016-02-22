﻿using UnityEngine;
using System.Collections;

//this script moves static objects s.t. it looks like the player is moving
public class RelativeObject : MonoBehaviour {
    [SerializeField] private float scale = 1.0f;
    [SerializeField] private Vector3 Velocity = Vector3.zero;

    public bool Move { private get; set; }

	// Update is called once per frame
	void Update () {
        if (Move)
        {
            //calculate our velocity relative to the player and move us by this amount
            Vector3 relativeVelocity = Velocity * scale * GameLogic.GameDeltaTime;
            relativeVelocity.y -= GameLogic.PlayerSpeed * scale * GameLogic.GameDeltaTime;

            transform.Translate(relativeVelocity);
        }
	}
}