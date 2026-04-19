using System.Drawing;

namespace Weightlifting_Comp_Warmup.Main
{
    public static class AppColors
    {
        // Live Runner Colors
        public static readonly Color Live_Default_FG = Color.FromArgb(240, 240, 240);
        public static readonly Color Live_Highlight_BG = Color.Yellow;
        public static readonly Color Live_Highlight_FG = Color.Black;
        public static readonly Color AdvanceButton_Active = Color.FromArgb(32, 150, 32);

        // Weight Plate Colors
        public static readonly Color Plate_Red = Color.FromArgb(251, 13, 27);
        public static readonly Color Plate_Blue = Color.FromArgb(10, 100, 255);
        public static readonly Color Plate_Yellow = Color.FromArgb(255, 253, 56);
        public static readonly Color Plate_Green = Color.FromArgb(15, 127, 18);
        public static readonly Color Plate_White = Color.FromArgb(255, 255, 255);

        // Barbell Graphic Color
        public static readonly Color Bar_Grey = Color.LightSteelBlue;

        public static Color Snatch_Live_BG;
        public static Color Cj_Live_BG;
    }
}
