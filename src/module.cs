global using Fahrenheit.FFX;

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Fahrenheit.Mods.CSR;

[FhLoad(FhGameId.FFX)]
public unsafe class CutsceneRemoverModule : FhModule {
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AtelEventSetUp(uint event_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate char* AtelGetEventName(uint event_id);

    public static char* get_event_name(uint event_id)
        => FhUtil.get_fptr<AtelGetEventName>(0x4796e0)(event_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate uint FUN_0086e990(nint signal_info_ptr);

    private FhMethodHandle<FUN_0086e990> _work_debug;
    private FhMethodHandle<AtelEventSetUp> _csr_event;

    public delegate void CsrEvent(byte* code_ptr);

    public static readonly Dictionary<string, CsrEvent> removers = new();

    public CutsceneRemoverModule() {
        _work_debug = new(this, "FFX.exe", 0x46e990, work_debug);
        _csr_event = new(this, "FFX.exe", 0x472e90, csr_event);
    }

    public override bool init(FhModContext mod_context, FileStream global_state_file) {
        Removers.init();

        return _csr_event.hook()
            && _work_debug.hook();
    }

    private uint work_debug(nint signal_info_ptr) {
        uint ret = _work_debug.orig_fptr(signal_info_ptr);

        add_work(*(short*)(signal_info_ptr + 0x4), *(short*)(signal_info_ptr + 0x10));

        return ret;
    }

    private static void add_work(short work_id, short entry_id) {
        // Was it already executed recently?
        for (int i = 0; i < rew.Count; i++) {
            var o = rew[i];
            if (o.work_id == work_id && o.entry_id == entry_id) {
                o.last_updated = 0;
                rew[i] = o;
                return;
            }
        }

        // Can we just add?
        if (rew.Count < REW_MAX) {
            rew.Add(new DisplayObject { work_id = work_id, entry_id = entry_id, last_updated = 0 });
            return;
        }

        // Find oldest work to replace
        int max = rew[0].last_updated;
        int longest_idx = 0;
        for (int i = 0; i < rew.Count; i++) {
            var o = rew[i];
            if (o.last_updated > max) {
                max = o.last_updated;
                longest_idx = i;
            }
        }

        rew[longest_idx] = new DisplayObject { work_id = work_id, entry_id = entry_id, last_updated = 0 };
    }

    private struct DisplayObject {
        public short work_id;
        public short entry_id;
        public int last_updated;
    }

    //TODO: Change whatever this was to use ImGui
    private const int REW_MAX = 42;
    private static List<DisplayObject> rew = new(REW_MAX);
    // private static byte[] colors = new byte[] { 0xE0, 0xB0, 0xA0 };
    // public override void render_game() {
    //     int x = 430;
    //     int y = 5;
    //
    //     draw_text(0, FhCharset.Us.to_bytes("CSR is running!"), x, y, color: 0x00, 0, scale: 0.5f, 0);
    //     y += 10;
    //
    //     if (Globals.actors != null) {
    //         Actor* tidus = Globals.actors;
    //         List<byte> tidus_pos = new();
    //         tidus_pos.AddRange(FhCharset.Us.to_bytes($"X: {tidus->chr_pos_vec.x:f4},"));
    //         tidus_pos.Add(0x2);
    //         tidus_pos.AddRange(FhCharset.Us.to_bytes($"Y: {tidus->chr_pos_vec.y:f4},"));
    //         tidus_pos.Add(0x2);
    //         tidus_pos.AddRange(FhCharset.Us.to_bytes($"Z: {tidus->chr_pos_vec.z:f4},"));
    //         draw_text(0, tidus_pos.ToArray(), x, y, color: 0x00, 0, scale: 0.5f, 0);
    //         y += 30;
    //     }
    //
    //     string event_name = Marshal.PtrToStringAnsi((nint)get_event_name(*(uint*)Globals.event_id))!;
    //     draw_text(0, FhCharset.Us.to_bytes($"Event: {event_name}"), x, y, color: 0x00, 0, scale: 0.5f, 0);
    //     y += 10;
    //
    //     draw_text(0, FhCharset.Us.to_bytes($"Map|Spawn: {Globals.save_data->current_room_id}|{Globals.save_data->current_spawnpoint}"),
    //               x, y, color: 0x00, 0, scale: 0.5f, 0);
    //     y += 10;
    //
    //     draw_text(0, FhCharset.Us.to_bytes($"Story Progress: {Globals.save_data->story_progress}"), x, y, color: 0x00, 0, scale: 0.5f, 0);
    //     y += 10;
    //
    //     List<byte> works = new();
    //     works.AddRange(FhCharset.Us.to_bytes("Recent Signal Targets:"));
    //
    //     works.Add(0x2);
    //
    //     for (int i = 0; i < rew.Count; i++) {
    //         var o = rew[i];
    //         if (o.last_updated < 12) {
    //             works.Add(0xA);
    //             works.Add(colors[o.last_updated / 4]);
    //         }
    //
    //         works.AddRange(FhCharset.Us.to_bytes($"{o.work_id:X2}:{o.entry_id:X2}"));
    //
    //         works.Add(0xA);
    //         works.Add(0x41);
    //
    //         works.AddRange(FhCharset.Us.to_bytes(", "));
    //
    //         if (i % 3 == 2) works.Add(0x2);
    //     }
    //
    //     draw_text(0, works.ToArray(), x, y, color: 0x00, 0, scale: 0.5f, 0);
    // }

    public override void post_update() {
        for (int i = 0; i < rew.Count; i++) {
            var o = rew[i];
            o.last_updated++;
            rew[i] = o;
        }
    }

    public void csr_event(uint event_id) {
        _csr_event.orig_fptr(event_id);

        rew.Clear();

        string event_name = Marshal.PtrToStringAnsi((nint)get_event_name(event_id))!;
        if (removers.TryGetValue(event_name, out CsrEvent? remover)) {
            _logger.Info($"Remover available for event \"{event_name}\"! Removing cutscenes...");
            byte* code_ptr = Globals.Atel.controllers[0].worker(0)->code_ptr;
            remover(code_ptr);
        } else {
            _logger.Info($"Remover not available for event \"{event_name}\".");
        }
    }
}
