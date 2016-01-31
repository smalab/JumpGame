using UnityEngine;
using System.Collections;

public class SignControl : MonoBehaviour {
    private ScoreManager mScorecontrol = null;
    private SignManager mSignmanager = null;

    void Start()
    {
        mScorecontrol = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        mSignmanager = GameObject.Find("SignManager").GetComponent<SignManager>();
    }

    //看板とプレイヤーがぶつかった場合
    void OnTriggerEnter (Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Event"))
        {
            mScorecontrol.SendMessage("AddScore");
            Destroy(gameObject, 0.5f);
            mSignmanager.SendMessage("CreateSign");
        }
    }
}
