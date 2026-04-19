using System;
using System.Collections.Generic;

namespace Weightlifting_Comp_Warmup.Main
{
    internal static class Defaults
    {
        internal const int int_default_Barbell = 20;
        internal const int int_default_snatch_SecondsStage = 55;
        internal const int int_default_snatch_OpenerWeight = 85;
        internal const int int_default_snatch_SecondsEnd = 60;
        internal const int int_default_snatch_LiftsOut = 3;
        internal const int int_default_cj_SecondsStage = 62;
        internal const int int_default_cj_OpenerWeight = 110;
        internal const int int_default_cj_SecondsEnd = 75;
        internal const int int_default_cj_LiftsOut = 3;
        internal const int int_default_cJ_SnatchLifts_Out = 0;
        internal const int int_default_cJ_SecondsBreak = 600;
        internal static TimeSpan timeSpan_default_Start = new(9, 0, 0);
        internal const bool bool_default_snatch_OpenerInWarmup = true;
        internal const bool bool_default_cj_OpenerInWarmup = false;
        internal const bool bool_default_Beep = false;

        private static List<Extra> _default_snatchExtras = null;
        internal static List<Extra> default_snatchExtras
        {
            get
            {
                if (_default_snatchExtras == null)
                {
                    populate_default_snatchExtras();
                }
                return _default_snatchExtras;
            }
        }
        private static Dictionary<int /*from weight*/, int /*jump size*/> _default_snatchJumps = null;
        internal static Dictionary<int /*from weight*/, int /*jump size*/> default_snatchJumps
        {
            get
            {
                if (_default_snatchJumps == null)
                {
                    populate_default_snatchJumps();
                }
                return _default_snatchJumps;
            }
        }
        private static Dictionary<int /*from weight*/, int /*time length*/> _default_snatchTimes = null;
        internal static Dictionary<int /*from weight*/, int /*jump size*/> default_snatchTimes
        {
            get
            {
                if (_default_snatchTimes == null)
                {
                    populate_default_snatchTimes();
                }
                return _default_snatchTimes;
            }
        }
        private static List<Extra> _default_cjExtras = null;
        internal static List<Extra> default_cjExtras
        {
            get
            {
                if (_default_cjExtras == null)
                {
                    populate_default_cjExtras();
                }
                return _default_cjExtras;
            }
        }
        private static Dictionary<int /*from weight*/, int /*jump size*/> _default_cjJumps = null;
        internal static Dictionary<int /*from weight*/, int /*jump size*/> default_cjJumps
        {
            get
            {
                if (_default_cjJumps == null)
                {
                    populate_default_cjJumps();
                }
                return _default_cjJumps;
            }
        }
        private static Dictionary<int /*from weight*/, int /*time length*/> _default_cjTimes = null;
        internal static Dictionary<int /*from weight*/, int /*jump size*/> default_cjTimes
        {
            get
            {
                if (_default_cjTimes == null)
                {
                    populate_default_cjTimes();
                }
                return _default_cjTimes;
            }
        }

        private static void populate_default_snatchExtras()
        {
            _default_snatchExtras = [];
        }
        private static void populate_default_snatchJumps()
        {
            _default_snatchJumps = new() { [1] = 20 };
        }
        private static void populate_default_snatchTimes()
        {
            _default_snatchTimes = new() { [1] = 140 };
        }
        private static void populate_default_cjExtras()
        {
            _default_cjExtras = [];
        }
        private static void populate_default_cjJumps()
        {
            _default_cjJumps = new() { [1] = 30 };
        }
        private static void populate_default_cjTimes()
        {
            _default_cjTimes = new() { [1] = 160 };
        }

        #region demonstration values
        internal static List<Extra> demo_snatchExtras
        {
            get
            {
                int intOrder = 0;
                List<Extra> _demo_snatchExtras =
                [
                    new(
                    action: "Ibuprofen, Coffee, Pre",
                    length: 20 * 60,
                    order: intOrder
                ),
            ];
                intOrder++;

                _demo_snatchExtras.Add(new(
                    action: "Foam Roll",
                    length: 5 * 60,
                    order: intOrder));
                intOrder++;

                _demo_snatchExtras.Add(new(
                    action: "Shoes, tape, etc.",
                    length: 5 * 60,
                    order: intOrder));
                intOrder++;

                _demo_snatchExtras.Add(new(
                    action: "Stretch",
                    length: 10 * 60,
                    order: intOrder));
                intOrder++;

                _demo_snatchExtras.Add(new(
                    action: "Empty bar stretch",
                    length: 5 * 60,
                    order: intOrder));
                return _demo_snatchExtras;
            }
        }
        internal static Dictionary<int /*from weight*/, int /*jump size*/> demo_snatchJumps
        {
            get
            {
                Dictionary<int /*from weight*/, int /*jump size*/> _demo_snatchJumps = new()
                {
                    [1] = 20,
                    [40] = 10,
                    [50] = 5,
                    [80] = 4,
                    [89] = 3
                };
                return _demo_snatchJumps;
            }
        }
        internal static Dictionary<int /*from weight*/, int /*jump size*/> demo_snatchTimes
        {
            get
            {
                Dictionary<int /*from weight*/, int /*jump size*/> _demo_snatchTimes = new()
                {
                    [1] = 210,
                    [41] = 150
                };
                return _demo_snatchTimes;
            }
        }
        internal static List<Extra> demo_cjExtras
        {
            get
            {
                int intOrder = 0;
                List<Extra> _demo_cjExtras =
                [
                    new(
                    action: "Stretch",
                    length: 5 * 60,
                    order: intOrder
                ),
            ];
                return _demo_cjExtras;
            }
        }
        internal static Dictionary<int /*from weight*/, int /*jump size*/> demo_cjJumps
        {
            get
            {
                Dictionary<int /*from weight*/, int /*jump size*/> _demo_cjJumps = new()
                {
                    [1] = 30,
                    [50] = 10,
                    [90] = 7,
                    [100] = 5,
                    [105] = 4
                };
                return _demo_cjJumps;
            }
        }
        internal static Dictionary<int /*from weight*/, int /*jump size*/> demo_cjTimes
        {
            get
            {
                Dictionary<int /*from weight*/, int /*jump size*/> _demo_cjTimes = new()
                {
                    [1] = 5 * 60,
                    [21] = 140,
                    [100] = 150
                };
                return _demo_cjTimes;
            }
        }
        #endregion
    }
}
