using UnityEngine;

namespace tarkin.moonitem
{
    internal class CoroutineRunner : MonoBehaviour
    {
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject("Coroutine Runner").AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(_instance.gameObject);
                return _instance;
            }
        }

        private static CoroutineRunner _instance;
    }
}
