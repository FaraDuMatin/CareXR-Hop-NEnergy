# GUIDE - Unity + Meta XR SDK Setup

This guide is based on the project README and gives you an exact, step-by-step setup flow, including everything you need to download.

## Goal
Set up a Unity project configured for Meta XR SDK (Quest/OpenXR), either by opening this ready-made project or by recreating it from scratch.

## What you need to download

### Required
1. This repository (ZIP download from GitHub).
2. Unity Hub.
3. Unity Editor version required by this project (or another version if you accept possible compatibility issues).
4. Unity modules during Editor install:
   - Microsoft Visual Studio Community 2022
   - Android Build Support

### Required inside Unity (package installs)
1. Meta XR SDK package:
   - Package name: com.meta.xr.sdk.all
2. XR Plugin Management (installed from Project Settings when prompted).

### Optional (only for PCVR development)
1. Meta Horizon Link desktop application.

## Path A (recommended): Open this project directly
Use this path if you want the fastest setup and avoid manual project configuration.

### Step 1 - Download project files
1. Open the repository page on GitHub.
2. Click Code > Download ZIP.
3. Extract the ZIP to a local folder.

### Step 2 - Install Unity Hub
1. Go to the Unity download page: https://unity.com/download
2. Download Unity Hub.
3. Install Unity Hub.

### Step 3 - Add this project in Unity Hub
1. Open Unity Hub.
2. Click Add project from disk.
3. Select the unzipped project folder.
4. Confirm the project appears in your Unity Hub project list.

### Step 4 - Install the correct Unity Editor version
1. Click the project in Unity Hub.
2. If Unity shows MISSING VERSION, select the suggested version and install it.
3. During installation, select these components:
   - Microsoft Visual Studio Community 2022
   - Android Build Support
4. Finish installation.
5. Open the project again.

### Step 5 - Let Unity finish importing
1. Wait for package import and project initialization.
2. If Unity asks to restart the editor, accept and restart.
3. Wait until the editor fully loads without pending setup prompts.

You are now ready to develop with Meta XR SDK in this project.

## Path B: Recreate a Meta XR project from scratch
Use this path if you want to build the same setup manually.

### Step 1 - Create a new Unity project
1. In Unity Hub, go to Projects.
2. Click New Project.
3. Choose:
   - Template: Universal 3D
   - Desired Unity Editor version
   - Project name and location
4. Leave Use Unity Version Control unchecked.
5. Click Create project.

### Step 2 - Install Meta XR SDK package
1. In Unity, open Window > Package Management > Package Manager.
2. Click the + button.
3. Select Install package by name.
4. Enter package name: com.meta.xr.sdk.all
5. (Optional) Specify an explicit version. If omitted, latest is installed.
6. Click Install.
7. Wait for installation to complete.
8. If prompted, click Restart Editor.

### Step 3 - Install and configure OpenXR support
1. Open Edit > Project Settings.
2. Go to XR Plugin Management.
3. Click Install XR Plugin Management when prompted.
4. Enable OpenXR for the active platform.
5. When prompted, click Yes to enable Meta Quest features.
6. Repeat OpenXR setup for Android configuration as well.

### Step 4 - Fix Meta XR project checks
1. Open the Meta XR section in Project Settings.
2. Click Fix All until all checks are green.
3. Repeat for Android-related checks until green.

### Step 5 - Scene bootstrap
1. In your scene, delete Main Camera if you are going to use Meta rig.
2. Use Meta XR Tools > Building Blocks.
3. Add the camera rig/building blocks needed for your app.

Your manually created project is now configured for Meta XR development.

## PCVR setup (optional)
Only required if you want to run in PCVR mode.

### Step 1 - Download Meta Horizon Link
1. Download and install Meta Horizon Link from Meta support pages.

### Step 2 - Configure OpenXR runtime
1. Open Meta Horizon Link.
2. Go to Settings > General.
3. Set Meta Horizon Link as the active OpenXR Runtime.

### Step 3 - Enable developer options
1. In Meta Horizon Link, open the Developer tab.
2. Enable the required developer settings shown there.

## Quick validation checklist
- Unity Hub installed.
- Project added in Unity Hub.
- Correct Unity Editor installed.
- Visual Studio 2022 component installed.
- Android Build Support installed.
- Meta package com.meta.xr.sdk.all installed.
- XR Plugin Management installed.
- OpenXR enabled for desktop and Android.
- Meta XR checks show green.
- (Optional) Meta Horizon Link configured for PCVR.

## Reference
This guide is derived from README.md in this repository and follows the same setup flow with expanded detail.