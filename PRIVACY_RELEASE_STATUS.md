# Privacy and publication status — 2026-10-25

Current game: Android 1.0 (2). Future timed shifts are approved design only; see DEVELOPMENT_PLAN.md. No production release is claimed.

## Completed source changes
- c247164: privacy scripts and draft. a0ae89c: button/listener in SampleScene, submitAnalytics 1 -> 0, unityanalytics removed from manifest and lock.
- 7a49414: AndroidBundleVersionCode 1 -> 2 only. eead606: final policy for 1.0 (2) and audit status.
- Earlier review covered the 12 Assets/Scripts files: local input/cleaning, PlayerPrefs car index and sound, fixed external-browser policy URL without user/gameplay parameters. No gameplay transmission calls found in those files.
- Manifest has no directly declared UGS Analytics, Ads, IAP or Firebase package; legacy analytics absent. Recorder 5.1.7 is retained for Editor capture. UnityWebRequest modules remain; availability is not evidence of use.
- UnityConnectSettings disables services, Insights/engine diagnostics, cloud crash reporting, purchasing, analytics, ads and performance reporting. Some initialize-on-startup flags remain 1; they were not reported as 0.
- Root Assets had no top-level Plugins folder. This was not a recursive binary/dependency audit. GitHub code search was incomplete; empty search was not evidence of absence.

## User reports — build and tests, 24 October
- AAB 33.1 MB, version 1.0/code 2; successful build without red errors. Console accepted it without errors/warnings. Internal release installed as an update.
- Startup, both cars, cleaning/progress, Before/Next/Restart, sound, offline, policy link and browser return work. Device per prior user report: Samsung Galaxy A57/Android 17; not independently queried.
- PCAPdroid gameplay capture: browser traffic visible before and after in same session; app 0 connections; no Unknown.
- Separate policy-link capture: page opened, traffic only Samsung Browser; app 0; no Unknown.
- These are user reports. Assistant did not inspect capture files/screenshots or run device tests. No detected connections in bounded scenarios is not a universal guarantee.

## Completed Console setup — user reports
- Final Pages policy is accessible and Draft disappeared. URL: https://olegdamage.github.io/Car-Wash-ASMR/ .
- User saved policy URL and Data safety. Preview: data not collected; data not shared. Counter reached 11/11 on 24 October.
- Saved declarations are NOT evidence of Google approval, review completion, public availability or production access. Assistant did not submit the Console form.
- Policy distinguishes local gameplay, external browser/GitHub processing, Android/Play processing and voluntary email support. No account creation flow. No MASA/independent security certification claimed.
- Answer no collection/sharing was recommended for reviewed build 2, not automatically older/future releases. Review every active release outside internal testing and all SDK behavior.

## Production access — screenshot, 25 October
Dashboard requires a published closed-test release, at least 12 participants and at least 14 days. Screenshot shows 0 participants and disabled production application. No completed closed test or production eligibility established. Invitations alone are not joined testers. Recheck official requirements before launch; do not promise automatic approval.

User has not found 12 participants. Mass recruitment/closed-test launch is deferred while adding a small timed-shift loop. Existing internal test remains the stable baseline; no instruction here ends it or promotes any release. Countries and tester list for closed testing are not confirmed configured.

## Upcoming shifts — not yet implemented
Future progress will store completed-shift totals instead of current car selection on every Next; sound remains independently saved. Active shift/partial cleaning is memory-only, lost after process termination. This differs from build-2 policy's local car-index description.

Do NOT change docs/index.html to claim shifts exist now. After implementation, align policy/version scope, local-data/retention text, Data safety and store materials with the actual release. A local-only change does not by itself establish network collection; repeat targeted review/tests instead of assuming unchanged behavior. No new SDK/online services planned. New binaries require a fresh unused version code and their own checks.

## Remaining publication work
- Revisit closed-test countries, real participants, suitable feedback process and release setup after shift prototype.
- Check Publishing overview for pending changes before any submission; do not assume saving 11/11 submitted all changes.
- Decide Google Play Games on PC participation separately; no PC compatibility verified.
- Recheck account/production requirements before release. No release date promised.

## Historical and independent-verification limits
- Earlier build 1 was user-tested; new privacy setup/network observations belong to build 2.
- Earlier AAB: seven arm64 ELF libraries passed static PT_LOAD alignment; Play reported 16 KB support. ZIP alignment/runtime 16 KB testing were not performed. Not revalidated on new AAB.
- Earlier permissions: INTERNET, package-specific DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION, CHECK_LICENSE. Added CHECK_LICENSE source in Play-generated artifacts remains unresolved; no new manifest analysis.
- Current AAB could not be supplied: no independent binary/dependency/signature/hash review or exact source-to-artifact verification. No legal certification, full Git-history secret scan, FPS or memory measurement.
- Preserve existing signing identity and user art. Never upload keys/passwords or reproduce the local key path. Documentation changes alone require no new AAB.
