using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Persist;
using SRMultiplayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(AutoSaveDirector))]
    [HarmonyPatch("SaveGame")]
    class AutoSaveDirector_SaveGame
    {
        static bool Prefix()
        {
            if(Globals.IsServer)
            {
                foreach(var player in Globals.Players.Values)
                {
                    if(player.UUID != null && player.UUID != Guid.Empty)
                    {
                        player.Save();
                    }
                }
                return true;
            }
            else if(Globals.ClientLoaded && !string.IsNullOrWhiteSpace(Globals.CurrentGameName))
            {
                try
                {
                    using (FileStream file = new FileStream(Path.Combine(SRMP.ModDataPath, Globals.CurrentGameName + ".player"), FileMode.Create))
                    {
                        using (BinaryWriter writer = new BinaryWriter(file))
                        {
                            var ammo = SRSingleton<SceneContext>.Instance.PlayerState.model.ammoDict;

                            Debug.Log($"Saving {Path.Combine(SRMP.ModDataPath, Globals.CurrentGameName + ".player")}");
                            writer.Write(ammo.Count);
                            foreach(var state in ammo.Keys)
                            {
                                writer.Write((byte)state);
                                writer.Write(ammo[state].usableSlots);
                                writer.Write(ammo[state].slots.Length);
                                for (int i = 0; i < ammo[state].slots.Length; i++)
                                {
                                    writer.Write(ammo[state].slots[i] != null);
                                    if(ammo[state].slots[i] != null)
                                    {
                                        writer.Write((ushort)ammo[state].slots[i].id);
                                        writer.Write(ammo[state].slots[i].count);
                                        writer.Write(ammo[state].slots[i].emotions != null);
                                        if(ammo[state].slots[i].emotions != null)
                                        {
                                            writer.Write(ammo[state].slots[i].emotions.Count);
                                            foreach(var emotion in ammo[state].slots[i].emotions)
                                            {
                                                writer.Write((ushort)emotion.Key);
                                                writer.Write(emotion.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.Log($"Couldn't save playerdata for {Globals.CurrentGameName}: {ex.Message}");
                }
            }
            return !Globals.IsClient;
        }
    }
}