using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
    public partial class form_Main
    {
        private void Clean_Settings()
        {
            Print_All_Settings();
            // ensures the string lists have the same number of entries as the profile id list
            int _int_Profile_Count = 0;
            if (savedSettings.ii_int_ProfileIds != null)
            {
                _int_Profile_Count = savedSettings.ii_int_ProfileIds.Count;
            }
            if (_int_Profile_Count == 0)
            {
                savedSettings.ii_int_ProfileIds = [];
                savedSettings.ii_string_ProfileName = [];
                savedSettings.ii_int_Barbell = [];
                savedSettings.ii_HHmm_StartTimes = [];
                savedSettings.ii_int_snatch_Sec_Stage = [];
                savedSettings.ii_int_snatch_Wgt_Opener = [];
                savedSettings.ii_bool_snatch_OpenerWarmup = [];
                savedSettings.ii_int_snatch_Sec_End = [];
                savedSettings.ii_int_snatch_Lifts_Out = [];
                savedSettings.ii_int_cj_Sec_Stage = [];
                savedSettings.ii_int_cj_Sec_Break = [];
                savedSettings.ii_int_cj_Wgt_Opener = [];
                savedSettings.ii_bool_cj_OpenerWarmup = [];
                savedSettings.ii_int_cj_Sec_End = [];
                savedSettings.ii_int_cj_Lifts_Out = [];
                savedSettings.ii_int_cj_snLifts_Out = [];
                savedSettings.ii_bool_Beep = [];
                savedSettings.ii_strings_snatch_Extras = [];
                savedSettings.ii_strings_snatch_Jumps = [];
                savedSettings.ii_strings_snatch_Times = [];
                savedSettings.ii_strings_cj_Extras = [];
                savedSettings.ii_strings_cj_Jumps = [];
                savedSettings.ii_strings_cj_Times = [];
            }
            else
            {
                List<string> _strings = [];
                if (savedSettings.ii_string_ProfileName != null)
                {
                    _strings = savedSettings.ii_string_ProfileName;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add("DefaultName" + i.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_string_ProfileName = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_Barbell != null)
                {
                    _strings = savedSettings.ii_int_Barbell;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_Barbell.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_Barbell = _strings;
                }

                _strings = [];
                if (savedSettings.ii_HHmm_StartTimes != null)
                {
                    _strings = savedSettings.ii_HHmm_StartTimes;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add("1200");
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_HHmm_StartTimes = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_snatch_Sec_Stage != null)
                {
                    _strings = savedSettings.ii_int_snatch_Sec_Stage;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_snatch_Sec_Stage.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_snatch_Sec_Stage = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_snatch_Wgt_Opener != null)
                {
                    _strings = savedSettings.ii_int_snatch_Wgt_Opener;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_snatch_Wgt_Opener.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_snatch_Wgt_Opener = _strings;
                }

                _strings = [];
                if (savedSettings.ii_bool_snatch_OpenerWarmup != null)
                {
                    _strings = savedSettings.ii_bool_snatch_OpenerWarmup;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(bool_default_snatch_OpenerWarmup.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_bool_snatch_OpenerWarmup = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_snatch_Sec_End != null)
                {
                    _strings = savedSettings.ii_int_snatch_Sec_End;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_snatch_Sec_End.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_snatch_Sec_End = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_snatch_Lifts_Out != null)
                {
                    _strings = savedSettings.ii_int_snatch_Lifts_Out;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_snatch_Lifts_Out.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_snatch_Lifts_Out = _strings;
                }


                _strings = [];
                if (savedSettings.ii_int_cj_Sec_Stage != null)
                {
                    _strings = savedSettings.ii_int_cj_Sec_Stage;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_cj_Sec_Stage.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_cj_Sec_Stage = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_cj_Sec_Break != null)
                {
                    _strings = savedSettings.ii_int_cj_Sec_Break;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_cj_Sec_Break.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_cj_Sec_Break = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_cj_Wgt_Opener != null)
                {
                    _strings = savedSettings.ii_int_cj_Wgt_Opener;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_cj_Wgt_Opener.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_cj_Wgt_Opener = _strings;
                }

                _strings = [];
                if (savedSettings.ii_bool_cj_OpenerWarmup != null)
                {
                    _strings = savedSettings.ii_bool_cj_OpenerWarmup;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(bool_default_cj_OpenerWarmup.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_bool_cj_OpenerWarmup = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_cj_Sec_End != null)
                {
                    _strings = savedSettings.ii_int_cj_Sec_End;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_cj_Sec_End.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_cj_Sec_End = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_cj_Lifts_Out != null)
                {
                    _strings = savedSettings.ii_int_cj_Lifts_Out;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_cj_Lifts_Out.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_cj_Lifts_Out = _strings;
                }

                _strings = [];
                if (savedSettings.ii_int_cj_snLifts_Out != null)
                {
                    _strings = savedSettings.ii_int_cj_snLifts_Out;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(int_default_cj_snLifts_Out.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_int_cj_snLifts_Out = _strings;
                }

                _strings = [];
                if (savedSettings.ii_bool_Beep != null)
                {
                    _strings = savedSettings.ii_bool_Beep;
                }
                if (_strings.Count != _int_Profile_Count)
                {
                    int _int_Count = _strings.Count;
                    if (_int_Count < _int_Profile_Count)
                    {
                        for (int i = _int_Count + 1; i <= _int_Profile_Count; i++)
                        {
                            _strings.Add(bool_default_Beep.ToString());
                        }
                    }
                    else
                    {
                        for (int i = _int_Count - 1; i > _int_Profile_Count; i--)
                        {
                            _strings.RemoveAt(i);
                        }
                    }
                    savedSettings.ii_bool_Beep = _strings;
                }

                savedSettings.ii_strings_snatch_Extras.RemoveAll(r => !TryParseExtras(record: r, profileId: out _, extra: out _));
                savedSettings.ii_strings_snatch_Jumps.RemoveAll(r => !TryParseJumpTime(record: r, profileId: out _, fromWeight: out _, step: out _));
                savedSettings.ii_strings_snatch_Times.RemoveAll(r => !TryParseJumpTime(record: r, profileId: out _, fromWeight: out _, step: out _));
                savedSettings.ii_strings_cj_Extras.RemoveAll(r => !TryParseExtras(record: r, profileId: out _, extra: out _));
                savedSettings.ii_strings_cj_Jumps.RemoveAll(r => !TryParseJumpTime(record: r, profileId: out _, fromWeight: out _, step: out _));
                savedSettings.ii_strings_cj_Times.RemoveAll(r => !TryParseJumpTime(record: r, profileId: out _, fromWeight: out _, step: out _));
            }
            Print_All_Settings();
        }
        private void ProfileId_Select(int _int_ProfileId)
        {
            Clean_Settings();
            int_ProfileId = _int_ProfileId;
            int _int_Barbell = int_default_Barbell;
            TimeSpan _timeSpan_Start = new(12, 0, 0);
            int _int_snatch_Sec_Stage = int_default_snatch_Sec_Stage;
            int _int_snatch_Wgt_Opener = int_default_snatch_Wgt_Opener;
            int _int_snatch_Sec_End = int_default_snatch_Sec_End;
            int _int_snatch_Lifts_Out = int_default_snatch_Lifts_Out;
            int _int_cj_Sec_Stage = int_default_cj_Sec_Stage;
            int _int_cj_Sec_Break = int_default_cj_Sec_Break;
            int _int_cj_Wgt_Opener = int_default_cj_Wgt_Opener;
            int _int_cj_Sec_End = int_default_cj_Sec_End;
            int _int_cj_Lifts_Out = int_default_cj_Lifts_Out;
            int _int_cj_snLifts_Out = int_default_cj_snLifts_Out;
            bool _bool_snatch_OpenerWarmup = bool_default_snatch_OpenerWarmup;
            bool _bool_cj_OpenerWarmup = bool_default_cj_OpenerWarmup;
            bool _bool_Beep = bool_default_Beep;

            Get_Settings_Defaults_Lists();

            int _int_Sequence = int_Profile_Sequence(_int_ProfileId: _int_ProfileId);
            if (_int_Sequence > -1)
            {
                int.TryParse(s: savedSettings.ii_int_Barbell[_int_Sequence], out _int_Barbell);
                string _str_Start = savedSettings.ii_HHmm_StartTimes[_int_Sequence];
                if (!string.IsNullOrEmpty(_str_Start) && _str_Start.Length == 4)
                {
                    try
                    {
                        _timeSpan_Start = new(int.Parse(s: _str_Start.Substring(0, 2)), int.Parse(s: _str_Start.Substring(2)), 0);
                    }
                    catch { }
                }
                int.TryParse(s: savedSettings.ii_int_snatch_Sec_Stage[_int_Sequence], out _int_snatch_Sec_Stage);
                int.TryParse(s: savedSettings.ii_int_snatch_Wgt_Opener[_int_Sequence], out _int_snatch_Wgt_Opener);
                bool.TryParse(value: savedSettings.ii_bool_snatch_OpenerWarmup[_int_Sequence], out _bool_snatch_OpenerWarmup);
                int.TryParse(s: savedSettings.ii_int_snatch_Sec_End[_int_Sequence], out _int_snatch_Sec_End);
                int.TryParse(s: savedSettings.ii_int_snatch_Lifts_Out[_int_Sequence], out _int_snatch_Lifts_Out);

                int.TryParse(s: savedSettings.ii_int_cj_Sec_Stage[_int_Sequence], out _int_cj_Sec_Stage);
                int.TryParse(s: savedSettings.ii_int_cj_Sec_Break[_int_Sequence], out _int_cj_Sec_Break);
                int.TryParse(s: savedSettings.ii_int_cj_Wgt_Opener[_int_Sequence], out _int_cj_Wgt_Opener);
                bool.TryParse(value: savedSettings.ii_bool_cj_OpenerWarmup[_int_Sequence], out _bool_cj_OpenerWarmup);
                int.TryParse(s: savedSettings.ii_int_cj_Sec_End[_int_Sequence], out _int_cj_Sec_End);
                int.TryParse(s: savedSettings.ii_int_cj_Lifts_Out[_int_Sequence], out _int_cj_Lifts_Out);
                int.TryParse(s: savedSettings.ii_int_cj_snLifts_Out[_int_Sequence], out _int_cj_snLifts_Out);

                bool.TryParse(value: savedSettings.ii_bool_Beep[_int_Sequence], out _bool_Beep);
            }

            int_Barbell = _int_Barbell;
            timeSpan_Start = _timeSpan_Start;
            int_snatch_Sec_Stage = _int_snatch_Sec_Stage;
            int_snatch_Wgt_Opener = _int_snatch_Wgt_Opener;
            int_snatch_Sec_End = _int_snatch_Sec_End;
            int_snatch_Lifts_Out = _int_snatch_Lifts_Out;
            int_cj_Sec_Stage = _int_cj_Sec_Stage;
            int_cj_Sec_Break = _int_cj_Sec_Break;
            int_cj_Wgt_Opener = _int_cj_Wgt_Opener;
            int_cj_Sec_End = _int_cj_Sec_End;
            int_cj_Lifts_Out = _int_cj_Lifts_Out;
            int_cj_snLifts_Out = _int_cj_snLifts_Out;
            bool_snatch_OpenerWarmup = _bool_snatch_OpenerWarmup;
            bool_cj_OpenerWarmup = _bool_cj_OpenerWarmup;
            bool_Beep = _bool_Beep;
        }
        private void Load_Profile_Values_To_Controls()
        {
            bool _bool_Loading = bool_Loading;
            bool_Loading = true;

            snatch_Stop_Live();
            cj_Stop_Live();

            color_snatch_Live_BG = splitContainer_snatch.Panel2.BackColor;
            color_cj_Live_BG = splitContainer_cj.Panel2.BackColor;

            if (int_Barbell < numericUpDown_snatch_weight_barbell.Minimum)
            {
                int_Barbell = 20;
            }
            numericUpDown_snatch_weight_barbell.Value = int_Barbell;

            DateTime dateTime = DateTime.Today.Add(timeSpan_Start);
            if (dateTime < DateTime.Now)
            {
                dateTime = dateTime.AddDays(1);
            }
            dateTimePicker_snatch_Start.Value = dateTime;

            if (int_snatch_Sec_Stage < numericUpDown_snatch_time_stage.Minimum)
            {
                int_snatch_Sec_Stage = 55;
            }
            numericUpDown_snatch_time_stage.Value = int_snatch_Sec_Stage;

            if (int_snatch_Wgt_Opener < int_Barbell)
            {
                int_snatch_Wgt_Opener = 85;
            }
            numericUpDown_snatch_weight_opener.Value = int_snatch_Wgt_Opener;

            if (int_snatch_Sec_End < numericUpDown_snatch_time_PostWarmup.Minimum)
            {
                int_snatch_Sec_End = 60;
            }
            numericUpDown_snatch_time_PostWarmup.Value = int_snatch_Sec_End;

            if (int_snatch_Lifts_Out < 0)
            {
                int_snatch_Lifts_Out = 3;
            }
            else if (int_snatch_Lifts_Out > 99)
            {
                int_snatch_Lifts_Out = 99;
            }
            label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            label_snatch_Live_LiftsPassed.Text = string.Empty;
            snatch_Stop_Live();


            if (int_cj_Sec_Stage < numericUpDown_cj_time_stage.Minimum)
            {
                int_cj_Sec_Stage = 62;
            }
            numericUpDown_cj_time_stage.Value = int_cj_Sec_Stage;

            if (int_cj_Sec_Break < (numericUpDown_cj_Live_Break.Minimum * 60))
            {
                int_cj_Sec_Break = 10 * 60;
            }
            numericUpDown_cj_Live_Break.Value = (int)((double)int_cj_Sec_Break / 60);

            if (int_cj_Wgt_Opener < int_Barbell)
            {
                int_cj_Wgt_Opener = 108;
            }
            numericUpDown_cj_weight_opener.Value = int_cj_Wgt_Opener;

            if (int_cj_Sec_End < numericUpDown_cj_time_PostWarmup.Minimum)
            {
                int_cj_Sec_End = 75;
            }
            numericUpDown_cj_time_PostWarmup.Value = int_cj_Sec_End;

            if (int_cj_Lifts_Out < 0)
            {
                int_cj_Lifts_Out = 3;
            }
            else if (int_cj_Lifts_Out > 99)
            {
                int_cj_Lifts_Out = 99;
            }
            label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            label_cj_Live_LiftsPassed.Text = string.Empty;

            if (int_cj_snLifts_Out < 0)
            {
                int_cj_snLifts_Out = 0;
            }
            label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();

            checkBox_snatch_Param_OpenerWarmup.Checked = bool_snatch_OpenerWarmup;
            checkBox_cj_Param_OpenerWarmup.Checked = bool_cj_OpenerWarmup;

            cj_Stop_Live();


            checkBox_snatch_Live_Beep.Checked = bool_Beep;
            checkBox_cj_Live_Beep.Checked = bool_Beep;


            InitialiseCollections();

            bool bool_AutoVals;

            bool_AutoVals = true;
            if (default_snatchExtras != null && default_snatchExtras.Count > 0)
            {
                snatchExtras = [.. default_snatchExtras];
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                snatchExtras = Defaults.default_snatchExtras();
            }
            PopulateExtras(liftType: LiftType.Snatch);

            bool_AutoVals = true;
            if (default_snatchJumps != null && default_snatchJumps.Count > 0)
            {
                snatchJumps = new(default_snatchJumps);
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                snatchJumps = Defaults.default_snatchJumps();
            }
            PopulateJumps(liftType: LiftType.Snatch);

            bool_AutoVals = true;
            if (default_snatchTimes != null && default_snatchTimes.Count > 0)
            {
                snatchTimes = new(default_snatchTimes);
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                snatchTimes = Defaults.default_snatchTimes();
            }
            PopulateTimes(liftType: LiftType.Snatch);

            PopulateSteps(liftType: LiftType.Snatch, preserveLifts: false);

            bool_AutoVals = true;
            if (default_cjExtras != null && default_cjExtras.Count > 0)
            {
                cjExtras = [.. default_cjExtras];
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                cjExtras = Defaults.default_cjExtras();
            }
            PopulateExtras(liftType: LiftType.CleanAndJerk);

            bool_AutoVals = true;
            if (default_cjJumps != null)
            {
                if (default_cjJumps.Count > 0)
                {
                    cjJumps = new(default_cjJumps);
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                cjJumps = Defaults.default_cjJumps();
            }
            PopulateJumps(liftType: LiftType.CleanAndJerk);

            bool_AutoVals = true;
            if (default_cjTimes != null)
            {
                if (default_cjTimes.Count > 0)
                {
                    cjTimes = new(default_cjTimes);
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                cjTimes = Defaults.default_cjTimes();
            }
            PopulateTimes(liftType: LiftType.CleanAndJerk);

            PopulateSteps(liftType: LiftType.CleanAndJerk, preserveLifts: false);

            CheckCollections();
            ApplyOpener(liftType: LiftType.Snatch);
            ApplyOpener(liftType: LiftType.CleanAndJerk);

            bool_Loading = _bool_Loading;
        }
        private int int_Profile_Sequence(int _int_ProfileId)
        {
            if (_int_ProfileId > -1 && savedSettings.ii_int_ProfileIds != null)
            {
                if (savedSettings.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    return savedSettings.ii_int_ProfileIds.IndexOf(_int_ProfileId.ToString());
                }
            }
            return -1;
        }
        private int Add_Profile(string _str_ProfileName)
        {
            Clean_Settings();
            int _int_ProfileId = 1;
            foreach (string s in savedSettings.ii_int_ProfileIds)
            {
                if (int.TryParse(s: s, result: out int _i) &&
                    _i >= _int_ProfileId)
                {
                    _int_ProfileId = _i + 1;
                }
            }
            savedSettings.ii_int_ProfileIds.Add(_int_ProfileId.ToString());
            savedSettings.ii_string_ProfileName.Add(_str_ProfileName);
            Print_All_Settings();
            Clean_Settings();
            Print_All_Settings();
            Settings_Changes_Save();
            Print_All_Settings();

            return _int_ProfileId;
        }
        private void Delete_Profile(int _int_ProfileId)
        {
            Clean_Settings();
            int _int_Sequence = int_Profile_Sequence(_int_ProfileId: _int_ProfileId);
            if (_int_Sequence >= 0)
            {
                savedSettings.ii_int_ProfileIds.RemoveAt(_int_Sequence);
                savedSettings.ii_string_ProfileName.RemoveAt(_int_Sequence);

                savedSettings.ii_int_Barbell.RemoveAt(_int_Sequence);
                savedSettings.ii_HHmm_StartTimes.RemoveAt(_int_Sequence);
                savedSettings.ii_int_snatch_Sec_Stage.RemoveAt(_int_Sequence);
                savedSettings.ii_int_snatch_Wgt_Opener.RemoveAt(_int_Sequence);
                savedSettings.ii_bool_snatch_OpenerWarmup.RemoveAt(_int_Sequence);
                savedSettings.ii_int_snatch_Sec_End.RemoveAt(_int_Sequence);
                savedSettings.ii_int_snatch_Lifts_Out.RemoveAt(_int_Sequence);

                savedSettings.ii_int_cj_Sec_Stage.RemoveAt(_int_Sequence);
                savedSettings.ii_int_cj_Wgt_Opener.RemoveAt(_int_Sequence);
                savedSettings.ii_int_cj_Sec_Break.RemoveAt(_int_Sequence);
                savedSettings.ii_bool_cj_OpenerWarmup.RemoveAt(_int_Sequence);
                savedSettings.ii_int_cj_Sec_End.RemoveAt(_int_Sequence);
                savedSettings.ii_int_cj_Lifts_Out.RemoveAt(_int_Sequence);
                savedSettings.ii_int_cj_snLifts_Out.RemoveAt(_int_Sequence);

                savedSettings.ii_strings_snatch_Extras.RemoveAll(r => int_ParseOutProfileId(record: r) == _int_ProfileId);
                savedSettings.ii_strings_snatch_Jumps.RemoveAll(r => int_ParseOutProfileId(record: r) == _int_ProfileId);
                savedSettings.ii_strings_snatch_Times.RemoveAll(r => int_ParseOutProfileId(record: r) == _int_ProfileId);
                savedSettings.ii_strings_cj_Extras.RemoveAll(r => int_ParseOutProfileId(record: r) == _int_ProfileId);
                savedSettings.ii_strings_cj_Jumps.RemoveAll(r => int_ParseOutProfileId(record: r) == _int_ProfileId);
                savedSettings.ii_strings_cj_Times.RemoveAll(r => int_ParseOutProfileId(record: r) == _int_ProfileId);
            }
            if (savedSettings.int_ProfileId == _int_ProfileId)
            {
                savedSettings.int_ProfileId = -1;
            }
        }
        private string string_Profile_Name_From_Id(int _int_ProfileId)
        {
            if (savedSettings.ii_string_ProfileName != null &&
                savedSettings.ii_string_ProfileName.Count > 0)
            {
                int _int_Sequence = int_Profile_Sequence(_int_ProfileId: _int_ProfileId);
                return savedSettings.ii_string_ProfileName[_int_Sequence];
            }
            return "default";
        }
        private void Settings_Changes_Save()
        {
            savedSettings.Save();
            Print_All_Settings();
        }
        private void Update_Settings()
        {
            Print_All_Settings();
            savedSettings.int_ProfileId = int_ProfileId;
            Clean_Settings();
            int _int_Sequence = int_Profile_Sequence(_int_ProfileId: int_ProfileId);
            if (_int_Sequence < 0)
            {
                if (savedSettings.ii_int_ProfileIds == null)
                {
                    savedSettings.ii_int_ProfileIds =
                    [
                        int_ProfileId.ToString()
                    ];
                }
                else
                {
                    savedSettings.ii_int_ProfileIds.Add(int_ProfileId.ToString());
                }
                Clean_Settings();
            }
            savedSettings.ii_int_Barbell[_int_Sequence] = int_Barbell.ToString();
            savedSettings.ii_HHmm_StartTimes[_int_Sequence] = dateTimePicker_snatch_Start.Value.ToString("HHmm");
            savedSettings.ii_int_snatch_Sec_Stage[_int_Sequence] = int_snatch_Sec_Stage.ToString();
            savedSettings.ii_int_snatch_Wgt_Opener[_int_Sequence] = int_snatch_Wgt_Opener.ToString();
            savedSettings.ii_bool_snatch_OpenerWarmup[_int_Sequence] = bool_snatch_OpenerWarmup.ToString();
            savedSettings.ii_int_snatch_Sec_End[_int_Sequence] = int_snatch_Sec_End.ToString();
            savedSettings.ii_int_snatch_Lifts_Out[_int_Sequence] = int_snatch_Lifts_Out.ToString();

            savedSettings.ii_int_cj_Sec_Stage[_int_Sequence] = int_cj_Sec_Stage.ToString();
            savedSettings.ii_int_cj_Wgt_Opener[_int_Sequence] = int_cj_Wgt_Opener.ToString();
            savedSettings.ii_int_cj_Sec_Break[_int_Sequence] = int_cj_Sec_Break.ToString();
            savedSettings.ii_bool_cj_OpenerWarmup[_int_Sequence] = bool_cj_OpenerWarmup.ToString();
            savedSettings.ii_int_cj_Sec_End[_int_Sequence] = int_cj_Sec_End.ToString();
            savedSettings.ii_int_cj_Lifts_Out[_int_Sequence] = int_cj_Lifts_Out.ToString();
            savedSettings.ii_int_cj_snLifts_Out[_int_Sequence] = int_cj_snLifts_Out.ToString();

            savedSettings.ii_bool_Beep[_int_Sequence] = bool_Beep.ToString();

            savedSettings.ii_strings_snatch_Extras.RemoveAll(r => int_ParseOutProfileId(r) == int_ProfileId || !savedSettings.ii_int_ProfileIds.Contains(int_ParseOutProfileId(r).ToString()));
            savedSettings.ii_strings_snatch_Jumps.RemoveAll(r => int_ParseOutProfileId(r) == int_ProfileId || !savedSettings.ii_int_ProfileIds.Contains(int_ParseOutProfileId(r).ToString()));
            savedSettings.ii_strings_snatch_Times.RemoveAll(r => int_ParseOutProfileId(r) == int_ProfileId || !savedSettings.ii_int_ProfileIds.Contains(int_ParseOutProfileId(r).ToString()));
            savedSettings.ii_strings_cj_Extras.RemoveAll(r => int_ParseOutProfileId(r) == int_ProfileId || !savedSettings.ii_int_ProfileIds.Contains(int_ParseOutProfileId(r).ToString()));
            savedSettings.ii_strings_cj_Jumps.RemoveAll(r => int_ParseOutProfileId(r) == int_ProfileId || !savedSettings.ii_int_ProfileIds.Contains(int_ParseOutProfileId(r).ToString()));
            savedSettings.ii_strings_cj_Times.RemoveAll(r => int_ParseOutProfileId(r) == int_ProfileId || !savedSettings.ii_int_ProfileIds.Contains(int_ParseOutProfileId(r).ToString()));

            savedSettings.ii_strings_snatch_Extras.AddRange(snatchExtras.OrderBy(r => r.id).Select(r => $"{int_ProfileId:000}{r}"));
            savedSettings.ii_strings_snatch_Jumps.AddRange(snatchJumps.OrderBy(r => r.Key).Select(r => $"{int_ProfileId:000}{r.Key:000}{r.Value:000}"));
            savedSettings.ii_strings_snatch_Times.AddRange(snatchTimes.OrderBy(r => r.Key).Select(r => $"{int_ProfileId:000}{r.Key:000}{r.Value:000}"));
            savedSettings.ii_strings_cj_Extras.AddRange(cjExtras.OrderBy(r => r.id).Select(r => $"{int_ProfileId:000}{r}"));
            savedSettings.ii_strings_cj_Jumps.AddRange(cjJumps.OrderBy(r => r.Key).Select(r => $"{int_ProfileId:000}{r.Key:000}{r.Value:000}"));
            savedSettings.ii_strings_cj_Times.AddRange(cjTimes.OrderBy(r => r.Key).Select(r => $"{int_ProfileId:000}{r.Key:000}{r.Value:000}"));

            Print_All_Settings();
        }
        private int int_ParseOutProfileId(string record)
        {
            if (string.IsNullOrEmpty(record) ||
                !int.TryParse(record.Substring(0, Math.Min(record.Length, 3)), out int i))
            {
                return default;
            }
            else
            {
                return i;
            }
        }
        private bool TryParseExtras(
            string record,
            out int profileId,
            out Extra extra)
        {
            //  at character:
            //  0   3 digit profile id
            //  3   6 digit id
            //  9   3 digit order
            //  12  5 digit length
            //  17  variable length string (action name)
            profileId = int_ParseOutProfileId(record: record);
            extra = default;
            if (record?.Length < 17)
            {
                return false;
            }

            if (int.TryParse(record.Substring(3, 6), out int id) &&
                int.TryParse(record.Substring(9, 3), out int order) && order > -1 &&
                int.TryParse(record.Substring(12, 5), out int length) && length > 0)
            {
                string action = record.Substring(17);
                extra = new(id, action, length, order);
                return true;
            }
            return false;
        }
        private bool TryParseJumpTime(string record, out int profileId, out int fromWeight, out int step)
        {
            profileId = int_ParseOutProfileId(record: record);
            fromWeight = 0;
            step = 0;
            if (record?.Length != 9)
            {
                return false;
            }
            return int.TryParse(record.Substring(3, 3), out fromWeight) &&
                   int.TryParse(record.Substring(6, 3), out step) && step > 0;
        }
        private void Get_Settings_Defaults_Lists()
        {
            default_snatchExtras = [];
            default_snatchJumps = [];
            default_snatchTimes = [];
            default_cjExtras = [];
            default_cjJumps = [];
            default_cjTimes = [];
            Clean_Settings();
            foreach (string s in savedSettings.ii_strings_snatch_Extras)
            {
                TryParseExtras(
                    record: s,
                    profileId: out int profileId,
                    extra: out Extra _extra);
                if (profileId == int_ProfileId)
                {
                    default_snatchExtras.Add(_extra);
                }
            }
            if (default_snatchExtras.Count == 0)
            {
                default_snatchExtras = Defaults.default_snatchExtras();
            }

            foreach (string s in savedSettings.ii_strings_snatch_Jumps)
            {
                if (TryParseJumpTime(
                    record: s,
                    profileId: out int profileId,
                    fromWeight: out int fromWeight,
                    step: out int step) &&
                    profileId == int_ProfileId)
                {
                    default_snatchJumps[fromWeight] = step;
                }
            }
            if (default_snatchJumps.Count == 0)
            {
                default_snatchJumps = Defaults.default_snatchJumps();
            }

            foreach (string s in savedSettings.ii_strings_snatch_Times)
            {
                if (TryParseJumpTime(
                    record: s,
                    profileId: out int profileId,
                    fromWeight: out int fromWeight,
                    step: out int step) &&
                    profileId == int_ProfileId)
                {
                    default_snatchTimes[fromWeight] = step;
                }
            }
            if (default_snatchTimes.Count == 0)
            {
                default_snatchTimes = Defaults.default_snatchTimes();
            }


            foreach (string s in savedSettings.ii_strings_cj_Extras)
            {
                TryParseExtras(
                    record: s,
                    profileId: out int profileId,
                    extra: out Extra _extra);
                if (profileId == int_ProfileId)
                {
                    default_cjExtras.Add(_extra);
                }
            }
            if (default_cjExtras.Count == 0)
            {
                default_cjExtras = Defaults.default_cjExtras();
            }

            foreach (string s in savedSettings.ii_strings_cj_Jumps)
            {
                if (TryParseJumpTime(
                    record: s,
                    profileId: out int profileId,
                    fromWeight: out int fromWeight,
                    step: out int step) &&
                    profileId == int_ProfileId)
                {
                    default_cjJumps[fromWeight] = step;
                }
            }
            if (default_cjJumps.Count == 0)
            {
                default_cjJumps = Defaults.default_cjJumps();
            }

            foreach (string s in savedSettings.ii_strings_cj_Times)
            {
                if (TryParseJumpTime(
                    record: s,
                    profileId: out int profileId,
                    fromWeight: out int fromWeight,
                    step: out int step) &&
                    profileId == int_ProfileId)
                {
                    default_cjTimes[fromWeight] = step;
                }
            }
            if (default_cjTimes.Count == 0)
            {
                default_cjTimes = Defaults.default_cjTimes();
            }
        }
        private void button_snatch_ClearSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"This will erase all profiles and restore all defaults." +
                Environment.NewLine + Environment.NewLine + "Continue?",
                "Reset settings?",
                buttons: MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                savedSettings.Reset();
                Settings_Changes_Save();
                int_ProfileId = Add_Profile(_str_ProfileName: "default");
                Initialise_Form();
            }
        }
        private void Print_All_Settings()
        {
            static void PrintCollection<T>(string name, IEnumerable<T>? collection)
            {
                string value = collection == null ? "null" : string.Join("|", collection);
                Console.WriteLine($"{name} = {value}");
            }

            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");

            // Main profile settings
            Console.WriteLine($"int_ProfileId = {savedSettings.int_ProfileId}");
            PrintCollection(nameof(savedSettings.ii_int_ProfileIds), savedSettings.ii_int_ProfileIds);
            PrintCollection(nameof(savedSettings.ii_string_ProfileName), savedSettings.ii_string_ProfileName);
            PrintCollection(nameof(savedSettings.ii_int_Barbell), savedSettings.ii_int_Barbell);
            PrintCollection(nameof(savedSettings.ii_HHmm_StartTimes), savedSettings.ii_HHmm_StartTimes);

            // Snatch settings
            PrintCollection(nameof(savedSettings.ii_int_snatch_Sec_Stage), savedSettings.ii_int_snatch_Sec_Stage);
            PrintCollection(nameof(savedSettings.ii_int_snatch_Wgt_Opener), savedSettings.ii_int_snatch_Wgt_Opener);
            PrintCollection(nameof(savedSettings.ii_int_snatch_Sec_End), savedSettings.ii_int_snatch_Sec_End);
            PrintCollection(nameof(savedSettings.ii_int_snatch_Lifts_Out), savedSettings.ii_int_snatch_Lifts_Out);
            PrintCollection(nameof(savedSettings.ii_bool_snatch_OpenerWarmup), savedSettings.ii_bool_snatch_OpenerWarmup);
            PrintCollection(nameof(savedSettings.ii_strings_snatch_Extras), savedSettings.ii_strings_snatch_Extras);
            PrintCollection(nameof(savedSettings.ii_strings_snatch_Jumps), savedSettings.ii_strings_snatch_Jumps);
            PrintCollection(nameof(savedSettings.ii_strings_snatch_Times), savedSettings.ii_strings_snatch_Times);

            // Clean & Jerk settings
            PrintCollection(nameof(savedSettings.ii_int_cj_Sec_Stage), savedSettings.ii_int_cj_Sec_Stage);
            PrintCollection(nameof(savedSettings.ii_int_cj_Wgt_Opener), savedSettings.ii_int_cj_Wgt_Opener);
            PrintCollection(nameof(savedSettings.ii_int_cj_Sec_End), savedSettings.ii_int_cj_Sec_End);
            PrintCollection(nameof(savedSettings.ii_int_cj_Lifts_Out), savedSettings.ii_int_cj_Lifts_Out);
            PrintCollection(nameof(savedSettings.ii_int_cj_Sec_Break), savedSettings.ii_int_cj_Sec_Break);
            PrintCollection(nameof(savedSettings.ii_int_cj_snLifts_Out), savedSettings.ii_int_cj_snLifts_Out);
            PrintCollection(nameof(savedSettings.ii_bool_cj_OpenerWarmup), savedSettings.ii_bool_cj_OpenerWarmup);
            PrintCollection(nameof(savedSettings.ii_strings_cj_Extras), savedSettings.ii_strings_cj_Extras);
            PrintCollection(nameof(savedSettings.ii_strings_cj_Jumps), savedSettings.ii_strings_cj_Jumps);
            PrintCollection(nameof(savedSettings.ii_strings_cj_Times), savedSettings.ii_strings_cj_Times);

            // Miscellaneous settings
            PrintCollection(nameof(savedSettings.ii_bool_Beep), savedSettings.ii_bool_Beep);

            Console.WriteLine("---------------------------------------");
        }
        private void Populate_MenuStrip()
        {
            menuStrip_Profile.Items.Clear();
            ToolStripLabel toolStripLabel = new()
            {
                Text = "v" + strVersion + "     ",
                Margin = new(0, 0, 10, 0),
                Font = new("Gadugi", 10F, FontStyle.Italic, GraphicsUnit.Point, 0)
            };
            menuStrip_Profile.Items.Add(toolStripLabel);
            for (int i = 0; i < savedSettings.ii_int_ProfileIds.Count; i++)
            {
                if (int.TryParse(s: savedSettings.ii_int_ProfileIds[i], result: out int _int_ProfileId) &&
                    _int_ProfileId > 0)
                {
                    string _str_ProfileName = string_Profile_Name_From_Id(_int_ProfileId: _int_ProfileId);
                    ToolStripMenuItem toolStripMenuItem = new()
                    {
                        Text = _str_ProfileName,
                        Tag = _int_ProfileId.ToString()
                    };
                    ToolStripButton toolStripButton;
                    if (_int_ProfileId == int_ProfileId)
                    {
                        toolStripMenuItem.BackColor = Color.Red;
                    }
                    else
                    {
                        toolStripButton = new()
                        {
                            Text = "load",
                            Tag = _int_ProfileId.ToString(),
                        };
                        toolStripButton.Click += ToolStripMenu_Load_Profile;
                        toolStripMenuItem.DropDownItems.Add(toolStripButton);
                    }
                    toolStripButton = new()
                    {
                        Text = "delete",
                        Tag = _int_ProfileId.ToString(),
                    };
                    toolStripButton.Click += ToolStripMenu_Delete_Profile;
                    toolStripMenuItem.DropDownItems.Add(toolStripButton);
                    menuStrip_Profile.Items.Add(toolStripMenuItem);
                    toolStripButton = new()
                    {
                        Text = "duplicate",
                        Tag = _int_ProfileId.ToString(),
                    };
                    toolStripButton.Click += ToolStripMenu_Duplicate_Profile;
                    toolStripMenuItem.DropDownItems.Add(toolStripButton);
                    menuStrip_Profile.Items.Add(toolStripMenuItem);
                    toolStripButton = new()
                    {
                        Text = "rename",
                        Tag = _int_ProfileId.ToString(),
                    };
                    toolStripButton.Click += ToolStripMenu_Rename_Profile;
                    toolStripMenuItem.DropDownItems.Add(toolStripButton);
                    menuStrip_Profile.Items.Add(toolStripMenuItem);
                }
            }
            {
                ToolStripMenuItem toolStripMenuItem = new()
                {
                    Text = "Add new profile",
                    Font = new("Gadugi", 9F, FontStyle.Italic, GraphicsUnit.Point, 0)
                };
                toolStripMenuItem.Click += ToolStripMenu_AddNew_Profile;
                menuStrip_Profile.Items.Add(toolStripMenuItem);
            }
        }
        private void ToolStripMenu_Load_Profile(object sender, EventArgs e)
        {
            Update_Settings();
            Settings_Changes_Save();
            Print_All_Settings();
            ToolStripButton toolStripButton = (ToolStripButton)sender;
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId))
            {
                ProfileId_Select(_int_ProfileId: _int_ProfileId);
                Populate_MenuStrip();
                Load_Profile_Values_To_Controls();
            }
        }
        private void ToolStripMenu_Delete_Profile(object sender, EventArgs e)
        {
            ToolStripButton toolStripButton = (ToolStripButton)sender;
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId))
            {
                if (MessageBox.Show(
                    text: "Are you sure you want to delete this profile? This action is permanent.",
                    caption: "Delete",
                    buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Delete_Profile(_int_ProfileId: _int_ProfileId);
                    if (_int_ProfileId == int_ProfileId)
                    {
                        int _int_NewProfileId = -1;
                        foreach (string s in savedSettings.ii_int_ProfileIds)
                        {
                            _int_NewProfileId = int.Parse(s: s);
                            break;
                        }
                        if (_int_NewProfileId < 1)
                        {
                            // could not find a current profile
                            _int_ProfileId = Add_Profile(_str_ProfileName: "default");
                        }
                        ProfileId_Select(_int_ProfileId: _int_ProfileId);
                        Populate_MenuStrip();
                        Load_Profile_Values_To_Controls();
                    }
                    else
                    {
                        Populate_MenuStrip();
                    }
                }
            }
        }
        private void ToolStripMenu_AddNew_Profile(object sender, EventArgs e)
        {
            string _str_Name = Interaction.InputBox("Enter a new name:");
            if (!string.IsNullOrEmpty(_str_Name))
            {
                Add_Profile(_str_ProfileName: _str_Name);
                Populate_MenuStrip();
            }
        }
        private void ToolStripMenu_Rename_Profile(object sender, EventArgs e)
        {
            ToolStripButton toolStripButton = (ToolStripButton)sender;
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId))
            {
                string _str_Name = string_Profile_Name_From_Id(_int_ProfileId: _int_ProfileId);
                _str_Name = Interaction.InputBox(Prompt: "Enter a new name:", DefaultResponse: _str_Name);
                if (!string.IsNullOrEmpty(_str_Name))
                {
                    Clean_Settings();
                    int _int_Sequence = int_Profile_Sequence(_int_ProfileId: _int_ProfileId);
                    if (_int_Sequence >= 0)
                    {
                        savedSettings.ii_string_ProfileName[_int_Sequence] = _str_Name;
                        Settings_Changes_Save();
                        Populate_MenuStrip();
                    }
                }
            }
        }
        private void ToolStripMenu_Duplicate_Profile(object sender, EventArgs e)
        {
            ToolStripButton toolStripButton = (ToolStripButton)sender;
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId))
            {
                Update_Settings();
                Settings_Changes_Save();
                Clean_Settings();
                string _str_Name = string_Profile_Name_From_Id(_int_ProfileId: _int_ProfileId);
                _str_Name = Interaction.InputBox(Prompt: "Enter a new name:", DefaultResponse: _str_Name);
                if (!string.IsNullOrEmpty(_str_Name))
                {
                    int _int_New_ProfileId = Add_Profile(_str_ProfileName: _str_Name);
                    if (_int_New_ProfileId > 0)
                    {
                        int _int_New_Sequence = int_Profile_Sequence(_int_ProfileId: _int_New_ProfileId);
                        int _int_Sequence = int_Profile_Sequence(_int_ProfileId: _int_ProfileId);
                        if (_int_Sequence >= 0 && _int_New_Sequence >= 0)
                        {
                            savedSettings.ii_int_Barbell[_int_New_Sequence] = savedSettings.ii_int_Barbell[_int_Sequence];
                            savedSettings.ii_HHmm_StartTimes[_int_New_Sequence] = savedSettings.ii_HHmm_StartTimes[_int_Sequence];

                            savedSettings.ii_int_snatch_Sec_Stage[_int_New_Sequence] = savedSettings.ii_int_snatch_Sec_Stage[_int_Sequence];
                            savedSettings.ii_int_snatch_Wgt_Opener[_int_New_Sequence] = savedSettings.ii_int_snatch_Wgt_Opener[_int_Sequence];
                            savedSettings.ii_int_snatch_Sec_End[_int_New_Sequence] = savedSettings.ii_int_snatch_Sec_End[_int_Sequence];
                            savedSettings.ii_int_snatch_Lifts_Out[_int_New_Sequence] = savedSettings.ii_int_snatch_Lifts_Out[_int_Sequence];
                            savedSettings.ii_bool_snatch_OpenerWarmup[_int_New_Sequence] = savedSettings.ii_bool_snatch_OpenerWarmup[_int_Sequence];
                            for (int i = 0; i < savedSettings.ii_strings_snatch_Extras.Count; i++)
                            {
                                string s = savedSettings.ii_strings_snatch_Extras[i];
                                TryParseExtras(
                                    record: s,
                                    profileId: out int __int_ProfileId,
                                    extra: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    savedSettings.ii_strings_snatch_Extras.Add($"{_int_New_ProfileId:000}{s.Substring(3)}");
                                }
                            }
                            for (int i = 0; i < savedSettings.ii_strings_snatch_Jumps.Count; i++)
                            {
                                string s = savedSettings.ii_strings_snatch_Jumps[i];
                                                                if (TryParseJumpTime(
                                    record: s,
                                    profileId: out int __int_ProfileId,
                                    fromWeight: out _,
                                    step: out _) &&
                                    __int_ProfileId == _int_ProfileId)
                                {
                                    savedSettings.ii_strings_snatch_Jumps.Add($"{_int_New_ProfileId:000}{s.Substring(3)}");
                                }
                            }
                            for (int i = 0; i < savedSettings.ii_strings_snatch_Times.Count; i++)
                            {
                                string s = savedSettings.ii_strings_snatch_Times[i];
                                if (TryParseJumpTime(
                                    record: s,
                                    profileId: out int __int_ProfileId,
                                    fromWeight: out _,
                                    step: out _) &&
                                    __int_ProfileId == _int_ProfileId)
                                {
                                                                        savedSettings.ii_strings_snatch_Times.Add($"{_int_New_ProfileId:000}{s.Substring(3)}");
                                }
                            }

                            savedSettings.ii_int_cj_Sec_Stage[_int_New_Sequence] = savedSettings.ii_int_cj_Sec_Stage[_int_Sequence];
                            savedSettings.ii_int_cj_Wgt_Opener[_int_New_Sequence] = savedSettings.ii_int_cj_Wgt_Opener[_int_Sequence];
                            savedSettings.ii_int_cj_Sec_End[_int_New_Sequence] = savedSettings.ii_int_cj_Sec_End[_int_Sequence];
                            savedSettings.ii_int_cj_Lifts_Out[_int_New_Sequence] = savedSettings.ii_int_cj_Lifts_Out[_int_Sequence];
                            savedSettings.ii_int_cj_Sec_Break[_int_New_Sequence] = savedSettings.ii_int_cj_Sec_Break[_int_Sequence];
                            savedSettings.ii_int_cj_snLifts_Out[_int_New_Sequence] = savedSettings.ii_int_cj_snLifts_Out[_int_Sequence];
                            savedSettings.ii_bool_cj_OpenerWarmup[_int_New_Sequence] = savedSettings.ii_bool_cj_OpenerWarmup[_int_Sequence];
                            for (int i = 0; i < savedSettings.ii_strings_cj_Extras.Count; i++)
                            {
                                string s = savedSettings.ii_strings_cj_Extras[i];
                                TryParseExtras(
                                    record: s,
                                    profileId: out int __int_ProfileId,
                                    extra: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    savedSettings.ii_strings_cj_Extras.Add($"{_int_New_ProfileId:000}{s.Substring(3)}");
                                }
                            }
                            for (int i = 0; i < savedSettings.ii_strings_cj_Jumps.Count; i++)
                            {
                                string s = savedSettings.ii_strings_cj_Jumps[i];
                                if (TryParseJumpTime(
                                    record: s,
                                    profileId: out int __int_ProfileId,
                                    fromWeight: out _,
                                    step: out _) &&
                                    __int_ProfileId == _int_ProfileId)
                                {
                                    savedSettings.ii_strings_cj_Jumps.Add($"{_int_New_ProfileId:000}{s.Substring(3)}");
                                }
                            }
                            for (int i = 0; i < savedSettings.ii_strings_cj_Times.Count; i++)
                            {
                                string s = savedSettings.ii_strings_cj_Times[i];
                                if (TryParseJumpTime(
                                    record: s,
                                    profileId: out int __int_ProfileId,
                                    fromWeight: out _,
                                    step: out _) &&
                                    __int_ProfileId == _int_ProfileId)
                                {
                                        savedSettings.ii_strings_cj_Times.Add($"{_int_New_ProfileId:000}{s.Substring(3)}");
                                }
                            }

                            savedSettings.ii_bool_Beep[_int_New_Sequence] = savedSettings.ii_bool_Beep[_int_Sequence];

                            Settings_Changes_Save();
                        }
                    }
                }
            }
            Populate_MenuStrip();
        }
    }
}
