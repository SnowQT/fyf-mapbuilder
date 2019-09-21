using System.Threading.Tasks;
using System.Collections.Generic;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;

namespace FYF.MapBuilder.Client
{
    internal delegate void InputKeyCallback(int pressedTime);
    internal delegate void InputMouseCallback(Vector2 mouseDelta);
    //internal delegate void InputMouseButtonCallback

    internal enum InputKeyType
    {
        Continuous,
        Once,
    }

    internal class InputKeyState
    {
        public int KeyGroup;
        public int KeyCode;
        public InputKeyType KeyType;
        public bool PressedState;
        public int TimePressed;
        public HashSet<InputKeyCallback> Callbacks;

        private bool isDisabled;
        public bool IsDisabled
        {
            get
            {
                return isDisabled;
            }

            set
            {
                isDisabled = value;

                //Reset the key from being disabled.
                if (isDisabled == false)
                {
                    EnableControlAction(KeyGroup, KeyCode, true);
                }
            }
        }

        public InputKeyState(int keyGroup, int keyCode, InputKeyType keyType, bool disabled)
        {
            KeyGroup = keyGroup;
            KeyCode = keyCode;
            KeyType = keyType;
            IsDisabled = disabled;
            TimePressed = -1;
            Callbacks = new HashSet<InputKeyCallback>();
        }

        public void AddCallback(InputKeyCallback callback)
        {
            Callbacks.Add(callback);
        }

        public bool Update()
        {
            bool isPressed = false;

            //@TODO: A cleaner way to do this would be nice.
            //Determine which function to use for the pressed keys.
            if (IsDisabled && KeyType == InputKeyType.Once)
            {
                switch (KeyType)
                {
                    case InputKeyType.Once:
                        isPressed = IsDisabledControlJustPressed(KeyGroup, KeyCode);
                        break;
                    case InputKeyType.Continuous:
                        isPressed = IsDisabledControlPressed(KeyGroup, KeyCode);
                        break;
                }
            }
            else
            {
                switch (KeyType)
                {
                    case InputKeyType.Once:
                        isPressed = IsControlJustPressed(KeyGroup, KeyCode);
                        break;
                    case InputKeyType.Continuous:
                        isPressed = IsControlPressed(KeyGroup, KeyCode);
                        break;
                }
            }

            if (isPressed)
            {
                //Set the time the key was first pressed.
                if (!PressedState)
                {
                    TimePressed = GetGameTimer();
                    PressedState = true;
                }

                //Invoke the key callbacks.
                int time = MathUtil.Clamp(GetGameTimer() - TimePressed, 0, int.MaxValue);
                foreach (InputKeyCallback callback in Callbacks)
                {
                    callback.Invoke(time);
                }
            }
            else
            {
                //Reset it!
                if (TimePressed > 0)
                {
                    TimePressed = -1;
                    PressedState = false;
                }
            }

            return isPressed;
        }
    }

    internal class Input
    {
        private readonly List<InputKeyState> keyStates = new List<InputKeyState>();
        private readonly HashSet<InputMouseCallback> mouseCallbacks = new HashSet<InputMouseCallback>();

        public Input()
        {
            IAccessor accessor = MapBuilderClient.Accessor;
            accessor.RegisterTick(Update);
            accessor.RegisterTick(UpdateMouse);

        }

        public InputKeyState RegisterKey(int keyGroup, int keyCode, InputKeyType type)
        {
            if (!FindKeyState(keyGroup, keyCode, out InputKeyState state))
            {
                state = new InputKeyState(keyGroup, keyCode, type, false);
            }

            keyStates.Add(state);
            return state;
        }

        public InputKeyState RegisterKey(int keyGroup, int keyCode, InputKeyType type, InputKeyCallback callback)
        {
            InputKeyState state = RegisterKey(keyGroup, keyCode, type);

            if (callback != null)
            {
                state.AddCallback(callback);
            }

            return state;
        }

        public void RegisterMouse(InputMouseCallback callback)
        {
            mouseCallbacks.Add(callback);
        }

        public void DisableKey(int keyGroup, int keyCode)
        {
            if (FindKeyState(keyGroup, keyCode, out InputKeyState state))
            {
                if (!state.IsDisabled)
                {
                    state.IsDisabled = true;
                }
            }
            else
            {
                Debug.WriteLine($"Cannot disable key [{keyGroup}, {keyCode}] because state doesn't exist. Forgot to call RegisterKey?");
            }
        }

        public void EnableKey(int keyGroup, int keyCode)
        {
            if (FindKeyState(keyGroup, keyCode, out InputKeyState state))
            {
                if (state.IsDisabled)
                {
                    state.IsDisabled = false;
                }
            }
            else
            {
                Debug.WriteLine($"Cannot enable key [{keyGroup}, {keyCode}] because state doesn't exist. Forgot to call RegisterKey?");
            }
        }

        public async Task Update()
        {
            foreach (InputKeyState state in keyStates)
            {
                //Update the key states
                state.Update();

                //Update any keys that are disabled.
                if (state.IsDisabled)
                {
                    DisableControlAction(state.KeyGroup, state.KeyCode, true);
                }
            }

            await BaseScript.Delay(0);
        }

        async Task UpdateMouse()
        {
            const float mouseEpsilon = 0.001f;

            if (mouseCallbacks.Count == 0)
            {
                return;
            }

            float mx = -GetControlNormal(0, 2);
            float my = -GetControlNormal(0, 1);

            if (Math.Abs(mx) < mouseEpsilon &&
                Math.Abs(my) < mouseEpsilon)
            {
                return;
            }

            Vector2 mousePosition = new Vector2(mx, my);

            foreach (InputMouseCallback callback in mouseCallbacks)
            {
                callback.Invoke(mousePosition);
            }

            await Task.FromResult(0);
        }

        bool FindKeyState(int keyGroup, int keyCode, out InputKeyState outState)
        {
            foreach (InputKeyState state in keyStates)
            {
                if (state.KeyGroup == keyGroup && state.KeyCode == keyCode)
                {
                    outState = state;
                    return true;
                }
            }

            outState = default;
            return false;
        }
    }
}