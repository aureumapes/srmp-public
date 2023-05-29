using UnityEngine;
using System.Collections;
using SRMultiplayer;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;
using SRMultiplayer.Networking;

public class MultiplayerUI : SRSingleton<MultiplayerUI>
{
    private Rect windowRect = new Rect(Screen.width - 300, 20, 300, 500);
    private Vector2 playersScroll = Vector2.zero;
    private string ipaddress = "localhost";
    private string port = "16500";
    private string servercode = "";
    private bool menuOpen;
    private string username;
    private float lastCodeUse;
    private ConnectError error;
    private ConnectHelp help;
    private string errorMessage;

    public enum ConnectError
    {
        None,
        InvalidServerCode,
        ServerCodeTimeout,
        Kicked,
        Message
    }

    public enum ConnectHelp
    {
        None,
        ServerCode,
        Hosting,
    }

    public override void Awake()
    {
        base.Awake();

        Globals.Username = PlayerPrefs.GetString("SRMP_Username", "");
        ipaddress = PlayerPrefs.GetString("SRMP_IP", "localhost");
        port = PlayerPrefs.GetString("SRMP_Port", "16500");

        menuOpen = true;
        username = Globals.Username;
    }

    private void Update()
    {
        if(lastCodeUse > 0f)
        {
            var prevTime = lastCodeUse;
            lastCodeUse -= Time.deltaTime;
            if(prevTime > 0f && lastCodeUse <= 0f)
            {
                error = ConnectError.ServerCodeTimeout;
            }
        }

        if(Input.GetKeyDown(KeyCode.F4))
        {
            menuOpen = !menuOpen;
        }
    }

    private void OnGUI()
    {
        if (!menuOpen || (!Levels.isMainMenu() && !Globals.IsMultiplayer && Globals.PauseState != PauseState.Pause)) return;
        if (Globals.IsMultiplayer && Globals.PauseState != PauseState.Pause) return;

        if (SceneManager.GetActiveScene().buildIndex >= 2)
        {
            windowRect = GUI.Window(1, windowRect, MultiplayerWindow, "SRMP v" + Globals.Version);
        }
    }

    private void MultiplayerWindow(int id)
    {
        if (id != 1) return;

        GUILayout.Label("You can close this menu with F4");
        GUILayout.Space(20);

        if(string.IsNullOrWhiteSpace(Globals.Username))
        {
            UsernameGUI();
        }
        else
        {
            if(Globals.IsServer)
            {
                ServerGUI();
            }
            else if(Globals.IsClient)
            {
                ClientGUI();
            }
            else
            {
                if (error != ConnectError.None)
                {
                    ErrorGUI();
                }
                else if(help != ConnectHelp.None)
                {
                    HelpGUI();
                }
                else
                {
                    if(lastCodeUse > 0f)
                    {
                        GUILayout.Label("Trying to connect with server code...");
                    }
                    else if (Levels.isMainMenu())
                    {
                        ConnectGUI();
                    }
                    else
                    {
                        HostGUI();
                    }
                }
            }
        }

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }

    private void ServerGUI()
    {
        GUILayout.Label("You are the server");
        GUILayout.Space(20);

        GUILayout.Label("Server Code: " + Globals.ServerCode);
        GUILayout.Label("Players");
        playersScroll = GUILayout.BeginScrollView(playersScroll, GUI.skin.box);
        foreach (var player in Globals.Players.Values)
        {
            if (player.IsLocal) continue;

            GUILayout.BeginHorizontal();
            GUILayout.Label(player.Username);
            if(GUILayout.Button("Kick"))
            {
                player.Connection.Disconnect("kicked");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    private void ClientGUI()
    {
        GUILayout.Label("You are a client");
        GUILayout.Space(20);

        GUILayout.Label("Players");
        playersScroll = GUILayout.BeginScrollView(playersScroll, GUI.skin.box);
        foreach (var player in Globals.Players.Values)
        {
            if (player.IsLocal) continue;

            GUILayout.BeginHorizontal();
            GUILayout.Label(player.Username);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    private void ConnectGUI()
    {
        GUILayout.Label("Username: " + Globals.Username);
        if (GUILayout.Button("Change Username"))
        {
            Globals.Username = "";
            return;
        }
        GUILayout.Space(20);
        if(GUILayout.Button("How do I host a game?"))
        {
            help = ConnectHelp.Hosting;
        }
        GUILayout.Space(20);

        if (SRSingleton<NetworkMasterServer>.Instance.Status == NetworkMasterServer.ConnectionStatus.Connected)
        {
            GUILayout.Label("Join with server code:");
            GUILayout.BeginHorizontal();
            servercode = GUILayout.TextField(servercode, 5, GUILayout.Width(80));
            servercode = servercode.Replace(" ", "").ToUpper();
            if (GUILayout.Button("Join"))
            {
                lastCodeUse = 5;
                SRSingleton<NetworkMasterServer>.Instance.JoinServer(servercode);
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("Server codes currently not available");
        }

        GUILayout.Space(20);

        GUILayout.Label("Join with IP Address:");
        GUILayout.BeginHorizontal();
        GUILayout.Label("IP Address", GUILayout.Width(80));
        ipaddress = GUILayout.TextField(ipaddress);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Port", GUILayout.Width(80));
        port = GUILayout.TextField(port);
        GUILayout.EndHorizontal();
        if (string.IsNullOrWhiteSpace(ipaddress))
        {
            GUILayout.Label("IPAddress invalid");
        }
        else if (!int.TryParse(port, out int numport) || numport < 1000 || numport > 65000)
        {
            GUILayout.Label("Invalid port, should be between 1000 and 65000");
        }
        else
        {
            if (GUILayout.Button("Connect"))
            {
                SRSingleton<NetworkClient>.Instance.Connect(ipaddress, numport, Globals.Username);
                SaveSettings();
            }
        }

        GUILayout.Space(20);
        GUILayout.Label("Games in the same Network:");
        if (GUILayout.Button("Refresh"))
        {
            SRSingleton<NetworkClient>.Instance.SendDiscoverMessage();
        }
        if (SRSingleton<NetworkClient>.Instance.LocalGames.Count == 0)
        {
            GUILayout.Label("No games found");
        }
        foreach (var game in SRSingleton<NetworkClient>.Instance.LocalGames)
        {
            if (GUILayout.Button(game.Name))
            {
                SRSingleton<NetworkClient>.Instance.Connect(game.IP, game.Port, Globals.Username);
            }
        }
    }

    private void HostGUI()
    {
        GUILayout.Label("Username: " + Globals.Username);
        if (GUILayout.Button("Change Username"))
        {
            Globals.Username = "";
            return;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Port", GUILayout.Width(80));
        port = GUILayout.TextField(port);
        GUILayout.EndHorizontal();

        if (!int.TryParse(port, out int numport) || numport < 1000 || numport > 65000)
        {
            GUILayout.Label("Invalid port");
        }
        else
        {
            if (GUILayout.Button("Host"))
            {
                NetworkServer.Instance.StartServer(numport);
                SaveSettings();
            }
        }
    }

    private void HelpGUI()
    {
        switch(help)
        {
            case ConnectHelp.ServerCode:
                {

                }
                break;
            case ConnectHelp.Hosting:
                {
                    GUILayout.Label("You can host a game by loading any Singleplayer save.");
                    GUILayout.Label("When the game is loaded, pause it and the HostUI should appear.");
                    GUILayout.Label("(Please make a backup of your Singleplayer save to avoid data loss on crashes or error)");

                    if (GUILayout.Button("Okay"))
                    {
                        help = ConnectHelp.None;
                    }
                }
                break;
        }
    }

    private void ErrorGUI()
    {
        switch(error)
        {
            case ConnectError.InvalidServerCode:
                {
                    GUILayout.Label("There is no server with this server code.");

                    if (GUILayout.Button("Okay"))
                    {
                        error = ConnectError.None;
                    }
                }
                break;
            case ConnectError.Kicked:
                {
                    GUILayout.Label("You got kicked from the game");

                    if (GUILayout.Button("Okay"))
                    {
                        error = ConnectError.None;
                    }
                }
                break;
            case ConnectError.Message:
                {
                    GUILayout.Label("Connection Error:");
                    GUILayout.Label(errorMessage);

                    if (GUILayout.Button("Okay"))
                    {
                        error = ConnectError.None;
                    }
                }
                break;
            case ConnectError.ServerCodeTimeout:
                {
                    GUILayout.Label("Connection via server code timed out.");
                    GUILayout.Label("Server codes do not have a 100% success chance.");
                    GUILayout.Label("You can try again, but if it keeps failing, you may have to port forward.");
                    GUILayout.Label("If you can't port forward, we recommend a service like 'Hamachi' or 'Radmin'");
                    GUILayout.Label("Please note: Hamachi and Radmin degrade the experience by quite a lot. Port forwarding is always better.");
                    GUILayout.Label("(We can not offer support for port forwarding)");

                    if (GUILayout.Button("Okay"))
                    {
                        error = ConnectError.None;
                    }
                }
                break;
        }
    }

    private void UsernameGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Username", GUILayout.Width(60));
        username = GUILayout.TextField(username, 30);
        GUILayout.EndHorizontal();

        if (string.IsNullOrWhiteSpace(username) || !new Regex(@"^[a-zA-Z_][\w]*$").IsMatch(username))
        {
            GUILayout.Label("Invalid username");
        }
        else
        {
            if (GUILayout.Button("Save Username"))
            {
                Globals.Username = username;
                SaveSettings();
                SRSingleton<NetworkMasterServer>.Instance.UpdateName(Globals.Username);
            }
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetString("SRMP_Username", Globals.Username);
        PlayerPrefs.SetString("SRMP_IP", ipaddress);
        PlayerPrefs.GetString("SRMP_Port", port);
    }

    public void ConnectResponse(ConnectError connectError, string message = "")
    {
        lastCodeUse = 0f;
        error = connectError;
        errorMessage = message;
    }
}
