# RDP Plugin for PowerToys Run

A [PowerToys Run](https://aka.ms/PowerToysOverview_PowerToysRun) plugin to launch RDP connections.

Most of the code is from FlowLauncher Plugin [RDP](https://github.com/MBeggiato/Flow.Launcher.Plugin.RDP).

Checkout the [Template](https://github.com/8LWXpg/PowerToysRun-PluginTemplate) for a starting point to create your own plugin.

## Installation

1. Download the latest release of the from the releases page.
2. Extract the zip file's contents to `%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins`
3. Restart PowerToys.

## Usage

1. Open PowerToys Run (default shortcut is `Alt+Space`).
2. Type `rdp` and search for a connection.

## Building

1. Clone the repository and the dependencies in `/lib`.
2. run `dotnet build -c Release`.
