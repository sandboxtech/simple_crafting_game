
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace W
{
    [JsonObject(MemberSerialization = MemberSerialization.Fields)]
    public class Idle
    {
        public Idle(long val, long max = long.MaxValue, long del = C.Second, long inc = 1) {

            this.del = del;
            this.inc = inc;
            this.val = val;
            this.max = max;

            this.time = C.Now;

            Max = max;
            Del = del;
            Inc = inc;
            this.val = val; // dont set Value. 
        }


        [JsonProperty] private long del;
        [JsonProperty] private long inc;
        [JsonProperty] private long val;
        [JsonProperty] private long time;
        [JsonProperty] private long max;

        public string DebugMessage => $"[ val({val}) Value({Value}) max({Max}) inc({Inc}) del({Del})  time({time})    {Progress} ]";


        private void Sync() {
            if (del == 0) return;
            long now = C.Now;
            long turn = (now - time) / Del;
            val += turn * Inc;
            val = M.Clamp(0, Max, val);
            time += turn * Del;
        }

        public long Max {
            get => max;
            set {
                Sync(); max = value;
            }
        }
        public long Inc {
            get => inc;
            set { Sync(); inc = value; }
        }
        public long Del {
            get => del;
            set { Sync(); del = value; }
        }

        public long Value {
            get {
                long turn = (C.Now - time) / Del;
                long result = turn * Inc + val;
                return M.Clamp(0, max, result);
            }
            set {
                if (value >= Max) {
                    val = Max;
                    time = C.Now;
                } else {
                    // time += (Value - value) * Del; // largechange: overflow
                    Sync();
                    if (val == Max) {
                        time = C.Now;
                    }
                    val = value;
                }
            }
        }
        public bool Empty => Value <= 0;
        public bool Maxed => Value >= Max;

        public void Clear() {
            time = C.Now;
            val = 0;
        }


        public long MaxSubValue => Max - Value;

        public float Progress => Value == Max ? 1 : (Value == 0 && Inc == 0) ? 0 : M.Progress(C.Now, time, Del) % 1;

        public float TotalProgress {
            get {
                return Max == 0 ? 0 : M.Clamp01((float)((double)(Value + Progress) / Max));
            }
        }
        public float OneSubTotalProgress => 1 - TotalProgress;
        public float OneSubProgress => 1 - Progress;

    }
}
