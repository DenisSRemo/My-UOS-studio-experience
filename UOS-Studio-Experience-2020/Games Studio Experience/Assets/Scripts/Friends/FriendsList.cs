using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FriendsList : MonoBehaviour
{
    public GameObject FriendPrefab;
    public Transform FriendsListParent;
    public InputField FriendInput;

    private static string FriendsListFolder = "FriendsData", FriendsFileName = "Data";
    private string FilePath;
    public static List<string> TrackedFriends = new List<string>();//live updooted list of frens

    private void Start()
    {
        //Keeps us running
        //DontDestroyOnLoad(gameObject);

        FilePath = Application.dataPath + "/Resources/" + FriendsListFolder +"/" +FriendsFileName + ".txt";

        UpdateFriends();

        //Keeps friends list updated for future things such as em going online or offline
        InvokeRepeating("RefreshFriends", 0, 3f);
    }

    public static void UpdateFriends()
    {
        TrackedFriends.Clear();

        string[] LoadedFriends = GetFriends();
        if (LoadedFriends != null)
        {
            TrackedFriends.AddRange(LoadedFriends);

        }
    }

    public void AddFriend()//Uses value from input field
    {
        if (!FriendInput) { Debug.LogError("No friend text input set"); }

        string FriendName = FriendInput.text;
        FriendInput.text = "";

        if (TrackedFriends.Contains(FriendName)) { return; }        

        TrackedFriends.Add(FriendName);

        RefreshFriends();

        SaveFriends(TrackedFriends.ToArray());
    }

    public void RemoveFriend(string FriendName)
    {
        if (!TrackedFriends.Contains(FriendName)) { return; }

        TrackedFriends.Remove(FriendName);

        RefreshFriends();

        SaveFriends(TrackedFriends.ToArray());
    }

    private void RefreshFriends()
    {
        if (!FriendsListParent) { return; }
        
        //CLear old friend cards
        List<Transform> SpawnedFriends = new List<Transform>();
        SpawnedFriends.AddRange(FriendsListParent.GetComponentsInChildren<Transform>());
        while(SpawnedFriends.Count>0)
        {
            if(SpawnedFriends[0] == FriendsListParent) { SpawnedFriends.RemoveAt(0);continue; }
            Destroy(SpawnedFriends[0].gameObject);
            SpawnedFriends.RemoveAt(0);
        }

        //Spawn new friend cards
        foreach (string s in TrackedFriends)
        {
            FriendObject newFriendCard = SpawnFriend(s);
            newFriendCard.UpdateStatus(GetFriendState(s));
        }
    }

    private FriendObject SpawnFriend(string Name)
    {
        FriendObject NewFriend = Instantiate(FriendPrefab, FriendsListParent).GetComponent<FriendObject>();
        NewFriend.MyName.text = Name;
        return NewFriend;
    }

    private FriendObject.StatusStates GetFriendState(string FriendName)
    {
        //ping them/call server here

        int rand = Random.Range(1, 3);

        if(rand == 1)
        {
            return FriendObject.StatusStates.Offline;
        }
        else
        {
            return FriendObject.StatusStates.Online;
        }
    }

    private static string[] GetFriends()
    {
        string FilePath = Application.dataPath + "/Resources/" + FriendsListFolder + "/" + FriendsFileName + ".txt";

        CheckDirectory();

        List<string> Friends = new List<string>();

        if (!File.Exists(FilePath)) { return null; }

        StreamReader sr = File.OpenText(FilePath);
        while (!sr.EndOfStream)
        {
            Friends.Add(sr.ReadLine());
        }

        sr.Close();

        return Friends.ToArray();
    }

    private void SaveFriends(string[] Friends)
    {
        CheckDirectory();

        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        StreamWriter SW = File.CreateText(FilePath);

        foreach(string s in Friends)
        {
            SW.WriteLine(s);
        }
        
        SW.Close();
    }

    private static void CheckDirectory()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/" + FriendsListFolder))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/" + FriendsListFolder);
            Debug.Log("Created friends data directory.");
        }
    }
}
