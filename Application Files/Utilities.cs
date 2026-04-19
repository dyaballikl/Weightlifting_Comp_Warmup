using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
    public partial class form_Main
    {
        private string ActionTextString(Step _step, bool _isFuture)
        {
            return ((_step.Weight.HasValue && _step.Weight > 0) ? $"{(_step.isOpener ? "open at" : "lift")} {_step.Weight}" : $"{_step.Action}") +
                (_isFuture ? $" ({Seconds_To_String(_step.Length)})" +
                $"{Environment.NewLine}from {Seconds_To_String(_step.TotalLengthReverse)} out" : string.Empty);
        }
        private string Seconds_To_String(int _int_Seconds, bool _bool_ShortString = false)
        {
            TimeSpan _timeSpan = TimeSpan.FromSeconds(_int_Seconds);
            if (_timeSpan.Hours == 0)
            {
                if (_timeSpan.Minutes == 0)
                {
                    return $"{_int_Seconds}s";
                }
                else
                {
                    return $"{_timeSpan.Minutes}m{_timeSpan.Seconds}s";
                }
            }
            else if (_bool_ShortString)
            {
                return $"{_timeSpan.Hours}h{_timeSpan.Minutes}m";
            }
            else
            {
                return $"{_timeSpan.Hours}h{_timeSpan.Minutes}m{_timeSpan.Seconds}s";
            }
        }
        private void Smooth_Last_Jumps(HashSet<int> _weights, int _int_Opener)
        {
            IEnumerable<int> topThreeWeights = _weights
                .Where(r => r > 0)
                .OrderByDescending(r => r)
                .Select(r => r);
            int _int_FinalWarmup = topThreeWeights.ElementAtOrDefault(0);
            int _int_SecondToFinalWarmup = topThreeWeights.ElementAtOrDefault(1);
            int _int_ThirdToFinalWarmup = topThreeWeights.ElementAtOrDefault(2);

            if (_int_Opener > 0)
            {
                if (_int_Opener > _int_FinalWarmup) // not hitting opener in warmup
                {
                    _int_ThirdToFinalWarmup = _int_SecondToFinalWarmup;
                    _int_SecondToFinalWarmup = _int_FinalWarmup;
                    _int_FinalWarmup = _int_Opener;
                }
                if ((_int_FinalWarmup > 0 & _int_SecondToFinalWarmup > 0 & _int_ThirdToFinalWarmup > 0) && // 3+ warmups
                    (_int_FinalWarmup - _int_ThirdToFinalWarmup > 3) && // last 3 warmups span more than 3kg
                    (((int)(decimal)(_int_FinalWarmup - _int_ThirdToFinalWarmup) / 2m + _int_ThirdToFinalWarmup) != _int_SecondToFinalWarmup)) // 2nd to last warmup not right in the middle (already smoothed)
                {
                    _weights.Remove(_int_SecondToFinalWarmup);
                    _int_SecondToFinalWarmup = (int)Math.Floor((_int_FinalWarmup - _int_ThirdToFinalWarmup) / 2m + _int_ThirdToFinalWarmup);
                }
            }
        }
        private List<Step> x_Steps(
            List<Extra> _extras,
            Dictionary<int, int> _jumps,
            Dictionary<int, int> _times,
            int _int_x_Sec_End,
            int _int_x_Wgt_Opener,
            bool _bool_Opener_in_Warmup
            )
        {
            if (_extras == null | _jumps == null | _times == null) { return null; }
            HashSet<int> _weights = [];
            List<Step> _steps = [];
            _weights = hashSet_Weights(
                _jumps: _jumps,
                bool_Opener_in_Warmup: _bool_Opener_in_Warmup,
                _int_Weight_Opener: _int_x_Wgt_Opener);
            if (_weights.Count == 0)
            {
                _weights.Add(profileActive.BarbellWeight);
            }
            Smooth_Last_Jumps(_weights: _weights, _int_Opener: _int_x_Wgt_Opener);

            int _int_TotalSeconds = _extras.Sum(r => r.Length);
            int _int_Order = 1;
            foreach (Extra _extra in _extras.OrderBy(r => r.Order))
            {
                _steps.Add(new Step(
                    action: _extra.Action,
                    weight: 0,
                    length: _extra.Length,
                    totalLength: _int_TotalSeconds,
                    order: _int_Order,
                    preStep: false,
                    isOpener: false));
                _int_Order++;
            }

            int _int_Weight_Last = _weights.Max();
            foreach (int _int_Weight in _weights)
            {
                int intTime = _times
                    .Where(r => r.Key <= _int_Weight)
                    .OrderByDescending(r => r.Key)
                    .FirstOrDefault().Value;
                if (_int_Weight == _int_Weight_Last)
                {
                    intTime += _int_x_Sec_End;
                }
                _int_TotalSeconds += intTime;
                _steps.Add(new Step(
                    action: "Lift",
                    weight: _int_Weight,
                    length: intTime,
                    totalLength: _int_TotalSeconds,
                    order: _int_Order,
                    preStep: false,
                    isOpener: false));
                _int_Order++;
            }

            _int_TotalSeconds = 0;
            for (int i = _steps.Count - 1; i >= 0; i--)
            {
                _int_TotalSeconds += _steps[i].Length;
                _steps[i].TotalLengthReverse = _int_TotalSeconds;
            }

            _steps.Insert(0, new Step(
                action: "wait",
                weight: 0,
                length: 0,
                totalLength: 0,
                totalLengthReverse: _int_TotalSeconds,
                order: 0,
                preStep: true,
                isOpener: false));
            return _steps;
        }
        private int GetJumpForWeight(Dictionary<int, int> _jumps, int _int_Weight)
        {
            // Phase 1: Find the last jump entry whose key is at or below the current weight.
            // This corresponds to the 'if' block in your loop.
            KeyValuePair<int, int> baseJumpKvp = _jumps
                .Where(kvp => kvp.Key <= _int_Weight && kvp.Value != 0 && kvp.Key != 0)
                .OrderByDescending(kvp => kvp.Key)
                .FirstOrDefault();
            // If no such jump exists (e.g., weight is less than the first jump key), return 0.
            if (baseJumpKvp.Key == 0)
            {
                return 0;
            }

            // Phase 2: Find the very next jump entry whose key is above the current weight.
            KeyValuePair<int, int> nextJumpKvp = _jumps
                .Where(r => r.Key > baseJumpKvp.Key && r.Value != 0 && r.Key != 0 &&
                    (r.Key + r.Value <= _int_Weight + baseJumpKvp.Value) // we're not yet to this jump, but this jump would take us to/beyond where this jump would take us
                    )
                .OrderBy(kvp => kvp.Key)
                .FirstOrDefault();
            if (nextJumpKvp.Key != 0)
            {
                return nextJumpKvp.Key - _int_Weight + nextJumpKvp.Value;
            }
            return baseJumpKvp.Value;
        }
        private HashSet<int> hashSet_Weights(
            Dictionary<int, int> _jumps,
            int _int_Weight_Opener,
            bool bool_Opener_in_Warmup
            )
        {
            HashSet<int> _weights = [];
            int _int_Weight = 0;
            do
            {
                int _int_Jump;
                if (_int_Weight == 0)
                {
                    _int_Jump = profileActive.BarbellWeight;
                }
                else
                {
                    _int_Jump = GetJumpForWeight(_jumps, _int_Weight);
                }
                if (_int_Jump < 1 || (_int_Weight + _int_Jump >= _int_Weight_Opener))
                {
                    break;
                }
                else
                {
                    _int_Weight += _int_Jump;
                    _weights.Add(_int_Weight);
                }
            } while (_int_Weight < _int_Weight_Opener);
            if (bool_Opener_in_Warmup && !_weights.Contains(_int_Weight_Opener))
            {
                _weights.Add(_int_Weight_Opener);
            }

            return _weights;
        }
        private static DialogResult ShowInputDialog(ref string input)
        {
            Size size = new(200, 70);
            Form inputBox = new()
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = "Input Weight",
                BackColor = Color.Yellow
            };

            TextBox textBox = new()
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, 5),
                Text = input
            };
            inputBox.Controls.Add(textBox);

            Button okButton = new()
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "&OK",
                Location = new Point(size.Width - 80 - 80, 39),
                BackColor = SystemColors.Control
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new()
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "&Cancel",
                Location = new Point(size.Width - 80, 39),
                BackColor = SystemColors.Control
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            if (result == DialogResult.OK)
            {
                input = textBox.Text;
            }
            else
            {
                input = String.Empty;
            }
            return result;
        }
        private void Apply_Opener_Graphic_Vector(
            int _intWeightBar,
            int _intWeightOpener,
            bool _boolSnatch
            )
        {
            Panel _panelGraphic;
            if (_boolSnatch)
            {
                _panelGraphic = panel_snatch_Param_Opener;
            }
            else
            {
                _panelGraphic = panel_cj_Param_Opener;
            }

            _panelGraphic.Controls.Clear();
            _panelGraphic.Controls.Add(new WeightBox(
                weight: _intWeightOpener,
                barWeight: _intWeightBar,
                isOpener: true,
                isCompCollars: true
            )
            {
                Location = new Point(0, 0),
                Size = _panelGraphic.Size,
            });
        }
        private void UpdateBattery()
        {
            System.Management.ObjectQuery _query = new("Select * FROM Win32_Battery");
            ManagementObjectSearcher _searcher = new(_query);
            ManagementObjectCollection _collection = _searcher.Get();
            PropertyData _propertyData_BatteryPercent = null;
            PropertyData _propertyData_BatteryMinutesRemaining = null;
            bool _bool_A = false, _bool_B = false;
            bool _bool_BatteryVisible;
            foreach (ManagementObject mo in _collection.Cast<ManagementObject>())
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "EstimatedChargeRemaining")
                    {
                        _propertyData_BatteryPercent = property;
                        _bool_A = true;
                    }
                    else if (property.Name == "EstimatedRunTime")
                    {
                        _propertyData_BatteryMinutesRemaining = property;
                        _bool_B = true;
                    }
                    if (_bool_A && _bool_B)
                    {
                        break;
                    }
                }
            }
            if (_propertyData_BatteryPercent is not null || _propertyData_BatteryMinutesRemaining is not null)
            {
                UInt32? _uint_BatteryPercent = 100;
                if (_propertyData_BatteryPercent is not null)
                {
                    try
                    {
                        _uint_BatteryPercent = Convert.ToUInt32(_propertyData_BatteryPercent.Value);
                    }
                    catch { }
                }
                UInt32? _uint_BatteryMinutes = 9999;
                if (_propertyData_BatteryMinutesRemaining is not null)
                {
                    try
                    {
                        _uint_BatteryMinutes = Convert.ToUInt32(_propertyData_BatteryMinutesRemaining.Value);
                    }
                    catch { }
                }
                if (_uint_BatteryPercent < 97 || _uint_BatteryMinutes < 500)
                {
                    _bool_BatteryVisible = true;
                    panel_Battery.BringToFront();
                    progressBar_Battery.Value = (int)(_uint_BatteryPercent ?? 100);
                    if (_uint_BatteryMinutes < 999)
                    {
                        TimeSpan _timeSpan = TimeSpan.FromMinutes((int)_uint_BatteryMinutes);
                        label_Battery_Minutes.Text = $"{_timeSpan.Hours}h{_timeSpan.Minutes}m ({_uint_BatteryPercent}%)";
                        label_Battery_Minutes.Visible = true;
                        label_Battery_Percentage.Visible = false;
                    }
                    else
                    {
                        label_Battery_Percentage.Text = $"{_uint_BatteryPercent}%";
                        label_Battery_Minutes.Visible = false;
                        label_Battery_Percentage.Visible = true;
                    }
                }
                else
                {
                    _bool_BatteryVisible = false;
                }
            }
            else
            {
                _bool_BatteryVisible = false;
            }
            panel_Battery.Visible = _bool_BatteryVisible;
        }
        private DateTime _now
        {
            get
            {
                DateTime currentTime = DateTime.Now;
                return currentTime.AddMilliseconds(500).AddTicks(-(currentTime.AddMilliseconds(500).Ticks % TimeSpan.TicksPerSecond));
            }
        }
    }
    public class WeightBox : PictureBox
    {
        // Your properties remain the same for now
        private bool _isOpener;
        public bool isOpener
        {
            get => _isOpener;
            set { _isOpener = value; this.Invalidate(); }
        }
        private bool _isCompCollars;
        public bool isCompCollars
        {
            get => _isCompCollars;
            set { _isCompCollars = value; this.Invalidate(); }
        }
        private int _barWeight;
        public int BarWeight
        {
            get => _barWeight;
            set { _barWeight = value; this.Invalidate(); }
        }
        private int _weight;
        public int Weight
        {
            get => _weight;
            set { _weight = value; this.Invalidate(); }
        }
        private int _outlineWidth = 1;
        public int OutlineWidth
        {
            get => _outlineWidth;
            set { _outlineWidth = value; this.Invalidate(); }
        }
        private int _plateGap = -1;
        public int PlateGap
        {
            get => _plateGap;
            set { _plateGap = value; this.Invalidate(); }
        }
        public WeightBox(bool isOpener, bool isCompCollars, int barWeight, int weight)
        {
            this.isOpener = isOpener;
            this.isCompCollars = isCompCollars;
            BarWeight = barWeight;
            Weight = weight;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // It's good practice to call the base method first
            base.OnPaint(e);

            int _int_LBuffer;
            int _int_TBuffer;
            int _int_BBuffer;
            bool _bool_Collars;
            bool _bool_5KGCollars;
            if (isCompCollars)
            {
                _bool_5KGCollars = ((Weight - 15) >= BarWeight);
            }
            else
            {
                _bool_5KGCollars = false;
            }

            if (isOpener)
            {
                _bool_Collars = Weight - 5 > BarWeight;
                _int_LBuffer = 0;
                _int_TBuffer = 0;
                _int_BBuffer = 0;
            }
            else
            {
                _bool_Collars = Weight > BarWeight;
                _int_TBuffer = 1;
                _int_LBuffer = _int_TBuffer;
                _int_BBuffer = _int_TBuffer + 4;
            }

            Dictionary<decimal, int> _plates = Plates_Count_For_Weight(
                _int_WeightBar: BarWeight,
                _bool_5KGCollar: _bool_5KGCollars,
                _int_WeightLift: Weight);

            int _int_Full_Height = this.Height - _int_TBuffer - _int_BBuffer;
            Dictionary<decimal, PlateParameter> _plateParameters = new()
            {
                [25m] = new PlateParameter(width: 16, height: _int_Full_Height, brush: new SolidBrush(color: AppColors.Plate_Red)),
                [20m] = new PlateParameter(width: 15, height: _int_Full_Height, brush: new SolidBrush(color: AppColors.Plate_Blue)),
                [15m] = new PlateParameter(width: 13, height: _int_Full_Height, brush: new SolidBrush(color: AppColors.Plate_Yellow)),
                [10m] = new PlateParameter(width: 11, height: _int_Full_Height, brush: new SolidBrush(color: AppColors.Plate_Green)),
            };
            // 5.0s
            int _int_Plate_Height = Convert.ToInt32(_int_Full_Height * .75);
            if (_plates.Any() && _plates.Where(r => r.Value > 0).Max(r => r.Key) == 5.0m)
            {
                _plateParameters[5.0m] = new PlateParameter(width: _plateParameters[25m].Width * 2, height: _int_Full_Height, brush: new SolidBrush(color: AppColors.Plate_White));
            }
            else
            {
                _plateParameters[5.0m] = new PlateParameter(width: 9, height: _int_Plate_Height, brush: new SolidBrush(color: AppColors.Plate_White));
            }
            // 2.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .85);
            if (_plates.Any() && _plates.Where(r => r.Value > 0).Max(r => r.Key) == 2.5m)
            {
                _plateParameters[2.5m] = new PlateParameter(width: _plateParameters[25m].Width * 2, height: _int_Full_Height, brush: new SolidBrush(color: AppColors.Plate_White));
            }
            else
            {
                _plateParameters[2.5m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: AppColors.Plate_Red));
            }
            // 2.0s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[2.0m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: AppColors.Plate_Blue));
            // 1.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[1.5m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: AppColors.Plate_Yellow));
            // 1.0s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[1.0m] = new PlateParameter(width: 7, height: _int_Plate_Height, brush: new SolidBrush(color: AppColors.Plate_Green));
            // 0.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[0.5m] = new PlateParameter(width: 6, height: _int_Plate_Height, brush: new SolidBrush(color: AppColors.Plate_White));

            int _int_Collar_Height = _bool_5KGCollars ? 18 : 18;
            int _int_Collar_Width = _bool_5KGCollars ? 8 : 6;
            int _int_MainBar_Width = (
                    isOpener ?
                    Math.Min(400, this.Width - _int_LBuffer - (2 * OutlineWidth)) :
                    10);
            int _int_Sleeve_Width =
                (
                    isOpener ?
                    (
                        Weight > 229 ?
                        125 :
                        (
                            Weight > 149 ?
                            100 :
                            75
                        )
                    ) :
                    75
                );
            int _int_SleeveKnuckle_Width = 9;
            bool _bool_Outline = (OutlineWidth > 0);

            SolidBrush _brush_BarOutline = new(color: Color.Black);
            SolidBrush _brush_PlateOutline = new(color: Color.Black);
            SolidBrush _brush_CollarSilver = new(color: Color.Gainsboro);
            SolidBrush _brush_BarGrey = new(color: AppColors.Bar_Grey);

            //add bar
            //add bar Outline
            Rectangle _rect_MainBar;
            Rectangle _rectOutline_MainBar;
            Rectangle _rect_SleeveKnuckle_Right;
            Rectangle _rectOutline_SleeveKnuckle_Right;
            Rectangle _rect_Sleeve_Right, _rectOutline_Sleeve_Right;
            Rectangle _rect_SleeveKnuckle_Left;
            Rectangle _rectOutline_SleeveKnuckle_Left;
            Rectangle _rect_Sleeve_Left;
            Rectangle _rectOutline_Sleeve_Left;

            _rect_MainBar = new(
                x: _int_LBuffer + OutlineWidth,
                y: 0,
                width: _int_MainBar_Width,
                height: 6);
            _rect_MainBar.Y = (_int_Full_Height / 2) - (_rect_MainBar.Height / 2) + _int_TBuffer;
            if (isOpener)
            {
                _rect_Sleeve_Right = new(
                    x: _rect_MainBar.X + _rect_MainBar.Width - _int_Sleeve_Width,
                    y: 0,
                    width: _int_Sleeve_Width,
                    height: 10);
                _rect_SleeveKnuckle_Right = new(
                    x: _rect_Sleeve_Right.X - _int_SleeveKnuckle_Width,
                    y: 0,
                    width: _int_SleeveKnuckle_Width,
                    height: 16);
                _rect_Sleeve_Left = new(
                    x: _int_LBuffer + OutlineWidth,
                    y: 0,
                    width: _int_Sleeve_Width,
                    height: 10);
                _rect_SleeveKnuckle_Left = new(
                    x: _rect_Sleeve_Left.X + _rect_Sleeve_Left.Width - 1,
                    y: 0,
                    width: _int_SleeveKnuckle_Width,
                    height: 16);
            }
            else
            {
                _rect_SleeveKnuckle_Right = new(
                    x: _rect_MainBar.X + _rect_MainBar.Width - 1,
                    y: 0,
                    width: _int_SleeveKnuckle_Width,
                    height: 16);
                _rect_Sleeve_Right = new(
                    x: _rect_SleeveKnuckle_Right.X + _rect_SleeveKnuckle_Right.Width - 1,
                    y: 0,
                    width: _int_Sleeve_Width,
                    height: 10);
                _rect_Sleeve_Left = new();
                _rect_SleeveKnuckle_Left = new();
            }
            _rect_SleeveKnuckle_Right.Y = (_int_Full_Height / 2) - (_rect_SleeveKnuckle_Right.Height / 2) + _int_TBuffer;
            _rect_Sleeve_Right.Y = (_int_Full_Height / 2) - (_rect_Sleeve_Right.Height / 2) + _int_TBuffer;
            _rect_SleeveKnuckle_Left.Y = _rect_SleeveKnuckle_Right.Y;
            _rect_Sleeve_Left.Y = _rect_Sleeve_Right.Y;
            if (_bool_Outline)
            {
                _rectOutline_MainBar = _rect_MainBar;
                _rectOutline_MainBar.Inflate(width: OutlineWidth, height: OutlineWidth);
                _rectOutline_SleeveKnuckle_Right = _rect_SleeveKnuckle_Right;
                _rectOutline_SleeveKnuckle_Right.Inflate(width: OutlineWidth, height: OutlineWidth);
                _rectOutline_Sleeve_Right = _rect_Sleeve_Right;
                _rectOutline_Sleeve_Right.Inflate(width: OutlineWidth, height: OutlineWidth);
                e.Graphics.FillRectangle(
                    brush: _brush_BarOutline,
                    rect: _rectOutline_MainBar);
                e.Graphics.FillRectangle(
                    brush: _brush_BarOutline,
                    rect: _rectOutline_SleeveKnuckle_Right);
                e.Graphics.FillRectangle(
                    brush: _brush_BarOutline,
                    rect: _rectOutline_Sleeve_Right);
                if (isOpener)
                {
                    _rectOutline_SleeveKnuckle_Left = _rect_SleeveKnuckle_Left;
                    _rectOutline_SleeveKnuckle_Left.Inflate(width: OutlineWidth, height: OutlineWidth);
                    _rectOutline_Sleeve_Left = _rect_Sleeve_Left;
                    _rectOutline_Sleeve_Left.Inflate(width: OutlineWidth, height: OutlineWidth);
                    e.Graphics.FillRectangle(
                        brush: _brush_BarOutline,
                        rect: _rectOutline_MainBar);
                    e.Graphics.FillRectangle(
                        brush: _brush_BarOutline,
                        rect: _rectOutline_SleeveKnuckle_Left);
                    e.Graphics.FillRectangle(
                        brush: _brush_BarOutline,
                        rect: _rectOutline_Sleeve_Left);
                }
            }
            e.Graphics.FillRectangle(
                brush: _brush_BarGrey,
                rect: _rect_MainBar);
            e.Graphics.FillRectangle(
                brush: _brush_BarGrey,
                rect: _rect_SleeveKnuckle_Right);
            e.Graphics.FillRectangle(
                brush: _brush_BarGrey,
                rect: _rect_Sleeve_Right);
            if (isOpener)
            {
                e.Graphics.FillRectangle(
                    brush: _brush_BarGrey,
                    rect: _rect_MainBar);
                e.Graphics.FillRectangle(
                    brush: _brush_BarGrey,
                    rect: _rect_SleeveKnuckle_Left);
                e.Graphics.FillRectangle(
                    brush: _brush_BarGrey,
                    rect: _rect_Sleeve_Left);
            }

            // add plates
            int _int_Left = _rect_SleeveKnuckle_Right.X + _int_SleeveKnuckle_Width + OutlineWidth;
            int _int_Right = _rect_SleeveKnuckle_Left.X - OutlineWidth;
            bool _bool_CollarsDone = !_bool_Collars; // if not doing collar, mark them already done
            void doCollars()
            {
                Rectangle _rect = new(
                    x: _int_Left,
                    y: (_int_Full_Height / 2) - (_int_Collar_Height / 2) + _int_TBuffer,
                    width: _int_Collar_Width,
                    height: _int_Collar_Height);
                Rectangle _rect_Outline = _rect;
                if (_bool_Outline)
                {
                    _rect_Outline.Width += OutlineWidth * 2;
                    _rect.X += OutlineWidth;
                    _rect.Inflate(width: 0, height: -OutlineWidth);
                    e.Graphics.FillRectangle(
                        brush: _brush_BarOutline,
                        rect: _rect_Outline);
                }
                Brush b = _bool_5KGCollars ? _brush_CollarSilver : Brushes.Black;
                e.Graphics.FillRectangle(
                    brush: b,
                    rect: _rect);
                if (_bool_5KGCollars) // add adjusting arm
                {
                    Rectangle _rect_Arm = new(
                        x: _int_Left + _int_Collar_Width / 2 - 1,
                        y: _rect.Top - 5,
                        width: 2,
                        height: 8);
                    Rectangle _rect_Arm_Outline = _rect_Arm;
                    if (_bool_Outline)
                    {
                        _rect_Arm_Outline.Width += OutlineWidth * 2;
                        _rect_Arm.X += OutlineWidth;
                        _rect_Arm.Inflate(width: 0, height: -OutlineWidth);
                        e.Graphics.FillRectangle(
                            brush: _brush_BarOutline,
                            rect: _rect_Arm_Outline);
                    }
                    b = _bool_5KGCollars ? _brush_CollarSilver : Brushes.Black;
                    e.Graphics.FillRectangle(
                        brush: b,
                        rect: _rect_Arm);
                }
                _int_Left += _rect_Outline.Width + PlateGap;
                if (isOpener) // must do both sides of the bar
                {
                    _int_Right -= _rect_Outline.Width;
                    _rect.X = _int_Right;
                    if (_bool_Outline)
                    {
                        _rect_Outline.X = _rect.X;
                        _rect.X += OutlineWidth;
                        e.Graphics.FillRectangle(
                            brush: _brush_PlateOutline,
                            rect: _rect_Outline);
                    }
                    e.Graphics.FillRectangle(
                        brush: b,
                        rect: _rect);
                    if (_bool_5KGCollars) // add adjusting arm
                    {
                        Rectangle _rect_Arm = new(
                            x: _int_Right + _int_Collar_Width / 2 - 1,
                            y: _rect.Top - 5,
                            width: 2,
                            height: 8);
                        Rectangle _rect_Arm_Outline = _rect_Arm;
                        if (_bool_Outline)
                        {
                            _rect_Arm_Outline.Width += OutlineWidth * 2;
                            _rect_Arm.X += OutlineWidth;
                            _rect_Arm.Inflate(width: 0, height: -OutlineWidth);
                            e.Graphics.FillRectangle(
                                brush: _brush_BarOutline,
                                rect: _rect_Arm_Outline);
                        }
                        b = _bool_5KGCollars ? _brush_CollarSilver : Brushes.Black;
                        e.Graphics.FillRectangle(
                            brush: b,
                            rect: _rect_Arm);
                    }
                    _int_Right -= PlateGap;
                }
            }
            foreach (KeyValuePair<decimal, int> _plate in _plates.OrderByDescending(r => r.Key).Where(r => r.Value > 0))
            {
                if (!_bool_CollarsDone && _plate.Key < 2.5m)
                {
                    doCollars();
                    _bool_CollarsDone = true;
                }
                if (_plateParameters.TryGetValue(_plate.Key, out PlateParameter _parameter))
                {
                    for (int _int_Plate = 1; _int_Plate <= _plate.Value; _int_Plate++)
                    {
                        Rectangle _rect = new(
                            x: _int_Left,
                            y: (_int_Full_Height / 2) - (_parameter.Height / 2) + _int_TBuffer,
                            width: _parameter.Width,
                            height: _parameter.Height);
                        Rectangle _rect_Outline = _rect;
                        if (_bool_Outline)
                        {
                            _rect_Outline.Width += OutlineWidth * 2;
                            _rect.X += OutlineWidth;
                            _rect.Inflate(width: 0, height: -OutlineWidth);
                            e.Graphics.FillRectangle(
                                brush: _brush_PlateOutline,
                                rect: _rect_Outline);
                        }
                        e.Graphics.FillRectangle(
                            brush: _parameter.Brush,
                            rect: _rect);
                        _int_Left += _rect_Outline.Width + PlateGap;
                        if (isOpener)
                        {
                            _int_Right -= _rect_Outline.Width;
                            _rect.X = _int_Right;
                            if (_bool_Outline)
                            {
                                _rect_Outline.X = _rect.X;
                                _rect.X += OutlineWidth;
                                e.Graphics.FillRectangle(
                                    brush: _brush_PlateOutline,
                                    rect: _rect_Outline);
                            }
                            e.Graphics.FillRectangle(
                                brush: _parameter.Brush,
                                rect: _rect);
                            _int_Right -= PlateGap;
                        }
                    }
                }
            }
            if (!_bool_CollarsDone)
            {
                doCollars();
            }
        }
        private Dictionary<decimal, int> Plates_Count_For_Weight(
            int _int_WeightBar,
            bool _bool_5KGCollar,
            int _int_WeightLift)
        {
            Dictionary<decimal, int> _plates = [];
            if (_bool_5KGCollar) { _int_WeightBar += 5; }
            if (_int_WeightLift > _int_WeightBar)
            {
                decimal _decToGo = Convert.ToDecimal(_int_WeightLift - _int_WeightBar) / 2m;
                decimal[] _decPlateWeights = [25m, 20m, 15m, 10m, 5.0m, 2.5m, 2.0m, 1.5m, 1.0m, 0.5m];
                foreach (decimal _decPlateWeight in _decPlateWeights)
                {
                    int _int = Convert.ToInt32((_decToGo - _decToGo % _decPlateWeight) / _decPlateWeight);
                    if (_int > 0)
                    {
                        _plates[_decPlateWeight] = _int;
                        _decToGo -= Convert.ToDecimal(_int) * _decPlateWeight;
                    }
                }
            }
            return _plates;
        }
    }
}
