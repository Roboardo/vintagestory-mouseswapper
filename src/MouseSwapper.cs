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


    [HarmonyPatch(typeof(ClientMain))]
    [HarmonyPatch("OnMouseDown")]
    class MouseDownPatch
    {
        static void Prefix(MouseEvent args, out MouseEvent __state)
        {
            __state = args;
        }

        static void Postfix(MouseEvent __state, ref MouseButtonState ___InWorldMouseState)
        {
            ___InWorldMouseState.Left = false;
            ___InWorldMouseState.Right = false;
            
            if (__state.Button == EnumMouseButton.Left)
                ___InWorldMouseState.Right = true;

            if (__state.Button == EnumMouseButton.Right)
                ___InWorldMouseState.Left = true;
        }
    }


    [HarmonyPatch(typeof(ClientMain))]
    [HarmonyPatch("OnMouseUp")]
    class MouseUpPatch
    {
        struct StateKeeper {
            public EnumMouseButton Button;
            public bool Left;
            public bool Right;
        }

        static void Prefix(MouseEvent args, out StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            __state = new StateKeeper
            {
                Button = args.Button, 
                Left = ___InWorldMouseState.Left, 
                Right = ___InWorldMouseState.Right,
            };
        }

        static void Postfix(StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            ___InWorldMouseState.Right = __state.Right;
            ___InWorldMouseState.Left = __state.Left;

            if (__state.Button == EnumMouseButton.Left)
                ___InWorldMouseState.Right = false;

            if (__state.Button == EnumMouseButton.Right)
                ___InWorldMouseState.Left = false;
        }
    }
}