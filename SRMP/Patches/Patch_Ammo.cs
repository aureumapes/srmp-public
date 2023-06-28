using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRMultiplayer;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("DecrementSelectedAmmo")]
    class Ammo_DecrementSelectedAmmo
    {
        static void Postfix(Ammo __instance, int amount)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__instance is NetworkAmmo)
            {
                new PacketLandPlotSiloAmmoRemove()
                {
                    ID = ((NetworkAmmo)__instance).ID,
                    Type = (byte)((NetworkAmmo)__instance).Silo.type,
                    Slot = __instance.selectedAmmoIdx,
                    Count = amount
                }.Send();
            }
            else if(__instance is NetworkPlayerAmmo)
            {
                SRMP.Log($"DecrementSelectedAmmo {((NetworkPlayerAmmo)__instance).ID} {amount}", "PLAYERAMMO");
            }
        }
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("Clear", new Type[] { typeof(int) })]
    class Ammo_Clear
    {
        static void Postfix(Ammo __instance, int index)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__instance is NetworkAmmo)
            {
                new PacketLandPlotSiloAmmoClear()
                {
                    ID = ((NetworkAmmo)__instance).ID,
                    Type = (byte)((NetworkAmmo)__instance).Silo.type,
                    Slot = index
                }.Send();
            }
            else if (__instance is NetworkDroneAmmo)
            {
                new PacketDroneAmmoClear()
                {
                    ID = ((NetworkDroneAmmo)__instance).Drone.droneModel.siteId
                }.Send();
            }
            else if (__instance is NetworkPlayerAmmo)
            {
                SRMP.Log($"Clear {((NetworkPlayerAmmo)__instance).ID} {index}", "PLAYERAMMO");
            }
        }
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("ReplaceWithQuicksilverAmmo")]
    class Ammo_ReplaceWithQuicksilverAmmo
    {
        static void Postfix(Ammo __instance, Identifiable.Id id, int count, ref bool __result)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__result && __instance is NetworkPlayerAmmo)
            {
                SRMP.Log($"ReplaceWithQuicksilverAmmo {((NetworkPlayerAmmo)__instance).ID} {id} {count}", "PLAYERAMMO");
            }
        }
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("SetAmmoSlot")]
    class Ammo_SetAmmoSlot
    {
        static void Postfix(Ammo __instance, int idx, ref bool __result)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__result && __instance is NetworkPlayerAmmo)
            {
                SRMP.Log($"SetAmmoSlot {((NetworkPlayerAmmo)__instance).ID} {idx}", "PLAYERAMMO");
            }
        }
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("Replace", new Type[] { typeof(Identifiable.Id), typeof(Identifiable.Id) })]
    class Ammo_Replace
    {
        static void Postfix(Ammo __instance, Identifiable.Id previous, Identifiable.Id next, ref bool __result)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__result && __instance is NetworkPlayerAmmo)
            {
                SRMP.Log($"Replace {((NetworkPlayerAmmo)__instance).ID} {previous} {next}", "PLAYERAMMO");
            }
        }
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("Decrement", new Type[] { typeof(int), typeof(int) })]
    class Ammo_Decrement
    {
        static void Postfix(Ammo __instance, int index, int count)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__instance is NetworkAmmo)
            {
                new PacketLandPlotSiloAmmoRemove()
                {
                    ID = ((NetworkAmmo)__instance).ID,
                    Type = (byte)((NetworkAmmo)__instance).Silo.type,
                    Slot = index,
                    Count = count
                }.Send();
            }
            else if (__instance is NetworkDroneAmmo)
            {
                new PacketDroneAmmoRemove()
                {
                    ID = ((NetworkDroneAmmo)__instance).Drone.droneModel.siteId
                }.Send();
            }
            else if (__instance is NetworkPlayerAmmo)
            {
                SRMP.Log($"Decrement {((NetworkPlayerAmmo)__instance).ID} {index} {count}", "PLAYERAMMO");
            }
        }
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("MaybeAddToSpecificSlot", new Type[] { typeof(Identifiable.Id), typeof(Identifiable), typeof(int), typeof(int), typeof(bool) })]
    class Ammo_MaybeAddToSpecificSlot
    {
        static void Postfix(Ammo __instance, ref bool __result, Identifiable.Id id, Identifiable identifiable, int slotIdx, int count, bool overflow)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__instance is NetworkAmmo && __result)
            {
                new PacketLandPlotSiloAmmoAdd()
                {
                    ID = ((NetworkAmmo)__instance).ID,
                    Type = (byte)((NetworkAmmo)__instance).Silo.type,
                    Ident = (ushort)id,
                    Slot = slotIdx,
                    Count = count,
                    Overflow = overflow,
                    Emotions = __instance.Slots[slotIdx].emotions
                }.Send();
            }
            else if(__instance is NetworkDroneAmmo && __result)
            {
                new PacketDroneAmmoAdd()
                {
                    ID = ((NetworkDroneAmmo)__instance).Drone.droneModel.siteId,
                    Ident = (ushort)id
                }.Send();
            }
        }
    }

    [HarmonyPatch(typeof(Ammo))]
    [HarmonyPatch("MaybeAddToSlot")]
    class Ammo_MaybeAddToSlot
    {
        static bool Prefix(Ammo __instance, ref bool __result, Identifiable.Id id, Identifiable identifiable)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return true;

            bool flag = id == Identifiable.Id.MAGIC_WATER_LIQUID;
            if (flag)
            {
                id = Identifiable.Id.WATER_LIQUID;
            }
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < __instance.ammoModel.usableSlots; i++)
            {
                if (__instance.Slots[i] != null && __instance.Slots[i].id == id)
                {
                    int slotMaxCount = __instance.GetSlotMaxCount(id, i);
                    if (flag)
                    {
                        if (__instance is NetworkAmmo)
                        {
                            new PacketLandPlotSiloAmmoAdd()
                            {
                                ID = ((NetworkAmmo)__instance).ID,
                                Type = (byte)((NetworkAmmo)__instance).Silo.type,
                                Ident = (ushort)id,
                                Slot = i,
                                Count = slotMaxCount - __instance.Slots[i].count,
                                Overflow = false,
                                Emotions = __instance.Slots[i].emotions
                            }.Send();
                        }
                        else if(__instance is NetworkPlayerAmmo)
                        {
                            SRMP.Log($"MaybeAddToSlot1 {((NetworkPlayerAmmo)__instance).ID} {id} {i}", "PLAYERAMMO");
                        }

                        __instance.Slots[i].count = slotMaxCount;
                        __instance.waterIsMagicUntil = __instance.timeDir.HoursFromNow(0.5f);
                    }
                    else if (__instance.Slots[i].count >= slotMaxCount)
                    {
                        flag3 = true;
                    }
                    else
                    {
                        int prev = __instance.Slots[i].count;
                        __instance.Slots[i].count = Mathf.Min(slotMaxCount, __instance.Slots[i].count + __instance.GetAmountFilledPerVac(id, i));
                        SlimeEmotions slimeEmotions = (identifiable == null) ? null : identifiable.GetComponent<SlimeEmotions>();
                        if (slimeEmotions != null)
                        {
                            __instance.Slots[i].AverageIn(slimeEmotions);
                        }

                        if (__instance is NetworkAmmo)
                        {
                            new PacketLandPlotSiloAmmoAdd()
                            {
                                ID = ((NetworkAmmo)__instance).ID,
                                Type = (byte)((NetworkAmmo)__instance).Silo.type,
                                Ident = (ushort)id,
                                Slot = i,
                                Count = __instance.Slots[i].count - prev,
                                Overflow = false,
                                Emotions = __instance.Slots[i].emotions
                            }.Send();
                        }
                        else if (__instance is NetworkPlayerAmmo)
                        {
                            SRMP.Log($"MaybeAddToSlot2 {((NetworkPlayerAmmo)__instance).ID} {id} {i}", "PLAYERAMMO");
                        }
                    }
                    flag2 = true;
                    break;
                }
            }
            if (!flag2)
            {
                int num = 0;
                while (num < __instance.ammoModel.usableSlots && !flag2)
                {
                    if ((__instance.slotPreds[num] == null || __instance.slotPreds[num](id)) && __instance.Slots[num] == null && __instance.potentialAmmo.Contains(id))
                    {
                        SlimeEmotions slimeEmotions2 = (identifiable == null) ? null : identifiable.GetComponent<SlimeEmotions>();
                        if (flag)
                        {
                            __instance.Slots[num] = new Ammo.Slot(id, __instance.GetSlotMaxCount(id, num));
                            __instance.waterIsMagicUntil = __instance.timeDir.HoursFromNow(0.5f);
                        }
                        else
                        {
                            __instance.Slots[num] = new Ammo.Slot(id, __instance.GetAmountFilledPerVac(id, num));
                        }
                        if (slimeEmotions2 != null)
                        {
                            __instance.Slots[num].AverageIn(slimeEmotions2);
                        }
                        if (__instance is NetworkAmmo)
                        {
                            new PacketLandPlotSiloAmmoAdd()
                            {
                                ID = ((NetworkAmmo)__instance).ID,
                                Type = (byte)((NetworkAmmo)__instance).Silo.type,
                                Ident = (ushort)id,
                                Slot = num,
                                Count = __instance.Slots[num].count,
                                Overflow = false,
                                Emotions = __instance.Slots[num].emotions
                            }.Send();
                        }
                        else if (__instance is NetworkPlayerAmmo)
                        {
                            SRMP.Log($"MaybeAddToSlot3 {((NetworkPlayerAmmo)__instance).ID} {id} {id}", "PLAYERAMMO");
                        }
                        flag2 = true;
                    }
                    num++;
                }
            }
            __result = flag2 && !flag3;
            return false;
        }
    }
}