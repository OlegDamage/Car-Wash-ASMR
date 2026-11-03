# Timed shifts — 2026-11-03

Branch feature/timed-shifts only; main/released 1.0 (2) unchanged. This status OVERRIDES the old hard cutoff in DEVELOPMENT_PLAN.md: user approved a final eligible car after closing on 3 November. No completed-shift saving yet.

## Confirmed history
Clock source e0abb38; feedback 06abce9; scene e0fef86 with white clock at (0,-190), 10–18, 60-second test. User explicitly confirmed Next continuity, focus pause, one controller and accepted appearance. Counter source c397e5f; user said everything works after counter instructions, without an independently supplied capture or a newly enumerated device test. Counter scene changes remain local until user pushes; latest remote commit checked was c397e5f. Assistant has not compiled or executed Unity.

## Approved rule replacing hard cutoff
At closing, snapshot ACTUAL CleaningProgress >= 0.01f on the unfinished current car, not rounded HUD percent. Qualifying car enters FinishingLastCar; clock stays at 18:00 indefinitely and only that same scene car can be completed. Next is prohibited. A completed current car is counted once and the shift ends immediately; an unfinished car below 1% does not count and shift ends immediately. Cannot earn eligibility by starting after closing. Final car is counted automatically on completion and summary follows without Next. No real-world overtime limit; frozen game clock does not limit physical session duration.

## Prepared code — untested until user confirms
- ShiftController: Working / FinishingLastCar / Finished; identity-bound last car, count, focus/input pause, immediate audio stop at blocking points, summary state and restart confirmation. Scene reload during overtime does not grant eligibility to another instance.
- CarSequence: refuses Next after admission closes; uses in-memory CurrentCarIndex with controller. No old car-index PlayerPrefs writes during shift mode. Legacy fallback only when no controller. Old keys not deleted; sound saving unchanged. New shifts start at first car; random selection still later.
- LevelRestart: Restart requests whole-shift confirmation; Next uses separate ReloadForNextCar. Cancel resumes same time/car/count; confirm resets all in-memory shift progress and reloads. Restart during overtime abandons the whole old shift rather than swapping the last car.
- ShiftClockUI: time plus Finish this car / Shift ended / Paused; accepted white color and position stay in scene. ShiftCounterUI unchanged.
- ShiftEndUI/ShiftEndSetup: modal summary and restart confirmation, built from existing TMP font without new art. Only new root is added locally by setup. No existing UI moved; no direct remote scene modification.
- Source change does not include DirtCleaner, CleaningAudio, BeforeAfterPreview, packages, settings, public policy, store assets or main. Existing audio stops when controller disables cleaning; extra immediate stop is applied.

## Apply locally
1. Save local scene and Fetch/Pull on feature/timed-shifts. Stop on compilation errors. Do not discard local counter objects.
2. Outside Play, open/save SampleScene. Run ONLY Car Wash ASMR > Setup Shift End UI once. It requires existing clock/counter/controller. Save. New SafeArea/ShiftEndRoot contains inactive Panel; ShiftEndUI must stay on the active root outside Panel. All serialized fields wired by setup. Do not rerun clock/counter setup.
3. Keep 120 seconds for normal tests. For quick deadline cases use 15–30 seconds outside Play; restore 120 afterward. No final balance claimed.

## Required tests
- 0% at closing -> immediate summary with no extra car; input/sound stop, Next inaccessible.
- Some progress but actual <1% at closing -> same. Rounded percent alone is not an exact threshold test.
- Clearly >1% and unfinished before closing -> Finish this car, time frozen, clean/sound continue; no Next. Finish -> exactly +1, summary automatically. Wait -> no double count.
- 97% before closing -> can finish; fully clean before closing -> counted once, summary at closing, not an extra entitlement.
- Next before closing to a fresh 0% car -> fresh car does not qualify. No cleaning after closing may turn an ineligible car into an eligible one.
- Restart before closing AND during overtime: confirmation pauses clock/cleaning; cancel restores same session; confirm resets time/count/car. No extra machine in same overtime shift.
- Next shift on summary -> new 10:00/count 0/first car, one controller, no stuck disabled cleaner. Rapid clicks do not create multiple reloads.
- Focus/browser pause, UI gesture exclusion, sound setting, Before, counter continuity and one controller remain correct. In Finished cleaner is disabled so Before/Next hide via existing checks.
- Summary is session-only: no completed totals or money written. Closing app/Play loses unsaved shift. Public policy/store must be reviewed for a future release; do not publish this development branch.

## Next
Fix errors before adding features. After explicit acceptance, push local scene and review. Then completed-shift save/migration, random selection, third car, balancing and internal regression. Decorative wall-mounted digital clock remains planned after core systems, using SAME time source, no automatic paid assets/fonts. No production or merge into main here.
