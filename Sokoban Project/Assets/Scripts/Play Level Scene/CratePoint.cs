using UnityEngine;
using System.Collections;
using System;

public class CratePoint : MonoBehaviour {

    private string cratesTag = "Crate";
    private SpriteRenderer myRenderer;
    private Sprite mySprite;
    private AudioSource myAudio;

    private LevelManager levelManager;

	// Use this for initialization
	void Start () {

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() != UnityEngine.SceneManagement.SceneManager.GetSceneByName("LevelEditor"))
        {
            levelManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<LevelManager>();
            myRenderer = GetComponent<SpriteRenderer>();
            mySprite = myRenderer.sprite;
            myAudio = GetComponent<AudioSource>();
        }
	}
	
	// Update is called once per frame
	void Update () {

	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == cratesTag)
        {
            myRenderer.sprite = null;
            myAudio.PlayOneShot(myAudio.clip);
            other.GetComponent<Crate>().SwitchStateTo(true);
            levelManager.updateCratesPoints(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == cratesTag)
        {
            myRenderer.sprite = mySprite;
            myAudio.PlayOneShot(myAudio.clip);
            other.GetComponent<Crate>().SwitchStateTo(false);
            levelManager.updateCratesPoints(false);
        }
    }
}
