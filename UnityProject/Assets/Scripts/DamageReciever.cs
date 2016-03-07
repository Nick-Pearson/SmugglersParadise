﻿using UnityEngine;

public class DamageReciever : MonoBehaviour {
    public int MaxHealth = 100;
    public int CurrentHealth;

    private bool isPlayer = false;

    void Start() { SetActive();  }

    //set our health to max
    void SetActive()
    {
        if (gameObject.GetComponent<PlayerCharacter>() == null)
            CurrentHealth = MaxHealth;
        else
        {
            isPlayer = true;
            CurrentHealth = (int)(MaxHealth * (1 - GameState.PlayerDamage));
        }
    }

    //add some damage
    public void ApplyDamage(float val)
    {
        CurrentHealth -= (int)Mathf.Ceil(val);

        if (isPlayer && CurrentHealth <= 0)
        {
            CurrentHealth = (int)(MaxHealth * (1 - GameState.PlayerDamage));
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameLogic>().GameOver();
        }
        else if (gameObject.tag == "Enemy" && CurrentHealth <= 0)
            ObsticalFactory.Return(gameObject);
    }
}
