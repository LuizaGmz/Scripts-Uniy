using System.Collections; // Importa funcionalidades para usar corrotinas (IEnumerator, WaitForSeconds, etc.)
using System.Collections.Generic; // Importa coleções genéricas (listas, dicionários, etc.)
using UnityEngine; // Importa a engine Unity (classes principais como MonoBehaviour, GameObject, Transform, etc.)
using UnityEngine.UI; // Importa recursos para trabalhar com UI (Slider, Button, etc.)

// Classe Player que herda de MonoBehaviour, ou seja, é um componente que pode ser anexado a objetos no Unity
public class Player : MonoBehaviour
{
    public bool temChave = false; // Indica se o jogador pegou a chave (começa como falso)
    public Entity entity; // Referência à entidade do jogador (provavelmente atributos como vida, mana, etc.)

    [Header("Player Regen System")] // Cabeçalho visível no Inspector
    public bool regenHPEnabled = true; // Ativa ou desativa regeneração de vida
    public float regenHPTime = 5f; // Tempo em segundos entre cada regeneração de vida
    public int regenHPValue = 5; // Quanto de vida é regenerado a cada ciclo
    public bool regenMPEnabled = true; // Ativa ou desativa regeneração de mana
    public float regenMPTime = 10f; // Tempo em segundos entre cada regeneração de mana
    public int regenMPValue = 5; // Quanto de mana é regenerado a cada ciclo

    [Header("Game Manager")]
    public GameManager manager; // Referência ao gerenciador do jogo (responsável por cálculos gerais)

    [Header("Player UI")]
    public Slider health; // Slider que representa a vida do jogador
    public Slider mana; // Slider que representa a mana do jogador
    public Slider exp; // Slider que representa a experiência do jogador

    [Header("Respawn")]
    public float respawnTime = 5; // Tempo de espera para respawn após a morte
    public GameObject prefab; // Prefab do jogador (será instanciado ao renascer)

    // Função chamada ao iniciar o jogo
    void Start()
    {
        // Verifica se o GameManager foi atribuído
        if (manager == null)
        {
            Debug.LogError("Você precisa anexar o game manager aqui no player"); // Mensagem de erro no console
            return; // Sai da função se não tiver GameManager
        }

        // Calcula vida e mana máximas com base no manager
        entity.maxHealth = manager.CalculateHealth(entity);
        entity.maxMana = manager.CalculateMana(entity);

        // Inicializa vida e mana atuais no máximo
        entity.currentHealth = entity.maxHealth;
        entity.currentMana = entity.maxMana;

        // Configura os sliders da UI com os valores máximos
        health.maxValue = entity.maxHealth;
        health.value = health.maxValue;

        mana.maxValue = entity.maxMana;
        mana.value = mana.maxValue;

        exp.value = 0; // Começa com experiência zerada

        // Inicia corrotinas de regeneração de HP e Mana
        StartCoroutine(RegenHealth());
        StartCoroutine(RegenMana());
    }

    // Função chamada a cada frame
    private void Update()
    {
        if (entity.dead) // Se o jogador já estiver morto, não faz nada
            return;

        // Se a vida chegar a 0 ou menos, chama a função de morte
        if (entity.currentHealth <= 0)
        {
            Die();
        }

        // Atualiza os sliders da UI com os valores atuais
        health.value = entity.currentHealth;
        mana.value = entity.currentMana;
    }

    // Corrotina de regeneração de vida
    IEnumerator RegenHealth()
    {
        while (true) // Loop infinito
        {
            if (regenHPEnabled) // Só regenera se estiver habilitado
            {
                if (entity.currentHealth < entity.maxHealth) // Se a vida não estiver cheia
                {
                    Debug.LogFormat("Recuperando HP do jogador"); // Mostra no console
                    entity.currentHealth += regenHPValue; // Aumenta a vida
                    yield return new WaitForSeconds(regenHPTime); // Espera o tempo configurado
                }
                else
                {
                    yield return null; // Espera até o próximo frame se já estiver cheio
                }
            }
            else
            {
                yield return null; // Espera até o próximo frame se a regeneração estiver desativada
            }
        }
    }

    // Corrotina de regeneração de mana
    IEnumerator RegenMana()
    {
        while (true) // Loop infinito
        {
            if (regenHPEnabled) // ⚠️ Aqui provavelmente era para ser "regenMPEnabled", mas está usando regenHPEnabled
            {
                if (entity.currentMana < entity.maxMana) // Se a mana não estiver cheia
                {
                    Debug.LogFormat("Recuperando MP do jogador"); // Mostra no console
                    entity.currentMana += regenMPValue; // Aumenta a mana
                    yield return new WaitForSeconds(regenMPTime); // Espera o tempo configurado
                }
                else
                {
                    yield return null; // Espera até o próximo frame
                }
            }
            else
            {
                yield return null; // Espera até o próximo frame se a regeneração estiver desativada
            }
        }
    }

    // Função chamada quando o jogador morre
    void Die()
    {
        entity.currentHealth = 0; // Garante que a vida é 0
        entity.dead = true; // Marca o jogador como morto
        entity.target = null; // Remove alvo do jogador

        StopAllCoroutines(); // Para todas as corrotinas em execução (inclusive regen)
        StartCoroutine(Respawn()); // Inicia processo de respawn
    }

    // Corrotina de respawn do jogador
    IEnumerator Respawn()
    {
        GetComponent<PlayerController>().enabled = false; // Desabilita o controle do jogador morto

        yield return new WaitForSeconds(respawnTime); // Espera o tempo de respawn

        // Cria um novo jogador (instancia o prefab)
        GameObject newPlayer = Instantiate(prefab, transform.position, transform.rotation, null);
        newPlayer.name = prefab.name; // Dá o nome do prefab ao novo objeto
        newPlayer.GetComponent<Player>().entity.dead = false; // Marca como vivo
        newPlayer.GetComponent<Player>().entity.combatCoroutine = false; // Reseta o estado de combate
        newPlayer.GetComponent<PlayerController>().enabled = true; // Habilita controle novamente

        Destroy(this.gameObject); // Destrói o jogador antigo (morto)
    }
    
    // Detecta colisão com objetos que tenham Trigger 2D
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Chave")) // Se colidir com objeto com tag "Chave"
        {
            temChave = true; // Jogador agora tem a chave
            Destroy(collision.gameObject); // Destrói o objeto chave da cena
        }
    }
}
