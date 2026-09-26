# &#127758; Refresh 

A second-generation custom server for LittleBigPlanet that focuses on ease of use and reliability. &#127758;&#127918;

[![Discord](https://img.shields.io/discord/1049223665243389953?label=Discord)](https://discord.gg/xN5yKdxmWG)

<p align="center">
  <img width="600" src="https://github.com/LittleBigRefresh/Branding/blob/main/logos/refresh_type_transparent.png">
</p>

## &#128187; Running 

### &#128220; Legalities 
> [!WARNING]
> While Refresh is stable and mostly secure in our testing, we cannot make any guarantees about anything. You use Refresh at YOUR OWN RISK.
> Refresh is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
> See the [GNU Affero General Public License](https://github.com/LittleBigRefresh/Refresh/blob/main/LICENSE) for more details.

> [!NOTE]
> Refresh is free software: you can redistribute it and/or modify it under the terms of the [GNU Affero General Public License](https://github.com/LittleBigRefresh/Refresh/blob/main/LICENSE) as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Anyway, with the legal disclaimers out of the way...

### &#128214; Instructions 

#### From Release 
1. [Find the latest release](https://github.com/LittleBigRefresh/Refresh/releases/latest) 
1. Download the artifact for your OS, extract it somewhere most convenient to you, and run it! 
1. Optionally, run through configuring `bunkum.json` and `refreshGameServer.json` to your liking. These files contain settings like instance name, announce text, whether or not registration is enabled, and more. 

To update, you can simply repeat this process, overwriting the previous files.

#### Using Docker (compose) 
1. Check out this repository and install Docker with Compose.
1. Run `docker compose up --build -d` from the repository directory. Compose starts both PostgreSQL and the game server and keeps their data in persistent volumes.
1. Check `docker compose logs -f gameserver` and `http://localhost:10061/_health` before connecting a patched game client.

To update, you simply run a `git pull` to pull the latest changes,
and then run `docker compose up --build` to rebuild the image.

If you would like refresh-web, head to [here](https://github.com/LittleBigRefresh/refresh-web/actions) to view the latest artifacts, then grab them.
Once you've downloaded the artifact, browse to your data folder and create a folder called 'web' and extract the zip you've just downloaded to that folder.

### Archived and user-made levels

`AllowUserMadeLevels` in `refreshGameServer.json` defaults to `true`. Set it to `false` to stop new level publishing and republishing. Existing levels remain playable. This is a server setting; refresh-web is a separate project.

The Docker Compose setup connects to the [2023 dry archive](https://archive.org/details/dry23db) on a fresh installation. It sets `PREFRESH_DRY_ARCHIVE_ENABLED=true` when the initial `dry.json` is created. An existing `dry.json` with `Enabled: false` keeps that setting until changed. Its relevant options are:

```json
{
  "Enabled": true,
  "RemoteEnabled": true,
  "RemoteBaseUrl": "https://archive.org/download/"
}
```

The server first checks the local `Location` for each resource and then fetches missing SHA-1 assets from the archive's `dry23r*` bundles. Frequently requested assets are cached in the writable server data store. Players can use the existing `POST /api/v3/levels/hash/{hash}/setAsOverride` route with an archive root hash to open a level in game. This connection serves resources by hash; browsing the archive's `dry.db` metadata is not yet integrated into level search.

Levels marked as reuploads or with `[archive]` or `(archive)` in the title are offered to the game as unlocked and copyable. Authenticated users can also create an independently owned copy with `POST /api/v3/levels/id/{id}/fork` while user-made levels are enabled. The archived original retains its publisher and attribution.

The level copy flag does not alter sharing restrictions inside LittleBigPlanet's prize item assets. Those assets require separate format-aware processing before copy-locked goodies can be reused freely.

## &#128293; It's on fire! What do I do? 
Refresh isn't perfect, so it's not exactly uncommon to run into bugs. If you'd like, you can [create an issue](https://github.com/LittleBigRefresh/Refresh/issues/new/choose) here on GitHub or join our [Discord](https://discord.gg/xN5yKdxmWG) for support. 

Wherever you choose to post, be sure to include details about how to trigger the bug, text logs (not screenshots!), your environment, the bug's symptoms, and anything else you might find relevant to the bug. 

When dealing with authentication problems, it can be particularly helpful to check your user's notifications (the bell on the web interface will take you there) as authentication errors are logged here. 

## &#128295; Building & Contributing 
To contribute to Refresh, it may be helpful to refer to our [contributing guide](CONTRIBUTING.md) to get a basic development environment set up. If you're a pro, feel free to skip this as it's just your bog-standard setting up C# guide. 

However, something important for all those involved: we also serve additional documentation relating to Refresh, Bunkum, and LittleBigPlanet in general in our [Docs repo](https://littlebigrefresh.github.io/Docs/).

*Made with* &#128153; *for the LittleBigPlanet community*
