using System.Collections;              // Importa coleções não genéricas (ex: ArrayList)
using System.Collections.Generic;      // Importa coleções genéricas (ex: List<T>)
using UnityEngine;                     // Importa funções, classes e componentes do Unity
using System;                          // Importa recursos gerais do .NET (ex: Serializable)

[Serializable]                         // Permite que a classe apareça e seja salva no Inspector do Unity
public class Entity
{
    [Header("Name")]                   // Cabeçalho para organizar no Inspector
    public string name;                // Nome da entidade (ex: "Goblin", "Player")
    public int level;                  // Nível da entidade

    [Header("Health")]                 // Cabeçalho para atributos de vida
    public int currentHealth;          // Vida atual da entidade
    public int maxHealth;              // Vida máxima da entidade

    [Header("Mana")]                   // Cabeçalho para atributos de mana
    public int currentMana;            // Mana atual
    public int maxMana;                // Mana máxima

    [Header("Inventário")]             // Cabeçalho para itens/inventário
    public bool temChave = false;      // Indica se a entidade tem uma chave (true/false)

    [Header("Stats")]                  // Cabeçalho para estatísticas do personagem
    public int strength = 1;           // Força física
    public int resistence = 1;         // Resistência física
    public int intelligence = 1;       // Inteligência (magia)
    public int willpower = 1;          // Força de vontade
    public int damage = 1;             // Dano base
    public int defense = 1;            // Defesa
    public float speed = 2f;           // Velocidade de movimento

    [Header("Combat")]                 // Cabeçalho para atributos de combate
    public float attackDistance = 0.5f;// Distância mínima para atacar
    public float attackTimer = 1;      // Tempo de duração do ataque
    public float cooldown = 2;         // Tempo de recarga entre ataques
    public bool inCombat = false;      // Indica se a entidade está em combate
    public GameObject target;          // Alvo atual (ex: inimigo a atacar)
    public bool combatCoroutine = false;// Verifica se a corrotina de combate já está ativa
    public bool dead = false;          // Indica se a entidade está morta
}
