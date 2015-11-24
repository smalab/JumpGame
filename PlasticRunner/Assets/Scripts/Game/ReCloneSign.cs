using UnityEngine;
using System.Collections;

public class ReCloneSign : MonoBehaviour {
    private PlayMP3 mPlaymp3 = null;
    private ScoreManager mScorecontrol = null;
    private SignManager mSignmanager = null;

    void Start()
    {
        mPlaymp3 = GameObject.Find("AudioManager").GetComponent<PlayMP3>();
        mScorecontrol = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        mSignmanager = GameObject.Find("SignManager").GetComponent<SignManager>();
    }

    // Update is called once per frame
    void Update () {
	
	}

    //看板に当たれなかった場合
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Event"))
        {
            //mPlaymp3.SendMessage("Playmp3");
            //mScorecontrol.SendMessage("AddScore");
            Destroy(gameObject, 0.5f);
            mSignmanager.SendMessage("CreateSign");
        }
    }
}
