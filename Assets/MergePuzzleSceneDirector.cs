using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MergePuzzleSceneDirector : MonoBehaviour
{
    // アイテムのプレハブ
    [SerializeField] List<BubbleController> prefabBubbles;
    // UI
    [SerializeField] TextMeshProUGUI textScore;
    [SerializeField] GameObject panelResult;
    // Audio
    [SerializeField] AudioClip seDrop;
    [SerializeField] AudioClip seMerge;

    // スコア
    int score;
    // 現在のアイテム
    BubbleController currentBubble;
    // 次のアイテム
    BubbleController nextBubble;
    // 生成位置
    const float SpawnItemY = 3.5f;
    // Nextバブル表示位置
    Vector2 nextBubblePosition = new Vector2(6, 3);
    // Audio再生装置
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // サウンド再生用
        audioSource = GetComponent<AudioSource>();

        // リザルト画面非表示
        panelResult.SetActive(false);

        // 最初のNextバブルを生成
        nextBubble = SpawnItem(nextBubblePosition);
        nextBubble.GetComponent<Rigidbody2D>().gravityScale = 0;
        nextBubble.GetComponent<Collider2D>().enabled = false; // 衝突判定オフ

        // 最初のアイテムを生成
        StartCoroutine(SpawnCurrentItem());
    }

    // Update is called once per frame
    void Update()
    {
        // アイテムがなければここから下の処理はしない
        if (!currentBubble) return;

        // マウスポジション(スクリーン座標)からワールド座標に変換
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // x座標をマウスに合わせる
        Vector2 bubblePosition = new Vector2(worldPoint.x, SpawnItemY);

        // x座標の可動域
        float minX = -2.5f;
        float maxX = 2.5f;
        bubblePosition.x = Mathf.Clamp(bubblePosition.x, minX, maxX);
        currentBubble.transform.position = bubblePosition;

        // タッチ処理
        if (Input.GetMouseButtonUp(0))
        {
            // 重力をセットしてドロップ
            currentBubble.GetComponent<Rigidbody2D>().gravityScale = 1;
            // 所持アイテムリセット
            currentBubble = null;
            // 次のアイテム
            StartCoroutine(SpawnCurrentItem());
            // SE再生
            audioSource.PlayOneShot(seDrop);
        }
    }

    // アイテム生成
    BubbleController SpawnItem(Vector2 position, int colorType = -1)
    {
        // 色ランダム
        int index = Random.Range(0, prefabBubbles.Count / 2);

        // 色の指定があれば上書き
        if (0 < colorType)
        {
            index = colorType;
        }

        // 生成
        BubbleController bubble = Instantiate(prefabBubbles[index], position, Quaternion.identity);

        // 必須データセット
        bubble.SceneDirector = this;
        bubble.ColorType = index;

        return bubble;
    }

    // 所持アイテム生成
    IEnumerator SpawnCurrentItem()
    {
        // 指定された秒数を待つ
        yield return new WaitForSeconds(1.0f);

        // NextバブルをCurrentに移動
        currentBubble = nextBubble;
        currentBubble.transform.position = new Vector2(0, SpawnItemY);
        currentBubble.GetComponent<Rigidbody2D>().gravityScale = 0;
        currentBubble.GetComponent<Collider2D>().enabled = true; // 衝突判定オン

        // 新しいNextバブルを生成して右上に表示
        nextBubble = SpawnItem(nextBubblePosition);
        nextBubble.GetComponent<Rigidbody2D>().gravityScale = 0;
        nextBubble.GetComponent<Collider2D>().enabled = false; // 衝突判定オフ
    }

    // アイテムを合体させる
    public void Merge(BubbleController bubbleA, BubbleController bubbleB)
    {
        // 操作中のアイテムとぶつかったらゲームオーバー
        if (currentBubble == bubbleA || currentBubble == bubbleB)
        {
            // このUpdateに入らないようにする
            enabled = false;
            // リザルトパネル表示
            panelResult.SetActive(true);

            return;
        }

        // マージ済
        if (bubbleA.IsMerged || bubbleB.IsMerged) return;

        // 違う色
        if (bubbleA.ColorType != bubbleB.ColorType) return;

        // 次に生成する色が用意してあるリストの最大数を超える
        int nextColor = bubbleA.ColorType + 1;
        if (prefabBubbles.Count - 1 < nextColor) return;

        // 2点間の中心
        Vector2 leapPosition = Vector2.Lerp(bubbleA.transform.position, bubbleB.transform.position, 0.5f);

        // 新しいアイテムを生成
        BubbleController newBubble = SpawnItem(leapPosition, nextColor);

        // マージ済フラグON
        bubbleA.IsMerged = true;
        bubbleB.IsMerged = true;

        // シーンから削除
        Destroy(bubbleA.gameObject);
        Destroy(bubbleB.gameObject);

        // 点数計算と表示更新
        score += newBubble.ColorType * 10;
        textScore.text = "" + score;

        // SE再生
        audioSource.PlayOneShot(seMerge);
    }

    // リトライボタン
    public void OnClickRetry()
    {
        SceneManager.LoadScene("MergePuzzleScene");
    }
}
