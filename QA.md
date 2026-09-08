# Traffic Simulator — QA handoff

## Run the checks

```powershell
dotnet build TrafficSimulator.QA/TrafficSimulator.QA.csproj -p:OutputPath=bin/QA/
dotnet TrafficSimulator.QA/bin/QA/TrafficSimulator.QA.dll
dotnet build TrafficSimulator.sln -c Release
```

The separate QA executable uses the existing .NET/WinForms framework, with no test packages. Its outputs and day/night renderings are under `TrafficSimulator.QA/bin/QA/` (ignored by Git).

## Verified automatically

- Seed contains all seven entity types; pedestrians begin fully on sidewalks.
- Hundreds of placement coordinates, including off-screen points and the intersection centre, project onto a sidewalk using full pedestrian bounds.
- Actual form drag handlers project pedestrians during dragging and after release; moving a pedestrian resets its route.
- Routes from all four corners approach a crosswalk, cross on its centreline, continue along the far sidewalk and disappear only when the entire body leaves the canvas.
- Moving emergency vehicles prevent new crossing entry even with the siren off. Pedestrians already crossing continue clearing the road.
- Mid-crossing serialization produces the same subsequent positions and route state after loading.
- Vehicles avoid obstacles in all four directions, overtake when the neighbouring lane is clear, and wait when it is occupied. Movement is bounded by the free space ahead.
- Red and amber signals prevent entry; stop lines precede crosswalks.
- Buses added manually start in the right lane. Station boarding updates both counters, including a stop exactly 50 pixels away; loading does not duplicate boarding.
- All seven entity types, day/night, signal direction, amber state and phase ticks round-trip through Save/Load.
- Travel distance counts movement in all four directions, excludes dragging/pedestrians and is labelled in pixels. Delete All resets it.
- Day/night painting succeeds; the rendered day scene was visually inspected.
- 800 simulation ticks execute without vehicle/object overlap in the tested Seed scenario. This is a bounded scenario, not a proof for every scene the editor can construct.

Latest automated result: **14,262 assertions passed**, build **0 warnings / 0 errors**.

## Manual presentation check

1. Close the older simulator window after saving any scene you need. Open `TrafficSimulator/bin/Release/net8.0-windows/TrafficSimulator.exe`.
2. Drag a pedestrian onto each road and the intersection centre. It should snap to a sidewalk. Run and watch it approach, cross and continue until outside the canvas.
3. Switch to night, save through the actual file dialog, change to day, then load. Check the scene, button and signal visually. File dialogs themselves were not automated.
4. Add a hazard ahead of a car; test once with a clear adjacent lane and once with a vehicle occupying it.
5. Exercise right-click delete, wheel resize, Pause/Run and Delete All once on the presentation computer.

## Remaining limits

- This is still a discrete-step simulator. Lane changes happen in one step, without smooth steering. Existing overlap introduced by manually placing vehicles on top of one another is not automatically untangled.
- Pedestrians already crossing clear the road if an emergency appears; they are not frozen in the road. Sidewalk pedestrians can keep walking during emergency priority.
- The emergency siren is still automatically activated by a vehicle ahead; it is not a fully developed dispatch system. Without an active siren the emergency vehicle now follows its signal.
- Bus boarding is a passenger counter, not an animated transfer from a pedestrian. A bus can temporarily leave its right lane to avoid an obstruction and returns when space permits.
- BinaryFormatter remains the project's existing persistence format. Use locally created simulation files; arbitrary external files are not a supported input. Legacy pedestrian routes are reset safely to a sidewalk when loaded.
- WASD, audio and headlight beams remain outside this stabilization pass. No claim is made that the entire extended specification is complete.
