using UnityEngine;
using UnityEngine.UI;

public class LoadTextData : MonoBehaviour {
    //Text mTextSign = (Resources.Load("Prefabs/CanvasSign/Panel/Text")) as Text;
    TextAsset mAlphabetTextAsset = null;
    string mAlphabetText = "";
    string[] mLines = null;
    //static public string mExportText = "";
    static public int idx = 0;
    static public Text mText = null;

    // Use this for initialization
    void Start () {
        mText = GetComponent<Text>();
        LoadText();
        ShowText();
        //ExportText();
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
        idx = rnd.Next(mLines.Length - 1);
        mText.text = mLines[idx];
    }

    //void ExportText()
    //{
    //    if (mExportText == "")
    //        mExportText = mLines[idx];
    //    else
    //        mExportText = "";
    //}
}
