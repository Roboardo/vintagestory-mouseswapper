using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.Client.NoObf;
using HarmonyLib;
using static HarmonyLib.AccessTools;

namespace mouseswapper
{
    class MouseSwapper : ModSystem
    {
        ICoreClientAPI _clientApi;
        public override void StartClientSide(ICoreClientAPI api)
        {
            _clientApi = api;
            MyPatcher.DoPatching();
            System.Diagnostics.Debug.WriteLine("I like 4 limbed dragons");
        }
    }

    public class MyPatcher
    {
        // make sure DoPatching() is called at start either by
        // the mod loader or by your injector

        public static void DoPatching()
        {
            var harmony = new Harmony("com.example.patch");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(ClientMain))]
    [HarmonyPatch("OnMouseDown")]
    class Patch01
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
            {
                ___InWorldMouseState.Right = true;
            }
            if (__state.Button == EnumMouseButton.Right)
            {
                ___InWorldMouseState.Left = true;
            }
        }
    }

    class StateKeeper {
        public MouseEvent mouseEvent;
        public bool Left;
        public bool Right;
    }

    [HarmonyPatch(typeof(ClientMain))]
    [HarmonyPatch("OnMouseUp")]
    class Patch02
    {
        static void Prefix(MouseEvent args, out StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            StateKeeper state = new StateKeeper();
            state.mouseEvent = args;
            state.Left = ___InWorldMouseState.Left;
            state.Right = ___InWorldMouseState.Right;
            __state = state;
        }
        static void Postfix(StateKeeper __state, ref MouseButtonState ___InWorldMouseState)
        {
            ___InWorldMouseState.Right = __state.Right;
            ___InWorldMouseState.Left = __state.Left;

            if (__state.mouseEvent.Button == EnumMouseButton.Left)
            {
                ___InWorldMouseState.Right = false;
            }
            
            if (__state.mouseEvent.Button == EnumMouseButton.Right)
            {
                ___InWorldMouseState.Left = false;
            }
        }
    }
}