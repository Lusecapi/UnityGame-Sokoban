using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class EditorTileImage : MonoBehaviour, IPointerClickHandler {


    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        LevelEditorUIController.editorAssistant.editionInfo.targetTileName = gameObject.name;
        //LevelEditorUIController.editionInfo.targetTile = this.transform;
        LevelEditorUIController.editorAssistant.editionInfo.tilesSelectionFrame.anchoredPosition = this.GetComponent<RectTransform>().anchoredPosition;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
