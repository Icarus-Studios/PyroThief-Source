using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mouse.Utils;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private float attackDelay = 0.4f;
    //[SerializeField] private float walkingSpeed = 1.0f;
    [SerializeField] public int attackDamage = 35;
    public int turretDamage = 10;
    public float walkingSpeed = 1.0f;
    public static int test = 3;


    [Space]

    private new Animator animation;
    private Rigidbody2D rb;
    private GameObject crossHair;
    private string currentAnimaton;
    public Vector3 movement;
    private Vector3 mousePosition;
    private Vector2 bowMousePosition;
    public Vector3 aimDirection;
    public bool isAttacking;
    private bool isAttackPressed;
    public static bool isShieldPressed;
    public static int shieldNum = 0;

    public Transform attackPoint;
    public Transform arrowPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    private GameObject SFX;
    public GameObject turret;
    public GameObject TurretUI;
    public GameObject shieldBox;
    private BoxCollider2D shieldCollider;
    private GameObject player;
    int facingInt;

    public static PlayerController Instance { get; private set; }
    [Space]

    [Header("Arrow Related Things")]
    public GameObject arrowFront;
    public GameObject arrowBack;
    public GameObject arrowLeft;
    public GameObject arrowRight;
    public int arrowForce = 12;
    private void Start()
    {
        Instance = this;
        Cursor.visible = false;
        crossHair = GameObject.Find("CrossHair");
        shieldCollider = shieldBox.GetComponent<BoxCollider2D>();
        shieldCollider.enabled = false;
        animation = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        SFX = GameObject.Find("SFX");
        player = GameObject.Find("Promethesus");

    }

    void Update()
    {
        //Get Mouse Coordinates
        mousePosition = UtilsClass.GetMouseWorldPosition();
        //Get Bow Mouse Coordinates
        bowMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
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
            if(!GameManager.isPaused && GameManager.Instance.shop.activeInHierarchy == false)
            {
                isAttackPressed = true;
                //Debug.Log("Pressed primary button.");
            }
            
        }
        if (Input.GetKey("left shift"))
        {
            isShieldPressed = true;
            shieldNum = 1;
            Debug.Log("player shield is true");
        }
        else
        {
            shieldNum = 0;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(TurretUI.activeInHierarchy)
                addTurret();  
        }
    }

    private void addTurret()
    {
        if(GameManager.Instance.goldAmount >= 20)
        {
            GameManager.Instance.addGold(-20);
            Vector3 playerPos = transform.position;
            Vector3 playerDirection = transform.forward;
            Quaternion playerRotation = transform.rotation;


            crossHair.transform.localPosition = aimDirection;

            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

            if (angle < 0.0f) { angle += 360f; }

            if (movement.magnitude < 0.001f)
            {
                if (angle > 45f && angle <= 135f)
                {
                    playerPos.y += 4;
                }
                else if (angle > 135f && angle <= 225f)
                {
                    playerPos.x -= 4;
                }
                else if (angle > 225f && angle <= 315f)
                {
                    playerPos.y -= 4;
                }
                else if (angle > 315f || angle <= 45)
                {
                    playerPos.x += 4;
                }
            }
            Vector3 spawnPos = playerPos + playerDirection;
            Instantiate(turret, spawnPos, playerRotation);
        }
        else
        {
            SFX.GetComponent<SFX>().PlayErrorSound();
            CinemachineShake.Instance.ShakeCamera(10f, .3f);
        }
        
    }

    private void FixedUpdate()
    {
        player.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        //Is character moving? Set animation if so
        if (movement.magnitude > 0.001f)
        {
            animation.SetBool("IsMoving", true);
        }
        else
        {
            animation.SetBool("IsMoving", false);
        }

        //Move cross-hair into correct spot
        crossHair.transform.localPosition = aimDirection;

        //Determine which way character is facing depending on angle of mouse
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        if (angle < 0.0f) { angle += 360f; }

        //Facing up
        if (angle > 45f && angle <= 135f)
        {
            facingInt = 1;
            animation.SetInteger("angle", facingInt);
            attackPoint.transform.localPosition = new Vector3(0, .8f, 0);
            arrowPoint.transform.localPosition = new Vector3(-0.5f, .8f, 3);
        }
        //Facing left
        else if (angle > 135f && angle <= 225f)
        {
            facingInt = 2;
            animation.SetInteger("angle", facingInt);
            attackPoint.transform.localPosition = new Vector3(-1.1f, 0, 0);
            arrowPoint.transform.localPosition = new Vector3(-1.1f, 0.5f, 3);
        }
        //Facing down
        else if (angle > 225f && angle <= 315f)
        {
            facingInt = 3;
            animation.SetInteger("angle", facingInt);
            attackPoint.transform.localPosition = new Vector3(0, -1.0f, 0);
            arrowPoint.transform.localPosition = new Vector3(-0.3f, -1.5f, 3);
        }
        //Facing right
        else if (angle > 315f || angle <= 45)
        {
            facingInt = 4;
            animation.SetInteger("angle", facingInt);
            attackPoint.transform.localPosition = new Vector3(1.1f, 0, 0);
            arrowPoint.transform.localPosition = new Vector3(0.5f, 0.5f, 3);
        }

        if (isAttackPressed)
        {
            isAttackPressed = false;

            if (!isAttacking)
            {
                isAttacking = true;
                //Debug.Log("Attacking");

                if (GameManager.Instance.swordActive)
                {
                    animation.SetTrigger("IsAttacking");
                    SFX.GetComponent<SFX>().PlaySwordSwing();
                    Attack();
                }
                else
                {
                    animation.SetTrigger("IsShooting");
                    //SFX.GetComponent<SFX>().PlaySwordSwing(); Play arrow-3.wav here!
                    if(facingInt == 1)
                    {
                        Attack();
                        GameObject arrow = Instantiate(arrowFront, arrowPoint.position, Quaternion.identity);
                        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, arrowForce);
                    }
                    else if(facingInt == 2)
                    {
                        Attack();
                        GameObject arrow = Instantiate(arrowLeft, arrowPoint.position, Quaternion.identity);
                        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(-arrowForce, 0.0f);
                    }
                    else if(facingInt == 3)
                    {
                        Attack();
                        GameObject arrow = Instantiate(arrowBack, arrowPoint.position, Quaternion.identity);
                        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -arrowForce);
                    }
                    else if (facingInt == 4)
                    {
                        Attack();
                        GameObject arrow = Instantiate(arrowRight, arrowPoint.position, Quaternion.identity);
                        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(arrowForce, 0.0f);
                    }
                }
                Invoke("AttackComplete", attackDelay);
            }
        }

        if (isShieldPressed)
        {
            animation.SetBool("IsDefending", true);
            shieldCollider.enabled = true;
            if(facingInt == 1)
            {
                shieldCollider.size = new Vector2(2.0f, .75f);
                shieldCollider.offset = new Vector2(0, .8f);
            }
            else if(facingInt == 2)
            {
                shieldCollider.size = new Vector2(0.75f, 2.0f);
                shieldCollider.offset = new Vector2(-0.46f, .42f);
            }
            else if (facingInt == 3)
            {
                shieldCollider.size = new Vector2(2.0f, .75f);
                shieldCollider.offset = new Vector2(0, -0.75f);
            }
            else if (facingInt == 4)
            {
                shieldCollider.size = new Vector2(0.75f, 2.0f);
                shieldCollider.offset = new Vector2(0.46f, 0.42f);
            }
            rb.velocity = new Vector2(0, 0);
        }
        else
        {
            shieldCollider.enabled = false;
            animation.SetBool("IsDefending", false);
        }
        //Move Bitch!
        if (!isShieldPressed)
        {
            rb.velocity = new Vector2(movement.x * walkingSpeed, movement.y * walkingSpeed);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            isShieldPressed = false;
        }
    }
    public void Attack()
    {
        //Debug.Log("Attacking!");
        //SFX.GetComponent<SFX>().PlaySwordSwing();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.gameObject.GetComponent<SoldierAI>() != null)
            {
                enemy.GetComponent<SoldierAI>().takeDamage(attackDamage);
            }
            else if (enemy.gameObject.GetComponent<MinotaurAI>() != null)
            {
                enemy.GetComponent<MinotaurAI>().takeDamage(attackDamage);
            }
            else if (enemy.gameObject.GetComponent<ArcherAI>() != null)
            {
                enemy.GetComponent<ArcherAI>().takeDamage(attackDamage);
            }
            else if (enemy.gameObject.GetComponent<CerberusAI>() != null)
            {
                Debug.Log("You applied:" + attackDamage + "damage.");
                enemy.GetComponent<CerberusAI>().takeDamage(attackDamage);
            }


            //enemy.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    }

    private void AttackComplete()
    {
        isAttacking = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Gold"))
        {
            SFX.GetComponent<SFX>().PlayPickupSound();
            Destroy(other.gameObject);
            other.gameObject.GetComponent<addGoldAmount>().addGoldToCount();
        }
        else if (other.gameObject.CompareTag("Heart"))
        {
            SFX.GetComponent<SFX>().PlayPickupSound();
            Destroy(other.gameObject);
            other.gameObject.GetComponent<addHPAmount>().addHPToCount();
        }
        else if (other.gameObject.CompareTag("Key"))
        {
            SFX.GetComponent<SFX>().PlayPickupSound();
            Destroy(other.gameObject);
            GameManager.Instance.updateRounds();
        }
    }

    public int getTurretDamage() { return turretDamage; }
}
