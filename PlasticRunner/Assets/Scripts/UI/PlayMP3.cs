using UnityEngine;
using System.Collections;

public class PlayMP3 : MonoBehaviour {

	public GameObject unity;
	public float speed = 0.07f;
	public AudioClip audioclip;
	AudioSource audiosource;

	// Use this for initialization
	void Start () {
		audiosource = gameObject.GetComponent<AudioSource> ();
		audiosource.clip = audioclip;
		unity = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		// transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (unity.transform.position - transform.position), speed);
		// transform.position += transform.forward * speed;
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.name == "Player") {
			audiosource.PlayOneShot (audioclip);
			Debug.Log ("entertag");
			// gameObject.GetComponent<Hitbucket> ().enabled = false;
		}
	}
}
