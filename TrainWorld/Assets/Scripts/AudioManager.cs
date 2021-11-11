using System.Collections.Generic;
using UnityEngine;

namespace TrainWorld.Audio
{
    [System.Serializable]
    public class Sound
    {
        public string soundName;
        public AudioClip audioClip;

        [Range(0.0f, 1.0f)]
        public float volume = 0.7f;
        [Range(0.5f, 1.5f)]
        public float pitch = 1.0f;

        [Range(0.0f, 0.5f)]
        public float randomVolume = 0.1f;
        [Range(0.0f, 0.5f)]
        public float randomPitch = 0.1f;

        public bool loop;


        private AudioSource audioSource;

        public void SetSource(AudioSource source)
        {
            audioSource = source;
        }

        public void Play()
        {
            audioSource.volume = volume * 1 + Random.Range(-randomVolume / 2f, randomVolume / 2f);
            audioSource.pitch = pitch * 1 + Random.Range(-randomPitch / 2f, randomPitch / 2f); ;
            audioSource.Play();
        }
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField]
        private List<Sound> sounds;

        private Dictionary<string, Sound> soundDictionary;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            soundDictionary = new Dictionary<string, Sound>();
            foreach (var sound in sounds)
            {
                GameObject go = new GameObject(sound.soundName);
                AudioSource audioSource = go.AddComponent<AudioSource>();
                audioSource.clip = sound.audioClip;
                audioSource.loop = sound.loop;

                sound.SetSource(audioSource);
                soundDictionary.Add(sound.soundName, sound);
            }
            PlaySound("BGM");
        }

        public void PlaySound(string soundName)
        {
            if (soundDictionary.ContainsKey(soundName) == false)
            {
                Debug.LogError("AudioManager : Sound not Found in Dictionary. Your Request : " + soundName);
                return;
            }

            soundDictionary[soundName].Play();
        }


    }
}