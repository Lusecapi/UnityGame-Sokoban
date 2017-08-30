using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public enum EditAction
{
    Draw,
    Erase,
    EraseAll
}

/// <summary>
/// To manage all Editor operation
/// </summary>
public class LevelEditorAssistant {

    public EditionInfo editionInfo;//Every Editor Assistant has an edition info object to storage all the edition information, this is the one

    private string levelPath;

    /// <summary>
    /// Constructor method of Editor Assistant
    /// </summary>
    public LevelEditorAssistant()
    {
        this.editionInfo = new EditionInfo();//Here we create the instance of edition info class we will need;
    }

    /// <summary>
    /// Has all the info about the actual level edition
    /// </summary>
    public class EditionInfo
    {
        public string levelName;
        public Vector2 levelSize;
        public string targetTileName = "Green Ground";
        public RectTransform tilesSelectionFrame;
        public bool reeditingLevel = false;
        public bool isPencilSelected = true;
        public bool isEraserSelected;
        public bool wasUndo;
        public bool wasEraseAll;
        public EditAction lastEditAction;
        public List<TileInEditorWorldSpace.TileInWorldSpace> tilesPlaced = new List<TileInEditorWorldSpace.TileInWorldSpace>();
        public List<TileInEditorWorldSpace.TileInWorldSpace> lastDeletedTiles = new List<TileInEditorWorldSpace.TileInWorldSpace>();
        public Vector2 maxCameraDistance;
    }

    /// <summary>
    /// To generate the back tiles (Canvas) for editing the world
    /// </summary>
    /// <param name="levelSize">The level size in terms of number of tiles</param>
    public void generateBackTiles(Vector2 levelSize)
    {
        GameObject backTilesContainer = new GameObject("Back Tiles");
        GameObject backTilePrefab = Resources.Load("Tiles/Back Tile") as GameObject;
        for (int row = 0; row < levelSize.y; row++)
        {
            for (int colum = 0; colum < levelSize.x; colum++)
            {
                GameObject bt = MonoBehaviour.Instantiate(backTilePrefab, new Vector3(TileCodification.TileSize * colum, TileCodification.TileSize * ((levelSize.y - 1) - row), 0), Quaternion.identity) as GameObject;
                bt.transform.SetParent(backTilesContainer.transform);
            }
        }
        LevelEditorUIController.editorAssistant.editionInfo.maxCameraDistance = new Vector2(TileCodification.TileSize * levelSize.x, TileCodification.TileSize * levelSize.y);
    }

    /// <summary>
    /// To Verify the level name is valid and theres is not another level with that name
    /// </summary>
    /// <param name="levelName">The level name</param>
    /// <returns>true if is valid, false if not</returns>
    public bool verifLevelName(string levelName)
    {
        setLevelPath(levelName);
        if (levelName != "")
        {
            if (!File.Exists(levelPath))
            {
                return true;
            }
            else
            {
                ShowMessage.showMessageText("That level already exist", MessageType.Error);
            }
        }
        else
        {
            ShowMessage.showMessageText("Invalid name", MessageType.Error);
        }
        return false;
    }

    /// <summary>
    /// To set the actual levelName, the path
    /// </summary>
    /// <param name="levelName">The level name</param>
    private void setLevelPath(string levelName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            levelPath = SettingsManager.LevelsFilesPath + "/" + levelName + ".txt";
#elif UNITY_EDITOR
        levelPath = SettingsManager.LevelsFilesPath + "\\" + levelName + ".txt";
#endif
    }

    /// <summary>
    /// To read the existing level file and set the scene for edit it
    /// </summary>
    /// <param name="editionInfo"></param>
    public void readLevelFile(EditionInfo editionInfo)
    {
        StreamReader sr = new StreamReader(SettingsManager.LevelsFilesPath + "/" + editionInfo.levelName + ".txt");
        string line = sr.ReadLine();
        int columns = int.Parse(line.Split('x')[0]);
        int rows = int.Parse(line.Split('x')[1]);//if needs
        editionInfo.levelSize = new Vector2(columns, rows);
        generateBackTiles(editionInfo.levelSize);
        int row = 0;
        while (!sr.EndOfStream)
        {
            line = sr.ReadLine();
            string[] cells = line.Split(';');
            for (int colum = 0; colum < columns; colum++)
            {
                string tileName;
                string[] cellsTiles = cells[colum].Split('-');//The two tiles in this position: 0 if there is nothing
                if (cellsTiles[0] != "0")
                {
                    tileName = TileCodification.getTileName(cellsTiles[0]);
                    instantiateTile(tileName, new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row), 0f));
                    if (cellsTiles[1] != "0")
                    {
                        tileName = TileCodification.getTileName(cellsTiles[1]);
                        instantiateTile(tileName, new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row), 0f));
                    }
                    else
                    {
                        //Instantiate nothing
                    }
                }
                else
                {
                    if (cellsTiles[1] != "0")
                    {
                        tileName = TileCodification.getTileName(cellsTiles[1]);
                        instantiateTile(tileName, new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row), 0f));
                    }
                    else
                    {
                        //Instantiate nothing
                    }
                }
            }
            row++;
        }
        sr.Close();
    }

    /// <summary>
    /// To instantiate tile and set as tilesGameOcject child, also to optimize and to clean code because we use this multiple times
    /// </summary>
    /// <param name="tileName"></param>
    /// <param name="position"></param>
    private void instantiateTile(string tileName, Vector3 position)
    {
        GameObject tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
        GameObject tile = MonoBehaviour.Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
        tile.AddComponent<BoxCollider2D>();
        tile.AddComponent<TileInEditorWorldSpace>();
        LevelEditorUIController.editorAssistant.editionInfo.tilesPlaced.Add(new TileInEditorWorldSpace.TileInWorldSpace(tileName, position, tile));
    }


    /// <summary>
    /// To verify spawn points on the level
    /// </summary>
    /// <param name="tilesPlaced">The tiles</param>
    /// <returns>true if there is on spawn point, fase if not or are more than one</returns>
    private bool verifSpawnPoint(List<TileInEditorWorldSpace.TileInWorldSpace> tilesPlaced)
    {
        int spawnPoints = 0;
        if (tilesPlaced.Count != 0)
        {
            for (int index = 0; index < tilesPlaced.Count; index++)
            {
                Debug.Log(tilesPlaced[index].tileName);
                if (tilesPlaced[index].tileName == "Spawn Point")
                {
                    spawnPoints++;
                }
                if (spawnPoints > 1)
                {
                    //Debug.Log("There are multiple spawn points on scene, there must be just one");
                    ShowMessage.showMessageText("There are multiple spawn points on scene, there must be just one", MessageType.Error, 5);
                    return false;
                }
            }
            if (spawnPoints == 1)
            {
                return true;
            }
            else
            {
                ShowMessage.showMessageText("There isn't a Spawn Point", MessageType.Error);
            }
        }
        else
        {
            ShowMessage.showMessageText("There is not a single one tile", MessageType.Error);
        }
        return false;
    }

    /// <summary>
    /// To verify the number of crates and crate points
    /// </summary>
    /// <param name="tilesPlaced"></param>
    /// <returns></returns>
    private bool verifCratesPointAndCrates(List<TileInEditorWorldSpace.TileInWorldSpace> tilesPlaced)
    {
        int cratePoint = 0;
        int crates = 0;
        for (int i = 0; i < tilesPlaced.Count; i++)
        {
            if(tilesPlaced[i].tileName.Contains("Crate Point"))
            {
                cratePoint++;
            }
            else
                if (tilesPlaced[i].tileName.Contains("Crate"))
            {
                crates++;
            }
        }
        if(cratePoint == crates && cratePoint > 0 && crates > 0) 
        {
            return true;
        }

        ShowMessage.showMessageText("The number of crates and crates points doesn't match or there isn't a single one of both", MessageType.Error, 5);
        return false;
    }

    /// <summary>
    /// To save the level
    /// </summary>
    /// <param name="levelName">the level name</param>
    /// <param name="levelSize">level size in terms of number of rows and columns</param>
    /// <param name="tilesPlaced">The tiles placed</param>
    public bool saveLevel(EditionInfo editInformation)
    {
        //string levelName, Vector2 levelSize, List<TileInEditorWorldSpace.TileInWorldSpace> tilesPlaced
        if (verifSpawnPoint(editInformation.tilesPlaced))//If there is one spawn point
        {
            if (verifCratesPointAndCrates(editInformation.tilesPlaced))
            {
                if (editInformation.reeditingLevel)
                {
                    setLevelPath("temp_");
                }
                try
                {
                    StreamWriter sw = new StreamWriter(levelPath);
                    sw.WriteLine(editInformation.levelSize.x + "x" + editInformation.levelSize.y);

                    for (int row = (int)editInformation.levelSize.y - 1; row >= 0; row--)
                    {
                        string line = "";
                        for (int colum = 0; colum <= editInformation.levelSize.x - 1; colum++)
                        {
                            bool swt1 = false;
                            int tileIndex = 0;
                            Vector3 searchTileOnPosition = new Vector3(TileCodification.TileSize * colum, TileCodification.TileSize * row, 0);
                            while (tileIndex < editInformation.tilesPlaced.Count && swt1 == false)
                            {
                                if (editInformation.tilesPlaced[tileIndex].position == searchTileOnPosition)//Look for the actual tile, placed at the position (tileSize*row,tileSize*colum,0)
                                {
                                    swt1 = true;
                                }
                                else
                                {
                                    tileIndex++;
                                }
                            }
                            if (swt1)//If found one
                            {
                                if (editInformation.tilesPlaced[tileIndex].tileGameObject.tag == "Ground")//Only if the Tile is a Ground Tile, we search again for another possible tile that could be over it
                                {

                                    //We make the second Search omiting the one we found
                                    bool swt2 = false;
                                    int otherTileIndex = 0;
                                    while (otherTileIndex < editInformation.tilesPlaced.Count && !swt2)
                                    {
                                        if (editInformation.tilesPlaced[otherTileIndex].position == searchTileOnPosition && otherTileIndex != tileIndex)//Look for the actual tile, placed at the same position
                                        {
                                            swt2 = true;
                                        }
                                        else
                                        {
                                            otherTileIndex++;
                                        }
                                    }

                                    if (swt2)//if we found another tile over the first one
                                    {
                                        string fistTileCode = TileCodification.getTileCode(editInformation.tilesPlaced[tileIndex].tileName);
                                        string secondTileCode = TileCodification.getTileCode(editInformation.tilesPlaced[otherTileIndex].tileName);
                                        if (colum == 0)
                                        {
                                            line = fistTileCode + "-" + secondTileCode;
                                        }
                                        else
                                        {
                                            line = line + ";" + fistTileCode + "-" + secondTileCode;
                                        }

                                    }
                                    else//Dind't found the posible second Tile
                                    {
                                        if (colum == 0)
                                        {
                                            line = TileCodification.getTileCode(editInformation.tilesPlaced[tileIndex].tileName) + "-0";
                                        }
                                        else
                                        {
                                            line = line + ";" + TileCodification.getTileCode(editInformation.tilesPlaced[tileIndex].tileName) + "-0";
                                        }
                                    }
                                }
                                else//Found a tile that isn't a ground tile
                                {
                                    if (colum == 0)
                                    {
                                        line = "0-" + TileCodification.getTileCode(editInformation.tilesPlaced[tileIndex].tileName);
                                    }
                                    else
                                    {
                                        line = line + ";0-" + TileCodification.getTileCode(editInformation.tilesPlaced[tileIndex].tileName);
                                    }

                                }
                            }
                            else//There is no tile there, so it's 0
                            {
                                if (colum == 0)
                                {
                                    line = "0-0";
                                }
                                else
                                {
                                    line = line + ";0-0";
                                }
                            }
                        }
                        //Debug.Log(line);
                        sw.WriteLine(line);
                    }
                    sw.Close();

                    if (editInformation.reeditingLevel)
                    {
                        string sourceFileName = levelPath;
                        setLevelPath(editionInfo.levelName);
                        string destFileName = levelPath;
                        File.Delete(destFileName);
                        File.Move(sourceFileName, destFileName);
                        File.Delete(sourceFileName);
                    }
                    ShowMessage.showMessageText("World Saved Succesfully", MessageType.Confirmation);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                    ShowMessage.showMessageText(e.ToString(), MessageType.Error);
                }
            }
        }

        return false;
    }
}
