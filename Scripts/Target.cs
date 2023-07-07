using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class Target : MonoBehaviour
{
    public float health = 100f;
    public int userScore = 0;
    public int headshotPoints = 50;
    public int accuracyPoints = 10;
    public float objectiveTimeLimit = 40f;
    public int damageTakenPenalty = 5;
    public float difficultyMultiplier = 1f;

    private int enemiesKilled = 0;
    private int totalShotsFired = 0;
    private int hitsOnTarget = 0;
    private float objectiveStartTime;
    public bool isObjectiveComplete = false;
    private int failinarow = 0;

    void Start()
    {
        // Set the start time of the objective
        objectiveStartTime = Time.time;

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
    }

    void Update()
    {
        // Check if the objective has been completed
        if (!isObjectiveComplete && Time.time - objectiveStartTime > objectiveTimeLimit)
        {
            // Objective time limit exceeded, penalize the user
            SubtractPoints(accuracyPoints);
            isObjectiveComplete = true;
        }

    }

    public void FiredBullet(bool hitTarget, bool isHeadshot)
    {
        totalShotsFired++;

        if (hitTarget)
        {
            hitsOnTarget++;

            if (isHeadshot)
            {
                AddPoints(headshotPoints);
            }

            // Calculate shooting accuracy
            float accuracy = (float)hitsOnTarget / totalShotsFired;
            AddPoints(Mathf.RoundToInt(accuracy * accuracyPoints));
            failinarow = 0;
        }
        else
        {
            failinarow++;

            if (failinarow > 3)
            {
                SubtractPoints(10 * failinarow);
            }
        }


    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Get the enemy's difficulty rating
            Zombie enemy = collision.gameObject.GetComponent<Zombie>();
            int enemyDifficulty = Mathf.RoundToInt(enemy.difficulty);

            // Calculate the points based on enemy difficulty
            int points = Mathf.RoundToInt(enemyDifficulty * difficultyMultiplier);

            // Add or subtract points based on the enemy's difficulty
            if (enemyDifficulty > 0)
            {
                AddPoints(points);
            }
            else
            {
                SubtractPoints(-points);
            }

            // Increment the number of enemies killed
            enemiesKilled++;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Penalize the user for colliding with an obstacle
            SubtractPoints(damageTakenPenalty);
        }

        if (collision.gameObject.CompareTag("Objetive"))
        {
            Gem objective = collision.gameObject.GetComponent<Gem>();
            if (objective != null && objective.isOver)
            {
                // The round is over, perform any necessary actions
                Debug.Log("Round is over");

                // You can access the user's score using the 'userScore' variable

                // Restart the round or do other logic as needed
            }
        }

        if(collision.gameObject.CompareTag("Lava") || collision.gameObject.CompareTag("Hot"))
            {

                Debug.Log("AAAAAAAAAAAAAAAAAA");
                TakeDamage(0.2f);

                // You can access the user's score using the 'userScore' variable

                // Restart the round or do other logic as needed
        }
    }


    public void AddPoints(int points)
    {
        // Add points to the user's score
        userScore += points;
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.AddXP(points);
        }
    }

    public void SubtractPoints(int points)
    {
        // Subtract points from the user's score
        userScore -= points;

        // Ensure the user's score does not go below zero
        userScore = Mathf.Max(userScore, 0);
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SubtractXP(points);
        }
    }

    public void SetObjectiveComplete()
    {
        isObjectiveComplete = true;
        Debug.Log("Objective is complete!");
        // Add any additional logic or actions you want to perform when the objective is complete
        AddPoints(150);
        // Switch scenes based on the current scene
        SceneChanger();
    }

    public void TakeDamage(float damage)
    {
        damage = 0.1f;
        health = health - damage;
        Debug.Log(health);
        if (userScore > 0)
        {
            SubtractPoints(Mathf.RoundToInt(damage * 3 / userScore));
        }


        if (health <= 0f)
        {
            if (userScore > 0)
            {
                SubtractPoints(Mathf.RoundToInt(userScore/4));
            }
            Die();
        }

        // Send the health information to PlayerHealth script
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }



    public void Healing(float heal)
    {
        if (health + heal <= 100f)
        {
            health += heal;
        }

        // Send the health information to PlayerHealth script
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.RestoreHealth(heal);
        }
    }



    private void Die()
    {

        Debug.Log("Matado Bien Muerto");
        SceneChanger();
        // Disable any necessary components or behaviors on the target object
        // to prevent further interactions or collisions
        /*
        // Create the prompt panel dynamically
        promptPanel = new GameObject("PromptPanel");
        RectTransform panelRectTransform = promptPanel.AddComponent<RectTransform>();
        CanvasRenderer panelCanvasRenderer = promptPanel.AddComponent<CanvasRenderer>();
        Image panelImage = promptPanel.AddComponent<Image>();
        panelImage.color = Color.black;
        panelRectTransform.anchorMin = Vector2.zero;
        panelRectTransform.anchorMax = Vector2.one;
        panelRectTransform.offsetMin = Vector2.zero;
        panelRectTransform.offsetMax = Vector2.zero;

        // Create the prompt text dynamically
        promptText = new GameObject("PromptText");
        RectTransform textRectTransform = promptText.AddComponent<RectTransform>();
        CanvasRenderer textCanvasRenderer = promptText.AddComponent<CanvasRenderer>();
        Text textComponent = promptText.AddComponent<Text>();
        textComponent.text = "You died!";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 24;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textRectTransform.SetParent(panelRectTransform);
        textRectTransform.localPosition = Vector3.zero;
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = new Vector2(20, 20);
        textRectTransform.offsetMax = new Vector2(-20, -20);

        // Create the restart button dynamically
        restartButton = new GameObject("RestartButton");
        RectTransform buttonRectTransform = restartButton.AddComponent<RectTransform>();
        CanvasRenderer buttonCanvasRenderer = restartButton.AddComponent<CanvasRenderer>();
        Image buttonImage = restartButton.AddComponent<Image>();
        buttonImage.color = Color.green;
        Button buttonComponent = restartButton.AddComponent<Button>();
        buttonComponent.onClick.AddListener(RestartGame);
        buttonRectTransform.SetParent(panelRectTransform);
        buttonRectTransform.localPosition = Vector3.zero;
        buttonRectTransform.anchorMin = Vector2.zero;
        buttonRectTransform.anchorMax = Vector2.one;
        buttonRectTransform.offsetMin = new Vector2(100, 100);
        buttonRectTransform.offsetMax = new Vector2(-100, -100);

        // Set the button text
        GameObject buttonText = new GameObject("ButtonText");
        RectTransform buttonTextRectTransform = buttonText.AddComponent<RectTransform>();
        CanvasRenderer buttonTextCanvasRenderer = buttonText.AddComponent<CanvasRenderer>();
        Text buttonTextComponent = buttonText.AddComponent<Text>();
        buttonTextComponent.text = "Restart";
        buttonTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonTextComponent.fontSize = 18;
        buttonTextComponent.alignment = TextAnchor.MiddleCenter;
        buttonTextRectTransform.SetParent(buttonRectTransform);
        buttonTextRectTransform.localPosition = Vector3.zero;
        buttonTextRectTransform.anchorMin = Vector2.zero;
        buttonTextRectTransform.anchorMax = Vector2.one;

        // Set the prompt panel as a child of the target object
        promptPanel.transform.SetParent(transform, false);
        */
    }

    private void RestartGame()
    {
        // Implement your restart logic here
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

    }

    public void SceneChanger()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene1
            SceneManager.LoadScene("SampleScene1");
            //}
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene1")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene
            SceneManager.LoadScene("SampleScene2");
            //}
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene2")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene
            SceneManager.LoadScene("SampleScene3");
            //}
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene3")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene
            SceneManager.LoadScene("SampleScene4");
            //}
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene4")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene
            SceneManager.LoadScene("SampleScene5");
            //}
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene5")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene
            SceneManager.LoadScene("SampleScene6");
            //}
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene6")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene
            SceneManager.LoadScene("SampleScene7");
            //}
        }
        else if (SceneManager.GetActiveScene().name == "SampleScene7")
        {
            // Check if the input key is pressed
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            // Save the UserScore to PlayerPrefs
            PlayerPrefs.SetInt("UserScore", userScore);

            // Load SampleScene
            SceneManager.LoadScene("SampleScene8");
            //}
        }
    }
}
