using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using SRMultiplayer;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;

public class ChatUI : SRSingleton<ChatUI>
{
    private bool openChat;
    private string message;
    private float fadeTime;
    private List<ChatMessage> messages = new List<ChatMessage>();
    private Vector2 chatScroll;
    int maxWidth = 290;

    /// <summary>
    /// create a chat message to be displayed
    /// </summary>
    public class ChatMessage
    {
        public string Text;
        public float FadeTime;
        public DateTime Time;

        public ChatMessage(string msg)
        {
            Text = msg;
            FadeTime = 10f;
            Time = DateTime.Now;
        }
    }
    /// <summary>
    /// On chat ui update triggered, handle it
    /// </summary>
    private void Update()
    {
        //only display chat if in multiplayer mode
        if (!Globals.IsMultiplayer)
        {
            openChat = false;
            message = "";
            return;
        }
        //if enter is pressed start chat typing mode
        if (Input.GetKeyUp(KeyCode.Return))
        {
            StartCoroutine(FocusChat());
        }
    }
    /// <summary>
    /// Crreate the Chat gui
    /// </summary>
    private void OnGUI()
    {
        //only mess with the gui in multiplayer mode
        if (!Globals.IsMultiplayer) return;

        if (openChat)
        {
            //draw chat area 
            GUILayout.BeginArea(new Rect(20, Screen.height / 2, maxWidth + 10, 300), GUI.skin.box);
            chatScroll = GUILayout.BeginScrollView(chatScroll);
            var skin = GUI.skin.box;
            skin.wordWrap = true;
            skin.alignment = TextAnchor.MiddleLeft;

            //add each mesage into the chat box scroller
            foreach (var msg in messages)
            {
                GUILayout.Label(wrapString(msg.Text, maxWidth), skin, GUILayout.MaxWidth(maxWidth));
            }
            GUILayout.EndScrollView();

            //add display for text input area
            GUI.SetNextControlName("ChatInput");
            message = GUILayout.TextField(message);
            GUILayout.EndArea();

            //focus the input
            GUI.FocusControl("ChatInput");

            //watch for changes to the text input
            Event e = Event.current;
            if (e.rawType == EventType.KeyUp && e.keyCode == KeyCode.Return)
            {
                //on send close chat
                openChat = !openChat;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    //if server send message to all plauers
                    if (Globals.IsServer)
                    {
                        //if server send it to the server
                        AddChatMessage(Globals.Username + ": " + message);
                        new PacketPlayerChat()
                        {
                            message = Globals.Username + ": " + message
                        }.Send();
                    }
                    else
                    {
                        //if player send it to the server
                        new PacketPlayerChat()
                        {
                            message = message
                        }.Send();
                    }
                    message = "";
                }
            }
        }
        else //if chat isnt open yet 
        {
            //draw chat area 
            GUILayout.BeginArea(new Rect(20, Screen.height / 2, maxWidth+ 10, 300));
            chatScroll = GUILayout.BeginScrollView(chatScroll);
            var skin = GUI.skin.box;
            skin.wordWrap = true;
            skin.alignment = TextAnchor.MiddleLeft;

            //add each mesage into the chat box scroller
            foreach (var msg in messages)
            {
                if (msg.FadeTime > 0f)
                {
                    msg.FadeTime -= Time.deltaTime;
                    var c = GUI.color;
                    c.a = (msg.FadeTime / 5f);
                    GUI.color = c;
                    GUILayout.Label(wrapString(msg.Text, maxWidth), skin, GUILayout.MaxWidth(maxWidth));
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
    /// <summary>
    /// Add a chat message to be displayed
    /// </summary>
    /// <param name="message">Message to display</param>
    public void AddChatMessage(string message)
    {
        fadeTime = 10f;
        messages.Add(new ChatMessage(message));
        chatScroll = new Vector2(0, 100000);
    }
    /// <summary>
    /// Trigger a full clear of the chat
    /// </summary>
    public void Clear()
    {
        messages.Clear();
    }

    /// <summary>
    /// Trigger focus on the chat box for when the uer hits enter
    /// </summary>
    private IEnumerator FocusChat()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        openChat = !openChat;
        GUI.FocusControl("ChatInput");
    }

    /// <summary>
    /// Create the wrapped string to display in the chat box
    /// </summary>
    /// <param name="msg">Message to display</param>
    /// <param name="width">Width of the chat box</param>
    /// <returns></returns>
    string wrapString(string msg, int width)
    {
        string[] words = msg.Split(" "[0]);
        string retVal = ""; //returning string 
        string NLstr = "";  //leftover string on new line
        for (int index = 0; index < words.Length; index++)
        {
            string word = words[index].Trim();
            //if word exceeds width
            if (words[index].Length >= width + 2)
            {
                string[] temp = new string[5];
                int i = 0;
                while (words[index].Length > width)
                { //word exceeds width, cut it at widrh
                    temp[i] = words[index].Substring(0, width) + "\n"; //cut the word at width
                    words[index] = words[index].Substring(width);     //keep remaining word
                    i++;
                    if (words[index].Length <= width)
                    { //the balance is smaller than width
                        temp[i] = words[index];
                        NLstr = temp[i];
                    }
                }
                retVal += "\n";
                for (int x = 0; x < i + 1; x++)
                { //loops through temp array
                    retVal = retVal + temp[x];
                }
            }
            else if (index == 0)
            {
                retVal = words[0];
                NLstr = retVal;
            }
            else if (index > 0)
            {
                if (NLstr.Length + words[index].Length <= width)
                {
                    retVal = retVal + " " + words[index];
                    NLstr = NLstr + " " + words[index]; //add the current line length
                }
                else if (NLstr.Length + words[index].Length > width)
                {
                    retVal = retVal + "\n" + words[index];
                    NLstr = words[index]; //reset the line length
                    print("newline! at word " + words[index]);
                }
            }
        }
        return retVal;
    }
}
