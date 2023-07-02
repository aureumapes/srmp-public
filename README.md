# srmp-public
This is the code for the Slime Rancher MultiPlayer Mod (SRMP) by SatyPardus.

The user manual which includes information about compatability information, download and installation instructions and standard troubleshooting can be found [here](/manual.md).


## Bug Status 
Note: Bug list compiled from last known bug list of version 1488

FIXED:
- Multiplayer window doesn't show up on resolutions < 1920x1080

IN PROGRESS (NEEDS MORE TESTING):
- Exchange sometimes skips rewards if multiple players put items in at the same time
- Exchange chest disappears without rewards

Known Bugs:
- Plort collectors don't work properly sometimes (Playing effect but not pulling anything)
- Nutcracker doesn't spit out right amount, or spits out "babies"
- Slimes sometimes appear "angry" for other players
- Game stutters when placing/removing gadgets
- First time using the mod can apparently break a lot of things (falling through world, slimes not eating) - Restarting the game fixes it
- DLCs don't seem to be loaded correctly when leaving and joining
- DLC are not initialized on game start, making the "You need following DLCs:" message pop up (Can be fixed by loading the "Manage DLCs" menu and trying again)
- Gordos don't drop things sometimes
- Slimes sometimes dont produce plorts
- Drones get stuck in place sometimes
- Chat sometimes empty for remote players
- Upgrades sometimes does not get applied to All players


## Current Status
@Twirlbug:
- Currently working on going through the code, adding notes and fixing some of the bugs in my free time. 
- I adore this mod and want to give both credit and a huge thank you to Saty for the origional creation of the mod. 
I am slowly working my way through the list of bugs as seen above.

### Notation Status
Files in the following folders still need more notation:
- Networking
- Packets
- Patches
