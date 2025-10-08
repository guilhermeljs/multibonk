# Megabonk Multiplayer Mod

![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg) ![Status: Unstable](https://img.shields.io/badge/Status-Unstable-red.svg)

**Status:** Early Stages

This project is an open-source mod that enables multiplayer functionality for **Megabonk**. It is currently in its early development stages, so expect incomplete features and experimental implementations.

## Features

| Feature | Status | Description |
|---------|--------|-------------|
| Player synchronization | ![OK](https://img.shields.io/badge/OK-green.svg) | Players can see each other in real-time |
| Map synchronization | ![OK](https://img.shields.io/badge/OK-green.svg) | Same map is generated for all players |
| TCP connection | ![OK](https://img.shields.io/badge/OK-green.svg) | Reliable network connection established |
| Drops sync | ![Planned](https://img.shields.io/badge/Planned-orange.svg) | Items dropped by players will be synchronized |
| Level sync | ![Planned](https://img.shields.io/badge/Planned-orange.svg) | Player levels will be synchronized |
| XP sync | ![Planned](https://img.shields.io/badge/Planned-orange.svg) | Experience points will be synchronized |
| Chest sync | ![Planned](https://img.shields.io/badge/Planned-orange.svg) | Chests and loot will be synchronized |
| Minimap sync | ![Planned](https://img.shields.io/badge/Planned-orange.svg) | Minimap updates shared between players |
| And much more planned! | ![Planned](https://img.shields.io/badge/Planned-orange.svg) | -|

## Getting Started

1. Download and install with the steps provided at [Melon Loader Website](https://melonwiki.xyz/#)  
2. Follow the steps on the Melon Loader website to install Melon Loader  
3. Download a mod release (STILL NOT AVAILABLE)  
4. Paste the downloaded release folder into the game's folder  
5. Join the game and host a lobby  
6. Use ngrok for tunneling your IP to your friend, or use RadminVPN. The server is always started at port 25565  
7. Tell your friend to join the game and connect with IP:PORT

## Contributing

Contributions are very welcome and highly needed to help improve the mod!

To contribute:

1. Create a fork of this repository.  
2. Clone your fork locally.  
3. Open the `.sln` project in Visual Studio.  
4. Remove any references that are pointing to other paths.  
5. In Visual Studio, right-click the project dependencies and select **Add Project Reference**.  
6. Add all references from the game folders:  
   - `MelonLoader/Il2CppAssemblies`  
   - `MelonLoader/net6`  
7. Implement your feature or fix.  
8. Push your changes and open a Pull Request.  

Every contribution helps make the multiplayer experience better!

## Contact me

Discord: guijas5308