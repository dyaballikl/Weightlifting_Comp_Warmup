using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
    public partial class form_Main
    {
        private Dictionary<int, Profile> profiles;
        private void LoadSettings()
        {
            profiles = [];
            for (int i = 0; i < (savedSettings.ii_int_ProfileIds?.Count ?? 0); i++)
            {
                int id = int.Parse(savedSettings.ii_int_ProfileIds[i]);
                setting_TryFetch(
                    settings: savedSettings.ii_string_ProfileName,
                    id: id,
                    defaultGenerator: (i) => $"DefaultName_{i}",
                    setting: out string name);
                setting_TryFetch(
                    settings: savedSettings.ii_int_BarbellWeight,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => Defaults.int_default_Barbell,
                    setting: out int barbellWeight);
                setting_TryFetch(
                    settings: savedSettings.ii_hhmm_Start,
                    id: id,
                    defaultGenerator: (_) => new(hours: 9, minutes: 0, seconds: 0),
                    setting: out TimeSpan start);
                setting_TryFetch(
                    settings: savedSettings.ii_int_Snatch_SecondsStage,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => Defaults.int_default_snatch_SecondsStage,
                    setting: out int snatch_SecondsStage);
                setting_TryFetch(
                    settings: savedSettings.ii_int_Snatch_OpenerWeight,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => Defaults.int_default_snatch_OpenerWeight,
                    setting: out int snatch_OpenerWeight);
                setting_TryFetch(
                    settings: savedSettings.ii_bool_Snatch_OpenerInWarmup,
                    id: id,
                    defaultGenerator: (_) => Defaults.bool_default_snatch_OpenerInWarmup,
                    setting: out bool snatch_OpenerInWarmup);
                setting_TryFetch(
                    settings: savedSettings.ii_int_Snatch_SecondsEnd,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => Defaults.int_default_snatch_SecondsEnd,
                    setting: out int snatch_SecondsEnd);
                setting_TryFetch(
                    settings: savedSettings.ii_int_Snatch_LiftsOut,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => Defaults.int_default_snatch_LiftsOut,
                    setting: out int snatch_LiftsOut);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SecondsStage,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => Defaults.int_default_cj_SecondsStage,
                    setting: out int cj_SecondsStage);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_OpenerWeight,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => Defaults.int_default_cj_OpenerWeight,
                    setting: out int cj_OpenerWeight);
                setting_TryFetch(
                    settings: savedSettings.ii_bool_CJ_OpenerInWarmup,
                    id: id,
                    defaultGenerator: (_) => Defaults.bool_default_cj_OpenerInWarmup,
                    setting: out bool cj_OpenerInWarmup);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SecondsEnd,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => Defaults.int_default_cj_SecondsEnd,
                    setting: out int cj_SecondsEnd);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_LiftsOut,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => Defaults.int_default_cj_LiftsOut,
                    setting: out int cj_LiftsOut);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SecondsBreak,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => Defaults.int_default_cJ_SecondsBreak,
                    setting: out int cJ_SecondsBreak);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SnatchLifts_Out,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => Defaults.int_default_cJ_SnatchLifts_Out,
                    setting: out int cJ_SnatchLifts_Out);
                setting_TryFetch(
                    settings: savedSettings.ii_bool_Beep,
                    id: id,
                    defaultGenerator: (_) => Defaults.bool_default_Beep,
                    setting: out bool beep);
                List<Extra> snatchExtras = [];
                foreach (string str in savedSettings.ii_strings_SnatchExtras)
                {
                    if (TryParseExtras(record: str, profileId: out int profileId, extra: out Extra extra) && profileId == id)
                    {
                        snatchExtras.Add(extra);
                    }
                }
                Dictionary<int, int> snatchJumps = [];
                foreach (string str in savedSettings.ii_strings_SnatchJumps)
                {
                    if (TryParseJumpTime(record: str, profileId: out int profileId, fromWeight: out int fromWeight, stepValue: out int stepValue) && profileId == id)
                    {
                        snatchJumps[fromWeight] = stepValue;
                    }
                }
                Dictionary<int, int> snatchTimes = [];
                foreach (string str in savedSettings.ii_strings_SnatchTimes)
                {
                    if (TryParseJumpTime(record: str, profileId: out int profileId, fromWeight: out int fromWeight, stepValue: out int stepValue) && profileId == id)
                    {
                        snatchTimes[fromWeight] = stepValue;
                    }
                }
                List<Extra> cJExtras = [];
                foreach (string str in savedSettings.ii_strings_CJExtras)
                {
                    if (TryParseExtras(record: str, profileId: out int profileId, extra: out Extra extra) && profileId == id)
                    {
                        cJExtras.Add(extra);
                    }
                }
                Dictionary<int, int> cJJumps = [];
                foreach (string str in savedSettings.ii_strings_CJJumps)
                {
                    if (TryParseJumpTime(record: str, profileId: out int profileId, fromWeight: out int fromWeight, stepValue: out int stepValue) && profileId == id)
                    {
                        cJJumps[fromWeight] = stepValue;
                    }
                }
                Dictionary<int, int> cJTimes = [];
                foreach (string str in savedSettings.ii_strings_CJTimes)
                {
                    if (TryParseJumpTime(record: str, profileId: out int profileId, fromWeight: out int fromWeight, stepValue: out int stepValue) && profileId == id)
                    {
                        cJTimes[fromWeight] = stepValue;
                    }
                }

                Profile _profile = new(
                    id: id,
                    name: name,
                    barbellWeight: barbellWeight,
                    start: start,
                    snatch_SecondsStage: snatch_SecondsStage,
                    snatch_OpenerWeight: snatch_OpenerWeight,
                    snatch_OpenerInWarmup: snatch_OpenerInWarmup,
                    snatch_SecondsEnd: snatch_SecondsEnd,
                    snatch_LiftsOut: snatch_LiftsOut,
                    cJ_SecondsStage: cj_SecondsStage,
                    cJ_SecondsBreak: cJ_SecondsBreak,
                    cJ_OpenerWeight: cj_OpenerWeight,
                    cJ_OpenerInWarmup: cj_OpenerInWarmup,
                    cJ_SecondsEnd: cj_SecondsEnd,
                    cJ_LiftsOut: cj_LiftsOut,
                    cJ_SnatchLifts_Out: cJ_SnatchLifts_Out,
                    beep: beep,
                    snatchExtras: snatchExtras,
                    snatchJumps: snatchJumps,
                    snatchTimes: snatchTimes,
                    cjExtras: cJExtras,
                    cjJumps: cJJumps,
                    cjTimes: cJTimes);

                profiles[id] = _profile;
            }
            profiles_AddIfNone();
        }
        private void profiles_AddIfNone()
        {
            if (profiles == null || profiles.Count == 0)
            {
                profiles_New(_bool_Demo: true);
            }
        }
        private Profile profiles_New(bool _bool_Demo = false)
        {
            profiles ??= [];
            int id;
            if (profiles.Any())
            {
                id = profiles.Keys.Max() + 1;
            }
            else
            {
                id = 1;
            }
            Profile _profile = new(
                id: id,
                name: (_bool_Demo ? "Demo Profile" : $"Profile_{id}"),
                barbellWeight: Defaults.int_default_Barbell,
                start: Defaults.timeSpan_default_Start,
                snatch_SecondsStage: Defaults.int_default_snatch_SecondsStage,
                snatch_OpenerWeight: Defaults.int_default_snatch_OpenerWeight,
                snatch_OpenerInWarmup: Defaults.bool_default_snatch_OpenerInWarmup,
                snatch_SecondsEnd: Defaults.int_default_snatch_SecondsEnd,
                snatch_LiftsOut: Defaults.int_default_snatch_LiftsOut,
                cJ_SecondsStage: Defaults.int_default_cj_SecondsStage,
                cJ_OpenerWeight: Defaults.int_default_cj_OpenerWeight,
                cJ_OpenerInWarmup: Defaults.bool_default_cj_OpenerInWarmup,
                cJ_SecondsEnd: Defaults.int_default_cj_SecondsEnd,
                cJ_LiftsOut: Defaults.int_default_cj_LiftsOut,
                cJ_SnatchLifts_Out: Defaults.int_default_cJ_SnatchLifts_Out,
                cJ_SecondsBreak: Defaults.int_default_cJ_SecondsBreak,
                beep: Defaults.bool_default_Beep,
                snatchExtras: (_bool_Demo ? Defaults.demo_snatchExtras : []),
                snatchJumps: (_bool_Demo ? Defaults.demo_snatchJumps : Defaults.default_snatchJumps),
                snatchTimes: (_bool_Demo ? Defaults.demo_snatchTimes : Defaults.default_snatchTimes),
                cjExtras: (_bool_Demo ? Defaults.demo_cjExtras : []),
                cjJumps: (_bool_Demo ? Defaults.demo_cjJumps : Defaults.default_cjJumps),
                cjTimes: (_bool_Demo ? Defaults.demo_cjTimes : Defaults.default_cjTimes));
            profiles.Add(_profile.id, _profile);
            return _profile;
        }
        private void setting_TryFetch( // string
            List<string> settings,
            int id,
            Func<int, string> defaultGenerator,
            out string setting)
        {
            setting = settings?.FirstOrDefault(r => r.Length >= 3 && int.TryParse(r.Substring(0, 3), out int i) && i == id);
            if (!string.IsNullOrEmpty(setting))
            {
                setting = (setting.Length > 3 ? setting.Substring(3) : string.Empty);
            }
            if (string.IsNullOrEmpty(setting))
            {
                setting = defaultGenerator(id);
            }
        }
        private void setting_TryFetch( // int
            List<string> settings,
            int id,
            int minValue,
            Func<int, int> defaultGenerator,
            out int setting)
        {
            setting = default;
            string _setting = settings?.FirstOrDefault(r => r.Length >= 3 && int.TryParse(r.Substring(0, 3), out int i) && i == id);
            bool _bool_Default = true;
            if (!string.IsNullOrEmpty(_setting) && _setting.Length > 3)
            {
                _setting = _setting.Substring(3);
                _bool_Default = !int.TryParse(_setting, out setting) || setting < minValue;
            }
            if (_bool_Default)
            {
                setting = defaultGenerator(id);
            }
        }
        private void setting_TryFetch( // bool
            List<string> settings,
            int id,
            Func<int, bool> defaultGenerator,
            out bool setting)
        {
            setting = default;
            string _setting = settings?.FirstOrDefault(r => r.Length >= 3 && int.TryParse(r.Substring(0, 3), out int i) && i == id);
            bool _bool_Default = true;
            if (!string.IsNullOrEmpty(_setting) && _setting.Length > 3)
            {
                _setting = _setting.Substring(3);
                _bool_Default = !bool.TryParse(_setting, out setting);
            }
            if (_bool_Default)
            {
                setting = defaultGenerator(id);
            }
        }
        private void setting_TryFetch( // TimeSpan
            List<string> settings,
            int id,
            Func<int, TimeSpan> defaultGenerator,
            out TimeSpan setting)
        {
            setting = default;
            string _setting = settings?.FirstOrDefault(r => r.Length >= 3 && int.TryParse(r.Substring(0, 3), out int i) && i == id);
            bool _bool_Default = true;
            if (!string.IsNullOrEmpty(_setting) && _setting.Length == 7)
            {
                _setting = _setting.Substring(3);
                if (int.TryParse(_setting.Substring(0, 2), out int hour) &&
                    int.TryParse(_setting.Substring(2, 2), out int minute) &&
                    hour >= 0 && hour < 24 && minute >= 0 && minute < 60)
                {
                    setting = new(hours: hour, minutes: minute, seconds: 0);
                    _bool_Default = false;
                }
            }
            if (_bool_Default)
            {
                setting = defaultGenerator(id);
            }
        }
        private bool ProfileSelect(int _int_ProfileId)
        {
            if (profiles == null || !profiles.TryGetValue(_int_ProfileId, out Profile _profile))
            {
                return false;
            }
            if (_profile.snatchJumps.Count == 0)
            {
                _profile.snatchJumps = Defaults.default_snatchJumps;
            }
            if (_profile.snatchTimes.Count == 0)
            {
                _profile.snatchTimes = Defaults.default_snatchTimes;
            }
            if (_profile.cjJumps.Count == 0)
            {
                _profile.cjJumps = Defaults.default_cjJumps;
            }
            if (_profile.cjTimes.Count == 0)
            {
                _profile.cjTimes = Defaults.default_cjTimes;
            }
            if (!_profile.snatchJumps.ContainsKey(1))
            {
                _profile.snatchJumps[1] = 1;
            }
            if (!_profile.snatchTimes.ContainsKey(1))
            {
                _profile.snatchTimes[1] = 1;
            }
            if (!_profile.cjJumps.ContainsKey(1))
            {
                _profile.cjJumps[1] = 1;
            }
            if (!_profile.cjTimes.ContainsKey(1))
            {
                _profile.cjTimes[1] = 1;
            }
            profileActive = _profile;
            return true;
        }
        private void Load_Profile_Values_To_Controls()
        {
            bool _bool_Loading = bool_Loading;
            bool_Loading = true;

            snatch_Stop_Live();
            cj_Stop_Live();

            AppColors.snatch_Live_BG = splitContainer_snatch.Panel2.BackColor;
            AppColors.cj_Live_BG = splitContainer_cj.Panel2.BackColor;

            if (profileActive.BarbellWeight < numericUpDown_snatch_weight_barbell.Minimum || profileActive.BarbellWeight > numericUpDown_snatch_weight_barbell.Maximum)
            {
                if (Defaults.int_default_Barbell < numericUpDown_snatch_weight_barbell.Minimum || Defaults.int_default_Barbell > numericUpDown_snatch_weight_barbell.Maximum)
                {
                    profileActive.BarbellWeight = (int)numericUpDown_snatch_weight_barbell.Minimum;
                }
                else
                {
                    profileActive.BarbellWeight = Defaults.int_default_Barbell;
                }
            }
            numericUpDown_snatch_weight_barbell.Value = profileActive.BarbellWeight;

            DateTime dateTime = DateTime.Today.Add(profileActive.Start);
            if (dateTime < DateTime.Now)
            {
                dateTime = dateTime.AddDays(1);
            }
            dateTimePicker_snatch_Start.Value = dateTime;

            if (profileActive.snatch_SecondsStage < numericUpDown_snatch_time_stage.Minimum || profileActive.snatch_SecondsStage > numericUpDown_snatch_time_stage.Maximum)
            {
                if (Defaults.int_default_snatch_SecondsStage < numericUpDown_snatch_time_stage.Minimum || Defaults.int_default_snatch_SecondsStage > numericUpDown_snatch_time_stage.Maximum)
                {
                    profileActive.snatch_SecondsStage = (int)numericUpDown_snatch_time_stage.Minimum;
                }
                else
                {
                    profileActive.snatch_SecondsStage = Defaults.int_default_snatch_SecondsStage;
                }
            }

            numericUpDown_snatch_time_stage.Value = profileActive.snatch_SecondsStage;

            if (profileActive.snatch_OpenerWeight < profileActive.BarbellWeight)
            {
                profileActive.snatch_OpenerWeight = Defaults.int_default_snatch_OpenerWeight;
            }
            numericUpDown_snatch_weight_opener.Value = profileActive.snatch_OpenerWeight;

            if (profileActive.snatch_SecondsEnd < numericUpDown_snatch_time_PostWarmup.Minimum)
            {
                profileActive.snatch_SecondsEnd = Defaults.int_default_snatch_SecondsEnd;
            }
            numericUpDown_snatch_time_PostWarmup.Value = profileActive.snatch_SecondsEnd;

            if (profileActive.snatch_LiftsOut < 0)
            {
                profileActive.snatch_LiftsOut = 3;
            }
            else if (profileActive.snatch_LiftsOut > 99)
            {
                profileActive.snatch_LiftsOut = 99;
            }
            label_snatch_Live_LiftsOut.Text = profileActive.snatch_LiftsOut.ToString();
            label_snatch_Live_LiftsPassed.Text = string.Empty;


            if (profileActive.cj_SecondsStage < numericUpDown_cj_time_stage.Minimum || profileActive.cj_SecondsStage > numericUpDown_cj_time_stage.Maximum)
            {
                if (Defaults.int_default_cj_SecondsStage < numericUpDown_cj_time_stage.Minimum || Defaults.int_default_cj_SecondsStage > numericUpDown_cj_time_stage.Maximum)
                {
                    profileActive.cj_SecondsStage = (int)numericUpDown_cj_time_stage.Minimum;
                }
                else
                {
                    profileActive.cj_SecondsStage = Defaults.int_default_cj_SecondsStage;
                }
            }
            numericUpDown_cj_time_stage.Value = profileActive.cj_SecondsStage;

            if (profileActive.cj_SecondsBreak < (numericUpDown_cj_Live_Break.Minimum * 60))
            {
                profileActive.cj_SecondsBreak = Defaults.int_default_cJ_SecondsBreak;
            }
            numericUpDown_cj_Live_Break.Value = (int)((double)profileActive.cj_SecondsBreak / 60);

            if (profileActive.cj_OpenerWeight < profileActive.BarbellWeight)
            {
                profileActive.cj_OpenerWeight = Defaults.int_default_cj_OpenerWeight;
            }
            numericUpDown_cj_weight_opener.Value = profileActive.cj_OpenerWeight;

            if (profileActive.cj_SecondsEnd < numericUpDown_cj_time_PostWarmup.Minimum)
            {
                profileActive.cj_SecondsEnd = Defaults.int_default_cj_SecondsEnd;
            }
            numericUpDown_cj_time_PostWarmup.Value = profileActive.cj_SecondsEnd;

            if (profileActive.cj_LiftsOut < 0)
            {
                profileActive.cj_LiftsOut = 3;
            }
            else if (profileActive.cj_LiftsOut > 99)
            {
                profileActive.cj_LiftsOut = 99;
            }
            label_cj_Live_LiftsOut.Text = profileActive.cj_LiftsOut.ToString();
            label_cj_Live_LiftsPassed.Text = string.Empty;

            if (profileActive.cj_SnatchLifts_Out < 0)
            {
                profileActive.cj_SnatchLifts_Out = 0;
            }
            label_cj_Live_snLeft.Text = profileActive.cj_SnatchLifts_Out.ToString();

            checkBox_snatch_Param_OpenerWarmup.Checked = profileActive.snatch_OpenerInWarmup;
            checkBox_cj_Param_OpenerWarmup.Checked = profileActive.cj_OpenerInWarmup;


            checkBox_snatch_Live_Beep.Checked = profileActive.Beep;
            checkBox_cj_Live_Beep.Checked = profileActive.Beep;

            profileActive.snatchExtras ??= Defaults.default_snatchExtras;
            PopulateExtras(liftType: LiftType.Snatch);

            profileActive.snatchJumps ??= Defaults.default_snatchJumps;
            if (!profileActive.snatchJumps.ContainsKey(1))
            {
                profileActive.snatchJumps[1] = Defaults.default_snatchJumps[1];
            }
            PopulateJumps(liftType: LiftType.Snatch);

            profileActive.snatchTimes ??= Defaults.default_snatchTimes;
            if (!profileActive.snatchTimes.ContainsKey(1))
            {
                profileActive.snatchTimes[1] = Defaults.default_snatchTimes[1];
            }
            PopulateTimes(liftType: LiftType.Snatch);

            PopulateSteps(liftType: LiftType.Snatch);

            profileActive.cjExtras ??= Defaults.default_cjExtras;
            PopulateExtras(liftType: LiftType.CleanAndJerk);

            profileActive.cjJumps ??= Defaults.default_cjJumps;
            if (!profileActive.cjJumps.ContainsKey(1))
            {
                profileActive.cjJumps[1] = Defaults.default_cjJumps[1];
            }
            PopulateJumps(liftType: LiftType.CleanAndJerk);

            profileActive.cjTimes ??= Defaults.default_cjTimes;
            if (!profileActive.cjTimes.ContainsKey(1))
            {
                profileActive.cjTimes[1] = Defaults.default_cjTimes[1];
            }
            PopulateTimes(liftType: LiftType.CleanAndJerk);

            PopulateSteps(liftType: LiftType.CleanAndJerk);

            ApplyOpener(liftType: LiftType.Snatch);
            ApplyOpener(liftType: LiftType.CleanAndJerk);

            bool_Loading = _bool_Loading;
        }
        private Profile Add_Profile(string _str_ProfileName)
        {
            Profile _profile = profiles_New(_bool_Demo: false);
            _profile.Name = _str_ProfileName;
            SaveSettings();
            return _profile;
        }
        private bool ParseOutSetting(string record, out int id, out int setting)
        {
            setting = default;
            bool b = ParseOutSetting(record: record, id: out id, setting: out string _setting) &&
                int.TryParse(_setting, out setting);
            Console.WriteLine(b ?
                $"Loaded {setting.GetType().Name} - record: {record} - id: {id} - value out: {setting}" :
                $"Failed to load {setting.GetType().Name} - record: {record} - id: {id}");
            return b;
        }
        private bool ParseOutSetting(string record, out int id, out bool setting)
        {
            setting = default;
            bool b = ParseOutSetting(record: record, id: out id, setting: out string _setting) &&
                bool.TryParse(_setting, out setting);
            Console.WriteLine(b ?
                $"Loaded {setting.GetType().Name} - record: {record} - id: {id} - value out: {setting}" :
                $"Failed to load {setting.GetType().Name} - record: {record} - id: {id}");
            return b;
        }
        private bool ParseOutSetting(string record, out int id, out TimeSpan setting)
        {
            if (ParseOutSetting(record: record, id: out id, setting: out string _setting) && _setting.Length == 4)
            {
                setting = new(
                    hours: int.Parse(_setting.Substring(0, 2)),
                    minutes: int.Parse(_setting.Substring(2, 2)),
                    seconds: 0);
                Console.WriteLine($"Loaded {setting.GetType().Name} - record: {record} - id: {id} - value out: {setting}");
                return true;
            }
            setting = default;
            Console.WriteLine($"Failed to load {setting.GetType().Name} - record: {record} - id: {id}");
            return false;
        }
        private bool ParseOutSetting(string record, out int id, out string setting)
        {
            if (string.IsNullOrEmpty(record) || record.Length < 3 ||
                !int.TryParse(record.Substring(0, 3), out id))
            {
                id = -1;
                setting = default;
                Console.WriteLine($"Failed to load {setting.GetType().Name} - record: {record} - id: {id}");
                return false;
            }
            else
            {
                setting = (record.Length > 3 ? record.Substring(3) : string.Empty);
                Console.WriteLine($"Loaded {setting.GetType().Name} - record: {record} - id: {id} - value out: {setting}");
                return true;
            }
        }
        private bool TryParseExtras(
            string record,
            out int profileId,
            out Extra extra)
        {
            extra = default;
            if (!ParseOutSetting(record: record, id: out profileId, setting: out string setting))
            {
                return false;
            }
            //  at character:
            //  0   3 digit id
            //  3   3 digit order
            //  6   5 digit length
            //  11  variable length string (action name)
            if (setting.Length >= 11 &&
                int.TryParse(setting.Substring(0, 3), out int extraId) && extraId > -1 &&
                int.TryParse(setting.Substring(3, 3), out int order) && order > -1 &&
                int.TryParse(setting.Substring(6, 5), out int length) && length > 0)
            {
                string action = (setting.Length == 11 ? string.Empty : setting.Substring(11, setting.Length - 11));
                Console.WriteLine($"Loaded Extra - record: {record} - profileId: {profileId} - id: {extraId} - order: {order} - length: {length} - action: {action}");
                extra = new(id: extraId, action: action, length: length, order: order);
                return true;
            }
            Console.WriteLine($"Failed to load Extra - record: {record}");
            return false;
        }
        private bool TryParseJumpTime(string record, out int profileId, out int fromWeight, out int stepValue)
        {
            fromWeight = 0;
            stepValue = 0;
            if (!ParseOutSetting(record: record, id: out profileId, setting: out string setting))
            {
                return false;
            }
            if (setting.Length != 6)
            {
                return false;
            }
            bool b = int.TryParse(setting.Substring(0, 3), out fromWeight) &&
                int.TryParse(setting.Substring(3, 3), out stepValue) && stepValue > 0;
            Console.WriteLine(b ?
                $"Loaded Extra - record: {record} - profileId: {profileId} - fromWeight: {fromWeight} - stepValue: {stepValue}" :
                $"Failed to load Extra - record: {record}");
            return b;
        }
        private void ClearSettings()
        {
            if (MessageBox.Show($"This will erase all profiles and restore all defaults.{Environment.NewLine}{Environment.NewLine}Continue?",
                "Reset settings?", buttons: MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                savedSettings.Reset();
                savedSettings.Save();
                profiles.Clear();
                profiles_AddIfNone();
                int _int_ProfileId = profiles.FirstOrDefault().Key;
                if (!ProfileSelect(_int_ProfileId: _int_ProfileId))
                {
                    MessageBox.Show("An error occurred and the profile could not be loaded. Please reopen and try again.", "Profile Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Populate_MenuStrip();
                Load_Profile_Values_To_Controls();
            }
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
            foreach (KeyValuePair<int, Profile> _profilePair in profiles)
            {
                ToolStripMenuItem toolStripMenuItem = new()
                {
                    Text = _profilePair.Value.Name,
                    Tag = _profilePair.Key.ToString()
                };
                ToolStripButton toolStripButton;
                if (_profilePair.Key == profileActive.id)
                {
                    toolStripMenuItem.BackColor = Color.Red;
                }
                else
                {
                    toolStripButton = new()
                    {
                        Text = "load",
                        Tag = _profilePair.Key.ToString(),
                    };
                    toolStripButton.Click += ToolStripMenu_Load_Profile;
                    toolStripMenuItem.DropDownItems.Add(toolStripButton);
                }
                toolStripButton = new()
                {
                    Text = "delete",
                    Tag = _profilePair.Key.ToString(),
                };
                toolStripButton.Click += ToolStripMenu_Delete_Profile;
                toolStripMenuItem.DropDownItems.Add(toolStripButton);
                menuStrip_Profile.Items.Add(toolStripMenuItem);
                toolStripButton = new()
                {
                    Text = "duplicate",
                    Tag = _profilePair.Key.ToString(),
                };
                toolStripButton.Click += ToolStripMenu_Duplicate_Profile;
                toolStripMenuItem.DropDownItems.Add(toolStripButton);
                menuStrip_Profile.Items.Add(toolStripMenuItem);
                toolStripButton = new()
                {
                    Text = "rename",
                    Tag = _profilePair.Key.ToString(),
                };
                toolStripButton.Click += ToolStripMenu_Rename_Profile;
                toolStripMenuItem.DropDownItems.Add(toolStripButton);
                menuStrip_Profile.Items.Add(toolStripMenuItem);
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
        private void Load_Profile(ToolStripButton toolStripButton)
        {
            SaveSettings();
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId))
            {
                if (!ProfileSelect(_int_ProfileId: _int_ProfileId))
                {
                    MessageBox.Show("An error occurred and the profile could not be loaded. Please reopen and try again.", "Profile Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }
                Populate_MenuStrip();
                Load_Profile_Values_To_Controls();
            }
        }
        private void Delete_Profile(ToolStripButton toolStripButton)
        {
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId) &&
                profiles.ContainsKey(_int_ProfileId) &&
                MessageBox.Show(
                    text: "Are you sure you want to delete this profile? This action is permanent.",
                    caption: "Delete",
                    buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int _int_SelectedProfileId = profileActive.id;
                profiles.Remove(_int_ProfileId);
                profiles_AddIfNone();
                SaveSettings();
                if (_int_ProfileId == _int_SelectedProfileId)
                {
                    _int_ProfileId = profiles.FirstOrDefault().Key;
                    if (!ProfileSelect(_int_ProfileId: _int_ProfileId))
                    {
                        MessageBox.Show("An error occurred and the profile could not be loaded. Please reopen and try again.", "Profile Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                        return;
                    }
                    Populate_MenuStrip();
                    Load_Profile_Values_To_Controls();
                }
                else
                {
                    Populate_MenuStrip();
                }
            }
        }
        private void AddNew_Profile()
        {
            string _str_Name = Interaction.InputBox("Enter a new name:");
            if (!string.IsNullOrEmpty(_str_Name))
            {
                Add_Profile(_str_ProfileName: _str_Name);
                Populate_MenuStrip();
            }
        }
        private void Rename_Profile(ToolStripButton toolStripButton)
        {
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId) &&
                profiles.TryGetValue(_int_ProfileId, out Profile _profile))
            {
                string _str_Name = Interaction.InputBox(Prompt: "Enter a new name:", DefaultResponse: _profile.Name);
                if (!string.IsNullOrEmpty(_str_Name))
                {
                    _profile.Name = _str_Name;
                    SaveSettings();
                    Populate_MenuStrip();
                }
            }
        }
        private void Duplicate_Profile(ToolStripButton toolStripButton)
        {
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId) &&
                profiles.TryGetValue(_int_ProfileId, out Profile _profile_ToCopy))
            {
                string _str_Name = Interaction.InputBox(Prompt: "Enter a new name:", DefaultResponse: _profile_ToCopy.Name);
                if (!string.IsNullOrEmpty(_str_Name) && !string.Equals(_str_Name, _profile_ToCopy.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    int idNew = profiles.Max(r => r.Key) + 1;
                    Profile _profile_New = _profile_ToCopy.Clone(idNew: idNew);
                    _profile_New.Name = _str_Name;
                    profiles[idNew] = _profile_New;
                    SaveSettings();
                }
            }
            else
            {
                MessageBox.Show("Unable to duplicate, could not find the original. Reopen the app to fix.", "Profile duplication error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Populate_MenuStrip();
        }
    }
}
