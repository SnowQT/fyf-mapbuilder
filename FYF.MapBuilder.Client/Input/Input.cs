using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System.Linq;

namespace FYF.MapBuilder.Client
{
    internal delegate void InputKeyCallback(int pressedTime);
    internal delegate void InputMouseCallback(Vector2 mouseDelta);

    internal enum InputKeyType
    {
        Continuous,
        Once,
    }

    internal class InputKeyState
    {
        IDictionary<bool[], Func<int, int, bool>> KeyPressFuncMap = new Dictionary<bool[], Func<int, int, bool>>
        {
            //Left = Continuous or not, Right = disabled or not.
            { new bool[] { true, false}, IsControlPressed },
            { new bool[] { false, false}, IsControlJustPressed },
            { new bool[] { true, true}, IsDisabledControlPressed },
            { new bool[] { false, true}, IsDisabledControlJustPressed },
        };

        public int KeyGroup;
        public int KeyCode;
        public InputKeyType KeyType;
        public bool WasPressedLastPoll;
        public int TimePressed;
        public HashSet<InputKeyCallback> Callbacks;

        private Func<int, int, bool> KeyMethod;
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

                //Update the method the keypress should use.
                UpdateKeyMethod();

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
            Profiler.Enter("InputKeyState_Update");

            bool isPressed = KeyMethod(KeyGroup, KeyCode);

            if (isPressed)
            {
                //Set the time the key was first pressed.
                if (!WasPressedLastPoll)
                {
                    TimePressed = GetGameTimer();
                    WasPressedLastPoll = true;
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
                    WasPressedLastPoll = false;
                }
            }

            Profiler.Exit();

            return false;
        }

        private void UpdateKeyMethod()
        {
            bool[] identity = new bool[] {
                KeyType == InputKeyType.Continuous,
                IsDisabled
            };

            var function = KeyPressFuncMap.FirstOrDefault(
                (id) => id.Key[0] == identity[0] && id.Key[1] == identity[1]
            ).Value;

            if (function != null)
            {
                KeyMethod = function;
                return;
            }
        }
    }

    internal class Input
    {
        private readonly List<InputKeyState> keyStates = new List<InputKeyState>();
        private Vector2 mouseChangeVector = Vector2.Zero;

        public Input()
        {
            IAccessor accessor = MapBuilderClient.Accessor;
            accessor.OnUpdateTick(UpdateDisabledKeys);
            accessor.OnUpdateTick(UpdateKeyStates);
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

        public bool PollKey(int keyGroup, int keyCode, out int time)
        {
            time = -1;

            if (FindKeyState(keyGroup, keyCode, out InputKeyState state))
            {
                if (state.WasPressedLastPoll)
                {
                    time = GetGameTimer() - state.TimePressed;
                    return true;
                }
            }
            else
            {
                Debug.WriteLine($"Cannot poll key [{keyGroup}, {keyCode}] because state doesn't exist. Forgot to call RegisterKey?");
            }

            return false;
        }

        public Vector2 PollMouse()
        {
            mouseChangeVector.X = -GetControlNormal(0, 2);
            mouseChangeVector.Y = -GetControlNormal(0, 1);
            return mouseChangeVector;
        }

        private Task UpdateKeyStates()
        {
            Profiler.Enter("Input_UpdateKeyStates");

            foreach (InputKeyState state in keyStates)
            {
                state.Update();
            }

            Profiler.Exit();

            return Task.FromResult(0);
        }

        private Task UpdateDisabledKeys()
        {
            Profiler.Enter("Input_UpdateDisabledKeys");

            foreach (InputKeyState state in keyStates)
            {
                if (state.IsDisabled)
                {
                    DisableControlAction(state.KeyGroup, state.KeyCode, true);
                }
            }

            Profiler.Exit();

            return Task.FromResult(0);
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