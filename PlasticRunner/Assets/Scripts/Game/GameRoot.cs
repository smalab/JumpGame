using UnityEngine;
using System.Collections;

// ゲームの進行管理.
public class GameRoot : MonoBehaviour
{

    private PlayerControl player = null;
    //private ScoreControl score_control = null;

    // ---------------------------------------------------------------- //

    public enum STEP
    {

        NONE = -1,

        PLAY = 0,       // ゲーム中.
        WAIT_CLICK,     // ゲームオーバー後、クリック待ち.
        RESULT,         // リザルトへ.

        NUM,
    };

    public STEP step = STEP.NONE;
    public STEP next_step = STEP.NONE;
    public float step_timer = 0.0f;

    private SoundControl sound_control = null;

    // ================================================================ //
    // MonoBehaviour からの継承.

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        //this.score_control = this.gameObject.GetComponent<ScoreControl>();
        this.sound_control = GameObject.Find("SoundRoot").GetComponent<SoundControl>();

        this.next_step = STEP.PLAY;
    }
    void Update()
    {
        // ---------------------------------------------------------------- //
        // ステップ内の経過時間を進める.

        this.step_timer += Time.deltaTime;

        // ---------------------------------------------------------------- //
        // 次の状態に移るかどうかを、チェックする.


        if (this.next_step == STEP.NONE)
        {

            switch (this.step)
            {

                case STEP.PLAY:
                    {
                        if (this.player.isPlayEnd())
                        {

                            this.next_step = STEP.WAIT_CLICK;
                        }
                    }
                    break;

                case STEP.WAIT_CLICK:
                    {
                        if (Input.GetMouseButtonDown(0))
                        {

                            this.next_step = STEP.RESULT;
                        }
                    }
                    break;

            }
        }

        // ---------------------------------------------------------------- //
        // 状態が遷移したときの初期化.

        while (this.next_step != STEP.NONE)
        {

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            switch (this.step)
            {

                case STEP.PLAY:
                    {
                        this.sound_control.playBgm(Sound.BGM.PLAY);
                    }
                    break;

                //case STEP.WAIT_CLICK:
                //    {
                //        this.sound_control.stopBgm();
                //    }
                //    break;

                //case STEP.RESULT:
                //    {
                //        // 今回のスコアーを記録しておく.
                //        ScoreControl.Score score = this.score_control.getCurrentScore();
                //        GlobalParam.getInstance().setLastScore(score);
                //        Application.LoadLevel("ResultScene");
                //    }
                //    break;
            }

            this.step_timer = 0.0f;
        }

        // ---------------------------------------------------------------- //
        // 各状態での実行処理.

        switch (this.step)
        {

            case STEP.PLAY:
                {
                }
                break;
        }
    }

    // ================================================================ //

    public float getPlayTime()
    {
        float time;

        if (this.step == STEP.PLAY)
        {

            time = this.step_timer;

        }
        else
        {

            time = 0.0f;
        }

        return (time);
    }
}
