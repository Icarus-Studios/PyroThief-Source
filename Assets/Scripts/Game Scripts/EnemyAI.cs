using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private Transform target;
    //Transform target;
    [SerializeField] private float walkingSpeed = 1.0f;
    //The distance the enemy has to be from the next waypoint to shift to the next waypoint
    [SerializeField] private float nextWaypointDistance = 3.0f;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float health = 100f;

    private ParticleSystem blood;
    public SpriteRenderer healthBar;
    public Transform attackPoint;
    public float attackRange = 1f;

    Path path;
    //Index of currently targeted waypoint
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;
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
    private string currentAnimation;
    public static bool isAttacking;


    void Start()
    {
        target = GameObject.Find("Promethesus").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animation = GetComponentInChildren<Animator>();
        blood = gameObject.GetComponentInChildren(typeof(ParticleSystem), true) as ParticleSystem;
        localScale = healthBar.transform.localScale;
        SFX = GameObject.Find("SFX");

        InvokeRepeating("UpdatePath", 0f, .1f);
    }



    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            //Once a new path is generated, reset the index of the first waypoint
            currentWaypoint = 0;
        }
    }

    void UpdatePath()
    {
        //Generates a path from the enemy to the character using modified Dijkstra's Algo.
        //Once a path is generated, a function callback occurs passing in the new path obj.
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void Update()
    {
        localScale.x = health/100;
        healthBar.transform.localScale = localScale;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }
    private void MoveCharacter()
    {
        if (path == null){ return; }
        if (currentWaypoint >= path.vectorPath.Count - 1)
        {
            //Debug.Log("Current waypoint: " + currentWaypoint + "  VectorPath.Count: " + path.vectorPath.Count);
            reachedEndOfPath = true;
            return;
        }
        else { Debug.Log("Set false"); reachedEndOfPath = false; }


        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 directionEnd = ((Vector2)path.vectorPath[path.vectorPath.Count - 1] - rb.position).normalized;
        //Vector2 force = direction * walkingSpeed;


        Debug.Log("Direction mag: " + direction.magnitude);
        if (direction.magnitude > 0.001f)
        {
            animation.SetBool("IsMoving", true);
        }
        else
        {
            animation.SetBool("IsMoving", false);
        }

        //rb.AddForce(force);
        rb.MovePosition((Vector2)transform.position + (direction * walkingSpeed * Time.deltaTime));

        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        Debug.Log("Distance to target:" + distanceToTarget);
        if(distanceToTarget < 4f)
        {
            if(!isAttacking)
            {
                Debug.Log("Attacking");
                animation.SetTrigger("IsAttacking");
                isAttacking = true;
                Attack();
            }
        }

        

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distanceToWaypoint < nextWaypointDistance)
        {
            ++currentWaypoint;
        }

        float angle = Mathf.Atan2(directionEnd.y, directionEnd.x) * Mathf.Rad2Deg;
        if (angle < 0.0f) { angle += 360f; }

        //Facing up
        if (angle > 45f && angle <= 135f)
        {
            animation.SetInteger("angle", 1);
            attackPoint.transform.localPosition = new Vector3(0, .8f, 0);
        }
        //Facing left
        else if (angle > 135f && angle <= 225f)
        {
            animation.SetInteger("angle", 2);
            attackPoint.transform.localPosition = new Vector3(-1.1f, 0, 0);
        }
        //Facing down
        else if (angle > 225f && angle <= 315f)
        {
            animation.SetInteger("angle", 3);
            attackPoint.transform.localPosition = new Vector3(0, -1.0f, 0);
        }
        //Facing right
        else if (angle > 315f || angle <= 45)
        {
            animation.SetInteger("angle", 4);
            attackPoint.transform.localPosition = new Vector3(1.1f, 0, 0);
        }
    }

    void AttackComplete()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);
        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                GameManager.Instance.updateHP(-10);
            }
        }
        SFX.GetComponent<SFX>().PlaySwordSwing();
        isAttacking = false;
    }

    public void Attack()
    {
        if(isAttacking)
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
        blood.Play();
        SFX.GetComponent<SFX>().PlayDamageSound();
        health -= damage;
        if (health <= 0)
        {
           // Debug.Log("Enemy died!");
            dropItem();

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
