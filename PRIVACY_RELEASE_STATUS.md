# Privacy release preparation - 2026-10-24

Base: d7996a4d3d4018380b9b748325d5da2ebe241737. This status supplements README.md and DEVELOPMENT_PLAN.md; their older next-step statements are superseded by the pending tasks here. No new build or network test has been performed by the assistant.

## Confirmed
- User installed the internal version 1 (1.0) from Google Play and reported that it works. This is not a measured network/privacy test or a newly enumerated full checklist.
- Play Console reports 16 KB support. The supplied AAB's seven arm64 ELF libraries also passed the static PT_LOAD alignment check. APK ZIP alignment and runtime testing on a 16 KB device were not performed here.
- App setup is 9/11 according to user: store listing, category/contact, app access, ads, content rating, intended audience 13+, government, finance and health declarations completed. Privacy policy and Data safety remain open.
- Console permissions: INTERNET, package-specific DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION, CHECK_LICENSE. The original AAB parser saw the first two uses-permission entries; the source of the added CHECK_LICENSE in Play-generated artifacts remains unverified.
- Latest commit updates banner/screenshots, archives old images, enables AAB, and adds Recorder 5.1.7 plus OpenImageIO. Recorder is an Editor tool, not a player recorder. Preserve user images.

## Prepared, NOT applied or tested
- PrivacyPolicyLink opens the planned Pages URL; it has not been verified live.
- Run Car Wash ASMR/Prepare Privacy Release ONCE with saved SampleScene outside Play Mode. It adds a bottom-right SafeArea button without moving existing UI, sets submitAnalytics to 0, and removes com.unity.modules.unityanalytics from manifest.json. Unity must regenerate packages-lock.json; do not hand-edit it. Existing analytics/diagnostics service flags remain disabled. This is a hardening change, not proof of no data collection.
- New scripts require compilation and device testing. Scene layout is based on serialized coordinates, not a visual test.
- docs/index.html is deliberately a DRAFT, not a completed compliance policy. Do not submit its URL or declare no collection yet.

## User steps after Pull
1. Let Unity compile. Open and save SampleScene; run the new privacy setup command once. Check Console and Git diff.
2. Check that only expected scene, PlayerSettings, manifest and lock changes occurred. Keep signing key settings and package name. Never upload keystore/passwords.
3. Configure repository Settings > Pages > Deploy from a branch > main > /docs; Save. Connector has no Pages administration tool. Publishing is a separate action. Verify the HTTPS URL without login; do not use a draft for Google Play.
4. Confirm policy button layout, browser opening, no dirt removal for gestures started on it, and return-to-game behavior. The page requires internet; gameplay should remain offline.
5. Choose a fresh Android Bundle Version Code greater than every uploaded code (at least 2 if 1 is the only used code); keep visible version 1.0 unless intentionally changing it. Build a signed non-development AAB with the existing upload key.
6. Record source commit, SHA-256, version code and build results. Test the new build; old build 1 is NOT covered by these pending changes. Review manifest, built libraries and runtime network behavior before finalizing the policy and Data safety. No claim that settings alone guarantee absence of traffic.
7. Resolve whether Google Play Games on PC should remain enabled; no PC compatibility is claimed.
8. Finalize policy text only against verified release behavior, then remove the draft notice, verify public URL, and complete policy/Data safety declarations. Do not promote to production merely to change the unreviewed store appearance.

## Limits and privacy
GitHub Pages logs visitors' IP addresses for security; website/browser processing must not be described as offline gameplay. No network capture, legal certification, full Git history secret scan, Unity run, or AAB signing validation was performed in this preparation. A local signing-key path is present in existing tracked settings; do not reproduce it in reports or replace key configuration blindly.
