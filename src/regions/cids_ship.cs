// SPDX-License-Identifier: MIT

using Fahrenheit.Core.Atel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fahrenheit.Mods.CSR {
    internal unsafe static partial class Removers {
        private static class CidsShip {
            public static void remove_underwater_ruins(byte* code_ptr) {
                // Remove the initial cutscene of entering the ruins
                remove(code_ptr, 0x3671, 3578);
                set(code_ptr, 0x3671, [
                    AtelOp.PUSHII.build(0x4C),
                    AtelOp.POPV.build(0x0)
                ]); // Set the story progress to the expected value

                remove(code_ptr, 0x36B4, 0x36BA); // Remove a fade from black
            }
        }
    }
}
