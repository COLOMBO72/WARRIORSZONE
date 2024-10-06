using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BotScript : MonoBehaviour
{
    private int playerObject, collideObject, enemyObject, camObject, verticalObject;
    private Rigidbody2D _rb;
    private Transform player;
    public float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private Animator _anim;
    private Vector3 movement;
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded = false;
    public LayerMask enemies;


    public float attackRange = 0.3f; // Расстояние, на котором противник начнет атаковать
    public GameObject attackPoint;

    public float attackCooldown = 1f; // Задержка между атаками
    public int attackDamage = 27; // Урон от атаки


    private bool isAttacking = false; // Флаг, указывающий, что противник сейчас атакует
    private float lastAttackTime; // Время последней атаки


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerObject = LayerMask.NameToLayer("Enemy");
        collideObject = LayerMask.NameToLayer("Ground");
        enemyObject = LayerMask.NameToLayer("Player");
        camObject = LayerMask.NameToLayer("Collide");
        verticalObject = LayerMask.NameToLayer("VerticalObject");

        _anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void Update()
    {
        
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            if (distanceToPlayer <= attackRange)
            {
                // Если игрок находится в зоне атаки, противник атакует
                AttackPlayer();
            }
            else
            {
                // Иначе противник двигается к игроку
                MoveTowardsPlayer();
            }
        }

        void MoveTowardsPlayer()
        {
            //ЗДЕСЬ ЧЕЛ ПЫТАЕТСЯ СРАВНИТЬСЯ ПО X
            GoToX();

            //СРАВНИВАЕТСЯ Y ОБЕИХ ИГРОКОВ И ЕСЛИ НЕ РАВНО ТО ПЫТАЕТСЯ СРАВНИТЬ Y, ПРИ ЭТОМ X ДОЛЖЕН БЫТЬ одинаковый +-
            if (player.position.x == transform.position.x && player.position.y > transform.position.y)
            {
                GoToY();
            }
        }

        //JUMP


        if (movement.x > 0)
        {
            transform.localScale = new Vector3((float)-0.5, (float)0.5, (float)0.5);
            _anim.SetBool("isMoving", true);
            _anim.SetBool("isSit", false);
        }
        else if (movement.x > 0)
        {
            transform.localScale = new Vector3((float)0.5, (float)0.5, (float)0.5);
            _anim.SetBool("isMoving", true);
            _anim.SetBool("isSit", false);
        }
        else if (movement.x == 0)
        {
            _anim.SetBool("isMoving", false);
        }
        // die if out map
        if (transform.position.y < -10f)
        {
            GetComponent<PlayerHealth>().health -= 100;
        }
        //JUMP THROUGH PLATFORMS
        if (_rb.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, collideObject, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerObject, collideObject, false);
        }


        //WALK ENEMY THROUGH ENEMY
        if (_rb.velocity.x > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, playerObject, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerObject, playerObject, true);

        }
        if (_rb.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, playerObject, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerObject, playerObject, true);

        }

        if (_rb.velocity.x > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, camObject, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerObject, camObject, true);
        }
        //WALK THROUGH VERTICAL OBJECTS
        if (_rb.velocity.x > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, verticalObject, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerObject, verticalObject, true);
        }

        void GoToX()
        {
            if (player != null)
            {
                _anim.SetBool("isJump", false);
                // Получаем текущую позицию бота
                Vector3 currentPosition = transform.position;

                // Получаем текущую позицию игрока
                Vector3 targetPosition = player.position;

                // Делаем так, чтобы бот двигался только по оси X, оставляя Y без изменений
                targetPosition.y = currentPosition.y;
                targetPosition.z = currentPosition.z;

                // Поворачиваем бота к игроку (если нужно)
                if (targetPosition.x > currentPosition.x)
                {
                    transform.localScale = new Vector3(-(float)0.5, (float)0.5, (float)0.5); // Поворачиваем влево
                }
                else if (targetPosition.x < currentPosition.x)
                {
                    transform.localScale = new Vector3((float)0.5, (float)0.5, (float)0.5); // Поворачиваем вправо
                }

                // Перемещаем бота к игроку по оси X
                transform.position = Vector3.MoveTowards(currentPosition, targetPosition, movementSpeed * Time.deltaTime);
            }
        }

        void GoToY()
        {
            Vector3 currentPosition = transform.position;
            
            Vector3 targetPosition = player.position;

            isGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.74f, 0.27f), CapsuleDirection2D.Horizontal, 0, groundLayer);

            if (isGrounded)
            {
                _anim.SetTrigger("StartJump");
                _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                _anim.SetBool("isJump", false);
            }
            else
            {
                _anim.SetBool("isJump", true);
                _anim.SetBool("isSit", false);
            }

            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, movementSpeed * Time.deltaTime);


        }

        void FinishAttack()
        {
            isAttacking = false;
        }

        void AttackPlayer()
        {
            if (!isAttacking && Time.time - lastAttackTime > attackCooldown)
            {
                // Противник атакует игрока
                // Здесь может быть реализована анимация атаки или другие действия

                // Наносим урон игроку
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                Debug.Log("Player hit" + player.name);
                if (playerHealth.health <= 0)
                {
                    playerHealth.health = 0;
                }
                else
                {
                    playerHealth.health -= attackDamage;
                    //PushAway(transform.position, 5f);
                }
            }
            // Устанавливаем время последней атаки
            lastAttackTime = Time.time;

            // Помечаем, что противник сейчас атакует
            isAttacking = true;

            // Устанавливаем флаг атаки в false после небольшой задержки, чтобы противник мог совершать следующую атаку
            Invoke(nameof(FinishAttack), 0.5f);
        }

        //void PushAway(Vector2 pushFrom, float pushPower)
        //{
        //    // Если нет прикреплённого Rigidbody2D, то выйдем из функции
        //    if (_rb == null || pushPower == 0)
        //    {
        //        return;
        //    }

        //    // Определяем в каком направлении должен отлететь объект
        //    // А также нормализуем этот вектор, чтобы можно было точно указать силу "отскока"
        //    var pushDirection = (pushFrom - transform.position).Normalize();

        //    // Толкаем объект в нужном направлении с силой pushPower
        //    _rb.AddForce(pushDirection * pushPower);
        //}

    }
}
