using UnityEngine;
using UnityEngine.UI;

public class LoadTxtData : MonoBehaviour {
    Text mTextSign = (Resources.Load("Prefabs/CanvasSign/Panel/Text")) as Text;
    public TextAsset mAlphabetTextAsset = null;
    string mAlphabetText = "";
    string[] mLines = null;
    //オーディオファイルをロードするためのプロパティ
    public string GetLinesIndex { get { return mLines[idx]; } }
    int idx = 0;

    // Use this for initialization
    void Start () {
        LoadText();
        ShowText();
    }
	
	// Update is called once per frame
	void Update () {}

    void LoadText ()
    {
        mAlphabetTextAsset = Resources.Load("Text/Alphabet") as TextAsset;
        mAlphabetText = mAlphabetTextAsset.text;
        mLines = mAlphabetText.Split('\n');
    }

    void ShowText()
    {
        System.Random rnd = new System.Random();
        idx = rnd.Next(mLines.Length);
        mTextSign.GetComponent<Text>().text = mLines[idx];
        Debug.Log(mLines[idx]);
    }
}
