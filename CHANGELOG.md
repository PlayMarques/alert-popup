# Changelog

## 2.0.0

### Added
- Added localization support directly to `AlertPopup.Show`.
- Added `AlertPopup.SetInputValue(string value, bool notify = false)` to update popup input text programmatically.

### Changed
- Unified Alert Popup and Alert Popup Localization into a single package at the repository root.
- Moved `Editor`, `Runtime`, and `package.json` to the root package structure.
- Updated the Alert Popup prefab to keep `LocalizeStringEvent` references in the unified popup.
- Updated the creation menu to initialize the localization table and default string references.

### Removed
- Removed the separate `com.playmarques.alert-popup-localization` package.
- Removed the `AlertPopupLocalized` API.
