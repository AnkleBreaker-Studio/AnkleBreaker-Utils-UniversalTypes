# Changelog

## [1.2.1] - 2026-10-22

### Fixed

- I2 Localization assembly setup no longer prompts on editor startup or domain reload. Create the assemblies explicitly from AnkleBreaker > UniversalTypes > Create I2L Assembly Definitions.

### Added

- EditMode regression tests for loading the setup without prompts or file writes and for creating both assemblies through the menu.

## [1.2.0] - 2026-04-01

### Changed

- **DefineManager**: Replace local `UniversalTypesDefineManager` with declarative `[assembly: ABDefine(...)]` attributes from AnkleBreaker.Core
- Add `com.anklebreaker-studio.core` (>= 1.1.0) as dependency
- Add `AnkleBreaker.Core.Editor` assembly reference in Editor asmdef

### Added

- `Editor/AssemblyInfo.cs` — Declares `AB_WWISE`, `AB_FMOD`, `AB_I2_LOCALIZE` via `[assembly: ABDefine(...)]`

### Removed

- `Editor/DefineManager/UniversalTypesDefineManager.cs` — Replaced by Core's `ABDefineManager`

## [1.1.0] - 2026-03-04

### Changed

- **UniversalString**: I2L localization now uses `I2.Loc.LocalizedString` directly instead of a plain string term with reflection
- **UniversalString**: `ToString()` supports both I2L and Unity Localization simultaneously with I2L priority
- **UniversalStringDrawer**: Localized mode now displays I2L's native term picker (popup + RTL mask) instead of a raw text field

### Added

- `UniversalString` constructors: `(string)`, `(I2.Loc.LocalizedString)`, `(UnityEngine.Localization.LocalizedString)`
- `implicit operator UniversalString(string)` — allows `UniversalString us = "hello";`
- `I2LocAsmdefSetup` editor script — auto-detects I2L without asmdef and offers to create `I2.Loc.asmdef` + `I2.Loc.Editor.asmdef`
- Menu item: **AnkleBreaker > UniversalTypes > Create I2L Assembly Definitions**
- Assembly references to `I2.Loc` and `Unity.Localization` in both Runtime and Editor asmdefs

### Removed

- Reflection-based I2L translation resolution (replaced by direct `LocalizedString.ToString()`)

## [1.0.0] - 2026-03-03

### Added

- `UniversalString` — Inspector-friendly string that supports PlainText, i2Localize term, or Unity Localization (table + entry key)
- `UniversalAsset<T>` — Generic asset reference supporting Direct reference or Addressable key
- `UniversalSprite` — Convenience alias for `UniversalAsset<Sprite>`
- `UniversalSound` — Inspector-friendly sound reference supporting AudioClip, Wwise event name, or FMOD event path
- Custom PropertyDrawers for all types with mode dropdown and contextual fields
- `versionDefines` in asmdef for auto-detection of Unity Localization and Addressables packages
