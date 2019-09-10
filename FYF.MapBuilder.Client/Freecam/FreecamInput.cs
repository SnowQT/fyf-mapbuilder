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

    internal delegate void InputKeyCallback(int msec);
    internal delegate void InputMouseCallback(Vector2 deltaPosition);

    internal class FreecamInputEntry
    {
        public FreecamKeys Key;
        public bool CurrentState;
        public int CurrentTime;
        public HashSet<InputKeyCallback> Callbacks;

        public FreecamInputEntry(FreecamKeys key, params InputKeyCallback[] callbacks)
        {
            Key = key;
            Callbacks = new HashSet<InputKeyCallback>(callbacks);

            CurrentState = false;
            CurrentTime = 0;
        }

        public void Update(bool state)
        {
            //If the states are NOT synced, reset.
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

            foreach (InputKeyCallback cb in Callbacks)
            {
                cb.Invoke(heldTime);
            }
        }
    }


    //@TODO: Right now we always assume the input group is 0.
    //       Which for some scenarios might not be ideal, but ideal for our intents and purposes.
    internal sealed class FreecamInput
    {
        private Dictionary<FreecamKeys, FreecamInputEntry> inputEntries = 
            new Dictionary<FreecamKeys, FreecamInputEntry>();

        private InputMouseCallback mouseCallback;

        public void BindKey(FreecamKeys key, InputKeyCallback callback)
        {
            bool found = inputEntries.TryGetValue(key, out FreecamInputEntry entry);

            if (found)
            {
                entry.Callbacks.Add(callback);
            }
            else
            {
                inputEntries[key] = new FreecamInputEntry(key, callback);
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
                bool found = inputEntries.TryGetValue(keyCallback.Key, out FreecamInputEntry entry);

                if (!found)
                {
                    continue;
                }

                int key = (int)keyCallback.Key;
                bool state = IsControlPressed(0, key);

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
