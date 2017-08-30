using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayLevelUIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnExitButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnRetryButtonClick()
    {
        SceneManager.LoadScene("PlayLevel");
    }
}
