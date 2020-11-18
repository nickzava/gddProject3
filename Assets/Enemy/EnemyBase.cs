﻿using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    // Some basic vars that all enemies have
    Vector2 movement;
    bool hunting;
    // Target is the current object we are pathfinding to, goal is the overall target (The player).
    // Target will change when we pathfind, goal should always be the same thing
    [SerializeField]
    GameObject target;
    [SerializeField]
    GameObject goal;
    // Bool to indicate if we are actively seeking a pathfinding node
    bool seekingNode;

    // Stats which can be set in prefabs
    [SerializeField]
    float speed;
    [SerializeField]
    int health;
    [SerializeField]
    float visionRange;
    [SerializeField]
    float maxVisionRange;
    [SerializeField]
    float approachRadius;

    // Start is called before the first frame update
    protected void Start()
    {
        // Hunting set to false by default, will switch on if player gets too close
        hunting = false;

        // If player exists, set it to the target
        try
        {
            goal = GameObject.FindGameObjectWithTag("Player");
        }
        catch (System.Exception)
        {

            Debug.LogWarning("Player not found in scene!");
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        LocateTarget();
        SeekTarget();
        DoMovement();
    }

    // Sets our movement vector so we can move towards the target
    void SeekTarget()
    {
        // If not hunting, stops the method
        if (!hunting)
        {
            movement = new Vector2(0, 0);
            return;
        }
        // Else, set our movement vector to a seeking vector only if we are not within seeking radius
        // If we are looking at a pathfinding node, will always be true (So we can ignore approach radius)
        if (GetDist(gameObject, target) > approachRadius || seekingNode)
        {
            movement = Vector2.ClampMagnitude(target.transform.position - gameObject.transform.position, speed);
        } else
        {
            movement = new Vector2(0, 0);
        }
    }

    // Moves the enemy
    void DoMovement()
    {
        // Cast to vec3
        Vector3 tempMove = movement;
        gameObject.transform.position += tempMove;
    }

    // Helper method to find distance between two gameobjects
    float GetDist(GameObject seeker, GameObject target)
    {
        float dist = Vector2.Distance(seeker.transform.position, target.transform.position);

        return dist;
    }

    // Checks if player is within sight range, Then decides to seek them or a pathfinding node
    void LocateTarget()
    {
        // Checks to see if the target is within vision range, drops target if not
        if (GetDist(gameObject, goal) <= visionRange)
        {
            hunting = true;
        }
        else if (hunting == true && GetDist(gameObject, goal) > maxVisionRange)
        {
            hunting = false;
        }

        // If we are hunting, figure out if player is within target range
        if (hunting)
        {
            RaycastHit hitInfo;
            int rayLength = 10;

            if (Physics.Raycast(gameObject.transform.position, (goal.transform.position - gameObject.transform.position), out hitInfo, rayLength))
            {
                if (hitInfo.collider.gameObject.tag == "Player")
                {
                    // Player is within range, and is not obstructed
                    target = goal;
                    seekingNode = false;
                }
                else
                {
                    // Player is within range, but IS obstructed, set target to closest node
                    target = GetPathfindNode();
                    seekingNode = true;
                }
            }
        }


    }

    // Gets the closest pathfinding node
    GameObject GetPathfindNode()
    {
        return GameObject.Find("pathfindingNode");
    }
}
