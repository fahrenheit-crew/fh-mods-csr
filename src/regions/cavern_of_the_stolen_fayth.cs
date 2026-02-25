// SPDX-License-Identifier: MIT

using Fahrenheit.Core.Atel;
using System;
using System.Collections.Generic;
using System.Text;
using static Fahrenheit.Core.FhUtil;

namespace Fahrenheit.Mods.CSR {
    internal unsafe static partial class Removers {
        private static class CavernOfTheStolenFayth {
            public static void remove_stolen_fayth(byte* code_ptr) {
                remove(code_ptr, 0x6355, 0x68D2);
                Globals.save_data->progression_flags_calm_lands_quest.set_bit(0, true);
            }
        }
    }
}
