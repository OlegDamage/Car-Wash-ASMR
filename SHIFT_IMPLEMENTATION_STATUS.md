# Timed shifts — 2026-11-03

Branch feature/timed-shifts only; main/released 1.0 (2) unchanged. This document overrides the obsolete hard cutoff in DEVELOPMENT_PLAN.md. Approved rule: admit the same unfinished car to overtime only if actual progress >=1% at closing; freeze clock at 18:00, count final car once, then end. No physical overtime limit.

## Accepted baseline and source
- e0abb38 clock; 06abce9 feedback; e0fef86 clock scene. User explicitly confirmed time continuity, pause, one controller and interim white label.
- c397e5f counter; user reported all works. f2f43ad added overtime, summary, whole-shift Restart/Cancel and next shift. User reported everything works after A–C and Restart checks; not independent assistant execution.
- 521115e changes ONLY SampleScene: counter, active ShiftEndRoot with assigned panel/message/buttons, inactive modal Panel, duration 120 seconds. Accepted clock preserved. Assistant read the diff; source correctness is not an independent runtime test.

## Current prepared substep: completed-shift saving
Not compiled, installed or tested by assistant. Files: new plain static ShiftSave service and meta; updated ShiftController and ShiftEndUI; this document. No component/setup/scene/settings/package/art/main/public-policy change required. Do not rerun Setup.

### Storage and behavior
- Single PlayerPrefs JSON key CarWashASMR.CompletedShiftStats.v1. Schema 1: completedShifts, totalCars (long), lastShiftCars, lastResultId. Result GUID is a local per-shift duplicate guard, not an account/advertising identifier and is never transmitted by this code.
- Save explicitly in FinishShift AFTER counting the final eligible car and BEFORE UI interaction. Zero-car naturally completed shifts count as completed shifts too. No writes for active progress, Next, focus loss, overtime entry, Cancel or abandoned Restart. Sound's existing independent saving remains.
- Every new/restarted shift gets a fresh in-memory result ID. IsResultSaved and lastResultId protect normal repeated completion/retry from double credit. Latest-result retry with conflicting count is rejected. Save includes one JSON snapshot, not a claim of an atomic filesystem transaction.
- No old counters existed: missing new key starts totals at zero in memory without writing. Old car-index key remains ignored in shift mode; no DeleteAll/DeleteKey, no sound reset. Unsupported/corrupt existing stats are reported, NOT silently reset or overwritten.
- On save failure, keep result in memory, show Retry save, block advancing until retry succeeds. Retry of a cached same-ID result flushes again without increment. This cannot guarantee recovery from sudden power loss, OS/storage failure, tampering or concurrent processes. PlayerPrefs is local, unencrypted, not cloud sync, not anti-cheat.
- On startup log loaded shifts/total/last to Console. On successful completion log saved values. Summary shows current cars, saved shift count and total cars; temporary session-only text removed. No new always-visible HUD added.

## Local application
1. Save scene, Fetch/Pull on feature/timed-shifts; wait for compile. No new command, Inspector fields or components. Existing ShiftEndRoot references are reused; no manual file recreation. Stop on errors.
2. Keep 120-second normal test. Short 15–30 seconds can be used for zero-car deadline tests; restore 120 outside Play and save afterward. Do not delete existing PlayerPrefs to simplify testing.

## Required tests, relative to loaded baseline
- Note startup Console line: shifts=S, total cars=T, last=L. Missing-key first run should show zero. This is Editor storage; Android must be tested separately.
- Finish a shift with N cars including eligible overtime car: summary saved shifts=S+1, total=T+N. Wait/return from background: unchanged. Save happens even WITHOUT Next shift.
- Stop Play on that summary, reenter: loaded line must retain S+1/T+N/last=N; active shift starts 10:00/current count 0.
- Wash a car in the NEW shift but quit BEFORE closure: on next launch saved totals/last must be unchanged; unfinished time/cleaning/current count lost. Same rule while still finishing last car after 18:00.
- Start another shift, Restart and confirm before completion: no extra completed shift, no total increment. Cancel also leaves totals unchanged. Sound preference must survive.
- Complete a subsequent zero-car shift: saved shifts increments by one, total stays unchanged, last becomes 0. Next shift itself adds nothing. Wait/rapid button interactions must not credit the old result twice.
- Regression: overtime eligibility, exact-once count, Next restrictions, summary/restart, focus pause, one controller and normal sound still work.
- If Retry save or red save/load error appears, report it; do not delete or manually edit stored data. Error/corruption handling and crash durability have NOT been experimentally verified by this commit.

## Remaining work
After explicit acceptance, update status and review expected local scene changes if test duration changed. Then random cars/third agreed pair and balance, followed by new identified internal Android build and regression including real process restart/network checks. Save tests in Editor do not prove Android durability.

Decorative wall-mounted digital clock stays planned after core systems: same time source, no automatic paid art/fonts. No economy/upgrades/ads/SDK added. Public policy and store text still describe 1.0 (2); before distributing a new binary, revise scope/local-statistics description and reassess Data safety. No main merge/production release now.
