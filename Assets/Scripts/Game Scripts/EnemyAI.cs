using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    [Header("Character Attributes:")]
    [SerializeField] private Transform target;
    [SerializeField] private float walkingSpeed = 1.0f;
    //The distance the enemy has to be from the next waypoint to shift to the next waypoint
    [SerializeField] private float nextWaypointDistance = 3.0f;
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
    public GameObject reward;
    private GameObject SFX;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        blood = gameObject.GetComponentInChildren(typeof(ParticleSystem), true) as ParticleSystem;
        localScale = healthBar.transform.localScale;
        SFX = GameObject.Find("SFX");

        //Generates a path from the enemy to the character using modified Dijkstra's Algo.
        //Once a path is generated, a function callback occurs passing in the new path obj.
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            //Once a new path is generated, reset the index of the first waypoint
            currentWaypoint = 0;
        }
    }

    void Update()
    {
        localScale.x = health/100;
        healthBar.transform.localScale = localScale;
    }

    public void takeDamage(int damage)
    {

        blood.Play();
        SFX.GetComponent<SFX>().PlayDamageSound();
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Enemy died!");

            GameObject gold = Instantiate(reward, transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }
    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name.ToString());
        //Debug.Log(health.ToString());
       
        if (collision.gameObject.name == "Bullet(Clone)")
        {

            takeDamage(10);

            /*
            if(PlayerController.isAttacking || collision.gameObject.name == "Bullet(Clone)")
            {
                
                blood.Play();
                health -= 10;
                if (health <= 0)
                {
                    Destroy(this.gameObject);
                }

                if(PlayerController.isAttacking)
                {
                    Vector2 playerPos = (Vector2)PlayerController.movement;
                    Vector2 direction = (Vector2)PlayerController.aimDirection;
                    Vector2 pushForce = new Vector2(50 * direction.x, 50 * direction.y);
                    Vector2 enemyPos = transform.position;

                    if (playerPos.x > enemyPos.x)
                    {
                        pushForce.x = -pushForce.x;
                    }

                    rb.AddForce(pushForce);
                }
            */
        }
    }
}
