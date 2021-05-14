using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;
using HarmonyLib;

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

    struct StateKeeper
    {
        public EnumMouseButton Button;
        public bool Left;
        public bool Right;
    }

    [HarmonyPatch(typeof(ClientMain))]
    [HarmonyPatch("OnMouseDown")]
    class MouseDownPatch
    {
        static void Prefix(out StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Save mouse state before original method call
            __state = new StateKeeper
            {
                Left = ___InWorldMouseState.Left,
                Right = ___InWorldMouseState.Right,
            };
        }

        static void Postfix(MouseEvent args, StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Revert mouse state to the way it was before the call
            ___InWorldMouseState.Right = __state.Right;
            ___InWorldMouseState.Left = __state.Left;

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
    [HarmonyPatch("OnMouseUp")]
    class MouseUpPatch
    {
        static void Prefix(out StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Save mouse state before original method call
            __state = new StateKeeper
            {
                Left = ___InWorldMouseState.Left, 
                Right = ___InWorldMouseState.Right,
            };
        }

        static void Postfix(MouseEvent args, StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            // Revert mouse state to the way it was before the call
            ___InWorldMouseState.Right = __state.Right;
            ___InWorldMouseState.Left = __state.Left;

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