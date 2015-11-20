using UnityEngine;
using System.Collections;

public class SignManager : MonoBehaviour {
    public GameObject mSign = null;
    private Vector3 mSignPosxAdd = new Vector3(5.0f, 0, 0);
    private Vector3 mSignPos = new Vector3(0, 0, 0);
    //private Quaternion mSignQuaternion;

    // Use this for initialization
    void Start ()
    {
        //mSignQuaternion = new Quaternion();
        //mSignQuaternion = Quaternion.identity;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void CreateSign()
    { 
        Instantiate(mSign, new Vector3(Random.Range(mSignPosxAdd.x + 50, mSignPosxAdd.x + 90),
                    Random.Range(3.0f, 5.0f), 0.0f), mSign.transform.rotation);
        mSignPos += mSignPosxAdd;
    }
}
