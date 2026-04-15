using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
    public partial class form_Main
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

        private const string strVersion = "1.16.0";

        private List<Step> snatchStepsLIVE = null;
        private List<Step> snatchStepsPLAN = null;
        private List<Step> cjStepsLIVE = null;
        private List<Step> cjStepsPLAN = null;

        private List<Extra> default_snatchExtras = null;
        private Dictionary<int /*from weight*/, int /*jump size*/> default_snatchJumps = null;
        private Dictionary<int /*from weight*/, int /*time length*/> default_snatchTimes = null;
        private List<Extra> default_cjExtras = null;
        private Dictionary<int /*from weight*/, int /*jump size*/> default_cjJumps = null;
        private Dictionary<int /*from weight*/, int /*time length*/> default_cjTimes = null;

        private List<Extra> snatchExtras = null;
        private Dictionary<int /*from weight*/, int /*jump size*/> snatchJumps = null;
        private Dictionary<int /*from weight*/, int /*time length*/> snatchTimes = null;
        private List<Extra> cjExtras = null;
        private Dictionary<int /*from weight*/, int /*jump size*/> cjJumps = null;
        private Dictionary<int /*from weight*/, int /*time length*/> cjTimes = null;

        private const string str_buttontext_up = "^";
        private const string str_buttontext_down = "v";
        private const string str_buttontext_delete = "X";
        private const string str_buttontext_commit = "commit";

        private int int_ProfileId;
        private int int_Barbell;
        private int int_snatch_Sec_Stage;
        private int int_snatch_Wgt_Opener;
        private int int_snatch_Sec_End;
        private int int_snatch_Lifts_Out;
        private int int_snatch_Lifts_Passed;
        private int int_snatch_Warmup_Step;
        private int int_cj_Sec_Stage;
        private int int_cj_Wgt_Opener;
        private int int_cj_Sec_End;
        private int int_cj_Lifts_Out;
        private int int_cj_Lifts_Passed;
        private int int_cj_snLifts_Out;
        private int int_cj_Warmup_Step;
        private int int_cj_Sec_Break;

        private TimeSpan timeSpan_Start;

        private bool bool_snatch_Live;
        private bool bool_snatch_LiveLifting;
        private bool bool_snatch_AutoAdvance = false;
        private bool bool_cj_Live;
        private bool bool_cj_LiveLifting;
        private bool bool_cj_BreakRunning;
        private bool bool_cj_sn_Lifting;
        private bool bool_cj_AutoAdvance = false;
        private bool bool_Beep = false;
        private bool bool_Loading = true;
        private bool bool_snatch_OpenerWarmup;
        private bool bool_cj_OpenerWarmup;

        private const int int_default_Barbell = 20;
        private const int int_default_snatch_Sec_Stage = 55;
        private const int int_default_snatch_Wgt_Opener = 85;
        private const int int_default_snatch_Sec_End = 60;
        private const int int_default_snatch_Lifts_Out = 0;
        private const int int_default_cj_Sec_Stage = 62;
        private const int int_default_cj_Wgt_Opener = 110;
        private const int int_default_cj_Sec_End = 75;
        private const int int_default_cj_Lifts_Out = 0;
        private const int int_default_cj_snLifts_Out = 0;
        private const int int_default_cj_Sec_Break = 0;

        private const bool bool_default_Beep = false;
        private const bool bool_default_snatch_OpenerWarmup = true;
        private const bool bool_default_cj_OpenerWarmup = false;

        private DateTime datetime_snatch_Start;

        private Timer timer_snatch_Live;
        private Timer timer_cj_Live;

        private readonly Color color_Live_Default_FG = Color.FromArgb(240, 240, 240);
        private readonly Color color_Live_Highlight_BG = Color.Yellow;
        private readonly Color color_Live_Highlight_FG = Color.Black;
        private readonly Color color_AdvanceButton_Active = Color.FromArgb(32, 150, 32);
        private readonly Color color_Plate_Red = Color.FromArgb(251, 13, 27);
        private readonly Color color_Plate_Blue = Color.FromArgb(10, 100, 255);
        private readonly Color color_Plate_Yellow = Color.FromArgb(255, 253, 56);
        private readonly Color color_Plate_Green = Color.FromArgb(15, 127, 18);
        private readonly Color color_Plate_White = Color.FromArgb(255, 255, 255);
        private readonly Color color_BarGrey = Color.LightSteelBlue;

        private Color color_snatch_Live_BG;
        private Color color_cj_Live_BG;

        PropertyData propertyData_BatteryPercent = null;
        PropertyData propertyData_BatteryMinutesRemaining = null;
        readonly Properties.Settings savedSettings = Properties.Settings.Default;
    }
}
