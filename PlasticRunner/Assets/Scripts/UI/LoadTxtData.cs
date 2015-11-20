using UnityEngine;
using UnityEngine.UI;

public class LoadTxtData : MonoBehaviour {
    public TextAsset AlphabetTextAsset = null;
    string AlphabetText = "";
    string[] lines = null;
    int idx = 0;

    // Use this for initialization
    void Start () {
        LoadText();
        ShowText();
    }
	
	// Update is called once per frame
	void Update () {
       
    }

    void LoadText ()
    {
        AlphabetTextAsset = Resources.Load("text/Alphabet") as TextAsset;
        AlphabetText = AlphabetTextAsset.text;
        lines = AlphabetText.Split('\n');
    }

    void ShowText()
    {
        System.Random rnd = new System.Random();
        idx = rnd.Next(lines.Length);
        GetComponent<Text>().text = lines[idx];
    }
}
