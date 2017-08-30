using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

    [SerializeField]
    private int minShowTime = 4;//The miniun time to show the splah image(in seconds)
    private bool isLoading = true;

	// Use this for initialization
	void Start () {

        isLoading = new SettingsManager().setConfigurationsAndSettings();
    }
	
	// Update is called once per frame
	void Update () {

        if((int)Time.timeSinceLevelLoad >= minShowTime && !isLoading)
        {
            SceneManager.LoadScene(1);
        }
    }
}
