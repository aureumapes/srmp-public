<!--

Hi! So I decided to rewrite the manual a little bit, this time in MarkDown format.
I knew from the start that putting the manual on github in md format was the better option, but I didn't.
Anyway, feel free to contribute to this manual I guess. I hope that this isn't too hard to work on!

- Zervó

-->

# SRMP Manual

This is a detailed manual for the Slime Rancher MultiPlayer mod (SRMP), it has installation instructions, compability information, usage instructions, troubleshooting steps and some more. It is updated frequently to make it more readable and easier to understand. Now that it is on github, feel free to contribute!

## Content

Here is a list of content in this document.
Click a link to jump to that section of the document.

[What is SRMP?](#what-is-srmp)

[Compability](#compability)

[Download the mod](#download-the-mod)

[Install the mod](#install-the-mod)

[Using the mod](#using-the-mod)

[Troubleshooting](#troubleshooting)

[Credits](#credits)

## What is SRMP?

Slime Rancher MultiPlayer, or SRMP, is a mod developed by Saty that adds multiplayer functionality to the game Slime Rancher. As of the writing of this document, development of the mod has been discontinued, however it still works with the last Slime Rancher release. SRMP has been in development since around March 2019, but later got hold. Active development was later resumed in 2020, but has now stopped, might be continued now that it has been made opensource though. The reasons for all this, can be read in #announcements in [Saty's Discord](https://discord.gg/NtB7baV).

## Compability

SRMP works on all PC versions of the game except for the Microsoft Store version, so: Steam, Epic Games and GoG. Remember, this mod is for Slime Rancher 1, not 2! As Slime Rancher runs natively on windows, so will the mod. Both Slime Rancher and SRMP works on Linux, and the non-PC platform Mac, however getting Slime Rancher to work on those may require additional steps, and there are some issues with the mod that you may encounter (described in the Troubleshooting section). Below is a table to better visualize the platform compability for Slime Rancher + SRMP. Note: Table may currently be inaccurate as I don’t have enough data.

<!-- Only update this table if you have done enough testing to feel confident about your results, try not to provide inaccurate information. -->

|   Compability   |     Windows     |       Linux      |        Mac      |      Other      |
| :-------------: | :-------------: | :--------------: | :-------------: | :-------------: |
| Steam           | 🟢 Compatible   | 🟢 Compatible   | ⚪ Unknown      | 🔴 Incombatible |
| Epic Games      | 🟢 Combatible   | 🟡 Kinda works  | ⚪ Unknown      | 🔴 Incombatible |
| GoG             | 🟢 Combatible   | ⚪ Unknown      | ⚪ Unknown      | 🔴 Incombatible |
| Microsoft Store | 🔴 Incombatible | 🔴 Incombatible | 🔴 Incombatible | 🔴 Incombatible |

Unknown just means that the original author didn't have enough data to say if the platform would work or not.

If you want an explanation to a specific entry in the table, select the operating system and platform below:

<!-- REMEMBER: If you updated the table above, also update the explanation below. -->

<details>
<summary>Windows</summary>
    <details>
        <summary>Windows: Steam</summary>
        🟢 Compatible: The game runs natively on Windows and Steam, no issues should be caused by this combination.
    </details>
    <details>
        <summary>Windows: Epic Games</summary>
        🟢 Compatible: The game runs natively on Windows and Epic Games, no issues should be caused by this combination.
    </details>
    <details>
        <summary>Windows: GoG</summary>
        🟢 Compatible: The game runs natively on Windows and GoG, no issues should be caused by this combination.
    </details>
    <details>
        <summary>Windows: Microsoft Store</summary>
        🔴 Incombatible: As with everything else from the Microsoft Store, this version of the game is scuffed.
    </details>
</details>

<details>
<summary>Linux</summary>
    <details>
        <summary>Linux: Steam</summary>
        🟢 Compatible: By using proton and then installing the mod as you would on windows, you get the same performance and probably no issues.
    </details>
    <details>
        <summary>Linux: Epic Games</summary>
        🟡 Kinda works: So far only tested through Heroic Games Launcher as well as Lutris. Sometimes it works without issues, but broken UI is common.
    </details>
    <details>
        <summary>Linux: GoG</summary>
        ⚪ Unknown: Not tested (by the original manual author) yet. Not enough data to cover this entry.
    </details>
    <details>
        <summary>Linux: Microsoft Store</summary>
        🔴 Incombatible: Good luck getting Microsoft Store working on linux. Even if you did, this version doesn't work with the mod.
    </details>
</details>

<details>
<summary>Mac</summary>
    <details>
        <summary>Mac: Steam</summary>
        ⚪ Unknown: Not tested (by the original manual author) yet. Not enough data to cover this entry.
    </details>
    <details>
        <summary>Mac: Epic Games</summary>
        ⚪ Unknown: Not tested (by the original manual author) yet. Not enough data to cover this entry.
    </details>
    <details>
        <summary>Mac: GoG</summary>
        ⚪ Unknown: Not tested (by the original manual author) yet. Not enough data to cover this entry.
    </details>
    <details>
        <summary>Mac: Microsoft Store</summary>
        🔴 Incombatible: Good luck getting Microsoft Store working on Mac. Even if you did, this version doesn't work with the mod.
    </details>
</details>



## Download the mod

The first step for any platform is to download the mod. There are two ways to do this, one free and one paid, and there are multiple versions to choose from so here is a short explanation. The first alternative: you can get access to the mod by purchasing it on itch.io, which is mainly to support Saty. Even though the mod isn’t being developed by her anymore, Saty did a lot for the community and helped change Slime Rancher for the better, so paying for the mod even though development has stopped is a great way of giving back. If you can’t afford to pay for the mod or if you are selfish then there are two other methods: Discord and Github (not sure about github but I assume the mod will be released here as well). Do not get it from any other sources. Before downloading, decide which version to use, read below

### Which version to use?

There are a lot of versions of the mod, and two different “types”. Whether you get the mod from Itch, Discord or github, trying to find the correct file can be confusing. To keep this short: the recommended version is 1488 (might change if someone picks up development), there is a newer version available on discord (1503) but users seem to have more issues with that version. All versions prior to 1503 has a bug that breaks trading in the game, so that you don’t get any rewards when completing trades. 1503 fixes that, but tends to break other things. Only use this version if trades are extremely important to you or if you can’t get 1488 or other versions to work despite “trying everything”. When you have decided which version to use there is still a choice left to make..

There are two “types” for each version, one called SRML and one called Standalone. SRML is for the Slime Rancher Modlauncher, and allows you to use it with other mods (with varied results). Since that version is installed like any other SRML mod, instructions for that aren’t provided here (yet). Usage instructions still apply. Standalone is for running SRMP standalone, who could have guessed! That means indepentent of other modloaders, and It is actually really easy to install! That’s all you need to know for picking a version, if you are unsure, go with SRMP_1488_Standalone. The recommended version might change in the future, the manual will be updated if it does.

### Download on Itch.io (paid)
If you want to get the mod and support Saty at the same time, then head to the [itch.io page](https://satypardus.itch.io/slime-rancher-multiplayer-mod)! Here you pay and download your desired version. If you are new to this mod and didn’t read the ‘Which version to use?’ section above, I highly recommend you do so instead of downloading a random file and then bothering people in the discord with your questions. Note that if development is picked up, new releases probably won't be published to itch. Now [install the mod](#install-the-mod) for Windows, Linux, or Mac.


### Download on Discord (free)

The mod is available to download for free in [Saty's Discord](https://discord.gg/NtB7baV). All you need to do is join the discord and then download your desired version from the #multiplayer-development channel. So, now that you have downloaded your mod zip archive, move on to the next step, installation! This document includes instructions for [installing the mod](#install-the-mod) on Windows, Linux and Mac.

<!-- Someone please correct me on this if necessary -->
### Download on Github (free)?

If development is picked up, future releases will probably be published on GitHub. But at the time of writing this, there are no releases to be found here.



## Install the mod

Now that you have downloaded the mod (because you have done that, right?) it is time to actually install it! Remember that these instructions are for the Standalone version, SRML version is installed like other SRML mods and therefor instructions are not provided here (yet). Now follow the steps for your OS and platform.

### Windows

<!-- Decided to put instructions in details tags to reduce the amount of scrolling needed -->

<details>
<summary>1. Finding the game installation directory</summary>
The first step to installing the mod is to know where to install it to. We need to find the game installation directory (we will refer to it as GID from now on). This is done a little different depending on which platform (aka launcher) you have the game on. If you have it on multiple and one of them is steam, use steam. If you pirated the game, get out of my manual. Read the section for your launcher.
    <details>
    <summary>1.a. Steam</summary>
    It is really easy to find the GID on Steam, it only takes a few clicks. Open your library and find Slime Rancher in the list. Rightclick it, hover over ‘Manage’ and click ‘Browse local files’. This should open the explorer (or your default file explorer) in the GID. You know it is the correct directory if it contains a folder called SlimeRancher_Data.
    </details>
    <details>
    <summary>1.b. Epic Games</summary>
    Finding the GID on Epic Games can be a little more complicated, as they aren’t installed to a universal location. The easiest thing is to initialize an install for another game and check what the pre-selected installation directory is, cancel the install and then check that directory for your GID. You know it is the correct directory if it contains a folder called SlimeRancher_Data. Another way to find it is to check the path of the Slime Rancher desktop entry/shortcut, or to just reinstall the game to a known location.
    </details>
    <details>
    <summary>1.c. GoG</summary>
    Since I (the original manual author) have never used GoG, this is written using information I could find online. According to the GoG support center, games are installed to a universal directory by default, called ‘GoG Games’. This is located in a directory defined by the HOME environment variable. You should be able to find this path by opening command prompt and running the command ‘echo %HOME%’. You know it is the correct directory if it contains a folder called SlimeRancher_Data.
    </details>
</details>

<details>
<summary>2. Installing the mod</summary>
Once you know the GID (game installation directory) of your game you can move on to actually installing the game. First up, make sure you have your GID open in a file explorer. Again the GID can be identified by containing a folder called SlimeRancher_Data. Next, open the directory containing the downloaded mod zip archive (probably Downloads). Copy or move the zip into the GID, not into SlimeRancher_Data or another folder inside of the GID, but the GID itself, the directory containing SlimeRancher_Data and such. Now that the zip archive is in place, extract it by rightclicking and selecting ‘extract here’. It should ask to replace a file in SlimeRancher_Data, if it does then select yes, if it doesn’t then you have extracted it to the wrong directory or you just need to move it out of a folder first. Once extracted correctly, it should have replaced a file in SlimeRancher_Data and created a new folder called Mods (do not put other mods in here). That’s it! Now move on to step 3.
</details>

<details>
<summary>3. Verifying the install</summary>
Now to test if everything has installed correctly. Start the game from your launcher and wait for it to load. Once in the main menu, you should see a gray box in the top-right. If you do, you did everything correctly. If you don’t, try pressing f4 and if it still doesn’t show up then you did something wrong, go back. Now that you have the mod installed correctly, you can enter a username in the gray box, it can be anything and will be the name shown in-game. Now that the mod is installed and working you can move on to using the mod.
</details>

### Linux
<!-- Unless someone else does it, I will update this section to be a lot more detailed and easier to understand in the future. -->

This section will be updated with actual instructions in the future, but for now here are the basic instructions for steam: Install the game with proton, then install the mod as you would on windows. Done!

### Mac

As I (the original manual author, zervó) has never touched a Mac in my life, I have no idea how this works. Please update on this section.



## Using the mod
<!-- This section requires some work, which I am willing to give but feel free to update this to make it more accurate, more inclusive and easier to understand. -->

Now that you have installed the mod and it is working correctly it is time to actually use it. There are 3 main methods to choose from: Playing over LAN, Playing over “fake” LAN (VPN), and Playing over the Internet (port forwarding). The options are ranked below from easiest to hardest. There used to be a 4th method, server codes, but as far as I know they no longer work as the server for them has been shut down.

All three methods have a few things in common. First, make sure to choose a username if you haven’t already, this can be anything and will be the name shown in-game. Second, all three methods use some combination of IP address and port to host and join. An Ipv4 address which is the type you will most likely be using, looks like this: xxx.xxx.xxx.xxx, with x being numbers. Common ones for private Ips (local networks) are 192.168.1.xxx and 10.10.1.xxx. EG 192.168.1.43 or 10.10.1.107. Public Ips (external/WAN connections) usually have completely different numbers but the structure is the same. The port is a combination of 5 numbers, which the host can find once they are in a loaded save.

Note down the port before attempting any of the methods, or use a method with a port that you choose and change the port in the game later.

### Playing over LAN

This method only works if you and the other player/s are on the same local network (or same wifi, in incorrect terminology aka Layer 8 lingo). One player takes the role as the host. The host needs to load a save, it can be an old one or a new one it doesn’t matter just enter a save. Now in the game, the host checks the menu top right. There will be a few options there. Now the host notes down the port and clicks ‘Host’. The host now needs to find their local IP address. The fastest way to do this is probably through a terminal. On windows: open command prompt, enter ‘ipconfig’ (no ‘ signs) and press enter. Find the correct network adapter in the list and then the local ip. On linux, open a terminal and enter ‘ifconfig’, then do kinda the same thing as on windows. Players now from the main menu choose to join a game and join by IP address, entering it into the correct field, and then the port into the correct field and then joining. It’s that simple.

### Playing over "fake" LAN (VPN)

If you aren’t on the same network then there is still a pretty simple way to do it. Introducing, Hamachi! Before you get too exited you should know there is a drawback with this method, two actually. The first one is that you have to sign up for an account, the second one is that Hamachi has a rate limit, meaning that you could run into issues like not being able to see other players and falling through the map. Now the first step to using Hamachi is to getting an account (NOTE: these hamachi instructions are for Windows/Mac, if you are using linux you know better than to use hamachi but you still can, however it is through commandline so you won’t get any help here). You can create an account by going here. Once your account is created and verified and whatnot, download hamachi from their homepage. Now you need to setup and join a hamachi network, which is described pretty well here, skipping the ‘Host a Server’ section and the other game specific part at the end. Once all players are connected to the hamachi network, you just host and join as if you were on the same local network as described in the ‘Playing over LAN’ section above. If Hamachi fails you can try an alternative called Radmin. Hamachi local Ips may be different, if they are then google how to find them or something. I will update this to include instructions for that at some point.

### Playing over the Internet (port forwarding)

The third and most advanced option is port forwarding, but it is also the one giving the best results if all players aren’t on the same network. There are two ways to port forward: via your router’s admin interface or through a UPnP portmapping client. The second one is easiest and most convenient, but not all routers support it, it could also pose a potential security risk. I would recommend trying UPnP first as it is pretty easy with the client I will be using, and try to do it via the admin interface if UPnP doesn’t work. Using a UPnP client doesn't make the security problems with UPnP bigger, because if it works then it is already enabled in your router which means the security flaws are already present.

<details>
<summary>Via the router's admin interface</summary>
This method of port forwarding is the most complicated of the two, to be honest the UPnP client I used in those instructions isn’t even complicated at all so try that one first if you haven’t already. Now, to get started with this method you need login credentials for your router. They are usually found on a sticker on the back of the router. There might be one for wifi credentials and one for admin credentials so doublecheck that you have the right one. If you are lucky there is a default address listed on the sticker as well (usually called “internal address” or just “ipv4 address”) that will save you some time by skipping the next part of the instruction, finding your router’s local ip.

To find your router’s local ip on windows: open command prompt and run ‘ipconfig’. Look for something that says ‘default gateway’, that should be your router ip. On Linux: open a terminal and run ‘ip route’. The first thing that is displayed should be something like “default via 192.168.1.1”, that ip address at the end is the one you are looking for. On mac: open System Preferences, navigate to Network > Advanced > TCP/IP, and find the IP address listed next to “Router”, that is the one you are looking for.

Now that you have the IP of your router, open a web browser and put it into the address field (do not search for it, enter it as an address). It should open a webpage, if it doesn’t and just searches instead, put http:// in front of the address. Now log in with the credentials you found earlier, some routers display a default status page and requires you to press something before showing the login prompt. Once you are logged in try to find the port forwarding section, for me it was under “NAT/QoS”. Add a new rule. Below are the values you should input, each field might be called something different in your router and there might be extra fields, or less. Generally the other fields can be left empty, otherwise just google them. If you get confused, look up a video or image of port forwarding.

Application name: can be whatever, I will use ‘srmp’
Source net: can be left empty
Protocol: UDP
External start port: the port you noted down earlier
External end port: same as above
Internal port: same as above
IP Address: the IP of your machine (instructions on how to find in ‘Playing over LAN’)
Enable: yes

Now that the rule is complete, save and/or apply it. You should now be able to exit the admin interface of your router and test the rule. Host the save in the game if you haven’t already. Your friends join with your public IP and the port as described in the intro to this section. If you are unsure you can find your public IP at [https://whatismyipaddress.com](https://whatismyipaddress.com) or similar websites. No they don’t “doxx” you, your public IP address is just as the name suggests publically accessable and is necessary to access the internet. If you are using a VPN, turn it of as it "changes" your public IP to something where you can't map ports.
</details>

<details>
<summary>Via an UPnP client</summary>

If you are lucky and your router supports UpnP and it is enabled this method will work for you. Using a special program you can utilize UPnP to create port forwards without having access to your router’s admin interface. To do this we will use a program called portmapper, download the .jar file from Assets [here](https://github.com/kaklakariada/portmapper/releases). You need to have java installed on your system. Once you have the program downloaded run the jar file, a white window with a  bunch of stuff will show up. Click the button that says connect and wait, if it connects, great! It might say that there are multiple routers, check the list and if they are the same just click ok. If it can’t connect, go into ‘PortMapper Settings’ and change the UPnP library to something else and try connecting again. If you have tried them all and it doesn’t work, then I’m sorry, UPnP won’t work for you. 

Now if you are connected, all active UPnP rules will show up. You might see some that other games have created. You will also see your router’s external address, or public ip, which is another way to find it. Now in the box that says ‘Port mapping presets’ click ‘Create’. A new window will open. The ‘Description’ field is basically the name, so I will call it srmp. ‘Remote host’ can be left empty. For Internal Client, check ‘Use local host’. It should select your local network ip. Now click ‘Add’. For the Protocol, change it to UDP. And for both External Port and Internal Port you should put the port that you noted down earlier. Now click ‘Save. Now in the ‘Port mapping presets’ box you should see your srmp record. Click ‘Use’. This should add the port forward to your router. Click ‘Update’ to make sure that it is still there. Now players can try joining as described earlier, with public ip and port! Public IP can be found on sites like [this](https://whatismyipaddress.com/), but it usually also shows up in the interface of the portmapper software. Unlike traditional port forwarding you probably don’t need to worry about removing these records, as most routers clears them when you reboot them. If you want to remove them, you can do so by selecting the record in the list and clicking remove. This method of port forwarding can also be used to forward for any other game, and is in my opinion very easy and convenient. You could let your friends from all over the world join your minecraft world for example!

NOTE: If this method didn't work and UPnP isn't enabled in your router, I don't recommend trying to enable it for future use. As this program demostrates, you don't need any form of authentication to map ports with UPnP. This means that infected devices on your network could open ports which would allow hackers and other people with malicious intents to get full access to your home network. So if UPnP is disabled, leave it like that.
</details>



## Troubleshooting

Here is a bunch of scenarios where some things with the mod might not work for different reasons, and how to solve those issues. Not all issues are listed here, and if this doesn’t help you, ask in the [Discord](https://discord.gg/NtB7baV). Sometimes, all players restarting the game can also fix certain issues. This section will be extended in the future.

<details>
<summary>Players are invisible</summary>

If you are using Hamachi, Radmin or a similar solution then this is likely caused by rate limiting. The mod uses a lot of network traffic, especially when loading. One solution you could try is to disable encryption in Hamachi or whatever solution you are using, or if possible changing/removing any limits. If it doesn’t help immediately then wait a bit. If you have waited for a while and nothing has changed then you probably want to use another method in [using the mod](#using-the-mod). If you have tried everything up to port forwarding and it still doesn’t work then there is something on your network limiting your connection or your network/internet connection is just slow. Try to let someone else be the host.
</details>

<details>
<summary>"Someone with that UUID already online"</summary>
This error is caused by an occupied UUID. If you are using a pirated version of the game, get out of this manual! If you aren’t and you still get the issue then open your game installation directory, find the UserData folder in the SRMP folder and delete it. Restart the game.
</details>

<details>
<summary>DLC Mismatch</summary>
This is a common connection error caused by players not having the same DLC activated. This can be solved by deactivating all DLCs or all players getting the same DLCs.
</details>

<details>
<summary>Can't connect to a server</summary>
If you can’t connect to a server it is because something is blocking traffic. This could be because the selected port is already in use, there are firewall rules on the host/client machine blocking traffic, there are firewall rules on the router doing the same thing, incorrectly forwarded port etc.
</details>

<details>
<summary>Falling through the map</summary>
This issue can be caused by the same things that case the "Players are invisible" issue. Try the fixes there. According to some user reports, walking around the map before hosting and letting other players join can also fix these issues.
</details>

<details>
<summary>Game crashing instantly on startup</summary>
You probably did something wrong with installing the mod, like installing Standalone when you needed SRMP or the other way around. Or trying to install multiple versions on top of eachother. It is best to start with a fresh install of the game before installing the mod, and that includes clearing the installation directory before uninstalling it from your launcher.
</details>



## Credits

Mainly wanted to include this section for people who want to ask me (the original manual author) questions.
The manual was originally written by [Zervó](https://github.com/ZervoTheProtogen) (Zervó#9755 on discord).
Contact me if for example you need clarification on something before committing to this manual.

Also I want to include Saty here, she has done an amazing job creating this mod. Thank you Saty.
