using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MinotaurAI : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackDelay = 2f;
    [SerializeField] private float health = 100f;
    [SerializeField] private AIPath AIPath;
    [SerializeField] private float aggroDistance = 20f; //20
    [SerializeField] private float minChargeDistance = 10f; //10
    [SerializeField] private float g_rushSpeed = 12f;
    [SerializeField] private LayerMask g_layerMask;
    [SerializeField] private float stunnedTime = 2f;

    private ParticleSystem blood;
    public SpriteRenderer healthBar;
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

    private new Animator animation;
    private new Rigidbody2D rb;
    private new BoxCollider2D box;
    private new GameObject targetObj;
    public static bool isAttacking;
    public static bool hasChargedUp;
    int facingInt;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        targetObj = GameObject.Find("Promethesus");
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
        float angle = Mathf.Atan2(AIPath.destination.y - AIPath.position.y, AIPath.destination.x - AIPath.position.x) * Mathf.Rad2Deg;
        if (angle < 0.0f) { angle += 360f; }
        //Debug.Log("Angle:" + angle + "x:" + AIPath.destination.x + "y:" + AIPath.destination.y);
        


        //Up - 1
        //Left - 2
        //Down - 3
        //Right - 4
        //Idle
        if (!isAttacking)
        {
            if (angle > 45f && angle <= 135f)
            {
                facingInt = 1;
                animation.SetInteger("angle", facingInt);
            }
            else if (angle > 135f && angle <= 225f)
            {
                facingInt = 2;
                animation.SetInteger("angle", facingInt);
            }
            else if (angle > 225f && angle <= 315f)
            {
                facingInt = 3;
                animation.SetInteger("angle", facingInt);
            }
            else if (angle > 315f || angle <= 45)
            {
                facingInt = 4;
                animation.SetInteger("angle", facingInt);
            }
            Debug.Log("Facing int:" + facingInt);
        }

        if (AIPath.remainingDistance > aggroDistance)
        {
            AIPath.canMove = false;
            animation.SetBool("IsMoving", false);
        }
        else if (AIPath.remainingDistance > minChargeDistance && !isAttacking)
        {
            AIPath.canMove = true;
            Debug.Log("Mino is walking!");
            animation.SetBool("IsMoving", true);
        }
        else if (!isAttacking)
        {
            Debug.Log("Mino is charging");
            AIPath.canSearch = false;
            AIPath.canMove = false;
            animation.SetBool("IsCharging", true);
            //animation.SetBool("IsMoving", false);
            isAttacking = true;
            StartCoroutine(waitToCharge());
        }
    }
    IEnumerator waitToCharge()
    {
        yield return new WaitForSeconds(2f);
        animation.SetBool("IsCharging", false);
        StartCoroutine(AttackComplete());
    }
    IEnumerator AttackComplete()
    {
        animation.SetBool("IsRushing", true);
        bool isPlayerHit = false;
        bool isSomethingHit = false;
        //Vector3 targetOldPosition = AIPath.destination;
        Vector2 targetOldPosition = targetObj.transform.position;
        Vector2 rayDirection = targetOldPosition - (Vector2)transform.position;
        Vector2 minoOldPosition = transform.position;
        Vector3 chargeTarget;
        RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position, rayDirection, Mathf.Infinity, g_layerMask);
        if(rayHit.collider != null)
        {
            Debug.DrawRay(transform.position, rayHit.point - (Vector2)transform.position, Color.red, 1f);
        }
        else
        {
            Debug.DrawRay(transform.position, targetOldPosition - (Vector2)transform.position, Color.blue, 1f);
        }



        if (rayHit.collider != null)
        {
            chargeTarget = (Vector3)rayHit.point;
            //Debug.Log("Collider hit is:" + rayHit.collider.gameObject.name);
        }
        else
        {
            //Debug.Log("Collider was fucking NULL!");
            chargeTarget = targetOldPosition;

            //isAttacking = false;
            //AIPath.canSearch = true;
            //yield break;
        }

        while (!isSomethingHit)
        {
            //Vector3 smoothedDelta = Vector3.MoveTowards(transform.position, chargeTarget, Time.fixedDeltaTime * g_rushSpeed);
            //rb.MovePosition((Vector2)smoothedDelta)

            Vector2 velocity = (chargeTarget - transform.position);
            velocity *= g_rushSpeed;
            rb.AddForce(velocity, ForceMode2D.Force);
            animation.SetBool("IsCharging", false);

            //rb.MovePosition((Vector2)transform.position + velocity*Time.fixedDeltaTime * g_rushSpeed);

            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(box.transform.position, 1.4f*box.size,0f);
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.CompareTag("Player"))
                {
                    animation.SetBool("IsRushing", false);
                    CinemachineShake.Instance.ShakeCamera(10f, .3f);
                    SFX.GetComponent<SFX>().PlaySwordSwing();
                    isPlayerHit = true;
                    isSomethingHit = true;
                    GameManager.Instance.updateHP(-attackDamage);
                }
                else if (hit.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                {
                    animation.SetBool("IsRushing", false);
                    CinemachineShake.Instance.ShakeCamera(10f, .3f);
                    rb.velocity = new Vector2(0f, 0f);
                    rb.AddForce(rayDirection.normalized * -100f);
                    isSomethingHit = true;
                }
            }

            yield return null;
        }
        if (isPlayerHit)
        {
            yield return new WaitForSeconds(.75f * stunnedTime);
            rb.velocity = new Vector2(0, 0);
            rb.transform.Translate(-1* rayDirection.normalized);
            yield return new WaitForSeconds(.25f * stunnedTime);
        }
        else
        {
            yield return new WaitForSeconds(stunnedTime);
        }
        Debug.Log("Mino is ready!");
  
        isAttacking = false;
        AIPath.canSearch = true;
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
            takeDamage(10);
        }
    }
}
