using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class TileInEditorWorldSpace: MonoBehaviour, IPointerClickHandler
{

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (LevelEditorUIController.editorAssistant.editionInfo.isEraserSelected)
        {
            bool sw = false;
            LevelEditorUIController.editorAssistant.editionInfo.wasUndo = false;
            LevelEditorUIController.editorAssistant.editionInfo.lastEditAction = EditAction.Erase;
            for (int i = LevelEditorUIController.editorAssistant.editionInfo.tilesPlaced.Count - 1; i >= 0; i--)//Toca buscarlo en Reversa para borrar las posibles tiles que esten sobre otras, las cuales fueron colocadas de ultimo
            {
                TileInEditorWorldSpace.TileInWorldSpace tile = LevelEditorUIController.editorAssistant.editionInfo.tilesPlaced[i];
                if (tile.position == transform.position && !sw)
                {
                    //Delete the Object and removing from the list
                    LevelEditorUIController.editorAssistant.editionInfo.lastDeletedTiles.Insert(0, tile);
                    LevelEditorUIController.editorAssistant.editionInfo.tilesPlaced.Remove(tile);
                    Destroy(tile.tileGameObject);
                    sw = true;
                }
            }
            if (LevelEditorUIController.editorAssistant.editionInfo.tilesPlaced.Count == 0)
            {
                LevelEditorUIController.editorAssistant.editionInfo.wasEraseAll = true;
            }
            else
            {
                LevelEditorUIController.editorAssistant.editionInfo.wasEraseAll = false;
            }
        }
        else
            if (LevelEditorUIController.editorAssistant.editionInfo.isPencilSelected)
        {
            GameObject tilePrefab = Resources.Load("Tiles/" + LevelEditorUIController.editorAssistant.editionInfo.targetTileName) as GameObject;
            if (transform.tag == "Ground" && tilePrefab.tag != "Ground")//Only can put another tile over a ground tile
            {

                LevelEditorUIController.editorAssistant.editionInfo.wasUndo = false;
                LevelEditorUIController.editorAssistant.editionInfo.wasEraseAll = false;
                LevelEditorUIController.editorAssistant.editionInfo.lastEditAction = EditAction.Draw;
                GameObject tile = Instantiate(tilePrefab, transform.position, Quaternion.identity) as GameObject;

                //Now We Add the Components We really Need
                tile.AddComponent<BoxCollider2D>();
                tile.AddComponent<TileInEditorWorldSpace>();
                LevelEditorUIController.editorAssistant.editionInfo.tilesPlaced.Add(new TileInWorldSpace(LevelEditorUIController.editorAssistant.editionInfo.targetTileName, transform.position, tile));
            }
        }
    }

    /// <summary>
    /// THis class is to save the tilesplaced in worldspace on the tilesplaced list
    /// </summary>
    public class TileInWorldSpace
    {
        public string tileName;
        public Vector3 position;
        public GameObject tileGameObject;

        public TileInWorldSpace(string theName, Vector3 thePosition, GameObject theGameObject)
        {
            this.tileName = theName;
            this.position = thePosition;
            this.tileGameObject = theGameObject;
        }
    }
}
