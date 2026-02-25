// SPDX-License-Identifier: MIT

using Fahrenheit.Atel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fahrenheit.Mods.CSR {
    internal unsafe static partial class Removers {
        private static class Besaid {
            public static void remove_valley(byte* code_ptr) {
                remove(code_ptr, 0x1B29, 0x1C43); // Wakka pushes Tidus into the water

                remove(code_ptr, 0x1CFC, 0x1FCF); // Wakka asks Tidus to join the Aurochs

                // Skip the Promontory since it'd be instantly skipped anyway
                // We essentially copy bsil0600:3EB3..3EC8 to bsil0300:1FCF..1FEA
                set(code_ptr, 0x1FCF, AtelOp.PUSHII.build(0x7E)); // GameMoment = 124 -> GameMoment = 126
                set(code_ptr, 0x1FD5, AtelOp.PUSHII.build(0x0F)); // Common.00BB(0) -> Common.00BB(15)
                set(code_ptr, 0x1FE1, AtelOp.PUSHII.build(69));
                set(code_ptr, 0x1FE7, AtelOp.CALL.build(0x11)); // Common.010C(67, 0) -> Common.transitionToMap(69, 0)

                remove(code_ptr, 0x3398, 0x33C6); // Initial fadeout into cutscene
            }

            public static void remove_promontory(byte* code_ptr) {
                remove(code_ptr, 0x3DB4, 0x3EAD); // Cutscene coming from Valley DEPRECATED
            }

            public static void remove_village_slope(byte* code_ptr) {
                remove(code_ptr, 0x264D, 0x289A); // First cutscene

                remove(code_ptr, 0x28B7, 0x28C0); // Don't make the game wait for a fade that won't happen

                set_tp(code_ptr, 0x264D, 0xD, 0xE, 0xF); // Set player position to the vanilla post-cutscene one
            }
        }
    }
}
