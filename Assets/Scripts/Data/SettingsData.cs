
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{

    [JsonObject(MemberSerialization.Fields)]
    public class SettingsData
    {
        public static SettingsData I;

        //private float masterVolume = 0.5f;
        //private float soundVolume = 0.5f;
        //private float musicVolume = 0.5f;
        //private int ToPercent(float x) => (int)(100 * x);




    }
}
