using System.Drawing;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
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
        public override readonly string ToString() => $"{id:000000}{Order:000}{Length:00000}{Action}";
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
        public bool Override { get; set; }
        public LiveStepControls Controls { get; set; }

        public Step(string action, int weight, int length, int totalLength, int order, bool preStep, bool @override)
        { // extras
            Action = action;
            Weight = weight;
            Length = length;
            TotalLength = totalLength;
            Order = order;
            PreStep = preStep;
            Override = @override;
            Controls = new();
        }

        public Step(string action, int length, int totalLength, /*int totalLengthReverse, */int order, bool @override)
        { // non-lift
            Action = action;
            Weight = null;
            Length = length;
            TotalLength = totalLength;
            //TotalLengthReverse = totalLengthReverse;
            Order = order;
            PreStep = true;
            Override = @override;
            Controls = new();
        }
        public Step(string action, int weight, bool @override) // add step phase
        { // is lift
            Action = action;
            Weight = weight;
            PreStep = false;
            Override = @override;
            Controls = new();
        }
        public Step(string action, int? weight, int length, int totalLength, int totalLengthReverse, int order, bool preStep, bool @override)
        {
            Action = action;
            Weight = weight;
            Length = length;
            TotalLength = totalLength;
            TotalLengthReverse = totalLengthReverse;
            Order = order;
            PreStep = preStep;
            Override = @override;
            Controls = new();
        }
        public Step Clone()
        {
            return (Step)this.MemberwiseClone();
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
