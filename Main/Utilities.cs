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
            return ((_step.Weight.HasValue && _step.Weight > 0) ? $"lift {_step.Weight}" : $"{_step.Action}") + (_isFuture ?
                $" ({Seconds_To_String(_step.Length)})" +
                $"{Environment.NewLine}from {Seconds_To_String(_step.TotalLengthReverse)} out" : string.Empty);
        }
        private string Seconds_To_String(int _int_Seconds, bool _bool_ShortString = false)
        {
            int intHrs = (_int_Seconds - (_int_Seconds % 3600)) / 3600;
            _int_Seconds -= (intHrs * 3600);
            int intMns = (_int_Seconds - (_int_Seconds % 60)) / 60;
            _int_Seconds -= (intMns * 60);
            if (intHrs == 0)
            {
                if (intMns == 0)
                {
                    return _int_Seconds.ToString() + "s";
                }
                else
                {
                    return intMns.ToString() + "m" + _int_Seconds.ToString() + "s";
                }
            }
            else if (_bool_ShortString)
            {
                return intHrs.ToString() + "h" + intMns.ToString() + "m";
            }
            else
            {
                return intHrs.ToString() + "h" + intMns.ToString() + "m" + _int_Seconds.ToString() + "s";
            }
        }
        private void Smooth_Last_Jumps(List<Step> _steps, int intOpener)
        {
            List<int> topThreeWeights = [.. _steps
                .Where(step => (step.Weight ?? 0) > 0)
                .Select(step => step.Weight.Value)
                .Distinct()
                .OrderByDescending(weight => weight)];
            int _int_LastWarmup = topThreeWeights.ElementAtOrDefault(0);
            int _int_SecondToLastWarmup = topThreeWeights.ElementAtOrDefault(1);
            int _int_ThirdToLastWarmup = topThreeWeights.ElementAtOrDefault(2);

            if (intOpener > 0)
            {
                if (intOpener == _int_LastWarmup)
                {
                    if ((_int_LastWarmup > 0 & _int_SecondToLastWarmup > 0 & _int_ThirdToLastWarmup > 0) &&
                        (_int_LastWarmup - _int_ThirdToLastWarmup > 3) &&
                        (((int)(decimal)(_int_LastWarmup - _int_ThirdToLastWarmup) / 2 + (decimal)_int_ThirdToLastWarmup) != _int_SecondToLastWarmup))
                    {
                        Step _step = _steps.FirstOrDefault(r => r.Weight == _int_SecondToLastWarmup && !r.Override);
                        if (_step != null)
                        {
                            _step.Weight = (int)Math.Floor((decimal)(_int_LastWarmup - _int_ThirdToLastWarmup) / 2 + _int_ThirdToLastWarmup);
                        }
                    }
                }
                else if (_int_LastWarmup > 0 & _int_SecondToLastWarmup > 0)
                {
                    if ((intOpener - _int_SecondToLastWarmup > 3) &&
                        (((int)(decimal)(intOpener - _int_SecondToLastWarmup) / 2 + (decimal)_int_SecondToLastWarmup) != _int_LastWarmup))
                    {
                        Step _step = _steps.FirstOrDefault(r => r.Weight == _int_LastWarmup && !r.Override);
                        if (_step != null)
                        {
                            _step.Weight = (int)Math.Floor((decimal)(intOpener - _int_SecondToLastWarmup) / 2 + _int_SecondToLastWarmup);
                        }
                    }
                }
            }
        }
        private List<Step> x_Steps(
            bool _bool_PreserveLifts,
            List<Extra> _extras,
            Dictionary<int, int> _jumps,
            Dictionary<int, int> _times,
            int _int_x_Sec_End,
            int _int_x_Wgt_Opener,
            bool _bool_Opener_in_Warmup,
            List<Step> _stepsIn = null
            )
        {
            if (_extras == null | _jumps == null | _times == null) { return null; }
            Dictionary<int, bool> _weightInputs = [];
            Dictionary<int, bool> _weights = [];
            List<Step> _steps = [];
            if (_bool_PreserveLifts & _stepsIn != null)
            {
                foreach (Step _step in _stepsIn)
                {
                    if ((_step.Weight ?? 0) > 0)
                    {
                        _weightInputs[(int)_step.Weight] = _step.Override;
                    }
                }
                //remove final auto populated weights
                int _max = _weightInputs.Max().Key;
                if (_max > 0)
                {
                    foreach (int key in _weightInputs.Keys.Where(r => r > _max))
                    {
                        _weightInputs.Remove(key);
                    }
                }
            }
            _weights = dictionary_Weights(
                _jumps: _jumps,
                _weightInputs: _weightInputs,
                bool_Opener_in_Warmup: _bool_Opener_in_Warmup,
                _int_Weight_Opener: _int_x_Wgt_Opener);

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
                    @override: false
                    ));
                _int_Order++;
            }

            _times = _times.OrderBy(r => r.Key).ToDictionary(r => r.Key, r => r.Value);
            int _int_Weight_Last = _weights.Last().Key;
            foreach (KeyValuePair<int, bool> _kvp_Weight in _weights)
            {
                int intTime = _times.Where(r => r.Key <= _kvp_Weight.Key).Max(r => r.Value);
                if (_kvp_Weight.Key == _int_Weight_Last)
                {
                    intTime += _int_x_Sec_End;
                }
                _int_TotalSeconds += intTime;
                _steps.Add(new Step(
                    action: "Lift",
                    weight: _kvp_Weight.Key,
                    length: intTime,
                    totalLength: _int_TotalSeconds,
                    order: _int_Order,
                    preStep: false,
                    @override: _kvp_Weight.Value));
                _int_Order++;
            }

            _int_TotalSeconds = 0;
            for (int i = _steps.Count - 1; i >= 0; i--)
            {
                _int_TotalSeconds += _steps[i].Length;
                _steps[i].TotalLengthReverse = _int_TotalSeconds;
            }

            Smooth_Last_Jumps(_steps: _steps, intOpener: _int_x_Wgt_Opener);

            _steps.Insert(0, new Step(
                action: "wait",
                weight: 0,
                length: 0,
                totalLength: 0,
                totalLengthReverse: _int_TotalSeconds,
                order: 0,
                preStep: true,
                @override: false));
            return _steps;
        }
        private Dictionary<int, bool> dictionary_Weights(
            Dictionary<int, int> _jumps,
            Dictionary<int, bool> _weightInputs,
            int _int_Weight_Opener,
            bool bool_Opener_in_Warmup
            )
        {
            List<KeyValuePair<int, bool>> _weightInputs_List = [.. _weightInputs.OrderBy(r => r.Key)];
            Dictionary<int, bool> _weights = [];
            int _int_Weight = 0;
            int _int_Lift = 0;
            bool _bool_Override;
            do
            {
                int _int_Jump = 0;
                if (_int_Lift < _weightInputs_List.Count)
                {
                    _int_Jump = _weightInputs_List[_int_Lift].Key - _int_Weight;
                    _bool_Override = _weightInputs_List[_int_Lift].Value;
                }
                else if (_int_Weight == 0)
                {
                    _int_Jump = int_Barbell;
                    _bool_Override = false;
                }
                else
                {
                    _bool_Override = false;
                    foreach (KeyValuePair<int, int> _jump in _jumps.OrderBy(r => r.Key))
                    {
                        if (_jump.Key <= _int_Weight)
                        {
                            _int_Jump = _jump.Value;
                        }
                        else if (_int_Jump > 0 && _jump.Key + _jump.Value <= _int_Weight + _int_Jump)
                        {
                            _int_Jump = _jump.Key - _int_Weight + _jump.Value;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (_int_Jump < 1 || (_int_Weight + _int_Jump >= _int_Weight_Opener))
                {
                    break;
                }
                else
                {
                    _int_Weight += _int_Jump;
                    _weights[_int_Weight] = _bool_Override;
                    _int_Lift++;
                }
            } while (_int_Weight < _int_Weight_Opener);
            if (bool_Opener_in_Warmup && _weights.Count > 0 &&
                _weights.Max(r => r.Key) < _int_Weight_Opener)
            {
                _weights[_int_Weight_Opener] = _bool_Override;
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

            WeightBox weightBoxGraphic = new()
            {
                intWeight = _intWeightOpener,
                intWeightBar = _intWeightBar,
                boolOpener = true,
                intOutlineWidth = 1,
                intPlateGap = -1,
                Location = new Point(0, 0),
                Size = _panelGraphic.Size,
            };

            weightBoxGraphic.Paint += Apply_Vector_Weight_Graphic;
            _panelGraphic.Controls.Add(weightBoxGraphic);
        }
        private void Apply_Vector_Weight_Graphic(object sender, PaintEventArgs e)
        {
            WeightBox _weightBox_Graphic = (WeightBox)sender;
            int _int_WeightBar = _weightBox_Graphic.intWeightBar;
            int _int_Weight = _weightBox_Graphic.intWeight;
            int _int_Outline_Width = _weightBox_Graphic.intOutlineWidth;
            int _int_PlateGap = _weightBox_Graphic.intPlateGap;
            bool _bool_Opener = _weightBox_Graphic.boolOpener;

            int _int_LBuffer;
            int _int_TBuffer;
            int _int_BBuffer;
            bool _bool_Collars;
            bool _bool_5KGCollars;
            if (_bool_Opener)
            {
                _bool_Collars = _int_Weight - 5 > _int_WeightBar;
                _bool_5KGCollars = ((_int_Weight - 15) >= _int_WeightBar);
                _int_LBuffer = 0;
                _int_TBuffer = 0;
                _int_BBuffer = 0;
            }
            else
            {
                _bool_Collars = _int_Weight > _int_WeightBar;
                _bool_5KGCollars = false;
                _int_TBuffer = 1;
                _int_LBuffer = _int_TBuffer;
                _int_BBuffer = _int_TBuffer + 4;
            }

            Dictionary<decimal, int> _plates = Plates_Count_For_Weight(
                _int_WeightBar: _int_WeightBar,
                _bool_5KGCollar: _bool_5KGCollars,
                _int_WeightLift: _int_Weight);

            int _int_Full_Height = _weightBox_Graphic.Height - _int_TBuffer - _int_BBuffer;
            Dictionary<decimal, PlateParameter> _plateParameters = new()
            {
                [25m] = new PlateParameter(width: 16, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Red)),
                [20m] = new PlateParameter(width: 15, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Blue)),
                [15m] = new PlateParameter(width: 13, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Yellow)),
                [10m] = new PlateParameter(width: 11, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Green)),
            };
            // 5.0s
            int _int_Plate_Height = Convert.ToInt32(_int_Full_Height * .75);
            if (_plates.Any() && _plates.Where(r => r.Value > 0).Max(r => r.Key) == 5.0m)
            {
                _plateParameters[5.0m] = new PlateParameter(width: _plateParameters[25m].Width * 2, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_White));
            }
            else
            {
                _plateParameters[5.0m] = new PlateParameter(width: 9, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_White));
            }
            // 2.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .85);
            if (_plates.Any() && _plates.Where(r => r.Value > 0).Max(r => r.Key) == 2.5m)
            {
                _plateParameters[2.5m] = new PlateParameter(width: _plateParameters[25m].Width * 2, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_White));
            }
            else
            {
                _plateParameters[2.5m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Red));
            }
            // 2.0s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[2.0m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Blue));
            // 1.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[1.5m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Yellow));
            // 1.0s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[1.0m] = new PlateParameter(width: 7, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Green));
            // 0.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[0.5m] = new PlateParameter(width: 6, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_White));

            int _int_Collar_Height = _bool_5KGCollars ? 18 : 18;
            int _int_Collar_Width = _bool_5KGCollars ? 8 : 6;
            int _int_MainBar_Width = (
                    _bool_Opener ?
                    Math.Min(400, _weightBox_Graphic.Width - _int_LBuffer - (2 * _int_Outline_Width)) :
                    10);
            int _int_Sleeve_Width =
                (
                    _bool_Opener ?
                    (
                        _int_Weight > 229 ?
                        125 :
                        (
                            _int_Weight > 149 ?
                            100 :
                            75
                        )
                    ) :
                    75
                );
            int _int_SleeveKnuckle_Width = 9;
            bool _bool_Outline = (_int_Outline_Width > 0);

            SolidBrush _brush_BarOutline = new(color: Color.Black);
            SolidBrush _brush_PlateOutline = new(color: Color.Black);
            SolidBrush _brush_CollarSilver = new(color: Color.Gainsboro);
            SolidBrush _brush_BarGrey = new(color: color_BarGrey);

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
                x: _int_LBuffer + _int_Outline_Width,
                y: 0,
                width: _int_MainBar_Width,
                height: 6);
            _rect_MainBar.Y = (_int_Full_Height / 2) - (_rect_MainBar.Height / 2) + _int_TBuffer;
            if (_bool_Opener)
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
                    x: _int_LBuffer + _int_Outline_Width,
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
                _rectOutline_MainBar.Inflate(width: _int_Outline_Width, height: _int_Outline_Width);
                _rectOutline_SleeveKnuckle_Right = _rect_SleeveKnuckle_Right;
                _rectOutline_SleeveKnuckle_Right.Inflate(width: _int_Outline_Width, height: _int_Outline_Width);
                _rectOutline_Sleeve_Right = _rect_Sleeve_Right;
                _rectOutline_Sleeve_Right.Inflate(width: _int_Outline_Width, height: _int_Outline_Width);
                e.Graphics.FillRectangle(
                    brush: _brush_BarOutline,
                    rect: _rectOutline_MainBar);
                e.Graphics.FillRectangle(
                    brush: _brush_BarOutline,
                    rect: _rectOutline_SleeveKnuckle_Right);
                e.Graphics.FillRectangle(
                    brush: _brush_BarOutline,
                    rect: _rectOutline_Sleeve_Right);
                if (_bool_Opener)
                {
                    _rectOutline_SleeveKnuckle_Left = _rect_SleeveKnuckle_Left;
                    _rectOutline_SleeveKnuckle_Left.Inflate(width: _int_Outline_Width, height: _int_Outline_Width);
                    _rectOutline_Sleeve_Left = _rect_Sleeve_Left;
                    _rectOutline_Sleeve_Left.Inflate(width: _int_Outline_Width, height: _int_Outline_Width);
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
            if (_bool_Opener)
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
            int _int_Left = _rect_SleeveKnuckle_Right.X + _int_SleeveKnuckle_Width + _int_Outline_Width;
            int _int_Right = _rect_SleeveKnuckle_Left.X - _int_Outline_Width;
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
                    _rect_Outline.Width += _int_Outline_Width * 2;
                    _rect.X += _int_Outline_Width;
                    _rect.Inflate(width: 0, height: -_int_Outline_Width);
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
                        _rect_Arm_Outline.Width += _int_Outline_Width * 2;
                        _rect_Arm.X += _int_Outline_Width;
                        _rect_Arm.Inflate(width: 0, height: -_int_Outline_Width);
                        e.Graphics.FillRectangle(
                            brush: _brush_BarOutline,
                            rect: _rect_Arm_Outline);
                    }
                    b = _bool_5KGCollars ? _brush_CollarSilver : Brushes.Black;
                    e.Graphics.FillRectangle(
                        brush: b,
                        rect: _rect_Arm);
                }
                _int_Left += _rect_Outline.Width + _int_PlateGap;
                if (_bool_Opener) // must do both sides of the bar
                {
                    _int_Right -= _rect_Outline.Width;
                    _rect.X = _int_Right;
                    if (_bool_Outline)
                    {
                        _rect_Outline.X = _rect.X;
                        _rect.X += _int_Outline_Width;
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
                            _rect_Arm_Outline.Width += _int_Outline_Width * 2;
                            _rect_Arm.X += _int_Outline_Width;
                            _rect_Arm.Inflate(width: 0, height: -_int_Outline_Width);
                            e.Graphics.FillRectangle(
                                brush: _brush_BarOutline,
                                rect: _rect_Arm_Outline);
                        }
                        b = _bool_5KGCollars ? _brush_CollarSilver : Brushes.Black;
                        e.Graphics.FillRectangle(
                            brush: b,
                            rect: _rect_Arm);
                    }
                    _int_Right -= _int_PlateGap;
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
                            _rect_Outline.Width += _int_Outline_Width * 2;
                            _rect.X += _int_Outline_Width;
                            _rect.Inflate(width: 0, height: -_int_Outline_Width);
                            e.Graphics.FillRectangle(
                                brush: _brush_PlateOutline,
                                rect: _rect_Outline);
                        }
                        e.Graphics.FillRectangle(
                            brush: _parameter.Brush,
                            rect: _rect);
                        _int_Left += _rect_Outline.Width + _int_PlateGap;
                        if (_bool_Opener)
                        {
                            _int_Right -= _rect_Outline.Width;
                            _rect.X = _int_Right;
                            if (_bool_Outline)
                            {
                                _rect_Outline.X = _rect.X;
                                _rect.X += _int_Outline_Width;
                                e.Graphics.FillRectangle(
                                    brush: _brush_PlateOutline,
                                    rect: _rect_Outline);
                            }
                            e.Graphics.FillRectangle(
                                brush: _parameter.Brush,
                                rect: _rect);
                            _int_Right -= _int_PlateGap;
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
        private UInt32? uint_LastBatteryPercent = null;
        private UInt32? uint_LastBatteryMinutes = null;
        private void UpdateBattery()
        {
            if (propertyData_BatteryPercent is null || propertyData_BatteryMinutesRemaining is null)
            {
                System.Management.ObjectQuery query = new("Select * FROM Win32_Battery");
                ManagementObjectSearcher searcher = new(query);
                ManagementObjectCollection collection = searcher.Get();
                bool _bool_A = false, _bool_B = false;
                foreach (ManagementObject mo in collection.Cast<ManagementObject>())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        if (property.Name == "EstimatedChargeRemaining")
                        {
                            propertyData_BatteryPercent = property;
                            _bool_A = true;
                        }
                        else if (property.Name == "EstimatedRunTime")
                        {
                            propertyData_BatteryMinutesRemaining = property;
                            _bool_B = true;
                        }
                        if (_bool_A && _bool_B)
                        {
                            break;
                        }
                    }
                }
            }
            bool _bool_Visible = false;
            if (propertyData_BatteryPercent is not null || propertyData_BatteryMinutesRemaining is not null)
            {
                UInt32? _uint_BatteryPercent = 100;
                if (propertyData_BatteryPercent is not null)
                {
                    try
                    {
                        _uint_BatteryPercent = Convert.ToUInt32(propertyData_BatteryPercent.Value);
                    }
                    catch { }
                }
                UInt32? _uint_BatteryMinutes = 9999;
                if (propertyData_BatteryMinutesRemaining is not null)
                {
                    try
                    {
                        _uint_BatteryMinutes = Convert.ToUInt32(propertyData_BatteryMinutesRemaining.Value);
                    }
                    catch { }
                }
                if (uint_LastBatteryPercent != _uint_BatteryPercent || uint_LastBatteryMinutes != _uint_BatteryMinutes)
                {
                    uint_LastBatteryPercent = _uint_BatteryPercent;
                    uint_LastBatteryMinutes = _uint_BatteryMinutes;
                    if (uint_LastBatteryPercent < 97 || uint_LastBatteryMinutes < 500)
                    {
                        _bool_Visible = true;
                        panel_Battery.BringToFront();
                        panel_Battery.Visible = true;
                        progressBar_Battery.Value = (int)(_uint_BatteryPercent ?? 100);
                        string _str_LabelText;
                        if (uint_LastBatteryMinutes < 500)
                        {
                            _str_LabelText = $"{uint_LastBatteryMinutes} min";
                        }
                        else
                        {
                            _str_LabelText = $"{_uint_BatteryPercent:%}";
                        }
                        label_Battery_Percentage.Text = _str_LabelText;
                    }
                }
            }
            panel_Battery.Visible = _bool_Visible;
        }
    }
}
