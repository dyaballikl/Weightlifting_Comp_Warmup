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
        private void Smooth_Last_Jumps(Dictionary<int, bool> _weights, int _int_Opener)
        {
            IEnumerable<int> topThreeWeights = _weights
                .Where(r => r.Key > 0)
                .OrderByDescending(r => r.Key)
                .Select(r => r.Key);
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
                    (((int)(decimal)(_int_FinalWarmup - _int_ThirdToFinalWarmup) / 2m + _int_ThirdToFinalWarmup) != _int_SecondToFinalWarmup) && // 2nd to last warmup not right in the middle (already smoothed)
                    !_weights[_int_SecondToFinalWarmup]) // not an override
                {
                    _weights.Remove(_int_SecondToFinalWarmup);
                    _int_SecondToFinalWarmup = (int)Math.Floor((_int_FinalWarmup - _int_ThirdToFinalWarmup) / 2m + _int_ThirdToFinalWarmup);
                    _weights[_int_SecondToFinalWarmup] = false;
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
            if (_bool_PreserveLifts && _stepsIn != null)
            {
                foreach (Step _step in _stepsIn)
                {
                    if ((_step.Weight ?? 0) > 0)
                    {
                        _weightInputs[(int)_step.Weight] = _step.Override;
                    }
                }
                //remove final auto populated weights
                int _max = _weightInputs.Keys.Max();
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
                    @override: false
                    ));
                _int_Order++;
            }

            int _int_Weight_Last = _weights.Last().Key;
            foreach (KeyValuePair<int, bool> _kvp_Weight in _weights)
            {
                int intTime = _times
                    .Where(r => r.Key <= _kvp_Weight.Key)
                    .OrderByDescending(r => r.Key)
                    .FirstOrDefault().Value;
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
                    _int_Jump = profileActive.BarbellWeight;
                    _bool_Override = false;
                }
                else
                {
                    _bool_Override = false;
                    _int_Jump = GetJumpForWeight(_jumps, _int_Weight);
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
            _panelGraphic.Controls.Add(new WeightBox()
            {
                Weight = _intWeightOpener,
                BarWeight = _intWeightBar,
                isOpener = true,
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
}
