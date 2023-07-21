# Stationeers mod "Fuel Jetpack" for BepInEx.

# Description

This mod add/changes the following on Jetpacks:

* The thrust increase/decrease buttons in the jetpack will now affect your flying speed. More thrust power will make you fly faster and less thrust power will make you go slower, but it will also greatly affect fuel consumption, so use it wisely.
- On full thrust, it's 250% of normal speed.
- On minimum thrust, it's 60% of normal speed.

* You can temporarily boost the Thruster/Speed to max by holding the Shift key during flight. When you release shift, the thrust will be back to the previous value.

* You need a proper fuel mix in the jetpack to fly. You can use O2+Vol or NOS+VOL fuel. Note that the default startingcondition from most planets will still give you a Nitrogen canister in the Jetpack so, if you want to use the jetpack early game, you have a couple of options:

If playing on any planet where you start with a WeldingTorch, you can borrow the fuel canister from the welding torch to use in the jetpack. This has the drawback that you need to keep swapping the fuel canister between the welder and the jetpack.
You can build a Canister Storage and use it to transfer some of the welding torch fuel to a new canister, that way you don't need to keep swapping the same fuel canister. Just remember that some amount of fuel will be left in the piping.
On Venus and Vulcan, you will start with no fuel canister on the lander. You can stay without jetpack at early game and earn it by the game means by buying one from the trader or making your own.
If you don't like the previous options, you can edit the startingconditions file to change the canister type your jetpack will get at the start. On Stationeers folder, go to \Stationeers\rocketstation_Data\StreamingAssets\Data then open the startconditions.xml file in your preferred text editor. Search and overwrite the text "ItemGasCanisterNitrogen" without quotes to "ItemGasCanisterFuel" (Overwrite all). Then close and reopen the game and after creating a new world, you should get a new fuel canister inside the jetpack.

* If you're using O2+Vol fuel mix, keep in mind that you'll get the combustion byproducts (CO2 and pollutants) in your environment, just like when you use the welding torch.

* The type of fuel will make a difference in speed, the N2O fuel burns stronger, so it will increase the jetpack base speed by 25%

* The planet gravity will affect the fuel consumption of the jetpack.


# Install Instructions

* If you don't have BepInEx installed, download the version v5.4.21, 64 bit, available at [https://github.com/BepInEx/BepInEx/releases](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.21) and follow the BepInEx installation instructions. Basically you will need to:
     - Unpack the Bepinex files inside Stationeers folder
     - Start the game once to finish installing BepInEx and check if it created the folders called \Stationeers\BepInEx\plugins, if yes, the BepInEx installation is completed.
* Download the lastest release from this mod here: https://github.com/ThndrDev/Stationeers-FuelJetpack-BepInEx/releases/
* Unpack the mod file inside the folder \BepInEx\plugins
* Subscribe for the mod on the Steam Workshop page https://steamcommunity.com/sharedfiles/filedetails/?id=2584817814
* Start the game.

# Contributions

If you want to contribute to this mod, feel free to do a pull request.
