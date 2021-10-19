# Rhino To Energy 2D

Developed by Leland Jobson @ the Interdisciplinary Technology Lab

![image](https://user-images.githubusercontent.com/17731181/137999568-0bb8b45f-d987-4a6a-9438-e8b6ad93fed9.png)

A 1 step commmand to convert your curve section in Rhinoceros 6.x + into a E2D file.


Instructions

- Instructions deck here! https://docs.google.com/presentation/d/1mAq0MSd-OrRLqBsI5Y9oHY-39lL3d0qk3SnN__xIkqI/edit?usp=sharing

Installation

-Unzip the build folder
-Drag and drop the .rhp file onto Rhino 6+

https://energy.concord.org/energy2d/

Usage

-Select closed curves representing a section of your building, room or object
-Use the command DFG_Export2E2D
-Select the curves you want to export (in 2d, side view)
-Choose a location / name to save your file
-Open in Energy2D and you're done!

Caveats

-Curve geometry must be on the XY plane
-Curve geoemtry must be degree 1 (polylines)
-Document must be in meters
-Does not export materials/material settings. All of this is set on the E2D side.
-Exports separate files only

