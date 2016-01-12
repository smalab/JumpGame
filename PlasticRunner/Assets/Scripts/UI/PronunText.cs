using UnityEngine;
using UnityEngine.UI;

public class PronunText : MonoBehaviour {
    public GameObject MessageText = null;
    private PlayMP3 mPlaymp3 = null;

	// Use this for initialization
	void Start () {
        MessageText.SetActive(false);
        mPlaymp3 = GameObject.Find("AudioManager").GetComponent<PlayMP3>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider collider)
    {
       if(collider.gameObject.layer == LayerMask.NameToLayer("Event"))
        {
            mPlaymp3.SendMessage("PlayAlphaMp3");
            MessageText.SetActive(true);
        }
    }
}
