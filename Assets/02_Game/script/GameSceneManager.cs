using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseSystem;
using BaseSystem.Utility;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace KCP2023
{
    public class GameSceneManager : SingletonBase<GameSceneManager>
    {
        [SerializeField] private Camera m_camera = null;
        [SerializeField] private Vector3[] m_cameraPos = null;

        //現在の試合状況
        [NonSerialized] public Matches nowMatches = new Matches();

        //入力欄リスト
        public List<TMP_InputField> inputFields = new List<TMP_InputField>();

        //ログ表示リスト
        [SerializeField] private TextMeshProUGUI outputLogField = new TextMeshProUGUI();

        //ターン数表示テキスト
        [SerializeField] private TextMeshProUGUI turnCnt = new TextMeshProUGUI();

        //次のターンまでの残り時間
        [SerializeField] private TextMeshProUGUI nextTurnTimer = new TextMeshProUGUI();

        //ログカウンタ
        private int m_logCnt = 0;

        //現在のターン
        private int m_nowTurn = 0;

        /// <summary>
        /// 操作/実行ログの表示
        /// </summary>
        /// <param name="message">ログ内容</param>
        public void ShowLogMessage(string message, Utility.Level level = Utility.Level.Success)
        {
            //ログナンバー更新
            m_logCnt++;
            //ログ上方範囲削除で更新
            const int logRange = 8;
            const int pad = 6;
            if (m_logCnt > logRange)
            {
                outputLogField.text =
                    outputLogField.text.Substring(outputLogField.text.IndexOf("\n") + 1);
            }

            //エラーごとに色分け
            Color[] levelColor = { Color.white, Color.yellow, Color.red, Color.green, };
            //ログ出力
            outputLogField.text += $"<color=#{levelColor[(int)level].ToHexString()}> "
                                   + $"{m_logCnt.ToString().PadRight(pad)}:"
                                   + $"{message}</color>\n";
        }

        /// <summary>
        /// カメラをフィールド中央に設置
        /// </summary>
        public void SetCameraPosition()
        {
            int fieldSize = GameManager.Instance.gameConfig.nowMatches.size;
            int[] allSize = new[] { 11, 13, 15, 17, 21, 25 };
            int index = Array.IndexOf(allSize, fieldSize);
            m_camera.transform.position = m_cameraPos[index];
        }

        /// <summary>
        /// ボタンクリックでActionを更新する(１ターンごとに有効)
        /// </summary>
        public void PostCommandData()
        {
            try
            {
                //クライアントの準備が出来ていない場合はスルー
                if (!ClientManager.Instance.isStart) return;
                int mason = GameManager.Instance.gameConfig.nowMatches.masons;
                //アクション　方向　インデックス
                const int act = 0;
                const int dir = 1;
                //データの数（アクション,方向）２
                const int elm_cnt = 2;
                //コマンド文字列取得
                List<Command> cmds = new List<Command>();
                for (int i = 0; i < mason; i++)
                {
                    //文字列コマンドをint配列に変換
                    var cmd_data = ArrayUtility.PuckStrToIntArrayEx(inputFields[i].text);
                    //配列サイズが異なる場合のデフォルト修正(エラー回避)
                    if (cmd_data.Length != elm_cnt)
                    {
                        Array.Resize(ref cmd_data, elm_cnt);
                        cmd_data[act] = 0;
                        cmd_data[dir] = 0;
                    }

                    cmds.Add(new Command { actType = cmd_data[act], dir = cmd_data[dir] });
                }

                //POSTができたかどうか?
                if (ClientManager.Instance.PostCommandJson(m_nowTurn + 1, cmds.ToArray()))
                {
                    ShowLogMessage("POST完了");
                }
                else ShowLogMessage("POSTエラー", Utility.Level.Error);
            }
            catch (Exception e)
            {
                DebugEx.Log(e.Message);
            }
        }

        /// <summary>
        /// ランダムな命令を自動でセット
        /// </summary>
        /// <param name="index">テキストのインデックス, -1の場合は全て, -2全コマンドクリア</param>
        public void SetRandomCommand(int index)
        {
            //ランダムコマンド用簡易ラムダ
            Func<string> RandomCmd = () =>
            {
                int rnd_act = Random.Range(1, 4);
                int rnd_dir = Random.Range(1, 8);
                return $"{rnd_act} {rnd_dir}";
            };

            switch (index)
            {
                case -1: //全コマンド更新
                    foreach (var inputs in inputFields) inputs.text = RandomCmd();
                    break;
                case -2: //全コマンド削除
                    foreach (var inputs in inputFields) inputs.text = "";
                    break;
                default: //インデックスのコマンド更新
                    inputFields[index].text = RandomCmd();
                    break;
            }
        }


        private float turnPastTime = 0.0f; //ターン経過時間
        private float turnSeconds = 3.0f; //次のターンまでの時間

        protected override void Init()
        {
            //1ターン時間設定
            turnSeconds = (float)GameManager.Instance.gameConfig.nowMatches.turnSeconds;
        }

        private void Start()
        {
            //タイマー初期化
            nextTurnTimer.text = $"ターン猶予時間 {turnSeconds.ToString("f2")}秒";
            //カメラ位置調整
            SetCameraPosition();
            StartCoroutine(AsyncUpdate());
        }

        /// <summary>
        /// 非同期アップデート
        /// </summary>
        private IEnumerator AsyncUpdate()
        {
            //サーバー接続可能までスルー
            while (!ClientManager.Instance.ablePost) yield return null;
            //接続後に初期マップの配置
            //getリクエスト成功まで送信
            while (!ClientManager.Instance.isStart) yield return null;

            //クライアントマネージャ側で全ターンが終わるまで
            while (!ClientManager.Instance.isEnd)
            {
                //入力化の時間のカウントダウン(次のターンまでの残り時間)
                turnPastTime += Time.deltaTime;
                //0秒への正規化
                if (turnPastTime > turnSeconds) turnPastTime = turnSeconds;
                float limTime = turnSeconds - turnPastTime;
                nextTurnTimer.text = $"ターン猶予時間 {limTime.ToString("f2")}秒";

                //強制コマンドポスト
                const float powerPostTime = 2.0f;
                if (limTime < powerPostTime) PostCommandData();

                //ターン更新時にフィールドを更新
                if (nowMatches.turn != m_nowTurn)
                {
                    turnPastTime = 0.0f;
                    m_nowTurn = nowMatches.turn;
                    //DebugEx.Log($"turn change! {m_nowTurn}");
                    string first = GameManager.Instance.gameConfig.nowMatches.first ? "自分" : "相手";
                    string nowTurn = nowMatches.turn % 2 == 0 ? $"後攻({first})" : $"先攻({first})";
                    turnCnt.text = $"現在 {m_nowTurn}ターン目 {nowTurn}";
                    
                    ShowLogMessage($"ターン更新！ {m_nowTurn}ターン", Utility.Level.PopUp);
                    MapCreator.Instance.SetGameFieldNow();
                }

                yield return null;
            }
        }
    }
}