using MonomiPark.SlimeRancher.Regions;
using SRMultiplayer.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SRMultiplayer
{
    public class SRMPConsole : MonoBehaviour
    {
        ConsoleWindow console = new ConsoleWindow();
        ConsoleInput input = new ConsoleInput();

        //
        // Create console window, register callbacks
        //
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            console.Initialize();
            console.SetTitle("Slime Rancher");

            input.OnInputText += OnInputText;

            Application.logMessageReceived += Application_logMessageReceived;

            SRMP.Log("Console Started");
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Warning)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (type == LogType.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.White;

            // We're half way through typing something, so clear this line ..
            if (Console.CursorLeft != 0)
                input.ClearLine();

            Console.WriteLine(condition);
            if (!string.IsNullOrEmpty(stackTrace))
                Console.WriteLine(stackTrace);

            // If we were typing something re-add it.
            input.RedrawInputLine();
        }

        //
        // Text has been entered into the console
        // Run it as a console command
        //
        void OnInputText(string obj)
        {
            var args = obj.Split(' ');
            var cmd = args.FirstOrDefault();
            args = args.Skip(1).ToArray();

            switch (cmd.ToLower())
            {
                case "cheat":
                    {
                        if (args.Length > 0)
                        {
                            if (args[0].Equals("money"))
                            {
                                if (args.Length != 2 || !int.TryParse(args[1], out int money))
                                {
                                    SRMP.Log("Usage: cheat money <amount>");
                                }
                                else
                                {
                                    if(money > 0)
                                        SRSingleton<SceneContext>.Instance.PlayerState.AddCurrency(money);
                                    else
                                        SRSingleton<SceneContext>.Instance.PlayerState.SpendCurrency(money);
                                }
                            }
                            else if(args[0].Equals("enable"))
                            {
                                //TestUI.Instance.cheat = !TestUI.Instance.cheat;
                            }
                            else if (args[0].Equals("keys"))
                            {
                                if (args.Length != 2 || !int.TryParse(args[1], out int value))
                                {
                                    SRMP.Log("Usage: cheat keys <amount>");
                                }
                                else
                                {
                                    SRSingleton<SceneContext>.Instance.PlayerState.model.keys = value;
                                }
                            }
                            else if (args[0].Equals("allgadgets"))
                            {
                                foreach (var data in (Gadget.Id[])Enum.GetValues(typeof(Gadget.Id)))
                                {
                                    SRSingleton<SceneContext>.Instance.GadgetDirector.model.gadgets[data] = int.MaxValue;
                                }
                                foreach (var data in Identifiable.CRAFT_CLASS.Union(Identifiable.PLORT_CLASS))
                                {
                                    SRSingleton<SceneContext>.Instance.GadgetDirector.model.craftMatCounts[data] = int.MaxValue;
                                }
                            }
                            else if (args[0].Equals("spawn"))
                            {
                                if (args.Length == 2)
                                {
                                    if (Enum.TryParse<Identifiable.Id>(args[1].ToUpper(), out Identifiable.Id id))
                                    {
                                        GameObject prefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(id);
                                        if (prefab != null)
                                        {
                                            SRBehaviour.InstantiateActor(prefab, SRSingleton<SceneContext>.Instance.GameModel.player.currRegionSetId, SRSingleton<SceneContext>.Instance.GameModel.player.GetPos() + new Vector3(0, 4, 0), Quaternion.identity, false);
                                        }
                                        else
                                        {
                                            SRMP.Log(id + " can not be spawned");
                                        }
                                    }
                                    else
                                    {
                                        var data = Enum.GetNames(typeof(Identifiable.Id)).Where(n => n.ToLower().Contains(args[1].ToLower()));
                                        SRMP.Log(args[1] + " not found. " + (data.Count() > 0 ? " Did you mean one of these?" : ""));
                                        foreach (var name in data)
                                        {
                                            SRMP.Log(name);
                                        }
                                    }
                                }
                                else if (args.Length == 3 && int.TryParse(args[2], out int amount))
                                {
                                    if (Enum.TryParse<Identifiable.Id>(args[1].ToUpper(), out Identifiable.Id id))
                                    {
                                        GameObject prefab = SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(id);
                                        if (prefab != null)
                                        {
                                            for (int i = 0; i < amount; i++)
                                            {
                                                SRBehaviour.InstantiateActor(prefab, SRSingleton<SceneContext>.Instance.GameModel.player.currRegionSetId, SRSingleton<SceneContext>.Instance.GameModel.player.GetPos() + new Vector3(0, 4, 0), Quaternion.identity, false);
                                            }
                                        }
                                        else
                                        {
                                            SRMP.Log(id + " can not be spawned");
                                        }
                                    }
                                    else
                                    {
                                        var data = Enum.GetNames(typeof(Identifiable.Id)).Where(n => n.ToLower().Contains(args[1].ToLower()));
                                        SRMP.Log(args[1] + " not found. " + (data.Count() > 0 ? " Did you mean one of these?" : ""));
                                        foreach (var name in data)
                                        {
                                            SRMP.Log(name);
                                        }
                                    }
                                }
                                else
                                {
                                    SRMP.Log("Usage: cheat spawn <id> (<amount>)");
                                }
                            }
                        }
                        else
                        {
                            SRMP.Log("Available sub commands:");
                            SRMP.Log("cheat money <amount>");
                            SRMP.Log("cheat keys <amount>");
                            SRMP.Log("cheat spawn <id> (<amount>)");
                            SRMP.Log("cheat allgadgets");
                        }
                    }
                    break;
                case "tp":
                    {
                        if (args.Length == 1)
                        {
                            var player = Globals.Players.Values.FirstOrDefault(p => p.Username.Equals(args[0], StringComparison.CurrentCultureIgnoreCase));
                            if (player != null)
                            {
                                SRSingleton<SceneContext>.Instance.player.transform.position = player.transform.position;
                            }
                            else
                            {
                                SRMP.Log("Player not found");
                            }
                        }
                        else
                        {
                            SRMP.Log("Usage: tp <username>");
                        }
                    }
                    break;
                case "listplayers":
                    {
                        SRMP.Log("Players:");
                        foreach (var player in Globals.Players.Values)
                        {
                            SRMP.Log(player.Username);
                        }
                    }
                    break;
                case "sleep":
                    {
                        if (args.Length == 1)
                        {
                            SRSingleton<SceneContext>.Instance.TimeDirector.FastForwardTo(SRSingleton<SceneContext>.Instance.TimeDirector.HoursFromNow(float.Parse(args[0])));
                        }
                    }
                    break;
            }
        }

        //
        // Update the input every frame
        // This gets new key input and calls the OnInputText callback
        //
        void Update()
        {
            input.Update();
        }

        //
        // It's important to call console.ShutDown in OnDestroy
        // because compiling will error out in the editor if you don't
        // because we redirected output. This sets it back to normal.
        //
        void OnDestroy()
        {
            console.Shutdown();
        }
    }
}