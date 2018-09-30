using UnityEngine;
using System.Collections;

    public class SoundManager : MonoBehaviour 
    {
        public const int
            location_bomb = 0,
            location_zombieSpawn = 1,
            location_portcullis = 2,
            locaction_sheep = 3,
            location_grab_coin = 4,
            location_grab_gem = 5,
            location_gulp = 6,
            location_cracking = 7;
        public AudioSource efxSource1;
        public AudioSource efxSource2;
        public AudioSource musicSource;
        public AudioClip[] music;
        public AudioClip[] fxs;

        public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.             
        public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
        public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
        [HideInInspector] public bool playMusic = false;
        
        void Awake (){
            //Check if there is already an instance of SoundManager
            if (instance == null)
                //if not, set it to this.
                instance = this;
            //If instance already exists:
            else if (instance != this)
                //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
                Destroy (gameObject);
            
            //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
            DontDestroyOnLoad (gameObject);
        }

        void Update(){
            if(playMusic && !musicSource.isPlaying) {
                musicSource.clip = GetRandomClip();
                musicSource.Play();
            }
        }

        private AudioClip GetRandomClip(){
            return music[Random.Range(0, music.Length)];
        }

        public void playFxClip(int position){
            efxSource1.PlayOneShot(fxs[position]);
        }

        public void playFx1 (AudioClip clips) {
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            efxSource1.pitch = randomPitch;
            efxSource1.PlayOneShot(clips);
        }
        public void playFx2 (AudioClip clips) {
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            efxSource2.pitch = randomPitch;
            efxSource2.clip = clips;
            efxSource2.Play();
        }
    }