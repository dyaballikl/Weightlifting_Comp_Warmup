﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Management;
using Microsoft.Win32;
using System.Linq;

namespace Weightlifting_Comp_Warmup
{
    public partial class Form_WL_Comp_Warmup : Form
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }
        #region Variables
        private const string
            strVersion = "1.15.0";

        private DataTable
            dt_snatch_extras = null,
            dt_snatch_jumps = null,
            dt_snatch_times = null,
            dt_snatch_LIVE = null,
            dt_snatch_PLAN = null,
            dt_cj_extras = null,
            dt_cj_jumps = null,
            dt_cj_times = null,
            dt_cj_LIVE = null,
            dt_cj_PLAN = null,
            dt_default_snatch_extras = null,
            dt_default_snatch_jumps = null,
            dt_default_snatch_times = null,
            dt_default_cj_extras = null,
            dt_default_cj_jumps = null,
            dt_default_cj_times = null;
        
        private const string
            str_col_Action = "action",
            str_col_Length = "length",
            str_col_TotalLength = "totallength",
            str_col_TotalLengthReverse = "totallengthrev",
            str_col_FromWeight = "fromweight",
            str_col_Weight = "weight",
            str_col_Order = "order",
            str_col_PreStep = "prestep",
            str_col_Override = "override",
            str_col_Id = "id",
            str_col_Jump = "jump",
            str_col_PanelLiveStep = "pls",
            str_col_LabelAction = "la",
            str_col_LabelTime = "lt",
            str_col_LabelWeight = "lw",
            str_col_ProgressBarStep = "pbs",
            str_col_GraphicPanel = "gp",
            str_col_LabelProgressTime = "lpt",
            str_buttontext_up = "^",
            str_buttontext_down = "v",
            str_buttontext_delete = "X",
            str_buttontext_commit = "commit";

        private int
            int_ProfileId,
            int_Barbell,
            int_snatch_Sec_Stage,
            int_snatch_Wgt_Opener,
            int_snatch_Sec_End,
            int_snatch_Lifts_Out,
            int_snatch_Lifts_Passed,
            int_snatch_Warmup_Step,
            int_cj_Sec_Stage,
            int_cj_Wgt_Opener,
            int_cj_Sec_End,
            int_cj_Lifts_Out,
            int_cj_Lifts_Passed,
            int_cj_snLifts_Out,
            int_cj_Warmup_Step,
            int_cj_Sec_Break;

        private TimeSpan
            timeSpan_Start;

        private bool
            bool_snatch_Live, 
            bool_snatch_LiveLifting,
            bool_snatch_AutoAdvance = false,
            bool_cj_Live,
            bool_cj_LiveLifting,
            bool_cj_BreakRunning,
            bool_cj_sn_Lifting,
            bool_cj_AutoAdvance = false,
            bool_Beep = false,
            bool_Loading = true,
            bool_snatch_OpenerWarmup,
            bool_cj_OpenerWarmup;

        private const int
            int_default_Barbell = 20,
            int_default_snatch_Sec_Stage = 55,
            int_default_snatch_Wgt_Opener = 85,
            int_default_snatch_Sec_End = 60,
            int_default_snatch_Lifts_Out = 0,
            int_default_cj_Sec_Stage = 62,
            int_default_cj_Wgt_Opener = 110,
            int_default_cj_Sec_End = 75,
            int_default_cj_Lifts_Out = 0,
            int_default_cj_snLifts_Out = 0,
            int_default_cj_Sec_Break = 0;

        private const bool
            bool_default_Beep = false,
            bool_default_snatch_OpenerWarmup = true,
            bool_default_cj_OpenerWarmup = false;

        private DateTime
            datetime_snatch_Start;

        private Timer
            timer_snatch_Live,
            timer_cj_Live;

        private readonly Color
            color_Live_Default_FG = Color.FromArgb(240, 240, 240),
            color_Live_Highlight_BG = Color.Yellow,
            color_Live_Highlight_FG = Color.Black,
            color_AdvanceButton_Active = Color.FromArgb(32, 150, 32),
            color_Plate_Red = Color.FromArgb(251, 13, 27),
            color_Plate_Blue = Color.FromArgb(10, 100, 255),
            color_Plate_Yellow = Color.FromArgb(255, 253, 56),
            color_Plate_Green = Color.FromArgb(15, 127, 18),
            color_Plate_White = Color.FromArgb(255, 255, 255),
            color_BarGrey = Color.LightSteelBlue;

        private Color
            color_snatch_Live_BG,
            color_cj_Live_BG;

        PropertyData propertyData_Battery = null;

        #endregion

        #region Form Level
        public Form_WL_Comp_Warmup()
        {
            //Properties.Settings.Default.Reset();
            //Settings_Changes_Save();
            Clean_Settings();
            Print_All_Settings();
            //Properties.Settings.Default.Reset();
            //Settings_Changes_Save();

            int _int_ProfileId = -1;
            try
            {
                _int_ProfileId = Properties.Settings.Default.int_ProfileId;
            }
            catch { };

            if (_int_ProfileId < 1)
            {
                if (Properties.Settings.Default.ii_int_ProfileIds != null &&
                    Properties.Settings.Default.ii_int_ProfileIds.Count > 0)
                {
                    int.TryParse(s: Properties.Settings.Default.ii_int_ProfileIds[0], result: out _int_ProfileId);
                }
            }
            if (_int_ProfileId < 1)
            {
                _int_ProfileId = Add_Profile(_str_ProfileName: "default");
                Print_All_Settings();
            }
            ProfileId_Select(_int_ProfileId: _int_ProfileId);

            InitializeComponent();
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
                int.TryParse(s: Properties.Settings.Default.ii_int_Barbell[_int_Sequence], out _int_Barbell);
                string _str_Start = Properties.Settings.Default.ii_HHmm_StartTimes[_int_Sequence];
                if (!string.IsNullOrEmpty(_str_Start) && _str_Start.Length == 4)
                {
                    try
                    {
                        _timeSpan_Start = new(int.Parse(s: _str_Start.Substring(0, 2)), int.Parse(s: _str_Start.Substring(2)), 0);
                    }
                    catch { }
                }
                int.TryParse(s: Properties.Settings.Default.ii_int_snatch_Sec_Stage[_int_Sequence], out _int_snatch_Sec_Stage);
                int.TryParse(s: Properties.Settings.Default.ii_int_snatch_Wgt_Opener[_int_Sequence], out _int_snatch_Wgt_Opener);
                bool.TryParse(value: Properties.Settings.Default.ii_bool_snatch_OpenerWarmup[_int_Sequence], out _bool_snatch_OpenerWarmup);
                int.TryParse(s: Properties.Settings.Default.ii_int_snatch_Sec_End[_int_Sequence], out _int_snatch_Sec_End);
                int.TryParse(s: Properties.Settings.Default.ii_int_snatch_Lifts_Out[_int_Sequence], out _int_snatch_Lifts_Out);

                int.TryParse(s: Properties.Settings.Default.ii_int_cj_Sec_Stage[_int_Sequence], out _int_cj_Sec_Stage);
                int.TryParse(s: Properties.Settings.Default.ii_int_cj_Sec_Break[_int_Sequence], out _int_cj_Sec_Break);
                int.TryParse(s: Properties.Settings.Default.ii_int_cj_Wgt_Opener[_int_Sequence], out _int_cj_Wgt_Opener);
                bool.TryParse(value: Properties.Settings.Default.ii_bool_cj_OpenerWarmup[_int_Sequence], out _bool_cj_OpenerWarmup);
                int.TryParse(s: Properties.Settings.Default.ii_int_cj_Sec_End[_int_Sequence], out _int_cj_Sec_End);
                int.TryParse(s: Properties.Settings.Default.ii_int_cj_Lifts_Out[_int_Sequence], out _int_cj_Lifts_Out);
                int.TryParse(s: Properties.Settings.Default.ii_int_cj_snLifts_Out[_int_Sequence], out _int_cj_snLifts_Out);

                bool.TryParse(value: Properties.Settings.Default.ii_bool_Beep[_int_Sequence], out _bool_Beep);
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


            Initialise_datatables();

            bool bool_AutoVals;

            bool_AutoVals = true;
            if (dt_default_snatch_extras != null)
            {
                if (dt_default_snatch_extras.Rows.Count > 0)
                {
                    dt_snatch_extras = dt_default_snatch_extras.Copy();
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                Insert_Auto_snatch_Extras(dt_snatch_extras);
            }
            snatch_Populate_Extras();

            bool_AutoVals = true;
            if (dt_default_snatch_jumps != null)
            {
                if (dt_default_snatch_jumps.Rows.Count > 0)
                {
                    dt_snatch_jumps = dt_default_snatch_jumps.Copy();
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                Insert_Default_snatch_Jumps(dt_snatch_jumps);
            }
            snatch_Populate_Jumps();

            bool_AutoVals = true;
            if (dt_default_snatch_times != null)
            {
                if (dt_default_snatch_times.Rows.Count > 0)
                {
                    dt_snatch_times = dt_default_snatch_times.Copy();
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                Insert_Default_snatch_Times(dt_snatch_times);
            }
            snatch_Populate_Times();

            snatch_Populate_Steps(boolPreserveLifts: false);

            bool_AutoVals = true;
            if (dt_default_cj_extras != null)
            {
                if (dt_default_cj_extras.Rows.Count > 0)
                {
                    dt_cj_extras = dt_default_cj_extras.Copy();
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                Insert_Auto_cj_Extras(dt_cj_extras);
            }
            cj_Populate_Extras();

            bool_AutoVals = true;
            if (dt_default_cj_jumps != null)
            {
                if (dt_default_cj_jumps.Rows.Count > 0)
                {
                    dt_cj_jumps = dt_default_cj_jumps.Copy();
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                Insert_Default_cj_Jumps(dt_cj_jumps);
            }
            cj_Populate_Jumps();

            bool_AutoVals = true;
            if (dt_default_cj_times != null)
            {
                if (dt_default_cj_times.Rows.Count > 0)
                {
                    dt_cj_times = dt_default_cj_times.Copy();
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                Insert_Default_cj_Times(dt_cj_times);
            }
            cj_Populate_Times();

            cj_Populate_Steps(boolPreserveLifts: false);

            Check_DataTables();
            Snatch_Opener_Set();
            CJ_Opener_Set();

            bool_Loading = _bool_Loading;
        }
        private int int_Profile_Sequence(int _int_ProfileId)
        {
            if (_int_ProfileId > -1 && Properties.Settings.Default.ii_int_ProfileIds != null)
            {
                if (Properties.Settings.Default.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    return Properties.Settings.Default.ii_int_ProfileIds.IndexOf(_int_ProfileId.ToString());
                }
            }
            return -1;
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void buttonRestore_Click(object sender, EventArgs e)
        {
            buttonRestore.Dispose();
            buttonClose.Dispose();
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
        }
        private void Form_WL_Comp_Warmup_Load(object sender, EventArgs e)
        {
            Bounds = Screen.PrimaryScreen.Bounds;
            Initialise_Form();
        }
        private void Initialise_Form()
        {
            bool_Loading = true;
            Populate_MenuStrip();

            Load_Profile_Values_To_Controls();

            buttonClose.BringToFront();
            bool_Loading = false;
        }
        private void Clean_Settings()
        {
            Print_All_Settings();
            // ensures the string lists have the same number of entries as the profile id list
            int _int_Profile_Count = 0;
            if (Properties.Settings.Default.ii_int_ProfileIds != null)
            {
                _int_Profile_Count = Properties.Settings.Default.ii_int_ProfileIds.Count;
            }
            if (_int_Profile_Count == 0)
            {
                Properties.Settings.Default.ii_int_ProfileIds = new();
                Properties.Settings.Default.ii_string_ProfileName = new();
                Properties.Settings.Default.ii_int_Barbell = new();
                Properties.Settings.Default.ii_HHmm_StartTimes = new();
                Properties.Settings.Default.ii_int_snatch_Sec_Stage = new();
                Properties.Settings.Default.ii_int_snatch_Wgt_Opener = new();
                Properties.Settings.Default.ii_bool_snatch_OpenerWarmup = new();
                Properties.Settings.Default.ii_int_snatch_Sec_End = new();
                Properties.Settings.Default.ii_int_snatch_Lifts_Out = new();
                Properties.Settings.Default.ii_int_cj_Sec_Stage = new();
                Properties.Settings.Default.ii_int_cj_Sec_Break = new();
                Properties.Settings.Default.ii_int_cj_Wgt_Opener = new();
                Properties.Settings.Default.ii_bool_cj_OpenerWarmup = new();
                Properties.Settings.Default.ii_int_cj_Sec_End = new();
                Properties.Settings.Default.ii_int_cj_Lifts_Out = new();
                Properties.Settings.Default.ii_int_cj_snLifts_Out = new();
                Properties.Settings.Default.ii_bool_Beep = new();
                Properties.Settings.Default.ii_strings_snatch_Extras = new();
                Properties.Settings.Default.ii_strings_snatch_Jumps = new();
                Properties.Settings.Default.ii_strings_snatch_Times = new();
                Properties.Settings.Default.ii_strings_cj_Extras = new();
                Properties.Settings.Default.ii_strings_cj_Jumps = new();
                Properties.Settings.Default.ii_strings_cj_Times = new();
            }
            else
            {
                List<string> _strings = new();
                if (Properties.Settings.Default.ii_string_ProfileName != null)
                {
                    _strings = Properties.Settings.Default.ii_string_ProfileName;
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
                    Properties.Settings.Default.ii_string_ProfileName = _strings;
                }
                
                _strings = new();
                if (Properties.Settings.Default.ii_int_Barbell != null)
                {
                    _strings = Properties.Settings.Default.ii_int_Barbell;
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
                    Properties.Settings.Default.ii_int_Barbell = _strings;
                }
                
                _strings = new();
                if (Properties.Settings.Default.ii_HHmm_StartTimes != null)
                {
                    _strings = Properties.Settings.Default.ii_HHmm_StartTimes;
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
                    Properties.Settings.Default.ii_HHmm_StartTimes = _strings;
                }
                
                _strings = new();
                if (Properties.Settings.Default.ii_int_snatch_Sec_Stage != null)
                {
                    _strings = Properties.Settings.Default.ii_int_snatch_Sec_Stage;
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
                    Properties.Settings.Default.ii_int_snatch_Sec_Stage = _strings;
                }
                
                _strings = new();
                if (Properties.Settings.Default.ii_int_snatch_Wgt_Opener != null)
                {
                    _strings = Properties.Settings.Default.ii_int_snatch_Wgt_Opener;
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
                    Properties.Settings.Default.ii_int_snatch_Wgt_Opener = _strings;
                }
                
                _strings = new();
                if (Properties.Settings.Default.ii_bool_snatch_OpenerWarmup != null)
                {
                    _strings = Properties.Settings.Default.ii_bool_snatch_OpenerWarmup;
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
                    Properties.Settings.Default.ii_bool_snatch_OpenerWarmup = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_int_snatch_Sec_End != null)
                {
                    _strings = Properties.Settings.Default.ii_int_snatch_Sec_End;
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
                    Properties.Settings.Default.ii_int_snatch_Sec_End = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_int_snatch_Lifts_Out != null)
                {
                    _strings = Properties.Settings.Default.ii_int_snatch_Lifts_Out;
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
                    Properties.Settings.Default.ii_int_snatch_Lifts_Out = _strings;
                }


                _strings = new();
                if (Properties.Settings.Default.ii_int_cj_Sec_Stage != null)
                {
                    _strings = Properties.Settings.Default.ii_int_cj_Sec_Stage;
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
                    Properties.Settings.Default.ii_int_cj_Sec_Stage = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_int_cj_Sec_Break != null)
                {
                    _strings = Properties.Settings.Default.ii_int_cj_Sec_Break;
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
                    Properties.Settings.Default.ii_int_cj_Sec_Break = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_int_cj_Wgt_Opener != null)
                {
                    _strings = Properties.Settings.Default.ii_int_cj_Wgt_Opener;
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
                    Properties.Settings.Default.ii_int_cj_Wgt_Opener = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_bool_cj_OpenerWarmup != null)
                {
                    _strings = Properties.Settings.Default.ii_bool_cj_OpenerWarmup;
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
                    Properties.Settings.Default.ii_bool_cj_OpenerWarmup = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_int_cj_Sec_End != null)
                {
                    _strings = Properties.Settings.Default.ii_int_cj_Sec_End;
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
                    Properties.Settings.Default.ii_int_cj_Sec_End = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_int_cj_Lifts_Out != null)
                {
                    _strings = Properties.Settings.Default.ii_int_cj_Lifts_Out;
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
                    Properties.Settings.Default.ii_int_cj_Lifts_Out = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_int_cj_snLifts_Out != null)
                {
                    _strings = Properties.Settings.Default.ii_int_cj_snLifts_Out;
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
                    Properties.Settings.Default.ii_int_cj_snLifts_Out = _strings;
                }

                _strings = new();
                if (Properties.Settings.Default.ii_bool_Beep != null)
                {
                    _strings = Properties.Settings.Default.ii_bool_Beep;
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
                    Properties.Settings.Default.ii_bool_Beep = _strings;
                }
            }
            Print_All_Settings();
        }
        private void Form_WL_Comp_Warmup_FormClosing(object sender, FormClosingEventArgs e)
        {
            Update_Settings();
            Settings_Changes_Save();
        }
        private int Add_Profile(string _str_ProfileName)
        {
            Clean_Settings();
            int _int_ProfileId = 1;
            foreach (string s in Properties.Settings.Default.ii_int_ProfileIds)
            {
                if (int.TryParse(s: s, result: out int _i) &&
                    _i >= _int_ProfileId)
                {
                    _int_ProfileId = _i + 1;
                }
            }
            Properties.Settings.Default.ii_int_ProfileIds.Add(_int_ProfileId.ToString());
            Properties.Settings.Default.ii_string_ProfileName.Add(_str_ProfileName);
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
                Properties.Settings.Default.ii_int_ProfileIds.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_string_ProfileName.RemoveAt(_int_Sequence);

                Properties.Settings.Default.ii_int_Barbell.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_HHmm_StartTimes.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_snatch_Sec_Stage.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_snatch_Wgt_Opener.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_bool_snatch_OpenerWarmup.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_snatch_Sec_End.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_snatch_Lifts_Out.RemoveAt(_int_Sequence);

                Properties.Settings.Default.ii_int_cj_Sec_Stage.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_cj_Wgt_Opener.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_cj_Sec_Break.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_bool_cj_OpenerWarmup.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_cj_Sec_End.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_cj_Lifts_Out.RemoveAt(_int_Sequence);
                Properties.Settings.Default.ii_int_cj_snLifts_Out.RemoveAt(_int_Sequence);

                for (int i = Properties.Settings.Default.ii_strings_snatch_Extras.Count - 1; i >= 0; i--)
                {
                    Parse_string_extras(
                        _string: Properties.Settings.Default.ii_strings_snatch_Extras[i],
                        _int_ProfileId: out int __int_ProfileId,
                        _int_Id: out _,
                        _int_Order: out _,
                        _int_Length: out _,
                        _str_Action: out _);
                    if (_int_ProfileId == __int_ProfileId)
                    {
                        Properties.Settings.Default.ii_strings_snatch_Extras.RemoveAt(i);
                    }
                }
                for (int i = Properties.Settings.Default.ii_strings_snatch_Jumps.Count - 1; i >= 0; i--)
                {
                    Parse_string_jumps(
                        _string: Properties.Settings.Default.ii_strings_snatch_Jumps[i],
                        _int_ProfileId: out int __int_ProfileId,
                        _int_Id: out _,
                        _int_FromWeight: out _,
                        _int_Jump: out _);
                    if (_int_ProfileId == __int_ProfileId)
                    {
                        Properties.Settings.Default.ii_strings_snatch_Jumps.RemoveAt(i);
                    }
                }
                for (int i = Properties.Settings.Default.ii_strings_snatch_Times.Count - 1; i >= 0; i--)
                {
                    Parse_string_times(
                        _string: Properties.Settings.Default.ii_strings_snatch_Times[i],
                        _int_ProfileId: out int __int_ProfileId,
                        _int_Id: out _,
                        _int_FromWeight: out _,
                        _int_Time: out _);
                    if (_int_ProfileId == __int_ProfileId)
                    {
                        Properties.Settings.Default.ii_strings_snatch_Times.RemoveAt(i);
                    }
                }

                for (int i = Properties.Settings.Default.ii_strings_cj_Extras.Count - 1; i >= 0; i--)
                {
                    Parse_string_extras(
                        _string: Properties.Settings.Default.ii_strings_cj_Extras[i],
                        _int_ProfileId: out int __int_ProfileId,
                        _int_Id: out _,
                        _int_Order: out _,
                        _int_Length: out _,
                        _str_Action: out _);
                    if (_int_ProfileId == __int_ProfileId)
                    {
                        Properties.Settings.Default.ii_strings_cj_Extras.RemoveAt(i);
                    }
                }
                for (int i = Properties.Settings.Default.ii_strings_cj_Jumps.Count - 1; i >= 0; i--)
                {
                    Parse_string_jumps(
                        _string: Properties.Settings.Default.ii_strings_cj_Jumps[i],
                        _int_ProfileId: out int __int_ProfileId,
                        _int_Id: out _,
                        _int_FromWeight: out _,
                        _int_Jump: out _);
                    if (_int_ProfileId == __int_ProfileId)
                    {
                        Properties.Settings.Default.ii_strings_cj_Jumps.RemoveAt(i);
                    }
                }
                for (int i = Properties.Settings.Default.ii_strings_cj_Times.Count - 1; i >= 0; i--)
                {
                    Parse_string_times(
                        _string: Properties.Settings.Default.ii_strings_cj_Times[i],
                        _int_ProfileId: out int __int_ProfileId,
                        _int_Id: out _,
                        _int_FromWeight: out _,
                        _int_Time: out _);
                    if (_int_ProfileId == __int_ProfileId)
                    {
                        Properties.Settings.Default.ii_strings_cj_Times.RemoveAt(i);
                    }
                }
            }
            if (Properties.Settings.Default.int_ProfileId == _int_ProfileId)
            {
                Properties.Settings.Default.int_ProfileId = -1;
            }
        }
        private string string_Profile_Name_From_Id(int _int_ProfileId)
        {
            if (Properties.Settings.Default.ii_string_ProfileName != null &&
                Properties.Settings.Default.ii_string_ProfileName.Count > 0)
            {
                int _int_Sequence = int_Profile_Sequence(_int_ProfileId: _int_ProfileId);
                return Properties.Settings.Default.ii_string_ProfileName[_int_Sequence];
            }
            return "default";
        }
        private void Settings_Changes_Save()
        {
            Properties.Settings.Default.Save();
            Print_All_Settings();
        }
        private void Update_Settings()
        {
            Print_All_Settings();
            Properties.Settings.Default.int_ProfileId = int_ProfileId;
            Clean_Settings();
            int _int_Sequence = int_Profile_Sequence(_int_ProfileId: int_ProfileId);
            if (_int_Sequence < 0)
            {
                if (Properties.Settings.Default.ii_int_ProfileIds == null)
                {
                    Properties.Settings.Default.ii_int_ProfileIds = new()
                    {
                        int_ProfileId.ToString()
                    };
                }
                else
                {
                    Properties.Settings.Default.ii_int_ProfileIds.Add(int_ProfileId.ToString());
                }
                Clean_Settings();
            }
            Properties.Settings.Default.ii_int_Barbell[_int_Sequence] = int_Barbell.ToString();
            Properties.Settings.Default.ii_HHmm_StartTimes[_int_Sequence] = dateTimePicker_snatch_Start.Value.ToString("HHmm");
            Properties.Settings.Default.ii_int_snatch_Sec_Stage[_int_Sequence] = int_snatch_Sec_Stage.ToString();
            Properties.Settings.Default.ii_int_snatch_Wgt_Opener[_int_Sequence] = int_snatch_Wgt_Opener.ToString();
            Properties.Settings.Default.ii_bool_snatch_OpenerWarmup[_int_Sequence] = bool_snatch_OpenerWarmup.ToString();
            Properties.Settings.Default.ii_int_snatch_Sec_End[_int_Sequence] = int_snatch_Sec_End.ToString();
            Properties.Settings.Default.ii_int_snatch_Lifts_Out[_int_Sequence] = int_snatch_Lifts_Out.ToString();

            Properties.Settings.Default.ii_int_cj_Sec_Stage[_int_Sequence] = int_cj_Sec_Stage.ToString();
            Properties.Settings.Default.ii_int_cj_Wgt_Opener[_int_Sequence] = int_cj_Wgt_Opener.ToString();
            Properties.Settings.Default.ii_int_cj_Sec_Break[_int_Sequence] = int_cj_Sec_Break.ToString();
            Properties.Settings.Default.ii_bool_cj_OpenerWarmup[_int_Sequence] = bool_cj_OpenerWarmup.ToString();
            Properties.Settings.Default.ii_int_cj_Sec_End[_int_Sequence] = int_cj_Sec_End.ToString();
            Properties.Settings.Default.ii_int_cj_Lifts_Out[_int_Sequence] = int_cj_Lifts_Out.ToString();
            Properties.Settings.Default.ii_int_cj_snLifts_Out[_int_Sequence] = int_cj_snLifts_Out.ToString();

            Properties.Settings.Default.ii_bool_Beep[_int_Sequence] = bool_Beep.ToString();

            for (int i = Properties.Settings.Default.ii_strings_snatch_Extras.Count - 1; i >= 0; i--)
            {
                Parse_string_extras(
                    _string: Properties.Settings.Default.ii_strings_snatch_Extras[i],
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_Order: out _,
                    _int_Length: out _,
                    _str_Action: out _);
                if (_int_ProfileId == int_ProfileId ||
                    !Properties.Settings.Default.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    Properties.Settings.Default.ii_strings_snatch_Extras.RemoveAt(i);
                }
            }
            for (int i = Properties.Settings.Default.ii_strings_snatch_Jumps.Count - 1; i >= 0; i--)
            {
                Parse_string_jumps(
                    _string: Properties.Settings.Default.ii_strings_snatch_Jumps[i],
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out _,
                    _int_Jump: out _);
                if (_int_ProfileId == int_ProfileId ||
                    !Properties.Settings.Default.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    Properties.Settings.Default.ii_strings_snatch_Jumps.RemoveAt(i);
                }
            }
            for (int i = Properties.Settings.Default.ii_strings_snatch_Times.Count - 1; i >= 0; i--)
            {
                Parse_string_times(
                    _string: Properties.Settings.Default.ii_strings_snatch_Times[i],
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out _,
                    _int_Time: out _);
                if (_int_ProfileId == int_ProfileId ||
                    !Properties.Settings.Default.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    Properties.Settings.Default.ii_strings_snatch_Times.RemoveAt(i);
                }
            }

            for (int i = Properties.Settings.Default.ii_strings_cj_Extras.Count - 1; i >= 0; i--)
            {
                Parse_string_extras(
                    _string: Properties.Settings.Default.ii_strings_cj_Extras[i],
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_Order: out _,
                    _int_Length: out _,
                    _str_Action: out _);
                if (_int_ProfileId == int_ProfileId ||
                    !Properties.Settings.Default.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    Properties.Settings.Default.ii_strings_cj_Extras.RemoveAt(i);
                }
            }
            for (int i = Properties.Settings.Default.ii_strings_cj_Jumps.Count - 1; i >= 0; i--)
            {
                Parse_string_jumps(
                    _string: Properties.Settings.Default.ii_strings_cj_Jumps[i],
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out _,
                    _int_Jump: out _);
                if (_int_ProfileId == int_ProfileId ||
                    !Properties.Settings.Default.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    Properties.Settings.Default.ii_strings_cj_Jumps.RemoveAt(i);
                }
            }
            for (int i = Properties.Settings.Default.ii_strings_cj_Times.Count - 1; i >= 0; i--)
            {
                Parse_string_times(
                    _string: Properties.Settings.Default.ii_strings_cj_Times[i],
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out _,
                    _int_Time: out _);
                if (_int_ProfileId == int_ProfileId ||
                    !Properties.Settings.Default.ii_int_ProfileIds.Contains(_int_ProfileId.ToString()))
                {
                    Properties.Settings.Default.ii_strings_cj_Times.RemoveAt(i);
                }
            }

            dt_snatch_extras.DefaultView.Sort = str_col_Id + " ASC";
            foreach (DataRow dR in dt_snatch_extras.DefaultView.ToTable().Rows)
            {
                Properties.Settings.Default.ii_strings_snatch_Extras.Add(
                    int_ProfileId.ToString("000") +
                    dR.Field<int>(str_col_Id).ToString("000000") +
                    dR.Field<int>(str_col_Order).ToString("000") +
                    dR.Field<int>(str_col_Length).ToString("00000") +
                    dR.Field<string>(str_col_Action));
            }

            dt_snatch_jumps.DefaultView.Sort = str_col_Id + " ASC";
            foreach (DataRow dR in dt_snatch_jumps.DefaultView.ToTable().Rows)
            {
                Properties.Settings.Default.ii_strings_snatch_Jumps.Add(
                    int_ProfileId.ToString("000") +
                    dR.Field<int>(str_col_Id).ToString("000000") +
                    dR.Field<int>(str_col_FromWeight).ToString("000") +
                    dR.Field<int>(str_col_Jump).ToString("000"));
            }

            dt_snatch_times.DefaultView.Sort = str_col_Id + " ASC";
            foreach (DataRow dR in dt_snatch_times.DefaultView.ToTable().Rows)
            {
                Properties.Settings.Default.ii_strings_snatch_Times.Add(
                    int_ProfileId.ToString("000") +
                    dR.Field<int>(str_col_Id).ToString("000000") +
                    dR.Field<int>(str_col_FromWeight).ToString("000") +
                    dR.Field<int>(str_col_Length).ToString("000"));
            }

            dt_cj_extras.DefaultView.Sort = str_col_Id + " ASC";
            foreach (DataRow dR in dt_cj_extras.DefaultView.ToTable().Rows)
            {
                Properties.Settings.Default.ii_strings_cj_Extras.Add(
                    int_ProfileId.ToString("000") +
                    dR.Field<int>(str_col_Id).ToString("000000") +
                    dR.Field<int>(str_col_Order).ToString("000") +
                    dR.Field<int>(str_col_Length).ToString("00000") +
                    dR.Field<string>(str_col_Action));
            }

            dt_cj_jumps.DefaultView.Sort = str_col_Id + " ASC";
            foreach (DataRow dR in dt_cj_jumps.DefaultView.ToTable().Rows)
            {
                Properties.Settings.Default.ii_strings_cj_Jumps.Add(
                    int_ProfileId.ToString("000") +
                    dR.Field<int>(str_col_Id).ToString("000000") +
                    dR.Field<int>(str_col_FromWeight).ToString("000") +
                    dR.Field<int>(str_col_Jump).ToString("000"));
            }

            dt_cj_times.DefaultView.Sort = str_col_Id + " ASC";
            foreach (DataRow dR in dt_cj_times.DefaultView.ToTable().Rows)
            {
                Properties.Settings.Default.ii_strings_cj_Times.Add(
                    int_ProfileId.ToString("000") +
                    dR.Field<int>(str_col_Id).ToString("000000") +
                    dR.Field<int>(str_col_FromWeight).ToString("000") +
                    dR.Field<int>(str_col_Length).ToString("000"));
            }
            Print_All_Settings();
        }
        private void Parse_string_extras(
            string _string,
            out int _int_ProfileId,
            out int _int_Id,
            out int _int_Order,
            out int _int_Length,
            out string _str_Action)
        {
            //  at character:
            //  0   3 digit profile id
            //  3   6 digit id
            //  9   3 digit order
            //  12  5 digit length
            //  17  variable length string (action name)
            _int_ProfileId = default;
            _int_Id = default;
            _int_Order = default;
            _int_Length = default;
            _str_Action = string.Empty;
            try
            {
                _int_ProfileId = int.Parse(_string.Substring(0, 3));
                _int_Id = int.Parse(_string.Substring(3, 6));
                _int_Order = int.Parse(_string.Substring(9, 3));
                _int_Length = int.Parse(_string.Substring(12, 5));
                _str_Action = _string.Substring(17);
            }
            catch { }
        }
        private void Parse_string_jumps(
            string _string,
            out int _int_ProfileId,
            out int _int_Id,
            out int _int_FromWeight,
            out int _int_Jump)
        {
            //  at character:
            //  0   3 digit profile id
            //  3   6 digit id
            //  9   3 digit from weight
            //  12  3 digit jump
            _int_ProfileId = default;
            _int_Id = default;
            _int_FromWeight = default;
            _int_Jump = default;
            try
            {
                _int_ProfileId = int.Parse(_string.Substring(0, 3));
                _int_Id = int.Parse(_string.Substring(3, 6));
                _int_FromWeight = int.Parse(_string.Substring(9, 3));
                _int_Jump = int.Parse(_string.Substring(12, 3));
            }
            catch { }
        }
        private void Parse_string_times(
            string _string,
            out int _int_ProfileId,
            out int _int_Id,
            out int _int_FromWeight,
            out int _int_Time)
        {
            //  at character:
            //  0   3 digit profile id
            //  3   6 digit id
            //  9   3 digit from weight
            //  12  3 digit time
            _int_ProfileId = default;
            _int_Id = default;
            _int_FromWeight = default;
            _int_Time = default;
            try
            {
                _int_ProfileId = int.Parse(_string.Substring(0, 3));
                _int_Id = int.Parse(_string.Substring(3, 6));
                _int_FromWeight = int.Parse(_string.Substring(9, 3));
                _int_Time = int.Parse(_string.Substring(12, 3));
            }
            catch { }
        }
        private void Initialise_datatables()
        {
            dt_snatch_extras = new DataTable();
            dt_snatch_extras.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_Action, DataType = typeof(string) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Order, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });

            dt_snatch_jumps = new DataTable();
            dt_snatch_jumps.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Jump, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });

            dt_snatch_times = new DataTable();
            dt_snatch_times.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });

            dt_cj_extras = new DataTable();
            dt_cj_extras.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_Action, DataType = typeof(string) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Order, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });

            dt_cj_jumps = new DataTable();
            dt_cj_jumps.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Jump, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });

            dt_cj_times = new DataTable();
            dt_cj_times.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });
        }
        private void Check_DataTables()
        {
            if (dt_snatch_extras == null |
                dt_snatch_jumps == null |
                dt_snatch_times == null |
                dt_cj_extras == null |
                dt_cj_jumps == null |
                dt_cj_times == null)
            {
                Initialise_datatables(); 
            }

            bool boolHas1;

            boolHas1 = false;
            foreach (DataRow dataRow in dt_snatch_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == 1)
                {
                    boolHas1 = true;
                    break;
                }
            }
            if (!boolHas1)
            {
                DataRow dataRow = dt_snatch_jumps.NewRow();
                dataRow[str_col_FromWeight] = 1;
                dataRow[str_col_Jump] = 1;
                dt_snatch_jumps.Rows.Add(dataRow);
                dt_snatch_jumps.AcceptChanges();
            }

            boolHas1 = false;
            foreach (DataRow dataRow in dt_snatch_times.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == 1)
                {
                    boolHas1 = true;
                    break;
                }
            }
            if (!boolHas1)
            {
                DataRow dataRow = dt_snatch_times.NewRow();
                dataRow[str_col_FromWeight] = 1;
                dataRow[str_col_Jump] = 1;
                dt_snatch_times.Rows.Add(dataRow);
                dt_snatch_times.AcceptChanges();
            }

            boolHas1 = false;
            foreach (DataRow dataRow in dt_cj_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == 1)
                {
                    boolHas1 = true;
                    break;
                }
            }
            if (!boolHas1)
            {
                DataRow dataRow = dt_cj_jumps.NewRow();
                dataRow[str_col_FromWeight] = 1;
                dataRow[str_col_Jump] = 1;
                dt_cj_jumps.Rows.Add(dataRow);
                dt_cj_jumps.AcceptChanges();
            }

            boolHas1 = false;
            foreach (DataRow dataRow in dt_cj_times.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == 1)
                {
                    boolHas1 = true;
                    break;
                }
            }
            if (!boolHas1)
            {
                DataRow dataRow = dt_cj_times.NewRow();
                dataRow[str_col_FromWeight] = 1;
                dataRow[str_col_Jump] = 1;
                dt_cj_times.Rows.Add(dataRow);
                dt_cj_times.AcceptChanges();
            }
        }
        private void Get_Settings_Defaults_Lists()
        {
            dt_default_snatch_extras = new();
            dt_default_snatch_extras.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_Action, DataType = typeof(string) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Order, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });
            dt_default_snatch_jumps = new();
            dt_default_snatch_jumps.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Jump, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });
            dt_default_snatch_times = new();
            dt_default_snatch_times.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });

            dt_default_cj_extras = new();
            dt_default_cj_extras.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_Action, DataType = typeof(string) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Order, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });
            dt_default_cj_jumps = new();
            dt_default_cj_jumps.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Jump, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });
            dt_default_cj_times = new();
            dt_default_cj_times.Columns.AddRange(
                new DataColumn[] {
                    new DataColumn { ColumnName = str_col_FromWeight, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Length, DataType = typeof(int) },
                    new DataColumn { ColumnName = str_col_Id, DataType = typeof(int), AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 },
                });
            Clean_Settings();
            foreach (string s in Properties.Settings.Default.ii_strings_snatch_Extras)
            {
                Parse_string_extras(
                    _string: s,
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_Order: out int _int_Order,
                    _int_Length: out int _int_Length,
                    _str_Action: out string _str_Action);
                if (_int_ProfileId == int_ProfileId)
                {
                    dt_default_snatch_extras.Rows.Add(new object[]
                    {
                        _str_Action,
                        _int_Length,
                        _int_Order
                    });
                }
            }
            dt_default_snatch_extras.AcceptChanges();
            if (dt_default_snatch_extras.Rows.Count == 0)
            {
                Insert_Auto_snatch_Extras(dt_default_snatch_extras);
            }

            foreach (string s in Properties.Settings.Default.ii_strings_snatch_Jumps)
            {
                Parse_string_jumps(
                    _string: s,
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out int _int_FromWeight,
                    _int_Jump: out int _int_Jump);
                if (_int_ProfileId == int_ProfileId)
                {
                    dt_default_snatch_jumps.Rows.Add(new object[]
                    {
                        _int_FromWeight,
                        _int_Jump,
                    });
                }
            }
            dt_default_snatch_jumps.AcceptChanges();
            if (dt_default_snatch_jumps.Rows.Count == 0)
            {
                Insert_Default_snatch_Jumps(dt_default_snatch_jumps);
            }

            foreach (string s in Properties.Settings.Default.ii_strings_snatch_Times)
            {
                Parse_string_times(
                    _string: s,
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out int _int_FromWeight,
                    _int_Time: out int _int_Time);
                if (_int_ProfileId == int_ProfileId)
                {
                    dt_default_snatch_times.Rows.Add(new object[]
                    {
                        _int_FromWeight,
                        _int_Time,
                    });
                }
            }
            dt_default_snatch_times.AcceptChanges();
            if (dt_default_snatch_times.Rows.Count == 0)
            {
                Insert_Default_snatch_Times(dt_default_snatch_times);
            }


            foreach (string s in Properties.Settings.Default.ii_strings_cj_Extras)
            {
                Parse_string_extras(
                    _string: s,
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_Order: out int _int_Order,
                    _int_Length: out int _int_Length,
                    _str_Action: out string _str_Action);
                if (_int_ProfileId == int_ProfileId)
                {
                    dt_default_cj_extras.Rows.Add(new object[]
                    {
                        _str_Action,
                        _int_Length,
                        _int_Order
                    });
                }
            }
            dt_default_cj_extras.AcceptChanges();
            if (dt_default_cj_extras.Rows.Count == 0)
            {
                Insert_Auto_cj_Extras(dt_default_cj_extras);
            }

            foreach (string s in Properties.Settings.Default.ii_strings_cj_Jumps)
            {
                Parse_string_jumps(
                    _string: s,
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out int _int_FromWeight,
                    _int_Jump: out int _int_Jump);
                if (_int_ProfileId == int_ProfileId)
                {
                    dt_default_cj_jumps.Rows.Add(new object[]
                    {
                        _int_FromWeight,
                        _int_Jump,
                    });
                }
            }
            dt_default_cj_jumps.AcceptChanges();
            if (dt_default_cj_jumps.Rows.Count == 0)
            {
                Insert_Default_cj_Jumps(dt_default_cj_jumps);
            }

            foreach (string s in Properties.Settings.Default.ii_strings_cj_Times)
            {
                Parse_string_times(
                    _string: s,
                    _int_ProfileId: out int _int_ProfileId,
                    _int_Id: out _,
                    _int_FromWeight: out int _int_FromWeight,
                    _int_Time: out int _int_Time);
                if (_int_ProfileId == int_ProfileId)
                {
                    dt_default_cj_times.Rows.Add(new object[]
                    {
                        _int_FromWeight,
                        _int_Time,
                    });
                }
            }
            dt_default_cj_times.AcceptChanges();
            if (dt_default_cj_times.Rows.Count == 0)
            {
                Insert_Default_cj_Times(dt_default_cj_times);
            }
        }
        private void numericUpDown_snatch_weight_barbell_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            int _int_Barbell;
            try
            {
                _int_Barbell = (int)(numericUpDown_snatch_weight_barbell.Value);
            }
            catch
            {
                (numericUpDown_snatch_weight_barbell).BackColor = Color.Yellow;
                return;
            }

            if (_int_Barbell < 1) 
            {
                (numericUpDown_snatch_weight_barbell).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_snatch_weight_barbell).BackColor = Color.White;

            int_Barbell = _int_Barbell;

            numericUpDown_snatch_weight_opener.Minimum = _int_Barbell;
            numericUpDown_cj_weight_opener.Minimum = _int_Barbell;

            Snatch_Opener_Set();
            CJ_Opener_Set();
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            snatch_Stop_Live();
            cj_Stop_Live();
        }
        private void button_snatch_ClearSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"This will erase all profiles and restore all defaults." +
                Environment.NewLine + Environment.NewLine + "Continue?",
                "Reset settings?",
                buttons:MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Properties.Settings.Default.Reset();
                Settings_Changes_Save();
                int_ProfileId = Add_Profile(_str_ProfileName: "default");
                Initialise_Form();
            }
        }
        private void Print_All_Settings()
        {
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("int_ProfileId = " + Properties.Settings.Default.int_ProfileId.ToString());
            Console.WriteLine("ii_int_ProfileIds = " + (Properties.Settings.Default.ii_int_ProfileIds == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_ProfileIds)));
            Console.WriteLine("ii_string_ProfileName = " + (Properties.Settings.Default.ii_string_ProfileName == null ? "null" : string.Join("|", Properties.Settings.Default.ii_string_ProfileName)));
            Console.WriteLine("ii_int_Barbell = " + (Properties.Settings.Default.ii_int_Barbell == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_Barbell)));
            Console.WriteLine("ii_HHmm_StartTimes = " + (Properties.Settings.Default.ii_HHmm_StartTimes == null ? "null" : string.Join("|", Properties.Settings.Default.ii_HHmm_StartTimes)));

            Console.WriteLine("ii_int_snatch_Sec_Stage = " + (Properties.Settings.Default.ii_int_snatch_Sec_Stage == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_snatch_Sec_Stage)));
            Console.WriteLine("ii_int_snatch_Wgt_Opener = " + (Properties.Settings.Default.ii_int_snatch_Wgt_Opener == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_snatch_Wgt_Opener)));
            Console.WriteLine("ii_int_snatch_Sec_End = " + (Properties.Settings.Default.ii_int_snatch_Sec_End == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_snatch_Sec_End)));
            Console.WriteLine("ii_int_snatch_Lifts_Out = " + (Properties.Settings.Default.ii_int_snatch_Lifts_Out == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_snatch_Lifts_Out)));
            Console.WriteLine("ii_bool_snatch_OpenerWarmup = " + (Properties.Settings.Default.ii_bool_snatch_OpenerWarmup == null ? "null" : string.Join("|", Properties.Settings.Default.ii_bool_snatch_OpenerWarmup)));
            Console.WriteLine("ii_strings_snatch_Extras = " + (Properties.Settings.Default.ii_strings_snatch_Extras == null ? "null" : string.Join("|", Properties.Settings.Default.ii_strings_snatch_Extras)));
            Console.WriteLine("ii_strings_snatch_Jumps = " + (Properties.Settings.Default.ii_strings_snatch_Jumps == null ? "null" : string.Join("|", Properties.Settings.Default.ii_strings_snatch_Jumps)));
            Console.WriteLine("ii_strings_snatch_Times = " + (Properties.Settings.Default.ii_strings_snatch_Times == null ? "null" : string.Join("|", Properties.Settings.Default.ii_strings_snatch_Times)));

            Console.WriteLine("ii_int_cj_Sec_Stage = " + (Properties.Settings.Default.ii_int_cj_Sec_Stage == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_cj_Sec_Stage)));
            Console.WriteLine("ii_int_cj_Wgt_Opener = " + (Properties.Settings.Default.ii_int_cj_Wgt_Opener == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_cj_Wgt_Opener)));
            Console.WriteLine("ii_int_cj_Sec_End = " + (Properties.Settings.Default.ii_int_cj_Sec_End == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_cj_Sec_End)));
            Console.WriteLine("ii_int_cj_Lifts_Out = " + (Properties.Settings.Default.ii_int_cj_Lifts_Out == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_cj_Lifts_Out)));
            Console.WriteLine("ii_int_cj_Sec_Break = " + (Properties.Settings.Default.ii_int_cj_Sec_Break == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_cj_Sec_Break)));
            Console.WriteLine("ii_int_cj_snLifts_Out = " + (Properties.Settings.Default.ii_int_cj_snLifts_Out == null ? "null" : string.Join("|", Properties.Settings.Default.ii_int_cj_snLifts_Out)));
            Console.WriteLine("ii_bool_cj_OpenerWarmup = " + (Properties.Settings.Default.ii_bool_cj_OpenerWarmup == null ? "null" : string.Join("|", Properties.Settings.Default.ii_bool_cj_OpenerWarmup)));
            Console.WriteLine("ii_strings_cj_Extras = " + (Properties.Settings.Default.ii_strings_cj_Extras == null ? "null" : string.Join("|", Properties.Settings.Default.ii_strings_cj_Extras)));
            Console.WriteLine("ii_strings_cj_Jumps = " + (Properties.Settings.Default.ii_strings_cj_Jumps == null ? "null" : string.Join("|", Properties.Settings.Default.ii_strings_cj_Jumps)));
            Console.WriteLine("ii_strings_cj_Times = " + (Properties.Settings.Default.ii_strings_cj_Times == null ? "null" : string.Join("|", Properties.Settings.Default.ii_strings_cj_Times)));

            Console.WriteLine("ii_bool_Beep = " + (Properties.Settings.Default.ii_bool_Beep == null ? "null" : string.Join("|", Properties.Settings.Default.ii_bool_Beep)));
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
            for (int i = 0; i < Properties.Settings.Default.ii_int_ProfileIds.Count; i++)
            {
                if (int.TryParse(s: Properties.Settings.Default.ii_int_ProfileIds[i], result: out int _int_ProfileId) &&
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
                        foreach (string s in Properties.Settings.Default.ii_int_ProfileIds)
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
                        Properties.Settings.Default.ii_string_ProfileName[_int_Sequence] = _str_Name;
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
                            Properties.Settings.Default.ii_int_Barbell[_int_New_Sequence] = Properties.Settings.Default.ii_int_Barbell[_int_Sequence];
                            Properties.Settings.Default.ii_HHmm_StartTimes[_int_New_Sequence] = Properties.Settings.Default.ii_HHmm_StartTimes[_int_Sequence];

                            Properties.Settings.Default.ii_int_snatch_Sec_Stage[_int_New_Sequence] = Properties.Settings.Default.ii_int_snatch_Sec_Stage[_int_Sequence];
                            Properties.Settings.Default.ii_int_snatch_Wgt_Opener[_int_New_Sequence] = Properties.Settings.Default.ii_int_snatch_Wgt_Opener[_int_Sequence];
                            Properties.Settings.Default.ii_int_snatch_Sec_End[_int_New_Sequence] = Properties.Settings.Default.ii_int_snatch_Sec_End[_int_Sequence];
                            Properties.Settings.Default.ii_int_snatch_Lifts_Out[_int_New_Sequence] = Properties.Settings.Default.ii_int_snatch_Lifts_Out[_int_Sequence];
                            Properties.Settings.Default.ii_bool_snatch_OpenerWarmup[_int_New_Sequence] = Properties.Settings.Default.ii_bool_snatch_OpenerWarmup[_int_Sequence];
                            for (int i = 0; i < Properties.Settings.Default.ii_strings_snatch_Extras.Count; i++)
                            {
                                string s = Properties.Settings.Default.ii_strings_snatch_Extras[i];
                                Parse_string_extras(
                                    _string: s,
                                    _int_ProfileId: out int __int_ProfileId,
                                    _int_Id: out _,
                                    _int_Order: out _,
                                    _int_Length: out _,
                                    _str_Action: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    Properties.Settings.Default.ii_strings_snatch_Extras.Add(
                                        _int_New_ProfileId.ToString("000") +
                                        s.Substring(3));
                                }
                            }
                            for (int i = 0; i < Properties.Settings.Default.ii_strings_snatch_Jumps.Count; i++)
                            {
                                string s = Properties.Settings.Default.ii_strings_snatch_Jumps[i];
                                Parse_string_jumps(
                                    _string: s,
                                    _int_ProfileId: out int __int_ProfileId,
                                    _int_Id: out _,
                                    _int_FromWeight: out _,
                                    _int_Jump: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    Properties.Settings.Default.ii_strings_snatch_Jumps.Add(
                                        _int_New_ProfileId.ToString("000") +
                                        s.Substring(3));
                                }
                            }
                            for (int i = 0; i < Properties.Settings.Default.ii_strings_snatch_Times.Count; i++)
                            {
                                string s = Properties.Settings.Default.ii_strings_snatch_Times[i];
                                Parse_string_times(
                                    _string: s,
                                    _int_ProfileId: out int __int_ProfileId,
                                    _int_Id: out _,
                                    _int_FromWeight: out _,
                                    _int_Time: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    Properties.Settings.Default.ii_strings_snatch_Times.Add(
                                        _int_New_ProfileId.ToString("000") +
                                        s.Substring(3));
                                }
                            }
                            
                            Properties.Settings.Default.ii_int_cj_Sec_Stage[_int_New_Sequence] = Properties.Settings.Default.ii_int_cj_Sec_Stage[_int_Sequence];
                            Properties.Settings.Default.ii_int_cj_Wgt_Opener[_int_New_Sequence] = Properties.Settings.Default.ii_int_cj_Wgt_Opener[_int_Sequence];
                            Properties.Settings.Default.ii_int_cj_Sec_End[_int_New_Sequence] = Properties.Settings.Default.ii_int_cj_Sec_End[_int_Sequence];
                            Properties.Settings.Default.ii_int_cj_Lifts_Out[_int_New_Sequence] = Properties.Settings.Default.ii_int_cj_Lifts_Out[_int_Sequence];
                            Properties.Settings.Default.ii_int_cj_Sec_Break[_int_New_Sequence] = Properties.Settings.Default.ii_int_cj_Sec_Break[_int_Sequence];
                            Properties.Settings.Default.ii_int_cj_snLifts_Out[_int_New_Sequence] = Properties.Settings.Default.ii_int_cj_snLifts_Out[_int_Sequence];
                            Properties.Settings.Default.ii_bool_cj_OpenerWarmup[_int_New_Sequence] = Properties.Settings.Default.ii_bool_cj_OpenerWarmup[_int_Sequence];
                            for (int i = 0; i < Properties.Settings.Default.ii_strings_cj_Extras.Count; i++)
                            {
                                string s = Properties.Settings.Default.ii_strings_cj_Extras[i];
                                Parse_string_extras(
                                    _string: s,
                                    _int_ProfileId: out int __int_ProfileId,
                                    _int_Id: out _,
                                    _int_Order: out _,
                                    _int_Length: out _,
                                    _str_Action: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    Properties.Settings.Default.ii_strings_cj_Extras.Add(
                                        _int_New_ProfileId.ToString("000") +
                                        s.Substring(3));
                                }
                            }
                            for (int i = 0; i < Properties.Settings.Default.ii_strings_cj_Jumps.Count; i++)
                            {
                                string s = Properties.Settings.Default.ii_strings_cj_Jumps[i];
                                Parse_string_jumps(
                                    _string: s,
                                    _int_ProfileId: out int __int_ProfileId,
                                    _int_Id: out _,
                                    _int_FromWeight: out _,
                                    _int_Jump: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    Properties.Settings.Default.ii_strings_cj_Jumps.Add(
                                        _int_New_ProfileId.ToString("000") +
                                        s.Substring(3));
                                }
                            }
                            for (int i = 0; i < Properties.Settings.Default.ii_strings_cj_Times.Count; i++)
                            {
                                string s = Properties.Settings.Default.ii_strings_cj_Times[i];
                                Parse_string_times(
                                    _string: s,
                                    _int_ProfileId: out int __int_ProfileId,
                                    _int_Id: out _,
                                    _int_FromWeight: out _,
                                    _int_Time: out _);
                                if (__int_ProfileId == _int_ProfileId)
                                {
                                    Properties.Settings.Default.ii_strings_cj_Times.Add(
                                        _int_New_ProfileId.ToString("000") +
                                        s.Substring(3));
                                }
                            }

                            Properties.Settings.Default.ii_bool_Beep[_int_New_Sequence] = Properties.Settings.Default.ii_bool_Beep[_int_Sequence];
             
                            Settings_Changes_Save();
                        }
                    }
                }
            }
            Populate_MenuStrip();
        }
        private void PreventMonitorPowerdown()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        private void AllowMonitorPowerdown()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        #endregion

        #region snatch_Setup_Controls
        private void numericUpDown_snatch_time_stage_ValueChanged(object sender, EventArgs e)
        {
            int _int_snatch_Sec_Stage;
            try
            {
                _int_snatch_Sec_Stage = (int)(numericUpDown_snatch_time_stage.Value);
            }
            catch
            {
                (numericUpDown_snatch_time_stage).BackColor = Color.Yellow;
                return;
            }

            if (_int_snatch_Sec_Stage < 1) 
            {
                (numericUpDown_snatch_time_stage).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_snatch_time_stage).BackColor = Color.White;

            int_snatch_Sec_Stage = _int_snatch_Sec_Stage;

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_snatch_weight_opener_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            Snatch_Opener_Set();
        }
        private void checkBox_snatch_Param_OpenerWarmup_CheckedChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            bool_snatch_OpenerWarmup = checkBox_snatch_Param_OpenerWarmup.Checked;
            Snatch_Opener_Set();
        }
        private void Snatch_Opener_Set()
        {
            int _int_snatch_Wgt_Opener;
            try
            {
                _int_snatch_Wgt_Opener = (int)(numericUpDown_snatch_weight_opener.Value);
            }
            catch
            {
                numericUpDown_snatch_weight_opener.BackColor = Color.Yellow;
                return;
            }

            if (_int_snatch_Wgt_Opener < 1)
            {
                numericUpDown_snatch_weight_opener.BackColor = Color.Yellow;
                return;
            }
            numericUpDown_snatch_weight_opener.BackColor = Color.White;

            int_snatch_Wgt_Opener = _int_snatch_Wgt_Opener;

            Apply_Opener_Graphic_Vector(_intWeightBar: int_Barbell, _intWeightOpener: int_snatch_Wgt_Opener, _boolSnatch: true);
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void numericUpDown_snatch_time_PostWarmup_ValueChanged(object sender, EventArgs e)
        {
            int _int_snatch_Sec_End;
            try
            {
                _int_snatch_Sec_End = (int)(numericUpDown_snatch_time_PostWarmup.Value);
            }
            catch
            {
                (numericUpDown_snatch_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }

            if (_int_snatch_Sec_End < 0)
            {
                (numericUpDown_snatch_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_snatch_time_PostWarmup).BackColor = Color.White;
            
            int_snatch_Sec_End = _int_snatch_Sec_End;

            snatch_Populate_Steps(boolPreserveLifts: true);
        }

        #endregion

        #region snatch extras
        private void Insert_Auto_snatch_Extras(DataTable dataTable)
        {
            int intOrder = 0;

            dataTable.Rows.Clear();
            dt_default_snatch_extras.Rows.Add(new object[]
            {
                "Ibuprofen, Coffee, Pre",
                20 * 60,
                intOrder
            });
            intOrder++;

            dt_default_snatch_extras.Rows.Add(new object[]
            {
                "Foam Roll",
                5 * 60,
                intOrder
            });
            intOrder++;

            dt_default_snatch_extras.Rows.Add(new object[]
            {
                "Shoes, tape, etc.",
                5 * 60,
                intOrder
            });
            intOrder++;

            dt_default_snatch_extras.Rows.Add(new object[]
            {
                "Stretch",
                10 * 60,
                intOrder
            });
            intOrder++;

            dt_default_snatch_extras.Rows.Add(new object[]
            {
                "Empty bar stretch",
                5 * 60,
                intOrder
            });

            dataTable.AcceptChanges();
        }
        private void snatch_Populate_Extras()
        {
            snatch_Stop_Live();
            int intY = 1;
            panel_snatch_extra.Controls.Clear();

            dt_snatch_extras.DefaultView.Sort = str_col_Order + " ASC";
            foreach (DataRow dataRow in dt_snatch_extras.DefaultView.ToTable().Rows)
            {
                snatch_Add_Extra_IndividualControls(
                    intY: intY
                    , intId: dataRow.Field<int>(str_col_Id)
                    , strTBText: dataRow.Field<string>(str_col_Action)
                    , intLength: dataRow.Field<int>(str_col_Length)
                    , bool_Add_Blank: false
                    );
                intY += 30;
            }
            snatch_Add_Extra_IndividualControls(
                intY: intY
                , intId: -1
                , strTBText: string.Empty
                , intLength: 60
                , bool_Add_Blank: true
                );
        }
        private void snatch_Add_Extra_IndividualControls(
            int intY
            , int intId
            , string strTBText
            , int intLength
            , bool bool_Add_Blank
            )
        {
            TextBox tb = new()
            {
                Text = strTBText,
                Location = new Point(6, intY),
                Size = new Size(157, 25),
                Tag = intId,
                BackColor = Color.White
            };
            NumericUpDown nmud = new()
            {
                Location = new Point(169, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intLength, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            Label lbl = new()
            {
                Location = new Point(250, intY + 3),
                Text = Seconds_To_String(intLength),
                Tag = intId
            };
            tb.TextChanged += textBox_snatch_extra_TextChanged;
            nmud.ValueChanged += numericUpDown_snatch_extra_ValueChanged;
            panel_snatch_extra.Controls.AddRange(new Control[] { tb, nmud, lbl });

            if (bool_Add_Blank)
            {
                Button btn4 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(258 + 100, intY),
                    Size = new Size(125, 25),
                    Text = str_buttontext_commit,
                    UseVisualStyleBackColor = true
                };
                btn4.Click += button_snatch_extra_commit_click;
                panel_snatch_extra.Controls.Add(btn4);

                tb.Select();
            }
            else
            {
                Button btn1 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(258 + 100, intY),
                    Size = new Size(32, 25),
                    Text = str_buttontext_up,
                    UseVisualStyleBackColor = true,
                    Tag = intId
                };
                Button btn2 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(301 + 100, intY),
                    Size = new Size(32, 25),
                    Text = str_buttontext_down,
                    UseVisualStyleBackColor = true,
                    Tag = intId
                };
                Button btn3 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(347 + 100, intY),
                    Size = new Size(36, 25),
                    Text = str_buttontext_delete,
                    UseVisualStyleBackColor = true,
                    Tag = intId
                };
                btn1.Click += button_snatch_extra_up_click;
                btn2.Click += button_snatch_extra_down_click;
                btn3.Click += button_snatch_extra_delete_click;
                panel_snatch_extra.Controls.AddRange(new Control[] { btn1, btn2, btn3 });
            }
        }
        private void button_snatch_extra_up_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            int intOrder = -1;

            foreach (DataRow dataRow in dt_snatch_extras.Rows)
            { 
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    intOrder = dataRow.Field<int>(str_col_Order);
                }
            }

            if (intOrder < 1)
            { return; }

            foreach (DataRow dataRow in dt_snatch_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI & dataRow.Field<int>(str_col_Order) == intOrder)
                {
                    dataRow[str_col_Order] = intOrder - 1;
                }
                else if (dataRow.Field<int>(str_col_Order) == intOrder - 1)
                {
                    dataRow[str_col_Order] = intOrder;
                }
            }

            snatch_Populate_Extras();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_extra_down_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            int intMax = dt_snatch_extras_Max_Order();
            int intOrder = -1;

            foreach (DataRow dataRow in dt_snatch_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    intOrder = dataRow.Field<int>(str_col_Order);
                }
            }

            if (intOrder < 0 | intOrder == intMax)
            { return; }

            foreach (DataRow dataRow in dt_snatch_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI & dataRow.Field<int>(str_col_Order) == intOrder)
                {
                    dataRow[str_col_Order] = intOrder + 1;
                }
                else if (dataRow.Field<int>(str_col_Order) == intOrder + 1)
                {
                    dataRow[str_col_Order] = intOrder;
                }
            }

            snatch_Populate_Extras();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_extra_delete_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            for (int i = 0; i < dt_snatch_extras.Rows.Count; i++)
            {
                DataRow dataRow = dt_snatch_extras.Rows[i];
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dt_snatch_extras.Rows.RemoveAt(i);
                    break;
                }
            }
            
            dt_snatch_extras_Reassign_Order();
            snatch_Populate_Extras();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_extra_commit_click(object sender, EventArgs e)
        {
            string strAction = string.Empty;
            int intLength = -1;

            foreach (Control ctrl in panel_snatch_extra.Controls)
            {
                if (strAction != String.Empty & intLength > -1)
                { break; }
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if ((int)(((TextBox)ctrl).Tag) == -1)
                    {
                        strAction = ((TextBox)ctrl).Text;
                        if (strAction == String.Empty)
                        {
                            MessageBox.Show("Action cannot be blank");
                            return;
                        }
                    }
                }
                else if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(((NumericUpDown)ctrl).Tag) == -1)
                    {
                        try
                        {
                            intLength = (int)(((NumericUpDown)ctrl).Value);
                            if (intLength < 1)
                            {
                                MessageBox.Show("Length cannot be < 1");
                                return;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Failed to parse seconds length");
                            return;
                        }
                    }
                }
            }
            if (strAction != String.Empty & intLength > -1)
            {
                if (intLength < 1)
                {
                    MessageBox.Show("Length cannot be < 1");
                    return;
                }
                DataRow dataRow = dt_snatch_extras.NewRow();
                dataRow[str_col_Action] = strAction;
                dataRow[str_col_Length] = intLength;
                dataRow[str_col_Order] = dt_snatch_extras_Max_Order() + 1;
                dt_snatch_extras.Rows.Add(dataRow);
                dt_snatch_extras_Reassign_Order();
                snatch_Populate_Extras();
                snatch_Populate_Steps(boolPreserveLifts: true);
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                snatch_Populate_Extras();
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void textBox_snatch_extra_TextChanged(object sender, EventArgs e)
        {
            int intI = (int)(((TextBox)(sender)).Tag);

            if (intI < 1) { return; }

            string strAction = ((TextBox)sender).Text;

            if (strAction == String.Empty)
            {
                ((TextBox)sender).BackColor = Color.Yellow;
                return;
            }
            else
            {
                ((TextBox)sender).BackColor = Color.White;
            }

            foreach (DataRow dataRow in dt_snatch_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Action] = strAction;
                    return;
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_snatch_extra_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intLength;
            try
            {
                intLength = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intLength < 1) { return; }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_snatch_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Length] = intLength;
                    break;
                }
            }

            foreach (Control control in panel_snatch_extra.Controls)
            {
                if (control.GetType() == typeof(Label))
                {
                    if ((int)((Label)control).Tag == intI)
                    {
                        ((Label)control).Text = Seconds_To_String(intLength);
                        break;
                    }
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private int dt_snatch_extras_Max_Order()
        {
            int intOut = -1;
            foreach (DataRow dataRow in dt_snatch_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Order) > intOut)
                {
                    intOut = dataRow.Field<int>(str_col_Order);
                }
            }

            return intOut;
        }
        private void dt_snatch_extras_Reassign_Order()
        {
            dt_snatch_extras.DefaultView.Sort = str_col_Order + " ASC";
            DataTable dataTable = dt_snatch_extras.DefaultView.ToTable();

            int intI = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                dataRow[str_col_Order] = intI;
                intI++;
            }

            dt_snatch_extras = dataTable;
        }
        #endregion

        #region snatch jumps
        private void Insert_Default_snatch_Jumps(DataTable dataTable)
        {
            dataTable.Rows.Clear();

            dataTable.Rows.Add(new object[]
            {
                1,
                20,
            });
            dataTable.Rows.Add(new object[]
            {
                40,
                10,
            });
            dataTable.Rows.Add(new object[]
            {
                50,
                5,
            });
            dataTable.Rows.Add(new object[]
            {
                80,
                4,
            });
            dataTable.Rows.Add(new object[]
            {
                89,
                3,
            });

            dataTable.AcceptChanges();
        }
        private void snatch_Populate_Jumps()
        {
            snatch_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intJump = 1;
            panel_snatch_jump.Controls.Clear();

            if (dt_snatch_jumps.Rows.Count == 0)
            {
                Insert_Default_snatch_Jumps(dt_snatch_jumps); 
            }

            dt_snatch_jumps.DefaultView.Sort = str_col_FromWeight + " ASC";
            foreach (DataRow dataRow in dt_snatch_jumps.DefaultView.ToTable().Rows)
            {
                intFromWeight = dataRow.Field<int>(str_col_FromWeight);
                intJump = dataRow.Field<int>(str_col_Jump);
                snatch_Add_Jump_IndividualControls(
                    intY
                    , dataRow.Field<int>(str_col_Id)
                    , intFromWeight
                    , intJump
                    , false
                    );
                intY += 30;
            }
            snatch_Add_Jump_IndividualControls(
                intY
                , -1
                , intFromWeight + 1
                , intJump
                , true
                );
        }
        private void snatch_Add_Jump_IndividualControls(
            int intY
            , int intId
            , int intFromWeight
            , int intJump
            , bool bool_Add_Blank
            )
        {
            NumericUpDown nmud1 = new()
            {
                Location = new Point(6, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intFromWeight, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            NumericUpDown nmud2 = new()
            {
                Location = new Point(100, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intJump, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            nmud1.ValueChanged += button_snatch_jump_FromWeight_ValueChanged;
            nmud2.ValueChanged += button_snatch_jump_Jump_ValueChanged;
            panel_snatch_jump.Controls.AddRange(new Control[] { nmud1, nmud2 });

            if (bool_Add_Blank)
            {
                Button btn4 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(200, intY),
                    Size = new Size(90, 25),
                    Text = str_buttontext_commit,
                    UseVisualStyleBackColor = true
                };
                btn4.Click += button_snatch_jump_commit_click;
                panel_snatch_jump.Controls.Add(btn4);

                nmud1.Select();
            }
            else
            {
                if (intFromWeight > 1)
                {
                    Button btn1 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(36, 25),
                        Text = str_buttontext_delete,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    btn1.Click += button_snatch_jump_delete_click;
                    panel_snatch_jump.Controls.Add(btn1);
                }
            }
        }
        private void button_snatch_jump_delete_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            for (int i = 0; i < dt_snatch_jumps.Rows.Count; i++)
            {
                DataRow dataRow = dt_snatch_jumps.Rows[i];
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    if (dataRow.Field<int>(str_col_FromWeight) > 1)
                    {
                        dt_snatch_jumps.Rows.RemoveAt(i);
                        break;
                    }
                }
            }

            snatch_Populate_Jumps();
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void button_snatch_jump_commit_click(object sender, EventArgs e)
        {
            int intFromWeight = -1;
            int intJump = -1;

            foreach (Control ctrl in panel_snatch_jump.Controls)
            {
                if (intFromWeight > -1 & intJump > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(((NumericUpDown)ctrl).Tag) == -1)
                    {
                        if (((NumericUpDown)ctrl).Left < 10)
                        {
                            try
                            {
                                intFromWeight = (int)(((NumericUpDown)ctrl).Value);
                                if (intFromWeight < 1)
                                {
                                    MessageBox.Show("From Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse From Weight");
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                intJump = (int)(((NumericUpDown)ctrl).Value);
                                if (intJump < 1)
                                {
                                    MessageBox.Show("Jump Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse Jump Weight");
                                return;
                            }
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intJump > 0)
            {
                if (snatch_Jump_Exists(intFromWeight, -1))
                {
                    MessageBox.Show("From Weight - Jump already exists");
                    return;
                }
                else
                {
                    DataRow dataRow = dt_snatch_jumps.NewRow();
                    dataRow[str_col_FromWeight] = intFromWeight;
                    dataRow[str_col_Jump] = intJump;
                    dt_snatch_jumps.Rows.Add(dataRow);
                    snatch_Populate_Jumps();
                    snatch_Populate_Steps(boolPreserveLifts: false);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                snatch_Populate_Jumps();
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void button_snatch_jump_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intFromWeight;
            try
            {
                intFromWeight = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intFromWeight < 1) 
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return; 
            }
            else if (snatch_Jump_Exists(intFromWeight, intI)) 
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return; 
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_snatch_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_FromWeight] = intFromWeight;
                    break;
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void button_snatch_jump_Jump_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intJump;
            try
            {
                intJump = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intJump < 1) 
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return; 
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_snatch_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Jump] = intJump;
                    break;
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private bool snatch_Jump_Exists(int intFromWeight, int intExcludeId)
        {
            foreach (DataRow dataRow in dt_snatch_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == intFromWeight & dataRow.Field<int>(str_col_Id) != intExcludeId) { return true; }
            }
            return false;
        }

        #endregion

        #region snatch times
        private void Insert_Default_snatch_Times(DataTable dataTable)
        {
            dataTable.Rows.Clear();
            dataTable.Rows.Add(new object[]
            {
                1,
                210,
            });
            dataTable.Rows.Add(new object[]
            {
                41,
                150,
            });

            dataTable.AcceptChanges();
        }
        private void snatch_Populate_Times()
        {
            snatch_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intTime = 1;
            panel_snatch_time.Controls.Clear();

            if (dt_snatch_times.Rows.Count == 0)
            {
                Insert_Default_snatch_Times(dt_snatch_times);
            }

            dt_snatch_times.DefaultView.Sort = str_col_FromWeight + " ASC";
            foreach (DataRow dataRow in dt_snatch_times.DefaultView.ToTable().Rows)
            {
                intFromWeight = dataRow.Field<int>(str_col_FromWeight);
                intTime = dataRow.Field<int>(str_col_Length);
                snatch_Add_Time_IndividualControls(
                    intY
                    , dataRow.Field<int>(str_col_Id)
                    , intFromWeight
                    , intTime
                    , false
                    );
                intY += 30;
            }
            snatch_Add_Time_IndividualControls(
                intY
                , -1
                , intFromWeight + 1
                , intTime
                , true
                );
        }
        private void snatch_Add_Time_IndividualControls(
            int intY
            , int intId
            , int intFromWeight
            , int intTime
            , bool bool_Add_Blank
            )
        {
            NumericUpDown nmud1 = new()
            {
                Location = new Point(6, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intFromWeight, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            NumericUpDown nmud2 = new()
            {
                Location = new Point(100, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intTime, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            nmud1.ValueChanged += button_snatch_time_FromWeight_ValueChanged;
            nmud2.ValueChanged += button_snatch_time_Time_ValueChanged;
            panel_snatch_time.Controls.AddRange(new Control[] { nmud1, nmud2 });

            if (bool_Add_Blank)
            {
                Button btn4 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(200, intY),
                    Size = new Size(90, 25),
                    Text = str_buttontext_commit,
                    UseVisualStyleBackColor = true
                };
                btn4.Click += button_snatch_time_commit_click;
                panel_snatch_time.Controls.Add(btn4);

                nmud1.Select();
            }
            else
            {
                if (intFromWeight > 1)
                {
                    Button btn1 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(36, 25),
                        Text = str_buttontext_delete,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    btn1.Click += button_snatch_time_delete_click;
                    panel_snatch_time.Controls.Add(btn1);
                }
            }
        }
        private void button_snatch_time_delete_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            for (int i = 0; i < dt_snatch_times.Rows.Count; i++)
            {
                DataRow dataRow = dt_snatch_times.Rows[i];
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    if (dataRow.Field<int>(str_col_FromWeight) > 1)
                    {
                        dt_snatch_times.Rows.RemoveAt(i);
                        break;
                    }
                }
            }

            snatch_Populate_Times();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_time_commit_click(object sender, EventArgs e)
        {
            int intFromWeight = -1;
            int intTime = -1;

            foreach (Control ctrl in panel_snatch_time.Controls)
            {
                if (intFromWeight > -1 & intTime > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(((NumericUpDown)ctrl).Tag) == -1)
                    {
                        if (((NumericUpDown)ctrl).Left < 10)
                        {
                            try
                            {
                                intFromWeight = (int)(((NumericUpDown)ctrl).Value);
                                if (intFromWeight < 1)
                                {
                                    MessageBox.Show("From Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse From Weight");
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                intTime = (int)(((NumericUpDown)ctrl).Value);
                                if (intTime < 1)
                                {
                                    MessageBox.Show("Time Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse Time Weight");
                                return;
                            }
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intTime > 0)
            {
                if (snatch_Time_Exists(intFromWeight, -1))
                {
                    MessageBox.Show("From Weight - Time already exists");
                    return;
                }
                else
                {
                    DataRow dataRow = dt_snatch_times.NewRow();
                    dataRow[str_col_FromWeight] = intFromWeight;
                    dataRow[str_col_Length] = intTime;
                    dt_snatch_times.Rows.Add(dataRow);
                    snatch_Populate_Times();
                    snatch_Populate_Steps(boolPreserveLifts: true);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                snatch_Populate_Times();
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void button_snatch_time_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intFromWeight;
            try
            {
                intFromWeight = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intFromWeight < 1)
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }
            else if (snatch_Time_Exists(intFromWeight, intI))
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_snatch_times.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_FromWeight] = intFromWeight;
                    break;
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_time_Time_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intTime;
            try
            {
                intTime = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intTime < 1)
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_snatch_times.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Length] = intTime;
                    break;
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private bool snatch_Time_Exists(int intFromWeight, int intExcludeId)
        {
            foreach (DataRow dataRow in dt_snatch_times.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == intFromWeight & dataRow.Field<int>(str_col_Id) != intExcludeId) { return true; }
            }
            return false;
        }

        #endregion

        #region snatch Steps
        private void snatch_Populate_Steps(bool boolPreserveLifts)
        {
            snatch_Stop_Live();
            int intY = 1;
            bool boolHasOverrides = false;
            panel_snatch_steps.Controls.Clear();
            
            dt_snatch_PLAN = datatable_snatch_Steps(
                boolPreserveLifts: boolPreserveLifts,
                datatableIn: dt_snatch_PLAN);

            if (dt_snatch_PLAN == null) { return; }
            dt_snatch_PLAN.DefaultView.Sort = str_col_Order + " ASC";
            foreach (DataRow dataRow in dt_snatch_PLAN.DefaultView.ToTable().Rows)
            {
                if (!boolHasOverrides)
                {
                    if (dataRow.Field<bool>(str_col_Override))
                    {
                        boolHasOverrides = true;
                    }
                }
                if (! dataRow.Field<bool>(str_col_PreStep))
                {
                    snatch_Add_Step_IndividualControls(
                        intY: intY
                        , strAction: dataRow.Field<string>(str_col_Action)
                        , intWeight: dataRow.Field<int>(str_col_Weight)
                        , intSeconds: dataRow.Field<int>(str_col_Length)
                        , intTotalSeconds: dataRow.Field<int>(str_col_TotalLength)
                        , boolOverride: dataRow.Field<bool>(str_col_Override)
                        );
                    intY += 30;
                }
            }
            Button btn = new()
            { 
                Location = new Point(6, intY),
                Size = new Size(50, 28),
                Text = "+"
                };
            btn.Click += snatch_Step_Add;
            panel_snatch_steps.Controls.Add(btn);
            if (boolHasOverrides)
            {
                Button btn2 = new()
                {
                    Location = new Point(70, intY),
                    Size = new Size(100, 28),
                    Text = "reset overrides"
                };
                btn2.Click += snatch_Step_ResetOverrides;
                panel_snatch_steps.Controls.Add(btn2);
            }
            label_snatch_Setup_StepCount.Text = (dt_snatch_PLAN.Rows.Count - 1).ToString() + " steps";
        }
        private void snatch_Add_Step_IndividualControls(
            int intY
            , string strAction
            , int intWeight
            , int intSeconds
            , int intTotalSeconds
            , bool boolOverride
            )
        {
            Label lbl1 = new()
            {
                Location = new Point(6, intY),
                AutoSize = false,
                Size = new Size(150,28),
                Text = strAction,
                Tag = intWeight
            };
            Label lbl3 = new()
            {
                Location = new Point(226, intY),
                AutoSize = false,
                Size = new Size(90, 28),
                Text = Seconds_To_String(intSeconds),
                Tag = intWeight
            };
            Label lbl4 = new()
            {
                Location = new Point(317, intY),
                AutoSize = false,
                Size = new Size(90, 28),
                Text = Seconds_To_String(intTotalSeconds),
                Tag = intWeight
            };
            panel_snatch_steps.Controls.AddRange(new Control[] { lbl1, lbl3, lbl4 });

            if (intWeight > 0)
            {
                lbl1.Click += snatch_Weight_Override_Click;
                lbl3.Click += snatch_Weight_Override_Click;
                lbl4.Click += snatch_Weight_Override_Click;
                Label lbl2 = new()
                {
                    Location = new Point(152, intY),
                    AutoSize = false,
                    Size = new Size(50, 28),
                    Text = intWeight.ToString(),
                    Tag = intWeight
                };
                lbl2.Click += snatch_Weight_Override_Click;
                if (boolOverride)
                {
                    Font fontx = new("Gadugi", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lbl1.Font = fontx;
                    lbl2.Font = fontx;
                    lbl3.Font = fontx;
                    lbl4.Font = fontx;
                }
                panel_snatch_steps.Controls.Add(lbl2);
            }
        }
        
        private DataTable datatable_snatch_Steps(
            bool boolPreserveLifts,
            DataTable datatableIn = null
            )
        {
            return datatable_X_Steps(
                boolPreserveLifts: boolPreserveLifts,
                dt_x_extras: dt_snatch_extras,
                dt_x_jumps: dt_snatch_jumps,
                dt_x_times: dt_snatch_times,
                int_x_Sec_End: int_snatch_Sec_End,
                int_x_Wgt_Opener: int_snatch_Wgt_Opener,
                bool_Opener_in_Warmup: bool_snatch_OpenerWarmup,
                datatableIn: datatableIn);
        }
        private void snatch_Step_Add(object sender, EventArgs e)
        {
            if (bool_snatch_Live)
            {
                snatch_Stop_Live();
            }

            if (dt_snatch_PLAN != null)
            {
                if (dt_snatch_PLAN.Rows.Count > 2)
                {
                    if (!dt_snatch_PLAN.Rows[dt_snatch_PLAN.Rows.Count - 1].IsNull(str_col_Weight))
                    {
                        if (dt_snatch_PLAN.Rows[dt_snatch_PLAN.Rows.Count - 1].Field<int>(str_col_Weight) > 0)
                        {
                            int int_RowIndex = dt_snatch_PLAN.Rows.Count - 1;
                            string strNewWeight = dt_snatch_PLAN.Rows[int_RowIndex].Field<int>(str_col_Weight).ToString();
                            ShowInputDialog(ref strNewWeight);
                            if (int.TryParse(strNewWeight, out int intNewWeight))
                            {
                                bool boolFound = false;
                                foreach (DataRow dR in dt_snatch_PLAN.Rows)
                                {
                                    if (!dR.IsNull(str_col_Weight))
                                    {
                                        if (dR.Field<int>(str_col_Weight) == intNewWeight)
                                        {
                                            boolFound = true;
                                            break;
                                        }
                                    }
                                }
                                if (boolFound)
                                {
                                    MessageBox.Show(intNewWeight.ToString() + " is already a step");
                                }
                                else
                                {
                                    DataRow dRNew = dt_snatch_PLAN.NewRow();
                                    dRNew[str_col_Action] = "Lift";
                                    dRNew[str_col_Weight] = intNewWeight;
                                    dRNew[str_col_PreStep] = false;
                                    dRNew[str_col_Override] = true;
                                    dt_snatch_PLAN.Rows.Add(dRNew);
                                    dt_snatch_PLAN.AcceptChanges();

                                    snatch_Populate_Steps(boolPreserveLifts: true);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void snatch_Step_ResetOverrides(object sender, EventArgs e)
        {
            if (bool_snatch_Live)
            {
                snatch_Stop_Live();
            }
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void snatch_Weight_Override_Click(object sender, EventArgs e)
        {
            int intStartWeight = 0;
            Label _labelI = (Label)sender;
            try
            {
                intStartWeight = (int)_labelI.Tag;
            } catch { }

            if (intStartWeight > 0 & dt_snatch_PLAN != null)
            {
                foreach (DataRow dR in dt_snatch_PLAN.Rows)
                {
                    if (!dR.IsNull(str_col_Weight))
                    {
                        if (dR.Field<int>(str_col_Weight) == intStartWeight)
                        {
                            if (bool_snatch_Live)
                            {
                                snatch_Stop_Live();
                            }
                            string strNewWeight = intStartWeight.ToString();
                            if (ShowInputDialog(ref strNewWeight) == DialogResult.OK)
                            {
                                if (int.TryParse(strNewWeight, out int intNewWeight))
                                {
                                    if (intNewWeight != intStartWeight)
                                    {
                                        bool boolFound = false;
                                        foreach (DataRow dRCheck in dt_snatch_PLAN.Rows)
                                        {
                                            if (!dRCheck.IsNull(str_col_Weight))
                                            {
                                                if (dRCheck.Field<int>(str_col_Weight) == intNewWeight)
                                                {
                                                    boolFound = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (boolFound)
                                        {
                                            MessageBox.Show(intNewWeight.ToString() + " is already a step");
                                        }
                                        else
                                        {
                                            dR[str_col_Weight] = intNewWeight;
                                            dR[str_col_Override] = true;
                                            snatch_Populate_Steps(boolPreserveLifts: true);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region snatch LIVE
        private void snatch_Stop_Live()
        {
            bool_snatch_Live = false;
            if (timer_snatch_Live != null)
            {
                timer_snatch_Live.Enabled = false;
                try
                {
                    timer_snatch_Live.Tick -= timer_snatch_Live_Tick;
                }
                catch { }
                timer_snatch_Live = null;
            }
            textBox_snatch_Live_LiftsOut.Visible = false;
            label_snatch_Live_TimeTillStart.Text = String.Empty;
            label_snatch_Live_TimeTillOpener.Text = String.Empty;
            label_snatch_Live_LiftsPassed.Text = String.Empty;
            Clear_snatch_Live_Steps();
            progressBar_snatch_Live_StageLift.Value = 0;
            button_snatch_Live_StageAdvance.BackColor = color_snatch_Live_BG;
            button_snatch_Live_StageAdvance.Tag = 0;
            bool_snatch_LiveLifting = false;
            button_snatch_Live_StartStop.Text = "start";
            panel_snatch_Live_Times.Visible = false;
            panel_Battery.Visible = false;
            if (!bool_cj_Live) { AllowMonitorPowerdown(); }
        }
        private void snatch_Start_Live()
        {
            bool_snatch_Live = true;
            if (timer_snatch_Live != null)
            {
                if (timer_snatch_Live.Enabled)
                {
                    timer_snatch_Live.Enabled = false; 
                    try
                    {
                        timer_snatch_Live.Tick -= timer_snatch_Live_Tick;
                    } catch { }
                }
            }
            datetime_snatch_Start = DateTime.Today.AddHours(dateTimePicker_snatch_Start.Value.Hour).AddMinutes(dateTimePicker_snatch_Start.Value.Minute);
            int_snatch_Lifts_Passed = 0;
            textBox_snatch_Live_LiftsOut.Visible = false;
            label_snatch_Live_LiftsPassed.Text = "0";
            Populate_snatch_Live_Steps();
            progressBar_snatch_Live_StageLift.Value = 0;
            progressBar_snatch_Live_StageLift.Maximum = int_snatch_Sec_Stage;
            label_snatch_Live_TimeTillStart.Text = String.Empty;
            label_snatch_Live_TimeTillOpener.Text = String.Empty;
            sim_timer_snatch_Live_Tick();
            timer_snatch_Live = new Timer { Interval = 1000 };
            timer_snatch_Live.Tick += timer_snatch_Live_Tick;
            timer_snatch_Live.Start();
            button_snatch_Live_StartStop.Text = "stop";
            button_snatch_Live_StageAdvance.Select();
            PreventMonitorPowerdown();
        }
        private void button_snatch_Live_StartStop_Click(object sender, EventArgs e)
        {
            if (bool_snatch_Live)
            {
                snatch_Stop_Live();
            }
            else
            {
                snatch_Start_Live();
            }
        }
        private void Clear_snatch_Live_Steps()
        {
            dt_snatch_LIVE = null;
            panel_snatch_Live_Steps.Controls.Clear();
        }
        private void Populate_snatch_Live_Steps()
        {
            Clear_snatch_Live_Steps();

            if (dt_snatch_PLAN is null)
            {
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
            if (dt_snatch_PLAN is null)
            {
                MessageBox.Show("An error has occurred. Step plan could not be determined.");
                this.Close();
                return;
            }
            dt_snatch_LIVE = dt_snatch_PLAN.Copy();
            dt_snatch_LIVE.Columns.AddRange(
                new DataColumn[]
                {
                    new DataColumn(str_col_PanelLiveStep, typeof(Panel))
                    , new DataColumn(str_col_LabelAction, typeof(Label))
                    , new DataColumn(str_col_LabelTime, typeof(Label))
                    , new DataColumn(str_col_LabelProgressTime, typeof(Label))
                    , new DataColumn(str_col_LabelWeight, typeof(Label))
                    , new DataColumn(str_col_ProgressBarStep, typeof(ProgressBar))
                    , new DataColumn(str_col_GraphicPanel, typeof(Panel))
                });

            int intY = 1;
            int _int_panel_Live_Step_Width = panel_snatch_Live_Steps.Width - 4;
            int _int_progressBar_Step_Width_NoScroll = _int_panel_Live_Step_Width - 350;
            int _int_progressBar_Step_Width_Scroll = _int_progressBar_Step_Width_NoScroll - SystemInformation.VerticalScrollBarWidth;
            int _int_progressBar_Step_Width;
            Point _point_progressBar_Step_Location = new(300, 6);

            SuspendLayout();
            foreach (DataRow dataRow in dt_snatch_LIVE.Rows)
            {
                if (panel_snatch_Live_Steps.VerticalScroll.Visible)
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_Scroll;
                }
                else
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_NoScroll;
                }
                string strActionText;
                bool boolIsLift = (dataRow.Field<int>(str_col_Weight) > 0);
                if (boolIsLift)
                {
                    strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString() + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                        Environment.NewLine +
                        "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                }
                else
                {
                    strActionText = dataRow.Field<string>(str_col_Action) + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                        Environment.NewLine +
                        "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                }
                Panel panel_Live_Step = new()
                {
                    Size = new Size(_int_panel_Live_Step_Width, 80),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = color_snatch_Live_BG,
                    ForeColor = color_Live_Default_FG,
                    Location = new Point(1, intY)
                };
                Label label_Action = new()
                {
                    Text = strActionText,
                    AutoSize = false,
                    Size = new Size(280, 75),
                    TextAlign = ContentAlignment.TopRight,
                    Location = new Point(6, 1)
                };
                ProgressBar progressBar_Step = new()
                {
                    Size = new Size(_int_progressBar_Step_Width, 65),
                    Location = _point_progressBar_Step_Location,
                    Maximum = dataRow.Field<int>(str_col_Length),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                if (progressBar_Step.Maximum == 0)
                {
                    progressBar_Step.Maximum = 1;
                    progressBar_Step.Value = 1;
                }
                Label label_Weight = null;
                if (boolIsLift)
                {
                    label_Weight = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.Black,
                        AutoSize = false,
                        Size = new Size(105, 30),
                        Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width - 105, 6),
                        ForeColor = SystemColors.Window,
                        Text = "lift " + dataRow.Field<int>(str_col_Weight).ToString(),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    };
                    label_Weight.Text = "lift " + dataRow.Field<int>(str_col_Weight).ToString();

                    if (dataRow.Field<bool>(str_col_Override))
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Italic);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold | FontStyle.Italic);
                    }
                    else
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Regular);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold);
                    }
                }
                Label label_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(120, 30),
                    Location = _point_progressBar_Step_Location,
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                };
                Label label_Progress_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(105, 30),
                    Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width - 105, 6),
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                };
                panel_Live_Step.Controls.AddRange(new Control[]
                {
                    label_Action,
                    progressBar_Step,
                    label_Progress_Time,
                    label_Time
                });
                WeightBox weightBoxGraphic = null;
                if (boolIsLift)
                {
                    weightBoxGraphic = new()
                    {
                        boolOpener = false,
                        intWeightBar = int_Barbell,
                        intWeight = dataRow.Field<int>(str_col_Weight),
                        intShadowWidth = 1,
                        intPlateGap = -1,
                        Size = new Size(120, 65),
                        BackColor = Color.FromArgb(128, 128, 128),
                        BorderStyle = BorderStyle.Fixed3D,
                        Location = new Point(10, 6),
                        Visible = true,
                    };
                    weightBoxGraphic.Paint += Apply_Vector_Weight_Graphic;
                    panel_Live_Step.Controls.Add(weightBoxGraphic);
                }
                progressBar_Step.BringToFront();
                dataRow[str_col_PanelLiveStep] = panel_Live_Step;
                dataRow[str_col_LabelAction] = label_Action;
                dataRow[str_col_ProgressBarStep] = progressBar_Step;
                weightBoxGraphic?.BringToFront();
                if (label_Weight != null) 
                {
                    panel_Live_Step.Controls.Add(label_Weight);
                    dataRow[str_col_LabelWeight] = label_Weight;
                    label_Weight.BringToFront();
                }
                label_Time.BringToFront();
                dataRow[str_col_LabelTime] = label_Time;
                label_Progress_Time.BringToFront();
                dataRow[str_col_LabelProgressTime] = label_Progress_Time;
                panel_snatch_Live_Steps.Controls.Add(panel_Live_Step);

                intY += 81;
            }
            dt_snatch_LIVE.AcceptChanges();
            int_snatch_Warmup_Step = -1;
            label_snatch_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
            panel_snatch_Live_Times.Visible = true;
            ResumeLayout();
        }
        private void timer_snatch_Live_Tick(object sender, EventArgs e)
        {
            sim_timer_snatch_Live_Tick();
        }
        private void sim_timer_snatch_Live_Tick()
        {
            DateTime _dateTime_Now = DateTime.Now;
            int intSecondsToStart = (int)datetime_snatch_Start.Subtract(_dateTime_Now).TotalSeconds;
            int intSecondsToOpen = 0;

            if (propertyData_Battery is null)
            {
                System.Management.ObjectQuery query = new("Select * FROM Win32_Battery");
                ManagementObjectSearcher searcher = new(query);
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject mo in collection.Cast<ManagementObject>())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        if (property.Name == "EstimatedChargeRemaining")
                        {
                            propertyData_Battery = property;
                        }
                    }
                }
            }
            if (propertyData_Battery is not null)
            {
                System.UInt16 _uInt_Battery = 100;
                try
                {
                    _uInt_Battery = (System.UInt16)propertyData_Battery.Value;
                }
                catch { }
                if (_uInt_Battery > 0 && _uInt_Battery < 100)
                {
                    panel_Battery.Visible = true;
                    progressBar_Battery.Value = _uInt_Battery;
                    label_Battery_Percentage.Text = _uInt_Battery.ToString() + "%";
                }
                else
                {
                    panel_Battery.Visible = false;
                }
            }
            else
            {
                panel_Battery.Visible = false;
            }

            if (intSecondsToStart > 0)
            {
                if ((int)button_snatch_Live_StageAdvance.Tag == 1)
                {
                    button_snatch_Live_StageAdvance.Tag = 0;
                    button_snatch_Live_StageAdvance.BackColor = color_snatch_Live_BG;
                }
                label_snatch_Live_TimeTillStart.Text = Seconds_To_String(intSecondsToStart);
                intSecondsToOpen = intSecondsToStart + (int_snatch_Lifts_Out * int_snatch_Sec_Stage);
                bool_snatch_LiveLifting = false;
            }
            else
            {
                if (!bool_snatch_LiveLifting)
                {
                    if (int_snatch_Lifts_Out > 0)
                    {
                        bool_snatch_LiveLifting = true;
                        progressBar_snatch_Live_StageLift.Value = 0;
                    }
                    label_snatch_Live_TimeTillStart.Text = "passed";
                }

                if (int_snatch_Lifts_Out > 0)
                {
                    if ((int)button_snatch_Live_StageAdvance.Tag == 0)
                    {
                        button_snatch_Live_StageAdvance.Tag = 1;
                        button_snatch_Live_StageAdvance.BackColor = color_AdvanceButton_Active;
                    }
                    progressBar_snatch_Live_StageLift.PerformStep();
                    if (bool_snatch_AutoAdvance)
                    {
                        if (progressBar_snatch_Live_StageLift.Value == progressBar_snatch_Live_StageLift.Maximum)
                        {
                            snatch_Advance_StageLift();
                        }
                    }
                    intSecondsToOpen += (int_snatch_Lifts_Out - 1) * int_snatch_Sec_Stage +
                        progressBar_snatch_Live_StageLift.Maximum - progressBar_snatch_Live_StageLift.Value;
                }
                else
                {
                    if ((int)button_snatch_Live_StageAdvance.Tag == 1)
                    {
                        button_snatch_Live_StageAdvance.Tag = 0;
                        button_snatch_Live_StageAdvance.BackColor = color_snatch_Live_BG;
                    }
                }
            }


            DateTime _dateTime_Open = _dateTime_Now.AddSeconds(intSecondsToOpen);
            if (intSecondsToOpen > 0)
            {
                label_snatch_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
                label_snatch_Live_OpenTime.Text = _dateTime_Open.ToString("HH:mm:ss");
            }
            else
            {
                label_snatch_Live_TimeTillOpener.Text = "-";
                label_snatch_Live_OpenTime.Text = "passed";
            }

            int _intStep = -1;
            foreach (DataRow dataRow in dt_snatch_LIVE.Rows)
            {
                if (dataRow.Field<int>(str_col_TotalLengthReverse) >= intSecondsToOpen)
                {
                    if (dataRow.Field<int>(str_col_Order) > _intStep)
                    {
                        _intStep = dataRow.Field<int>(str_col_Order);
                    }
                }
            }

            if (_intStep == -1) // adjust wait time
            {
                int intTLR = 0;
                foreach (DataRow dataRow in dt_snatch_LIVE.Rows)
                {
                    if (dataRow.Field<int>(str_col_Order) == 1)
                    {
                        intTLR = dataRow.Field<int>(str_col_TotalLengthReverse);
                        break;
                    }
                }
                if (intTLR > 0 & intSecondsToOpen > intTLR)
                {
                    int intSecToAdd = 0;
                    foreach (DataRow dataRow in dt_snatch_LIVE.Rows)
                    {
                        if (dataRow.Field<bool>(str_col_PreStep))
                        {
                            intSecToAdd = (intSecondsToOpen - intTLR) - dataRow.Field<int>(str_col_Length);
                            dataRow[str_col_Length] = intSecondsToOpen - intTLR;
                            dataRow.Field<ProgressBar>(str_col_ProgressBarStep).Maximum = intSecondsToOpen - intTLR;
                            dataRow[str_col_TotalLength] = intSecondsToOpen - intTLR;
                            dataRow[str_col_TotalLengthReverse] = intSecondsToOpen;
                            _intStep = 0;
                            break;
                        }
                    }
                    if (_intStep == 0 & intSecToAdd != 0)
                    {
                        foreach (DataRow dataRow in dt_snatch_LIVE.Rows)
                        {
                            if (! dataRow.Field<bool>(str_col_PreStep))
                            {
                                dataRow[str_col_TotalLength] = dataRow.Field<int>(str_col_TotalLength) + intSecToAdd;
                            }
                        }
                    }
                }
            }

            if (_intStep > -1)
            {
                Panel panel_Live_Step = null;
                Label label_Action;
                ProgressBar progressBar_Step;
                Label label_Progress_Time;
                bool boolUpdBGsFGs = (_intStep != int_snatch_Warmup_Step);

                foreach (DataRow dataRow in dt_snatch_LIVE.Rows)
                {
                    int _int_Order = dataRow.Field<int>(str_col_Order);
                    Label label_Time = dataRow.Field<Label>(str_col_LabelTime);
                    if (_int_Order > _intStep)
                    {
                        label_Time.Visible = true;
                        label_Time.Text = _dateTime_Open.AddSeconds(-dataRow.Field<int>(str_col_TotalLengthReverse)).ToString("HH:mm:ss");
                    }
                    else
                    {
                        label_Time.Visible = false;
                    }
                }

                foreach (DataRow dataRow in dt_snatch_LIVE.Rows)
                {
                    int _int_Order = dataRow.Field<int>(str_col_Order);
                    if (_int_Order == _intStep)
                    {
                        panel_Live_Step = dataRow.Field<Panel>(str_col_PanelLiveStep);
                        label_Progress_Time = dataRow.Field<Label>(str_col_LabelProgressTime);
                        progressBar_Step = dataRow.Field<ProgressBar>(str_col_ProgressBarStep);

                        int intStepLength = dataRow.Field<int>(str_col_Length);
                        int intSecIntoStep = dataRow.Field<int>(str_col_TotalLengthReverse) - intSecondsToOpen;
                        label_Progress_Time.Text = Seconds_To_String(_int_Seconds: intStepLength - intSecIntoStep, _bool_ShortString: true);
                        progressBar_Step.Value = intSecIntoStep;

                        if (boolUpdBGsFGs)
                        {
                            string strActionText;
                            bool boolIsLift = (dataRow.Field<int>(str_col_Weight) > 0);
                            if (boolIsLift)
                            {
                                strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString();
                            }
                            else
                            {
                                strActionText = dataRow.Field<string>(str_col_Action);
                            }
                            label_Action = dataRow.Field<Label>(str_col_LabelAction);
                            label_Action.Text = strActionText;
                            label_Progress_Time.Visible = true;
                        }

                        if (boolUpdBGsFGs)
                        {
                            panel_Live_Step.BackColor = color_Live_Highlight_BG;
                            panel_Live_Step.ForeColor = color_Live_Highlight_FG;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (boolUpdBGsFGs)
                    {
                        panel_Live_Step = dataRow.Field<Panel>(str_col_PanelLiveStep);
                        panel_Live_Step.BackColor = color_snatch_Live_BG;
                        panel_Live_Step.ForeColor = color_Live_Default_FG;
                        label_Progress_Time = dataRow.Field<Label>(str_col_LabelProgressTime);
                        label_Progress_Time.Visible = false;
                        progressBar_Step = dataRow.Field<ProgressBar>(str_col_ProgressBarStep);
                        label_Action = dataRow.Field<Label>(str_col_LabelAction);
                        string strActionText;
                        bool boolIsLift = (dataRow.Field<int>(str_col_Weight) > 0);

                        if (_int_Order < _intStep)
                        {
                            progressBar_Step.Value = progressBar_Step.Maximum;
                            if (boolIsLift)
                            {
                                strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString();
                            }
                            else
                            {
                                strActionText = dataRow.Field<string>(str_col_Action);
                            }
                        }
                        else
                        {
                            progressBar_Step.Value = 0;
                            if (boolIsLift)
                            {
                                strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString() + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                                    Environment.NewLine +
                                    "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                            }
                            else
                            {
                                strActionText = dataRow.Field<string>(str_col_Action) + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                                    Environment.NewLine +
                                    "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                            }
                        }

                        label_Action.Text = strActionText;
                    }
                }
                int_snatch_Warmup_Step = _intStep;
                if (boolUpdBGsFGs & bool_Beep)
                {
                    Console.Beep(750, 600);
                }
            }
            else
            {
                snatch_Stop_Live();
            }

            label_snatch_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void menuStrip_Profile_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void progressBar_snatch_Live_StageLift_MouseClick(object sender, MouseEventArgs e)
        {
            if (bool_snatch_Live & bool_snatch_LiveLifting)
            {
                double dbl_Percent = (double)(e.X) / (double)(progressBar_snatch_Live_StageLift.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_snatch_Live_StageLift.Value = (int)((double)progressBar_snatch_Live_StageLift.Maximum * dbl_Percent);
                }
            }
        }
        private void button_snatch_Live_LiftsDecr_Click(object sender, EventArgs e)
        {
            if (int_snatch_Lifts_Out > 0)
            {
                int_snatch_Lifts_Out--;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
                if (int_snatch_Lifts_Out == 0)
                {
                    if (bool_snatch_LiveLifting)
                    {
                        bool_snatch_LiveLifting = false;
                        progressBar_snatch_Live_StageLift.Value = 0;
                    }
                }
            }
        }
        private void button_snatch_Live_LiftsIncr_Click(object sender, EventArgs e)
        {
            if (int_snatch_Lifts_Out < 99)
            {
                int_snatch_Lifts_Out++;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            }
        }
        private void button_snatch_Live_StageAdvance_Click(object sender, EventArgs e)
        {
            if (int_snatch_Lifts_Out >= 0 & bool_snatch_LiveLifting)
            {
                snatch_Advance_StageLift();
            }
        }
        private void label_snatch_Live_LiftsOut_Click(object sender, EventArgs e)
        {
            bool_Loading = true;
            textBox_snatch_Live_LiftsOut.Location = label_snatch_Live_LiftsOut.Location;
            textBox_snatch_Live_LiftsOut.Size = label_snatch_Live_LiftsOut.Size;
            textBox_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            textBox_snatch_Live_LiftsOut.Visible = true;
            textBox_snatch_Live_LiftsOut.BringToFront();
            textBox_snatch_Live_LiftsOut.Select();
            bool_Loading = false;
        }
        private void textBox_snatch_Live_LiftsOut_TextChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_snatch_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_snatch_Lifts_Out) &&
                _int_snatch_Lifts_Out >= 0 &&
                _int_snatch_Lifts_Out < 100 &&
                int_snatch_Lifts_Out != _int_snatch_Lifts_Out)
            {
                int_snatch_Lifts_Out = _int_snatch_Lifts_Out;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            }
        }
        private void textBox_snatch_Live_LiftsOut_Leave(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_snatch_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_snatch_Lifts_Out) &&
                _int_snatch_Lifts_Out >= 0 &&
                _int_snatch_Lifts_Out < 100 &&
                int_snatch_Lifts_Out != _int_snatch_Lifts_Out)
            {
                int_snatch_Lifts_Out = _int_snatch_Lifts_Out;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            }
            textBox_snatch_Live_LiftsOut.Visible = false;
        }
        private void snatch_Advance_StageLift()
        {
            if (int_snatch_Lifts_Out > 0)
            {
                int_snatch_Lifts_Out--;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
                if (bool_snatch_Live && datetime_snatch_Start < DateTime.Now)
                {
                    int_snatch_Lifts_Passed++;
                }
                else
                {
                    int_snatch_Lifts_Passed = 0;
                }
                label_snatch_Live_LiftsPassed.Text = (bool_snatch_Live ? int_snatch_Lifts_Passed.ToString() : string.Empty);
            }
            progressBar_snatch_Live_StageLift.Value = 0;
        }
        private void dateTimePicker_snatch_Start_ValueChanged(object sender, EventArgs e)
        {
            datetime_snatch_Start = DateTime.Today.AddHours(dateTimePicker_snatch_Start.Value.Hour).AddMinutes(dateTimePicker_snatch_Start.Value.Minute);
            if (datetime_snatch_Start > DateTime.Now && int_snatch_Lifts_Passed != 0)
            {
                int_snatch_Lifts_Passed = 0;
                label_snatch_Live_LiftsPassed.Text = (bool_snatch_Live ? int_snatch_Lifts_Passed.ToString() : string.Empty);
            }
        }
        private void checkBox_snatch_Live_Auto_CheckedChanged(object sender, EventArgs e)
        {
            bool_snatch_AutoAdvance = checkBox_snatch_Live_Auto.Checked;
        }
        private void checkBox_Live_Beep_CheckedChanged(object sender, EventArgs e)
        {
            if (!bool_Loading)
            {
                bool_Loading = true;
                bool_Beep = ((CheckBox)sender).Checked;
                checkBox_snatch_Live_Beep.Checked = bool_Beep;
                checkBox_cj_Live_Beep.Checked = bool_Beep;
                bool_Loading = false;
            }
        }
        private void splitContainer_snatch_DoubleClick(object sender, EventArgs e)
        {
            splitContainer_snatch.SplitterDistance = 0;
        }

        #endregion

        #region cj_Setup_Controls
        private void numericUpDown_cj_time_stage_ValueChanged(object sender, EventArgs e)
        {
            int _int_cj_Sec_Stage;
            try
            {
                _int_cj_Sec_Stage = (int)(numericUpDown_cj_time_stage.Value);
            }
            catch
            {
                (numericUpDown_cj_time_stage).BackColor = Color.Yellow;
                return;
            }

            if (_int_cj_Sec_Stage < 1) 
            {
                (numericUpDown_cj_time_stage).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_cj_time_stage).BackColor = Color.White;

            int_cj_Sec_Stage = _int_cj_Sec_Stage;

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_cj_weight_opener_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            CJ_Opener_Set();
        }
        private void checkBox_cj_Param_OpenerWarmup_CheckedChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            bool_cj_OpenerWarmup = checkBox_cj_Param_OpenerWarmup.Checked;
            CJ_Opener_Set();
        }
        private void CJ_Opener_Set()
        {
            int _int_cj_Wgt_Opener;
            try
            {
                _int_cj_Wgt_Opener = (int)(numericUpDown_cj_weight_opener.Value);
            }
            catch
            {
                (numericUpDown_cj_weight_opener).BackColor = Color.Yellow;
                return;
            }

            if (_int_cj_Wgt_Opener < 1)
            {
                (numericUpDown_cj_weight_opener).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_cj_weight_opener).BackColor = Color.White;

            int_cj_Wgt_Opener = _int_cj_Wgt_Opener;

            Apply_Opener_Graphic_Vector(_intWeightBar: int_Barbell, _intWeightOpener: int_cj_Wgt_Opener, _boolSnatch: false);
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void numericUpDown_cj_time_PostWarmup_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            int _int_cj_Sec_End;
            try
            {
                _int_cj_Sec_End = (int)(numericUpDown_cj_time_PostWarmup.Value);
            }
            catch
            {
                (numericUpDown_cj_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }

            if (_int_cj_Sec_End < 0)
            {
                (numericUpDown_cj_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_cj_time_PostWarmup).BackColor = Color.White;

            int_cj_Sec_End = _int_cj_Sec_End;

            cj_Populate_Steps(boolPreserveLifts: true);
        }

        #endregion

        #region cj extras
        private void Insert_Auto_cj_Extras(DataTable dataTable)
        {
            DataRow dataRow;
            int intOrder = 0;

            dataTable.Rows.Clear();

            dataRow = dataTable.NewRow();
            dataRow[str_col_Action] = "Stretch";
            dataRow[str_col_Length] = 5 * 60;
            dataRow[str_col_Order] = intOrder;
            dataTable.Rows.Add(dataRow);

            dataTable.AcceptChanges();
        }
        private void cj_Populate_Extras()
        {
            cj_Stop_Live();
            int intY = 1;
            panel_cj_extra.Controls.Clear();

            dt_cj_extras.DefaultView.Sort = str_col_Order + " ASC";
            foreach (DataRow dataRow in dt_cj_extras.DefaultView.ToTable().Rows)
            {
                cj_Add_Extra_IndividualControls(
                    intY: intY
                    , intId: dataRow.Field<int>(str_col_Id)
                    , strTBText: dataRow.Field<string>(str_col_Action)
                    , intLength: dataRow.Field<int>(str_col_Length)
                    , bool_Add_Blank: false
                    );
                intY += 30;
            }
            cj_Add_Extra_IndividualControls(
                intY: intY
                , intId: -1
                , strTBText: string.Empty
                , intLength: 60
                , bool_Add_Blank: true
                );
        }
        private void cj_Add_Extra_IndividualControls(
            int intY
            , int intId
            , string strTBText
            , int intLength
            , bool bool_Add_Blank
            )
        {
            TextBox tb = new()
            {
                Text = strTBText,
                Location = new Point(6, intY),
                Size = new Size(157, 25),
                Tag = intId,
                BackColor = Color.White
            };
            NumericUpDown nmud = new()
            {
                Location = new Point(169, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intLength, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            Label lbl = new()
            {
                Location = new Point(250, intY + 3),
                Text = Seconds_To_String(intLength),
                Tag = intId
            };
            tb.TextChanged += textBox_cj_extra_TextChanged;
            nmud.ValueChanged += numericUpDown_cj_extra_ValueChanged;
            panel_cj_extra.Controls.AddRange(new Control[] { tb, nmud, lbl });

            if (bool_Add_Blank)
            {
                Button btn4 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(258 + 100, intY),
                    Size = new Size(125, 25),
                    Text = str_buttontext_commit,
                    UseVisualStyleBackColor = true
                };
                btn4.Click += button_cj_extra_commit_click;
                panel_cj_extra.Controls.Add(btn4);

                tb.Select();
            }
            else
            {
                Button btn1 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(258 + 100, intY),
                    Size = new Size(32, 25),
                    Text = str_buttontext_up,
                    UseVisualStyleBackColor = true,
                    Tag = intId
                };
                Button btn2 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(301 + 100, intY),
                    Size = new Size(32, 25),
                    Text = str_buttontext_down,
                    UseVisualStyleBackColor = true,
                    Tag = intId
                };
                Button btn3 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(347 + 100, intY),
                    Size = new Size(36, 25),
                    Text = str_buttontext_delete,
                    UseVisualStyleBackColor = true,
                    Tag = intId
                };
                btn1.Click += button_cj_extra_up_click;
                btn2.Click += button_cj_extra_down_click;
                btn3.Click += button_cj_extra_delete_click;
                panel_cj_extra.Controls.AddRange(new Control[] { btn1, btn2, btn3 });
            }
        }
        private void button_cj_extra_up_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            int intOrder = -1;

            foreach (DataRow dataRow in dt_cj_extras.Rows)
            { 
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    intOrder = dataRow.Field<int>(str_col_Order);
                }
            }

            if (intOrder < 1)
            { return; }

            foreach (DataRow dataRow in dt_cj_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI & dataRow.Field<int>(str_col_Order) == intOrder)
                {
                    dataRow[str_col_Order] = intOrder - 1;
                }
                else if (dataRow.Field<int>(str_col_Order) == intOrder - 1)
                {
                    dataRow[str_col_Order] = intOrder;
                }
            }

            cj_Populate_Extras();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_extra_down_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            int intMax = dt_cj_extras_Max_Order();
            int intOrder = -1;

            foreach (DataRow dataRow in dt_cj_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    intOrder = dataRow.Field<int>(str_col_Order);
                }
            }

            if (intOrder < 0 | intOrder == intMax)
            { return; }

            foreach (DataRow dataRow in dt_cj_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI & dataRow.Field<int>(str_col_Order) == intOrder)
                {
                    dataRow[str_col_Order] = intOrder + 1;
                }
                else if (dataRow.Field<int>(str_col_Order) == intOrder + 1)
                {
                    dataRow[str_col_Order] = intOrder;
                }
            }

            cj_Populate_Extras();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_extra_delete_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            for (int i = 0; i < dt_cj_extras.Rows.Count; i++)
            {
                DataRow dataRow = dt_cj_extras.Rows[i];
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dt_cj_extras.Rows.RemoveAt(i);
                    break;
                }
            }
            
            dt_cj_extras_Reassign_Order();
            cj_Populate_Extras();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_extra_commit_click(object sender, EventArgs e)
        {
            string strAction = string.Empty;
            int intLength = -1;

            foreach (Control ctrl in panel_cj_extra.Controls)
            {
                if (strAction != String.Empty & intLength > -1)
                { break; }
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if ((int)(((TextBox)ctrl).Tag) == -1)
                    {
                        strAction = ((TextBox)ctrl).Text;
                        if (strAction == String.Empty)
                        {
                            MessageBox.Show("Action cannot be blank");
                            return;
                        }
                    }
                }
                else if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(((NumericUpDown)ctrl).Tag) == -1)
                    {
                        try
                        {
                            intLength = (int)(((NumericUpDown)ctrl).Value);
                            if (intLength < 1)
                            {
                                MessageBox.Show("Length cannot be < 1");
                                return;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Failed to parse seconds length");
                            return;
                        }
                    }
                }
            }
            if (strAction != String.Empty & intLength > -1)
            {
                if (intLength < 1)
                {
                    MessageBox.Show("Length cannot be < 1");
                    return;
                }
                DataRow dataRow = dt_cj_extras.NewRow();
                dataRow[str_col_Action] = strAction;
                dataRow[str_col_Length] = intLength;
                dataRow[str_col_Order] = dt_cj_extras_Max_Order() + 1;
                dt_cj_extras.Rows.Add(dataRow);
                dt_cj_extras_Reassign_Order();
                cj_Populate_Extras();
                cj_Populate_Steps(boolPreserveLifts: true);
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                cj_Populate_Extras();
                cj_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void textBox_cj_extra_TextChanged(object sender, EventArgs e)
        {
            int intI = (int)(((TextBox)(sender)).Tag);

            if (intI < 1) { return; }

            string strAction = ((TextBox)sender).Text;

            if (strAction == String.Empty)
            {
                ((TextBox)sender).BackColor = Color.Yellow;
                return;
            }
            else
            {
                ((TextBox)sender).BackColor = Color.White;
            }

            foreach (DataRow dataRow in dt_cj_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Action] = strAction;
                    return;
                }
            }

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_cj_extra_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intLength;
            try
            {
                intLength = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intLength < 1) { return; }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_cj_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Length] = intLength;
                    break;
                }
            }

            foreach (Control control in panel_cj_extra.Controls)
            {
                if (control.GetType() == typeof(Label))
                {
                    if ((int)((Label)control).Tag == intI)
                    {
                        ((Label)control).Text = Seconds_To_String(intLength);
                        break;
                    }
                }
            }

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private int dt_cj_extras_Max_Order()
        {
            int intOut = -1;
            foreach (DataRow dataRow in dt_cj_extras.Rows)
            {
                if (dataRow.Field<int>(str_col_Order) > intOut)
                {
                    intOut = dataRow.Field<int>(str_col_Order);
                }
            }

            return intOut;
        }
        private void dt_cj_extras_Reassign_Order()
        {
            dt_cj_extras.DefaultView.Sort = str_col_Order + " ASC";
            DataTable dataTable = dt_cj_extras.DefaultView.ToTable();

            int intI = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                dataRow[str_col_Order] = intI;
                intI++;
            }

            dt_cj_extras = dataTable;
        }
        #endregion

        #region cj jumps
        private void Insert_Default_cj_Jumps(DataTable dataTable)
        {
            DataRow dataRow;

            dataTable.Rows.Clear();

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 1;
            dataRow[str_col_Jump] = 30;
            dataTable.Rows.Add(dataRow);

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 50;
            dataRow[str_col_Jump] = 10;
            dataTable.Rows.Add(dataRow);

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 90;
            dataRow[str_col_Jump] = 7;
            dataTable.Rows.Add(dataRow);

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 100;
            dataRow[str_col_Jump] = 5;
            dataTable.Rows.Add(dataRow);

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 105;
            dataRow[str_col_Jump] = 4;
            dataTable.Rows.Add(dataRow);

            dataTable.AcceptChanges();
        }
        private void cj_Populate_Jumps()
        {
            cj_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intJump = 1;
            panel_cj_jump.Controls.Clear();

            if (dt_cj_jumps.Rows.Count == 0)
            {
                Insert_Default_cj_Jumps(dt_cj_jumps); 
            }

            dt_cj_jumps.DefaultView.Sort = str_col_FromWeight + " ASC";
            foreach (DataRow dataRow in dt_cj_jumps.DefaultView.ToTable().Rows)
            {
                intFromWeight = dataRow.Field<int>(str_col_FromWeight);
                intJump = dataRow.Field<int>(str_col_Jump);
                cj_Add_Jump_IndividualControls(
                    intY
                    , dataRow.Field<int>(str_col_Id)
                    , intFromWeight
                    , intJump
                    , false
                    );
                intY += 30;
            }
            cj_Add_Jump_IndividualControls(
                intY
                , -1
                , intFromWeight + 1
                , intJump
                , true
                );
        }
        private void cj_Add_Jump_IndividualControls(
            int intY
            , int intId
            , int intFromWeight
            , int intJump
            , bool bool_Add_Blank
            )
        {
            NumericUpDown nmud1 = new()
            {
                Location = new Point(6, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intFromWeight, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            NumericUpDown nmud2 = new()
            {
                Location = new Point(100, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intJump, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            nmud1.ValueChanged += button_cj_jump_FromWeight_ValueChanged;
            nmud2.ValueChanged += button_cj_jump_Jump_ValueChanged;
            panel_cj_jump.Controls.AddRange(new Control[] { nmud1, nmud2 });

            if (bool_Add_Blank)
            {
                Button btn4 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(200, intY),
                    Size = new Size(90, 25),
                    Text = str_buttontext_commit,
                    UseVisualStyleBackColor = true
                };
                btn4.Click += button_cj_jump_commit_click;
                panel_cj_jump.Controls.Add(btn4);

                nmud1.Select();
            }
            else
            {
                if (intFromWeight > 1)
                {
                    Button btn1 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(36, 25),
                        Text = str_buttontext_delete,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    btn1.Click += button_cj_jump_delete_click;
                    panel_cj_jump.Controls.Add( btn1);
                }
            }
        }
        private void button_cj_jump_delete_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            for (int i = 0; i < dt_cj_jumps.Rows.Count; i++)
            {
                DataRow dataRow = dt_cj_jumps.Rows[i];
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    if (dataRow.Field<int>(str_col_FromWeight) > 1)
                    {
                        dt_cj_jumps.Rows.RemoveAt(i);
                        break;
                    }
                }
            }

            cj_Populate_Jumps();
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void button_cj_jump_commit_click(object sender, EventArgs e)
        {
            int intFromWeight = -1;
            int intJump = -1;

            foreach (Control ctrl in panel_cj_jump.Controls)
            {
                if (intFromWeight > -1 & intJump > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(((NumericUpDown)ctrl).Tag) == -1)
                    {
                        if (((NumericUpDown)ctrl).Left < 10)
                        {
                            try
                            {
                                intFromWeight = (int)(((NumericUpDown)ctrl).Value);
                                if (intFromWeight < 1)
                                {
                                    MessageBox.Show("From Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse From Weight");
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                intJump = (int)(((NumericUpDown)ctrl).Value);
                                if (intJump < 1)
                                {
                                    MessageBox.Show("Jump Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse Jump Weight");
                                return;
                            }
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intJump > 0)
            {
                if (cj_Jump_Exists(intFromWeight, -1))
                {
                    MessageBox.Show("From Weight - Jump already exists");
                    return;
                }
                else
                {
                    DataRow dataRow = dt_cj_jumps.NewRow();
                    dataRow[str_col_FromWeight] = intFromWeight;
                    dataRow[str_col_Jump] = intJump;
                    dt_cj_jumps.Rows.Add(dataRow);
                    cj_Populate_Jumps();
                    cj_Populate_Steps(boolPreserveLifts: false);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                cj_Populate_Jumps();
                cj_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void button_cj_jump_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intFromWeight;
            try
            {
                intFromWeight = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intFromWeight < 1) 
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return; 
            }
            else if (cj_Jump_Exists(intFromWeight, intI)) 
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return; 
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_cj_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_FromWeight] = intFromWeight;
                    break;
                }
            }

            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void button_cj_jump_Jump_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intJump;
            try
            {
                intJump = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intJump < 1) 
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return; 
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_cj_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Jump] = intJump;
                    break;
                }
            }

            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private bool cj_Jump_Exists(int intFromWeight, int intExcludeId)
        {
            foreach (DataRow dataRow in dt_cj_jumps.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == intFromWeight & dataRow.Field<int>(str_col_Id) != intExcludeId) { return true; }
            }
            return false;
        }

        #endregion

        #region cj times
        private void Insert_Default_cj_Times(DataTable dataTable)
        {
            DataRow dataRow;

            dataTable.Rows.Clear();

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 1;
            dataRow[str_col_Length] = 5 * 60;
            dataTable.Rows.Add(dataRow);

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 21;
            dataRow[str_col_Length] = 140;
            dataTable.Rows.Add(dataRow);

            dataRow = dataTable.NewRow();
            dataRow[str_col_FromWeight] = 100;
            dataRow[str_col_Length] = 150;
            dataTable.Rows.Add(dataRow);

            dataTable.AcceptChanges();
        }
        private void cj_Populate_Times()
        {
            cj_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intTime = 1;
            panel_cj_time.Controls.Clear();

            if (dt_cj_times.Rows.Count == 0)
            {
                Insert_Default_cj_Times(dt_cj_times);
            }

            dt_cj_times.DefaultView.Sort = str_col_FromWeight + " ASC";
            foreach (DataRow dataRow in dt_cj_times.DefaultView.ToTable().Rows)
            {
                intFromWeight = dataRow.Field<int>(str_col_FromWeight);
                intTime = dataRow.Field<int>(str_col_Length);
                cj_Add_Time_IndividualControls(
                    intY
                    , dataRow.Field<int>(str_col_Id)
                    , intFromWeight
                    , intTime
                    , false
                    );
                intY += 30;
            }
            cj_Add_Time_IndividualControls(
                intY
                , -1
                , intFromWeight + 1
                , intTime
                , true
                );
        }
        private void cj_Add_Time_IndividualControls(
            int intY
            , int intId
            , int intFromWeight
            , int intTime
            , bool bool_Add_Blank
            )
        {
            NumericUpDown nmud1 = new()
            {
                Location = new Point(6, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intFromWeight, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            NumericUpDown nmud2 = new()
            {
                Location = new Point(100, intY),
                Maximum = new decimal(new int[] { 9999, 0, 0, 0 }),
                Minimum = new decimal(new int[] { 1, 0, 0, 0 }),
                Size = new Size(72, 25),
                TextAlign = HorizontalAlignment.Center,
                Value = new decimal(new int[] { intTime, 0, 0, 0 }),
                Tag = intId,
                BackColor = Color.White
            };
            nmud1.ValueChanged += button_cj_time_FromWeight_ValueChanged;
            nmud2.ValueChanged += button_cj_time_Time_ValueChanged;
            panel_cj_time.Controls.AddRange(new Control[] { nmud1, nmud2 });

            if (bool_Add_Blank)
            {
                Button btn4 = new()
                {
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    Location = new Point(200, intY),
                    Size = new Size(90, 25),
                    Text = str_buttontext_commit,
                    UseVisualStyleBackColor = true
                };
                btn4.Click += button_cj_time_commit_click;
                panel_cj_time.Controls.Add(btn4);

                nmud1.Select();
            }
            else
            {
                if (intFromWeight > 1)
                {
                    Button btn1 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(36, 25),
                        Text = str_buttontext_delete,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    btn1.Click += button_cj_time_delete_click;
                    panel_cj_time.Controls.Add(btn1);
                }
            }
        }
        private void button_cj_time_delete_click(object sender, EventArgs e)
        {
            int intI = (int)(((Button)(sender)).Tag);

            for (int i = 0; i < dt_cj_times.Rows.Count; i++)
            {
                DataRow dataRow = dt_cj_times.Rows[i];
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    if (dataRow.Field<int>(str_col_FromWeight) > 1)
                    {
                        dt_cj_times.Rows.RemoveAt(i);
                        break;
                    }
                }
            }

            cj_Populate_Times();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_time_commit_click(object sender, EventArgs e)
        {
            int intFromWeight = -1;
            int intTime = -1;

            foreach (Control ctrl in panel_cj_time.Controls)
            {
                if (intFromWeight > -1 & intTime > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(((NumericUpDown)ctrl).Tag) == -1)
                    {
                        if (((NumericUpDown)ctrl).Left < 10)
                        {
                            try
                            {
                                intFromWeight = (int)(((NumericUpDown)ctrl).Value);
                                if (intFromWeight < 1)
                                {
                                    MessageBox.Show("From Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse From Weight");
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                intTime = (int)(((NumericUpDown)ctrl).Value);
                                if (intTime < 1)
                                {
                                    MessageBox.Show("Time Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse Time Weight");
                                return;
                            }
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intTime > 0)
            {
                if (cj_Time_Exists(intFromWeight, -1))
                {
                    MessageBox.Show("From Weight - Time already exists");
                    return;
                }
                else
                {
                    DataRow dataRow = dt_cj_times.NewRow();
                    dataRow[str_col_FromWeight] = intFromWeight;
                    dataRow[str_col_Length] = intTime;
                    dt_cj_times.Rows.Add(dataRow);
                    cj_Populate_Times();
                    cj_Populate_Steps(boolPreserveLifts: true);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                cj_Populate_Times();
                cj_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void button_cj_time_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intFromWeight;
            try
            {
                intFromWeight = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intFromWeight < 1)
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }
            else if (cj_Time_Exists(intFromWeight, intI))
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_cj_times.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_FromWeight] = intFromWeight;
                    break;
                }
            }

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_time_Time_ValueChanged(object sender, EventArgs e)
        {
            int intI = (int)(((NumericUpDown)(sender)).Tag);

            if (intI < 1) { return; }

            int intTime;
            try
            {
                intTime = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intTime < 1)
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }
            ((NumericUpDown)sender).BackColor = Color.White;

            foreach (DataRow dataRow in dt_cj_times.Rows)
            {
                if (dataRow.Field<int>(str_col_Id) == intI)
                {
                    dataRow[str_col_Length] = intTime;
                    break;
                }
            }

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private bool cj_Time_Exists(int intFromWeight, int intExcludeId)
        {
            foreach (DataRow dataRow in dt_cj_times.Rows)
            {
                if (dataRow.Field<int>(str_col_FromWeight) == intFromWeight & dataRow.Field<int>(str_col_Id) != intExcludeId) { return true; }
            }
            return false;
        }

        #endregion

        #region cj Steps
        private void cj_Populate_Steps(bool boolPreserveLifts)
        {
            cj_Stop_Live();
            int intY = 1;
            bool boolHasOverrides = false;
            panel_cj_steps.Controls.Clear();

            dt_cj_PLAN = datatable_cj_Steps(
                boolPreserveLifts: boolPreserveLifts,
                datatableIn: dt_cj_PLAN);
            if (dt_cj_PLAN == null) { return; }
            dt_cj_PLAN.DefaultView.Sort = str_col_Order + " ASC";
            foreach (DataRow dataRow in dt_cj_PLAN.DefaultView.ToTable().Rows)
            {
                if (!boolHasOverrides)
                {
                    if (dataRow.Field<bool>(str_col_Override))
                    {
                        boolHasOverrides = true;
                    }
                }
                if (!dataRow.Field<bool>(str_col_PreStep))
                {
                    cj_Add_Step_IndividualControls(
                        intY: intY
                        , strAction: dataRow.Field<string>(str_col_Action)
                        , intWeight: dataRow.Field<int>(str_col_Weight)
                        , intSeconds: dataRow.Field<int>(str_col_Length)
                        , intTotalSeconds: dataRow.Field<int>(str_col_TotalLength)
                        , boolOverride: dataRow.Field<bool>(str_col_Override)
                    );
                    intY += 30;
                }
            }
            Button btn = new()
            {
                Location = new Point(6, intY),
                Size = new Size(50, 28),
                Text = "+"
            };
            btn.Click += cj_Step_Add;
            panel_cj_steps.Controls.Add(btn);
            if (boolHasOverrides)
            {
                Button btn2 = new()
                {
                    Location = new Point(70, intY),
                    Size = new Size(100, 28),
                    Text = "reset overrides"
                };
                btn2.Click += cj_Step_ResetOverrides;
                panel_cj_steps.Controls.Add(btn2);
            }
            label_cj_Setup_StepCount.Text = (dt_cj_PLAN.Rows.Count - 1).ToString() + " steps";
        }
        private void cj_Add_Step_IndividualControls(
            int intY
            , string strAction
            , int intWeight
            , int intSeconds
            , int intTotalSeconds
            , bool boolOverride
            )
        {
            Label lbl1 = new()
            {
                Location = new Point(6, intY),
                AutoSize = false,
                Size = new Size(150,28),
                Text = strAction,
                Tag = intWeight
            };
            Label lbl3 = new()
            {
                Location = new Point(226, intY),
                AutoSize = false,
                Size = new Size(90, 28),
                Text = Seconds_To_String(intSeconds),
                Tag = intWeight
            };
            Label lbl4 = new()
            {
                Location = new Point(317, intY),
                AutoSize = false,
                Size = new Size(90, 28),
                Text = Seconds_To_String(intTotalSeconds),
                Tag = intWeight
            };
            panel_cj_steps.Controls.AddRange(new Control[] { lbl1, lbl3, lbl4 });

            if (intWeight > 0)
            {
                lbl1.Click += cj_Weight_Override_Click;
                lbl3.Click += cj_Weight_Override_Click;
                lbl4.Click += cj_Weight_Override_Click;
                Label lbl2 = new()
                {
                    Location = new Point(152, intY),
                    AutoSize = false,
                    Size = new Size(50, 28),
                    Text = intWeight.ToString(),
                    Tag = intWeight
                };
                lbl2.Click += cj_Weight_Override_Click;
                if (boolOverride)
                {
                    Font fontx = new("Gadugi", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lbl1.Font = fontx;
                    lbl2.Font = fontx;
                    lbl3.Font = fontx;
                    lbl4.Font = fontx;
                }
                panel_cj_steps.Controls.Add(lbl2);
            }
        }
        private DataTable datatable_cj_Steps(
            bool boolPreserveLifts,
            DataTable datatableIn = null)
        {
            return datatable_X_Steps(
                boolPreserveLifts: boolPreserveLifts,
                dt_x_extras: dt_cj_extras,
                dt_x_jumps: dt_cj_jumps,
                dt_x_times: dt_cj_times,
                int_x_Sec_End: int_cj_Sec_End,
                int_x_Wgt_Opener: int_cj_Wgt_Opener,
                bool_Opener_in_Warmup: bool_cj_OpenerWarmup,
                datatableIn: datatableIn);
        }
        private void cj_Step_Add(object sender, EventArgs e)
        {
            if (bool_cj_Live)
            {
                cj_Stop_Live();
            }

            if (dt_cj_PLAN != null)
            {
                if (dt_cj_PLAN.Rows.Count > 2)
                {
                    if (!dt_cj_PLAN.Rows[dt_cj_PLAN.Rows.Count - 1].IsNull(str_col_Weight))
                    {
                        if (dt_cj_PLAN.Rows[dt_cj_PLAN.Rows.Count - 1].Field<int>(str_col_Weight) > 0)
                        {
                            int int_RowIndex = dt_cj_PLAN.Rows.Count - 1;
                            string strNewWeight = dt_cj_PLAN.Rows[int_RowIndex].Field<int>(str_col_Weight).ToString();
                            ShowInputDialog(ref strNewWeight);
                            if (int.TryParse(strNewWeight, out int intNewWeight))
                            {
                                bool boolFound = false;
                                foreach (DataRow dR in dt_cj_PLAN.Rows)
                                {
                                    if (!dR.IsNull(str_col_Weight))
                                    {
                                        if (dR.Field<int>(str_col_Weight) == intNewWeight)
                                        {
                                            boolFound = true;
                                            break;
                                        }
                                    }
                                }
                                if (boolFound)
                                {
                                    MessageBox.Show(intNewWeight.ToString() + " is already a step");
                                }
                                else
                                {
                                    DataRow dRNew = dt_cj_PLAN.NewRow();
                                    dRNew[str_col_Action] = "Lift";
                                    dRNew[str_col_Weight] = intNewWeight;
                                    dRNew[str_col_PreStep] = false;
                                    dRNew[str_col_Override] = true;
                                    dt_cj_PLAN.Rows.Add(dRNew);
                                    dt_cj_PLAN.AcceptChanges();

                                    cj_Populate_Steps(boolPreserveLifts: true);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void cj_Step_ResetOverrides(object sender, EventArgs e)
        {
            if (bool_cj_Live)
            {
                cj_Stop_Live();
            }
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void cj_Weight_Override_Click(object sender, EventArgs e)
        {
            int intStartWeight = 0;
            Label _labelI = (Label)sender;
            try
            {
                intStartWeight = (int)_labelI.Tag;
            }
            catch { }

            if (intStartWeight > 0 & dt_cj_PLAN != null)
            {
                foreach (DataRow dR in dt_cj_PLAN.Rows)
                {
                    if (!dR.IsNull(str_col_Weight))
                    {
                        if (dR.Field<int>(str_col_Weight) == intStartWeight)
                        {
                            if (bool_cj_Live)
                            {
                                cj_Stop_Live();
                            }
                            string strNewWeight = intStartWeight.ToString();
                            if (ShowInputDialog(ref strNewWeight) == DialogResult.OK)
                            {
                                if (int.TryParse(strNewWeight, out int intNewWeight))
                                {
                                    if (intNewWeight != intStartWeight)
                                    {
                                        bool boolFound = false;
                                        foreach (DataRow dRCheck in dt_cj_PLAN.Rows)
                                        {
                                            if (!dRCheck.IsNull(str_col_Weight))
                                            {
                                                if (dRCheck.Field<int>(str_col_Weight) == intNewWeight)
                                                {
                                                    boolFound = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (boolFound)
                                        {
                                            MessageBox.Show(intNewWeight.ToString() + " is already a step");
                                        }
                                        else
                                        {
                                            dR[str_col_Weight] = intNewWeight;
                                            dR[str_col_Override] = true;
                                            cj_Populate_Steps(boolPreserveLifts: true);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region cj LIVE
        private void cj_Stop_Live()
        {
            bool_cj_Live = false;
            if (timer_cj_Live != null)
            {
                timer_cj_Live.Enabled = false;
                try
                {
                    timer_cj_Live.Tick -= timer_cj_Live_Tick;
                }
                catch { }
                timer_cj_Live = null;
            }
            label_cj_Live_TimeTillOpener.Text = String.Empty;
            textBox_cj_Live_snLeft.Visible = false;
            textBox_cj_Live_LiftsOut.Visible = false;
            label_cj_Live_LiftsPassed.Text = string.Empty;
            Clear_cj_Live_Steps();
            progressBar_cj_Live_StageLift.Value = 0;
            progressBar_cj_Live_sn.Value = 0;
            progressBar_cj_Live_Break.Value = 0;
            label_cj_Live_Break.Text = String.Empty;
            button_cj_Live_StageAdvance.BackColor = color_cj_Live_BG;
            button_cj_Live_StageAdvance.Tag = 0;
            button_cj_Live_snStageAdvance.BackColor = color_cj_Live_BG;
            button_cj_Live_snStageAdvance.Tag = 0;
            bool_cj_LiveLifting = false;
            bool_cj_BreakRunning = false;
            bool_cj_sn_Lifting = false;
            button_cj_Live_StartStop.Text = "start";
            panel_cj_Live_Times.Visible = false;
            panel_Battery.Visible = false;
            if (!bool_snatch_Live) { AllowMonitorPowerdown(); }
        }
        private void cj_Start_Live()
        {
            bool_cj_Live = true;
            if (timer_cj_Live != null)
            {
                if (timer_cj_Live.Enabled)
                {
                    timer_cj_Live.Enabled = false; 
                    try
                    {
                        timer_cj_Live.Tick -= timer_cj_Live_Tick;
                    } catch { }
                }
            }
            textBox_cj_Live_snLeft.Visible = false;
            int_cj_Lifts_Passed = 0;
            textBox_cj_Live_LiftsOut.Visible = false;
            label_cj_Live_LiftsPassed.Text = "0";
            Populate_cj_Live_Steps();
            progressBar_cj_Live_StageLift.Value = 0;
            progressBar_cj_Live_StageLift.Maximum = int_cj_Sec_Stage;
            progressBar_cj_Live_sn.Value = 0;
            progressBar_cj_Live_sn.Maximum = int_snatch_Sec_Stage;
            progressBar_cj_Live_Break.Value = 0;
            progressBar_cj_Live_Break.Maximum = int_cj_Sec_Break;
            label_cj_Live_Break.Text = String.Empty;
            label_cj_Live_TimeTillOpener.Text = String.Empty;
            sim_timer_cj_Live_Tick();
            timer_cj_Live = new Timer { Interval = 1000 };
            timer_cj_Live.Tick += timer_cj_Live_Tick;
            timer_cj_Live.Start();
            button_cj_Live_StartStop.Text = "stop";
            button_cj_Live_StageAdvance.Select();
            PreventMonitorPowerdown();
        }
        private void button_cj_Live_StartStop_Click(object sender, EventArgs e)
        {
            if (bool_cj_Live)
            {
                cj_Stop_Live();
            }
            else
            {
                cj_Start_Live();
            }
        }
        private void Clear_cj_Live_Steps()
        {
            dt_cj_LIVE = null;
            panel_cj_Live_Steps.Controls.Clear();
        }
        private void Populate_cj_Live_Steps()
        {
            Clear_cj_Live_Steps();

            if (dt_cj_PLAN is null)
            {
                cj_Populate_Steps(boolPreserveLifts: false);
            }
            if (dt_cj_PLAN is null)
            {
                MessageBox.Show("An error has occurred. Step plan could not be determined.");
                this.Close();
                return;
            }
            dt_cj_LIVE = dt_cj_PLAN.Copy();
            dt_cj_LIVE.Columns.AddRange(
                new DataColumn[]
                {
                    new DataColumn(str_col_PanelLiveStep, typeof(Panel))
                    , new DataColumn(str_col_LabelAction, typeof(Label))
                    , new DataColumn(str_col_LabelTime, typeof(Label))
                    , new DataColumn(str_col_LabelProgressTime, typeof(Label))
                    , new DataColumn(str_col_LabelWeight, typeof(Label))
                    , new DataColumn(str_col_ProgressBarStep, typeof(ProgressBar))
                    , new DataColumn(str_col_GraphicPanel, typeof(Panel))
                });

            int intY = 1;
            int _int_panel_Live_Step_Width = panel_cj_Live_Steps.Width - 4;
            int _int_progressBar_Step_Width_NoScroll = _int_panel_Live_Step_Width - 350;
            int _int_progressBar_Step_Width_Scroll = _int_progressBar_Step_Width_NoScroll - SystemInformation.VerticalScrollBarWidth;
            int _int_progressBar_Step_Width;
            Point _point_progressBar_Step_Location = new(300, 6);

            SuspendLayout();
            foreach (DataRow dataRow in dt_cj_LIVE.Rows)
            {
                if (panel_cj_Live_Steps.VerticalScroll.Visible)
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_Scroll;
                }
                else
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_NoScroll;
                }
                string strActionText;
                bool boolIsLift = (dataRow.Field<int>(str_col_Weight) > 0);
                if (boolIsLift)
                {
                    strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString() + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                        Environment.NewLine +
                        "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                }
                else
                {
                    strActionText = dataRow.Field<string>(str_col_Action) + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                        Environment.NewLine +
                        "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                }
                Panel panel_Live_Step = new()
                {
                    Size = new Size(_int_panel_Live_Step_Width, 80),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = color_cj_Live_BG,
                    ForeColor = color_Live_Default_FG,
                    Location = new Point(1, intY)
                };
                Label label_Action = new()
                {
                    Text = strActionText,
                    AutoSize = false,
                    Size = new Size(280, 75),
                    TextAlign = ContentAlignment.TopRight,
                    Location = new Point(6, 1)
                };
                ProgressBar progressBar_Step = new()
                {
                    Size = new Size(_int_progressBar_Step_Width, 65),
                    Location = _point_progressBar_Step_Location,
                    Maximum = dataRow.Field<int>(str_col_Length),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                if (progressBar_Step.Maximum == 0)
                {
                    progressBar_Step.Maximum = 1;
                    progressBar_Step.Value = 1;
                }
                Label label_Weight = null;
                if (boolIsLift)
                {
                    label_Weight = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.Black,
                        AutoSize = false,
                        Size = new Size(105, 30),
                        Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width - 105, 6),
                        ForeColor = SystemColors.Window,
                        Text = "lift " + dataRow.Field<int>(str_col_Weight).ToString(),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    };
                    label_Weight.Text = "lift " + dataRow.Field<int>(str_col_Weight).ToString();

                    if (dataRow.Field<bool>(str_col_Override))
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Italic);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold | FontStyle.Italic);
                    }
                    else
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Regular);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold);
                    }
                }
                Label label_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(120, 30),
                    Location = _point_progressBar_Step_Location,
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                };
                Label label_Progress_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(105, 30),
                    Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width - 105, 6),
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                };
                panel_Live_Step.Controls.AddRange(new Control[]
                {
                    label_Action,
                    progressBar_Step,
                    label_Progress_Time,
                    label_Time
                });
                WeightBox weightBoxGraphic = null;
                if (boolIsLift)
                {
                    weightBoxGraphic = new()
                    {
                        boolOpener = false,
                        intWeightBar = int_Barbell,
                        intWeight = dataRow.Field<int>(str_col_Weight),
                        intShadowWidth = 1,
                        intPlateGap = -1,
                        Size = new Size(120, 65),
                        BackColor = Color.FromArgb(128, 128, 128),
                        BorderStyle = BorderStyle.Fixed3D,
                        Location = new Point(10, 6),
                        Visible = true
                    };
                    weightBoxGraphic.Paint += Apply_Vector_Weight_Graphic;
                    panel_Live_Step.Controls.Add(weightBoxGraphic);
                }
                progressBar_Step.BringToFront();
                dataRow[str_col_PanelLiveStep] = panel_Live_Step;
                dataRow[str_col_LabelAction] = label_Action;
                dataRow[str_col_ProgressBarStep] = progressBar_Step;
                weightBoxGraphic?.BringToFront();
                if (label_Weight != null)
                {
                    panel_Live_Step.Controls.Add(label_Weight);
                    dataRow[str_col_LabelWeight] = label_Weight;
                    label_Weight.BringToFront();
                }
                label_Time.BringToFront();
                dataRow[str_col_LabelTime] = label_Time;
                label_Progress_Time.BringToFront();
                dataRow[str_col_LabelProgressTime] = label_Progress_Time;
                panel_cj_Live_Steps.Controls.Add(panel_Live_Step);

                intY += 81;
            }
            dt_cj_LIVE.AcceptChanges();
            int_cj_Warmup_Step = -1;
            label_cj_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
            panel_cj_Live_Times.Visible = true;
            ResumeLayout();
        }
        private void timer_cj_Live_Tick(object sender, EventArgs e)
        {
            sim_timer_cj_Live_Tick();
        }
        private void sim_timer_cj_Live_Tick()
        {
            DateTime _dateTime_Now = DateTime.Now;
            int intSecondsToOpen = 0;

            if (propertyData_Battery is null)
            {
                System.Management.ObjectQuery query = new("Select * FROM Win32_Battery");
                ManagementObjectSearcher searcher = new(query);
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject mo in collection.Cast<ManagementObject>())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        if (property.Name == "EstimatedChargeRemaining")
                        {
                            propertyData_Battery = property;
                        }
                    }
                }
            }
            if (propertyData_Battery is not null)
            {
                System.UInt16 _uInt_Battery = 100;
                try
                {
                    _uInt_Battery = (System.UInt16)propertyData_Battery.Value;
                }
                catch { }
                if (_uInt_Battery > 0 && _uInt_Battery < 100)
                {
                    panel_Battery.Visible = true;
                    progressBar_Battery.Value = _uInt_Battery;
                    label_Battery_Percentage.Text = _uInt_Battery.ToString() + "%";
                }
                else
                {
                    panel_Battery.Visible = false;
                }
            }
            else
            {
                panel_Battery.Visible = false;
            }

            if (bool_cj_sn_Lifting) // snatches still running
            {
                if (int_cj_snLifts_Out == 0)
                {
                    bool_cj_sn_Lifting = false;
                    bool_cj_BreakRunning = true;
                    progressBar_cj_Live_Break.Value = 0;
                    bool_cj_LiveLifting = false;
                }
                else
                {
                    progressBar_cj_Live_sn.PerformStep();
                    if (bool_cj_AutoAdvance)
                    {
                        if (progressBar_cj_Live_sn.Value == progressBar_cj_Live_sn.Maximum)
                        {
                            cj_Advance_snLift();
                        }
                    }

                    if (int_cj_snLifts_Out == 0)
                    {
                        bool_cj_sn_Lifting = false;
                        progressBar_cj_Live_Break.Value = 0;
                        bool_cj_BreakRunning = true;
                    }
                    else
                    {
                        bool_cj_BreakRunning = false;
                    }
                    bool_cj_LiveLifting = false;
                }
            }
            else if (int_cj_snLifts_Out > 0)
            {
                bool_cj_sn_Lifting = true;
                progressBar_cj_Live_Break.Value = 0;
                bool_cj_BreakRunning = false;
                bool_cj_LiveLifting = false;
            }

            if (bool_cj_BreakRunning) // break is running
            {
                progressBar_cj_Live_Break.PerformStep();
                if (progressBar_cj_Live_Break.Value == progressBar_cj_Live_Break.Maximum)
                {
                    bool_cj_BreakRunning = false;
                    progressBar_cj_Live_StageLift.Value = 0;
                    label_cj_Live_Break.Text = String.Empty;
                    bool_cj_LiveLifting = true;
                }
                else
                {
                    bool_cj_LiveLifting = false;
                    label_cj_Live_Break.Text = Seconds_To_String(progressBar_cj_Live_Break.Maximum - progressBar_cj_Live_Break.Value);
                }
            }
            else if (!bool_cj_sn_Lifting)
            {
                if (progressBar_cj_Live_Break.Value < progressBar_cj_Live_Break.Maximum)
                {
                    bool_cj_BreakRunning = true;
                    progressBar_cj_Live_StageLift.Value = 0;
                    label_cj_Live_Break.Text = Seconds_To_String(0);
                    bool_cj_LiveLifting = false;
                }
            }
            else
            {
                label_cj_Live_Break.Text = String.Empty;
            }
            
            if (bool_cj_LiveLifting) // stage lifts are going
            {
                progressBar_cj_Live_StageLift.PerformStep();
                if (progressBar_cj_Live_StageLift.Value == progressBar_cj_Live_StageLift.Maximum)
                {
                    if (bool_cj_AutoAdvance | int_cj_Lifts_Out == 1)
                    {
                        cj_Advance_StageLift();
                    }
                }

                if (int_cj_Lifts_Out == 0)
                {
                    bool_cj_LiveLifting = false;
                }
            }
            else if (!bool_cj_sn_Lifting & !bool_cj_BreakRunning)
            {
                if (int_cj_Lifts_Out > 0)
                {
                    if (progressBar_cj_Live_StageLift.Value < progressBar_cj_Live_StageLift.Maximum)
                    {
                        bool_cj_LiveLifting = true;
                        progressBar_cj_Live_StageLift.Value = 0;
                    }
                }
            }


            if (bool_cj_sn_Lifting)
            {
                if ((int)button_cj_Live_snStageAdvance.Tag == 0)
                {
                    button_cj_Live_snStageAdvance.Tag = 1;
                    button_cj_Live_snStageAdvance.BackColor = color_AdvanceButton_Active;
                }
            }
            else
            {
                if ((int)button_cj_Live_snStageAdvance.Tag == 1)
                {
                    button_cj_Live_snStageAdvance.Tag = 0;
                    button_cj_Live_snStageAdvance.BackColor = color_cj_Live_BG;
                }
            }

            if (bool_cj_LiveLifting)
            {
                if ((int)button_cj_Live_StageAdvance.Tag == 0)
                {
                    button_cj_Live_StageAdvance.Tag = 1;
                    button_cj_Live_StageAdvance.BackColor = color_AdvanceButton_Active;
                }
            }
            else
            {
                if ((int)button_cj_Live_StageAdvance.Tag == 1)
                {
                    button_cj_Live_StageAdvance.Tag = 0;
                    button_cj_Live_StageAdvance.BackColor = color_cj_Live_BG;
                }
            }


            if (bool_cj_sn_Lifting | bool_cj_BreakRunning | bool_cj_LiveLifting)
            {
                intSecondsToOpen = (int_cj_Lifts_Out - 1) * int_cj_Sec_Stage +
                    progressBar_cj_Live_StageLift.Maximum - progressBar_cj_Live_StageLift.Value;
            }

            if (bool_cj_sn_Lifting | bool_cj_BreakRunning)
            {
                intSecondsToOpen += progressBar_cj_Live_Break.Maximum - progressBar_cj_Live_Break.Value;
            }

            if (bool_cj_sn_Lifting) // snatches still running
            {
                intSecondsToOpen += (int_cj_snLifts_Out - 1) * int_snatch_Sec_Stage +
                    progressBar_cj_Live_sn.Maximum - progressBar_cj_Live_sn.Value;
            }


            if (intSecondsToOpen == 0)
            {
                label_cj_Live_TimeTillOpener.Text = "passed";
            }
            else
            {
                label_cj_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
            }
            DateTime _dateTime_Open = _dateTime_Now.AddSeconds(intSecondsToOpen);
            if (intSecondsToOpen > 0)
            {
                label_cj_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
                label_cj_Live_OpenTime.Text = _dateTime_Open.ToString("HH:mm:ss");
            }
            else
            {
                label_cj_Live_TimeTillOpener.Text = "-";
                label_cj_Live_OpenTime.Text = "passed";
            }

            int _intStep = -1;
            foreach (DataRow dataRow in dt_cj_LIVE.Rows)
            {
                if (dataRow.Field<int>(str_col_TotalLengthReverse) >= intSecondsToOpen)
                {
                    if (dataRow.Field<int>(str_col_Order) > _intStep)
                    {
                        _intStep = dataRow.Field<int>(str_col_Order);
                    }
                }
            }

            if (_intStep == -1) // adjust wait time
            {
                int intTLR = 0;
                foreach (DataRow dataRow in dt_cj_LIVE.Rows)
                {
                    if (dataRow.Field<int>(str_col_Order) == 1)
                    {
                        intTLR = dataRow.Field<int>(str_col_TotalLengthReverse);
                        break;
                    }
                }
                if (intTLR > 0 & intSecondsToOpen > intTLR)
                {
                    int intSecToAdd = 0;
                    foreach (DataRow dataRow in dt_cj_LIVE.Rows)
                    {
                        if (dataRow.Field<bool>(str_col_PreStep))
                        {
                            intSecToAdd = (intSecondsToOpen - intTLR) - dataRow.Field<int>(str_col_Length);
                            dataRow[str_col_Length] = intSecondsToOpen - intTLR;
                            dataRow.Field<ProgressBar>(str_col_ProgressBarStep).Maximum = intSecondsToOpen - intTLR;
                            dataRow[str_col_TotalLength] = intSecondsToOpen - intTLR;
                            dataRow[str_col_TotalLengthReverse] = intSecondsToOpen;
                            _intStep = 0;
                            break;
                        }
                    }
                    if (_intStep == 0 & intSecToAdd != 0)
                    {
                        foreach (DataRow dataRow in dt_cj_LIVE.Rows)
                        {
                            if (!dataRow.Field<bool>(str_col_PreStep))
                            {
                                dataRow[str_col_TotalLength] = dataRow.Field<int>(str_col_TotalLength) + intSecToAdd;
                            }
                        }
                    }
                }
            }

            if (_intStep > -1)
            {
                Panel panel_Live_Step = null;
                Label label_Action;
                ProgressBar progressBar_Step;
                Label label_Progress_Time;
                bool boolUpdBGsFGs = (_intStep != int_cj_Warmup_Step);

                foreach (DataRow dataRow in dt_cj_LIVE.Rows)
                {
                    int _int_Order = dataRow.Field<int>(str_col_Order);
                    Label label_Time = dataRow.Field<Label>(str_col_LabelTime);
                    if (_int_Order > _intStep)
                    {
                        label_Time.Visible = true;
                        label_Time.Text = _dateTime_Open.AddSeconds(-dataRow.Field<int>(str_col_TotalLengthReverse)).ToString("HH:mm:ss");
                    }
                    else
                    {
                        label_Time.Visible = false;
                    }
                }

                foreach (DataRow dataRow in dt_cj_LIVE.Rows)
                {
                    int _int_Order = dataRow.Field<int>(str_col_Order);
                    if (_int_Order == _intStep)
                    {
                        panel_Live_Step = dataRow.Field<Panel>(str_col_PanelLiveStep);
                        label_Progress_Time = dataRow.Field<Label>(str_col_LabelProgressTime);
                        progressBar_Step = dataRow.Field<ProgressBar>(str_col_ProgressBarStep);

                        int intStepLength = dataRow.Field<int>(str_col_Length);
                        int intSecIntoStep = dataRow.Field<int>(str_col_TotalLengthReverse) - intSecondsToOpen;
                        label_Progress_Time.Text = Seconds_To_String(_int_Seconds: intStepLength - intSecIntoStep, _bool_ShortString: true);
                        progressBar_Step.Value = intSecIntoStep;

                        if (boolUpdBGsFGs)
                        {
                            string strActionText;
                            bool boolIsLift = (dataRow.Field<int>(str_col_Weight) > 0);
                            if (boolIsLift)
                            {
                                strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString();
                            }
                            else
                            {
                                strActionText = dataRow.Field<string>(str_col_Action);
                            }
                            label_Action = dataRow.Field<Label>(str_col_LabelAction);
                            label_Action.Text = strActionText;
                            label_Progress_Time.Visible = true;
                        }

                        if (boolUpdBGsFGs)
                        {
                            panel_Live_Step.BackColor = color_Live_Highlight_BG;
                            panel_Live_Step.ForeColor = color_Live_Highlight_FG;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (boolUpdBGsFGs)
                    {
                        panel_Live_Step = dataRow.Field<Panel>(str_col_PanelLiveStep);
                        panel_Live_Step.BackColor = color_cj_Live_BG;
                        panel_Live_Step.ForeColor = color_Live_Default_FG;
                        label_Progress_Time = dataRow.Field<Label>(str_col_LabelProgressTime);
                        label_Progress_Time.Visible = false;
                        progressBar_Step = dataRow.Field<ProgressBar>(str_col_ProgressBarStep);
                        label_Action = dataRow.Field<Label>(str_col_LabelAction);
                        string strActionText;
                        bool boolIsLift = (dataRow.Field<int>(str_col_Weight) > 0);

                        if (_int_Order < _intStep)
                        {
                            progressBar_Step.Value = progressBar_Step.Maximum;
                            if (boolIsLift)
                            {
                                strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString();
                            }
                            else
                            {
                                strActionText = dataRow.Field<string>(str_col_Action);
                            }
                        }
                        else
                        {
                            progressBar_Step.Value = 0;
                            if (boolIsLift)
                            {
                                strActionText = "lift " + dataRow.Field<int>(str_col_Weight).ToString() + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                                    Environment.NewLine +
                                    "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                            }
                            else
                            {
                                strActionText = dataRow.Field<string>(str_col_Action) + " (" + Seconds_To_String(dataRow.Field<int>(str_col_Length)) + ")" +
                                    Environment.NewLine +
                                    "from " + Seconds_To_String(dataRow.Field<int>(str_col_TotalLengthReverse)) + " out";
                            }
                        }

                        label_Action.Text = strActionText;
                    }
                }
                int_cj_Warmup_Step = _intStep;
                if (boolUpdBGsFGs & bool_Beep)
                {
                    Console.Beep(750, 600);
                }
            }
            else
            {
                cj_Stop_Live();
            }

            label_cj_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void progressBar_cj_Live_StageLift_MouseClick(object sender, MouseEventArgs e)
        {
            if (bool_cj_Live & bool_cj_LiveLifting)
            {
                double dbl_Percent = (double)(e.X) / (double)(progressBar_cj_Live_StageLift.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_cj_Live_StageLift.Value = (int)((double)progressBar_cj_Live_StageLift.Maximum * dbl_Percent);
                }
            }
        } 
        private void progressBar_cj_Live_sn_MouseClick(object sender, MouseEventArgs e)
        {
            if (bool_cj_Live & bool_cj_sn_Lifting)
            {
                double dbl_Percent = (double)(e.X) / (double)(progressBar_cj_Live_sn.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_cj_Live_sn.Value = (int)((double)progressBar_cj_Live_sn.Maximum * dbl_Percent);
                }
            }
        } 
        private void progressBar_cj_Live_Break_MouseClick(object sender, MouseEventArgs e)
        {
            if (bool_cj_Live & bool_cj_BreakRunning)
            {
                double dbl_Percent = (double)(e.X) / (double)(progressBar_cj_Live_Break.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_cj_Live_Break.Value = (int)((double)progressBar_cj_Live_Break.Maximum * dbl_Percent);
                }
            }
        } 
        private void button_cj_Live_LiftsDecr_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out > 0)
            {
                int_cj_Lifts_Out--;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
                if (int_cj_Lifts_Out == 0)
                {
                    if (bool_cj_LiveLifting)
                    {
                        bool_cj_LiveLifting = false;
                        progressBar_cj_Live_StageLift.Value = 0;
                    }
                }
            }
        }
        private void button_cj_Live_LiftsIncr_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out < 99)
            {
                int_cj_Lifts_Out++;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            }
        }
        private void label_cj_Live_LiftsOut_Click(object sender, EventArgs e)
        {
            bool_Loading = true;
            textBox_cj_Live_LiftsOut.Location = label_cj_Live_LiftsOut.Location;
            textBox_cj_Live_LiftsOut.Size = label_cj_Live_LiftsOut.Size;
            textBox_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            textBox_cj_Live_LiftsOut.Visible = true;
            textBox_cj_Live_LiftsOut.BringToFront();
            textBox_cj_Live_LiftsOut.Select();
            bool_Loading = false;
        }
        private void textBox_cj_Live_LiftsOut_TextChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_Lifts_Out) &&
                _int_cj_Lifts_Out >= 0 &&
                _int_cj_Lifts_Out < 100 &&
                int_cj_Lifts_Out != _int_cj_Lifts_Out)
            {
                int_cj_Lifts_Out = _int_cj_Lifts_Out;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            }
        }
        private void textBox_cj_Live_LiftsOut_Leave(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_Lifts_Out) &&
                _int_cj_Lifts_Out >= 0 &&
                _int_cj_Lifts_Out < 100 &&
                int_cj_Lifts_Out != _int_cj_Lifts_Out)
            {
                int_cj_Lifts_Out = _int_cj_Lifts_Out;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            }
            textBox_cj_Live_LiftsOut.Visible = false;
        }
        private void button_cj_Live_StageAdvance_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out >= 0 & bool_cj_LiveLifting)
            {
                cj_Advance_StageLift();
            }
            else if (int_cj_snLifts_Out >= 0 & bool_cj_sn_Lifting)
            {
                cj_Advance_snLift();
            }
        }
        private void cj_Advance_StageLift()
        {
            if (int_cj_Lifts_Out > 0)
            {
                int_cj_Lifts_Out--;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
                if (bool_cj_Live && !bool_cj_sn_Lifting && !bool_cj_BreakRunning)
                {
                    int_cj_Lifts_Passed++;
                }
                else
                {
                    int_cj_Lifts_Passed = 0;
                }
                label_cj_Live_LiftsPassed.Text = (bool_cj_Live ? int_cj_Lifts_Passed.ToString() : string.Empty);
            }
            progressBar_cj_Live_StageLift.Value = 0;
        }
        private void checkBox_cj_Live_Auto_CheckedChanged(object sender, EventArgs e)
        {
            bool_cj_AutoAdvance = checkBox_cj_Live_Auto.Checked;
        }
        private void numericUpDown_cj_Live_Break_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            int _int_cj_Sec_Break = (int)(numericUpDown_cj_Live_Break.Value);
            if (_int_cj_Sec_Break < 1)
            {
                int_cj_Sec_Break = 1; 
            }
            else
            {
                int_cj_Sec_Break = _int_cj_Sec_Break * 60;
            }

            if (progressBar_cj_Live_Break.Value > int_cj_Sec_Break)
            { progressBar_cj_Live_Break.Value = int_cj_Sec_Break; }
            progressBar_cj_Live_Break.Maximum = int_cj_Sec_Break;
        }
        private void button_cj_Live_snDecr_Click(object sender, EventArgs e)
        {
            if (int_cj_snLifts_Out > 0)
            {
                int_cj_snLifts_Out--;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
                if (int_cj_snLifts_Out == 0)
                {
                    if (bool_cj_sn_Lifting)
                    {
                        bool_cj_sn_Lifting = false;
                        progressBar_cj_Live_sn.Value = 0;
                    }
                }
            }
        }
        private void button_cj_Live_snIncr_Click(object sender, EventArgs e)
        {
            if (int_cj_snLifts_Out < 98)
            {
                int_cj_snLifts_Out++;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
        }
        private void label_cj_Live_snLeft_Click(object sender, EventArgs e)
        {
            bool_Loading = true;
            textBox_cj_Live_snLeft.Location = label_cj_Live_snLeft.Location;
            textBox_cj_Live_snLeft.Size = label_cj_Live_snLeft.Size;
            textBox_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            textBox_cj_Live_snLeft.Visible = true;
            textBox_cj_Live_snLeft.BringToFront();
            textBox_cj_Live_snLeft.Select();
            bool_Loading = false;
        }
        private void textBox_cj_Live_snLeft_TextChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_snLeft.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_snLifts_Out) &&
                _int_cj_snLifts_Out >= 0 &&
                _int_cj_snLifts_Out < 100 &&
                int_cj_snLifts_Out != _int_cj_snLifts_Out)
            {
                int_cj_snLifts_Out = _int_cj_snLifts_Out;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
        }
        private void textBox_cj_Live_snLeft_Leave(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_snLeft.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_snLifts_Out) &&
                _int_cj_snLifts_Out >= 0 &&
                _int_cj_snLifts_Out < 100 &&
                int_cj_snLifts_Out != _int_cj_snLifts_Out)
            {
                int_cj_snLifts_Out = _int_cj_snLifts_Out;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
            textBox_cj_Live_snLeft.Visible = false;
        }
        private void button_cj_Live_snStageAdvance_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out >= 0 & bool_cj_LiveLifting)
            {
                cj_Advance_StageLift();
            }
            else if (int_cj_snLifts_Out >= 0 & bool_cj_sn_Lifting)
            {
                cj_Advance_snLift();
            }
        }
        private void cj_Advance_snLift()
        {
            if (int_cj_snLifts_Out > 0)
            {
                int_cj_snLifts_Out--;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
            progressBar_cj_Live_sn.Value = 0;
        }
        private void splitContainer_cj_DoubleClick(object sender, EventArgs e)
        {
            splitContainer_cj.SplitterDistance = 0;
        }

        #endregion

        #region utilities
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
        private void Smooth_Last_Jumps(ref DataTable dataTable, int intOpener)
        {
            int intLastWarmup = 0, intSecondToLastWarmup = 0, intThirdToLastWarmup = 0;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                if (dataRow.Field<int>(str_col_Weight) > 0)
                {
                    if (dataRow.Field<int>(str_col_Weight) > intLastWarmup)
                    {
                        intThirdToLastWarmup = intSecondToLastWarmup;
                        intSecondToLastWarmup = intLastWarmup;
                        intLastWarmup = dataRow.Field<int>(str_col_Weight);
                    }
                    else if (dataRow.Field<int>(str_col_Weight) > intSecondToLastWarmup)
                    {
                        intThirdToLastWarmup = intSecondToLastWarmup;
                        intSecondToLastWarmup = dataRow.Field<int>(str_col_Weight);
                    }
                    else if (dataRow.Field<int>(str_col_Weight) > intThirdToLastWarmup)
                    {
                        intThirdToLastWarmup = dataRow.Field<int>(str_col_Weight);
                    }
                }
            }

            if (intOpener > 0)
            {
                if (intOpener == intLastWarmup)
                {
                    if (intLastWarmup > 0 & intSecondToLastWarmup > 0 & intThirdToLastWarmup > 0)
                    {
                        if (intLastWarmup - intThirdToLastWarmup > 3)
                        {
                            if (((int)(decimal)(intLastWarmup - intThirdToLastWarmup) / 2 + (decimal)intThirdToLastWarmup) != intSecondToLastWarmup)
                            {
                                foreach (DataRow dataRow in dataTable.Rows)
                                {
                                    if (dataRow.Field<int>(str_col_Weight) == intSecondToLastWarmup)
                                    {
                                        if (!dataRow.Field<bool>(str_col_Override))
                                        {
                                            dataRow[str_col_Weight] = (int)Math.Floor((decimal)(intLastWarmup - intThirdToLastWarmup) / 2 + (decimal)intThirdToLastWarmup);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (intLastWarmup > 0 & intSecondToLastWarmup > 0)
                {
                    if (intOpener - intSecondToLastWarmup > 3)
                    {
                        if (((int)(decimal)(intOpener - intSecondToLastWarmup) / 2 + (decimal)intSecondToLastWarmup) != intLastWarmup)
                        {
                            foreach (DataRow dataRow in dataTable.Rows)
                            {
                                if (dataRow.Field<int>(str_col_Weight) == intLastWarmup)
                                {
                                    if (!dataRow.Field<bool>(str_col_Override))
                                    {
                                        dataRow[str_col_Weight] = (int)Math.Floor((decimal)(intOpener - intSecondToLastWarmup) / 2 + (decimal)intSecondToLastWarmup);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private DataTable datatable_X_Steps(
            bool boolPreserveLifts,
            DataTable dt_x_extras,
            DataTable dt_x_jumps,
            DataTable dt_x_times,
            int int_x_Sec_End,
            int int_x_Wgt_Opener,
            bool bool_Opener_in_Warmup,
            DataTable datatableIn = null
            )
        {
            if (dt_x_extras == null | dt_x_jumps == null | dt_x_times == null) { return null; }
            int _intSeconds
                , intTotalSeconds = 0
                , intOrder = 1
                ;
            DataTable dt = new(),
                dataTableWeight_Input = new(),
                dataTableWeights;

            dt.Columns.AddRange(
                new DataColumn[]
                {
                    new DataColumn(str_col_Action, typeof(string)),
                    new DataColumn(str_col_Weight, typeof(int)),
                    new DataColumn(str_col_Length, typeof(int)),
                    new DataColumn(str_col_TotalLength, typeof(int)),
                    new DataColumn(str_col_TotalLengthReverse, typeof(int)),
                    new DataColumn(str_col_Order, typeof(int)),
                    new DataColumn(str_col_PreStep, typeof(bool)),
                    new DataColumn(str_col_Override, typeof(bool)),
                }
            );

            dataTableWeight_Input.Columns.AddRange(new DataColumn[]
            {
                new DataColumn(str_col_Weight, typeof(int)),
                new DataColumn(str_col_Override, typeof(bool))
            });
            if (boolPreserveLifts & datatableIn != null)
            {
                foreach (DataRow dR in datatableIn.Rows)
                {
                    if (!dR.IsNull(str_col_Weight))
                    {
                        if (dR.Field<int>(str_col_Weight) > 0)
                        {
                            dataTableWeight_Input.Rows.Add(
                                dR.Field<int>(str_col_Weight),
                                dR.Field<bool>(str_col_Override));
                        }
                    }
                }
                //remove final auto populated weights
                for (int i = dataTableWeight_Input.Rows.Count - 1; i >= 0; i--)
                {
                    if (dataTableWeight_Input.Rows[i].Field<bool>(str_col_Override))
                    { break; }
                    else
                    {
                        dataTableWeight_Input.Rows.RemoveAt(i);
                    }
                }
            }
            dataTableWeights = dataTable_Weights(
                dt_x_jumps:dt_x_jumps,
                _dataTableWeight_Input: dataTableWeight_Input,
                bool_Opener_in_Warmup: bool_Opener_in_Warmup,
                int_x_Wgt_Opener:int_x_Wgt_Opener
                );

            dt_x_extras.DefaultView.Sort = str_col_Order + " ASC";
            foreach (DataRow dataRow in dt_x_extras.DefaultView.ToTable().Rows)
            {
                DataRow dR = dt.NewRow();
                _intSeconds = dataRow.Field<int>(str_col_Length);
                intTotalSeconds += _intSeconds;
                dR[str_col_Action] = dataRow.Field<string>(str_col_Action);
                dR[str_col_Weight] = 0;
                dR[str_col_Length] = _intSeconds;
                dR[str_col_TotalLength] = intTotalSeconds;
                dR[str_col_Order] = intOrder;
                dR[str_col_PreStep] = false;
                dR[str_col_Override] = false;
                intOrder++;
                dt.Rows.Add(dR);
            }

            int intWeight;
            int intTime;
            dt_x_times.DefaultView.Sort = str_col_FromWeight;
            using (DataTable dTTimes = dt_x_times.DefaultView.ToTable())
            {
                foreach (DataRow dRWeight in dataTableWeights.Rows)
                {
                    intWeight = dRWeight.Field<int>(str_col_Weight);
                    intTime = 0;
                    foreach (DataRow dataRow in dt_x_times.DefaultView.ToTable().Rows)
                    {
                        if (dataRow.Field<int>(str_col_FromWeight) <= intWeight)
                        {
                            intTime = dataRow.Field<int>(str_col_Length);
                        }
                        else break;
                    }
                    if (dataTableWeights.Rows.IndexOf(dRWeight) == dataTableWeights.Rows.Count - 1)
                    {
                        intTime += int_x_Sec_End;
                    }
                    intTotalSeconds += intTime;
                    DataRow dR = dt.NewRow();
                    dR[str_col_Action] = "Lift";
                    dR[str_col_Weight] = intWeight;
                    dR[str_col_Length] = intTime;
                    dR[str_col_TotalLength] = intTotalSeconds;
                    dR[str_col_Order] = intOrder;
                    dR[str_col_PreStep] = false;
                    dR[str_col_Override] = dRWeight.Field<bool>(str_col_Override);
                    intOrder++;
                    dt.Rows.Add(dR);
                }
            }

            intTotalSeconds = 0;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                _intSeconds = dt.Rows[i].Field<int>(str_col_Length);
                intTotalSeconds += _intSeconds;
                dt.Rows[i][str_col_TotalLengthReverse] = intTotalSeconds;
            }

            Smooth_Last_Jumps(ref dt, int_x_Wgt_Opener);

            DataRow dRPre = dt.NewRow();
            dRPre[str_col_Action] = "wait";
            dRPre[str_col_Weight] = 0;
            dRPre[str_col_Length] = 0;
            dRPre[str_col_TotalLength] = 0;
            dRPre[str_col_TotalLengthReverse] = intTotalSeconds;
            dRPre[str_col_Order] = 0;
            dRPre[str_col_PreStep] = true;
            dRPre[str_col_Override] = false;
            dt.Rows.InsertAt(dRPre, 0);

            return dt;
        }
        private DataTable dataTable_Weights(
            DataTable dt_x_jumps,
            DataTable _dataTableWeight_Input,
            int int_x_Wgt_Opener,
            bool bool_Opener_in_Warmup
            )
        {
            DataTable dt = new();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn(str_col_Weight, typeof(int)),
                new DataColumn(str_col_Override, typeof(bool))
            });

            int intWeight = 0;
            int intJump;
            int intLift = 0;
            bool boolOverride;

            dt_x_jumps.DefaultView.Sort = str_col_FromWeight + " ASC";
            using (DataTable dtJumps = dt_x_jumps.DefaultView.ToTable())
            {
                do
                {
                    intJump = 0;
                    if (intLift < _dataTableWeight_Input.Rows.Count)
                    {
                        intJump = _dataTableWeight_Input.Rows[intLift].Field<int>(str_col_Weight) - intWeight;
                        boolOverride = _dataTableWeight_Input.Rows[intLift].Field<bool>(str_col_Override);
                    }
                    else if (intWeight == 0)
                    {
                        intJump = int_Barbell;
                        boolOverride = false;
                    }
                    else
                    {
                        boolOverride = false;
                        foreach (DataRow dataRow in dtJumps.Rows)
                        {
                            if (dataRow.Field<int>(str_col_FromWeight) <= intWeight)
                            {
                                intJump = dataRow.Field<int>(str_col_Jump);
                            }
                            else if (intJump > 0 & dataRow.Field<int>(str_col_FromWeight) + dataRow.Field<int>(str_col_Jump) <= intWeight + intJump)
                            {
                                intJump = dataRow.Field<int>(str_col_FromWeight) - intWeight + dataRow.Field<int>(str_col_Jump);
                            }
                            else break;
                        }
                    }
                    if (intJump < 1)
                    { break; }
                    else if (intWeight + intJump >= int_x_Wgt_Opener)
                    {
                        break;
                    }
                    else
                    {
                        intWeight += intJump;
                        DataRow dataRow1 = dt.NewRow();
                        dataRow1[0] = intWeight;
                        dataRow1[1] = boolOverride;
                        dt.Rows.Add(dataRow1);
                        intLift++;
                    }
                } while (intWeight < int_x_Wgt_Opener);
                if (bool_Opener_in_Warmup && dt.Rows.Count > 0)
                {
                    if (dt.Rows[dt.Rows.Count - 1].Field<int>(0) < int_x_Wgt_Opener)
                    {
                        intWeight = int_x_Wgt_Opener;
                        DataRow dataRow1 = dt.NewRow();
                        dataRow1[0] = intWeight;
                        dataRow1[1] = boolOverride;
                        dt.Rows.Add(dataRow1);
                        intLift++;
                    }
                }
            }

            return dt;
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
            int _intWeightBar
            , int _intWeightOpener
            , bool _boolSnatch
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
                intShadowWidth = 1,
                intPlateGap = -1,
                Location = new Point(0, 0),
                Size = _panelGraphic.Size,
            };

            weightBoxGraphic.Paint += Apply_Vector_Weight_Graphic;
            _panelGraphic.Controls.Add(weightBoxGraphic);
        }
        private void Apply_Vector_Weight_Graphic(object sender, PaintEventArgs e)
        {
            WeightBox weightBoxGraphic = (WeightBox)sender;
            int _intWeightBar = weightBoxGraphic.intWeightBar;
            int _intWeight = weightBoxGraphic.intWeight;
            int _int_Shadow_Width = weightBoxGraphic.intShadowWidth;
            int _intPlateGap = weightBoxGraphic.intPlateGap;
            bool _boolOpener = weightBoxGraphic.boolOpener;

            int intLBuffer, intTBuffer, intBBuffer;
            bool _boolCollars;
            bool _bool5KGCollars;
            if (_boolOpener)
            {
                _boolCollars = _intWeight - 5 >= _intWeightBar;
                _bool5KGCollars = ((_intWeight - 15) >= _intWeightBar);
                intLBuffer = 0;
                intTBuffer = 0;
                intBBuffer = 0;
            }
            else
            {
                _boolCollars = false;
                _bool5KGCollars = false;
                intTBuffer = 1;
                intLBuffer = intTBuffer;
                intBBuffer = intTBuffer + 4;
            }

            Plates_Count_For_Weight(
                _intWeightBar: _intWeightBar,
                _bool5KGCollar: _bool5KGCollars,
                _intWeightLift: _intWeight,
                _int25_0: out int int25_0,
                _bool20_0: out bool bool20_0,
                _bool15_0: out bool bool15_0,
                _bool10_0: out bool bool10_0,
                _bool05_0: out bool bool05_0,
                _bool02_5: out bool bool02_5,
                _bool02_0: out bool bool02_0,
                _bool01_5: out bool bool01_5,
                _bool01_0: out bool bool01_0,
                _bool00_5: out bool bool00_5);

            int int_Full_Height = weightBoxGraphic.Height - intTBuffer - intBBuffer;
            int int_Plate_Height = int_Full_Height;
            int int_Plate_Width25_0 = 16;
            int int_Plate_Width20_0 = 15;
            int int_Plate_Width15_0 = 13;
            int int_Plate_Width10_0 = 11;
            int int_Plate_Width05_0 = 9;
            int int_Plate_Width02_5 = 8;
            int int_Plate_Width02_0 = 8;
            int int_Plate_Width01_5 = 8;
            int int_Plate_Width01_0 = 7;
            int int_Plate_Width00_5 = 6;
            int int_Collar_Height = _bool5KGCollars ? 18 : 12;
            int int_Collar_Width = _bool5KGCollars ? 8 : 6;
            int int_MainBar_Width = (
                    _boolOpener ?
                    Math.Min(400, weightBoxGraphic.Width - intLBuffer - (2 * _int_Shadow_Width)) :
                    10);
            int int_Sleeve_Width = 
                (
                    _boolOpener ?
                    (
                        _intWeight > 229 ?
                        125 :
                        (
                            _intWeight > 149 ?
                            100 :
                            75
                        )
                    ) :
                    75
                );
            int int_SleeveKnuckle_Width = 9;
            bool _boolShadow = (_int_Shadow_Width > 0);

            SolidBrush brush_BarShadow = new(color: Color.Black);
            SolidBrush brush_PlateShadow = new(color: Color.Black);
            SolidBrush brush_CollarSilver = new(color: Color.Gainsboro);
            SolidBrush brush_BarGrey = new(color: color_BarGrey);
            SolidBrush brush_Red = new(color: color_Plate_Red);
            SolidBrush brush_Blue = new(color: color_Plate_Blue);
            SolidBrush brush_Yellow = new(color: color_Plate_Yellow);
            SolidBrush brush_Green = new(color: color_Plate_Green);
            SolidBrush brush_White = new(color: color_Plate_White);

            //add bar
            //add bar shadow
            Rectangle 
                rect_MainBar, rectShadow_MainBar,
                rect_SleeveKnuckle_Right, rectShadow_SleeveKnuckle_Right,
                rect_Sleeve_Right, rectShadow_Sleeve_Right,
                rect_SleeveKnuckle_Left, rectShadow_SleeveKnuckle_Left,
                rect_Sleeve_Left, rectShadow_Sleeve_Left;

            rect_MainBar = new(
                x: intLBuffer + _int_Shadow_Width,
                y: 0,
                width: int_MainBar_Width,
                height: 6);
            rect_MainBar.Y = (int_Full_Height / 2) - (rect_MainBar.Height / 2) + intTBuffer;
            if (_boolOpener)
            {
                rect_Sleeve_Right = new(
                    x: rect_MainBar.X + rect_MainBar.Width - int_Sleeve_Width,
                    y: 0,
                    width: int_Sleeve_Width,
                    height: 10);
                rect_SleeveKnuckle_Right = new(
                    x: rect_Sleeve_Right.X - int_SleeveKnuckle_Width,
                    y: 0,
                    width: int_SleeveKnuckle_Width,
                    height: 16);
                rect_Sleeve_Left = new(
                    x: intLBuffer + _int_Shadow_Width,
                    y: 0,
                    width: int_Sleeve_Width,
                    height: 10);
                rect_SleeveKnuckle_Left = new(
                    x: rect_Sleeve_Left.X + rect_Sleeve_Left.Width - 1,
                    y: 0,
                    width: int_SleeveKnuckle_Width,
                    height: 16);
            }
            else
            {
                rect_SleeveKnuckle_Right = new(
                    x: rect_MainBar.X + rect_MainBar.Width - 1,
                    y: 0,
                    width: int_SleeveKnuckle_Width,
                    height: 16);
                rect_Sleeve_Right = new(
                    x: rect_SleeveKnuckle_Right.X + rect_SleeveKnuckle_Right.Width - 1,
                    y: 0,
                    width: int_Sleeve_Width,
                    height: 10);
                rect_Sleeve_Left = new();
                rect_SleeveKnuckle_Left = new();
            }
            rect_SleeveKnuckle_Right.Y = (int_Full_Height / 2) - (rect_SleeveKnuckle_Right.Height / 2) + intTBuffer;
            rect_Sleeve_Right.Y = (int_Full_Height / 2) - (rect_Sleeve_Right.Height / 2) + intTBuffer;
            rect_SleeveKnuckle_Left.Y = rect_SleeveKnuckle_Right.Y;
            rect_Sleeve_Left.Y = rect_Sleeve_Right.Y;
            if (_boolShadow)
            {
                rectShadow_MainBar = rect_MainBar;
                rectShadow_MainBar.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                rectShadow_SleeveKnuckle_Right = rect_SleeveKnuckle_Right;
                rectShadow_SleeveKnuckle_Right.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                rectShadow_Sleeve_Right = rect_Sleeve_Right;
                rectShadow_Sleeve_Right.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                e.Graphics.FillRectangle(
                    brush: brush_BarShadow,
                    rect: rectShadow_MainBar);
                e.Graphics.FillRectangle(
                    brush: brush_BarShadow,
                    rect: rectShadow_SleeveKnuckle_Right);
                e.Graphics.FillRectangle(
                    brush: brush_BarShadow,
                    rect: rectShadow_Sleeve_Right);
                if (_boolOpener)
                {
                    rectShadow_SleeveKnuckle_Left = rect_SleeveKnuckle_Left;
                    rectShadow_SleeveKnuckle_Left.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                    rectShadow_Sleeve_Left = rect_Sleeve_Left;
                    rectShadow_Sleeve_Left.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_BarShadow,
                        rect: rectShadow_MainBar);
                    e.Graphics.FillRectangle(
                        brush: brush_BarShadow,
                        rect: rectShadow_SleeveKnuckle_Left);
                    e.Graphics.FillRectangle(
                        brush: brush_BarShadow,
                        rect: rectShadow_Sleeve_Left);
                }
            }
            e.Graphics.FillRectangle(
                brush: brush_BarGrey,
                rect: rect_MainBar);
            e.Graphics.FillRectangle(
                brush: brush_BarGrey,
                rect: rect_SleeveKnuckle_Right);
            e.Graphics.FillRectangle(
                brush: brush_BarGrey,
                rect: rect_Sleeve_Right);
            if (_boolOpener)
            {
                e.Graphics.FillRectangle(
                    brush: brush_BarGrey,
                    rect: rect_MainBar);
                e.Graphics.FillRectangle(
                    brush: brush_BarGrey,
                    rect: rect_SleeveKnuckle_Left);
                e.Graphics.FillRectangle(
                    brush: brush_BarGrey,
                    rect: rect_Sleeve_Left);
            }
            
            // add plates
            Rectangle rect, rectShadow;
            int intLeft = rect_SleeveKnuckle_Right.X + int_SleeveKnuckle_Width + _int_Shadow_Width + 1;
            int intRight = rect_SleeveKnuckle_Left.X - _int_Shadow_Width - 1;
            for (int intI = 1; intI <= int25_0; intI++)
            {
                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                    width: int_Plate_Width25_0,
                    height: int_Plate_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_PlateShadow,
                        rect: rectShadow);
                }
                e.Graphics.FillRectangle(
                    brush: brush_Red,
                    rect: rect);
                intLeft += rectShadow.Width + _intPlateGap;
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_Red,
                        rect: rect);
                    intRight -= _intPlateGap;
                }
            }
            
            if (bool20_0)
            {
                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                    width: int_Plate_Width20_0,
                    height: int_Plate_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_PlateShadow,
                        rect: rectShadow);
                }
                e.Graphics.FillRectangle(
                    brush: brush_Blue,
                    rect: rect);
                intLeft += rectShadow.Width + _intPlateGap;
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_Blue,
                        rect: rect);
                    intRight -= _intPlateGap;
                }
            }
            
            if (bool15_0)
            {
                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                    width: int_Plate_Width15_0,
                    height: int_Plate_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_PlateShadow,
                        rect: rectShadow);
                }
                e.Graphics.FillRectangle(
                    brush: brush_Yellow,
                    rect: rect);
                intLeft += rectShadow.Width + _intPlateGap;
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_Yellow,
                        rect: rect);
                    intRight -= _intPlateGap;
                }
            }
            
            if (bool10_0)
            {
                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                    width: int_Plate_Width10_0,
                    height: int_Plate_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_PlateShadow,
                        rect: rectShadow);
                }
                e.Graphics.FillRectangle(
                    brush: brush_Green,
                    rect: rect);
                intLeft += rectShadow.Width + _intPlateGap;
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_Green,
                        rect: rect);
                    intRight -= _intPlateGap;
                }
            }

            int_Plate_Height = Convert.ToInt32(int_Plate_Height * .75);
            if (bool05_0)
            {
                int int_050_Height;
                if (int25_0 == 0 & !bool20_0 & !bool15_0 & !bool10_0)
                {
                    int_050_Height = int_Full_Height;
                    int_Plate_Width05_0 = int_Plate_Width25_0 * 2;
                }
                else
                {
                    int_050_Height = int_Plate_Height;
                }

                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_050_Height / 2) + intTBuffer,
                    width: int_Plate_Width05_0,
                    height: int_050_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_PlateShadow,
                        rect: rectShadow);
                }
                e.Graphics.FillRectangle(
                    brush: brush_White,
                    rect: rect);
                intLeft += rectShadow.Width + _intPlateGap;
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_White,
                        rect: rect);
                    intRight -= _intPlateGap;
                }
            }

            int_Plate_Height = Convert.ToInt32(int_Plate_Height * .85);
            if (bool02_5)
            {
                int int_025_Height;
                if (int25_0 == 0 & !bool20_0 & !bool15_0 & !bool10_0 & !bool05_0)
                {
                    int_025_Height = int_Full_Height;
                    int_Plate_Width02_5 = int_Plate_Width25_0 * 2;
                }
                else
                {
                    int_025_Height = int_Plate_Height;
                }

                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_025_Height / 2) + intTBuffer,
                    width: int_Plate_Width02_5,
                    height: int_025_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_PlateShadow,
                        rect: rectShadow);
                }
                e.Graphics.FillRectangle(
                    brush: brush_Red,
                    rect: rect);
                intLeft += rectShadow.Width + _intPlateGap;
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_Red,
                        rect: rect);
                    intRight -= _intPlateGap;
                }
            }

            if (_boolCollars)
            {
                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_Collar_Height / 2) + intTBuffer,
                    width: int_Collar_Width,
                    height: int_Collar_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_BarShadow,
                        rect: rectShadow);
                }
                Brush b = _bool5KGCollars ? brush_CollarSilver : Brushes.Black;
                e.Graphics.FillRectangle(
                    brush: b,
                    rect: rect);
                intLeft += rectShadow.Width + _intPlateGap;
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: b,
                        rect: rect);
                    intRight -= _intPlateGap;
                }
            }

            int_Plate_Height = Convert.ToInt32(int_Plate_Height * .90);
            if (bool02_0)
            {
                rect = new(
                    x: intLeft,
                    y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                    width: int_Plate_Width02_0,
                    height: int_Plate_Height);
                rectShadow = rect;
                if (_boolShadow)
                {
                    rectShadow.Width += _int_Shadow_Width * 2;
                    rect.X += _int_Shadow_Width;
                    rect.Inflate(width: 0, height: -_int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: brush_PlateShadow,
                        rect: rectShadow);
                }
                e.Graphics.FillRectangle(
                    brush: brush_Blue,
                    rect: rect);
                if (_boolOpener)
                {
                    intRight -= rectShadow.Width;
                    rect.X = intRight;
                    if (_boolShadow)
                    {
                        rectShadow.X = rect.X;
                        rect.X += _int_Shadow_Width;
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_Blue,
                        rect: rect);
                }
            }
            else
            {
                int_Plate_Height = Convert.ToInt32(int_Plate_Height * .90);
                if (bool01_5)
                {
                    rect = new(
                        x: intLeft,
                        y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                        width: int_Plate_Width01_5,
                        height: int_Plate_Height);
                    rectShadow = rect;
                    if (_boolShadow)
                    {
                        rectShadow.Width += _int_Shadow_Width * 2;
                        rect.X += _int_Shadow_Width;
                        rect.Inflate(width: 0, height: -_int_Shadow_Width);
                        e.Graphics.FillRectangle(
                            brush: brush_PlateShadow,
                            rect: rectShadow);
                    }
                    e.Graphics.FillRectangle(
                        brush: brush_Yellow,
                        rect: rect);
                    if (_boolOpener)
                    {
                        intRight -= rectShadow.Width;
                        rect.X = intRight;
                        if (_boolShadow)
                        {
                            rectShadow.X = rect.X;
                            rect.X += _int_Shadow_Width;
                            e.Graphics.FillRectangle(
                                brush: brush_PlateShadow,
                                rect: rectShadow);
                        }
                        e.Graphics.FillRectangle(
                            brush: brush_Yellow,
                            rect: rect);
                    }
                }
                else
                {
                    int_Plate_Height = Convert.ToInt32(int_Plate_Height * .90);
                    if (bool01_0)
                    {
                        rect = new(
                            x: intLeft,
                            y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                            width: int_Plate_Width01_0,
                            height: int_Plate_Height);
                        rectShadow = rect;
                        if (_boolShadow)
                        {
                            rectShadow.Width += _int_Shadow_Width * 2;
                            rect.X += _int_Shadow_Width;
                            rect.Inflate(width: 0, height: -_int_Shadow_Width);
                            e.Graphics.FillRectangle(
                                brush: brush_PlateShadow,
                                rect: rectShadow);
                        }
                        e.Graphics.FillRectangle(
                            brush: brush_Green,
                            rect: rect);
                        if (_boolOpener)
                        {
                            intRight -= rectShadow.Width;
                            rect.X = intRight;
                            if (_boolShadow)
                            {
                                rectShadow.X = rect.X;
                                rect.X += _int_Shadow_Width;
                                e.Graphics.FillRectangle(
                                    brush: brush_PlateShadow,
                                    rect: rectShadow);
                            }
                            e.Graphics.FillRectangle(
                                brush: brush_Green,
                                rect: rect);
                        }
                    }
                    else
                    {
                        int_Plate_Height = Convert.ToInt32(int_Plate_Height * .90);
                        if (bool00_5)
                        {
                            rect = new(
                                x: intLeft,
                                y: (int_Full_Height / 2) - (int_Plate_Height / 2) + intTBuffer,
                                width: int_Plate_Width00_5,
                                height: int_Plate_Height);
                            rectShadow = rect;
                            if (_boolShadow)
                            {
                                rectShadow.Width += _int_Shadow_Width * 2;
                                rect.X += _int_Shadow_Width;
                                rect.Inflate(width: 0, height: -_int_Shadow_Width);
                                e.Graphics.FillRectangle(
                                    brush: brush_PlateShadow,
                                    rect: rectShadow);
                            }
                            e.Graphics.FillRectangle(
                                brush: brush_White,
                                rect: rect);
                            if (_boolOpener)
                            {
                                intRight -= rectShadow.Width;
                                rect.X = intRight;
                                if (_boolShadow)
                                {
                                    rectShadow.X = rect.X;
                                    rect.X += _int_Shadow_Width;
                                    e.Graphics.FillRectangle(
                                        brush: brush_PlateShadow,
                                        rect: rectShadow);
                                }
                                e.Graphics.FillRectangle(
                                    brush: brush_White,
                                    rect: rect);
                            }
                        }
                    }
                }
            }
        }
        private void Plates_Count_For_Weight(
            int _intWeightBar
            , bool _bool5KGCollar
            , int _intWeightLift
            , out int _int25_0     // 25kg     red
            , out bool _bool20_0     // 20kg     blue
            , out bool _bool15_0     // 15kg     yellow
            , out bool _bool10_0     // 10kg     green
            , out bool _bool05_0      // 5kg      largechange_white
            , out bool _bool02_5    // 2.5kg    change_red
            , out bool _bool02_0      // 2kg      change_blue
            , out bool _bool01_5    // 1.5kg    change_yellow
            , out bool _bool01_0      // 1kg      change_green
            , out bool _bool00_5    // 0.5kg    change_white
            )
        {
            _bool20_0 = false;
            _bool15_0 = false;
            _bool10_0 = false;
            _bool05_0 = false;
            _bool02_5 = false;
            _bool02_0 = false;
            _bool01_5 = false;
            _bool01_0 = false;
            _bool00_5 = false;

            if (_bool5KGCollar) { _intWeightBar += 5; }
            if (_intWeightLift > _intWeightBar)
            {
                decimal _decToGo = Convert.ToDecimal(_intWeightLift - _intWeightBar) / 2;
                decimal _decPlateWeight;

                _decPlateWeight = 25M;
                _int25_0 = Convert.ToInt32((_decToGo - _decToGo % _decPlateWeight) / _decPlateWeight);
                _decToGo -= Convert.ToDecimal(_int25_0) * _decPlateWeight;

                _decPlateWeight = 20M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool20_0 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 15M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool15_0 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 10M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool10_0 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 5M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool05_0 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 2.5M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool02_5 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 2M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool02_0 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 1.5M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool01_5 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 1M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool01_0 = true;
                    _decToGo -= _decPlateWeight;
                }

                _decPlateWeight = 0.5M;
                if (_decToGo >= _decPlateWeight)
                {
                    _bool00_5 = true;
                }
            }
            else
            {
                _int25_0 = 0;
            }
        }

        #endregion

    }
    public class WeightBox : PictureBox
    {
        public bool boolOpener; //is opener vs is warmup
        public int intWeightBar;
        public int intWeight;
        public int intShadowWidth;
        public int intPlateGap;

    }
}