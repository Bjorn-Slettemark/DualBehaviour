using System.Linq;
using UnityEngine;

public class MultiplayerChannelManager : MonoBehaviour
{
    public static MultiplayerChannelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void BroadcastEvent(string channelName, string eventData)
    {
        string message = $"{channelName}:{eventData}";
        WebRTCEngine.Instance.SendDataMessage(message);
    }
}