using UnityEngine;
using System.Collections;

public class ReCloneSign : MonoBehaviour {
    private SignManagerScript mSignmanager = null;

    void Start()
    {
        mSignmanager = GameObject.Find("SignManager").GetComponent<SignManagerScript>();
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
            Destroy(SignManagerScript.tmpObject, 0.5f);
            mSignmanager.SendMessage("CreateSign");
        }
    }
}
