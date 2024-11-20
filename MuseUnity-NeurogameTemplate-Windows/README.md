# Muse-Unity Neurogame Template for Windows platforms

This Neurogame Template based on Interaxon's Muse SDK for Unity 6 Preview.  
Check examples of how your Neurosity device can easily be used in-game.  
Turn your Bluetooth on and hit play. **You don't need to connect your Muse 2 to your PC's Bluetooth devices beforehand.**  

It is recommended to use a headband with a full battery, adjust it well and avoid any headgear that could phisically interfere with its placement.  
Headphones, for instance, can skew the placement and interfere with the electrodes' contact with your skin.

![UnityMuse-NeurogameTemplate-Window](https://github.com/user-attachments/assets/81d5d1fa-91e6-4e93-813d-99f146cecb59)

## Where do I get a neuro headset to play?

You can buy your own Muse headset at the [Choose Muse store](https://choosemuse.com/). I recommend the Muse 2 headset.  
You can get 15% off if you use my affiliate code: NEUROGAMEDEV  
Yes, all together, all caps.  

## What can you do with it?

You can use this template to start your first neurogame using a [Muse BCI device](https://choosemuse.com/).  
Although the template is as accessible as I could make it, you *do* need to have previous experience making games in Unity to understand its inner workings.  
The examples only make use of some of the data provided by the Muse 2, so I recommend that you check what other data is offered by the InteraxonInterfacer prefab.  
Do remember that the refresh rate of different data may vary. For instance, while PPG data is sent many times per second, battery data is refreshed every few seconds.  

As a final recommendation, again, adjust your headset comfortably tight and avoid any headgear that could phisically interfere with its placement.  
Headphones, for instance, can skew the placement and interfere with the electrodes' contact with your skin.
Oh, and don't forget to turn your Bluetooth on. No, you don't need to connect your Muse 2 to your PC's Bluetooth devices beforehand.

## Where is the fun stuff?

Check the NeuroPlayground scene in `Assets\NeuroGameDev\Scenes` to check examples of how to integrate Muse's streams of data directly into your game.  
For a more complete map of what data is being streamed, check the TestScene in the same folder, which should show you the following data:  

![UnityMuse-TestScene](https://github.com/user-attachments/assets/c598fb41-75d6-4a85-b8e4-5e70f5c1c87e)

I have also included a quick Pyrokinesis demo for your amusement. Just open the scene in `Assets\NeuroGameDev\Pyrokinesis`.  

The Muse SDK in and of itself is located in the `Assets\Interaxon` folder.

## Licensing

This template contains assts from Unity Technologies and Interaxon. By downloading and using it, you agree to both their terms of use.  
There are certain stipulations you may want to review in the Interaxon's agreement.
Very importantly, you cannot sell your neurogame without signing a commercial contract with Interaxon first.

- [Unity Technology Terms of Use](https://unity.com/legal/terms-of-service)
- [Interaxon's Terms of Use](https://github.com/neurogamedev/MuseUnity-NeurogameTemplate-Windows/blob/main/Third_Party_Development_Non_Commercial_Agreement.pdf)

My only request as the developer of this template is that you [credit me somewwere in your neurogame](https://creativecommons.org/licenses/by-sa/4.0/deed.en).

## Documentation

- For a general overview of neurogame design and the tools in this template, [consult the Wiki](https://github.com/neurogamedev/MuseUnity-Windows-NeurogameTemplate/wiki).
- The README_Unity.html file in the root folder has the very basics pertaining how to setup the Muse Unity SDK from scratch.
- The Readme_Windows.html file in the root folder has a link to the proper documentation of the SDK, functionalities, methods, enums, etc.
- The scripts in the `Assets\NeuroGameDev\Scripts` folder are self-documented and otherwise commented.

## The shoulders of giants

This project was built upon the hard work of others. As such, it is also dependent on the scrutiny and update of the tools they so generously provide. Make sure to check their sites and repositories if you want to upgrade your own project or if you want to Build for mobile platforms.

- Thanks to the [Muse Developers](https://choosemuse.com/pages/developers) team for the SDK and distribution Licence.

- Thanks to Unity Technologies for the [Starter Assets - Third Person Character Controller](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526) and for making [Unity](https://unity.com//) accessible to everyone.

## Dependencies

The Muse Unity SDK is dependent on the Libraries specific to each Build platform. This template uses the Windows 7.2.2 SDK. Android and iOS SDKs are also available in the [Muse Developers](https://choosemuse.com/pages/developers) forum.

If you decide to follow the README_Unity.html instructions to switch platforms, make sure to also copy the README.html file form the new platform SDK somewhere you can find it.

## Documentation

- [Wiki : Muse-Unity Neurogame Tempate for Windows](https://github.com/neurogamedev/MuseUnity-Windows-NeurogameTemplate/wiki)
- [Muse SDK Documentation](https://github.com/neurogamedev/MuseUnity-NeurogameTemplate-Windows/blob/main/doc/index.html)
- [Muse Tech Specs](https://www.eegsales.com/Shared/images/General%20Use/PDF%20Files/Muse_Technical_Specs.pdf)
- [Muse Data in EDF Format](https://us.v-cdn.net/6031894/uploads/90WW95XRQ1DY/muse-data-in-edf-format-283-29.pdf)
