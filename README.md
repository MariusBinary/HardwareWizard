# Hardware Wizard
Hardware Wizard is a software that collects information about the hardware of your device and monitors some parameters such as the temperature.

## Get a general overview on your pc, with information that updates in real time.
<img src="https://www.mariusbinary.altervista.org/assets/hardware_wizard/docs/prev1.png" />

## Monitors the temperature detected by the thermal sensors and the speed of the fans.
<img src="https://www.mariusbinary.altervista.org/assets/hardware_wizard/docs/prev5.png" />

## Get detailed information on all peripherals including: [motherboard, processor, graphic cards, memory banks, storage peripherals, monitors, etc ...]
<img src="https://www.mariusbinary.altervista.org/assets/hardware_wizard/docs/prev2.png" />

---

## What peripherals are supported?
Hardware Wizard can provide the following information on:
- **Motherboard**: attached fans speed, temperature, specifications.
- **Processor**: load (system, user), temperature, top processes that use more cpu, specifications.
- **Memory**: usage (hardware reserved, in use, modified, standby, available), top processes that use more ram, specifications.
- **Graphic Cards**: load, temperature, fans speed, memory usage, specifications.
- **Monitors**: specifications.
- **Storage Devices**: storage (used, available), partitions, temperature, specifications.

## The settings allows you to:
- Change the temperature unit between: Celsius, Fahrenheit, Kelvin.
- Change the sensors update interval.
- Choose whether to detect changes to storage drives in real time (e.g. when a USB device is inserted/removed).
- Change the application theme between light/dark.
- Choose which page to open first when the application starts.

## Dependencies
Hardware Wizard depends on the **OpenHardwareMonitor (v0.9.5)** library (https://openhardwaremonitor.org/) which is used exclusively to retrieve temperature sensors data and fans speed in real time. All the other information (eg. peripherals details, cpu load, ram usage, main processes, storage partitions, ecc...) is obtained directly from the operating system without the use of any external liberaries.

## For help or questions about the program:
Contact me at: mcelcovan@gmail.com
