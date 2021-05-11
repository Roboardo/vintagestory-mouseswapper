using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;
using HarmonyLib;

namespace MouseSwapper
{
    class MouseSwapper : ModSystem
    {
        public override void StartClientSide(ICoreClientAPI api)
        {
            Patcher.DoPatching();
            System.Diagnostics.Debug.WriteLine("I like 4 limbed dragons");
        }
    }


    public class Patcher
    {
        // make sure DoPatching() is called at start either by
        // the mod loader or by your injector

        public static void DoPatching()
        {
            var harmony = new Harmony("vsmods.patch.mouseswapper");
            harmony.PatchAll();
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