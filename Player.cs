
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEditor.Scripting;

public class Player : MonoBehaviour
{
    public Entity entity;

    [Header("Player Regen System")]
    public bool regenHPEnable = true;
    public float regenHPTime = 5f;
    public int regenHPValue = 5;

    [Header("Game Manager")]

    public GameManager manager;

    [Header("Player UI")]
    public Slider health;
    public Slider mana;
    public Slider exp;
    void Start()
    {

        if (manager == null)
        {
            Debug.LogError("Voc~e precisa anexar o gamer manager no player");
            return;
        }

        entity.maxHealth = manager.CalculateHealth(this);
        entity.maxMana = manager.CalculateMana(this);

        int dmg = manager.CalculateDamage(this, 10); //player
        int def = manager.CalculateDefence(this, 5); //inimigo

        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;

        health.maxValue = entity.maxHealth;
        health.value = health.maxValue;

        mana.maxValue = entity.maxMana;
        mana.value = mana.maxValue;

       


        exp.value = 0;
        // iniciar regenheal
        StartCoroutine(RegenHealth());

    }

    private void Update()
    {
        health.value = entity.currentHealth;
        mana.value = entity.currentMana;


        //ataque d teste
        if (Input.GetKeyDown(KeyCode.Space))
            entity.currentHealth -= 10;


    }

    IEnumerator RegenHealth()
    {
        while (true)
        {
            if (regenHPEnable)
            {
                if (entity.currentHealth < entity.maxHealth)
                {
                    Debug.LogFormat("Recuperando HP do jogador");
                    entity.currentHealth += regenHPValue;
                    yield return new WaitForSeconds(regenHPTime);
                }
                else
                {
                    yield return null;
                }

            }
            else
            {
                yield return null;
            }
        }  
    }
    

}