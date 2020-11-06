using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mouse.Utils;

public class PlayerController : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float walkingSpeed = 1.0f;
    [Space]

    private new Animator animation;
    private Rigidbody2D rb;
    private GameObject crossHair;
    private string currentAnimaton;
    public static Vector3 movement;
    private Vector3 mousePosition;
    public static Vector3 aimDirection;
    public static bool isAttacking;
    private bool isAttackPressed;

    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    private GameObject SFX;

    private void Start()
    {
        Cursor.visible = false;
        crossHair = GameObject.Find("CrossHair");
        animation = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        SFX = GameObject.Find("SFX");

    }

    void Update()
    {
        //Get Mouse Coordinates
        mousePosition = UtilsClass.GetMouseWorldPosition();
        //Form vector from character to mouse cursor
        aimDirection = (mousePosition - transform.position);
        //Get WSAD keyboard input.GetAxis applies smoothing and makes character feel like he's sliding around
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0.0f);
        //Since Diagonal movement is faster than non-diagonal movement, we have to normalzie the movement vector
        if (movement.magnitude > 1.0f)
        {
            movement.Normalize();
        }
        //Check if attacking
        if (Input.GetMouseButtonDown(0))
        {
            isAttackPressed = true;
            Debug.Log("Pressed primary button.");
            Attack();
        }

    }

    private void FixedUpdate()
    {
        MoveCharacter();

    }

    public void Attack()
    {
        SFX.GetComponent<SFX>().PlaySwordSwing();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyAI>().takeDamage(10);
            //enemy.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }
    private void MoveCharacter()
    {
        crossHair.transform.localPosition = aimDirection;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        if (angle < 0.0f) { angle += 360f; }

        if (movement.magnitude < 0.001f && !isAttacking)
        {
            if (angle > 45f && angle <= 135f)
            {
                ChangeAnimationState("IdleFront");
            }
            else if (angle > 135f && angle <= 225f)
            {
                ChangeAnimationState("IdleLeft");
            }
            else if (angle > 225f && angle <= 315f)
            {
                ChangeAnimationState("IdleBack");
            }
            else if (angle > 315f || angle <= 45)
            {
                ChangeAnimationState("IdleRight");
            }
        }
        else if (!isAttacking)
        {
            if (angle > 45f && angle <= 135f)
            {
                ChangeAnimationState("RunForward");
            }
            else if (angle > 135f && angle <= 225f)
            {
                ChangeAnimationState("RunLeft");
            }
            else if (angle > 225f && angle <= 315f)
            {
                ChangeAnimationState("RunBackward");
            }
            else if (angle > 315f || angle <= 45)
            {
                ChangeAnimationState("RunRight");
            }
        }

        if (isAttackPressed)
        {
            isAttackPressed = false;

            if (!isAttacking)
            {
                isAttacking = true;

                if (angle > 45f && angle <= 135f)
                {
                    ChangeAnimationState("AttackFront");
                }
                else if (angle > 135f && angle <= 225f)
                {
                    ChangeAnimationState("AttackLeft");
                }
                else if (angle > 225f && angle <= 315f)
                {
                    ChangeAnimationState("AttackBack");
                }
                else if (angle > 315f || angle <= 45)
                {
                    ChangeAnimationState("AttackRight");
                }

                

                //This let's the attack animation play out completely
                Invoke("AttackComplete", attackDelay);


            }

        }

        //transform.position += movement * Time.deltaTime * walkingSpeed;
        rb.velocity = new Vector2(movement.x * walkingSpeed, movement.y * walkingSpeed);


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void AttackComplete()
    {
        isAttacking = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Item"))
        {
            Destroy(other.gameObject);
            GameManager.addGold(1);
        }
    }


    /*private void MoveCharacter()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        //These Sets determine which walking animation plays
        //animation.SetFloat("Horizontal", movement.x);
        //animation.SetFloat("Vertical", movement.y);
        //animation.SetFloat("Magnitude", movement.magnitude);

        //In order for the correct idle animation to play, the previous direction needs to be checked
        //SetPreviousAnimationDirection(movement);

        transform.position += movement * Time.deltaTime;


        if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed primary button.");
    }


    //I am open to sugestions on how to avoid this mess!
    private void SetPreviousAnimationDirection(Vector3 movement)
    {
        if (movement.magnitude > 0.001f)
        {
            if (movement.x > 0.001f && movement.y > 0.001f)
            {
                if (movement.x > movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", true);
                    animation.SetBool("WasLeft", false);
                }
                else
                {
                    animation.SetBool("WasFront", true);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.x < -0.001f && movement.y < -0.001f)
            {
                if (movement.x < movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", true);
                }
                else
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", true);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.x > 0.001f && movement.y < -0.001f)
            {
                if (movement.x > -1 * movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", true);
                    animation.SetBool("WasLeft", false);
                }
                else
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", true);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.x < -0.001f && movement.y > 0.001f)
            {
                if (-1 * movement.x > movement.y)
                {
                    animation.SetBool("WasFront", false);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", true);
                }
                else
                {
                    animation.SetBool("WasFront", true);
                    animation.SetBool("WasBack", false);
                    animation.SetBool("WasRight", false);
                    animation.SetBool("WasLeft", false);
                }
            }
            else if (movement.y > 0.001f)
            {
                animation.SetBool("WasFront", true);
                animation.SetBool("WasBack", false);
                animation.SetBool("WasRight", false);
                animation.SetBool("WasLeft", false);
            }
            else if (movement.y < -0.001f)
            {
                animation.SetBool("WasFront", false);
                animation.SetBool("WasBack", true);
                animation.SetBool("WasRight", false);
                animation.SetBool("WasLeft", false);
            }
            else if (movement.x > 0.001f)
            {
                animation.SetBool("WasFront", false);
                animation.SetBool("WasBack", false);
                animation.SetBool("WasRight", true);
                animation.SetBool("WasLeft", false);
            }
            else if (movement.x < -0.001f)
            {
                animation.SetBool("WasFront", false);
                animation.SetBool("WasBack", false);
                animation.SetBool("WasRight", false);
                animation.SetBool("WasLeft", true);
            }
        }
    }
    */
    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        animation.Play(newAnimation);
        currentAnimaton = newAnimation;
    }
}
