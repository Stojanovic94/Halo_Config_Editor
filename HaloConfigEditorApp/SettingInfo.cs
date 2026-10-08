#nullable enable

namespace HaloConfigEditorApp;

/// <summary>
/// Long-form descriptions shown in the right-side info panel when the user
/// hovers over a setting. Keyed as "section.key" (case-insensitive).
/// </summary>
public static class SettingInfo
{
    public static readonly Dictionary<string, string> Entries = new(StringComparer.OrdinalIgnoreCase)
    {
        // ----- [display] -----
        ["display.fullscreen"] =
            "Where display.mode is empty: start borderless over the whole display; " +
            "false starts in a window.\n" +
            "F11 switches.",

        ["display.mode"] =
            "\"fullscreen\" — takes the display at display.resolution's mode.\n" +
            "\"borderless\" — a window over the whole desktop.\n" +
            "\"windowed\"   — a window at display.window_size.\n" +
            "\n" +
            "Empty: uses display.fullscreen's value (true = borderless).\n" +
            "F11 switches to the window and back.",

        ["display.resolution"] =
            "What fullscreen and borderless draw at:\n" +
            "\n" +
            "\"native\"             the display's own resolution\n" +
            "\"<width>x<height>\"    e.g. \"1920x1080\", 640x480 or more\n" +
            "\n" +
            "Fullscreen sets the display to it; borderless draws it scaled to the display.",

        ["display.resolution_scaling"] =
            "\"native\"   — draws at the window's resolution (fullscreen: the display's or display.resolution).\n" +
            "\"original\" — draws the Xbox's 640x480 and scales it up.",

        ["display.window_size"] =
            "The window's size, \"<width>x<height>\" (\"1920x1080\"), 640x480 or more.\n" +
            "The window can be resized.\n" +
            "\n" +
            "Empty: uses display.window_scale instead.",

        ["display.window_scale"] =
            "Where display.window_size is empty: the window's size as a multiple of 640x480.",

        ["display.vsync"] =
            "Wait for the display between frames; false draws as fast as possible.",

        ["display.max_fps"] =
            "With vsync off, the most frames a second:\n" +
            "   0   twice the display's refresh rate\n" +
            "  -1   no limit (can hang some Intel graphics)",

        ["display.interpolation"] =
            "Draw a frame for every display refresh, blending between the game's 30 ticks a second.\n" +
            "False keeps the original 30 frames a second.",

        ["display.direct_camera"] =
            "In first person, point the view where the player aims now instead of where the last tick left it.\n" +
            "The view turns the frame the mouse moves, not up to two ticks (66 ms) later.",

        ["display.high_res_hud"] =
            "Draw the HUD (meters, counters, panels, motion sensor, reticles, waypoints, scopes) from the high-res assets (8x the maps' bitmaps).\n" +
            "False draws the maps' own bitmaps.",

        ["display.high_res_text"] =
            "Draw the menus' and HUD's text with the fonts in port/assets/fonts (Overpass) at the resolution the game draws at, " +
            "and the menus' titles from port/assets/titles.\n" +
            "False draws the maps' bitmap fonts and titles.",

        ["display.menus"] =
            "\"pc\"    — the PC version's main menu (port/assets/menus, and a menus folder here for your own)\n" +
            "\"xbox\"  — the Xbox's menus",

        ["display.player_names"] =
            "In multiplayer, whose names are drawn above their heads:\n" +
            "\n" +
            "\"all\"      everyone\n" +
            "\"allies\"   teammates only\n" +
            "\"enemies\"  opponents only\n" +
            "\"none\"     nobody\n" +
            "\n" +
            "An enemy's name shows only within the motion sensor's reach, in sight and not camouflaged.\n" +
            "None show if the gametype's motion tracker shows no players; only allies' if it shows only friends.",

        ["display.player_name_scale"] =
            "How large the players' names are drawn.\n" +
            "1.0 is three quarters the size of the HUD's text. Range: 0.25 to 4.",

        ["display.scoreboard_team_layout"] =
            "How the scoreboard lists a team game's players:\n" +
            "\n" +
            "\"teams\"  a column for each team (red on the left, blue on the right)\n" +
            "\"score\"  all in order of score",

        ["display.scoreboard_background"] =
            "Draw a panel behind the multiplayer scoreboard, for clearer text.",

        ["display.scoreboard_background_color"] =
            "The scoreboard panel's colour: \"red, green, blue, alpha\", each 0 to 255.\n" +
            "Alpha 0 is see-through, 255 solid.",

        ["display.anti_aliasing"] =
            "Smoothing of jagged edges, which the Xbox did not have:\n" +
            "\n" +
            "  \"off\"     no smoothing\n" +
            "  \"fxaa\"    fast; smooths the 3D view once drawn\n" +
            "  \"smaa\"    similar to fxaa, usually sharper\n" +
            "  \"ssaa2x\"  draws at twice the resolution each way (4x GPU work)\n" +
            "  \"msaa2x\"  two samples per pixel\n" +
            "  \"msaa4x\"  four samples per pixel\n" +
            "  \"msaa8x\"  eight samples per pixel\n" +
            "\n" +
            "The HUD and menus stay sharp when fxaa/smaa is used.\n" +
            "Android treats fxaa as smaa, and has no ssaa2x.",

        ["display.shadow_resolution"] =
            "The size the objects' shadows are drawn at, in pixels each way:\n" +
            "128 as on the Xbox, or 256, 512 or 1024 for smoother edges, as soft.",

        ["display.per_pixel_lighting"] =
            "Light the models (characters, weapons, vehicles, scenery) for each pixel by the lights the game gives them, " +
            "without the facets the light of each vertex shows across curved surfaces.\n" +
            "False lights each vertex, as the Xbox does.",

        // ----- [audio] -----
        ["audio.enabled"] = "Play sound.",
        ["audio.volume"] = "The volume of everything, 0.0 to 1.0.",
        ["audio.music_volume"] = "The music's volume, 0.0 to 1.0 (of audio.volume).",
        ["audio.effects_volume"] = "The volume of every other sound (effects and speech), 0.0 to 1.0 (of audio.volume).",

        ["audio.reverb"] =
            "Reverberate the world's sounds as the place the player is in does (the maps' sound environments, as the Xbox's I3DL2 reverb did).\n" +
            "False keeps them dry.",

        ["audio.loose_sounds"] =
            "For those making sounds: play each of a map's sounds that has a Halo PC sound tag file of its name under the data root's tags folder (tags/sound/.../name.sound) from that file.\n" +
            "\n" +
            "At the console, loose_sounds_reload reads the files again and loose_sounds false gives the map's sounds back.",

        // ----- [input] -----
        ["input.mouse_sensitivity"] = "How far the view turns for the mouse's movement.",
        ["input.invert_mouse"] = "Moving the mouse forward looks down.",

        ["input.mouse_aim_assist"] =
            "Magnetism while aiming with the mouse, as with a controller: the view slowed and dragged along by a target.\n" +
            "The last of the mouse and the right stick to move decides.\n" +
            "The bullets' autoaim (bent toward the target) stays either way.",

        ["input.mouse_vertical_sensitivity"] =
            "How far the view turns up and down for the mouse's movement.\n" +
            "0 for the same as input.mouse_sensitivity.",

        // ----- [controls] -----
        ["controls.move_forward"] =
            "The keyboard and mouse's controls, which Settings > Controls Setup changes: up to two keys or buttons each, separated by a comma.\n" +
            "\n" +
            "Keys by their names (\"W\", \"Space\", \"Left Ctrl\", \"F1\"), and:\n" +
            "  \"Mouse Left\", \"Mouse Right\", \"Mouse Middle\"\n" +
            "  \"Mouse 4\", \"Mouse 5\"\n" +
            "  \"Wheel\" (either way), \"Wheel Up\", \"Wheel Down\"\n" +
            "\n" +
            "Empty for none.",

        ["controls.move_backward"] = "Moving backward.",
        ["controls.strafe_left"] = "Moving left.",
        ["controls.strafe_right"] = "Moving right.",
        ["controls.jump"] = "Jumping (and skipping cutscenes).",
        ["controls.crouch"] = "Crouching.",
        ["controls.fire"] = "Firing.",
        ["controls.throw_grenade"] = "Throwing a grenade.",
        ["controls.melee"] = "Melee attack.",
        ["controls.reload"] = "Reloading.",
        ["controls.zoom"] = "Zooming the scope.",
        ["controls.switch_weapon"] = "Switching weapons.",
        ["controls.switch_grenade"] = "Switching grenades.",

        ["controls.action"] =
            "The action: picking up (held: swapping weapons), entering and leaving vehicles, pressing switches.\n" +
            "Never reloading — the controller's X does when there is nothing to act on.",

        ["controls.flashlight"] = "The flashlight.",
        ["controls.scoreboard"] = "Showing the scores (the controller's Back).",
        ["controls.pause"] = "The pause menu (the controller's Start).",

        // ----- [game] -----
        ["game.console_log"] =
            "What the game's console shows on screen of what it logs:\n" +
            "\n" +
            "\"important\"  bans, players dropped for cheating, what refuses a command, and the asserts that stop the game\n" +
            "\"all\"        every line, the game's own chatter too\n" +
            "\"none\"       the asserts that stop the game only\n" +
            "\n" +
            "What a command prints shows whatever this is; debug.txt has every line.",

        ["game.language"] =
            "The language the game asks the Xbox for: \"ja\", \"de\", \"fr\", \"es\" or \"it\".\n" +
            "Empty for English. The game data decides what is translated.",

        ["game.custom_edition"] =
            "Load and run Halo Custom Edition maps (not those that need OpenSauce).\n" +
            "Put them and Custom Edition's bitmaps.map, sounds.map and loc.map in the custom_maps folder beside the maps folder.\n" +
            "The map lists show them as CUSTOM SINGLEPLAYER and CUSTOM MULTIPLAYER.\n" +
            "\n" +
            "Their tags are checked as the game's own maps' are before they run.\n" +
            "False refuses them (see docs/custom_edition_caches.md).",

        // ----- [paths] -----
        ["paths.data"] =
            "The folder holding the game data's maps folder.\n" +
            "Empty looks in the working directory and its assets folder.\n" +
            "\n" +
            "Windows paths are easiest in single quotes: 'C:\\Games\\Halo'.",

        ["paths.saves"] =
            "Where saved games and profiles go.\n" +
            "Empty for the usual place (~/.local/share/halo-linux, or %APPDATA%\\halo on Windows).",

        ["paths.custom_edition"] =
            "A Halo Custom Edition install whose maps folder is looked in after the custom_maps folder for Custom Edition maps " +
            "and their bitmaps.map, sounds.map and loc.map (see game.custom_edition).\n" +
            "Empty for none.",

        // ----- [network] -----
        ["network.address"] =
            "This machine's IPv4 address for system link, for a machine on several networks.\n" +
            "Empty chooses one.",

        ["network.broadcast"] =
            "Comma-separated IPv4 addresses system link sends its announcements to instead of the local network's broadcast address (for VPNs).\n" +
            "Empty for the local network.",

        ["network.online"] =
            "Internet play: hosting makes an invite link (logged, and put on the clipboard) that lets whoever has it join over the internet.\n" +
            "Opening a link (or copying one before switching to the game) joins. Only people with the invite can join.\n" +
            "Off keeps system link to the local network.",

        ["network.join_from_clipboard"] =
            "Join the game of an invite link found on the clipboard when the game comes to the front.",

        ["network.tunnel_port"] =
            "The UDP port internet play uses; 0 picks one.\n" +
            "A fixed one can be forwarded on the router, for networks whose NAT stops connections.",

        ["network.allow_upnp"] =
            "Let internet play ask the router (UPnP) to forward its port, for networks whose NAT stops connections:\n" +
            "when a player joins this machine's game, and when joining a game takes too long.\n" +
            "False never asks.",

        ["network.public_lobby"] =
            "The server browser: public games are listed through the signalling brokers, and Join Game > Server Browser shows them.\n" +
            "False lists no game of this machine's and shows none.",

        ["network.host_public"] =
            "Whether a new game of Create Game > Internet starts as:\n" +
            "\n" +
            "  PUBLIC   listed in everyone's server browser; anyone can see and join\n" +
            "  PRIVATE  only players with its invite link can join (false)\n" +
            "\n" +
            "Server Setup's LISTING changes it for each game.",

        ["network.coop_public"] =
            "Whether an online co-op game (Create Game > Internet, a SINGLEPLAYER map) starts as PUBLIC or, false, PRIVATE.\n" +
            "Server Setup's LISTING in co-op, which writes its choice here.",

        ["network.coop_friendly_fire"] =
            "Whether the players of an online co-op game hurt each other:\n" +
            "\n" +
            "\"off\"               no friendly fire\n" +
            "\"on\"                full friendly fire\n" +
            "\"shields_only\"      only shields take damage\n" +
            "\"explosives_only\"   only explosives hurt\n" +
            "\n" +
            "Server Setup's FRIENDLY FIRE in co-op writes its choice here.\n" +
            "Their AI allies they always can, as in the campaign.",

        ["network.coop_enemies_mode"] =
            "Online co-op's extra enemies:\n" +
            "\n" +
            "\"none\"         no extras\n" +
            "\"per_player\"   each squad grows by coop_enemies for each player past the first\n" +
            "\"multiplier\"   each is coop_enemies_multiplier times as large, for any number of players\n" +
            "\n" +
            "Server Setup's EXTRA ENEMIES in co-op writes its choice here.",

        ["network.coop_enemies"] =
            "Online co-op's extra enemies per player, a percentage: for each player past the first, " +
            "each squad of enemies a level places gets this much of itself more.\n" +
            "100 = as many again. Range: 25 to 200.\n" +
            "\n" +
            "Server Setup's PER PLAYER in co-op writes its choice here.",

        ["network.coop_enemies_multiplier"] =
            "Online co-op's static multiplier of its enemies: each squad of enemies a level places is this many times as large.\n" +
            "Range: 2 to 32.\n" +
            "\n" +
            "Server Setup's MULTIPLIER in co-op writes its choice here.",

        ["network.brokers_file"] =
            "The file of the public MQTT brokers through which the machines of an invite find each other " +
            "(its messages are encrypted), beside this file unless a full path.\n" +
            "One host:port on each line, up to 4.\n" +
            "\n" +
            "Updates replace brokers.txt: keep a list of your own under another name.",

        ["network.stun_servers"] =
            "Public STUN servers that tell this machine its internet address; comma-separated host:port.",

        ["network.coop_player_collisions"] =
            "Whether the players of an online co-op game bump into each other.\n" +
            "False, they walk through each other (the AI's characters they still bump into).\n" +
            "\n" +
            "Server Setup's PLAYER COLLISIONS in co-op writes its choice here.",

        // ----- [discord] -----
        ["discord.application_id"] =
            "The Discord application internet play invites go through while the Discord desktop client runs.\n" +
            "Empty for none.",

        // ----- [update] -----
        ["update.auto"] =
            "Look for a new version when the game starts, and offer to update to it.\n" +
            "False never looks (the game's \"Do not ask again\" writes false here).",

        // ----- [debug] -----
        ["debug.network_test"] =
            "Automated system link sessions for testing (port/linux/game/network_test.c):\n" +
            "\n" +
            "\"host:<map>\"  hosts a game on that map\n" +
            "\"join\"        joins the first game found\n" +
            "\n" +
            "Empty for none.",

        ["debug.network_test_start"] = "Seconds after hosting that an automated test game starts.",
        ["debug.network_test_kill"] = "Every this many seconds an automated test host kills its last player; 0 never.",

        ["debug.network_test_score"] =
            "The score an automated test host's game type plays to (a short game, to test the next).\n" +
            "0 = the game type's own.",

        ["debug.network_test_shoot"] =
            "Every this many seconds each automated test player hits the next with their weapon, within its reach.\n" +
            "The host brings far players near the first a second before.\n" +
            "0 never.",

        ["debug.network_test_vehicle"] =
            "This many seconds into an automated test game the host seats its last player as a vehicle's driver (and out 15 seconds on).\n" +
            "0 never.",

        ["debug.network_test_pickup"] =
            "This many seconds into an automated test game the host stands its last player on a weapon, which a joining player then picks up.\n" +
            "0 never.",

        ["debug.network_test_pickup_weapon"] =
            "The weapon network_test_pickup stands the player on: the first whose tag name has this in it (\"sniper\", say).\n" +
            "Empty for any.",

        ["debug.telnet_console"] =
            "Listen on 127.0.0.1 (port telnet_console_port) for a script console that runs what it is sent as the game's console does, with no password.\n" +
            "False for none.",

        ["debug.telnet_console_port"] =
            "The port of the script console (telnet_console).\n" +
            "The Xbox's was 23, which only the administrator can listen on.",

        ["debug.network_latency"] =
            "Milliseconds everything received is held back (a round trip between two machines of twice it), to test the netcode as over the internet.\n" +
            "0 for none.",

        ["debug.network_loss"] = "Percent of datagrams received that are dropped, for the same; 0 for none.",

        ["debug.test_input"] =
            "\"bot:<seed>\"   plays controller 1 with a scripted pattern (automated network tests)\n" +
            "\"look:<seed>\"  stands still, only turning and looking up and down\n" +
            "\n" +
            "Empty for none.",

        ["debug.update_answer"] =
            "The answer to the new version question, for automated tests:\n" +
            "\n" +
            "\"yes\"    update now\n" +
            "\"no\"     skip this time\n" +
            "\"never\"  do not ask again (confirmed)\n" +
            "\n" +
            "Empty asks.",

        ["debug.exit_after"] = "Quit this many seconds after the window opens; 0 never.",
        ["debug.hidden_window"] = "Keep the window hidden (and never fullscreen).",
        ["debug.null_renderer"] = "Run without a window, drawing nothing.",
        ["debug.gl_debug"] = "Report OpenGL errors in the log.",

        ["debug.menu_open"] =
            "Start on this screen of the menus (port/assets/menus) instead of the main menu, a player profile being edited.\n" +
            "Empty for the main menu.",

        ["debug.gpu_flush_draws"] =
            "Flush the GPU's pipeline every this many draws:\n" +
            "  -1   every 3 on Intel graphics with Mesa's driver (which can hang without)\n" +
            "   0   never",

        ["debug.gpu_stats"] = "Log the renderer's draw counts once a second.",
        ["debug.gpu_trace_frame"] = "Log every draw of this frame; -1 none.",
        ["debug.gpu_trace_constants"] = "With gpu_trace_frame, also the vertex shader constants.",
        ["debug.gpu_skip_vertex_shaders"] = "Comma-separated ids of vertex shaders not to draw with.",
        ["debug.gpu_dump_shaders"] = "A folder to write the generated GLSL to; empty none.",
        ["debug.gpu_debug_expression"] = "A GLSL expression every pixel shader shows instead of its result.",
        ["debug.gpu_debug_texture0"] = "Pixel shaders show their first texture.",
        ["debug.gpu_debug_flat"] = "Pixel shaders show their vertex colour.",
        ["debug.screenshot_directory"] = "A folder to save frames to (with screenshot_every); empty none.",
        ["debug.screenshot_every"] = "Save every this many frames to screenshot_directory; 0 none.",
        ["debug.texture_dump_directory"] = "A folder to write every texture to as it is uploaded; empty none.",
        ["debug.texture_log"] = "Log texture uploads.",
        ["debug.texture_no_cache"] = "Upload textures again every time they are used.",

        ["debug.network_corrupt"] =
            "Percent of the datagrams received that are damaged at random, to test that nothing a machine sends can crash the game.\n" +
            "0 for none.",

        ["debug.network_corrupt_stream"] =
            "Percent of the reads of streams that are damaged at random, for the same.\n" +
            "A damaged stream is closed, so a little goes a long way. 0 for none.",

        ["debug.network_corrupt_after"] =
            "Seconds after the start before anything is damaged, so that a game can be set up and started first.\n" +
            "A host's messages to its own client are damaged too.",

        // ----- [crash_reports] -----
        ["crash_reports.upload"] =
            "Send a report of each crash (a minidump and halo.log) to the developers' Sentry project (port/windows/src/win32_crash.c):\n" +
            "\n" +
            "\"yes\"    send them\n" +
            "\"no\"     never send\n" +
            "\"ask\"    ask at the next crash and write the answer here"
    };

    public static string Get(string section, string key)
    {
        string text;

        if (Entries.TryGetValue($"{section}.{key}", out var found) && !string.IsNullOrWhiteSpace(found))
        {
            text = found;
        }
        else
        {
            text = $"Edit the {key} setting for the {section} section.";
        }

        // Normalise every line break to CRLF. WinForms' TextBox uses the
        // underlying Win32 EDIT control, which reliably breaks lines only
        // on \r\n — a lone \n can be silently ignored.
        return text
            .Replace("\r\n", "\n")
            .Replace("\n", "\r\n");
    }
}