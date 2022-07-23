
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace W
{
    public class GameEntry : MonoBehaviour
    {
        public static GameEntry I { get; private set; }
        private void Awake() {
            A.Assert(I == null);
            I = this;
        }

        private void Start() {
            // 保证存档文件夹存在
            TryCreateFolder(SaveFolder);
            TryCreateFolder(ModFolder);

            // 读取默认存档
            LoadQuickSave();
        }
        private void TryCreateFolder(string folder) {
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// 每渲染帧调用
        /// </summary>
        private void Update() {
            UISystem.I.Frame();
            Tick();
        }


        /// <summary>
        /// 逻辑帧间隔
        /// </summary>
        private const float DeltaTime = 0.1f;
        private float timeCount;
        /// <summary>
        /// 每逻辑帧调用
        /// </summary>
        private void Tick() {
            timeCount += Time.deltaTime;
            if (timeCount >= DeltaTime) {
                timeCount -= DeltaTime;
                GameData.I.Tick();
            }
        }

        public string ModFolder => $"{Application.persistentDataPath}/mods/";
        public string SaveFolder => $"{Application.persistentDataPath}/saves/";
        public string QuickSaveFile => $"{Application.persistentDataPath}/quicksave.txt";
        public string SettingsSaveFile => $"{Application.persistentDataPath}/settings.json";
        public long QuickSaveContent { 
            get {
                if (!File.Exists(QuickSaveFile)) {
                    return 0;
                }
                if (!long.TryParse(File.ReadAllText(QuickSaveFile), out long ticks)) {
                    return 0;
                }
                return ticks;
            }
        }

        private string SavePathOf(string s) => $"{SaveFolder}{s}";


        private const int maxSaveFileCount = 32;
        public List<string> GetAllSaveFilenames(string folder) {
            List<string> result = null;

            DirectoryInfo dir = new DirectoryInfo(folder);
            if (!dir.Exists) {
                return result;
            }
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            if (fileinfo.Length == 0) {
                return result;
            }

            result = new List<string>();

            int i = 0;
            foreach (FileSystemInfo info in fileinfo) {
                if (i >= maxSaveFileCount) break;

                if (info is DirectoryInfo) {
                    //DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                } else {
                    result.Add(info.Name);
                }
                i++;
            }

            return result;
        }

        public bool HasFile(string filename) {
            return File.Exists(SavePathOf(filename));
        }
        public void DeleteFile(string filename) {
            if (HasFile(filename)) {
                File.Delete(SavePathOf(filename));
            }
        }
        public int DeleteAll(string folder, List<string> filenames) {
            DirectoryInfo dir = new DirectoryInfo(folder);
            if (!dir.Exists) {
                return 0;
            }
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            if (fileinfo.Length == 0) {
                return 0;
            }
            foreach (FileSystemInfo info in fileinfo) {
                if (info is DirectoryInfo) {
                    //DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                } else {
                    // DeleteSave(info.Name);
                    filenames?.Add(info.Name);
                    info.Delete();
                }
            }
            return fileinfo.Length;
        }

        public GameData LoadSave(string filename) {
            string filepath = SavePathOf(filename);

            if (!File.Exists(filepath)) {
                return null;
            }

            // 读取游戏
            string game = File.ReadAllText(filepath);
            GameData gameData = JsonConvert.DeserializeObject<GameData>(game, Settings);

            // 读取设置
            string settings = File.ReadAllText(SettingsSaveFile);
            SettingsData settingsData = JsonConvert.DeserializeObject<SettingsData>(settings, Settings);
            SettingsData.I = settingsData;

            return gameData;
        }
        public void Save() {
            // 序列化 游戏 和 设置
            string game = JsonConvert.SerializeObject(GameData.I, Settings);
            string settings = JsonConvert.SerializeObject(SettingsData.I, Settings);

            // 存档名是ticks
            string filename = $"{C.Now}";

            File.WriteAllText(SavePathOf(filename), game);
            File.WriteAllText(QuickSaveFile, filename);
            File.WriteAllText(SettingsSaveFile, settings);
        }

        public static bool Loading { get; private set; }

        public void LoadQuickSave() {

            Loading = true;

            // 有存档，则读取存档
            if (File.Exists(QuickSaveFile)) {
                string filename = File.ReadAllText(QuickSaveFile);
                GameData.I = LoadSave(filename);
            }
            
            // 无存档，则新建游戏
            if (GameData.I == null) {
                SettingsData.I = new SettingsData();
                GameData.I = new GameData();

                GameData.I.OnAwake();
                GameData.I.OnStart();
            }
            else {
                GameData.I.OnAwake();
                GameData.I.OnLoad();
            }

            Loading = false;
        }


        public void QuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }

        private static JsonSerializerSettings Settings { get; } = new JsonSerializerSettings {
            MaxDepth = 24,
            // ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

            MissingMemberHandling = MissingMemberHandling.Ignore,

            MetadataPropertyHandling = MetadataPropertyHandling.Default, // 序列化自动生成 attribute property getter setter
            ReferenceLoopHandling = ReferenceLoopHandling.Error, // 循环依赖

            Formatting = Formatting.Indented, // 缩进
            DefaultValueHandling = DefaultValueHandling.Ignore, // 不序列化默认值
            TypeNameHandling = TypeNameHandling.Auto, // 只有 object 标出类型
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple, // 短 assembly name
        };

    }
}


