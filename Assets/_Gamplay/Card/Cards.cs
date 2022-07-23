using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{
    public static class Place {
        public static List<Card> Cards => GameData.I.PlaceOf(GameData.I.PlaceKey);

        public static bool Has(Card card) => Cards.Contains(card);
        public static void Add(Card card) => CardsExtension.Add(Cards, card);
        public static T Add<T>() where T : Card, new() => CardsExtension.Add<T>(Cards);
        public static void Add(string key, long second = -1) => Add(new Card(key, second));

        public static bool Remove(Card card) => CardsExtension.Remove(Cards, card);

        public static string Key {
            get => GameData.I.PlaceKey;
            set {
                if (GameData.I.PlaceKey != value) {

                    GameData.I.PlaceKey = value;

                    if (Cards.Count == 0) {

                        Card card = new Card(value);
                        Cards.Add(card);
                    }

                    if (!GameEntry.Loading) {
                        UISystem.I.Rerender = true;

                        string content = $"抵达 {value}";
                        Notice.Content = content;
                        UI.TextContent = content;
                    }
                }
            }
        }
    }

    public static class Hand
    {
        public static List<Card> Cards => GameData.I.PlaceOf(GameData.I.HandKey);

        public static bool Has(Card card) => Cards.Contains(card);
        public static void Add(Card card) => CardsExtension.Add(Cards, card);
        public static T Add<T>() where T : Card, new() => CardsExtension.Add<T>(Cards);
        public static void Add(string key, long second = -1) => Add(new Card(key, second));
        public static bool Remove(Card card) => CardsExtension.Remove(Cards, card);

        public static string Key {
            get => GameData.I.HandKey;
            set {
                A.Assert(GameData.I.HandKey == null);

                GameData.I.HandKey = value;
                UISystem.I.Rerender = true;
            }
        }
    }

    public struct CardsExtension
    {
        public List<Card> Cards;
        public static CardsExtension Of(string place) => new CardsExtension { Cards = GameData.I.PlaceOf(place) };


        public static void Add(List<Card> cards, Card card) {
            cards.Add(card);
            UISystem.I.Rerender = true;
        }

        public T Add<T>() where T : Card, new() {
            T result = new T();
            result.BirthFrame = Time.frameCount;
            Cards.Add(result);
            return result;
        }

        public static T Add<T>(List<Card> cards) where T : Card, new() {
            T card = new T();
            cards.Add(card);
            UISystem.I.Rerender = true;
            return card;
        }

        public static bool Remove(List<Card> cards, Card card) {
            UISystem.I.Rerender = true;
            return cards.Remove(card);
        }
    }

}


