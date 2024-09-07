using UnityEngine;
using System.Collections;

public class CoroutineUtility : MonoBehaviour
{
    private static CoroutineUtility _instance;

    public static CoroutineUtility Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("CoroutineUtility");
                _instance = go.AddComponent<CoroutineUtility>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // This method is static and uses the instance to start the coroutine
    public static Coroutine StartManagedCoroutine(IEnumerator coroutine)
    {
        Debug.Log("loaded" + coroutine);
        return Instance.StartCoroutine(coroutine);
    }
}
