using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour {

    public GameObject player;
    public Text scoreText;
//********** 開始 **********//
    public Text highScoreText; //ハイスコアを表示するTextオブジェクト
//********** 終了 **********//
    //public GameOverScript gameOverScript;
    private int score = 0;
    private int scoreUpPos = 3;
    private Transform playerTrans;
//********** 開始 **********//
    private int highScore; //ハイスコア計算用変数
    private string key = "HIGH SCORE"; //ハイスコアの保存先キー
//********** 終了 **********//

    void Start (){
        playerTrans = player.GetComponent<Transform>();
        scoreText.text = "Score: 0";
//********** 開始 **********//
        //保存しておいたハイスコアをキーで呼び出し取得
        //保存されていなければ0が返る
        highScore = PlayerPrefs.GetInt(key, 0);
        //ハイスコアを表示
        highScoreText.text = "HighScore: " + highScore.ToString();
//********** 終了 **********//
    }

    void Update ()
    {
        float playerHeight = playerTrans.position.y;
        float currentCameraHeight = transform.position.y;
        float newHeight = Mathf.Lerp (currentCameraHeight, playerHeight, 0.5f);
        if (playerHeight > currentCameraHeight) {
            transform.position = new Vector3 (transform.position.x, newHeight, transform.position.z);
        }

        if (playerTrans.position.y >= scoreUpPos) {
            scoreUpPos += 3;
            score += 10;
            scoreText.text = "Score: " + score.ToString ();
//********** 開始 **********//
            //ハイスコアより現在スコアが高い時
            if (score > highScore) {
                //ハイスコア更新
                highScore = score;
                //ハイスコアを保存
                PlayerPrefs.SetInt(key, highScore);
                //ハイスコアを表示
                highScoreText.text = "HighScore: " + highScore.ToString();
            }
//********** 終了 **********//
        }

        if (playerTrans.position.y <= currentCameraHeight - 6) {

            //gameOverScript.Lose();

            Destroy(player);
        }
    }
}