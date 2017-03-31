# WLAN Tweak

WLAN Tweak allows user configuration of the auto config, background scan and streaming mode parameters of a Wifi interface.

## Purpose

Current Intel Wifi drivers cause the interface to perform a regular scan for available networks, even when already connected to a network. (Every ten minutes, for an Intel Wireless-AC 8265 with driver version 19.50.1.5). This manifests as a spike in latency which is noticeable in low-latency applications, especially gaming where it manifests as a lag spike.

The only solution appears to be to set the interface to "streaming mode" which prevents the background scans. The auto config and background scan options may also be useful. Note that disabling auto config disables the standard Windows interface for connecting to Wifi networks.

## Credits

WLAN Tweak is based on the functionality of WLAN Optimizer by Martin Majowski: http://www.martin-majowski.de/
