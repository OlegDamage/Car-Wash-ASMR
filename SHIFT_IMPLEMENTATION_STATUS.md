# Timed shifts — 2026-11-03

Working branch feature/timed-shifts; stable main / released 1.0 (2) unchanged. New source is not user-tested until explicitly confirmed. This status overrides obsolete first-car/hard-cutoff rules in older planning text.

## Confirmed history and scope
- e0abb38 clock; 06abce9 feedback; e0fef86 clock scene. User explicitly confirmed Next continuity, focus pause, one controller and accepted interim white clock.
- c397e5f counter, user reported all works. f2f43ad overtime/summary/restart, accepted after A–C/Restart instructions.
- 521115e scene only: counter, wired ShiftEndRoot, inactive modal Panel, 120 seconds, accepted clock preserved.
- 121681f completed-shift save source. User reported everything works after save-test instructions. Treat as user-confirmed Editor checks, not independent capture/device/error-injection tests. On 3 November at 17:38 user stated there are no local changes. No needless scene commit required.

## Existing shift/save rules retained
At 18:00 admit only same unfinished car with actual progress >=1%; clock frozen, no Next. Completion counts once and triggers summary/save automatically. Below threshold or already clean -> summary immediately. Restart confirms abandoning whole active shift; Cancel preserves it. Focus loss pauses work.

ShiftSave JSON key CarWashASMR.CompletedShiftStats.v1 stores completedShifts, totalCars (long), lastShiftCars, lastResultId/schema. Only naturally completed shifts saved, including zero-car shifts and final overtime car. No active-shift save, no old key deletion or sound reset. Result ID prevents normal duplicate completion/retry. Corrupt/unsupported data is not silently overwritten; Retry save retains session result and blocks advancing. PlayerPrefs is local/unencrypted, not an atomic transaction, cloud sync, anti-cheat or crash guarantee. Android process-restart behavior still needs its own test.

## Prepared next substep — random selection
- New stateless CarSelection.Choose(count, excludedIndex): equal choice among allowed array indices, no reroll loop. One car -> index 0; invalid exclusion -> all indices; empty catalogue rejected. Runtime pair validation remains in CarSequence.
- First machine of a fresh process is random. Next excludes the currently shown index. New shift/confirmed Restart excludes last shown index while controller stays alive. Reload of an already selected machine does not reroll it.
- CurrentCarIndex uses -1 only as an internal not-selected sentinel and is resolved before assigning sprites. lastShownCarIndex survives only in controller memory; not persisted. After a full process restart a previous session's final car can reappear.
- With TWO machines, nonrepeating Next must alternate; randomness becomes visible with a third. All available indices have equal conditional probability; no guaranteed equal counts in a short sample or complete catalogue cycle. Empty/incomplete pairs remain errors, not silently skipped.
- Source files changed: CarSequence and ShiftController. New helper/meta plus this status. No ShiftSave/schema, scene, Inspector references, art, timer balance, public policy, settings, SDK or main changes.
- Default first-car behavior changes: no longer always first catalogue entry. New shift/restart still reset time/current count, not saved totals. Last-car overtime cannot reroll or grant another car after 18:00.
- Assistant did not compile/run Unity or an Android build for this substep.

## Local application and checks
1. Fetch/Pull on feature/timed-shifts; wait for compilation. No Setup commands, new components or Inspector fields. Keep 120-second test duration and existing saves. Stop on errors.
2. Start Play: either accepted car may appear; several fresh launches may coincidentally start with same car. That alone is not a bug.
3. Wash and Next through several cars before closing: no immediate repeat. With current two cars, expect alternation from whichever started. Time/current count continue; one controller.
4. Restart with confirmation: new active shift, current count 0/time10:00, other car when two available, no saved-total increment. Cancel preserves same car/time/count.
5. Finish shift (including overtime) -> saved summary -> Next shift: other car when two available; prior result not credited twice. Stop/reenter Play: saved totals load, active shift resets, first car arbitrary.
6. Do not create placeholder duplicates as a fake third car and do not remove catalogue entries merely to simplify this test. Helper's one/zero-car cases need separate validation if exercised; do not claim them tested merely from source.
7. Check no regression in focus pause, overtime eligibility, UI, sound and saved totals. After third actual pair is added, test all three, dirt alignment and lack of adjacent repeats across transitions.

## Next
After random-selection checks pass, prepare one third clean/dirt image pair matching existing perspective/style; preserve accepted two pairs. Then balance using phone measurements, not five-machines assumption. New identified internal build, full Android regression/process-restart/network check, and release-specific policy/store review before external distribution.

Decorative wall-mounted digital clock remains a later agreed task using same time source. Money, shop, upgrades and new SDKs not implemented. No main merge or production release now.
