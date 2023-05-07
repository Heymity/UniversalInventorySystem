using UnityEngine;

namespace MolecularLib.Helpers
{
    public static class PlayStatus
    {
        public static bool IsPlaying { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            IsPlaying = true;
            Application.quitting += () => IsPlaying = false;
        }
    }
}
