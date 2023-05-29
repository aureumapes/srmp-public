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

    private void Update()
    {
        if (!Globals.IsMultiplayer)
        {
            openChat = false;
            message = "";
            return;
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            StartCoroutine(FocusChat());
        }
    }

    private void OnGUI()
    {
        if (!Globals.IsMultiplayer) return;

        if (openChat)
        {
            GUILayout.BeginArea(new Rect(20, Screen.height / 2, 500, 300), GUI.skin.box);
            chatScroll = GUILayout.BeginScrollView(chatScroll);
            var skin = GUI.skin.box;
            skin.wordWrap = true;
            skin.alignment = TextAnchor.MiddleLeft;
            foreach (var msg in messages)
            {
                GUILayout.Label(wrapString(msg.Text, 490), skin, GUILayout.MaxWidth(490));
            }
            GUILayout.EndScrollView();
            GUI.SetNextControlName("ChatInput");
            message = GUILayout.TextField(message);
            GUILayout.EndArea();

            GUI.FocusControl("ChatInput");

            Event e = Event.current;
            if (e.rawType == EventType.KeyUp && e.keyCode == KeyCode.Return)
            {
                openChat = !openChat;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (Globals.IsServer)
                    {
                        AddChatMessage(Globals.Username + ": " + message);
                        new PacketPlayerChat()
                        {
                            message = Globals.Username + ": " + message
                        }.Send();
                    }
                    else
                    {
                        new PacketPlayerChat()
                        {
                            message = message
                        }.Send();
                    }
                    message = "";
                }
            }
        }
        else
        {
            GUILayout.BeginArea(new Rect(20, Screen.height / 2, 500, 300));
            chatScroll = GUILayout.BeginScrollView(chatScroll);
            var skin = GUI.skin.box;
            skin.wordWrap = true;
            skin.alignment = TextAnchor.MiddleLeft;
            foreach (var msg in messages)
            {
                if (msg.FadeTime > 0f)
                {
                    msg.FadeTime -= Time.deltaTime;
                    var c = GUI.color;
                    c.a = (msg.FadeTime / 5f);
                    GUI.color = c;
                    GUILayout.Label(wrapString(msg.Text, 490), skin, GUILayout.MaxWidth(490));
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }

    public void AddChatMessage(string message)
    {
        fadeTime = 10f;
        messages.Add(new ChatMessage(message));
        chatScroll = new Vector2(0, 100000);
    }

    public void Clear()
    {
        messages.Clear();
    }

    private IEnumerator FocusChat()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        openChat = !openChat;
        GUI.FocusControl("ChatInput");
    }

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
