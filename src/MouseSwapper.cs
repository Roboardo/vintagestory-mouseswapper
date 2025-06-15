using Vintagestory.API.Client;
using Vintagestory.API.Common;
using System;
using HarmonyLib;
using Vintagestory.Client.NoObf;

namespace MouseSwapper
{
    class MouseSwapper : ModSystem
    {
        // Prevent double patching and unpatching
        static bool mousePatched = false;
        public override void Start(ICoreAPI api)
        {
            if (!mousePatched)
            {
                mousePatched = true;
                System.Diagnostics.Debug.WriteLine("MouseSwapper: Patching methods");
                HarmonyPatcher.Patch();
            }
        }

        public override void Dispose()
        {
            if (mousePatched)
            {
                mousePatched = false;
                System.Diagnostics.Debug.WriteLine("MouseSwapper: Unpatching methods");
                HarmonyPatcher.Unpatch();
            }
        }
    }


    public class HarmonyPatcher
    {
        private static string harmonyId = "vsmods.patch.mouseswapper";
        private static Harmony harmony = new Harmony(harmonyId);
        public static void Patch()
        {
            harmony.PatchAll();
        }

        public static void Unpatch()
        {
            harmony.UnpatchAll(harmonyId);
        }
    }

    [HarmonyPatch(typeof(ClientMain))]
    [HarmonyPatch("OnMouseDownRaw")]
    class MouseDownPatch
    {
        static void Prefix(out int __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Save mouse state before original method call
            __state = (1 & Convert.ToInt16(___InWorldMouseState.Left)) |
                      (2 & Convert.ToInt16(___InWorldMouseState.Right));
        }

        static void Postfix(MouseEvent args, int __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Make sure to drop handled events
            if (args.Handled) return;

            // Revert mouse state to the way it was before the call
            ___InWorldMouseState.Left = Convert.ToBoolean(__state & 1);
            ___InWorldMouseState.Right = Convert.ToBoolean(__state & 2);

            // Flip mouse keys
            switch (args.Button)
            {
                case EnumMouseButton.Left:
                    ___InWorldMouseState.Right = true;
                    break;
                case EnumMouseButton.Right:
                    ___InWorldMouseState.Left = true;
                    break;
            }
        }
    }


    [HarmonyPatch(typeof(ClientMain))]
    [HarmonyPatch("OnMouseUpRaw")]
    class MouseUpPatch
    {
        static void Prefix(out int __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Save mouse state before original method call
            __state = (1 & Convert.ToInt16(___InWorldMouseState.Left)) |
                      (2 & Convert.ToInt16(___InWorldMouseState.Right));
        }

        static void Postfix(MouseEvent args, int __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Revert mouse state to the way it was before the call
            ___InWorldMouseState.Left = Convert.ToBoolean(__state & 1);
            ___InWorldMouseState.Right = Convert.ToBoolean(__state & 2);

            // Flip mouse keys
            switch (args.Button)
            {
                case EnumMouseButton.Left:
                    ___InWorldMouseState.Right = false;
                    break;
                case EnumMouseButton.Right:
                    ___InWorldMouseState.Left = false;
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(DrawWorldInteractionUtil))]
    [HarmonyPatch("drawHelp")]
    class DrawHelpPatch
    {
        static void Prefix(ref WorldInteraction wi, out EnumMouseButton __state)
        {
            // Save initial state (because the draw call can happen with the same WorldInterraction multiple times)
            __state = wi.MouseButton;

            // Switch mouse buttons
            switch (wi.MouseButton)
            {
                case EnumMouseButton.Left:
                    wi.MouseButton = EnumMouseButton.Right;
                    break;
                case EnumMouseButton.Right:
                    wi.MouseButton = EnumMouseButton.Left;
                    break;
            }
        }

        static void Postfix(ref WorldInteraction wi, EnumMouseButton __state)
        {
            // Restore original state
            wi.MouseButton = __state;
        }
    }
}
