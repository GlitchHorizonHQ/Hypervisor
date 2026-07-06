<div align="center">
<img src="https://github.com/GlitchHorizonHQ/Hypervisor/tree/main/.github/hypervisor.png" alt="Hypervisor banner" width="900"/>
 
<p><em>The community modding framework for <strong>Data Center</strong></em></p>
<img src="https://img.shields.io/github/actions/workflow/status/GlitchHorizonHQ/Hypervisor/build.yml?branch=main&style=flat-square" alt="Build Status"/>
<img src="https://img.shields.io/github/v/release/GlitchHorizonHQ/Hypervisor?style=flat-square" alt="Latest Release"/>
<img src="https://img.shields.io/github/license/GlitchHorizonHQ/Hypervisor?style=flat-square" alt="License"/>
<img src="https://img.shields.io/discord/1522707753267892275?style=flat-square&label=discord&logo=discord" alt="Discord"/>
<img src="https://img.shields.io/github/downloads/GlitchHorizonHQ/Hypervisor/total?style=flat-square" alt="Downloads"/>
</div>
<br/>

> Hypervisor is a runtime modding layer and API for **Data Center**, giving modders safe, structured hooks into racks, hardware, cabling, contracts, and the game's economy — without touching a single line of the base game's assemblies.
 
## Table of Contents
 
- [About](#about)
- [Features](#features)
- [Compatibility](#compatibility)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [API / Hooks Overview](#api--hooks-overview)
- [Creating Your First Mod](#creating-your-first-mod)
- [Mod Manifest Reference](#mod-manifest-reference)
- [Folder Structure](#folder-structure)
- [Contributing](#contributing)
- [Community & Support](#community--support)
- [Roadmap](#roadmap)
- [Credits & Acknowledgments](#credits--acknowledgments)
- [License](#license)
---
 
## About
 
**Data Center** simulates building and running a server empire — racking hardware, wiring up cabling, juggling thermals and power draw, and landing corporate contracts to grow revenue and reputation. Out of the box, the game has no supported way to extend any of that.
 
Hypervisor fills that gap. It's a lightweight loader and API layer that sits between the game and your mods, exposing stable, versioned hooks into the systems modders actually want to touch:
 
- Rack layout and slot placement rules
- Server hardware definitions and performance/thermal stats
- Cable routing and network topology
- Client contract generation, terms, and fulfillment checks
- Revenue, XP, and reputation formulas
- In-game UI panels and overlays
Instead of patching game binaries or fighting with fragile reflection hacks, mods built on Hypervisor register against a documented API, get hot-reloaded during development, and can declare dependencies on each other so complex modpacks load in the right order every time.
 
## Features
 
- 🔁 **Hot-reloading** — iterate on mod code and hardware JSON without restarting the game
- 🖥️ **Custom hardware definitions** — define new server units, NICs, cooling units, and rack types via data files
- 🪝 **Event hook system** — subscribe to lifecycle events (rack placed, contract signed, tick, save/load, revenue calculated, etc.)
- 🧩 **UI injection** — add panels, tabs, and overlays to existing in-game menus without replacing them
- 💾 **Save-data extensions** — attach custom serialized data to racks, servers, and contracts that persists across saves
- 📦 **Dependency resolution** — mods declare required/optional dependencies and load order is resolved automatically
- 🛡️ **Sandboxed API surface** — mods interact through Hypervisor's API, not raw game internals, so updates break far less often
- 🗂️ **Mod packaging & discovery** — drop-in folder structure, no manual DLL/script registration
- 📝 **Structured logging** — per-mod log channels for easier debugging
## Compatibility

> [!IMPORTANT]
> We currently don't know, the compatibility of **Hypervisor** Modding Framework for the game, but will update once we've checked that it works!
 
<!-- <table>
  <thead>
    <tr>
      <th>Data Center Version</th>
      <th>Hypervisor Version</th>
      <th>Status</th>
      <th>Notes</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>1.4.x</td>
      <td>2.3.x</td>
      <td>✅ Supported</td>
      <td>Current stable branch</td>
    </tr>
    <tr>
      <td>1.3.x</td>
      <td>2.0.x – 2.2.x</td>
      <td>✅ Supported</td>
      <td>Security fixes only</td>
    </tr>
    <tr>
      <td>1.2.x</td>
      <td>1.5.x – 1.8.x</td>
      <td>⚠️ Legacy</td>
      <td>No longer actively maintained</td>
    </tr>
    <tr>
      <td>1.1.x and earlier</td>
      <td>—</td>
      <td>❌ Unsupported</td>
      <td>Pre-dates Hypervisor's public API</td>
    </tr>
  </tbody>
</table> -->

## Installation
 
### For Players (installing the framework + mods)
 
1. Close Data Center if it's currently running.
2. Download the latest `Hypervisor.zip` from the [Releases page](https://github.com/hypervisor/hypervisor/releases).
3. Extract the archive into your Data Center install directory (the folder containing `DataCenter.exe`).
5. Launch Data Center — you should see a `Hypervisor vX.X.X` watermark on the main menu.
6. Drop mod folders into the `Mods/` directory created by the installer:
```bash
DataCenter/
└── Mods/
    └── your-mod-name/
```
 
7. Start (or restart) the game. Installed mods are listed and can be toggled from **Main Menu → Mods**.

### For Developers (building from source)
 
```bash
# Clone the repository
git clone https://github.com/GlitchHorizonHQ/Hypervisor.git
cd Hypervisor
 
# Restore dependencies
dotnet restore
 
# Build the framework in Release mode
dotnet build -c Release
 
# Run the local test harness against a copy of the game
dotnet run --project tools/Hypervisor.TestHarness -- --game-path "/path/to/DataCenter"
```
 
Build artifacts are output to `build/Hypervisor/`. Copy the contents into your local Data Center install to test changes, or symlink the folder for faster iteration:
 
```bash
ln -s "$(pwd)/build/Hypervisor" "/path/to/DataCenter/Hypervisor"
```
 
## Quick Start
 
Here's the smallest possible Hypervisor mod — it logs a message every time a new server is racked.
 
**Folder structure:**
 
```
HelloHypervisor/
├── mod.json
└── HelloHypervisor.dll
```
 
**`mod.json`:**
 
```json
{
  "id": "hello-hypervisor",
  "name": "Hello Hypervisor",
  "version": "1.0.0",
  "author": "your-name",
  "entryPoint": "HelloHypervisor.dll",
  "gameVersion": ">=1.4.0",
  "dependencies": []
}
```
 
**Entry point (`HelloMod.cs`):**
 
```csharp
using Hypervisor.API;
using Hypervisor.API.Events;
 
public class HelloMod : HypervisorMod
{
    public override void OnLoad()
    {
        Events.Hardware.OnServerMounted += (server, rack) =>
        {
            Log.Info($"Mounted {server.Name} into rack '{rack.Id}'.");
        };
    }
}
```
 
Build, drop the output into `Mods/HelloHypervisor/`, launch the game, and rack a server — the message will appear in `Logs/hello-hypervisor.log`.
 
## API / Hooks Overview
 
<details>
<summary><strong>Rack & Hardware API</strong></summary>
Register custom hardware, react to placement events, and read/modify live stats.
 
```csharp
Events.Hardware.OnServerMounted += (server, rack) => { /* ... */ };
Events.Hardware.OnServerUnmounted += (server, rack) => { /* ... */ };
Events.Rack.OnRackPlaced += (rack, position) => { /* ... */ };
 
HardwareRegistry.RegisterServerDefinition(new ServerDefinition
{
    Id = "acme-r7-blade",
    RackUnits = 2,
    PowerDrawWatts = 450,
    ThermalOutputBtu = 1536,
    ComputeScore = 820
});
```
 
Covers rack unit capacity checks, power/thermal budget queries, and slot occupancy events.
</details>
<details>
<summary><strong>Networking / Cabling API</strong></summary>
Hook into cable routing, topology changes, and bandwidth/latency calculations.
 
```csharp
Events.Network.OnCableRouted += (cable, fromPort, toPort) => { /* ... */ };
Events.Network.OnTopologyChanged += (topology) => { /* ... */ };
 
var throughput = NetworkGraph.CalculatePathBandwidth(rackA, rackB);
```
 
Useful for mods adding new cable types, switch hardware, or network-based contract requirements.
</details>
<details>
<summary><strong>Client Contract API</strong></summary>
Intercept contract generation, modify terms, and validate fulfillment.
 
```csharp
Events.Contracts.OnContractGenerated += (contract) =>
{
    contract.MinUptimePercent += 0.5f;
};
 
Events.Contracts.OnContractFulfillmentCheck += (contract, snapshot) =>
{
    return snapshot.AverageLatencyMs < contract.MaxLatencyMs;
};
```
 
Allows custom client archetypes, new SLA terms, and alternate contract-generation logic.
</details>
<details>
<summary><strong>Economy / Reputation API</strong></summary>
Read or adjust revenue, XP, and reputation formulas at key calculation points.
 
```csharp
Events.Economy.OnRevenueCalculated += (context) =>
{
    context.Revenue *= context.ClientTier == ClientTier.Enterprise ? 1.1f : 1f;
};
 
Events.Economy.OnReputationChanged += (delta, reason) => { /* ... */ };
```
 
Supports custom pricing models, reputation-gated content, and alternate progression curves.
</details>
<details>
<summary><strong>UI / Overlay API</strong></summary>
Inject panels, tabs, and HUD overlays into existing game menus.
 
```csharp
UI.Panels.RegisterTab("Datacenter Overview", "my-mod-tab", (container) =>
{
    container.AddLabel("Custom stats go here");
});
 
UI.Overlay.RegisterHudElement(new HudElement
{
    Anchor = HudAnchor.TopRight,
    Render = (ctx) => ctx.DrawText($"Uptime: {ctx.Uptime:P1}")
});
```
 
Panels and overlays are non-destructive — they layer on top of existing UI rather than replacing it.
</details>

## Creating Your First Mod
 
This walkthrough builds a mod that adds a new low-power "eco" server unit and tracks how many are deployed.
 
1. **Scaffold the project**
```bash
   dotnet new classlib -n EcoServerMod
   cd EcoServerMod
   dotnet add reference /path/to/Hypervisor.API.dll
```
 
2. **Create the manifest** at the project root as `mod.json`:
```json
   {
     "id": "eco-server-mod",
     "name": "Eco Server Pack",
     "version": "0.1.0",
     "author": "your-name",
     "entryPoint": "EcoServerMod.dll",
     "gameVersion": ">=1.4.0",
     "dependencies": []
   }
```
 
3. **Define the hardware** in code during `OnLoad`:
```csharp
   public class EcoServerMod : HypervisorMod
   {
       private int ecoServersDeployed = 0;
 
       public override void OnLoad()
       {
           HardwareRegistry.RegisterServerDefinition(new ServerDefinition
           {
               Id = "eco-lp-1u",
               DisplayName = "EcoLine LP-1U",
               RackUnits = 1,
               PowerDrawWatts = 90,
               ThermalOutputBtu = 307,
               ComputeScore = 210
           });
 
           Events.Hardware.OnServerMounted += OnServerMounted;
       }
 
       private void OnServerMounted(ServerInstance server, Rack rack)
       {
           if (server.DefinitionId == "eco-lp-1u")
           {
               ecoServersDeployed++;
               Log.Info($"Eco servers deployed: {ecoServersDeployed}");
           }
       }
   }
```
 
4. **Persist custom state** across saves using the save-data extension API:
```csharp
   public override void OnSave(SaveDataWriter writer)
   {
       writer.WriteInt("ecoServersDeployed", ecoServersDeployed);
   }
 
   public override void OnLoadSaveData(SaveDataReader reader)
   {
       ecoServersDeployed = reader.ReadInt("ecoServersDeployed", 0);
   }
```
 
5. **Add a UI readout** so players can see the count in-game:
```csharp
   UI.Panels.RegisterTab("Eco Stats", "eco-server-tab", (container) =>
   {
       container.AddLabel($"Eco servers deployed: {ecoServersDeployed}");
   });
```
 
6. **Build and test** using hot-reload:
```bash
   dotnet build
   cp bin/Debug/net8.0/EcoServerMod.dll "/path/to/DataCenter/Mods/eco-server-mod/"
```
 
   With the game running and hot-reload enabled, saving the manifest or DLL will trigger an automatic reload — no restart needed.
 
7. **Package for release** by zipping the mod folder (`mod.json` + compiled DLL) and sharing it, or submitting it to the community mod index.

## Mod Manifest Reference
 
<table>
  <thead>
    <tr>
      <th>Field</th>
      <th>Type</th>
      <th>Required</th>
      <th>Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>id</code></td>
      <td>string</td>
      <td>✅</td>
      <td>Unique, lowercase-hyphenated identifier for the mod</td>
    </tr>
    <tr>
      <td><code>name</code></td>
      <td>string</td>
      <td>✅</td>
      <td>Human-readable display name</td>
    </tr>
    <tr>
      <td><code>version</code></td>
      <td>string (semver)</td>
      <td>✅</td>
      <td>Mod version, e.g. <code>1.2.0</code></td>
    </tr>
    <tr>
      <td><code>author</code></td>
      <td>string</td>
      <td>❌</td>
      <td>Author or team name</td>
    </tr>
    <tr>
      <td><code>entryPoint</code></td>
      <td>string</td>
      <td>✅</td>
      <td>Relative path to the compiled DLL containing the mod's entry class</td>
    </tr>
    <tr>
      <td><code>gameVersion</code></td>
      <td>string (range)</td>
      <td>✅</td>
      <td>Compatible Data Center version range, e.g. <code>>=1.4.0 &lt;1.6.0</code></td>
    </tr>
    <tr>
      <td><code>dependencies</code></td>
      <td>array</td>
      <td>❌</td>
      <td>List of <code>{ "id": "...", "version": "...", "optional": false }</code> objects</td>
    </tr>
    <tr>
      <td><code>loadPriority</code></td>
      <td>integer</td>
      <td>❌</td>
      <td>Lower values load earlier when no explicit dependency exists (default <code>0</code>)</td>
    </tr>
  </tbody>
</table>

## Folder Structure
 
**Hypervisor framework install:**
 
```
DataCenter/
├── DataCenter.exe
├── Hypervisor/
│   ├── Hypervisor.Core.dll
│   ├── Hypervisor.API.dll
│   ├── Hypervisor.config.json
│   └── Logs/
└── Mods/
    ├── your-mod-name/
    │   ├── mod.json
    │   └── YourMod.dll
    └── another-mod/
        ├── mod.json
        └── AnotherMod.dll
```
 
**Typical mod project (source):**
 
```
YourModName/
├── mod.json
├── src/
│   ├── YourModName.cs
│   ├── Hardware/
│   │   └── CustomServers.cs
│   └── UI/
│       └── OverviewPanel.cs
├── assets/
│   └── icons/
└── YourModName.csproj
```
 
## Contributing
 
Contributions are welcome, whether that's bug fixes, new API surface, documentation, or examples.
 
- Follow the existing C# style conventions (`dotnet format` is run in CI — please run it locally before opening a PR).
- New public API additions should include XML doc comments and, where practical, a usage example in `/docs`.
- Please open an issue to discuss significant API changes before submitting a large PR.

## Community & Support
 
- 💬 [Discord Server](https://discord.gg/mVHfc7N9S3) — general chat, modding help, and release announcements
- 🐛 [Issue Tracker](https://github.com/GlitchHorizonHQ/Hypervisor/issues) — bug reports and feature requests

## Roadmap
 
- [ ] Core hook system (Hardware, Contracts, Economy, UI)
- [ ] Hot-reloading for scripts and hardware definitions
- [ ] Dependency resolution between mods
- [ ] In-game mod manager UI overhaul
- [ ] Scripting support for Lua-based lightweight mods
- [ ] Official mod index and one-click installer integration
- [ ] Multiplayer/co-op save compatibility layer
- [ ] Visual node editor for custom contract logic

## Credits & Acknowledgments
 
Hypervisor is an independent, community-built project. Huge thanks to the developers of **Data Center** for creating a game with such a satisfying simulation core to build on, and to everyone in the modding community who has contributed hardware packs, bug reports, and documentation.
 
## License
 
<img src="https://img.shields.io/github/license/GlitchHorizonHQ/Hypervisor?style=flat-square" alt="License badge"/>

Hypervisor is released under the **MIT License**. See [LICENSE](LICENSE) for the full text. You are free to use, modify, and distribute the framework and your own mods built on it, provided attribution is retained.
 
---
 
<div align="center">
 
<sub>Hypervisor is a fan-made, unofficial project and is not affiliated with, endorsed by, or sponsored by the developers or publisher of Data Center.</sub>
 
</div>
