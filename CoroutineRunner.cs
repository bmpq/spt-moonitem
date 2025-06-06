using System;
using System.Collections;
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

        public static void RunAfterDelay(Action action, float delay) => Instance.StartCoroutine(Instance.Delay(action, delay));
        IEnumerator Delay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);

            action.Invoke();
        }
    }
}
