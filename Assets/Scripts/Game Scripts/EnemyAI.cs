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
    private ParticleSystem blood;
    public float health = 100f;
    public SpriteRenderer healthBar;
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
    private string currentAnimaton;
    public static bool isAttacking;


    void Start()
    {
        target = GameObject.Find("Promethesus").transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        //animation = GetComponent<Animator>();
        animation = GetComponentInChildren<Animator>();
        blood = gameObject.GetComponentInChildren(typeof(ParticleSystem), true) as ParticleSystem;
        localScale = healthBar.transform.localScale;
        SFX = GameObject.Find("SFX");

        //gameObject.GetComponent<Renderer>().enabled = false;
        //gameObject.GetComponent<SpriteRenderer>().enabled = true;
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            //Debug.Log("Path seems okay!");
            path = p;
            //Once a new path is generated, reset the index of the first waypoint
            currentWaypoint = 0;
        }
        //else { Debug.Log("Path error!"); }
    }

    void UpdatePath()
    {
        //Debug.Log("UpdatingPath");

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
        if (path == null)
        { //Debug.Log("No Path!"); 
            return; 
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else { reachedEndOfPath = false; }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 directionEnd = ((Vector2)path.vectorPath[path.vectorPath.Count - 1] - rb.position).normalized;
        Vector2 force = direction * walkingSpeed;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            ++currentWaypoint;
        }

        float angle = Mathf.Atan2(directionEnd.y, directionEnd.x) * Mathf.Rad2Deg;
        if (angle < 0.0f) { angle += 360f; }

        Debug.Log("Angle: " + angle);
        //ChangeAnimationState("RunForward");

        //if (force.magnitude < 0.001f && !isAttacking)
        //{
        //    Debug.Log("Idling");
        //    if (angle > 45f && angle <= 135f)
        //    {
        //        ChangeAnimationState("IdleFront");
        //    }
        //    else if (angle > 135f && angle <= 225f)
        //    {
        //        ChangeAnimationState("IdleLeft");
        //    }
        //    else if (angle > 225f && angle <= 315f)
        //    {
        //        ChangeAnimationState("IdleBack");
        //    }
        //    else if (angle > 315f || angle <= 45)
        //    {
        //        ChangeAnimationState("IdleRight");
        //    }
        //}
        //else if (!isAttacking)
        //{
        //    Debug.Log("Running");
        //    if (angle > 45f && angle <= 135f)
        //    {
        //        ChangeAnimationState("RunForward");
        //    }
        //    else if (angle > 135f && angle <= 225f)
        //    {
        //        ChangeAnimationState("RunLeft");
        //    }
        //    else if (angle > 225f && angle <= 315f)
        //    {
        //        ChangeAnimationState("RunBackward");
        //    }
        //    else if (angle > 315f || angle <= 45)
        //    {
        //        ChangeAnimationState("RunRight");
        //    }
        //}
        if (angle > 45f && angle <= 135f)
        {
            Debug.Log("Running Up");
            ChangeAnimationState("RunForward");
        }
        else if (angle > 135f && angle <= 225f)
        {
            Debug.Log("Running Left");
            ChangeAnimationState("RunLeft");
        }
        else if (angle > 225f && angle <= 315f)
        {
            Debug.Log("Running Down");
            ChangeAnimationState("RunBackward");
        }
        else if (angle > 315f || angle <= 45)
        {
            Debug.Log("Running Right");
            ChangeAnimationState("RunRight");
        }


        //if (isAttacking)
        //{
        //    //isAttacking = true;

        //    if (angle > 45f && angle <= 135f)
        //    {
        //        ChangeAnimationState("AttackFront");
        //    }
        //    else if (angle > 135f && angle <= 225f)
        //    {
        //        ChangeAnimationState("AttackLeft");
        //    }
        //    else if (angle > 225f && angle <= 315f)
        //    {
        //        ChangeAnimationState("AttackBack");
        //    }
        //    else if (angle > 315f || angle <= 45)
        //    {
        //        ChangeAnimationState("AttackRight");
        //    }
        //    //This let's the attack animation play out completely
        //    Invoke("AttackComplete", attackDelay);
        //}


        //if (isAttackPressed)
        //{
        //    isAttackPressed = false;

        //    if (!isAttacking)
        //    {
        //        isAttacking = true;

        //        if (angle > 45f && angle <= 135f)
        //        {
        //            ChangeAnimationState("AttackFront");
        //        }
        //        else if (angle > 135f && angle <= 225f)
        //        {
        //            ChangeAnimationState("AttackLeft");
        //        }
        //        else if (angle > 225f && angle <= 315f)
        //        {
        //            ChangeAnimationState("AttackBack");
        //        }
        //        else if (angle > 315f || angle <= 45)
        //        {
        //            ChangeAnimationState("AttackRight");
        //        }



        //        //This let's the attack animation play out completely
        //        Invoke("AttackComplete", attackDelay);


        //    }

        //}

    }

    void AttackComplete()
    {
        isAttacking = false;
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

    void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimaton == newAnimation) return;

        animation.Play(newAnimation);
        currentAnimaton = newAnimation;
    }
}
