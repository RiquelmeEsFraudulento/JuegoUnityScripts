using UnityEngine;

public class Monster : MonoBehaviour
{
    private Animator animator;
    public int health = 100;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        if (collidedObject.CompareTag("Bullet"))
        {
            Debug.Log("The enemy has been hit by a bullet");


            int bulletDamage = 25; 
            //collision.gameObject.GetComponent<Bullet>().damage;
            health -= bulletDamage;

            // Trigger the "Die" animation
            animator.SetTrigger("Die");

            // Remove the bullet object
            Destroy(collidedObject);

            Destroy(gameObject, 1f);

        }
    }
}
