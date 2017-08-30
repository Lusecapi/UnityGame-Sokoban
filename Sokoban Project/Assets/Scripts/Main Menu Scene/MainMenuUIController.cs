using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.IO;
using System;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour {

    private enum ConfirmationAction
    {
        DeleteLevel,
        QuitApplication
    }

    [Header("Panels")]
    [SerializeField]
    private GameObject mainPanel;
    [SerializeField]
    private GameObject selectGamePanel;
    [SerializeField]
    private GameObject sokobanLevelsPanels;
    [SerializeField]
    private GameObject myLevelsPanel;
    [SerializeField]
    private GameObject confirmationPanel;
    [Header(" ")]
    [SerializeField]
    private RectTransform myLevelsScrollviewContent;

    private List<LevelButton> levelButtons;
    private int lastLevelButtonIndex = -1;

    private Confirmation confirmation = null;




	// Use this for initialization
	void Start () {

        activePanel(0);
        levelButtons = new List<LevelButton>();
        LevelButton.scrollviewContent = myLevelsScrollviewContent;
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Enable specifed panel
    /// </summary>
    /// <param name="panelIndex">The index, based on the order they are declared</param>
    private void activePanel(int panelIndex)
    {
        switch (panelIndex)
        {
            case 0:
                mainPanel.SetActive(true);
                selectGamePanel.SetActive(false);
                sokobanLevelsPanels.SetActive(false);
                myLevelsPanel.SetActive(false);
                confirmationPanel.SetActive(false);
                break;
            case 1:
                mainPanel.SetActive(false);
                selectGamePanel.SetActive(true);
                sokobanLevelsPanels.SetActive(false);
                myLevelsPanel.SetActive(false);
                confirmationPanel.SetActive(false);
                break;
            case 2:
                mainPanel.SetActive(false);
                selectGamePanel.SetActive(false);
                sokobanLevelsPanels.SetActive(true);
                myLevelsPanel.SetActive(false);
                confirmationPanel.SetActive(false);
                break;
            case 3:
                mainPanel.SetActive(false);
                selectGamePanel.SetActive(false);
                sokobanLevelsPanels.SetActive(false);
                myLevelsPanel.SetActive(true);
                confirmationPanel.SetActive(false);
                break;
            case 4:
                confirmationPanel.SetActive(true);
                break;
        }
    }

    #region UI Methods

    #region Main Panel Actions

    public void OnPlayButtonClick()
    {
        activePanel(1);
    }

    public void OnLevelEditorButtonClick()
    {
        SceneManager.LoadScene(3);
    }

    public void OnQuitGameButtonClick()
    {
        confirmation = new Confirmation(ConfirmationAction.QuitApplication);
        confirmationPanel.GetComponentInChildren<Text>().text = "Are You Sure?";
        activePanel(4);
    }

    #endregion

    #region Select Game Actions

    public void OnBackToMainMenuButtonClick()
    {
        activePanel(0);
    }

    public void OnSokobanLevelsButtonsClick()
    {
        activePanel(2);
    }

    public void OnMyLevelsButtonClick()
    {
        activePanel(3);
        updateMyLevelsButtons();
    }

    #endregion

    #region Levels Actions

    public void OnBackToSelectGameButtonClick()
    {
        activePanel(1);
    }

    public void playLevel(string levelFile)
    {
        LevelManager.runningLevel = new LevelManager.Level(levelFile);
        SceneManager.LoadScene("PlayLevel");
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

    #endregion

    #endregion

    #region Confirmation Actions

    public void deleteLevelButton(string levelFile)
    {
        confirmation = new Confirmation(ConfirmationAction.DeleteLevel, levelFile);
        confirmationPanel.GetComponentInChildren<Text>().text = "Are you Sure?";
        activePanel(4);
    }

    public void OnYesButtonClick()
    {
        if(confirmation.actionToConfirm == ConfirmationAction.QuitApplication)
        {
            Application.Quit();
        }
        else
            if(confirmation.actionToConfirm == ConfirmationAction.DeleteLevel)
        {
            try
            {
                File.Delete(SettingsManager.LevelsFilesPath + "/" + confirmation.levelFile);
                ShowMessage.showMessageText("Level deleted succesfully", MessageType.Confirmation);
                confirmationPanel.SetActive(false);
                updateMyLevelsButtons();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                ShowMessage.showMessageText(e.ToString());
            }
        }
    }


    public void OnNoButtonClick()
    {
        confirmationPanel.SetActive(false);
    }
    #endregion



    private class Confirmation
    {
        public ConfirmationAction actionToConfirm;

        /// <summary>
        /// Only when deleting a levelButton
        /// </summary>
        public string levelFile;

        public Confirmation(ConfirmationAction actionToConfirm)
        {
            this.actionToConfirm = actionToConfirm;

        }

        public Confirmation(ConfirmationAction actionToConfirm, string levelFile)
        {
            this.actionToConfirm = actionToConfirm;
            this.levelFile = levelFile;
        }
    }


    private class LevelButton
    {
        private GameObject levelButtonPrefab = Resources.Load("LevelButtonPrefab") as GameObject;
        public static RectTransform scrollviewContent;
        //private int positionDeltaY = -170;

        public GameObject levelButtonGameObject;
        public RectTransform buttonRectTransform;


        public LevelButton(string levelFile)
        {
            levelButtonGameObject = Instantiate(levelButtonPrefab);
            buttonRectTransform = levelButtonGameObject.GetComponent<RectTransform>();
            setProperties(levelFile);
        }

        public LevelButton(string levelFile, Vector2 anchoredPosition)
        {
            levelButtonGameObject = Instantiate(levelButtonPrefab);
            buttonRectTransform = levelButtonGameObject.GetComponent<RectTransform>();
            setProperties(levelFile);
            buttonRectTransform.anchoredPosition = anchoredPosition;
        }

        private void setProperties(string levelFile)
        {
            levelButtonGameObject.GetComponent<MyLevelButton>().levelFile = levelFile;
            levelButtonGameObject.GetComponentInChildren<Text>().text = levelFile.Split('.')[0];//We show the level name
            levelButtonGameObject.GetComponent<MyLevelButton>().levelType = LevelType.MyLevel;
            levelButtonGameObject.transform.SetParent(scrollviewContent);
            buttonRectTransform.localScale = Vector3.one;
            buttonRectTransform.offsetMax = Vector2.zero;
            buttonRectTransform.offsetMin = new Vector2(0, -170);
        }
    }
}
