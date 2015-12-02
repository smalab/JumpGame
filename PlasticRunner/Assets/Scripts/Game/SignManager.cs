using UnityEngine;
using System.Collections;

public class SignManager : MonoBehaviour {
    static public GameObject mSign = (Resources.Load("Prefabs/CanvasSign")) as GameObject;
    private Vector3 mSignPos = Vector3.zero;
    static public GameObject tmpObject = null;
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
    //(課題)Instantiateせずに看板の文字だけ消して、座標を変更してあげるように = MoveSign()
    void CreateSign()
    {
        tmpObject = (GameObject) Instantiate(mSign, Vector3.zero, Quaternion.identity);
        tmpObject.transform.position = new Vector3(mSignPos.x + Random.Range(30.0f, 45.0f), 
                                                   Random.Range(2.0f, 4.0f), tmpObject.transform.position.z);
        mSignPos = tmpObject.transform.position;
    }

    // void MoveSign()
    // {
    //     mSign.transform.Translate(new Vector3(mSignPos.x + Random.Range(30.0f, 45.0f),
    //                               Random.Range(2.0f, 4.0f), mSign.transform.position.z));
    //     mSignPos = mSign.transform.position;
    //     Debug.Log(mSignPos);
    // }

    //Startで呼び出し
    void CreatSign(Vector3 aVector3)
    {
        tmpObject = (GameObject) Instantiate(mSign, aVector3, mSign.transform.rotation);
        mSignPos = new Vector3(mSignPos.x + Random.Range(30.0f, 45.0f),
                               Random.Range(2.0f, 4.0f), mSign.transform.position.z);
    }
}
