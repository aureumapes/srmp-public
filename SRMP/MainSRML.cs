#if SRML
using Newtonsoft.Json;
using SRML;
using SRML.SR;
using SRMultiplayer.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMultiplayer
{
    public class MainSRML : ModEntryPoint
    {
        private static GameObject m_GameObject;

        // Called before GameContext.Awake
        // this is where you want to register stuff (like custom enum values or identifiable id's)
        // and patch anything you want to patch with harmony
        public override void PreLoad()
        {
            base.PreLoad();
        }


        // Called right before PostLoad
        // Used to register stuff that needs lookupdirector access
        public override void Load()
        {
            if (m_GameObject != null) return;

            SRMP.Log("Loading SRMP SRML Version");

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

            Application.runInBackground = true;

            HarmonyPatcher.GetInstance().PatchAll(Assembly.GetExecutingAssembly());
        }


        // Called after GameContext.Start
        // stuff like gamecontext.lookupdirector are available in this step, generally for when you want to access
        // ingame prefabs and the such
        public override void PostLoad()
        {

        }
    }
}
#endif