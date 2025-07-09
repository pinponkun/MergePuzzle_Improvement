using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    // シーンディレクター
    public MergePuzzleSceneDirector SceneDirector;
    // カラー
    public int ColorType;
    // マージ済フラグ
    public bool IsMerged;

    public float OverLineDuration = 3f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 画面外に落ちたらゲームオーバー
        if (transform.position.y < -10)
        {
            if (SceneDirector != null)
            {
                SceneDirector.GameOver();
            }
            Destroy(gameObject);
            return;
        }
    }

    // 当たり判定が発生したら呼ばれる
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // バブルじゃない
        BubbleController bubble = collision.gameObject.GetComponent<BubbleController>();
        if (!bubble) return;

        // 合体させる
        SceneDirector.Merge(this, bubble);
    }
}
