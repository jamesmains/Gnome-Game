using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour {
    public static UnityEvent<AudioClip> OnPlayClip = new();
    [SerializeField] private AudioSource audioSrc;

    private void OnEnable() {
        OnPlayClip.AddListener(PlayClip);
    }

    private void OnDisable() {
        OnPlayClip.RemoveListener(PlayClip);
    }

    private void PlayClip(AudioClip clip) {
        if (clip == null) return;
        audioSrc.PlayOneShot(clip);
    }
}