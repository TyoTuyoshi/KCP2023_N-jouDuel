using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseSystem;
using BaseSystem.Utility;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace KCP2023
{
    public class GameSceneManager : SingletonBase<GameSceneManager>
    {
        //自身が先手か否か。(先手A,後手B)
        public bool isFirst = false;
        
        [SerializeField] private Camera m_camera = null;
        [SerializeField] private Vector3[] m_cameraOffset = null;

        //現在の試合状況
        [NonSerialized] public Matches  nowMatches = new Matches();

        //試合前の状態
        [NonSerialized] public MatchesInfo matchesInfo = new MatchesInfo();

        //private List<Command> m_cmd = null;
        //入力欄リスト
        public List<TMP_InputField> inputFields = new List<TMP_InputField>();
        //ログ表示リスト
        [SerializeField] private TextMeshProUGUI outputLogField = new TextMeshProUGUI();

        public int nowTurnCnt = 0;

        private int m_logCnt = 0;

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
                    outputLogField.text.Substring(outputLogField.text.IndexOf("\n")+1);
            }
            //エラーごとに色分け
            Color[] levelColor = { Color.white, Color.yellow, Color.red };
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
            //職人数
            int mason = (ClientManager.Instance.hostType == 1)
                ? matchesInfo.matches.board.mason
                : nowMatches.board.mason;
            //int mason = nowMatches.board.mason;
            
            DebugEx.Log(mason);
            //アクション　方向　インデックス
            const int act = 0;
            const int dir = 1;
            //データの数（アクション,方向）２
            const int elm_cnt = 2;
            //コマンド文字列取得
            List<Command> cmds = new List<Command>();
            //文字列からコマンド群へ変換
            //todo 職人の数分ループ
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

            if (ClientManager.Instance.PostCommandJson(nowTurnCnt, cmds.ToArray()))
            {
                ShowLogMessage("POST完了");
            }
            else ShowLogMessage("POSTエラー", Utility.Level.Error);
        }

        /// <summary>
        /// コマンド初期化
        /// </summary>
        private void InitCommands()
        {
            //大工の数分コマンドを確保
            //todo null修正
            int size = 2;// GameSceneManager.Instance.nowMatches.board.mason;
            //m_cmd = new List<Command>(size);
        }

        void Start()
        {
            SetFieldCenterCameraPosition();
            InitCommands();
        }

        private void Update()
        {
            if (isFirst) ;
        }
    }
}