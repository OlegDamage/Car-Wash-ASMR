# Timed shifts — implementation status

Prepared: 2026-11-03, feature/timed-shifts, base 047657e. This branch-specific status refines the planned first stage in DEVELOPMENT_PLAN.md; it does not supersede the agreed design or change the released 1.0 (2).

## First substep: isolated work clock
Files prepared: ShiftController, ShiftClockUI, Editor-only ShiftClockSetup and their metas. Existing gameplay scripts, SampleScene, README, policy, store descriptions, packages and ProjectSettings unchanged by this commit. The scene is changed locally only when the new command is run.

No Unity compilation, Editor execution, device test or successful local installation is claimed. These classes now have prepared source files; the older plan's statements about not-created classes describe the pre-implementation baseline. No counting, earnings, shift summary, save or new car is implemented in this substep.

## Local setup
1. Confirm GitHub Desktop is on feature/timed-shifts, Fetch/Pull this commit. Stop on compilation errors.
2. Outside Play Mode, open Assets/Scenes/SampleScene.unity and save it. Run Car Wash ASMR > Setup Shift Clock Prototype ONCE. Do not rerun old setup commands.
3. The command creates a separate root ShiftController and SafeArea/ShiftClock. Existing UI is not moved. Inspect placement: initial clock anchor top-centre, position (0,-240), size 620x64 in Canvas reference units. This placement has not been visually tested. If it overlaps, move only ShiftClock or report a screenshot; do not redo the garage. Save the scene.
4. Root ShiftController Inspector: Start Hour 10, End Hour 18, Shift Duration Seconds 60 for a short test. Default/future balance hypothesis is 300, not calibrated. Set values OUTSIDE Play Mode. No fields need dragging; the label resolves the active controller.

## Checks
- Enter Play, focus Game view: starts at 10:00 and reaches 18:00 after approximately 60 active seconds; then displays Clock ended and stops advancing. At 30 active seconds it should be near 14:00. Time is measured in foreground, independent of Time.timeScale. First/resume frame is skipped to avoid charging background time.
- Complete a car and press Next before closing time: old car-selection behavior remains, but clock must not reset. After reload exactly one live ShiftController remains under DontDestroyOnLoad and the new label uses it. Duplicate scene controller destroys only its own root. Do not attach controller to Canvas/GameManager.
- Alt-Tab or focus another application for 10 seconds, return to Game: time should not jump by those 10 seconds. Android browser/focus behavior requires a later on-device check. Existing audio is NOT yet coordinated by this clock.
- Exit and reenter Play: fresh clock. Fully stopping the app does not save clock progress. No PlayerPrefs calls added.
- Existing cleaning, HUD, sound and policy button must still work. Clock label raycast is off.
- Restart STILL reloads the current car and preserves clock, just like Next preserves clock. This temporary behavior is NOT the agreed final Restart. After closing time gameplay STILL runs: only the isolated clock ends. Do not publish this intermediate prototype or describe it as a completed shift.

After this substep passes, integrate counting, end-of-shift input/audio blocking, summary and Restart confirmation as the remainder of stage 1. Then completed-shift saving, random selection, third car and balancing follow the existing plan. No new feature is confirmed installed until user reports it.

## Recording results
User should report compilation, layout, Next continuity, one live controller, focus pause and stopping at 18:00. If an error occurs fix it before proceeding. Save and push only expected scene/meta changes on feature/timed-shifts after review; no main merge or AAB release now.
