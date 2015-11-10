using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadTxtData : MonoBehaviour {
    public TextAsset AlphabetTextAsset = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //this.GetComponent<Text>().text = "test";
        LoadText();
	}

    void LoadText ()
    {
        AlphabetTextAsset = Resources.Load("text/Alphabet") as TextAsset;
        string AlphabetText = AlphabetTextAsset.text;
        string[] lines = AlphabetText.Split('\n');
        foreach(var line in lines) Debug.Log(line);
    }

}
