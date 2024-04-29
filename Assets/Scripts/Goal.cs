using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public AudioSource winSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            winSound.Play();
            // SceneManager.LoadScene("main");
            Invoke("mainMenu", 3.0f);
        }
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("main");
    }
}
