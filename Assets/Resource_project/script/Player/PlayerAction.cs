using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAction : MonoBehaviour
{
    // Start is called before the first frame update
    AsyncOperation async;
    void Start()
    {
        //async = SceneManager.LoadSceneAsync("no1");

        //async.allowSceneActivation = false;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "LevelExit")
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex  + 1);
            async.allowSceneActivation = true;

        }

    }
}
