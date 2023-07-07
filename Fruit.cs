using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{

    public int standardHealing = 25;
    public Transform target;



    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Target").transform;

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Familia hoy se come?");


        GameObject collidedObject = collision.gameObject;
        if (collidedObject.CompareTag("Bullet"))
        {
            Debug.Log("Me he curado");



            Heal();

            Destroy(gameObject, 0.2f);

        }
    }

   
    void Heal()
    {
        if (target != null)
        {
            target.GetComponent<Target>().Healing(standardHealing);
        }
    }

}
