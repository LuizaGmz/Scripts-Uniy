using System.Collections;                     // Importa coleções genéricas (listas, arrays dinâmicos, etc.)
using System.Collections.Generic;             // Importa coleções mais avançadas (List<T>, Dictionary, etc.)
using UnityEngine;                            // Biblioteca principal do Unity para componentes, física, etc.
 
[RequireComponent(typeof(Rigidbody2D))]       // Garante que o objeto terá um Rigidbody2D anexado (senão o Unity adiciona automaticamente)
[RequireComponent(typeof(Player))]            // Garante que o objeto terá o componente Player anexado
public class PlayerController : MonoBehaviour // Classe que controla o jogador, herda de MonoBehaviour para funcionar no Unity
{
    [HideInInspector] public Player player;   // Referência ao script Player, oculta no inspector mas pública para outros scripts
    public Animator playerAnimator;           // Referência ao Animator, para controlar as animações do personagem
    float input_x = 0;                        // Entrada do jogador no eixo horizontal (A/D ou ←/→)
    float input_y = 0;                        // Entrada do jogador no eixo vertical (W/S ou ↑/↓)
    bool isWalking = false;                   // Indica se o jogador está andando ou parado

    Rigidbody2D rb2D;                         // Referência ao Rigidbody2D do jogador, usado para movimentação física
    Vector2 movement = Vector2.zero;          // Vetor que guarda a direção do movimento do jogador
 
    // Start é chamado na primeira frame de execução
    void Start()
    {
        isWalking = false;                    // Começa parado
        rb2D = GetComponent<Rigidbody2D>();   // Pega o Rigidbody2D anexado no mesmo objeto
        player = GetComponent<Player>();      // Pega o componente Player do mesmo objeto
    }
 
    // Update é chamado uma vez por frame
    void Update()
    {
        input_x = Input.GetAxisRaw("Horizontal"); // Pega entrada horizontal crua (-1, 0, 1)
        input_y = Input.GetAxisRaw("Vertical");   // Pega entrada vertical crua (-1, 0, 1)
        isWalking = (input_x != 0 || input_y != 0); // Se há entrada em qualquer eixo, o player está andando
        movement = new Vector2(input_x, input_y);   // Cria o vetor de movimento baseado na entrada
 
        if (isWalking)                             // Se está andando...
        {
            playerAnimator.SetFloat("input_x", input_x); // Atualiza a animação com a direção no X
            playerAnimator.SetFloat("input_y", input_y); // Atualiza a animação com a direção no Y
        }
 
        playerAnimator.SetBool("isWalking", isWalking); // Define se está andando ou parado na animação
 
        // Controle do cooldown do ataque
        if (player.entity.attackTimer < 0)             // Se o timer ficou negativo...
            player.entity.attackTimer = 0;             // Reseta para zero
        else
            player.entity.attackTimer -= Time.deltaTime; // Diminui o tempo do cooldown com o tempo real do jogo
 
        // Se o cooldown acabou e o player está parado
        if(player.entity.attackTimer == 0 && !isWalking)
        {
            if (Input.GetButtonDown("Fire1"))              // Se o botão de ataque ("Fire1") foi pressionado
            {
                playerAnimator.SetTrigger("attack");       // Dispara a animação de ataque
                player.entity.attackTimer = player.entity.cooldown; // Reseta o cooldown de ataque
                Attack();                                  // Chama a função que lida com o ataque
            }
        }         
    }
    
    // FixedUpdate é chamado em intervalos fixos (ideal para física)
    private void FixedUpdate()
    {
        // Move o player de acordo com o vetor de movimento, velocidade e tempo fixo
        rb2D.MovePosition(rb2D.position + movement * player.entity.speed * Time.fixedDeltaTime);
    }
 
    // Detecta colisão enquanto o player está dentro da área de trigger
    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.tag == "Enemy")                // Se o objeto colidido tem tag "Enemy"
        {
            player.entity.target = collider.transform.gameObject; // Define o inimigo como alvo
        }
    }
 
    // Quando o player sai da colisão com o inimigo
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.transform.tag == "Enemy")               // Se saiu da área de um inimigo
        {
            player.entity.target = null;                     // Remove o alvo
        }
    }
 
    // Função de ataque
    void Attack()
    {
        if (player.entity.target == null)    // Se não há alvo, sai da função
            return;
 
        Monster monster = player.entity.target.GetComponent<Monster>(); // Pega o script do monstro alvo
 
        if (monster.entity.dead)             // Se o inimigo já está morto
        {
            player.entity.target = null;     // Remove o alvo
            return;
        }
 
        float distance = Vector2.Distance(transform.position, player.entity.target.transform.position); // Calcula a distância até o inimigo
 
        if(distance <= player.entity.attackDistance) // Se estiver dentro do alcance de ataque
        {
            int dmg = player.manager.CalculateDamage(player.entity, player.entity.damage); // Calcula dano do player
            int enemyDef = player.manager.CalculateDefense(monster.entity, monster.entity.defense); // Calcula defesa do inimigo
            int result = dmg - enemyDef; // Dano final = ataque - defesa
 
            if (result < 0) result = 0; // Impede dano negativo
 
            Debug.Log("Player dmg: " + result.ToString()); // Mostra no console o dano causado
            monster.entity.currentHealth -= result;        // Reduz a vida atual do inimigo
            monster.entity.target = this.gameObject;       // Faz o inimigo mirar no player
        }
    }
}
