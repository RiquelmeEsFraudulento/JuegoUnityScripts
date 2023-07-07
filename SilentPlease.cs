using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    private void Start()
    {
        // Get all Audio Listener components in the scene
        AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();

        // Mute or disable all Audio Listeners
        foreach (AudioListener audioListener in audioListeners)
        {
            audioListener.enabled = false; // Disable the Audio Listener
            // Alternatively, you can set the volume to zero to silence the Audio Listener
            //audioListener.volume = 0f;
        }
    }
}
