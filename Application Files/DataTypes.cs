using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
    public class Profile(int id, string name, int barbellWeight, TimeSpan start, int snatch_SecondsStage, int snatch_OpenerWeight, bool snatch_OpenerInWarmup, int snatch_SecondsEnd, int snatch_LiftsOut, int cJ_SecondsStage, int cJ_SecondsBreak, int cJ_OpenerWeight, bool cJ_OpenerInWarmup, int cJ_SecondsEnd, int cJ_LiftsOut, int cJ_SnatchLifts_Out, bool beep, List<Extra> snatchExtras, Dictionary<int, int> snatchJumps, Dictionary<int, int> snatchTimes, List<Extra> cjExtras, Dictionary<int, int> cjJumps, Dictionary<int, int> cjTimes)
    {
        public int id { get; set; } = id;
        public string Name { get; set; } = name;
        public int BarbellWeight { get; set; } = barbellWeight;
        public TimeSpan Start { get; set; } = start;
        public int snatch_SecondsStage { get; set; } = snatch_SecondsStage;
        public int snatch_OpenerWeight { get; set; } = snatch_OpenerWeight;
        public bool snatch_OpenerInWarmup { get; set; } = snatch_OpenerInWarmup;
        public int snatch_SecondsEnd { get; set; } = snatch_SecondsEnd;
        public int snatch_LiftsOut { get; set; } = snatch_LiftsOut;
        public int cj_SecondsStage { get; set; } = cJ_SecondsStage;
        public int cj_SecondsBreak { get; set; } = cJ_SecondsBreak;
        public int cj_OpenerWeight { get; set; } = cJ_OpenerWeight;
        public bool cj_OpenerInWarmup { get; set; } = cJ_OpenerInWarmup;
        public int cj_SecondsEnd { get; set; } = cJ_SecondsEnd;
        public int cj_LiftsOut { get; set; } = cJ_LiftsOut;
        public int cj_SnatchLifts_Out { get; set; } = cJ_SnatchLifts_Out;
        public bool Beep { get; set; } = beep;
        public List<Extra> snatchExtras { get; set; } = snatchExtras;
        public Dictionary<int /*from weight*/, int /*jump size*/> snatchJumps { get; set; } = snatchJumps;
        public Dictionary<int /*from weight*/, int /*time length*/> snatchTimes { get; set; } = snatchTimes;
        public List<Extra> cjExtras { get; set; } = cjExtras;
        public Dictionary<int /*from weight*/, int /*jump size*/> cjJumps { get; set; } = cjJumps;
        public Dictionary<int /*from weight*/, int /*time length*/> cjTimes { get; set; } = cjTimes;
        public Profile Clone(int idNew)
        {
            Profile clone = (Profile)this.MemberwiseClone();
            clone.id = idNew;
            clone.snatchExtras = [.. this.snatchExtras.Select(e => e.Clone())];
            clone.cjExtras = [.. this.cjExtras.Select(e => e.Clone())];
            clone.snatchJumps = new(this.snatchJumps);
            clone.snatchTimes = new(this.snatchTimes);
            clone.cjJumps = new(this.cjJumps);
            clone.cjTimes = new(this.cjTimes);
            return clone;
        }
    }
    public struct Extra
    {
        private static int _nextId = 1;
        public int id { get; private set; }
        public string Action { get; set; }
        public int Length { get; set; }
        public int Order { get; set; }
        public Extra(int id, string action, int length, int order)
        {
            this.id = id;
            if (id >= _nextId)
            {
                _nextId = id + 1;
            }
            Action = action;
            Length = length;
            Order = order;
        }
        public Extra(string action, int length, int order)
        {
            this.id = _nextId;
            _nextId++;
            Action = action;
            Length = length;
            Order = order;
        }
        public override readonly string ToString() => $"{id:D3}{Order:D3}{Length:D5}{Action}";
        public readonly Extra Clone()
        {
            return (Extra)this.MemberwiseClone();
        }
    }
    public class LiveStepControls
    {
        public Panel PanelLiveStep { get; set; }
        public Label LabelAction { get; set; }
        public ProgressBar ProgressBarStep { get; set; }
        public Label LabelWeight { get; set; }
        public Label LabelTime { get; set; }
        public Label LabelProgressTime { get; set; }
    }
    public class Step
    {
        public string Action { get; set; }
        public int? Weight { get; set; }
        public int Length { get; set; }
        public int TotalLength { get; set; }
        public int TotalLengthReverse { get; set; }
        public int Order { get; set; }
        public bool PreStep { get; set; }
        public bool isOpener { get; set; }
        public LiveStepControls Controls { get; set; }
        public string FormattedWeight => (this.Weight ?? 0) == 0 ? "" : $"{this.Weight}"; // for datagridview
        public string FormattedLength => Seconds_To_String(this.Length); // for datagridview
        public string FormattedTotalLength => Seconds_To_String(this.TotalLength); // for datagridview

        public Step(string action, int weight, int length, int totalLength, int order, bool preStep, bool isOpener)
        { // extras
            Action = action;
            Weight = weight;
            Length = length;
            TotalLength = totalLength;
            Order = order;
            PreStep = preStep;
            Controls = new();
            this.isOpener = isOpener;
        }
        public Step(string action, int length, int totalLength, /*int totalLengthReverse, */int order, bool isOpener)
        { // non-lift
            Action = action;
            Weight = null;
            Length = length;
            TotalLength = totalLength;
            //TotalLengthReverse = totalLengthReverse;
            Order = order;
            PreStep = true;
            Controls = new();
            this.isOpener = isOpener;
        }
        public Step(string action, int weight, bool isOpener) // add step phase
        { // is lift
            Action = action;
            Weight = weight;
            PreStep = false;
            Controls = new();
            this.isOpener = isOpener;
        }
        public Step(string action, int? weight, int length, int totalLength, int totalLengthReverse, int order, bool preStep, bool isOpener)
        {
            Action = action;
            Weight = weight;
            Length = length;
            TotalLength = totalLength;
            TotalLengthReverse = totalLengthReverse;
            Order = order;
            PreStep = preStep;
            Controls = new();
            this.isOpener = isOpener;
        }
        public Step Clone()
        {
            return (Step)this.MemberwiseClone();
        }
        private string Seconds_To_String(int seconds)
        {
            if (seconds == 0) return "-";
            TimeSpan ts = TimeSpan.FromSeconds(seconds);
            string formatted = "";
            if (ts.Hours > 0) formatted += $"{ts.Hours}h";
            if (ts.Minutes > 0) formatted += $"{ts.Minutes}m";
            if (ts.Seconds > 0 || formatted == "") formatted += $"{ts.Seconds}s";
            return formatted;
        }
    }
    public struct PlateParameter(int width, int height, SolidBrush brush)
    {
        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public SolidBrush Brush { get; set; } = brush;
    }
    public enum LiftType
    {
        Snatch,
        CleanAndJerk
    }

}
