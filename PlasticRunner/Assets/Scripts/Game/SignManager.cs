using UnityEngine;
using System.Collections;

public class SignManager : MonoBehaviour {
    public GameObject mSign = null;
    private Vector3 mSignPosxAdd = Vector3.right * 5;
    private Vector3 mSignPos = Vector3.zero;
    //private Quaternion mSignQuaternion;

    // Use this for initialization
    void Start ()
    {
        CreatSign(new Vector3(15.0f, 3.0f, 0.0f));
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    //OnTriggerEnterで呼び出し
    void CreateSign()
    { 
        Instantiate(mSign, new Vector3(Random.Range(mSignPos.x + 40.0f, mSignPos.x + 80.0f),
                    Random.Range(3.0f, 5.0f), 0.0f), mSign.transform.rotation);
        mSignPos += mSign.transform.localPosition;
    }

    //Startで呼び出し
    void CreatSign(Vector3 aVector3)
    {
        Instantiate(mSign, aVector3, mSign.transform.rotation);
    }
}
