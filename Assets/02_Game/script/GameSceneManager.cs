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
        //自身が先手か否か。(先手A,後手B)
        public bool isFirst = false;

        [SerializeField] private Camera m_camera = null;
        [SerializeField] private Vector3[] m_cameraOffset = null;

        //現在の試合状況
        [NonSerialized] public Matches nowMatches = new Matches();

        //試合前の状態
        [NonSerialized] public MatchesInfo matchesInfo = new MatchesInfo();

        //private List<Command> m_cmd = null;
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
            const int logRange = 13;
            if (m_logCnt > logRange)
            {
                outputLogField.text =
                    outputLogField.text.Substring(outputLogField.text.IndexOf("\n") + 1);
            }

            //エラーごとに色分け
            Color[] levelColor = { Color.white, Color.yellow, Color.red,Color.green, };
            //ログ出力
            outputLogField.text += $"<color=#{levelColor[(int)level].ToHexString()}> "
                                   + $"{m_logCnt.ToString().PadRight(7)}: "
                                   + $"{message}</color>\n";
        }

        /// <summary>
        /// カメラをフィールド中央に設置
        /// </summary>
        public void SetFieldCenterCameraPosition()
        {
            int fieldSize = GameManager.Instance.fieldSize;
            Vector3 cameraPos = m_camera.transform.position;
            float chipHalf = 0.5f;

            //X軸方向の位置調整
            m_camera.transform.position = ((float)fieldSize / 2.0f - chipHalf) * Vector3.right;
            //Y-Z軸方向の位置調整
            m_camera.transform.position += m_cameraOffset[GameManager.Instance.fieldSizeIndex];
        }

        /// <summary>
        /// ボタンクリックでActionを更新する(１ターンごとに有効)
        /// </summary>
        public void PostCommandData()
        {
            //クライアントの準備が出来ていない場合はスルー
            if (!ClientManager.Instance.ablePost) return;

            //職人数
            int mason = (ClientManager.Instance.hostType == 1)
                ? matchesInfo.matches.board.mason
                : nowMatches.board.mason;
            //int mason = nowMatches.board.mason;
            //DebugEx.Log(mason);
            
            //アクション　方向　インデックス
            const int act = 0;
            const int dir = 1;
            //データの数（アクション,方向）２
            const int elm_cnt = 2;
            //コマンド文字列取得
            List<Command> cmds = new List<Command>();
            //文字列からコマンド群へ変換
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
                //cmd_data デバッグ用
                //DebugEx.Log($"mason[{i}] cmd:{cmd_data[0]} act:{cmd_data[1]}");

                //Command _cmd = new Command { act = cmd_data[act], dir = cmd_data[dir] };
                cmds.Add(new Command { actType = cmd_data[act], dir = cmd_data[dir] });
            }

            //POSTができたかどうか?
            if (ClientManager.Instance.PostCommandJson(m_nowTurn + 1, cmds.ToArray()))
            {
                ShowLogMessage("POST完了");
            }
            else ShowLogMessage("POSTエラー", Utility.Level.Error);
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
        
        
        private float turnPastTime = 0.0f;
        private float nextTurnTime = 3.0f;

        void Start()
        {
            SetFieldCenterCameraPosition();
            nextTurnTime = GameSceneManager.Instance.matchesInfo.matches.turnSeconds;
        }
        private void Update()
        {
            //サーバー接続可能までスルー
            if (!ClientManager.Instance.ablePost) return;

            //入力化の時間のカウントダウン(次のターンまでの残り時間)
            turnPastTime += Time.deltaTime;
            nextTurnTimer.text = $"ターン猶予時間{(nextTurnTime - turnPastTime).ToString("f2")}秒";

            //ターン更新時にフィールドを更新
            if (nowMatches.turn != m_nowTurn)
            {
                turnPastTime = 0.0f;
                m_nowTurn = nowMatches.turn;
                //DebugEx.Log($"turn change! {m_nowTurn}");
                turnCnt.text = $"現在 {m_nowTurn}ターン目";
                ShowLogMessage($"ターン更新！ {m_nowTurn}ターン", Utility.Level.PopUp);
                MapCreator.Instance.SetGameField();
            }
        }
    }
}