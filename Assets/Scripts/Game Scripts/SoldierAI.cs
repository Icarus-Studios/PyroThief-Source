using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SoldierAI : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float health = 100f;
    [SerializeField] private AIPath AIPath;
    [SerializeField] private float minAttackDistance = 2.0f;

    private ParticleSystem blood;
    public SpriteRenderer healthBar;
    public Transform attackPoint;
    public float attackRange = 1f;

    Vector3 localScale;
    public GameObject plusOneReward;
    public GameObject plusFiveReward;
    public GameObject plusTenReward;
    public GameObject plusTwentyReward;
    public GameObject plusFiftyReward;
    public GameObject plus100Reward;
    public GameObject plus500Reward;
    public GameObject plus10Health;
    public GameObject plus50Health;
    private GameObject SFX;
    private bool hitShield;

    private new Animator animation;
    public static bool isAttacking;
    int facingInt;


    void Start()
    {
        animation = GetComponentInChildren<Animator>();
        blood = gameObject.GetComponentInChildren(typeof(ParticleSystem), true) as ParticleSystem;
        localScale = healthBar.transform.localScale;
        SFX = GameObject.Find("SFX");
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

        //float distanceToChar = Vector3.Distance(AIPath.destination, transform.position);
        float distanceToChar = ((Vector2)(AIPath.destination - transform.position)).magnitude;
        //Debug.Log("Distance to Char:" + distanceToChar);
        if (distanceToChar < minAttackDistance)
        {
            //Debug.Log("I should be attacking!");
            if (!isAttacking)
            {
                AIPath.canMove = false;
                //Debug.Log("Attacking");
                animation.SetTrigger("IsAttacking");
                isAttacking = true;
                SFX.GetComponent<SFX>().PlaySwordSwing();
                Attack();
            }
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
            attackPoint.transform.localPosition = new Vector3(0, .8f, 0);
        }
        //Facing left
        else if (angle > 135f && angle <= 225f)
        {
            facingInt = 2;
            animation.SetInteger("angle", facingInt);
            attackPoint.transform.localPosition = new Vector3(-1.1f, 0, 0);
        }
        //Facing down
        else if (angle > 225f && angle <= 315f)
        {
            facingInt = 3;
            animation.SetInteger("angle", facingInt);
            attackPoint.transform.localPosition = new Vector3(0, -1.0f, 0);
        }
        //Facing right
        else if (angle > 315f || angle <= 45)
        {
            facingInt = 4;
            animation.SetInteger("angle", facingInt);
            attackPoint.transform.localPosition = new Vector3(1.1f, 0, 0);
        }
    }

    void AttackComplete()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach(Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Shield"))
            {
                if (hit.enabled)
                {
                    hitShield = true;
                }
                else
                {
                    hitShield = false;
                }
            }
        }
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player") && !hitShield)
            {
                Debug.Log("Hitting player!");
                GameManager.Instance.updateHP(-attackDamage);
                Rigidbody2D rb = GameObject.Find("Promethesus").GetComponent<Rigidbody2D>();
                if (facingInt == 1) rb.AddForce(new Vector2(0, 1) * 20);
                else if (facingInt == 2) rb.AddForce(new Vector2(-1, 0) * 20f);
                else if (facingInt == 3) rb.AddForce(new Vector2(0, -1) * 20f);
                else if (facingInt == 4) rb.AddForce(new Vector2(1, 0) * 20f);
            }
            else if(hit.CompareTag("Player") && hitShield)
            {
                Debug.Log("Hitting shield!");
                CinemachineShake.Instance.ShakeCamera(10f, .3f);
                Rigidbody2D rb = GameObject.Find("Promethesus").GetComponent<Rigidbody2D>();
                //if (facingInt == 1) rb.AddForce(new Vector2(0, 1) * 2000f);
                //else if (facingInt == 2) rb.AddForce(new Vector2(-1, 0) * 20f);
                //else if (facingInt == 3) rb.AddForce(new Vector2(0, -1) * 20f);
                //else if (facingInt == 4) rb.AddForce(new Vector2(1, 0) * 20f);
                hitShield = false;
            }
        }
        SFX.GetComponent<SFX>().PlaySwordSwing();
        isAttacking = false;
        AIPath.canMove = true;
    }

    public void Attack()
    {
        if (isAttacking)
        {
            Invoke("AttackComplete", attackDelay);           
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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
        else if (collision.gameObject.tag == "Projectile")
        {
            takeDamage(PlayerController.Instance.attackDamage);
        }
    }
}
