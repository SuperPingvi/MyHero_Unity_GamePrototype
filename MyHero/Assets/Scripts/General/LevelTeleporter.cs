using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelTeleporter : MonoBehaviour
{
    GameObject player;
    GameObject hero;
    bool teleportEnabled = false;
    bool heroIn = false;
    bool playerIn = false;
    // Start is called before the first frame update
    void Start()
    {
        player = PartyManager.instance.player;
        hero = PartyManager.instance.hero;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Hero")
        {
            heroIn = true;
        }
        if(other.gameObject.tag == "Player")
        {
            playerIn = true;
        }
        if(heroIn && other.gameObject.tag == "Player")
        {
            TeleportLevel();
        }
        if (playerIn && other.gameObject.tag == "Hero")
        {
            TeleportLevel();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hero")
        {
            heroIn = false;
        }
        if (other.gameObject.tag == "Player")
        {
            playerIn = false;
        }
    }
    public void TeleportLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
