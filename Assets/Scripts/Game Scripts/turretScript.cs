using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretScript : MonoBehaviour
{

    public float range;
    private Transform target;
    bool detected = false;
    Vector2 direction;
    public GameObject turret;
    public Sprite active;
    public Sprite inactive;
    public GameObject projectile;
    public Transform ShootPoint;
    public float projectileForce;
    public float fireRate;
    float nextTimeToFire = 0;

    public GameObject gun;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }


    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }

    }


    // Update is called once per frame
    void Update()
    {
        try
        {
            Vector2 targetPos = target.position;
            direction = targetPos - (Vector2)transform.position;
            RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, direction, range);

            if (rayInfo)
            {
                if (rayInfo.collider.gameObject.tag == "Enemy")
                {
                    if (detected == false)
                    {
                        detected = true;
                        //turret.GetComponent<SpriteRenderer>().sprite = active;
                    }
                }
                else
                {
                    if (detected == true)
                    {
                        detected = false;
                        //turret.GetComponent<SpriteRenderer>().sprite = inactive;

                    }
                }
            }
        }
        catch
        {
            detected = false;
            //turret.GetComponent<SpriteRenderer>().sprite = inactive;
        }
        

       

        if(detected)
        {
            gun.transform.up = direction;
            if (Time.time > nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1 / fireRate;
                shoot();
            }
        }
       
    }

    private void shoot()
    {
        GameObject bullets = Instantiate(projectile, ShootPoint.position, Quaternion.identity);
        bullets.GetComponent<Rigidbody2D>().AddForce(direction * projectileForce);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
