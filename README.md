# SmartCuffs - London Studios
**SmartCuffs** is a **FiveM** resource coded in **C#** to enhance the player handcuff experience. You have the ability to both frontcuff and rearcuff other players, along with an included British Speedcuff Model and custom sounds.

The plugin gives you one pair of handcuffs, so ensure you get that back otherwise you will be unable to cuff other players until you retreive it, or another player hands you a pair.

The **British Speedcuffs** model will spawn on any ped, this is not EUP.

This plugin is made by **LondonStudios**, we have created a variety of releases including TaserFramework, SearchHandler, ActivateAlarm, SmartTester, SmartSounds, CustodyAlarm and more!

Join our Discord for exclusive plugin previews [here](https://discord.gg/AtPt9ND).

<a href="https://www.buymeacoffee.com/londonstudios" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/lato-orange.png" alt="Buy Me A Coffee" style="height: 51px !important;width: 217px !important;" ></a>

![SmartCuffs](https://i.imgur.com/dXM05Er.png)

## Usage
**/cuff** - Rearcuff the nearest player.
**/frontcuff** - Frontcuff the nearest player.
**/resetcuff** - Gives you a pair of handcuffs again.

Both **/cuff** and **/frontcuff** have been mapped to Keybinds, these can be configured by each player, allowing for their own custom keybinds. These can be set in **Settings > Keybinds > FiveM.**
These **must** be setup by each player upon their first time using the plugin.

The included **British speedcuffs** sound will play upon cuffing and uncuffing, to all players in the area and reduce in volume based on distance from the cuff location.

You are not able to cuff players if you are in a vehicle, if you are already cuffed or you have no pairs remaining.
## Installation
 1.  Create a new **resource folder** on your server.
 2.  Add the contents of **"resource"** inside it. This includes:
"Client.net.dll", "Server.net.dll", "fxmanifest.lua", "html", "stream"
3. In **server.cfg**, "ensure" SmartCuffs, to make it load with your server startup.

## Source Code
Please find the source code in the **"src"** folder. Please ensure you follow the licence in **"LICENCE.md"**.

## Feedback
We appreciate feedback, bugs and suggestions related to SmartCuffs and future plugins. We hope you enjoy using the resource and look forward to hearing from people!

## Credits
The British Speedcuffs model has been made by a third party 3D modeller, known as "That Alien". We would like to thank them for allowing the release of the model and we hope to work with them in the future. This model may not be redistributed, modified or released. Model is released for use in conjunction with SmartCuffs.
## Screenshots
Take a look at some screenshots of the plugin in action!

![Frontcuff Image](https://i.imgur.com/KQiqlKM.png)

![Rearcuff Image](https://i.imgur.com/urL878K.png)

## Plans and Updates
We are very happy with the outcome of this plugin, however at London Studios we are always looking for improvement and welcome your feedback. Here's our future plans for this plugin:

 - Add /drag, allowing you to drag the nearest civilian
 - Add an option allowing you to place somebody in handcuffs into a vehicle
 - Add a config.ini file giving users more control over the plugin.
 - Fix any bugs which are discovered and reported.
