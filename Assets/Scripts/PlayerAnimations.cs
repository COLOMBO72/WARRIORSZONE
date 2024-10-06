using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private float _speed = 12.5f;
    [SerializeField] private float jumpForce = 5f;

    public GameObject attackPoint;
    public float attackRange = 0.3f;
    public float attackDamage;
    public LayerMask enemies;

    float attackPush = 0.2f;

    private Vector3 movement;
    private Animator _anim;
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    bool isGrounded = false;

    int playerObject, collideObject, enemyObject, camObject, verticalObject;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        playerObject = LayerMask.NameToLayer("Player");
        collideObject = LayerMask.NameToLayer("Ground");
        enemyObject = LayerMask.NameToLayer("Enemy");
        camObject = LayerMask.NameToLayer("Collide");
        verticalObject = LayerMask.NameToLayer("VerticalObject");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AttackDjab1();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AttackSparta();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isGrounded)
            {
                _anim.SetBool("isSit", true);
                

            }
            else
            {

            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (isGrounded)
            {
                _anim.SetBool("isSit", false);
                
            }
            else
            {

            }
        }
        movement = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        transform.position += _speed * Time.deltaTime * movement;
        // KILL WHEN JUMP OUT MAP
        if (transform.position.y < -6f)
        {
            GetComponent<PlayerHealth>().health = 0;
        }

        // JUMP
        isGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1.74f, 0.27f), CapsuleDirection2D.Horizontal, 0, groundLayer);

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            _anim.SetTrigger("StartJump");
            _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        if (isGrounded == true)
        {
            _anim.SetBool("isJump", false);
        }
        else
        {
            _anim.SetBool("isJump", true);
            _anim.SetBool("isSit", false);

        }


        //MOVEMENTS LEFT - RIGHT
        if (movement.x > 0)
        {
            transform.localScale = new Vector3((float)0.5, (float)0.5, (float)0.5);
            _anim.SetBool("isMoving", true);
            _anim.SetBool("isSit", false);

        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3((float)-0.5, (float)0.5, (float)0.5);
            _anim.SetBool("isMoving", true);
            _anim.SetBool("isSit", false);

        }
        else if (movement.x == 0)
        {

            _anim.SetBool("isMoving", false);
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

        //WALK THROUGH ENEMIES
        if (_rb.velocity.x  > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, enemyObject, true);
        }
        else if (_rb.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, enemyObject, true);

        }
        
        //WALK THROUGH CAM OBJECT
        if (_rb.velocity.x > 0)
        {
            Physics2D.IgnoreLayerCollision(playerObject, camObject, true);
        }
        else if (_rb.velocity.y > 0)
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

    }
    
    //Ã≈“Œƒ ”ƒ¿–¿
    void AttackDjab1()
    {
        if (isGrounded == true)
        {
            _anim.SetTrigger("Djab1");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemies);
            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("Player hit" + enemy.name);
                if (enemy.GetComponent<PlayerHealth>().health <= 0)
                {
                    enemy.GetComponent<PlayerHealth>().health = 0;
                }
                else
                {
                    enemy.GetComponent<PlayerHealth>().health -= attackDamage;
                    //enemy.transform.position = new Vector3(transform.position.x - attackPush, transform.position.y);

                }
            }
        }
        else
        {
            _anim.SetTrigger("DjabInJump");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemies);
            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("Player hit" + enemy.name);
                if (enemy.GetComponent<PlayerHealth>().health <= 0)
                {
                    enemy.GetComponent<PlayerHealth>().health = 0;
                }
                else
                {
                    enemy.GetComponent<PlayerHealth>().health -= attackDamage;
                }
            }
        }
    }

    void AttackSparta()
    {
        if (isGrounded == true)
        {
            _anim.SetTrigger("Sparta");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemies);
            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("Player hit" + enemy.name);
                if (enemy.GetComponent<PlayerHealth>().health <= 0)
                {
                    enemy.GetComponent<PlayerHealth>().health = 0;
                }
                else
                {
                    enemy.GetComponent<PlayerHealth>().health -= attackDamage;
                    
                }
            }
        }
        else
        {
            _anim.SetTrigger("AttackLegInJump");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemies);
            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("Player hit" + enemy.name);
                if (enemy.GetComponent<PlayerHealth>().health <= 0)
                {
                    enemy.GetComponent<PlayerHealth>().health = 0;
                }
                else
                {
                    enemy.GetComponent<PlayerHealth>().health -= attackDamage;
                }
            }
        }
    }


    //–»—”≈“ —‘≈–” œ≈–≈ƒ »√–Œ ŒÃ  Œ“Œ–¿ﬂ Œ“¬≈◊¿≈“ «¿ ”ƒ¿–
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}
