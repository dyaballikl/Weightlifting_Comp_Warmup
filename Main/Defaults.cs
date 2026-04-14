using System.Collections.Generic;

namespace Weightlifting_Comp_Warmup.Main
{
    internal static class Defaults
    {
        internal static List<Extra> default_snatchExtras()
        {
            int intOrder = 0;
            List<Extra> _extras =
            [
                new(
                    action: "Ibuprofen, Coffee, Pre",
                    length: 20 * 60,
                    order: intOrder
                ),
            ];
            intOrder++;

            _extras.Add(new(
                action: "Foam Roll",
                length: 5 * 60,
                order: intOrder));
            intOrder++;

            _extras.Add(new(
                action: "Shoes, tape, etc.",
                length: 5 * 60,
                order: intOrder));
            intOrder++;

            _extras.Add(new(
                action: "Stretch",
                length: 10 * 60,
                order: intOrder));
            intOrder++;

            _extras.Add(new(
                action: "Empty bar stretch",
                length: 5 * 60,
                order: intOrder));
            return _extras;
        }
        internal static Dictionary<int, int> default_snatchJumps()
        {
            Dictionary<int, int> _jumps = new()
            {
                [1] = 20,
                [40] = 10,
                [50] = 5,
                [80] = 4,
                [89] = 3
            };
            return _jumps;
        }
        internal static Dictionary<int, int> default_snatchTimes()
        {
            Dictionary<int, int> _times = new()
            {
                [1] = 210,
                [41] = 150
            };
            return _times;
        }
        internal static List<Extra> default_cjExtras()
        {
            int intOrder = 0;
            List<Extra> _extras =
            [
                new(
                    action: "Stretch",
                    length: 5 * 60,
                    order: intOrder
                ),
            ];
            return _extras;
        }
        internal static Dictionary<int, int> default_cjJumps()
        {
            Dictionary<int, int> _jumps = new()
            {
                [1] = 30,
                [50] = 10,
                [90] = 7,
                [100] = 5,
                [105] = 4
            };
            return _jumps;
        }
        internal static Dictionary<int, int> default_cjTimes()
        {
            Dictionary<int, int> _times = new()
            {
                [1] = 5 * 60,
                [21] = 140,
                [100] = 150
            };
            return _times;
        }
    }
}
