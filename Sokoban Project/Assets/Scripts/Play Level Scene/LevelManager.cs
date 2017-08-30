using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour {

    public class Level
    {
        public string levelFile = "Level00";
        public LevelType levelType = LevelType.SokobanLevel;
        public int levelCratesPoints = 0;
        public int matchTime = 0;

        public Level(string levelFile, LevelType levelType = LevelType.SokobanLevel)
        {
            this.levelFile = levelFile;
            this.levelType = levelType;
        }

        public Level()
        {

        }
    }


    public static Level runningLevel = new Level();
    private GameObject spawnPoint;
    private GameObject tilesGameObject;
    [SerializeField]
    private Text matchTimeText;
    private AudioSource levelMusic;

    void Awake()
    {
        levelMusic = GetComponent<AudioSource>();
    }

	// Use this for initialization
	void Start () {
        loadLevel(runningLevel.levelFile);
        spawnPlayer();
    }
	
	// Update is called once per frame
	void Update () {

        runningLevel.matchTime = (int)Time.timeSinceLevelLoad;
        int minutes = runningLevel.matchTime / 60;
        int seconds = runningLevel.matchTime % 60;
        matchTimeText.text = string.Format("{0:0}:{1:00}", minutes, seconds);

    }


    /// <summary>
    /// Reads level file and load tiles in world space
    /// </summary>
    /// <param name="fileName">The level file</param>
    private void loadLevel(string fileName)
    {
        tilesGameObject = new GameObject("Tiles");

        if (runningLevel.levelType == LevelType.SokobanLevel)
        {
            //worldsFilesPath = Application.dataPath + "/Resources/Files/Level Files";
            //Reading a txt file inside resources folder of the project in adroid build (Android), also works on editor so no problem
            TextAsset path = Resources.Load<TextAsset>("Files/Levels Files/" + fileName);
            using (StreamReader sr = new StreamReader(new MemoryStream(path.bytes)))
            {
                string line = sr.ReadLine();
                int columns = int.Parse(line.Split('x')[0]);
                int rows = int.Parse(line.Split('x')[1]);//if needs
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
                                if (tileName != "Spawn Point")
                                {
                                    if (tileName.Contains("Crate Point"))
                                    {
                                        runningLevel.levelCratesPoints++;
                                    }

                                    instantiateTile(tileName, new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row), 0f));
                                }
                                else
                                {
                                    spawnPoint = new GameObject("Spawn Point");
                                    spawnPoint.transform.position = new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row));
                                }
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
                                if (tileName != "Spawn Point")
                                {
                                    if (tileName.Contains("Crate Point"))
                                    {
                                        runningLevel.levelCratesPoints++;
                                    }

                                    instantiateTile(tileName, new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row), 0f));
                                }
                                else
                                {
                                    spawnPoint = new GameObject("Spawn Point");
                                    spawnPoint.transform.position = new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row));
                                }
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
        }
        else
            if(runningLevel.levelType == LevelType.MyLevel)
        {
            StreamReader sr = new StreamReader(SettingsManager.LevelsFilesPath + "/" + fileName);
            string line = sr.ReadLine();
            int columns = int.Parse(line.Split('x')[0]);
            int rows = int.Parse(line.Split('x')[1]);//if needs
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
                            if (tileName != "Spawn Point")
                            {
                                if (tileName.Contains("Crate Point"))
                                {
                                    runningLevel.levelCratesPoints++;
                                }

                                instantiateTile(tileName, new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row), 0f));
                            }
                            else
                            {
                                spawnPoint = new GameObject("Spawn Point");
                                spawnPoint.transform.position = new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row));
                            }
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
                            if (tileName != "Spawn Point")
                            {
                                if (tileName.Contains("Crate Point"))
                                {
                                    runningLevel.levelCratesPoints++;
                                }

                                instantiateTile(tileName, new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row), 0f));
                            }
                            else
                            {
                                spawnPoint = new GameObject("Spawn Point");
                                spawnPoint.transform.position = new Vector3(TileCodification.TileSize * (colum), TileCodification.TileSize * ((rows - 1) - row));
                            }
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
    }


    /// <summary>
    /// To instantiate tile and set as tilesGameOcject child, also to optimize and to clean code because we use this multiple times
    /// </summary>
    /// <param name="tileName"></param>
    /// <param name="position"></param>
    private void instantiateTile(string tileName, Vector3 position)
    {
        GameObject tilePrefab = Resources.Load("Tiles/" + tileName) as GameObject;
        GameObject go = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
        go.transform.SetParent(tilesGameObject.transform);
    }

    /// <summary>
    /// Spawns the player on the level spawn point and set the main camera as child of players
    /// </summary>
    private void spawnPlayer()
    {
        GameObject playerPrefab = Resources.Load("Player") as GameObject;
        GameObject p = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity) as GameObject;
        Camera.main.transform.SetParent(p.transform);
        Camera.main.transform.localPosition = Vector3.zero + new Vector3(0, 0, -10);
    }

    private void finishGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void updateCratesPoints(bool operation)
    {
        if (operation)
        {
            runningLevel.levelCratesPoints--;
            if(runningLevel.levelCratesPoints == 0)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>().enabled = false;
                Invoke("finishGame", 1);
            }
        }
        else
        {
            runningLevel.levelCratesPoints++;
        }
    }


}
