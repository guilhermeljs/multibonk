# TODO

## Steam tunneling
- [x] Wire the `SteamTunnelService.RegisterEndpoint` API into the Steam callbacks so tunnel invitations actually populate the queue; right now nothing ever enqueues endpoints, so overlay joins can't work end-to-end.
- [x] Listen for new assembly loads and retry locating `SteamFriends` so the overlay hooks survive late Steamworks initialization instead of only probing during startup.
- [x] Consume accepted tunnel endpoints automatically and initiate a connection from the Connection screen so players do not have to press Connect after joining via overlay.

## UI polish
- [x] Surface lobby join failures and server-creation errors inside the UI rather than silently snapping back to the connection screen.
- [x] Expose the listen port separately from the address so hosts can communicate connection info without editing the combined endpoint field.
- [x] Cache GUIStyle instances in `OptionsWindow` to cut down on per-frame allocations once the layout stabilizes.

## Gameplay rules
- [x] Validate on the gameplay layer that the preference flags (PvP, revive, loot distribution) propagate to the actual game systems; the UI updates the preferences, but the runtime hooks have not been audited yet.
