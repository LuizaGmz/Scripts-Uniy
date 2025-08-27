using System.Collections;                     // Importa coleções não genéricas
using System.Collections.Generic;             // Importa coleções genéricas (List<T>, etc.)
using UnityEngine;                            // Importa funções e classes do Unity
using UnityEngine.UI;                         // Importa os elementos de interface (UI), como Slider

[RequireComponent(typeof(Rigidbody2D))]       // Garante que o objeto tenha um Rigidbody2D
[RequireComponent(typeof(Animator))]          // Garante que o objeto tenha um Animator
public class Monster : MonoBehaviour          // Classe do monstro, herda de MonoBehaviour
{
    [Header("Controller")]                    
    public Entity entity;                     // Dados da entidade (status, vida, mana, etc.)
    public GameManager manager;               // Referência ao GameManager

    [Header("Patrol")]                        
    public List<Transform> waypointList;      // Lista de pontos (waypoints) que o monstro vai patrulhar
    public float arrivalDistance = 0.5f;      // Distância mínima para considerar que chegou ao waypoint
    public float waitTime = 5;                // Tempo de espera em cada waypoint
    public int waypointID;                    // Identificador do conjunto de waypoints

    // Variáveis privadas para controle de patrulha
    Transform targetWapoint;                  // Próximo waypoint a ser seguido
    int currentWaypoint = 0;                  // Índice do waypoint atual
    float lastDistanceToTarget = 0f;          // Última distância registrada até o waypoint
    float currentWaitTime = 0f;               // Tempo de espera atual

    [Header("Experience Reward")]
    public int rewardExperience = 10;         // Experiência que o player ganha ao matar o monstro
    public int lootGoldMin = 0;               // Ouro mínimo que o monstro pode dropar
    public int lootGoldMax = 10;              // Ouro máximo que o monstro pode dropar

    [Header("Respawn")]                       
    public GameObject prefab;                 // Prefab do monstro (para respawnar)
    public bool respawn = true;               // Se o monstro vai renascer depois de morrer
    public float respawnTime = 10f;           // Tempo até o respawn

    [Header("UI")]                            
    public Slider healthSlider;               // Barra de vida do monstro

    Rigidbody2D rb2D;                         // Referência ao Rigidbody2D
    Animator animator;                        // Referência ao Animator

    private void Start()                      // Função chamada no início
    {
        rb2D = GetComponent<Rigidbody2D>();   // Pega o Rigidbody2D do monstro
        animator = GetComponent<Animator>();  // Pega o Animator do monstro
        manager = GameObject.Find("GameManager").GetComponent<GameManager>(); // Acha o GameManager na cena

        // Calcula vida e mana baseado nos atributos e regras do GameManager
        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);

        // Inicializa vida e mana atuais
        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;

        // Configura a barra de vida
        healthSlider.maxValue = entity.maxHealth;
        healthSlider.value = healthSlider.maxValue;

        // Procura waypoints na cena com o mesmo ID
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            int ID = obj.GetComponent<WaypointID>().ID;
            if (ID == waypointID)
            {
                waypointList.Add(obj.transform);
            }
        }

        // Define o tempo de espera inicial e o primeiro waypoint
        currentWaitTime = waitTime;
        if(waypointList.Count > 0)
        {
            targetWapoint = waypointList[currentWaypoint];
            lastDistanceToTarget = Vector2.Distance(transform.position, targetWapoint.position);
        }
    }

    private void Update()                     // Chamado a cada frame
    {
        if (entity.dead)                      // Se o monstro estiver morto, não faz nada
            return;

        if(entity.currentHealth <= 0)         // Se a vida acabar, mata o monstro
        {
            entity.currentHealth = 0;
            Die();
        }

        healthSlider.value = entity.currentHealth; // Atualiza a barra de vida

        if (!entity.inCombat)                 // Se não estiver em combate
        {
            if(waypointList.Count > 0)        // E tiver waypoints
            {
                Patrol();                     // Patrulha
            }
            else
            {
                animator.SetBool("isWalking", false); // Se não tiver, fica parado
            }
        }
        else                                  // Se estiver em combate
        {
            if (entity.attackTimer > 0)       // Controla o tempo de ataque
                entity.attackTimer -= Time.deltaTime;

            if (entity.attackTimer < 0)
                entity.attackTimer = 0;

            if(entity.target != null && entity.inCombat) // Se tiver alvo
            {
                if (!entity.combatCoroutine)  // Se não tiver coroutine rodando
                    StartCoroutine(Attack()); // Inicia a corrotina de ataque
            }
            else
            {
                entity.combatCoroutine = false; // Reseta status
                StopCoroutine(Attack());        // Para a corrotina
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider) // Detecta player dentro da área do monstro
    {
        if(collider.tag == "Player" && !entity.dead) 
        {
            entity.inCombat = true;                  // Inicia combate
            entity.target = collider.gameObject;     // Define o player como alvo
            entity.target.GetComponent<BoxCollider2D>().isTrigger = true; // Ajusta colisão
        }   
    }

    private void OnTriggerExit2D(Collider2D collider) // Detecta saída do player da área do monstro
    {
        if (collider.tag == "Player")
        {
            entity.inCombat = false;                 // Sai do combate
            if (entity.target)                       // Se tinha alvo, limpa
            {
                entity.target.GetComponent<BoxCollider2D>().isTrigger = false;
                entity.target = null;
            }
        }
    }

    void Patrol()                                     // Função de patrulha
    {
        if (entity.dead)                              // Não patrulha morto
            return;

        float distanceToTarget = Vector2.Distance(transform.position, targetWapoint.position);

        if(distanceToTarget <= arrivalDistance || distanceToTarget >= lastDistanceToTarget)
        {
            animator.SetBool("isWalking", false);

            if(currentWaitTime <= 0)                 // Troca de waypoint
            {
                currentWaypoint++;

                if (currentWaypoint >= waypointList.Count)
                    currentWaypoint = 0;

                targetWapoint = waypointList[currentWaypoint];
                lastDistanceToTarget = Vector2.Distance(transform.position, targetWapoint.position);

                currentWaitTime = waitTime;
            }
            else
            {
                currentWaitTime -= Time.deltaTime;   // Espera antes de andar de novo
            }
        }
        else
        {
            animator.SetBool("isWalking", true);
            lastDistanceToTarget = distanceToTarget;
        }

        // Define a direção para o Animator e move o monstro
        Vector2 direction = (targetWapoint.position - transform.position).normalized;
        animator.SetFloat("input_x", direction.x);
        animator.SetFloat("input_y", direction.y);

        rb2D.MovePosition(rb2D.position + direction * (entity.speed * Time.fixedDeltaTime));
    }

    IEnumerator Attack()                              // Corrotina de ataque
    {
        entity.combatCoroutine = true;

        while (true)
        {
            yield return new WaitForSeconds(entity.cooldown); // Espera o cooldown

            if (entity.target != null && !entity.target.GetComponent<Player>().entity.dead)
            {
                animator.SetBool("attack", true);     // Ativa animação de ataque

                float distance = Vector2.Distance(entity.target.transform.position, transform.position);

                if (distance <= entity.attackDistance) // Se estiver perto o suficiente
                {
                    int dmg = manager.CalculateDamage(entity, entity.damage); 
                    int targetDef = manager.CalculateDefense(entity.target.GetComponent<Player>().entity, entity.target.GetComponent<Player>().entity.defense);
                    int dmgResult = dmg - targetDef;  // Calcula dano final

                    if (dmgResult < 0)
                        dmgResult = 0;

                    Debug.Log("Inimigo atacou o player, Dmg: " + dmgResult);
                    entity.target.GetComponent<Player>().entity.currentHealth -= dmgResult; // Aplica dano
                }
            }
        }
    }

    void Die()                                        // Função chamada quando o monstro morre
    {
        entity.dead = true;
        entity.inCombat = false;
        entity.target = null;

        animator.SetBool("isWalking", false);

        // Aqui poderia adicionar experiência ao player
        // manager.GainExp(rewardExperience);

        Debug.Log("O inimigo morreu: " + entity.name);

        StopAllCoroutines();                          // Para tudo
        StartCoroutine(Respawn());                    // Inicia o respawn
    }

    IEnumerator Respawn()                             // Corrotina de respawn
    {
        yield return new WaitForSeconds(respawnTime); // Espera o tempo de respawn

        GameObject newMonster = Instantiate(prefab, transform.position, transform.rotation, null); // Cria novo monstro
        newMonster.name = prefab.name;
        newMonster.GetComponent<Monster>().entity.dead = false;
        newMonster.GetComponent<Monster>().entity.combatCoroutine = false;

        Destroy(this.gameObject);                     // Destroi o antigo
    }
}
