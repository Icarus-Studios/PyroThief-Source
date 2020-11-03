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

    Path path;
    //Index of currently targeted waypoint
    int currentWaypoint = 0;

    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

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
        
    }
}
