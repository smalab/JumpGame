using UnityEngine;
using System.Collections;

public class SignManager : MonoBehaviour {
    public GameObject mSign;
    private Vector3 mSignPosxInc = new Vector3(5.0f, 0, 0);
    private Vector3 mSignPos = new Vector3(0, 0, 0);
    private Quaternion mSignQuaternion;
    // Use this for initialization
    void Start () {
        mSignQuaternion = new Quaternion();
        mSignQuaternion = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
        StartCoroutine(CreateSign());
	}

    IEnumerator CreateSign()
    {
        Instantiate(mSign, mSignPos, mSignQuaternion);
        yield return new WaitForSeconds(5.0f);
        mSignPos += Vector3.right + mSignPosxInc;
    }

}
