using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;
using System;

public class DeleteLevelButton : MonoBehaviour, IPointerClickHandler {

    private string levelFile;

    // Use this for initialization
    void Start () {

        levelFile = GetComponentInParent<MyLevelButton>().levelFile;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        GameObject.FindGameObjectWithTag("UI Controller").GetComponent<MainMenuUIController>().deleteLevelButton(levelFile);
    }
}
