using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum MessageType
{
    Information,
    Confirmation,
    Error
}

public class ShowMessage {

    private static GameObject messageTextPrefab = Resources.Load("MessageTextPrefab") as GameObject;

    public static void showMessageText(string message, MessageType type = MessageType.Information, float showTime = 3)
    {
        GameObject shownMessage = MonoBehaviour.Instantiate(messageTextPrefab);
        shownMessage.GetComponentInChildren<Text>().text = message;
        if(type == MessageType.Information)
        {
            shownMessage.GetComponentInChildren<Text>().color = Color.black;
        }
        else
            if(type == MessageType.Confirmation)
        {
            shownMessage.GetComponentInChildren<Text>().color = Color.green;
        }
        else
            if(type == MessageType.Error)
        {
            shownMessage.GetComponentInChildren<Text>().color = Color.red;
        }
        MonoBehaviour.Destroy(shownMessage, showTime);
    }

}
