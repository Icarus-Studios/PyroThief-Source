using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ArcherAI : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private float attackDelay = 0.2f;
    [SerializeField] private float health = 100f;
    [SerializeField] private AIPath AIPath;

    private ParticleSystem blood;
    public SpriteRenderer healthBar;
    public CapsuleCollider2D frontAttackRange;
    public CapsuleCollider2D leftAttackRange;
    public CapsuleCollider2D rightAttackRange;
    public CapsuleCollider2D backAttackRange;
    public float attackRange = 1f;

    Vector3 localScale;
    [Header("Rewards")]
    public GameObject plusOneReward;
    public GameObject plusFiveReward;
    public GameObject plusTenReward;
    public GameObject plusTwentyReward;
    public GameObject plusFiftyReward;
    public GameObject plus100Reward;
    public GameObject plus500Reward;
    public GameObject plus10Health;
    public GameObject plus50Health;

    [Space]
    [Header("Arrows")]
    public GameObject arrowFront;
    public GameObject arrowLeft;
    public GameObject arrowBack;
    public GameObject arrowRight;
    public int arrowForce = 12;
    public GameObject attackPoint;

    [Space]
    private GameObject SFX;
    private GameObject player;
    private new Animator animation;
    private string currentAnimation;
    public static bool isAttacking;
    int facingInt;


    void Start()
    {
        animation = GetComponentInChildren<Animator>();
        blood = gameObject.GetComponentInChildren(typeof(ParticleSystem), true) as ParticleSystem;
        localScale = healthBar.transform.localScale;
        SFX = GameObject.Find("SFX");
        player = GameObject.Find("Promethesus");
        isAttacking = false;
    }

    void Update()
    {
        localScale.x = health / 100;
        healthBar.transform.localScale = localScale;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    void checkIfAttack(CapsuleCollider2D capsule)
    {
        if (capsule.IsTouching(player.GetComponent<CircleCollider2D>()))
        {
            if(!isAttacking)
            {
                isAttacking = true;
                animation.SetTrigger("IsAttacking");
                Attack();
            }
               
            
            
        }
    }
    private void MoveCharacter()
    {
        if (AIPath.steeringTarget.magnitude > 0.001f)
        {
            animation.SetBool("IsMoving", true);
        }
        else
        {
            animation.SetBool("IsMoving", false);
        }

        //Vector2 directionEnd = (AIPath.destination.normalized);
        float angle = Mathf.Atan2(AIPath.destination.y-AIPath.position.y, AIPath.destination.x-AIPath.position.x) * Mathf.Rad2Deg;
        if (angle < 0.0f) { angle += 360f; }
        //Debug.Log("Angle:" + angle + "x:" + AIPath.destination.x + "y:" + AIPath.destination.y);


        //Facing up
        if (angle > 45f && angle <= 135f)
        {
            facingInt = 1;
            animation.SetInteger("angle", facingInt);
            frontAttackRange.enabled = true;
            leftAttackRange.enabled = false;
            rightAttackRange.enabled = false;
            backAttackRange.enabled = false;

            checkIfAttack(frontAttackRange);
        }
        //Facing left
        else if (angle > 135f && angle <= 225f)
        {
            facingInt = 2;
            animation.SetInteger("angle", facingInt);
            frontAttackRange.enabled = false;
            leftAttackRange.enabled = true;
            rightAttackRange.enabled = false;
            backAttackRange.enabled = false;

            checkIfAttack(leftAttackRange);
        }
        //Facing down
        else if (angle > 225f && angle <= 315f)
        {
            facingInt = 3;
            animation.SetInteger("angle", facingInt);
            frontAttackRange.enabled = false;
            leftAttackRange.enabled = false;
            rightAttackRange.enabled = false;
            backAttackRange.enabled = true;

            checkIfAttack(backAttackRange);
        }
        //Facing right
        else if (angle > 315f || angle <= 45)
        {
            facingInt = 4;
            animation.SetInteger("angle", facingInt);
            frontAttackRange.enabled = false;
            leftAttackRange.enabled = false;
            rightAttackRange.enabled = true;
            backAttackRange.enabled = false;

            checkIfAttack(rightAttackRange);
        }
    }

    
    
    void AttackComplete()
    {
        frontAttackRange.enabled = false;
        leftAttackRange.enabled = false;
        rightAttackRange.enabled = false;
        backAttackRange.enabled = false;
        
        if (facingInt == 1)
        {
            attackPoint.transform.localPosition = new Vector3(0.12f, 2f, 3);
            GameObject arrow = Instantiate(arrowFront, attackPoint.transform.position, Quaternion.identity);
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, arrowForce);
        }
        else if (facingInt == 2)
        {
            attackPoint.transform.localPosition = new Vector3(-1.04f, 0.64f, 3);
            GameObject arrow = Instantiate(arrowLeft, attackPoint.transform.position, Quaternion.identity);
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(-arrowForce, 0.0f);
        }
        else if(facingInt == 3)
        {
            attackPoint.transform.localPosition = new Vector3(0.12f, -2f, 3);
            GameObject arrow = Instantiate(arrowBack, attackPoint.transform.position, Quaternion.identity);
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -arrowForce);
        }
        else if(facingInt == 4)
        {
            attackPoint.transform.localPosition = new Vector3(1.0f, 0.64f, 3);
            GameObject arrow = Instantiate(arrowRight, attackPoint.transform.position, Quaternion.identity);
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(arrowForce, 0.0f);
        }
        //Debug.LogError("Attack that bish!");
        SFX.GetComponent<SFX>().PlaySwordSwing();
        isAttacking = false;

    }

    public void Attack()
    {
        if (isAttacking)
        {
            animation.SetTrigger("IsAttacking");
            Invoke("AttackComplete", attackDelay);
            animation.SetTrigger("IsAttacking");
        }
    }
    

 

    public void takeDamage(int damage)
    {
        //Debug.Log("Enemy was attacked!");
        blood.Play();
        SFX.GetComponent<SFX>().PlayDamageSound();
        health -= damage;
        if (health <= 0)
        {
            // Debug.Log("Enemy died!");
            dropItem();
            GameManager.Instance.enemiesActive--;
            Destroy(this.gameObject);
        }
    }

    public void dropItem()
    {
        int dropNum = Random.Range(1, 10);
        GameObject gold;
        switch (dropNum)
        {
            case 1:
                gold = Instantiate(plus10Health, transform.position, Quaternion.identity);
                break;
            case 2:
                gold = Instantiate(plus50Health, transform.position, Quaternion.identity);
                break;
            case 3:
                gold = Instantiate(plusTenReward, transform.position, Quaternion.identity);
                break;
            case 4:
                gold = Instantiate(plusTwentyReward, transform.position, Quaternion.identity);
                break;
            case 5:
                gold = Instantiate(plusFiftyReward, transform.position, Quaternion.identity);
                break;
            case 6:
                gold = Instantiate(plus100Reward, transform.position, Quaternion.identity);
                break;
            case 10:
                gold = Instantiate(plus500Reward, transform.position, Quaternion.identity);
                break;

            default:
                gold = Instantiate(plusFiveReward, transform.position, Quaternion.identity);
                break;
        }
        //Debug.Log(dropNum.ToString());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name.ToString());
        //Debug.Log(health.ToString());

        if (collision.gameObject.name == "Bullet(Clone)")
        {
            takeDamage(PlayerController.Instance.turretDamage);
        }
        else if (collision.gameObject.tag == "Projectile" )
        {
            takeDamage(PlayerController.Instance.attackDamage/2);
        }
    }

  
}
