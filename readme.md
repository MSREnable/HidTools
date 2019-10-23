# Hid Tools for Eye Trackers

A collection of sample tools designed to help with development and debugging of HID based Eye Tracking devices.

## HidTimer

A tool which monitors the GazeMoved data stream and displays it on the screen. It keeps track of any gaps 
in the stream, and will display a simple message when there has been a time gap in the stream. A gap can 
be caused by gaze moving off of the window, eyes not being visible to the tracker, or a tracker malfunction.

This tool has been stress tested using a simulated HID Eye Tracker for 18+ hours and found to be working as
expected.