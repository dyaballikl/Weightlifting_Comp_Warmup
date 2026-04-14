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

        private const string
            strVersion = "1.16.0";

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
        readonly Properties.Settings savedSettings = Properties.Settings.Default;
    }
}
