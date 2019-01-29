# MicroDrop v2.28 Release Notes
<!-- vim-markdown-toc GFM -->

* [What's new?](#whats-new)
    * [Release format](#release-format)
    * [User Interface Features](#user-interface-features)
        * [Main window](#main-window)
        * [Device UI Window](#device-ui-window)
    * [DropBot Features](#dropbot-features)
    * [Other changes](#other-changes)
        * [Bug fixes](#bug-fixes)
        * [Performance improvements](#performance-improvements)
        * [Misc.](#misc)
    * [Known Issues](#known-issues)
* [Directional controls](#directional-controls)
    * [Usage](#usage)
* [Volume threshold](#volume-threshold)
    * [Required setup](#required-setup)
        * [Example:](#example)
* [Changes to actuation parameters](#changes-to-actuation-parameters)

<!-- vim-markdown-toc -->
-------------------------------------------------------------------------------

# What's new?

## Release format

 - **Smaller download and install size.**  Latest release available [here][microdrop-latest].
 - **Simplified installation process**; no options to set during install
 - **Over 10x faster install/uninstall**.
 - **Updates are automatically downloaded** and applied in the background
   (available on next program launch).

## User Interface Features

### Main window

![Main window][main-window]

 - **Simplify actuation parameters** (see [here](#changes-to-actuation-parameters) for changes):

   | Parameter                             | _Description_                                       |
   |---------------------------------------|-----------------------------------------------------|
   | ![Duration (s)][duration]             | Length of time (in seconds) to apply each actuation |
   | ![Voltage (V)][voltage]               | Actuation voltage (in RMS voltage)                  |
   | ![Frequency (Hz)][frequency]          | Actuation frequency (in Hz)                         |
   | ![Volume threshold][volume-threshold] | See [here](#volume-threshold) for more info         |
 - **Continuous update of actuation status**:  
   ![][continuous-actuation-status]
 - **Add statusbar to main window** _(displays channels reported as actuated from DropBot, etc.)_:  
   ![][statusbar]
 - **Display progress dialog during DropBot self-tests with option to cancel**:  
   ![][dropbot-self-test-progress-dialog]
 - **Output DropBot self-tests results as self-contained HTML report** _(not a Word document)_

### Device UI Window

![Device UI window][device-ui-window]

 - **Add directional controls in device UI**.  See [here](#directional-controls) for more info.
 - **Add "Find liquid" command to context menu**, which turns on electrodes where liquid is detected:  
   ![][find-liquid]
 - **Add "Visually identify electrode" command to context menu**, which quickly toggles neighbouring electrodes to, e.g., help figure out the camera orientation relative to the DropBot:  
   ![][identify-electrode]
 - **Draw routes while dragging:**  
   ![][draw-route]
 - Group electrode/route commands in sub-menus in context menu:  
   ![][electrodes-context-menu]
 - Draw dynamic actuation states as _light blue_ (i.e., electrode states applied only while
   protocol is running; e.g., electrodes along a route):  
   ![][dynamic-electrode-states]

## DropBot Features

 - **Automatically run shorts detection**:
     * Upon initial connection to DropBot
     * Each time a chip is inserted
 - **Display error when DropBot is halted due to exceeded current limit**
   * Helps to prevent damage to the DropBot due to, e.g., a short on the chip.

## Other changes

### Bug fixes

 - Device UI window
   * Apply layer alpha settings on application load
   * Fix [flicker on window focus change](https://github.com/sci-bots/microdrop/issues/254)
   * Set icon to MicroDrop icon
 - DropBot
   * Only refresh measured capacitance if high-voltage is enabled
   * Reapply channel states after shorts detection since some disabled channels may be re-enabled after one or more shorts are removed
   * Make DropBot connection process more robust
   * Improve DropBot communication stability by turning off HV during I2C communication

### Performance improvements

 - Reduce capacitance refresh frequency
 - Use faster compression for capacitance logging.

### Misc.

 - Change default device to [`SCI-BOTS 90-pin array`][sci-bots-chip]
 - Add optional plugins directories with `MICRODRROP_PLUGINS_PATH` environment var
 - Move DropBot help menu to main window help
 - Use context-aware logging to help with debugging issues
 - Reduce logging verbosity
 - Deprecate plugin update/download UI
   * Plugin updates are now bundled together with MicroDrop application updates, which are now handled automatically in the background.
 - Disable "realtime" checkbox while running


## Known Issues

 - Protocols generated with previous versions of MicroDrop will require updating to meet the [changes made to actuation parameters](#changes-to-actuation-parameters).

-------------------------------------------------------------------------------

# Directional controls

While actuating electrodes interactively, it is common to actuate a sequence of electrodes consecutively to move a drop across the chip.  For example:

![][click-actuation-sequence]

In this release of MicroDrop, we introduce a more convenient way to achieve this type of behaviour using **_directional controls_**.

## Usage

 1. Activate realtime mode:  
   ![][realtime-mode]
 2. Select the device UI window.
 3. Hold the `<Ctrl>` key while pressing an _arrow_ key on the keyboard.

The actuated electrodes will be adjusted such that the next electrode in the corresponding direction will be actuated (where possible).

For example:

![][directional-controls]

-------------------------------------------------------------------------------

# Volume threshold

![][volume-threshold-detail]

Without using the `Volume threshold` setting (i.e., when `Volume threshold` is set to 0), the electrode actuations within a step are considered completed once the specified `Duration (s)` has elapsed.

When the `Volume threshold` is set to a **_non-zero value_**, continuous capacitance feedback is used to determine when a requested actuation has completed.  This approach has the advantage of allowing a protocol to progress as fast as possible, where the next actuation is executed as soon as the completion of the requested actuation has occurred.

The `Volume threshold` is a value **between 0 and 1**, indicating the required **fraction of the expected capacitance** based on the actuated electrode areas that must be met before an actuation is considered completed.  In such cases, the specified `Duration (s)` is interpreted as a _maximum duration_ to wait for the target capacitance to be reached.

## Required setup

To use volume threshold, you must first calibrate the liquid capacitance (_which may vary slightly from chip to chip_) as follows:

 1. Activate realtime mode:  
   ![][realtime-mode]
 2. Actuate electrodes to cover an electrode with liquid.
 3. Right-click on any electrode in the device UI and select `Measure liquid capacitance`.
 4. The main MicroDrop window should display a **pF/mm^2** value.

This **pF/mm^2** value indicates the specific capacitance per unit area, which is used to calculate the expected capacitance for each actuation based on the area of the selected electrodes.

### Example:

![Calibrate liquid capacitance][liquid-capacitance]

-------------------------------------------------------------------------------

# Changes to actuation parameters

| Old                                                  | New                          | _Change_                                                                   |
|------------------------------------------------------|------------------------------|----------------------------------------------------------------------------|
| ![Duration][old-duration]                            | ![Duration (s)][duration]    | Renamed with units and units changed from **_milliseconds_ to _seconds_**. |
| ![Voltage][old-voltage]                              | ![Voltage (V)][voltage]      | Renamed with units; default is now **100 V**.                              |
| ![Frequency][old-frequency]                          | ![Frequency (Hz)][frequency] | Renamed with units.                                                        |
| ![Transition duration (ms)][old-transition-duration] | **N/A**                      | Deprecated.  `Duration (s)` is now applied to route actuations.            |


[realtime-mode]: static/program-electrode-actuation/realtime-mode.png
[duration]: static/program-electrode-actuation/duration.png
[voltage]: static/program-electrode-actuation/voltage.png
[frequency]: static/program-electrode-actuation/frequency.png
[volume-threshold]: static/program-electrode-actuation/volume-threshold.png
[volume-threshold-detail]: static/program-electrode-actuation/volume-threshold-detail.png
[old-duration]: static/program-electrode-actuation/microdrop-2.22.4/duration-v2.22.4.png
[old-voltage]: static/program-electrode-actuation/microdrop-2.22.4/voltage-v2.22.4.png
[old-frequency]: static/program-electrode-actuation/microdrop-2.22.4/frequency-v2.22.4.png
[old-transition-duration]: static/program-electrode-actuation/microdrop-2.22.4/transition-duration-v2.22.4.png
[liquid-capacitance]: static/program-electrode-actuation/calibrate-liquid-capacitance.gif
[click-actuation-sequence]: static/program-electrode-actuation/click-actuation-sequence.gif
[directional-controls]: static/program-electrode-actuation/directional-controls.gif
[continuous-actuation-status]: static/program-electrode-actuation/continuous-actuation-status.gif
[statusbar]: static/release-notes/statusbar.png
[sci-bots-chip]: https://github.com/sci-bots/dropbot-v3/wiki/DropBot-DB3-120-chips#digital-microfluidic-chips-for-dropbot-db3-120
[main-window]: static/release-notes/main-window.png
[device-ui-window]: static/release-notes/device-ui-window.png
[electrodes-context-menu]: static/release-notes/electrodes-context-menu.png
[dynamic-electrode-states]: static/release-notes/dynamic-electrode-states.gif
[find-liquid]: static/program-electrode-actuation/find-liquid.gif
[identify-electrode]: static/program-electrode-actuation/identify-electrode.gif

[dropbot-self-test-progress-dialog]: static/release-notes/dropbot-self-test-progress-dialog.gif
[draw-route]: static/program-electrode-actuation/draw-route.gif
[microdrop-latest]: https://sci-bots.com/microdrop-releases/Setup.exe
