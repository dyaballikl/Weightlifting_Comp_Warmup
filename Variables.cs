using System;
using System.Collections.Generic;
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

        private const string str_buttontext_up = "^";
        private const string str_buttontext_down = "v";
        private const string str_buttontext_delete = "X";
        private const string str_buttontext_commit = "commit";

        private int int_snatch_Lifts_Passed = 0;
        private int int_cj_Warmup_Step = 0;
        private int int_snatch_Warmup_Step = 0;
        private int int_cj_Lifts_Passed = 0;
        private Profile profileActive = null;

        private bool bool_snatch_Live;
        private bool bool_snatch_LiveLifting;
        private bool bool_snatch_AutoAdvance = false;
        private bool bool_cj_Live;
        private bool bool_cj_LiveLifting;
        private bool bool_cj_BreakRunning;
        private bool bool_cj_SnStillLifting;
        private bool bool_cj_AutoAdvance = false;
        private bool bool_Loading = true;

        private const int int_default_Barbell = 20;
        private const int int_default_snatch_SecondsStage = 55;
        private const int int_default_snatch_OpenerWeight = 85;
        private const int int_default_snatch_SecondsEnd = 60;
        private const int int_default_snatch_Lifts_Out = 0;
        private const int int_default_cj_SecondsStage = 62;
        private const int int_default_cj_OpenerWeight = 110;
        private const int int_default_cj_SecondsEnd = 75;
        private const int int_default_cj_LiftsOut = 0;
        private const int int_default_cJ_SnatchLifts_Out = 0;
        private const int int_default_cJ_SecondsBreak = 600;

        private const bool bool_default_Beep = false;
        private const bool bool_default_snatch_OpenerInWarmup = true;
        private const bool bool_default_cj_OpenerInWarmup = false;

        private DateTime datetime_snatch_Start;

        private Timer timer_snatch_Live;
        private Timer timer_cj_Live;
        private Timer timer_Battery;

        private readonly Properties.Settings savedSettings = Properties.Settings.Default;
    }
}
