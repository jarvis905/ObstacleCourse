using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // References
    [Header("References")]
    public Transform trans;
    public Transform modelTrans;
    public CharacterController characterController;
    public GameObject cam;

    // Movement
    [Header("Movement")]
    [Tooltip("Units moved per second at maximum speed.")]
    public float maxMovSpeed = 24;

    [Tooltip("Time in seconds, to reach maximum speed.")]
    public float timeToMaxSpeed = 0.26f;
    private float VelocityGainPerSecond
    {
        get
        {
            return maxMovSpeed / timeToMaxSpeed;
        }
    }

    [Tooltip("Time, in seconds, to go from maximum speed to stationary.")]
    public float timeToLoseMaxSpeed = 0.2f;

    private float VelocityLossPerSecond
    {
        get
        {
            return maxMovSpeed / timeToLoseMaxSpeed;
        }
    }

    [Tooltip("Multiplier for momentum when attempting to move in a direction opposite the current traveling direction.")]
    public float reverseMomentumMultiplier = 2.2f;

    private Vector3 movementVelocity = Vector3.zero;

    // Death and Respawning Variables
    [Header("Death and Respawning")]
    [Tooltip("How long after the player's death, in seconds, before they are respawned?")]
    public float respawnWaitTime = 2f;
    private bool dead = false;
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;

    // Dashing
    [Header("Dashing")]
    [Tooltip("Total distance traveled during a dash.")]
    public float dashDistance = 17;
    [Tooltip("Time taken for a dash (in seconds).")]
    public float dashTime = 0.26f;

    private bool IsDashing
    {
        get
        {
            return (Time.time < dashBeginTime + dashTime);
        }
    }

    private Vector3 dashDirection;
    private float dashBeginTime = Mathf.NegativeInfinity;

    [Tooltip("Time after dashing finishes before it can be performed again.")]
    public float dashCooldown = 1.8f;

    private bool CanDashNow
    {
        get
        {
            return (Time.time > dashBeginTime + dashTime + dashCooldown);
        }
    }

    // Pausing gameplay
    private bool paused = false;

    private void Pausing()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle pause status
            paused = !paused;

            // If we're now paused, then hold time: set timeScale to 0
            if (paused) 
            {
                Time.timeScale = 0;
            }
            // Otherwise if we're no longer paused then we reset the timeScale to 1
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        spawnRotation = modelTrans.rotation;
    }

    // Method to handle movement logic
    private void Movement()
    {
        if(!IsDashing)
        {
            // Forward Movement (Z Axis)
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (movementVelocity.z >= 0)
                    movementVelocity.z = Mathf.Min(maxMovSpeed, movementVelocity.z + VelocityGainPerSecond * Time.deltaTime);
                else
                    movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }

            // Backward Movement (Z Axis)
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (movementVelocity.z > 0)
                    movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
                else
                    movementVelocity.z = Mathf.Max(-maxMovSpeed, movementVelocity.z - VelocityGainPerSecond * Time.deltaTime);
            }

            // If neither forward nor back are being held
            else
            {
                if (movementVelocity.z > 0)
                    movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityLossPerSecond * Time.deltaTime);
                else
                    movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityLossPerSecond * Time.deltaTime);
            }

            // Right Movement (X Axis)
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (movementVelocity.x >= 0)
                    movementVelocity.x = Mathf.Min(maxMovSpeed, movementVelocity.x + VelocityGainPerSecond * Time.deltaTime);
                else
                    movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }

            // Left Movement (X Axis)
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (movementVelocity.x > 0)
                    movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
                else
                    movementVelocity.x = Mathf.Max(-maxMovSpeed, movementVelocity.x - VelocityGainPerSecond * Time.deltaTime);
            }

            // If neither right nor left are being held
            else
            {
                if (movementVelocity.x > 0)
                    movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityLossPerSecond * Time.deltaTime);
                else
                    movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityLossPerSecond * Time.deltaTime);
            }

            // if the player moves in either direction left/right or up/down
            if (movementVelocity.x != 0 || movementVelocity.z != 0)
            {
                // Applying the movement velocity
                characterController.Move(movementVelocity * Time.deltaTime);

                // Keeping the model holder related to the last movement direction based on movement velocity
                modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, Quaternion.LookRotation(movementVelocity), 0.18F);
            }
        }

    }

    private void Dashing()
    {
        if (!IsDashing) // If we aren't dashing right now
        {
            if (CanDashNow &&  Input.GetKey(KeyCode.Space)) // If dash is not on cooldown and the space key is pressed
            {
                Vector3 movementDir = Vector3.zero;

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    movementDir.z = 1;
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    movementDir.z = -1;

                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    movementDir.x = 1;
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    movementDir.x = -1;

                if (movementDir.x != 0 || movementDir.z != 0)
                {
                    dashDirection = movementDir;
                    dashBeginTime = Time.time;
                    movementVelocity = dashDirection * maxMovSpeed;
                    modelTrans.forward = dashDirection;
                }
            }
        }
        else // If dashing
        {
            characterController.Move(dashDirection * (dashDistance / dashTime) * Time.deltaTime);
        }
    }

    // Die method checks if the playerObject is dead or not
    // if the playerObject is dead, then it will invoke the Respawn method after a specific waitTime
    // and will disable and deactivate all playerObject related settings - the characterController, the playerScript, velocity is reset to 0, etc.
    public void Die()
    {
        if (!dead)
        {
            dead = true;
            Invoke("Respawn", respawnWaitTime);
            movementVelocity = Vector3.zero;
            enabled = false;
            characterController.enabled = false;
            modelTrans.gameObject.SetActive(false);
            dashBeginTime = Mathf.NegativeInfinity;
        }
    }

    // Respawn method will be invoked by the Die method after the waitTime
    // this method will re-enable all playerObject related settings - 
    // - will reset the dead variable to false since the player is live again
    // - will reset the playerObject's position to the spawnPoint which was set earlier
    // - will enable all characterController and activate gameObject settings
    public void Respawn()
    {
        dead = false;
        trans.position = spawnPoint;
        modelTrans.rotation = spawnRotation;
        enabled = true;
        characterController.enabled = true;
        modelTrans.gameObject.SetActive(true);
    }

    // Call the Movement method in the Update method
    // Update is called once per frame
    private void Update()
    {
        if (!paused)
        {
            Movement();
            Dashing();
        }
        Pausing();

        // invoke die method for testing by manually killing the playerObject
        if (Input.GetKey(KeyCode.T))
        {
            Die();
        }
    }

    // Creating the in-game paused menu
    void OnGUI()
    {
        if (paused)
        {
            float boxWidth = Screen.width * .4f;
            float boxHeight = Screen.height * .4f;
            GUILayout.BeginArea(new Rect(
                (Screen.width * .5f) - (boxWidth * .5f),
                (Screen.height * .5f) - (boxHeight * .5f),
                boxWidth,
                boxHeight));

            if (GUILayout.Button("RESUME GAME", GUILayout.Height(boxHeight * .5f)))
            {
                paused = false;
                Time.timeScale = 1;
            }

            if (GUILayout.Button("RETURN TO MAIN MENU", GUILayout.Height(boxHeight * .5f)))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }

            GUILayout.EndArea();
        }
    }


}
