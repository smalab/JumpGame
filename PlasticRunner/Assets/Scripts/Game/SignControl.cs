using UnityEngine;
using System.Collections;

public class SignControl : MonoBehaviour {
    private PlayMP3 mPlaymp3 = null;

    void Start()
    {
        mPlaymp3 = GameObject.Find("AudioManager").GetComponent<PlayMP3>();
    }

    void OnTriggerEnter (Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Event"))
        {
           mPlaymp3.SendMessage("Playmp3");
           Destroy(gameObject, 1.0f);
        }
    }
}
