# Turiman
Monotorrent UI application written in Blazor

Turiman is Monotorrent UI written in Blazor. It is cross plaftorm compatible and has Dockerfile for simplicity.

* Configurable Downloader
* Torrent creator with WebSeeds
* Tracker

Application watches .torrent file creation in configured folder, if a new one appears it is loaded and waits for further instructions.
User can select torrent files that he/she wishes to download with a context menu.
Particular Files from a torrent can be prioritized in a same way.
Torrent download speed can be modified, torrent can be paused, started, stopped or deleted.

Service lifetime is subject to change, asp tends to play with instances, make them static singletons outside asp.
