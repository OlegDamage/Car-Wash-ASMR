# Timed shifts — implementation status

Updated: 2026-11-03, feature/timed-shifts. Design: DEVELOPMENT_PLAN.md. Stable main / released 1.0 (2) unchanged. This file refines stage 1, not the agreed scope.

## Clock — user-confirmed substep
- e0abb38 introduced ShiftController, ShiftClockUI and ShiftClockSetup. 06abce9 recorded visual feedback and future wall-clock design.
- User accepted the interim white label below the hint. In response to the three explicit checks, user reported everything works: time continuity through Next, focus pause and one live controller after reload. No independent Unity/device execution by assistant; environment of this particular check not newly specified.
- e0fef86 committed SampleScene: separate root ShiftController, 10–18 hours, 60-second test; clock at (0,-190), white, size 620x64/font 36, raycast off. Diff also changes one TMP style hash; no old gameplay code changed. This is confirmed source configuration, not an artifact hash.
- Old clock setup still creates initial dark text at -240. Do NOT rerun it over accepted UI.

## Prepared next substep — car count, NOT tested/installed yet
- ShiftController keeps CarsWashed in memory, rebinds the single scene DirtCleaner after Single reload, and reads its existing IsCompleted in LateUpdate after cleaning Update. Current car is counted once per scene instance without waiting for Next; no new DirtCleaner event or duplicate cleaning logic. This is a small polling adapter using existing state, not the final end/save system.
- Clock Update runs before gameplay Update. At/after clock closure no further cars are counted; already counted cars remain. A finish arriving in the closing frame is excluded. Count survives Next scene reload with the live controller; leaving Play/process resets it.
- ShiftCounterUI displays Cars washed: N. Editor-only ShiftCounterSetup creates the label under SafeArea without moving clock/other UI.
- Commit does not modify SampleScene, DirtCleaner, CarSequence, LevelRestart, old saves, packages, settings, public policy, store assets or main. Unity compile/layout/device tests remain pending.

## Local setup and tests
1. On feature/timed-shifts, Fetch/Pull. Stop on red compilation errors. Outside Play, open and save SampleScene. Run ONLY Car Wash ASMR > Setup Shift Counter Prototype once, then save. No Inspector references need dragging. Existing clock/controller stay in place.
2. Counter defaults: bottom centre (0,260), 620x64, font 36, white, Raycast Target off. Layout not independently tested; inspect on the accepted portrait view and report overlap. Do not redraw garage or rerun old setup commands.
3. Temporarily use Shift Duration Seconds 120 outside Play so two car washes fit comfortably; this is a test value, not balance. Save scene. Expected start Cars washed: 0. Complete first car BEFORE 18:00: becomes 1 without Next. Wait/hold Before: stays 1. Next: stays 1 and clock continues. Complete second car: becomes 2.
4. Let time expire before cleaning another car; finish it AFTER 18:00: count must stay frozen. Gameplay is deliberately not blocked yet. Boundary and focus behavior must be tested, not assumed from code.
5. Exit/reenter Play: count/time reset; sound setting remains as before. Check clock, focus pause, one live controller, cleaning, sound, HUD and policy still work. Do not change script execution-order settings.
6. Existing Restart temporarily repeats a car without resetting time/count and can therefore count a newly washed scene instance again. Do not use Restart as a shift feature or publish this prototype. Its whole-shift confirmation/reset is still required in stage 1. Existing CarSequence still writes old car-index PlayerPrefs; removing that belongs to the upcoming coordinated transition, not a completed new save system.

## Future decorative clock — user requested, not implemented
Wall-mounted digital clock housing in the garage; digits use SAME ShiftController time. Agree art/typeface later without automatic paid assets/SDKs. Preserve accepted background/UI. Account for camera crop, safe area and portrait sizes; no cleaning input interception. Implement after core shift count/end/summary/save work. Interim white label remains.

## Remaining work
After counter checks pass: end-of-shift input/audio blocking, summary and confirmed whole-shift Restart. Then completed-shift saves/migration, random selection, third car, balance and full internal regression. No earnings yet. Core-shift closure is not complete just because clock/count freeze.

Record explicit user results only. Review/push expected local scene changes on this branch; no main merge, AAB release or production rollout now.
