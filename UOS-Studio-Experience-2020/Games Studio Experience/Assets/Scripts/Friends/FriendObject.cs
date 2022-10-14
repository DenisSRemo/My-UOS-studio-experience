using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendObject : MonoBehaviour
{
    public Text MyName;

    public Sprite OnlineSprite;
    public Sprite OfflineSprite;
    public Image StatusImage;

    public Sprite OfflineText;
    public Sprite OnlineText;
    public Image OnlineImage;

    public void RemoveFromFriends()
    {
        FriendsList FL = (FriendsList)GameObject.FindObjectOfType(typeof(FriendsList));

        FL.RemoveFriend(MyName.text);
    }

    public void OpenChat()
    {

    }

    public enum StatusStates
    {
        Offline,
        Online
    }
    public void UpdateStatus(StatusStates MyState)
    {
        switch(MyState)
        {
            case StatusStates.Offline:
                StatusImage.sprite = OfflineSprite;
                StatusImage.color = Color.red;
                OnlineImage.sprite = OfflineText;
                break;
            case StatusStates.Online:
                StatusImage.sprite = OnlineSprite;
                StatusImage.color = Color.green;
                OnlineImage.sprite = OnlineText;
                break;
        }
    }

}
