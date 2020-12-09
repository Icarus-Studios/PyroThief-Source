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
    private bool isAttacking;
    private bool hitShield;
    private int facingInt;
    private int flag;
    private MinotaurAI minoScript;


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
        setFlag();
    }

    public void setFlag()
    {
        flag = 1;
    }

    public int getFlag()
    {
        return flag;
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
        if (true)//!isAttacking)
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
            //Debug.Log(transform.name + "Facing int:" + facingInt);
        }
        //Debug.Log(transform.name + "AIPath remaining distance:" + AIPath.remainingDistance);
        //Debug.Log(transform.name + "aggrosdisatnce:" + aggroDistance);
        //Debug.Log(transform.name + "minChargedistance" + minChargeDistance);
        //Debug.Log(transform.name + "Am attacking?" + isAttacking);

        //If remaining distance is larger than the aggro distance, do nothing.
        if (AIPath.remainingDistance > aggroDistance)
        {
            AIPath.canMove = false;
            animation.SetBool("IsMoving", false);
        }
        //If remaining distance is greater than some charge distance
        else if (AIPath.remainingDistance > minChargeDistance && !isAttacking)
        {
            AIPath.canMove = true;
            //Debug.Log(transform.name + "Mino is walking!");
            animation.SetBool("IsMoving", true);
        }
        else if (!isAttacking)
        {
            //One mino is getting to here
            //Debug.Log(transform.name + "Mino is charging");
            AIPath.canSearch = false;
            AIPath.canMove = false;
            animation.SetBool("IsCharging", true);
            animation.SetBool("IsMoving", false);
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
        //Debug.Log("Rushing should be true here");
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
            Debug.Log("Collider hit is:" + rayHit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("Collider was fucking NULL!");
            chargeTarget = targetOldPosition;

            //isAttacking = false;
            //AIPath.canSearch = true;
            //yield break;
        }

        while (!isSomethingHit)
        {
            //Vector3 smoothedDelta = Vector3.MoveTowards(transform.position, chargeTarget, Time.fixedDeltaTime * g_rushSpeed);
            //rb.MovePosition((Vector2)smoothedDelta)
            //Debug.Log(transform.name + "Therefore: ChargeTar:" + chargeTarget + "And transform.position:" + transform.position);
            Vector2 velocity = (chargeTarget - transform.position);
            //Debug.Log("Velocity:" + velocity);
            velocity *= g_rushSpeed;
            rb.AddForce(velocity, ForceMode2D.Force);
            //animation.SetBool("IsCharging", false);

            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(box.transform.position, 1.4f*box.size,0f);
            //Debug.Log("The list is size: " + hitColliders.Length);
            foreach (Collider2D hit in hitColliders)
            {
                Debug.Log("Hit collider name: " + hit.gameObject.name);
            }
            foreach (Collider2D hit in hitColliders)
            {
                if (hit.CompareTag("Shield"))
                {
                    if (hit.enabled)
                    {
                        Debug.Log(transform.name + "Shield Detected!");
                        hitShield = true;
                        break;
                    }
                    else
                    {
                        Debug.Log(transform.name + "Shield not Detected!");
                        hitShield = false;
                        break;
                    }
                }
            }
            foreach (Collider2D hit in hitColliders)
            {
                minoScript = hit.gameObject.GetComponent<MinotaurAI>();
                if (hit.CompareTag("Player") && !hitShield)
                {
                    Debug.Log(transform.name + "Player hit" + hit.gameObject.name);
                    animation.SetBool("IsRushing", false);
                    //animation.SetBool("IsMoving", false);
                    CinemachineShake.Instance.ShakeCamera(10f, .3f);
                    SFX.GetComponent<SFX>().PlaySwordSwing();
                    isPlayerHit = true;
                    isSomethingHit = true;
                    GameManager.Instance.updateHP(-attackDamage);
                }

                else if (hitShield)
                {
                    Debug.Log(transform.name + "Shield hit" + hit.gameObject.name);
                    animation.SetBool("IsRushing", false);
                    //animation.SetBool("IsMoving", false);
                    CinemachineShake.Instance.ShakeCamera(10f, .3f);
                    isPlayerHit = true;
                    isSomethingHit = true;
                }
                else if (hit.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                {
                    Debug.Log(transform.name + "hit something" + hit.gameObject.name);
                    animation.SetBool("IsRushing", false);
                    //animation.SetBool("IsMoving", false);
                    CinemachineShake.Instance.ShakeCamera(10f, .3f);
                    rb.velocity = new Vector2(0f, 0f);
                    //rb.AddForce(rayDirection.normalized * -100f);
                    Vector2 closestPt = hit.ClosestPoint(transform.position);
                    Vector2 vecAway = ((Vector2)transform.position - closestPt).normalized;
                    rb.AddForce(vecAway * 100f);
                    isSomethingHit = true;
                }

                //You da issue
                else if (hit.tag == "Enemy" && minoScript == null)
                {
                    Debug.Log(transform.name + "Hit enemy tag of name" + hit.gameObject.name);
                    animation.SetBool("IsRushing", false);
                    //animation.SetBool("IsMoving", false);
                    rb.velocity = new Vector2(0f, 0f);
                    isSomethingHit = true;
                    //Debug.DrawRay(transform.position, (Vector2)(hit.transform.position - transform.position), Color.blue, 10f);
                    //hit.attachedRigidbody.transform.position = transform.position;
                }

                else if(hit.tag == "Enemy" && minoScript != null)
                {
                    Rigidbody2D colRB = hit.gameObject.GetComponent<Rigidbody2D>();
                    //Debug.Log(transform.name + "Hit enemy tag of name" + hit.gameObject.name + "Minoscript found.");
                    //Debug.Log(transform.name + " velcity mag. is :" + rb.velocity.magnitude);
                    //Debug.Log("And the collision's velcity is: " + colRB.velocity.magnitude);
                    if((rb.velocity.magnitude - colRB.velocity.magnitude > 0.05f) && colRB.velocity.magnitude > 0.001f)
                    {
                        animation.SetBool("IsRushing", false);
                        rb.velocity = new Vector2(0f, 0f);
                        isSomethingHit = true;
                    }
                    //animation.SetBool("IsRushing", false);
                    //animation.SetBool("IsMoving", false);
                    //rb.velocity = new Vector2(0f, 0f);
                    //isSomethingHit = true;
                }

                //StartCoroutine("stuck");
            }

            hitShield = false;

            yield return null;
        }
        AIPath.canSearch = true;

        if (isPlayerHit)
        {
            yield return new WaitForSeconds(.75f * stunnedTime);
            rb.velocity = new Vector2(0, 0);
            rb.transform.Translate(-1* rayDirection.normalized);
            yield return new WaitForSeconds(.25f * stunnedTime);
            AIPath.canMove = true;
        }
        else
        {
            yield return new WaitForSeconds(stunnedTime);
            AIPath.canMove = true;
        }  
        isAttacking = false;
    }

    //IEnumerator stuck()
    //{
    //    yield return new WaitForSeconds(2f);

    //    isAttacking = false;
    //}
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
        int dropNum = UnityEngine.Random.Range(1, 10);
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
            takeDamage(PlayerController.Instance.attackDamage/2);
        }
    }
}
