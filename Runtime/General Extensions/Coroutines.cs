using System;
using System.Collections;
using UnityEngine;

namespace DesignPatterns
{
    public static class Coroutines
    {
        public enum TimeMode { Scaled, Unscaled, FixedScaled, FixedUnscaled }
        private static float ScaledDeltaTime() => Time.deltaTime;
        private static float UnscaledDeltaTime() => Time.unscaledDeltaTime;
        private static float ScaledFixedDeltaTime() => Time.fixedDeltaTime;
        private static float UnscaledFixedDeltaTime() => Time.fixedUnscaledDeltaTime;
        private static Func<float>[] DeltaTimes = { ScaledDeltaTime, UnscaledDeltaTime, ScaledFixedDeltaTime, UnscaledFixedDeltaTime };

        public static IEnumerator Lerp(
            float duration, Func<YieldInstruction> step = null, 
            Action enter = null, Action<float> update = null, Action exit = null,
            TimeMode timeMode = TimeMode.Scaled
        )
        {
            enter?.Invoke();
            float timer = 0;
            duration = duration < 0 ? 0 : duration;
            update ??= _ => { };
            step ??= () => null;
            Func<float> deltaTime = DeltaTimes[(int)timeMode];
            while (timer < duration)
            {
                update(timer / duration);
                timer += deltaTime();
                yield return step.Invoke();
            }
            exit?.Invoke();
        }
    }
}