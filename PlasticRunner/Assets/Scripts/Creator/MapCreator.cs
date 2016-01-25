using UnityEngine;
using System.Collections;

// ブロック.
public class Block
{

    // 種類.
    public enum TYPE
    {

        NONE = -1,

        FLOOR = 0,          // 床.
        HOLE,               // 穴.

        NUM,
    };
};

// マップツクラー（ツクラー = 作る + er）.
public class MapCreator : MonoBehaviour
{

    public TextAsset level_data_text = null;

    // ================================================================ //

    public static float BLOCK_WIDTH = 1.0f;     // ブロックの幅　（X方向のサイズ）.
    public static float BLOCK_HEIGHT = 0.2f;        // ブロックの高さ（Y方向のサイズ）.
    public static int BLOCK_NUM_IN_SCREEN = 24; // 一画面中のブロックの数（横方向）.


    // 床ブロック.
    // ブロックがない場所を『空のブロックがある』とすることで、
    // ブロックのある場所と同じように扱えるようにする.
    private struct FloorBlock
    {

        public bool is_created;                     // false のときはいっこもブロックが作られてない.	
        public Vector3 position;                        // 位置.
    };

    private PlayerControl player = null;                // プレイヤー.
    private FloorBlock last_block;                  // 最後に作ったブロック.

    private GameRoot game_root = null;      // ゲームの進行管理.
    private LevelControl level_control = null;      // ブロック配置の管理（マップパターン。次に作るブロックのタイプを決める）.
    private BlockCreator block_creator = null;      // ブロックツクラー.
    //private CoinCreator coin_creator = null;        // コインツクラー.
    //private EnemyCreator enemy_creator = null;      // おじゃまキャラツクラー.

    // ================================================================ //
    // MonoBehaviour からの継承.

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        //

        this.last_block.is_created = false;

        this.level_control = gameObject.AddComponent<LevelControl>();
        this.level_control.initialize();
        this.level_control.loadLevelData(this.level_data_text);

        this.player.level_control = this.level_control;

        //

        this.game_root = this.gameObject.GetComponent<GameRoot>();
        this.block_creator = this.gameObject.GetComponent<BlockCreator>();
        //this.coin_creator = this.gameObject.GetComponent<CoinCreator>();
        //this.enemy_creator = this.gameObject.GetComponent<EnemyCreator>();

        this.block_creator.map_creator = this;
        //this.coin_creator.map_creator = this;
        //this.enemy_creator.map_creator = this;

        //

        this.create_floor_block();
    }

    void Update()
    {
        // DebugPrint.print("time  " + ((int)this.game_root.getPlayTime()).ToString());
        // DebugPrint.print("speed " + (this.level_control.getPlayerSpeed()).ToString());
        // DebugPrint.print("level " + this.level_control.current_level.ToString());

        // -------------------------------------------------------------------- //
        // プレイヤーと『最後につくったブロック』の距離がある程度近づいたら
        // 次のブロックを作る.

        float block_generate_x = this.player.transform.position.x;

        // プレイヤーの前方（左の方、だいたい画面の左端）.
        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;

        // 無限ループ防止用カウンター.
        int fail_safe_count = 100;

        while (this.last_block.position.x < block_generate_x)
        {

            this.create_floor_block();

            // プログラムにバグがあっても無限ループになってしまわないよう、
            // ある程度以上実行したら強制的に打ち切る.

            fail_safe_count--;

            if (fail_safe_count <= 0)
            {

                break;
            }
        }
    }

    // ================================================================ //

    // ブロック/コインを消す？.
    public bool isDelete(GameObject block_object)
    {
        bool ret = false;

        float left_limit = this.player.transform.position.x - BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

        // プレイヤーよりも一定距離以上左側になったら
        // （画面左端から外に出たら）消す.
        if (block_object.transform.position.x < left_limit)
        {

            ret = true;
        }

        // 画面の下に消えたら消す.
        if (block_object.transform.position.y < PlayerControl.NARAKU_HEIGHT)
        {

            ret = true;
        }

        return (ret);
    }

    // ================================================================ //

    // ブロックを作る.
    private void create_floor_block()
    {
        Vector3 block_position;

        // -------------------------------------------------------------------- //
        // 次につくるブロックのタイプ（床 or 穴）を決める.

        this.level_control.update(this.game_root.getPlayTime());

        // -------------------------------------------------------------------- //
        // ブロックの位置.

        // 『最後に（直前に）』作ったブロックの位置を取得する.
        if (!this.last_block.is_created)
        {

            // まだひとつもブロックを作っていないときは、
            // 『プレイヤーの後ろ、スクリーンの左端』を基準にする.
            block_position = this.player.transform.position;
            block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);

        }
        else
        {

            block_position = this.last_block.position;
        }

        block_position.x += BLOCK_WIDTH;
        block_position.y = (float)this.level_control.current_block.height * BLOCK_HEIGHT;

        // -------------------------------------------------------------------- //
        // ブロックのゲームオブジェクトを作る.

        LevelControl.CreationInfo current = this.level_control.current_block;

        this.block_creator.createBlock(current, block_position);

        // -------------------------------------------------------------------- //
        // コインを作る.

        //LevelData level_data = this.level_control.getCurrentLevelData();

        //this.coin_creator.createCoin(level_data, this.level_control.block_count, block_position);

        // -------------------------------------------------------------------- //
        // おじゃまキャラを作る.

        //this.enemy_creator.createEnemy(level_data, this.level_control.block_count, block_position);

        // -------------------------------------------------------------------- //
        // 『最後に作ったブロックの位置』を更新しておく.

        this.last_block.position = block_position;
        this.last_block.is_created = true;

        // -------------------------------------------------------------------- //
    }
}
