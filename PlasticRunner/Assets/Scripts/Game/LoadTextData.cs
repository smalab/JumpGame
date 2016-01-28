using UnityEngine;
using UnityEngine.UI;

public class LoadTextData : MonoBehaviour {
    TextAsset mAlphabetTextAsset = null;
    string mAlphabetText = "";
    string[] mLines = null;
    static public int idx = 0;
    static public Text mText = null;

    // Use this for initialization
    void Start () {
        mText = GetComponent<Text>();
        LoadText();
        ShowText();
    }
	
	// Update is called once per frame
	void Update () {}

    void LoadText ()
    {
        //Resourceフォルダからテキストファイルのデータをロード
        mAlphabetTextAsset = Resources.Load("Text/Alphabet") as TextAsset;
        mAlphabetText = mAlphabetTextAsset.text;
        //データを改行毎に分割して配列に格納
        mLines = mAlphabetText.Split('\n');
    }

    //看板にアルファベットを表示させる
    void ShowText()
    {
        System.Random rnd = new System.Random();
        idx = rnd.Next(mLines.Length - 1);
        mText.text = mLines[idx];
    }
}
