// SPDX-License-Identifier: MIT

namespace Fahrenheit.Mods.CSR; 
internal unsafe static partial class Removers {
    private static class DreamZanarkand {
        public static void remove_intro(byte* code_ptr) {
            // We basically write our own script here
             set(code_ptr, 0x3DC4, [
                 AtelOp.PUSHV.build(0x0),
                 AtelOp.PUSHII.build(0x2),
                 AtelOp.LS.build(),           // if (GameMoment < 2)
                 AtelOp.POPXCJMP.build(0x13), // goto intro_exit
                                              // else
                 AtelOp.JMP.build(0xE),       // goto midgame_exit
                 AtelOp.RET.build()           // return; // not actually necessary?
             ]);

            // Set the story progress higher than normal to skip the prelude camera pan
            set(code_ptr, 0x4450, AtelOp.PUSHII.build(3)); // GameMoment = 2 -> 3

            // Remove some hardcoded wait time
            remove(code_ptr, 0x43D3, 0x440A); // midgame
            remove(code_ptr, 0x4426, 0x4450); // intro
        }

        public static void remove_prelude(byte* code_ptr) {
            // The below code *works*, but it causes the game to think Tidus needs constant affirmation of his name
            // No, little child, I have a whale to sashimi
            remove(code_ptr, 0x6A35, 0x6A47); // Skips until running w00e10 (in w1Dtalk)
            remove(code_ptr, 0x6A55, 0x6A96); // Skips until running w00e10 (in w1Dtalk)
            remove(code_ptr, 0x6AA8, 0x6AE8); // Skips after running w00e10 (in w1Dtalk)
            remove(code_ptr, 0x18D, 0x23A);             // Skips until naming Tidus (in w00e10)
            set(code_ptr, 0x240, AtelOp.RET.build()); // Skips after naming Tidus (in w00e10)

            // Begone, thot
            remove(code_ptr, 0x6B3C, 0x6B48); // Begone, thot
            remove(code_ptr, 0x6B56, 0x6BE3); // Begone, thot
            remove(code_ptr, 0x6BE9, 0x6BEF); // Begone, thot
            remove(code_ptr, 0x6C06, 0x6C11); // Begone, thot
            remove(code_ptr, 0x6C1C, 0x6C40); // Skips after running w00e12 (in w1Etalk)
            remove(code_ptr, 0x48E, 0x54A);             // Skips until naming Tidus (in w00e12)
            set(code_ptr, 0x550, AtelOp.RET.build()); // Skips after naming Tidus (in w00e12)

            set(code_ptr, 0x6CD5, AtelOp.PUSHII.build(0x5)); // GameMoment = 4 -> 5 to skip overpass camera pan
            remove(code_ptr, 0x6CDB, 0x6CF2); // Skip fade out and waits when going to next map
        }

        public static void remove_overpass(byte* code_ptr) {
            ////TODO: Optionally skip girl's dialogue and instantly get 2 Potions

            //// Remove some extra waiting
            remove(code_ptr, 0x292A, 0x2933);

            //// Gotta do some manual sound stuffs
            set(code_ptr, 0x2809, AtelOp.JMP.build(0x2)); // jump our new code

            byte* ptr = code_ptr + 0x2810;

            //// BGM
            ptr = set(ptr, 0, [
                AtelOp.PUSHII.build(0xA1),
                AtelOp.CALLPOPA.build(0x102)
            ]); // call Common.setBgmToLoad(0xA1);

            ptr = set(ptr, 0, [
                AtelOp.CALLPOPA.build(0x105)
                ]); // call Common.loadBgm();

            ptr = set(ptr, 0, [
                AtelOp.PUSHII.build(0xA1),
                AtelOp.CALLPOPA.build(0x104)
            ]); // call Common.playBgm(0xA1);

            //// Zanar
            ptr = set(ptr, 0, [
                AtelOp.PUSHII.build(0x1),
                AtelOp.PUSHII.build(0x1B),
                AtelOp.PUSHII.build(0x6),
                AtelOp.REQ.build(),
                AtelOp.POPA.build()
            ]); // run w1Be06 (Level 1);

            ptr = set(ptr, 0, AtelOp.JMP.build(0x3)); // go back to the game's code
        }

        public static void remove_stadium(byte* code_ptr) {
            remove(code_ptr, 0x1AAF6, 0x1AED0);

            //TODO: Optionally remove the crowd's interruptions
        }

        public static void remove_stadium_attacked(byte* code_ptr) {
            // 00EF   AF0D00 AF0F00 AF0E00 D82601   call Common.warpToPoint [0126h](x=12.424045 [4146C8E3h], y=-40.966152 [C223DD57h], z=805.10956 [44494703h]);
        }
    }
}
