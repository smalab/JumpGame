using UnityEngine;
using System.Collections;

public class ReCloneSign : MonoBehaviour {
    private SignManager mSignmanager = null;

    void Start()
    {
        mSignmanager = GameObject.Find("SignManager").GetComponent<SignManager>();
    }

    // Update is called once per frame
    void Update () {}

    //看板に当たれなかった場合
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Sign"))
        {
            //mPlaymp3.SendMessage("Playmp3");
            //mScorecontrol.SendMessage("AddScore");
            Destroy(SignManager.tmpObject, 0.5f);
            mSignmanager.SendMessage("CreateSign");
        }
    }
}
