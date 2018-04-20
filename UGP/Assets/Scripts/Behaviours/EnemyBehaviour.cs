using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public int id;
    public bool Follow = false;
    public float DeathTimer = 1.0f;
    private NavMeshAgent Agent;
    private Transform target;

    private GameObject originalModel;
    public GameObject ragdollModel;

    [HideInInspector]
    public Animator EnemyAnimator;

    public float Radius = 10.0f;
    public int HitPoints = 20;

    [HideInInspector]
    public bool destroy = false;
    
    //MAKE ZOMBIE RAGDOLL
    private void Death()
    {
        //PLAY DEATH SOUND

        //RAGDOLL UP
        //SWITCH TO THE RAGDOLL MODEL
        Destroy(originalModel);

        var ragdoll = (GameObject)Instantiate(ragdollModel, transform.position, transform.rotation);
        
        //ragdoll.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
        
        Destroy(ragdoll, DeathTimer);
    }

    private void UpdateName()
    {
        originalModel = GameObject.Find("originalModel" + id);
    }

    void Start()
    {
        //originalModel = GameObject.FindGameObjectWithTag("originalModel");
        //originalModel = GetComponentInChildren<GameObject>();
        EnemyAnimator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();	
	}
	
    private void FixedUpdate()
    {
        UpdateName();

        if(HitPoints <= 0)
        {
            destroy = true;
        }

        if(destroy == true)
        {
            //Destroy(gameObject, DeathTimer);
            //RAGDOLL FOR 'DeathTimer' seconds
            EnemyAnimator.SetBool("Dead", true);
            Death();
            Destroy(gameObject);
        }

        if(Agent != null && destroy == false)
        {
            Agent.isStopped = true;

            if (Follow == true)
            {
                EnemyAnimator.SetBool("Follow", true);
                Agent.isStopped = false;
                Agent.SetDestination(target.position);
            }

            if (Follow == false)
            {
                Agent.isStopped = true;
                EnemyAnimator.SetBool("Follow", false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hand")
        {
            if(Follow != true)
            {
                destroy = true;
                //LET THE 'Death()' METHOD HANDLE DELETION
                //Destroy(gameObject, 2.0f);
            }

        }
    }
    public void LateUpdate()
    {
        
    }
}
