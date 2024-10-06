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


    public float attackRange = 0.3f; // ����������, �� ������� ��������� ������ ���������
    public GameObject attackPoint;

    public float attackCooldown = 1f; // �������� ����� �������
    public int attackDamage = 27; // ���� �� �����


    private bool isAttacking = false; // ����, �����������, ��� ��������� ������ �������
    private float lastAttackTime; // ����� ��������� �����


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
                // ���� ����� ��������� � ���� �����, ��������� �������
                AttackPlayer();
            }
            else
            {
                // ����� ��������� ��������� � ������
                MoveTowardsPlayer();
            }
        }

        void MoveTowardsPlayer()
        {
            //����� ��� �������� ���������� �� X
            GoToX();

            //������������ Y ����� ������� � ���� �� ����� �� �������� �������� Y, ��� ���� X ������ ���� ���������� +-
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
                // �������� ������� ������� ����
                Vector3 currentPosition = transform.position;

                // �������� ������� ������� ������
                Vector3 targetPosition = player.position;

                // ������ ���, ����� ��� �������� ������ �� ��� X, �������� Y ��� ���������
                targetPosition.y = currentPosition.y;
                targetPosition.z = currentPosition.z;

                // ������������ ���� � ������ (���� �����)
                if (targetPosition.x > currentPosition.x)
                {
                    transform.localScale = new Vector3(-(float)0.5, (float)0.5, (float)0.5); // ������������ �����
                }
                else if (targetPosition.x < currentPosition.x)
                {
                    transform.localScale = new Vector3((float)0.5, (float)0.5, (float)0.5); // ������������ ������
                }

                // ���������� ���� � ������ �� ��� X
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
                // ��������� ������� ������
                // ����� ����� ���� ����������� �������� ����� ��� ������ ��������

                // ������� ���� ������
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
            // ������������� ����� ��������� �����
            lastAttackTime = Time.time;

            // ��������, ��� ��������� ������ �������
            isAttacking = true;

            // ������������� ���� ����� � false ����� ��������� ��������, ����� ��������� ��� ��������� ��������� �����
            Invoke(nameof(FinishAttack), 0.5f);
        }

        //void PushAway(Vector2 pushFrom, float pushPower)
        //{
        //    // ���� ��� ������������� Rigidbody2D, �� ������ �� �������
        //    if (_rb == null || pushPower == 0)
        //    {
        //        return;
        //    }

        //    // ���������� � ����� ����������� ������ �������� ������
        //    // � ����� ����������� ���� ������, ����� ����� ���� ����� ������� ���� "�������"
        //    var pushDirection = (pushFrom - transform.position).Normalize();

        //    // ������� ������ � ������ ����������� � ����� pushPower
        //    _rb.AddForce(pushDirection * pushPower);
        //}

    }
}
