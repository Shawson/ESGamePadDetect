# ESGamePadDetect

Simple command line app which returns XML.  

Developed for use with a POWERSHELL script I'm working on, this takes the DeviceName and DeviceGUID from an EmulationStation es_input.cfg file, and tries to lookup the given details against DirectInput to grab the controllers Product Id & Vendor Id (VID/PID).  This can then be used to match this config with the Autoconfig files used to RetroArch.

Usage;

    ESGamePadDetect.exe -Name "XBox One Controller #1" -Guid 78696e70757401000000000000000000
