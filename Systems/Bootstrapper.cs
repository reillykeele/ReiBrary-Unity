using UnityEngine;

namespace Util.Systems
{
    public static class Bootstrapper
    {
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //public static void Execute() => Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));

        // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        // public static void Execute() => Object.DontDestroyOnLoad(Addressables.InstantiateAsync("Systems").WaitForCompletion);
    }
}
