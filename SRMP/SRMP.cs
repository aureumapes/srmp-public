using MonomiPark.SlimeRancher.Persist;
using MonomiPark.SlimeRancher.Regions;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRMultiplayer
{
    public class SRMP : SRSingleton<SRMP>
    {
        public static string ModDataPath { get { return Path.Combine(Application.dataPath, "..", "SRMP"); } }

        private float m_LastTimeSync;

        public override void Awake()
        {
            base.Awake();

            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            Application.logMessageReceived += Application_logMessageReceived;

            var myLoadedAssetBundle = AssetBundle.LoadFromMemory(Utils.ExtractResource("SRMultiplayer.srmultiplayer.dat"));
            if (myLoadedAssetBundle == null)
            {
                SRMP.Log("Failed to load AssetBundle!");
                return;
            }
            Globals.BeatrixController = myLoadedAssetBundle.LoadAsset<RuntimeAnimatorController>("Controller");
            Globals.IngameMultiplayerMenuPrefab = myLoadedAssetBundle.LoadAsset<GameObject>("IngameMultiplayerMenu");
            Globals.MainMultiplayerMenuPrefab = myLoadedAssetBundle.LoadAsset<GameObject>("MainMultiplayerMenu");
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            if(type == LogType.Error || type == LogType.Exception)
            {
                SRMP.Log(condition);
                if (!string.IsNullOrEmpty(stackTrace))
                    SRMP.Log(stackTrace);
            }
        }

        private void Start()
        {
            //var menuObj = Instantiate(Globals.MainMultiplayerMenuPrefab, null, false);
            //menuObj.AddComponent<NetworkClientUI>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            NetworkClient.Instance.Disconnect();
            NetworkServer.Instance.Disconnect();
        }

        private void OnApplicationQuit()
        {
            NetworkClient.Instance.Disconnect();
            NetworkServer.Instance.Disconnect();
        }

        private void Update()
        {
            if(Globals.GameLoaded)
            {
                if(Globals.IsServer)
                {
                    if(Time.time - m_LastTimeSync > 30)
                    {
                        m_LastTimeSync = Time.time;
                        new PacketWorldTime()
                        {
                            Time = SRSingleton<SceneContext>.Instance.TimeDirector.WorldTime()
                        }.Send();
                    }
                }

                //if(Time.time - m_LastActorTime > 0.5f)
                //{
                //    foreach(var actor in Globals.Actors.Values.ToList())
                //    {
                //        if(actor.IsLocal && !actor.gameObject.activeInHierarchy)
                //        {
                //            actor.DropOwnership();
                //            //SRMP.Log($"Dropping actor {actor.name} ({actor.ID}) as it's unloaded");
                //        }
                //    }
                //}
            }
        }

        private void SceneManager_activeSceneChanged(Scene from, Scene to)
        {
            if (to.buildIndex == 2) OnMainMenuLoaded();
            if (to.buildIndex == 3) OnGameLoaded();
        }

        private void OnMainMenuLoaded()
        {
            //var menuObj = Instantiate(Globals.MainMultiplayerMenuPrefab, null, false);
            //menuObj.AddComponent<NetworkClientUI>();

            Globals.LocalID = 0;
            Globals.DisableAchievements = false;
            Globals.GameLoaded = false;
            Globals.ClientLoaded = false;
            Globals.LocalPlayer = null;
            Globals.Audios.Clear();
            Globals.Actors.Clear();
            Globals.Regions.Clear();
            Globals.LandPlots.Clear();
            Globals.SpawnResources.Clear();
            Globals.FXPrefabs.Clear();
            Globals.AccessDoors.Clear();
            Globals.Gordos.Clear();
            Globals.PuzzleSlots.Clear();
            Globals.Switches.Clear();
            Globals.GadgetSites.Clear();
            Globals.PacketSize.Clear();
            Globals.Spawners.Clear();
            Globals.TreasurePods.Clear();
            Globals.ExchangeAcceptors.Clear();
            Globals.FireColumns.Clear();
            Globals.Kookadobas.Clear();
            Globals.LemonTrees.Clear();
            Globals.Nutcrackers.Clear();
            Globals.RaceTriggers.Clear();
            NetworkAmmo.All.Clear();
            foreach (var player in Globals.Players.Values.ToList())
            {
                if(player != null && player.gameObject != null)
                {
                    Destroy(player.gameObject);
                }
            }
            Globals.Players.Clear();

            ChatUI.Instance.Clear();
        }

        private void OnGameLoaded()
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var ranchui = Resources.FindObjectsOfTypeAll<RanchHouseUI>().FirstOrDefault();
            if (ranchui != null)
            {
                Globals.BeatrixModel = Instantiate(ranchui.beatrixPrefab.transform.GetChild(1).GetChild(0).gameObject);
                Globals.BeatrixModel.transform.localScale *= 0.75f;
                Globals.BeatrixModel.SetActive(false);

                Utils.SetLayer(Globals.BeatrixModel, 0);
            }

            foreach (var audio in Resources.FindObjectsOfTypeAll<SECTR_AudioCue>())
            {
                Globals.Audios.Add(audio.name, audio);
            }

            var splashOnTrigger = GameObject.FindObjectOfType<SplashOnTrigger>();
            Globals.FXPrefabs.Add(splashOnTrigger.playerSplashFX.name, splashOnTrigger.playerSplashFX);
            Globals.FXPrefabs.Add(splashOnTrigger.splashFX.name, splashOnTrigger.splashFX);

            foreach (var region in Resources.FindObjectsOfTypeAll<Region>())
            {
                var netRegion = region.gameObject.AddComponent<NetworkRegion>();
                netRegion.ID = region.gameObject.GetGameObjectPath().GetHashCode();
                netRegion.Region = region;
                netRegion.FastForwarder = region.gameObject.GetComponent<RanchCellFastForwarder>();

                Globals.Regions.Add(netRegion.ID, netRegion);

                foreach(var landPlotLocation in region.gameObject.GetComponentsInChildren<LandPlotLocation>(true))
                {
                    var netLandplot = landPlotLocation.gameObject.GetOrAddComponent<NetworkLandplot>();
                    netLandplot.Plot = landPlotLocation.GetComponentInChildren<LandPlot>(true);
                    netLandplot.Location = landPlotLocation;
                    netLandplot.Region = netRegion;

                    Globals.LandPlots.Add(netLandplot.Location.id, netLandplot);
                }

                foreach (var accessDoor in region.gameObject.GetComponentsInChildren<AccessDoor>(true))
                {
                    var netAccessDoor = accessDoor.gameObject.GetOrAddComponent<NetworkAccessDoor>();
                    netAccessDoor.Door = accessDoor;
                    netAccessDoor.Region = netRegion;

                    Globals.AccessDoors.Add(accessDoor.id, netAccessDoor);
                }

                foreach (var gordo in region.gameObject.GetComponentsInChildren<GordoEat>(true))
                {
                    var netGordo = gordo.gameObject.GetOrAddComponent<NetworkGordo>();
                    netGordo.Gordo = gordo;
                    netGordo.Region = netRegion;

                    Globals.Gordos.Add(netGordo.ID, netGordo);
                }

                foreach (var puzzleSlot in region.gameObject.GetComponentsInChildren<PuzzleSlot>(true))
                {
                    var netPuzzleSlot = puzzleSlot.gameObject.GetOrAddComponent<NetworkPuzzleSlot>();
                    netPuzzleSlot.Slot = puzzleSlot;
                    netPuzzleSlot.Region = netRegion;

                    Globals.PuzzleSlots.Add(puzzleSlot.id, netPuzzleSlot);
                }

                foreach (var masterSwitch in region.gameObject.GetComponentsInChildren<WorldStateMasterSwitch>(true))
                {
                    var netSwitch = masterSwitch.gameObject.GetOrAddComponent<NetworkWorldStateMasterSwitch>();
                    netSwitch.Switch = masterSwitch;
                    netSwitch.Region = netRegion;

                    Globals.Switches.Add(masterSwitch.id, netSwitch);
                }

                foreach (var gadgetSite in region.gameObject.GetComponentsInChildren<GadgetSite>(true))
                {
                    var netGadgetSite = gadgetSite.gameObject.GetOrAddComponent<NetworkGadgetSite>();
                    netGadgetSite.Site = gadgetSite;
                    netGadgetSite.Region = netRegion;

                    Globals.GadgetSites.Add(gadgetSite.id, netGadgetSite);
                }

                foreach (var treaturePod in region.gameObject.GetComponentsInChildren<TreasurePod>(true))
                {
                    var netTreasurePod = treaturePod.gameObject.GetOrAddComponent<NetworkTreasurePod>();
                    netTreasurePod.Pod = treaturePod;
                    netTreasurePod.Region = netRegion;

                    Globals.TreasurePods.Add(treaturePod.id, netTreasurePod);
                }

                foreach (var spawner in region.gameObject.GetComponentsInChildren<DirectedActorSpawner>(true))
                {
                    var netSpawner = spawner.gameObject.GetOrAddComponent<NetworkDirectedActorSpawner>();
                    netSpawner.ID = spawner.gameObject.GetGameObjectPath().GetHashCode();
                    netSpawner.Spawner = spawner;
                    netSpawner.Region = netRegion;

                    Globals.Spawners.Add(netSpawner.ID, netSpawner);
                }

                foreach (var exchangeAcceptor in region.gameObject.GetComponentsInChildren<ExchangeAcceptor>(true))
                {
                    var netAcceptor = exchangeAcceptor.gameObject.GetOrAddComponent<NetworkExchangeAcceptor>();
                    netAcceptor.ID = exchangeAcceptor.gameObject.GetGameObjectPath().GetHashCode();
                    netAcceptor.Acceptor = exchangeAcceptor;
                    netAcceptor.Region = netRegion;

                    Globals.ExchangeAcceptors.Add(netAcceptor.ID, netAcceptor);
                }

                foreach (var fireColumn in region.gameObject.GetComponentsInChildren<FireColumn>(true))
                {
                    var netColumn = fireColumn.gameObject.GetOrAddComponent<NetworkFireColumn>();
                    netColumn.ID = fireColumn.gameObject.GetGameObjectPath().GetHashCode();
                    netColumn.Column = fireColumn;
                    netColumn.Region = netRegion;

                    Globals.FireColumns.Add(netColumn.ID, netColumn);
                }

                foreach (var kookadobaPatchNode in region.gameObject.GetComponentsInChildren<KookadobaPatchNode>(true))
                {
                    var netNode = kookadobaPatchNode.gameObject.GetOrAddComponent<NetworkKookadobaPatchNode>();
                    netNode.ID = kookadobaPatchNode.gameObject.GetGameObjectPath().GetHashCode();
                    netNode.Node = kookadobaPatchNode;
                    netNode.Region = netRegion;

                    Globals.Kookadobas.Add(netNode.ID, netNode);
                }

                foreach (var nutcracker in region.gameObject.GetComponentsInChildren<Nutcracker>(true))
                {
                    var netCracker = nutcracker.gameObject.GetOrAddComponent<NetworkNutcracker>();
                    netCracker.ID = nutcracker.gameObject.GetGameObjectPath().GetHashCode();
                    netCracker.Cracker = nutcracker;

                    Globals.Nutcrackers.Add(netCracker.ID, netCracker);
                }

                foreach (var trigger in region.gameObject.GetComponentsInChildren<QuicksilverAmmoReplacer>(true))
                {
                    var netTrigger = trigger.gameObject.GetOrAddComponent<NetworkRaceTrigger>();
                    netTrigger.ID = trigger.gameObject.GetGameObjectPath().GetHashCode();
                    netTrigger.Ammo = trigger;

                    Globals.RaceTriggers.Add(netTrigger.ID, netTrigger);
                }

                foreach (var trigger in region.gameObject.GetComponentsInChildren<QuicksilverEnergyCheckpoint>(true))
                {
                    var netTrigger = trigger.gameObject.GetOrAddComponent<NetworkRaceTrigger>();
                    netTrigger.ID = trigger.gameObject.GetGameObjectPath().GetHashCode();
                    netTrigger.Checkpoint = trigger;

                    Globals.RaceTriggers.Add(netTrigger.ID, netTrigger);
                }

                foreach (var trigger in region.gameObject.GetComponentsInChildren<QuicksilverEnergyReplacer>(true))
                {
                    var netTrigger = trigger.gameObject.GetOrAddComponent<NetworkRaceTrigger>();
                    netTrigger.ID = trigger.gameObject.GetGameObjectPath().GetHashCode();
                    netTrigger.Energy = trigger;

                    Globals.RaceTriggers.Add(netTrigger.ID, netTrigger);
                }

                foreach (var spawnResource in region.gameObject.GetComponentsInChildren<SpawnResource>(true))
                {
                    var netSpawnResource = spawnResource.gameObject.GetOrAddComponent<NetworkSpawnResource>();
                    netSpawnResource.ID = spawnResource.gameObject.GetGameObjectPath().GetHashCode();
                    netSpawnResource.SpawnResource = spawnResource;
                    netSpawnResource.Region = netRegion;
                    netSpawnResource.LandPlot = spawnResource.GetComponentInParent<NetworkLandplot>(true);

                    if (!Globals.SpawnResources.ContainsKey(netSpawnResource.ID))
                    {
                        Globals.SpawnResources.Add(netSpawnResource.ID, netSpawnResource);
                    }
                }
            }

            if (Globals.IsClient)
            {
                Globals.DisableAchievements = true;
                Globals.LocalPlayer.transform.SetParent(SRSingleton<SceneContext>.Instance.Player.transform, false);
                Globals.LocalPlayer.HasLoaded = true;
                foreach (var player in Globals.Players.Values.ToList())
                {
                    if (player.HasLoaded)
                    {
                        player.Spawn();
                    }
                }
                new PacketPlayerLoaded().Send();
            }
            else
            {
                //var hostMenuObj = Instantiate(Globals.IngameMultiplayerMenuPrefab, SRSingleton<PauseMenu>.Instance.transform, true);
                //hostMenuObj.AddComponent<NetworkHostUI>();
                //hostMenuObj.SetActive(false);
            }

            Globals.PauseState = PauseState.Playing;
            Globals.GameLoaded = true;

            stopwatch.Stop();
            SRMP.Log($"Loaded the game in {stopwatch.ElapsedMilliseconds}ms");
        }

        private static FileStream m_LogFileStream;
        private static StreamWriter m_LogWriter;
        public static void Log(string msg, string prefix = null)
        {
            if(m_LogFileStream == null)
            {
                string n = string.Format("log-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
                Directory.CreateDirectory(Path.Combine(ModDataPath, "Logs"));
                m_LogFileStream = File.Create(Path.Combine(ModDataPath, "Logs", n));
                m_LogWriter = new StreamWriter(m_LogFileStream);
            }

            string txt = $"[{DateTime.Now.ToString("HH:mm:ss")}]{(prefix != null ? "[" + prefix + "]" : "")} {msg}";
            Debug.Log("[SRMP]" + txt);
            m_LogWriter.WriteLine(txt);
            m_LogWriter.Flush();
        }
    }
}
