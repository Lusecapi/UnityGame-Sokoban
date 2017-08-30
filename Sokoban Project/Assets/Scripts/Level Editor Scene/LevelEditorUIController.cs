using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelEditorUIController : MonoBehaviour {

    public float joystickSense = 0.2f;

    private enum ConfirmationAction
    {
        EraseAll,
        LeaveEditor
    }

    [SerializeField]
    private InputField nameInputField;
    [SerializeField]
    private InputField xInputField;
    [SerializeField]
    private InputField yInputField;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private GameObject editorPanel;
    [SerializeField]
    private GameObject confirmationPanel;
    [SerializeField]
    private RectTransform selectionFrame;
    [Header(" ")]
    [SerializeField]
    private RectTransform myLevelsScrollviewContent;

    private List<LevelButton> levelButtons;
    private int lastLevelButtonIndex = -1;

    private ConfirmationAction actionToConfirm;

    public static LevelEditorAssistant editorAssistant;



	// Use this for initialization
	void Start () {

        menuPanel.SetActive(true);
        editorPanel.SetActive(false);
        confirmationPanel.SetActive(false);
        editorAssistant = new LevelEditorAssistant();
        editorAssistant.editionInfo.tilesSelectionFrame = selectionFrame;
        levelButtons = new List<LevelButton>();
        LevelButton.scrollviewContent = myLevelsScrollviewContent;
        updateMyLevelsButtons();
    }
	
	// Update is called once per frame
	void Update () {

        Camera.main.transform.Translate(CrossPlatformInputManager.GetAxis("Horizontal") * joystickSense, CrossPlatformInputManager.GetAxis("Vertical") * joystickSense, 0);
        Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, 0, LevelEditorUIController.editorAssistant.editionInfo.maxCameraDistance.x), Mathf.Clamp(Camera.main.transform.position.y, 0, LevelEditorUIController.editorAssistant.editionInfo.maxCameraDistance.y), Camera.main.transform.position.z);
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// To open the editor panel and start editing level after set level info
    /// </summary>
    public void OnEditButtonClick()
    {
        editorAssistant.editionInfo.levelName = nameInputField.text;
        if (editorAssistant.verifLevelName(editorAssistant.editionInfo.levelName))
        {
            editorAssistant.editionInfo.levelSize = new Vector2(Int32.Parse(xInputField.text), Int32.Parse(yInputField.text));
            editorAssistant.generateBackTiles(editorAssistant.editionInfo.levelSize);
            setEditorCanvas();
        }

    }

    public void setEditorCanvas()
    {
        menuPanel.SetActive(false);
        editorPanel.SetActive(true);
    }

    /// <summary>
    /// When click on back button and leave editor
    /// </summary>
    public void OnExitButtonClick()
    {
        actionToConfirm = ConfirmationAction.LeaveEditor;
        confirmationPanel.GetComponentInChildren<Text>().text = "Are You Sure?";
        confirmationPanel.SetActive(true);
    }

    public void OnYesButtonClick()
    {
        if(actionToConfirm == ConfirmationAction.EraseAll)
        {
            eraseAllTiles();
            confirmationPanel.SetActive(false);
        }
        else
            if(actionToConfirm == ConfirmationAction.LeaveEditor)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void OnNoButtonClick()
    {
        confirmationPanel.SetActive(false);
    }

    public void OnSaveLevelButtonClick()
    {
        if (editorAssistant.saveLevel(editorAssistant.editionInfo))
        {
            Invoke("finishEditor", 3);
        }
    }

    private void finishEditor()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// When click on Pencil Tool button
    /// </summary>
    public void OnPencilToolButtonClick()
    {
        if (!editorAssistant.editionInfo.isPencilSelected)
        {
            editorAssistant.editionInfo.isPencilSelected = true;
            editorAssistant.editionInfo.isEraserSelected = false;
        }
    }

    /// <summary>
    /// When click on eraser tool button
    /// </summary>
    public void OnEraserToolButtonClick()
    {
        if (!editorAssistant.editionInfo.isEraserSelected)
        {
            editorAssistant.editionInfo.isEraserSelected = true;
            editorAssistant.editionInfo.isPencilSelected = false;
        }
    }

    /// <summary>
    /// when click on erase all tool button
    /// </summary>
    public void OnEraseAllToolButtonClick()
    {
        actionToConfirm = ConfirmationAction.EraseAll;
        confirmationPanel.GetComponentInChildren<Text>().text = "Are You Sure?";
        confirmationPanel.SetActive(true);
    }

    private void eraseAllTiles()
    {
        editorAssistant.editionInfo.wasEraseAll = true;
        editorAssistant.editionInfo.wasUndo = false;
        editorAssistant.editionInfo.lastEditAction = EditAction.EraseAll;
        editorAssistant.editionInfo.lastDeletedTiles.Clear();
        editorAssistant.editionInfo.lastDeletedTiles.AddRange(editorAssistant.editionInfo.tilesPlaced);//initialize with new objects and values values and not with pointer to objects
        for (int i = 0; i < editorAssistant.editionInfo.tilesPlaced.Count; i++)
        {
            Destroy(editorAssistant.editionInfo.tilesPlaced[i].tileGameObject);
        }
        editorAssistant.editionInfo.tilesPlaced.Clear();
    }

    /// <summary>
    /// When click on undo toll button
    /// </summary>
    public void OnUndoToolButtonClick()
    {
        if (!editorAssistant.editionInfo.wasUndo)
        {
            editorAssistant.editionInfo.wasUndo = true;
            if (editorAssistant.editionInfo.lastEditAction == EditAction.Draw)
            {
                Destroy(editorAssistant.editionInfo.tilesPlaced[editorAssistant.editionInfo.tilesPlaced.Count - 1].tileGameObject);
                editorAssistant.editionInfo.tilesPlaced.RemoveAt(editorAssistant.editionInfo.tilesPlaced.Count - 1);
            }
            else
                if (editorAssistant.editionInfo.lastEditAction == EditAction.Erase)
            {
                editorAssistant.editionInfo.wasEraseAll = false;
                GameObject tile = Instantiate(Resources.Load("Tiles/" + editorAssistant.editionInfo.lastDeletedTiles[0].tileName), editorAssistant.editionInfo.lastDeletedTiles[0].position, Quaternion.identity) as GameObject;
                //tile.tag = "Tile in Editor";
                tile.AddComponent<BoxCollider2D>();
                tile.AddComponent<TileInEditorWorldSpace>();
                editorAssistant.editionInfo.tilesPlaced.Add(new TileInEditorWorldSpace.TileInWorldSpace(editorAssistant.editionInfo.lastDeletedTiles[0].tileName, editorAssistant.editionInfo.lastDeletedTiles[0].position, tile));
            }
            else
                if (editorAssistant.editionInfo.lastEditAction == EditAction.EraseAll)
            {
                editorAssistant.editionInfo.wasEraseAll = false;
                for (int i = 0; i < editorAssistant.editionInfo.lastDeletedTiles.Count; i++)
                {
                    editorAssistant.editionInfo.lastDeletedTiles[i].tileGameObject = Instantiate(Resources.Load("Tiles/" + editorAssistant.editionInfo.lastDeletedTiles[i].tileName), editorAssistant.editionInfo.lastDeletedTiles[i].position, Quaternion.identity) as GameObject;
                    //lastDeletedTiles[i].TileGameObject.tag = "Tile in Editor";
                    editorAssistant.editionInfo.lastDeletedTiles[i].tileGameObject.AddComponent<BoxCollider2D>();
                    editorAssistant.editionInfo.lastDeletedTiles[i].tileGameObject.AddComponent<TileInEditorWorldSpace>();


                }
                editorAssistant.editionInfo.tilesPlaced.Clear();
                editorAssistant.editionInfo.tilesPlaced.AddRange(editorAssistant.editionInfo.lastDeletedTiles);
                editorAssistant.editionInfo.lastDeletedTiles.Clear();

            }
        }
    }


    private void updateMyLevelsButtons()
    {
        if (Directory.Exists((SettingsManager.LevelsFilesPath)))
        {
            destroyWorldsButtons();
            try
            {
                string[] levelsPath = Directory.GetFiles(SettingsManager.LevelsFilesPath);
                for (int i = 0; i < levelsPath.Length; i++)
                {
#if UNITY_ANDROID && !UNITY_EDITOR
                    string[] pathArray = levelsPath[i].Split('/');//on android he path are "/Folder1/Folder2/myworld.txt
#elif UNITY_EDITOR
                    string[] pathArray = levelsPath[i].Split('\\'); //on pc the path is like /Folder1/Folder2\\myworld.txt
#endif
                    string levelFile = pathArray[pathArray.Length - 1];
                    createLevelButton(levelFile);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                //Message.showMessageText(e.ToString(), 20);
            }
            //updateWorlds = false;
        }
    }
    /// <summary>
    /// To destroy all the world buttons and clear the world button list
    /// </summary>
    private void destroyWorldsButtons()
    {
        if (levelButtons.Count > 0)
        {
            for (int i = 0; i < levelButtons.Count; i++)
            {
                Destroy(levelButtons[i].levelButtonGameObject);
            }
            lastLevelButtonIndex = -1;
            levelButtons.Clear();
            myLevelsScrollviewContent.sizeDelta = new Vector2(0, 0);
            //worldButtonsList = new List<WorldButton>();
        }
    }

    private void createLevelButton(string levelFile)
    {
        if (levelButtons.Count != 0)
        {
            levelButtons.Add(new LevelButton(levelFile, new Vector2(levelButtons[lastLevelButtonIndex].buttonRectTransform.anchoredPosition.x, levelButtons[lastLevelButtonIndex].buttonRectTransform.anchoredPosition.y - 180)));
        }
        else
        {
            levelButtons.Add(new LevelButton(levelFile));
        }
        lastLevelButtonIndex++;
        fixContentHeight();
    }

    private void fixContentHeight()
    {
        myLevelsScrollviewContent.sizeDelta = new Vector2(0, Mathf.Abs(levelButtons[lastLevelButtonIndex].buttonRectTransform.offsetMin.y));//the offsetMin.y, returns something like the heigh of the worldbutton gameobject
    }



    private class LevelButton
    {
        private GameObject editlevelButtonPrefab = Resources.Load("EditLevelButtonPrefab") as GameObject;
        public static RectTransform scrollviewContent;
        //private int positionDeltaY = -170;

        public GameObject levelButtonGameObject;
        public RectTransform buttonRectTransform;


        public LevelButton(string levelFile)
        {
            levelButtonGameObject = Instantiate(editlevelButtonPrefab);
            buttonRectTransform = levelButtonGameObject.GetComponent<RectTransform>();
            setProperties(levelFile);
        }

        public LevelButton(string levelFile, Vector2 anchoredPosition)
        {
            levelButtonGameObject = Instantiate(editlevelButtonPrefab);
            buttonRectTransform = levelButtonGameObject.GetComponent<RectTransform>();
            setProperties(levelFile);
            buttonRectTransform.anchoredPosition = anchoredPosition;
        }

        private void setProperties(string levelFile)
        {
            levelButtonGameObject.GetComponent<EditLevelButton>().levelFile = levelFile;
            levelButtonGameObject.GetComponentInChildren<Text>().text = levelFile.Split('.')[0];//We show the level name
            //levelButtonGameObject.GetComponent<MyLevelButton>().levelType = LevelType.MyLevel;
            levelButtonGameObject.transform.SetParent(scrollviewContent);
            buttonRectTransform.localScale = Vector3.one;
            buttonRectTransform.offsetMax = Vector2.zero;
            buttonRectTransform.offsetMin = new Vector2(0, -170);
        }
    }

}
