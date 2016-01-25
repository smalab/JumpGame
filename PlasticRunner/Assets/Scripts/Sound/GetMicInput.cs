using UnityEngine;
using System.Collections;

// 空の Audio Source を作って置く
[RequireComponent (typeof (AudioSource))]
public class GetMicInput : MonoBehaviour {
    float lastLoudness;         //前フレームの音量.
    public static float m_aveVoice;       //平均的な声の音量
    public float second = 300.0f;   //録音する音データの長さ(s)
    static public float loudness;             //現フレームの音量.
    public float sensitivity = 100.0f;     //感度.音量の最大値.
    public float lastLoudnessInfluence; //前フレームの影響度合い.
    private AudioSource mAudio = null;  //マイクからの入力を取得する

    //public float AveVoice {get { return m_aveVoice; } }

    void Start() 
    {
        // 空の Audio Sourceを取得
        mAudio = GetComponent<AudioSource>();
        // Audio Source の Audio Clip をマイク入力に設定
        // 引数は、デバイス名（null ならデフォルト）、ループ、何秒取るか、サンプリング周波数
        mAudio.clip = Microphone.Start(null , false, 300, 44100);
        //mAudio.mute = true;
        // マイクが Ready になるまで待機（一瞬）
        while (Microphone.GetPosition(null) <= 0) {}
        // 再生開始（録った先から再生、スピーカーから出力するとハウリングします）
        mAudio.Play();
    }

    void Update () {
        CalcLoudness();
        CalcAveVoiceVolume();
        //Debug.Log(loudness);
        //Debug.Log(mAudio.volume);
    }
 
    //現フレームの音量を計算します.
    //マイクの感度が良すぎる場合は, lastLoudnessInfluence を上げたりして調節しましょう.
    void CalcLoudness() {
        lastLoudness = loudness;
        loudness = GetAveragedVolume() * sensitivity * ( 1 - lastLoudnessInfluence )
                   + lastLoudness * lastLoudnessInfluence;
    }

    //ユーザーの声の平均的な音量を計算する
    public void CalcAveVoiceVolume ()
    {
        m_aveVoice += lastLoudness;
        second -= Time.deltaTime;
        if (second <= 0.0f)
        {
            m_aveVoice /= second;
        }
    }
 
    //現フレームで再生されている AudioClip から平均的な音量を取得します.
    float GetAveragedVolume()
    {
        //AudioClip の情報を格納する配列.
        //256は適当です.少なすぎれば平均的なサンプルデータが得られなくなるかもしれず,
        //多すぎれば計算量が増えますので良い感じに...
        float[] data = new float[256];
        //最終的に返す音量データ.
        float a = 0;
        //AudioClipからデータを抽出します.
        var audio = GetComponent<AudioSource>();
        audio.GetOutputData(data, 0);
        //音データを絶対値に変換します.
        foreach(float s in data)
        {
            a += Mathf.Abs(s);
        }
        //平均を返します.
        return a/256;
    }
}
