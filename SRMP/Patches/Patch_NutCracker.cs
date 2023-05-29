using HarmonyLib;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMultiplayer.Patches
{
    //[HarmonyPatch(typeof(Nutcracker))]
    //[HarmonyPatch("OnTriggerEnter")]
    //class Nutcracker_OnTriggerEnter
    //{
    //    static bool Prefix(Nutcracker __instance, Collider col)
    //    {
    //        if (!Globals.IsMultiplayer) return true;

    //        var netActor = col.GetComponent<NetworkActor>();
    //        return (netActor != null && netActor.IsLocal);
    //    }
    //}

    [HarmonyPatch(typeof(Nutcracker))]
    [HarmonyPatch("DoCrack")]
    class Nutcracker_DoCrack
    {
        static bool Prefix(Nutcracker __instance, GameObject toCrack)
        {
            if (!Globals.IsMultiplayer) return true;

            var netActor = toCrack.GetComponent<NetworkActor>();
            if (netActor != null && !netActor.IsLocal)
            {
                __instance.GetComponent<NetworkNutcracker>().Crack(toCrack);
                return false;
            }
            return true;
        }
    }
}