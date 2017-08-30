using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class SettingsManager {

    public string userName = "Logan";

    private string settingsFile = "Settings.txt";
    private string settingsFilePath = Application.dataPath + "/Resources/Files/Settings.txt";//;by default
    private static string levelsFilesPath = Application.dataPath + "/Resources/Files/My Levels";////by default

    public static string LevelsFilesPath
    {
        get
        {
            return levelsFilesPath;
        }
    }

    /// <summary>
    ///  Verifye all the configuratios, files paths, folders, etc. and set the stuff application needs to work perfectly
    /// </summary>
    /// <returns>false when this finish all processes, it means there is not loading anymore</returns>
    public bool setConfigurationsAndSettings()
    {
        verifySettingsFilePath();
        verifyLevelsPath();
        return false;
    }

    /// <summary>
    /// VErify existence of the setings file, and if yes, load the data
    /// </summary>
    private void verifySettingsFilePath()
    {
        setSettingsPath();
        if (File.Exists(settingsFilePath))//Load previous settings
        {
            loadSettings();
        }
        else//create the settings file with the default value. (To creae the file when first installed, or if deleted
        {
            createSettingsFile();
        }
    }

    /// <summary>
    /// To set the settings file path depending on the execution platform
    /// </summary>
    private void setSettingsPath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        settingsFilePath = Application.persistentDataPath + "/" + settingsFile;
#elif UNITY_EDITOR
        settingsFilePath = Application.dataPath + "/Resources/Files/" + settingsFile;
#endif
    }

    /// <summary>
    /// Creates the Settings file
    /// </summary>
    private void createSettingsFile()
    {
        try
        {
            StreamWriter sw = new StreamWriter(settingsFilePath);
            sw.WriteLine("Name:" + userName);
            sw.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            //Message.showMessageText(e.ToString());
        }
    }

    /// <summary>
    /// Loads data from Settings File
    /// </summary>
    private void loadSettings()
    {
        try
        {
            StreamReader sr = new StreamReader(settingsFilePath);
            string line = sr.ReadLine();
            userName = line.Split(':')[1];
            sr.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            //Message.showMessageText(e.ToString());
        }
    }


    private void verifyLevelsPath()
    {
        setLevelsFilesPath();
        if (!Directory.Exists(levelsFilesPath))
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                if (!Directory.Exists("/storage/emulated/0/Sokoban World"))//If there is no Super Sokoban World Folder on sd (Usually when first installed)
                {
                    Directory.CreateDirectory("/storage/emulated/0/Sokoban World");//Creates the folder
                }
#elif UNITY_EDITOR
                //On the editor the folder "always exist", dont erase anaything, I create them manually
#endif
                Directory.CreateDirectory(levelsFilesPath);//Creates the My Levels folder where levels are storage
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                //Message.showMessageText(e.ToString());
            }
        }
    }

    private void setLevelsFilesPath()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        levelsFilesPath = "/storage/emulated/0/Sokoban World/My Levels";
#elif UNITY_EDITOR
        levelsFilesPath = Application.dataPath + "/Resources/Files/My Levels";
#endif
    }
}
