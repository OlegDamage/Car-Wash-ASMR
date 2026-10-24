# Privacy release status - 2026-10-24

This status supersedes outdated privacy next steps in README.md and DEVELOPMENT_PLAN.md. No production release is authorized or claimed here.

## Source changes and static review
- c247164 added privacy scripts, initial policy draft and this status.
- a0ae89c applied the setup: PrivacyPolicyButton with OpenPolicy listener added to SampleScene; submitAnalytics changed from 1 to 0; com.unity.modules.unityanalytics removed from manifest and lock. Only the four expected files changed.
- Reviewed all 12 files in Assets/Scripts. CarSequence saves CarWashASMR.CurrentCarIndex and SoundToggleUI saves CarWashASMR.SoundEnabled through PlayerPrefs. DirtCleaner processes input and texture data locally; partial cleaning is not persisted. PrivacyPolicyLink opens a fixed HTTPS URL without appending user or gameplay data. No gameplay network transmission calls found in these 12 files.
- Current manifest has no directly declared Unity Ads, UGS Analytics, IAP or Firebase package. Legacy analytics module is absent. Recorder 5.1.7 remains for Editor capture. UnityWebRequest modules remain available; their presence alone does not establish that requests occur.
- UnityConnectSettings: services, Insights/engine diagnostics, cloud crash reporting, purchasing, analytics, ads and performance reporting have disabled flags. Some serialized initialize-on-startup flags remain 1; they have NOT been changed or represented as 0.
- Root Assets listing has no top-level Plugins folder. This is not a recursive binary/plugin audit. GitHub code search returned incomplete results; conclusions above use direct file reads, not an empty search result.

## User-reported build and functional testing
- Android version 1.0, version code 2, AAB size 33.1 MB. User reports successful build without red Console errors.
- User reports Play Console accepted code 2 / version 1.0 without errors or warnings; release distributed through internal testing and installed as an update.
- User reports startup, cleaning, progress, Before/Next/Restart, sound, policy link, return from browser and offline gameplay work.
- User reports GitHub Pages is enabled and the policy page is accessible.

## User-reported network observations
- Tested installed Play build 1.0 (2) using PCAPdroid.
- Gameplay session: browser control traffic visible before AND after gameplay in the same capture; Car Wash ASMR connections 0; Unknown absent.
- Policy-link session: page opened, connections attributed only to Samsung Browser; Car Wash ASMR connections 0; Unknown absent.
- These are the user's reports, not assistant-run captures. No packet file or screenshots were independently inspected. Evidence supports no detected app connections in those scenarios, not a universal guarantee about all devices, delays or paths.

## Policy and Data safety
- docs/index.html now contains a final user-facing policy for Android 1.0 (2), replacing Draft. It distinguishes local gameplay, external browser/GitHub hosting, Android/Play processing and voluntary email support. Hosting IP logs are not described as offline gameplay.
- Recommended Data safety answer for this reviewed release: no collection or sharing of the required user data types by the app. Local-only processing is excluded under Google's definition. This recommendation is based on the reviewed source/settings and the user's bounded tests, not legal certification.
- Do not classify external-browser hosting as app telemetry solely because the policy button opens it. Reassess if user data is ever added to the link or an in-app webview/SDK is introduced.
- No in-game account creation: no account deletion flow to invent. Support email remains a separate voluntary contact channel.
- No independent security review/MASA certification has been performed; do not claim one.
- Data safety submission in Console has NOT been completed by the assistant. Internal-only apps are exempt; complete accurate disclosures before closed/open/production distribution. Reassess every release active outside internal testing; do not extend the code-2 conclusion automatically to older or future builds.

## Next tasks
1. After Pages deploys this commit, verify the public page no longer says Draft and that developer/contact information is correct.
2. Enter the final public policy URL in Play Console. Prepare Data safety answers for this release; save as draft and inspect the preview before submission. If Console asks additional questions, use their exact wording instead of guessing.
3. Decide whether Google Play Games on PC should remain enabled; no PC compatibility is claimed.
4. Check applicable account/testing/publication requirements separately before any production rollout. No extra AAB is required solely for this website/documentation change. Do not rerun Prepare Privacy Release.

## Historical findings and remaining limits
- Earlier build 1 (1.0) was installed successfully according to user; the privacy setup and new network test apply to build 2, not automatically to build 1.
- Earlier AAB: seven arm64 ELF libraries passed static PT_LOAD alignment check and Play reported 16 KB support. APK ZIP alignment and runtime testing on a 16 KB device were not performed. These earlier binary findings have not been revalidated on the new AAB.
- Earlier Console permissions: INTERNET, package-specific DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION and CHECK_LICENSE; the source of the added CHECK_LICENSE in Play-generated artifacts remains unverified. No new binary manifest analysis performed.
- Current AAB could not be uploaded to the assistant. Contents, signing identity, hash and built dependency tree were not independently checked. No full Git history secret scan or legal certification performed.
- Preserve user banners/screenshots and existing signing configuration. A local key path exists in tracked settings; do not reproduce it in reports, upload keys/passwords or change the signing identity blindly.
