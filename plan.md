# YellowPoint Plan

## Goal
Create a lightweight Windows 11 tray app that draws a filled yellow circle around the mouse pointer (with opacity), while keeping the normal pointer active. Toggle highlight on/off with Ctrl+Alt+Y. Allow the user to change size and opacity.

## Features
- Tray icon with context menu:
  - Toggle highlight
  - Settings
  - Exit
- Filled circle overlay that follows the cursor.
- Global hotkey: Ctrl+Alt+Y.
- Settings persisted in AppData (JSON):
  - Diameter (px)
  - Opacity (0.1–1.0)
  - Color (default yellow)

## Implementation Steps
1. Rename default Form1 to `MainForm` and keep it hidden/minimized.
2. Add `OverlayForm`:
   - Borderless, topmost, click-through, no taskbar.
   - Paint a filled circle using `Graphics.FillEllipse`.
   - Update opacity and size from settings.
3. Add `CursorTracker` (timer-based) to move overlay to cursor position.
4. Add `HotkeyManager` using `RegisterHotKey` and `UnregisterHotKey`.
5. Add `SettingsService` to load/save JSON in AppData.
6. Add `SettingsForm` with diameter/opacity controls.
7. Wire tray menu + hotkey to toggle overlay visibility.

## Defaults
- Diameter: 40 px
- Opacity: 0.5
- Color: #FFD700 (yellow)
- Hotkey: Ctrl+Alt+Y
