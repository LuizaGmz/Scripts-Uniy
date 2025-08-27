using System;                          // Importa funcionalidades básicas do .NET (ex: Int32, Random)
using System.Collections;              // Importa coleções não genéricas (pouco usado aqui)
using System.Collections.Generic;      // Importa coleções genéricas (List<T>, etc.)
using UnityEngine;                     // Importa funções e classes do Unity (MonoBehaviour, Debug.Log, etc.)

public class GameManager : MonoBehaviour   // Classe GameManager, que vai controlar cálculos e regras do jogo
{
    public Int32 CalculateHealth(Entity entity) // Função que calcula a vida máxima da entidade
    {
        // Fórmula: (resistence * 10) + (level * 4) + 10
        Int32 result = (entity.resistence * 10) + (entity.level * 4) + 10; // Aplica a fórmula usando resistência e nível
        Debug.LogFormat("CalculateHealth: {0}", result); // Mostra o resultado no Console do Unity
        return result;                                   // Retorna a vida calculada
    }
 
    public Int32 CalculateMana(Entity entity) // Função que calcula a mana máxima da entidade
    {
        // Fórmula: (intelligence * 10) + (level * 4) + 5
        Int32 result = (entity.intelligence * 10) + (entity.level * 4) + 5; // Aplica fórmula com inteligência e nível
        Debug.LogFormat("CalculateMana: {0}", result); // Loga no Console o valor calculado
        return result;                                 // Retorna a mana calculada
    }

    public Int32 CalculateDamage(Entity entity, int weaponDamage) // Função que calcula o dano causado pela entidade
    {
        // Fórmula: (força * 2) + (dano da arma * 2) + (nível * 3) + número aleatório (1–20)
        System.Random rnd = new System.Random(); // Cria um gerador de número aleatório
        Int32 result = (entity.strength * 2) + (weaponDamage * 2) + (entity.level * 3) + rnd.Next(1, 20);
        Debug.LogFormat("CalculateDamage: {0}", result); // Mostra no Console o dano calculado
        return result;                                   // Retorna o valor final do dano
    }
 
    public Int32 CalculateDefense(Entity entity, int armorDefense) // Função que calcula a defesa da entidade
    {
        // Fórmula: (resistência * 2) + (nível * 3) + defesa da armadura
        Int32 result = (entity.resistence * 2) + (entity.level * 3) + armorDefense; // Aplica a fórmula
        Debug.LogFormat("CalculateDefense: {0}", result); // Mostra no Console o valor da defesa
        return result;                                   // Retorna a defesa calculada
    }
}
