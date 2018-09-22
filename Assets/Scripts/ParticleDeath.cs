using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDeath : MonoBehaviour {

	// Use this for initialization

	private ParticleSystem thisParticleSystem;
	void Start () {
		thisParticleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		Destroy (gameObject,thisParticleSystem.main.duration);
	}
}
