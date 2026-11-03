# Timed shifts — implementation status

Updated: 2026-11-03, feature/timed-shifts. Design: DEVELOPMENT_PLAN.md. Stable baseline: main / 1.0 (2). This branch-specific status refines the first stage; future features are not installed merely because they are documented.

## Prepared source and local application
- e0abb38 added ShiftController, ShiftClockUI, Editor-only ShiftClockSetup, metas and this document. No existing gameplay script, scene, package, policy, store description or ProjectSettings was changed by that commit.
- User fetched the branch and applied the setup locally, as evidenced by their report and screenshot of the clock. No assistant-run Unity compilation or device test is claimed.
- User said the clock works; screenshot shows 18:00 | Clock ended. This confirms reported operation and visible end display, NOT an explicitly completed checklist for Next continuity, pause, duplicate prevention or Android behavior. Those remain to verify.

## Current visual issue and small local fix
- Screenshot shows dark clock text crossing the bright wall lamp; poor contrast/background competition, not established evidence of incorrect Canvas sorting. Do not change sorting or redraw the garage based on this screenshot.
- User requests slightly higher placement directly below the hint and white text.
- Outside Play Mode select Canvas/SafeArea/ShiftClock. Initial proposed RectTransform values: Pos X 0, Pos Y -190 (was -240); preserve anchors, pivot, width 620 and height 64. Set TextMeshProUGUI Vertex Color to opaque white (#FFFFFFFF); keep font size 36 and Raycast Target disabled. This placement is an unverified starting point for the shown layout; inspect all hint states and adjust only this label if needed. Save scene.
- Changes to the scene/appearance are NOT yet confirmed applied or pushed. Source setup still creates the old dark label at -240; do NOT rerun setup to apply this fix. Existing label is edited in Inspector.

## Future presentation — user requested, not implemented
Add a decorative wall-mounted digital clock in the garage, with the digits visually inside its housing rather than a floating HUD label. It must display the SAME ShiftController time, not run a second timer. Agree housing/art and digital typeface when this visual task starts; do not add paid assets, fonts or SDKs automatically. Keep the current garage and accepted UI; assess position/scale alongside camera crop, safe area and supported portrait formats. The clock must not intercept cleaning input.

Implement after core shift counting, end/summary and save behavior work. No new decorative art or digital font is created by this documentation change. Current white text is an interim readability fix.

## Remaining checks for clock substep
- Outside Play Mode: root ShiftController Start Hour 10, End Hour 18, Shift Duration Seconds 60 for short test (default/future balance hypothesis 300). Do not attach controller to Canvas/GameManager.
- Enter Play and focus Game: start 10:00, around 14:00 after 30 foreground seconds, stop at 18:00 around 60 seconds. First/resume frame skipped, time independent of Time.timeScale.
- Complete a car and press Next: clock must continue rather than reset. Exactly one live ShiftController under DontDestroyOnLoad after reload.
- Switch to another application for 10 seconds: after return clock must not jump by background duration. Android browser/focus still requires a later on-device test.
- Exit/reenter Play: fresh clock. Existing cleaning, progress, sound and policy must continue working. No new PlayerPrefs calls.
- Existing audio is NOT yet coordinated with clock pause. Restart temporarily repeats the current car without resetting clock. After 18:00 only clock stops; gameplay continues. No count, shift summary, shift save, earnings, random choice or third car yet. Do not publish this intermediate prototype.

## Next work
Finish visual fix and outstanding clock checks before adding functionality. Then complete stage 1: machine count, end-of-shift input/audio blocking, summary and confirmed full-shift Restart. Completed-shift saving, random choice, third car and balance follow DEVELOPMENT_PLAN.md.

Record only explicit user results. Save/push expected scene/meta changes on feature/timed-shifts after review. No merge into main, production rollout or AAB release now. Do not rerun old Setup commands.
