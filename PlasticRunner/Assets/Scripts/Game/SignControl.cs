using UnityEngine;

public class SignControl : MonoBehaviour {
    //private PlayMP3 mPlaymp3 = null;
    private ScoreManager mScorecontrol = null;
    private SignManager mSignmanager = null;

    void Start()
    {
        //mPlaymp3 = GameObject.Find("AudioManager").GetComponent<PlayMP3>();
        mScorecontrol = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        mSignmanager = GameObject.Find("SignManager").GetComponent<SignManager>();
    }

    //看板とプレイヤーがぶつかった場合
    void OnTriggerEnter (Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Event"))
        {
            //mPlaymp3.SendMessage("PlayAlphaMp3");
            mScorecontrol.SendMessage("AddScore");
            Destroy(gameObject, 0.5f);
            mSignmanager.SendMessage("CreateSign");
        }
    }
}
