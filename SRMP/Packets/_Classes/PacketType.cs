﻿namespace SRMultiplayer.Packets
{
    public enum PacketType
    {
        Unknown,
        PlayerJoined,
        PlayerLeft,
        PlayerPosition,
        PlayerLoaded,
        PlayerAnimation,
        PlayerFX,
        PlayAudio,
        ActorSpawn,
        ActorDestroy,
        RegionOwner,
        RegionChange,
        ActorPosition,
        ActorOwner,
        LandPlots,
        LandPlotAsh,
        LandPlotCollect,
        LandPlotFeederSpeed,
        LandPlotSiloRemove,
        LandPlotSiloInsert,
        LandPlotSiloSlot,
        LandPlotReplace,
        LandPlotUpgrade,
        LandPlotPlantGarden,
        LandPlotStartCollection,
        WorldData,
        GlobalFX,
        IncinerateFX,
        AccessDoors,
        Gordos,
        AccessDoorOpen,
        WorldTime,
        WorldFastForward,
        PuzzleSlotFilled,
        PuzzleSlots,
        WorldProgress,
        PediaShowPopup,
        PediaUnlock,
        PlayerUpgrade,
        WorldKey,
        WorldMapUnlock,
        PlayerUpgradeUnlock,
        PlayerCurrency,
        PuzzleGateActivate,
        WorldSwitchActivate,
        WorldSelectPalette,
        WorldSwitches,
        Gadgets,
        FashionAttach,
        FashionDetachAll,
        GadgetAdd,
        GadgetAddBlueprint,
        GadgetRemove,
        GadgetRotation,
        GadgetSpend,
        GadgetRefinerySpend,
        GadgetSpawn,
        PlayerCurrencyDisplay,
        LandPlotSiloAmmoAdd,
        LandPlotSiloAmmoRemove,
        LandPlotSiloAmmoClear,
        GadgetExtractorUpdate,
        DroneAmmoAdd,
        DroneAmmoRemove,
        DroneAmmoClear,
        DroneAnimation,
        DronePrograms,
        DroneLiquid,
        DroneStationEnabled,
        DronePosition,
        DroneActive,
        ActorResourceAttach,
        ActorResourceState,
        Actors,
        ActorReproduceTime,
        ActorFeral,
        ActorEmotions,
        WorldDecorizer,
        GadgetTurrets,
        GadgetSnareGordo,
        GadgetSnareAttach,
        WorldMarketSold,
        WorldMarketPrices,
        TreasurePodOpen,
        TreasurePods,
        WorldCredits,
        WorldMailSend,
        WorldMailRead,
        ExchangePrepareDaily,
        ExchangeOffer,
        ExchangeTryAccept,
        ExchangeClear,
        ExchangeOffers,
        ExchangeBreak,
        GordoEat,
        Oasis,
        OasisLive,
        FireColumnActivate,
        FireStormMode,
        GingerAction,
        KookadobaAction,
        ActorFX,
        WorldDecorizerSetting,
        GingerAttach,
        KookadobaAttach,
        EchoNetTime,
        GadgetEchoNetTime,
        RaceActivate,
        RaceEnd,
        RaceTime,
        RaceTrigger,
        PlayerChat,
        SteamPlayerJoined,
    }
}
