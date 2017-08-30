using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class EditLevelButton : MonoBehaviour, IPointerClickHandler {

    public string levelFile;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        LevelEditorUIController.editorAssistant.editionInfo.levelName = levelFile.Split('.')[0];//myFyile.txt we set the myfile as the name
        LevelEditorUIController.editorAssistant.editionInfo.reeditingLevel = true;
        GameObject.FindGameObjectWithTag("UI Controller").GetComponent<LevelEditorUIController>().setEditorCanvas();
        LevelEditorUIController.editorAssistant.readLevelFile(LevelEditorUIController.editorAssistant.editionInfo);
    }
}
