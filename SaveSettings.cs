using System;
using System.Diagnostics;
using System.Linq;

namespace Weightlifting_Comp_Warmup.Main
{
    public partial class form_Main
    {
        private void button_SaveSettings_Click(object sender, EventArgs eventArgs)
        {
            SaveSettings();
        }
        private void SaveSettings()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            profiles ??= [];
            savedSettings.int_ProfileId = profileActive?.id ?? 0;
            savedSettings.ii_int_ProfileIds = [.. profiles.Select(r => $"{r.Key:D3}")];
            savedSettings.ii_string_ProfileName = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Name}")];
            savedSettings.ii_int_BarbellWeight = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.BarbellWeight}")];
            savedSettings.ii_int_BarbellWeight = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.BarbellWeight}")];
            savedSettings.ii_hhmm_Start = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Start:hhmm}")];
            savedSettings.ii_int_Snatch_SecondsStage = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Snatch_SecondsStage}")];
            savedSettings.ii_int_Snatch_OpenerWeight = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Snatch_OpenerWeight}")];
            savedSettings.ii_bool_Snatch_OpenerInWarmup = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Snatch_OpenerInWarmup}")];
            savedSettings.ii_int_Snatch_SecondsEnd = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Snatch_SecondsEnd}")];
            savedSettings.ii_int_Snatch_LiftsOut = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Snatch_LiftsOut}")];
            savedSettings.ii_int_CJ_SecondsStage = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.CJ_SecondsStage}")];
            savedSettings.ii_int_CJ_OpenerWeight = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.CJ_OpenerWeight}")];
            savedSettings.ii_bool_CJ_OpenerInWarmup = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.CJ_OpenerInWarmup}")];
            savedSettings.ii_int_CJ_SecondsEnd = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.CJ_SecondsEnd}")];
            savedSettings.ii_int_CJ_LiftsOut = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.CJ_LiftsOut}")];
            savedSettings.ii_int_CJ_SnatchLifts_Out = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.CJ_SnatchLifts_Out}")];
            savedSettings.ii_int_CJ_SecondsBreak = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.CJ_SecondsBreak}")];
            savedSettings.ii_bool_Beep = [.. profiles.Select(r => $"{r.Key:D3}{r.Value.Beep}")];
            savedSettings.ii_strings_SnatchExtras =
                [.. profiles.Values
                .SelectMany(
                    p => p.SnatchExtras,
                    (p, x) => $"{p.id:D3}{x.Order:D3}{x.Length:D5}{x.Action}")];
            savedSettings.ii_strings_SnatchJumps =
                [.. profiles.Values
                .SelectMany(
                    p => p.SnatchJumps,
                    (p, j) => $"{p.id:D3}{j.Key:D3}{j.Value:D3}")];
            savedSettings.ii_strings_SnatchTimes =
                [.. profiles.Values
                .SelectMany(
                    p => p.SnatchTimes,
                    (p, t) => $"{p.id:D3}{t.Key:D3}{t.Value:D3}")];
            savedSettings.ii_strings_CJExtras =
                [.. profiles.Values
                .SelectMany(
                    p => p.CJExtras,
                    (p, x) => $"{p.id:D3}{x.Order:D3}{x.Length:D5}{x.Action}")];
            savedSettings.ii_strings_CJJumps =
                [.. profiles.Values
                .SelectMany(
                    p => p.CJJumps,
                    (p, j) => $"{p.id:D3}{j.Key:D3}{j.Value:D3}")];
            savedSettings.ii_strings_CJTimes =
                [.. profiles.Values
                .SelectMany(
                    p => p.CJTimes,
                    (p, t) => $"{p.id:D3}{t.Key:D3}{t.Value:D3}")];
            savedSettings.Save();
            stopwatch.Stop();
            Console.WriteLine($"save settings time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
