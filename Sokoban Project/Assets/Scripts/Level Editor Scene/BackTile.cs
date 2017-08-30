using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class BackTile : MonoBehaviour, IPointerClickHandler {


    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    { 
        if (LevelEditorUIController.editorAssistant.editionInfo.isPencilSelected)
        {
            LevelEditorUIController.editorAssistant.editionInfo.wasUndo = false;
            LevelEditorUIController.editorAssistant.editionInfo.wasEraseAll = false;
            LevelEditorUIController.editorAssistant.editionInfo.lastEditAction = EditAction.Draw;
            Debug.Log(LevelEditorUIController.editorAssistant.editionInfo.targetTileName);
            GameObject tile = Instantiate(Resources.Load("Tiles/" + LevelEditorUIController.editorAssistant.editionInfo.targetTileName), transform.position, Quaternion.identity) as GameObject;

            ////Now We Add the Components We really Need
            tile.AddComponent<BoxCollider2D>();
            tile.AddComponent<TileInEditorWorldSpace>();
            LevelEditorUIController.editorAssistant.editionInfo.tilesPlaced.Add(new TileInEditorWorldSpace.TileInWorldSpace(LevelEditorUIController.editorAssistant.editionInfo.targetTileName, transform.position, tile));
        }
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
