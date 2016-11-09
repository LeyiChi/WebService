using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebService.CommonLibrary
{
    public class TextLog
    {
        #region "< Constructor >"

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TextLog()
        {
        }

        #endregion

        #region "< Private Const >"


        private const int DEFAULT_RETRY = 1;

        private const int DEFAULT_SAVEDAYS = 30;
        #endregion

        #region "< Private Variable >"

        /// <summary>
        /// Log出力文字列格納用
        /// </summary>

        private static Queue<string> logQueue = new Queue<string>();
        /// <summary>
        /// Log出力中判定フラグ
        /// </summary>

        private static bool logWriting = false;
        #endregion

        #region "< Public Property >"

        private static string _LogFileDirectory = string.Empty;
        public static string LogFileDirectory
        {
            get { return _LogFileDirectory; }
        }


        private static string _BaseFileName = string.Empty;
        public static string BaseFileName
        {
            get { return _BaseFileName; }
        }

        private static int _Retry = DEFAULT_RETRY;
        public static int Retry
        {
            get { return _Retry; }

            set
            {
                // 0以下、11以上は異常値
                if (value <= 0 || value >= 11)
                {
                    // 初期値に設定
                    value = DEFAULT_RETRY;
                }

                _Retry = value;
            }
        }

        private static int _SaveDays = DEFAULT_SAVEDAYS;
        public static int SaveDays
        {
            get { return _SaveDays; }

            set
            {
                // 0以下、366以上は異常値
                if (value <= 0 || value >= 366)
                {
                    // 初期値に設定
                    value = DEFAULT_SAVEDAYS;
                }

                _SaveDays = value;
            }
        }

        private static string _LogSeparator = "\t";
        public static string LogSeparator
        {
            get { return _LogSeparator; }
            set { _LogSeparator = value; }
        }

        #endregion

        #region "< Private Method >"

        /// <summary>
        /// QueueからFileへLog出力
        /// </summary>
        private static void WriteQueueToFile()
        {
            // 出力中判定
            if (logWriting)
            {
                // 出力中の場合は終了
                return;
            }

            // 出力中に設定
            logWriting = true;

            try
            {
                // Logファイル作成
                using (StreamWriter sw = new StreamWriter(GetLogFileFullPath(), true, System.Text.Encoding.Default))
                {
                    // Queueをロック
                    lock (((System.Collections.ICollection)logQueue).SyncRoot)
                    {
                        // Queueからデータがなくなるまで出力
                        while (logQueue.Count > 0)
                        {
                            try
                            {
                                // Log出力
                                sw.WriteLine(logQueue.Dequeue());
                            }
                            catch
                            {
                                // ファイルアクセス例外は出力を停止
                                break; // TODO: might not be correct. Was : Exit Try
                            }
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                }
            }
            catch
            {
                // 例外は無視
            }
            finally
            {
                // 出力済に設定
                logWriting = false;
            }
        }

        /// <summary>
        /// Logファイルのフルパスを取得
        /// </summary>
        /// <returns>Logファイルのフルパス</returns>
        private static string GetLogFileFullPath()
        {
            // ファイル名作成(日付(yyyyMMdd) + ベースネーム + .log)
            string s = DateTime.Today.ToString("yyyyMMdd") + _BaseFileName + ".log";

            // Logファイルのフルパスを返す
            return Path.Combine(_LogFileDirectory, s);
        }

        #endregion

        #region "< Public Method >"

        /// <summary>
        /// Log出力関数の初期化
        /// </summary>
        /// <param name="logDirectory">ログ出力先ディレクトリ</param>
        /// <param name="baseName">Logファイルのベースネーム</param>
        public static void Initialize(string logDirectory, string baseName)
        {
            // オーバーロード+1を実行
            Initialize(logDirectory, baseName, _Retry, _SaveDays);
        }

        /// <summary>
        /// Log出力関数の初期化(オーバーロード+1)
        /// </summary>
        /// <param name="logDirectory">ログ出力先ディレクトリ</param>
        /// <param name="baseName">Logファイルのベースネーム</param>
        /// <param name="retry">リトライ回数</param>
        /// <param name="saveDays">ログ保存期間</param>
        public static void Initialize(string logDirectory, string baseName, int retry, int saveDays)
        {
            // 前後の空白を削除
            logDirectory = logDirectory.Trim();
            baseName = baseName.Trim();

            // Log出力先情報変更
            // 前回値チェック
            // ディレクトリ、ベースネームが設定されていなければ
            // 初回なので何もしない
            if ((_LogFileDirectory.Length == 0) && (_BaseFileName.Length == 0))
            {
            }
            else if ((_LogFileDirectory.Length == 0) && (_BaseFileName.Length > 0))
            {
                // ベースネームのみ設定されている場合
                if (!_BaseFileName.Equals(baseName))
                {
                    // ベースネームが前回値と違えばたまっているログ出力
                    FlushLog();
                }
            }
            else if ((_LogFileDirectory.Length > 0) && (_BaseFileName.Length == 0))
            {
                // ディレクトリのみ設定されている場合
                if (!_LogFileDirectory.Equals(logDirectory))
                {
                    // ディレクトリが前回値と違えばたまっているログ出力
                    FlushLog();
                }
            }
            else
            {
                // 両方設定されている場合
                if (!_LogFileDirectory.Equals(logDirectory))
                {
                    // ディレクトリが前回値と違えばたまっているログ出力
                    FlushLog();
                }
                else if (!_BaseFileName.Equals(baseName))
                {
                    // ベースネームが前回値と違えばたまっているログ出力
                    FlushLog();
                }
            }

            // Log出力先ディレクトリ検索
            if (!Directory.Exists(logDirectory))
            {
                // Log出力先ディレクトリ作成
                Directory.CreateDirectory(logDirectory);
            }

            // 値を保存
            _LogFileDirectory = logDirectory;
            _BaseFileName = baseName;
            _Retry = retry;
            _SaveDays = saveDays;
        }

        /// <summary>
        /// Log出力
        /// </summary>
        /// <param name="log">出力するLog</param>
        public static void WriteLog(string log)
        {
            // Queueをロック
            lock (((System.Collections.ICollection)logQueue).SyncRoot)
            {
                // 日時を取得
                log = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + _LogSeparator + log;

                // LogをQueueに格納
                logQueue.Enqueue(log);
            }

            // Log出力
            WriteQueueToFile();
        }

        /// <summary>
        /// 溜まっているLogを全て出力
        /// </summary>
        public static void FlushLog()
        {
            // Queueをロック
            lock (((System.Collections.ICollection)logQueue).SyncRoot)
            {
                // Retry回数分行う
                for (int i = 0; i <= _Retry - 1; i++)
                {
                    // 出力対象判定
                    if (logQueue.Count == 0)
                    {
                        break; // TODO: might not be correct. Was : Exit For
                    }

                    // Log出力
                    WriteQueueToFile();
                }

                // Queueクリア
                logQueue.Clear();
            }
        }

        /// <summary>
        /// Logファイルの削除
        /// </summary>
        public static void DeleteLog()
        {
            // オーバーロード+1を実行
            // BaseNameが一致するもののみ
            DeleteLog(false);
        }

        /// <summary>
        /// Logファイルの削除(オーバーロード+1)
        /// </summary>
        /// <param name="allLogs">全てのログの削除判定 true:全て / false:BaseNameが一致するもののみ</param>
        public static void DeleteLog(bool allLogs)
        {
            // 削除基準日を設定
            DateTime dt = DateTime.Now.AddDays(_SaveDays * -1);

            // ログファイル名
            string fileName = "????????";
            // BaseNameの判定
            if (allLogs)
            {
                // 全てのファイル
                fileName += "*.log";
            }
            else
            {
                // BaseNameを含める
                fileName += _BaseFileName + ".log";
            }

            // ログディレクトリ内の"*.log"ファイルを取得
            foreach (string s in Directory.GetFiles(_LogFileDirectory, fileName))
            {
                // ファイル情報を取得
                FileInfo fi = new FileInfo(s);

                // ファイルの最終更新日付と削除基準日とを比較
                if (fi.LastWriteTime <= dt)
                {
                    try
                    {
                        // ファイル削除
                        fi.Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion    }
    }
}