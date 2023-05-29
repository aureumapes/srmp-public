#if Standalone
using HarmonyLib;
using Newtonsoft.Json;
using SRMultiplayer.Networking;
using System.IO;
using System.Reflection;
using UnityCoreMod;
using UnityEngine;

namespace SRMultiplayer
{
    public class MainSaty : IUnityMod
    {
        private static GameObject m_GameObject;

        public void Load()
        {
            if (m_GameObject != null) return;

            SRMP.Log("Loading SRMP Standalone Version");

            if (!Directory.Exists(SRMP.ModDataPath))
            {
                Directory.CreateDirectory(SRMP.ModDataPath);
            }
            if (!File.Exists(Path.Combine(SRMP.ModDataPath, "userdata.json")))
            {
                Globals.UserData = new UserData()
                {
                    UUID = System.Guid.NewGuid(),
                    CheckDLC = true,
                    IgnoredMods = new System.Collections.Generic.List<string>()
                };
                File.WriteAllText(Path.Combine(SRMP.ModDataPath, "userdata.json"), JsonConvert.SerializeObject(Globals.UserData));
                SRMP.Log("Created userdata with UUID " + Globals.UserData.UUID);
            }
            else
            {
                Globals.UserData = JsonConvert.DeserializeObject<UserData>(File.ReadAllText(Path.Combine(SRMP.ModDataPath, "userdata.json")));
                if(Globals.UserData.IgnoredMods == null)
                {
                    Globals.UserData.IgnoredMods = new System.Collections.Generic.List<string>();
                }
                SRMP.Log("Loaded userdata with UUID " + Globals.UserData.UUID);
            }

            string[] args = System.Environment.GetCommandLineArgs();

            m_GameObject = new GameObject("SRMP");
            m_GameObject.AddComponent<SRMP>();
            m_GameObject.AddComponent<NetworkMasterServer>();
            m_GameObject.AddComponent<NetworkClient>();
            m_GameObject.AddComponent<NetworkServer>();
            m_GameObject.AddComponent<MultiplayerUI>();
            m_GameObject.AddComponent<ChatUI>();
            m_GameObject.AddComponent<SRMPConsole>();

            GameObject.DontDestroyOnLoad(m_GameObject);

            Globals.Version = Assembly.GetExecutingAssembly().GetName().Version.Revision;

            var harmony = new Harmony("saty.mod.srmp");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Application.runInBackground = true;
        }

        public void Unload() { }
    }
}
#endif