using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public float distanceThreshold = 10f;
    public Renderer gemRenderer;
    public Transform target;

    private Color originalColor;
    private bool isRed = false;
    public bool isOver = false;

    private void Start()
    {
        gemRenderer = GetComponentInChildren<Renderer>();
        originalColor = gemRenderer.material.color;
        UpdateTargetReference();
    }

    private void Update()
    {
        UpdateTargetReference();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= distanceThreshold && !isRed)
            {
                gemRenderer.material.color = Color.red;
                isRed = true;
            }
            else if (distance > distanceThreshold && isRed)
            {
                gemRenderer.material.color = originalColor;
                isRed = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && isRed)
        {
            TheEnd();
        }
    }

    private void UpdateTargetReference()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag("Target");
        if (targetObject != null && targetObject.transform != target)
        {
            target = targetObject.transform;
        }
    }

    private void TheEnd()
    {
        Debug.Log("Round is over");
        // Perform any other actions you want to happen when the round is over
        isOver = true;
        target.GetComponent<Target>().SetObjectiveComplete();

    }
}
