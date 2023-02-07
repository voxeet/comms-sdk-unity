# Dolby.io Virtual World plugin for Unity

With the Dolby.io Virtual World plugin for Unity, you can easily integrate Dolby.io Spatial Audio, powered by Dolby Atmos technology into your virtual world applications.

You can find the plugin documentation here: [Online documentation](https://api-references.dolby.io/comms-sdk-dotnet/documentation/unity/getting-started/installation.html)

### Prerequisites
- Apple MacOS x64 or Microsoft Windows 10+ x64.
- A Dolby.io account. If you do not have an account, you can [sign up](https://dolby.io/signup) for a free account.
- The [client access token](https://docs.dolby.io/communications-apis/docs/overview-developer-tools#client-access-token) copied from the Dolby.io dashboard. To create the token, log into the [Dolby.io dashboard](https://dashboard.dolby.io/), create an application, and navigate to the API keys section.
- Install `git-lfs` command line tool on your computer.

### How to install
You can install the plugin from the Unity Package Manager.

Open the Package Manager from the Unity Editor and click the ➕ icon in the upper left corner.

**Option 1. Install from GitHub**

Select **Add package from git URL..**, enter the URL below
```
https://github.com/DolbyIO/comms-sdk-unity.git
```

**Option 2. Install from the downloaded package**

- Download latest plugin from the [release](https://github.com/DolbyIO/comms-sdk-dotnet/releases) page.
- Unzip the package into a local folder.
- Select **Add package from the disk**, navigate to the local folder and select `package.json` file. 

> ⓘ On MacOS, it is necessary to unquarantine SDK libraries. Otherwise, quarantine attributes prevent their usage. The simplest way to unquarantine the SDK library is to strip the quarantine attributes recursively for all the files in the package. Follow these steps:
>1. In the Package Manager, select the plugin, right click and click on **Reveal in finder**
>2. In the finder window, click on the **Settings** icon and pick **New Terminal Tab at Folder**
>2. Run this command: 
>```
>xattr -d -r com.apple.quarantine .
>```