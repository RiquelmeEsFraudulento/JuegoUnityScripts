using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Unity.AI.Navigation;

public class Zombie : MonoBehaviour
{
    public Animator animator;
    public float health = 100f;
    public float attackRange = 2f;
    public float attackDamage = 0.00001f;
    public float headshotDamage = 100f;
    public float bulletDamage = 25f;
    public float explosionDamage = 3f;
    public float recognitionRadius = 10f;
    public Transform target;
    public NavMeshAgent agent;
    public bool isWalking;
    public bool isRunning;
    public float userScore;
    public GameObject agentPrefab; // Prefab for the default agent behavior
    public GameObject specialAgentPrefab; // Prefab for the special agent behavior
    public SphereCollider headCollider;
    public float difficulty = 1;
    public bool isAlive = true;


    private bool isSpecialAgent = false; // Flag to track if the agent is using special behavior

    void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Target").transform;

        // Check if UserScore exists in PlayerPrefs
        if (PlayerPrefs.HasKey("UserScore"))
        {
            // Load the UserScore from PlayerPrefs
            userScore = PlayerPrefs.GetInt("UserScore");
        }
        else
        {
            // UserScore not found, set it to 0
            userScore = 0;
        }

    // Instantiate the default agent prefab dynamically
    GameObject agentObject = CreateAgentPrefab(agentPrefab);

        // Set the agentObject as a child of the parent object in your scene
        agentObject.transform.parent = transform;

        // Get the NavMeshAgent component from the agentObject
        agent = agentObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isAlive)
        {
            // Perform any necessary update logic here

            if (agent == null || !agent.isOnNavMesh)
                return;

            // Check if the target is null
            if (target == null)
            {
                // Stop moving and trigger the "Idle" animation
                agent.isStopped = true;
                isWalking = false;
                isRunning = false;
                animator.SetTrigger("Idle");
                return;
            }

            // Check if the target is within recognition radius
            if (IsTargetInRecognitionRadius())
            {
                // Move towards the target
                agent.SetDestination(target.position);
                isWalking = true;
                isRunning = true;

                // Rotate towards the target
                RotateTowardsTarget();

                // Check if the target is within attack range
                if (IsTargetInRange())
                {
                    // Trigger the "Attack" animation
                    animator.SetTrigger("Attack");

                    // Perform the attack
                    Attack();
                }
            }
            else
            {
                // Stop moving and trigger the "Idle" animation
                agent.isStopped = true;
                isWalking = false;
                isRunning = false;
                animator.SetTrigger("Idle");
            }

            // Update the animator parameter for walking or running
            animator.SetBool("IsWalking", isWalking && isAlive);
            animator.SetBool("isRunning", isSpecialAgent && isRunning && isAlive); //

            // Check if the agent behavior needs to be altered dynamically
            if (ShouldSwitchToSpecialAgent())
            {
                // Destroy the existing agent object
                Destroy(agent.gameObject);

                // Instantiate the special agent prefab dynamically
                GameObject specialAgentObject = CreateSpecialAgentPrefab();

                // Set the specialAgentObject as a child of the parent object in your scene
                specialAgentObject.transform.parent = transform;

                // Get the NavMeshAgent component from the specialAgentObject
                agent = specialAgentObject.GetComponent<NavMeshAgent>();

                // Set the isSpecialAgent flag to true
                isSpecialAgent = true;
            }
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 targetDirection = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * agent.angularSpeed);
    }




    bool ShouldSwitchToSpecialAgent()
    {
    // Implement your own conditions to determine when to switch to special agent behavior
    // For example, based on health, score, time, or any other game-related logic

    // health <= 50f was only by health but now is in Userscore, but I can add a fury mode to the zombies.

    return (userScore>500); // Switch to special agent when health is less than or equal to 50
    }

    void Attack()
    {
        target.GetComponent<Target>().TakeDamage(attackDamage);
    }

    bool IsTargetInRecognitionRadius()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= recognitionRadius;
    }

    bool IsTargetInRange()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= attackRange;
    }

    GameObject CreateAgentPrefab(GameObject prefab)
    {
        // Create an instance of the specified prefab
        GameObject agentPrefab = Instantiate(prefab);

        // Customize the agent prefab's properties as needed
        NavMeshAgent navMeshAgent = agentPrefab.GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
            navMeshAgent = agentPrefab.AddComponent<NavMeshAgent>();

        


        navMeshAgent.speed = 3f + (3f* userScore/1000);
        navMeshAgent.acceleration = 8f + (8f * userScore / 1000);
        navMeshAgent.stoppingDistance = 3f;

        //Animator animator = agentPrefab.GetComponent<Animator>();
        //animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AgentAnimatorController");

        // Return the created agent prefab
        return agentPrefab;
    }

    GameObject CreateSpecialAgentPrefab()
    {
        // Create an instance of the special agent prefab
        GameObject specialAgentPrefabInstance = Instantiate(specialAgentPrefab);

        // Customize the special agent prefab's properties as needed
        NavMeshAgent navMeshAgent = specialAgentPrefabInstance.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
            navMeshAgent = specialAgentPrefabInstance.AddComponent<NavMeshAgent>();

        navMeshAgent.speed = 5f + (5f * userScore / 1500);
        navMeshAgent.acceleration = 12f + (5f * userScore / 1500);
        headshotDamage = 100f - (float)(userScore / 1000);
        if (headshotDamage < 50f)
        {
            headshotDamage = 50f;
        }

        health = 100f + (float)(userScore / 1000);
        if (health > 150f)
        {
            health = 150f;
        }


        attackRange = 2f + (float)(userScore / 25000);
        if (attackRange > 6f)
        {
            attackRange = 6f;
        }


        attackDamage = 0.0001f + (float)(userScore / 200000);
        if (attackDamage > 0.0001f)
        {
            attackRange = 0.0001f;
        }

        bulletDamage = 25f - (float)(userScore / 4000);
        if (headshotDamage < 12.5f)
        {
            headshotDamage = 12.5f;
        }

        recognitionRadius = 10f + (float)(userScore / 100);
        if (recognitionRadius < 15f)
        {
            recognitionRadius = 15f;
        }


        explosionDamage = 3f - (float)(userScore / 16666.66);
        if (explosionDamage > 1.5f)
        {
            explosionDamage = 1.5f;
        }

        navMeshAgent.stoppingDistance = 3f; //Previously was attackRange/2

        //Animator animator = specialAgentPrefabInstance.GetComponent<Animator>();
        //animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SpecialAgentAnimatorController");

        // Set the special agent's initial behavior
        if (userScore >= 100 && userScore < 500)
        {
            animator.SetBool("Level2", false);
            animator.SetBool("Level1", true);
        }
        else if (userScore >= 500)
        {
            animator.SetBool("Level1", false);
            animator.SetBool("Level2", true);
            //animator.SetTrigger("isRunning");
        }

        // Return the created special agent prefab
        return specialAgentPrefabInstance;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Get the collision point
            ContactPoint contact = collision.GetContact(0);
            Vector3 collisionPoint = contact.point;

            // Check if the collision is a headshot
            if (IsCollisionInHead(collisionPoint))
            {
                // Apply headshot damage
                
                health -= headshotDamage;

                // Perform headshot effects or trigger special animations if needed

                if (health <= 0)
                {
                    if (target != null)
                    {
                        target.GetComponent<Target>().AddPoints((int)(userScore / 100 + 40));
                    }

                    // Trigger the "Die" animation
                    animator.SetTrigger("Die");


                    // Disable the zombie's components
                    isAlive = false;
                    agent.isStopped = true;
                    headCollider.enabled = false;

                    // Destroy the enemy object after a delay
                    Destroy(gameObject, 1f);
                }
            }
            else
            {

                // Decrease health based on the bullet damage
                health -= bulletDamage;

                if (health <= 0f)
                {
                    if(target != null)
                    {
                        target.GetComponent<Target>().AddPoints((int)(userScore / 100 + 40));
                    }


                    // Trigger the "Die" animation
                    animator.SetTrigger("Die");


                    // Disable the zombie's components
                    isAlive = false;
                    agent.isStopped = true;
                    headCollider.enabled = false;

                    // Destroy the enemy object after a delay
                    Destroy(gameObject, 1f);
                }
            }
        }

        if (collision.gameObject.CompareTag("Explosion"))
        {

            if (target != null)
            {
                target.GetComponent<Target>().AddPoints((int)(userScore / 100 + 40));
            }


            // Decrease health based on the bullet damage
            health -= explosionDamage;

            if (health <= 0f)
            {

                // Trigger the "Die" animation
                animator.SetTrigger("Die");


                // Disable the zombie's components
                isAlive = false;
                agent.isStopped = true;
                headCollider.enabled = false;

                // Destroy the enemy object after a delay
                Destroy(gameObject, 1f);
            }
        }
    }

    bool IsCollisionInHead(Vector3 collisionPoint)
    {
        // Check if the collision point is within the head collider
        return headCollider.bounds.Contains(collisionPoint);
    }


}