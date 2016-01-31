using UnityEngine;
using System.Collections;

public class SignManager : MonoBehaviour
{
    static public GameObject mSign = null;
    private Vector3 mSignPos = Vector3.zero;
    static public GameObject tmpObject = null;
    //private Quaternion mSignQuaternion;

    // Use this for initialization
    void Start()
    {
        mSign = (Resources.Load("Prefabs/CanvasSign")) as GameObject;
        CreatSign(new Vector3(15.0f, 3.0f, 0.0f));

    }

    // Update is called once per frame
    void Update() { }


    void CreateSign()
    {
        tmpObject = (GameObject)Instantiate(mSign, Vector3.zero, Quaternion.identity); //次の看板を複製
        //複製した看板の座標を指定
        tmpObject.transform.position = new Vector3(mSignPos.x + Random.Range(25.0f, 35.0f),
                                                   Random.Range(2.0f, 4.0f), tmpObject.transform.position.z);
        tmpObject.transform.Rotate(new Vector3(0f, 90f, 0f));
        //次に複製するための座標を保持
        mSignPos = tmpObject.transform.position;
    }

    void CreatSign(Vector3 aVector3)
    {
        tmpObject = (GameObject)Instantiate(mSign, aVector3, mSign.transform.rotation);
        mSignPos = new Vector3(mSignPos.x + Random.Range(30.0f, 45.0f),
                               Random.Range(2.0f, 4.0f), mSign.transform.position.z);
    }
}
