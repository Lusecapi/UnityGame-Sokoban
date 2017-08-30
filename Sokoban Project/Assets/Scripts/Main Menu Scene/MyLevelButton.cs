using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.SceneManagement;

public enum LevelType
{
    SokobanLevel,
    MyLevel
}

public class MyLevelButton : MonoBehaviour, IPointerClickHandler {

    public string levelFile;
    public LevelType levelType;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        LevelManager.runningLevel = new LevelManager.Level(levelFile, levelType);
        SceneManager.LoadScene("PlayLevel");
    }


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
