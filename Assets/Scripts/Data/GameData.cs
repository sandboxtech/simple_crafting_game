
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{

    /// <summary>
    /// 全局对象
    /// </summary>
    [JsonObject(MemberSerialization.Fields)]
    public class GameData
    {
        public static GameData I;

        public Dictionary<string, List<Card>> PlaceDict { get; private set; }

        public long MusicIndex = 0;
        public bool MusicEnabled = true;

        public string HandKey;
        public string PlaceKey;
        public string NoticeContent;

        public string Code;

        public PlayerStatus PlayerStatus;

        private void CreateData() {
            PlaceDict = new Dictionary<string, List<Card>>();
            PlayerStatus = new PlayerStatus();

            MusicIndex = UnityEngine.Random.Range(0, 100);
        }

        public void OnAwake() {
            Config.Build();
        }

        public void OnStart() {

            CreateData();

            // ioc
            World.Build();

            UISystem.I.Rerender = true;

            A.Assert(HandKey != null);
            A.Assert(PlaceKey != null);
            A.Assert(NoticeContent != null);
        }

        public void Tick() => World.Tick();


        public void OnLoad() {
            Notice.Content = NoticeContent;

            SavePage.AfterLoadQuickSave();

            UISystem.I.Rerender = true;
        }

        public List<Card> PlaceOf(string place) {
            if (!PlaceDict.TryGetValue(place, out List<Card> cards)) {
                cards = new List<Card>();
                PlaceDict.Add(place, cards);
            }
            return cards;
        }


    }

}




