using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LaserChess
{
    public static class LevelDataManager
    {
        static readonly string[] levelsSaveFiles = { "level0.json", "level1.json", "level2.json" };

        public static List<LevelData> levelDatas;
        public static int selectedLevelDataIndex;

        public static void ReadData()
        {
            levelDatas = new List<LevelData>();
            /* string jsonPath = "";

            for (int i = 0; i < levelsSaveFiles.Length; i++)
            {
                // C:/Users/.../AppData/LocalLow/DefaultCompany/Unity-Task
                jsonPath = Path.Combine(Application.persistentDataPath, levelsSaveFiles[i]);

                if(File.Exists(jsonPath))
                    Read(jsonPath);
            } */

            CheckLevels();

            selectedLevelDataIndex = 0;
        }

        private static void Read(string jsonPath)
        {
            string jsonFromFile = File.ReadAllText(jsonPath);

            //Deserializing
            levelDatas.Add(JsonUtility.FromJson<LevelData>(jsonFromFile));
        }

        static void CheckLevels()
        {
            if (levelDatas == null)
                levelDatas = new List<LevelData>();
            
            if (levelDatas.Count == 0)
                Create();
        }

        public static LevelData GetLevelData()
        {
            CheckLevels();
            return levelDatas[selectedLevelDataIndex];
        }

        private static void Create()
        {
            levelDatas.Add(new LevelData()
            {
                pieces = new PiecesData[] {
                    new PiecesData {
                        index = 0,
                        positions = new Vector2Int[] { new Vector2Int(1, 1) }
                    },
                    new PiecesData {
                        index = 1,
                        positions = new Vector2Int[] { new Vector2Int(3, 1) }
                    },
                    new PiecesData {
                        index = 2,
                        positions = new Vector2Int[] { new Vector2Int(5, 1) }
                    },
                    new PiecesData {
                        index = 3,
                        positions = new Vector2Int[] { new Vector2Int(2, 6) }
                    },
                    new PiecesData {
                        index = 4,
                        positions = new Vector2Int[] { new Vector2Int(4, 6) }
                    },
                    new PiecesData {
                        index = 5,
                        positions = new Vector2Int[] { new Vector2Int(6, 6) }
                    }
                }
            });
        }
    }
}