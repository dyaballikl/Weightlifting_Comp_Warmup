using System.Collections.Generic;

namespace Weightlifting_Comp_Warmup.Main
{
    internal static class Defaults
    {
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
            int intOrder = 0;
            _default_snatchExtras =
            [
                new(
                    action: "Ibuprofen, Coffee, Pre",
                    length: 20 * 60,
                    order: intOrder
                ),
            ];
            intOrder++;

            _default_snatchExtras.Add(new(
                action: "Foam Roll",
                length: 5 * 60,
                order: intOrder));
            intOrder++;

            _default_snatchExtras.Add(new(
                action: "Shoes, tape, etc.",
                length: 5 * 60,
                order: intOrder));
            intOrder++;

            _default_snatchExtras.Add(new(
                action: "Stretch",
                length: 10 * 60,
                order: intOrder));
            intOrder++;

            _default_snatchExtras.Add(new(
                action: "Empty bar stretch",
                length: 5 * 60,
                order: intOrder));
        }
        private static void populate_default_snatchJumps()
        {
            _default_snatchJumps = new()
            {
                [1] = 20,
                [40] = 10,
                [50] = 5,
                [80] = 4,
                [89] = 3
            };
        }
        private static void populate_default_snatchTimes()
        {
            _default_snatchTimes = new()
            {
                [1] = 210,
                [41] = 150
            };
        }
        private static void populate_default_cjExtras()
        {
            int intOrder = 0;
            _default_cjExtras =
            [
                new(
                    action: "Stretch",
                    length: 5 * 60,
                    order: intOrder
                ),
            ];
        }
        private static void populate_default_cjJumps()
        {
            _default_cjJumps = new()
            {
                [1] = 30,
                [50] = 10,
                [90] = 7,
                [100] = 5,
                [105] = 4
            };
        }
        private static void populate_default_cjTimes()
        {
            _default_cjTimes = new()
            {
                [1] = 5 * 60,
                [21] = 140,
                [100] = 150
            };
        }
    }
}
