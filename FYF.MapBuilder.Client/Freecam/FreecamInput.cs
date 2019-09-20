using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;
using static CitizenFX.Core.Native.API;

namespace FYF.MapBuilder.Client
{
    internal enum FreecamKeys : int
    {
        Forward = 32,
        Backwards = 33,
        Left = 34,
        Right = 35,

        Up = 44,
        Down = 46,
    }

    internal delegate void InputKeyCallback(float reach);
    internal delegate void InputMouseCallback(Vector2 deltaPosition);

    internal sealed class FreecamInputEntry
    {
        public FreecamKeys Key;
        public int MaxReach;
        public HashSet<InputKeyCallback> Callbacks;

        private bool CurrentState;
        private int CurrentTime;

        public FreecamInputEntry(FreecamKeys key, int maxReach, params InputKeyCallback[] callbacks)
        {
            Key = key;
            MaxReach = maxReach;
            Callbacks = new HashSet<InputKeyCallback>(callbacks);

            CurrentState = false;
            CurrentTime = 0;
        }

        public void Update(bool state)
        {
            //if A != B...
            if ((state ^ CurrentState))
            {
                CurrentTime = GetGameTimer();
                CurrentState = state;
            }
        }

        public void Invoke()
        {
            if (Callbacks.Count == 0)
            {
                return;
            }

            int heldTime = GetGameTimer() - CurrentTime;
            float reach = MathUtil.Clamp((float)heldTime / (float)MaxReach, 0.01f, 1.0f);

            foreach (InputKeyCallback cb in Callbacks)
            {
                cb.Invoke(reach);
            }
        }
    }


    //@TODO: Right now we always assume the input group is 0. We could solve this by implementing a wrapper for the input system.
    //       Which for some scenarios might not be ideal, but ideal for our intents and purposes.
    internal sealed class FreecamInput
    {
        private Freecam self;

        private Dictionary<FreecamKeys, FreecamInputEntry> inputEntries = 
            new Dictionary<FreecamKeys, FreecamInputEntry>();

        private InputMouseCallback mouseCallback;

        public FreecamInput(Freecam self)
        {
            this.self = self;
        }

        public void BindKey(FreecamKeys key, InputKeyCallback callback)
        {
            bool found = inputEntries.TryGetValue(key, out FreecamInputEntry entry);

            if (found)
            {
                entry.Callbacks.Add(callback);
            }
            else
            {
                inputEntries[key] = new FreecamInputEntry(key, self.Config.KeySmoothTime, callback);
            }
        }

        public void BindMouse(InputMouseCallback callback)
        {
            mouseCallback = callback;
        }

        public void PollKeys()
        {
            foreach (var keyCallback in inputEntries)
            {
                FreecamInputEntry entry = keyCallback.Value;
                bool state = IsControlPressed(0, (int)keyCallback.Key);

                entry.Update(state);

                if (state)
                {
                    entry.Invoke();
                }
            }
        }

        public void PollMouse()
        {
            if (mouseCallback == null)
            {
                return;
            }

            Vector2 mousePosition = new Vector2(-GetControlNormal(0, 2), -GetControlNormal(0, 1));

            if (mousePosition.LengthSquared() > 0.0f)
            {
                mouseCallback.Invoke(mousePosition);
            }
        }
    }
}