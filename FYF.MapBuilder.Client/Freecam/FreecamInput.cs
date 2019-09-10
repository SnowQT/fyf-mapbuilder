using CitizenFX.Core;
using System.Collections.Generic;
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

        Mouse = 0
    }

    internal delegate void FreecamKeyInput(float time);
    internal delegate void FreecamMouseInput(Vector2 deltaPosition);

    //@TODO: Right now we always assume the input group is 0.
    //       Which for some scenarios might not be ideal, but ideal for our intents and purposes.
    internal sealed class FreecamInput
    {
        private struct FreecamKeyInputCallback_t
        {
            public FreecamKeys Key;
            public FreecamKeyInput Callback;

            public FreecamKeyInputCallback_t(FreecamKeys key, FreecamKeyInput callback )
            {
                Key = key;
                Callback = callback;
            }

            public void Invoke(float time)
            {
                if (Callback == null)
                {
                    return;
                }

                Callback.Invoke(time);
            }
        }

        private HashSet<FreecamKeyInputCallback_t> keyCallbacks =
            new HashSet<FreecamKeyInputCallback_t>();

        private FreecamMouseInput mouseCallback;

        public void BindKey(FreecamKeys key, FreecamKeyInput callback)
        {
            keyCallbacks.Add(new FreecamKeyInputCallback_t(key, callback));
        }

        public void BindMouse(FreecamMouseInput callback)
        {
            mouseCallback = callback;
        }

        public void PollKeys()
        {
            //@TODO @PERF: De-duplicate the keys, right now we potentially could poll the same key
            //             multiple times. Our data structure could deduplicate this.
            foreach (var keyCallback in keyCallbacks)
            {
                int key = (int)keyCallback.Key;

                if (IsControlPressed(0, key))
                {
                    //@TODO: Keep track of key held time.
                    keyCallback.Invoke(0.0f);
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
