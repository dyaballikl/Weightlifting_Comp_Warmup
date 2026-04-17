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
                    defaultGenerator: (_) => int_default_Barbell,
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
                    defaultGenerator: (_) => int_default_snatch_SecondsStage,
                    setting: out int snatch_SecondsStage);
                setting_TryFetch(
                    settings: savedSettings.ii_int_Snatch_OpenerWeight,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => int_default_snatch_OpenerWeight,
                    setting: out int snatch_OpenerWeight);
                setting_TryFetch(
                    settings: savedSettings.ii_bool_Snatch_OpenerInWarmup,
                    id: id,
                    defaultGenerator: (_) => bool_default_snatch_OpenerInWarmup,
                    setting: out bool snatch_OpenerInWarmup);
                setting_TryFetch(
                    settings: savedSettings.ii_int_Snatch_SecondsEnd,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => int_default_snatch_SecondsEnd,
                    setting: out int snatch_SecondsEnd);
                setting_TryFetch(
                    settings: savedSettings.ii_int_Snatch_LiftsOut,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => int_default_snatch_Lifts_Out,
                    setting: out int snatch_LiftsOut);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SecondsStage,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => int_default_cj_SecondsStage,
                    setting: out int cj_SecondsStage);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_OpenerWeight,
                    id: id,
                    minValue: 1,
                    defaultGenerator: (_) => int_default_cj_OpenerWeight,
                    setting: out int cj_OpenerWeight);
                setting_TryFetch(
                    settings: savedSettings.ii_bool_CJ_OpenerInWarmup,
                    id: id,
                    defaultGenerator: (_) => bool_default_cj_OpenerInWarmup,
                    setting: out bool cj_OpenerInWarmup);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SecondsEnd,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => int_default_cj_SecondsEnd,
                    setting: out int cj_SecondsEnd);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_LiftsOut,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => int_default_cj_LiftsOut,
                    setting: out int cj_LiftsOut);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SecondsBreak,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => int_default_cJ_SecondsBreak,
                    setting: out int cJ_SecondsBreak);
                setting_TryFetch(
                    settings: savedSettings.ii_int_CJ_SnatchLifts_Out,
                    id: id,
                    minValue: 0,
                    defaultGenerator: (_) => int_default_cJ_SnatchLifts_Out,
                    setting: out int cJ_SnatchLifts_Out);
                setting_TryFetch(
                    settings: savedSettings.ii_bool_Beep,
                    id: id,
                    defaultGenerator: (_) => bool_default_Beep,
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
                    cJExtras: cJExtras,
                    cJJumps: cJJumps,
                    cJTimes: cJTimes);

                profiles[id] = _profile;
            }
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
            if (_profile.SnatchExtras.Count == 0)
            {
                _profile.SnatchExtras = Defaults.default_snatchExtras();
            }
            if (_profile.SnatchJumps.Count == 0)
            {
                _profile.SnatchJumps = Defaults.default_snatchJumps();
            }
            if (_profile.SnatchTimes.Count == 0)
            {
                _profile.SnatchTimes = Defaults.default_snatchTimes();
            }
            if (_profile.CJExtras.Count == 0)
            {
                _profile.CJExtras = Defaults.default_cjExtras();
            }
            if (_profile.CJJumps.Count == 0)
            {
                _profile.CJJumps = Defaults.default_cjJumps();
            }
            if (_profile.CJTimes.Count == 0)
            {
                _profile.CJTimes = Defaults.default_cjTimes();
            }
            if (!_profile.SnatchJumps.TryGetValue(1, out _))
            {
                _profile.SnatchJumps[1] = 1;
            }
            if (!_profile.SnatchTimes.TryGetValue(1, out _))
            {
                _profile.SnatchTimes[1] = 1;
            }
            if (!_profile.CJJumps.TryGetValue(1, out _))
            {
                _profile.CJJumps[1] = 1;
            }
            if (!_profile.CJTimes.TryGetValue(1, out _))
            {
                _profile.CJTimes[1] = 1;
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

            color_snatch_Live_BG = splitContainer_snatch.Panel2.BackColor;
            color_cj_Live_BG = splitContainer_cj.Panel2.BackColor;

            if (profileActive.BarbellWeight < numericUpDown_snatch_weight_barbell.Minimum)
            {
                profileActive.BarbellWeight = int_default_Barbell;
            }
            numericUpDown_snatch_weight_barbell.Value = profileActive.BarbellWeight;

            DateTime dateTime = DateTime.Today.Add(profileActive.Start);
            if (dateTime < DateTime.Now)
            {
                dateTime = dateTime.AddDays(1);
            }
            dateTimePicker_snatch_Start.Value = dateTime;

            if (profileActive.Snatch_SecondsStage < numericUpDown_snatch_time_stage.Minimum)
            {
                profileActive.Snatch_SecondsStage = int_default_snatch_SecondsStage;
            }
            numericUpDown_snatch_time_stage.Value = profileActive.Snatch_SecondsStage;

            if (profileActive.Snatch_OpenerWeight < profileActive.BarbellWeight)
            {
                profileActive.Snatch_OpenerWeight = int_default_snatch_OpenerWeight;
            }
            numericUpDown_snatch_weight_opener.Value = profileActive.Snatch_OpenerWeight;

            if (profileActive.Snatch_SecondsEnd < numericUpDown_snatch_time_PostWarmup.Minimum)
            {
                profileActive.Snatch_SecondsEnd = int_default_snatch_SecondsEnd;
            }
            numericUpDown_snatch_time_PostWarmup.Value = profileActive.Snatch_SecondsEnd;

            if (profileActive.Snatch_LiftsOut < 0)
            {
                profileActive.Snatch_LiftsOut = 3;
            }
            else if (profileActive.Snatch_LiftsOut > 99)
            {
                profileActive.Snatch_LiftsOut = 99;
            }
            label_snatch_Live_LiftsOut.Text = profileActive.Snatch_LiftsOut.ToString();
            label_snatch_Live_LiftsPassed.Text = string.Empty;


            if (profileActive.CJ_SecondsStage < numericUpDown_cj_time_stage.Minimum)
            {
                profileActive.CJ_SecondsStage = int_default_cj_SecondsStage;
            }
            numericUpDown_cj_time_stage.Value = profileActive.CJ_SecondsStage;

            if (profileActive.CJ_SecondsBreak < (numericUpDown_cj_Live_Break.Minimum * 60))
            {
                profileActive.CJ_SecondsBreak = int_default_cJ_SecondsBreak;
            }
            numericUpDown_cj_Live_Break.Value = (int)((double)profileActive.CJ_SecondsBreak / 60);

            if (profileActive.CJ_OpenerWeight < profileActive.BarbellWeight)
            {
                profileActive.CJ_OpenerWeight = int_default_cj_OpenerWeight;
            }
            numericUpDown_cj_weight_opener.Value = profileActive.CJ_OpenerWeight;

            if (profileActive.CJ_SecondsEnd < numericUpDown_cj_time_PostWarmup.Minimum)
            {
                profileActive.CJ_SecondsEnd = int_default_cj_SecondsEnd;
            }
            numericUpDown_cj_time_PostWarmup.Value = profileActive.CJ_SecondsEnd;

            if (profileActive.CJ_LiftsOut < 0)
            {
                profileActive.CJ_LiftsOut = 3;
            }
            else if (profileActive.CJ_LiftsOut > 99)
            {
                profileActive.CJ_LiftsOut = 99;
            }
            label_cj_Live_LiftsOut.Text = profileActive.CJ_LiftsOut.ToString();
            label_cj_Live_LiftsPassed.Text = string.Empty;

            if (profileActive.CJ_SnatchLifts_Out < 0)
            {
                profileActive.CJ_SnatchLifts_Out = 0;
            }
            label_cj_Live_snLeft.Text = profileActive.CJ_SnatchLifts_Out.ToString();

            checkBox_snatch_Param_OpenerWarmup.Checked = profileActive.Snatch_OpenerInWarmup;
            checkBox_cj_Param_OpenerWarmup.Checked = profileActive.CJ_OpenerInWarmup;


            checkBox_snatch_Live_Beep.Checked = profileActive.Beep;
            checkBox_cj_Live_Beep.Checked = profileActive.Beep;

            bool bool_AutoVals;

            bool_AutoVals = true;
            if (default_snatchExtras != null && default_snatchExtras.Count > 0)
            {
                profileActive.SnatchExtras = [.. default_snatchExtras];
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                profileActive.SnatchExtras = Defaults.default_snatchExtras();
            }
            PopulateExtras(liftType: LiftType.Snatch);

            bool_AutoVals = true;
            if (default_snatchJumps != null && default_snatchJumps.Count > 0)
            {
                profileActive.SnatchJumps = new(default_snatchJumps);
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                profileActive.SnatchJumps = Defaults.default_snatchJumps();
            }
            PopulateJumps(liftType: LiftType.Snatch);

            bool_AutoVals = true;
            if (default_snatchTimes != null && default_snatchTimes.Count > 0)
            {
                profileActive.SnatchTimes = new(default_snatchTimes);
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                profileActive.SnatchTimes = Defaults.default_snatchTimes();
            }
            PopulateTimes(liftType: LiftType.Snatch);

            PopulateSteps(liftType: LiftType.Snatch, preserveLifts: false);

            bool_AutoVals = true;
            if (default_cjExtras != null && default_cjExtras.Count > 0)
            {
                profileActive.CJExtras = [.. default_cjExtras];
                bool_AutoVals = false;
            }
            if (bool_AutoVals)
            {
                profileActive.CJExtras = Defaults.default_cjExtras();
            }
            PopulateExtras(liftType: LiftType.CleanAndJerk);

            bool_AutoVals = true;
            if (default_cjJumps != null)
            {
                if (default_cjJumps.Count > 0)
                {
                    profileActive.CJJumps = new(default_cjJumps);
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                profileActive.CJJumps = Defaults.default_cjJumps();
            }
            PopulateJumps(liftType: LiftType.CleanAndJerk);

            bool_AutoVals = true;
            if (default_cjTimes != null)
            {
                if (default_cjTimes.Count > 0)
                {
                    profileActive.CJTimes = new(default_cjTimes);
                    bool_AutoVals = false;
                }
            }
            if (bool_AutoVals)
            {
                profileActive.CJTimes = Defaults.default_cjTimes();
            }
            PopulateTimes(liftType: LiftType.CleanAndJerk);

            PopulateSteps(liftType: LiftType.CleanAndJerk, preserveLifts: false);

            ApplyOpener(liftType: LiftType.Snatch);
            ApplyOpener(liftType: LiftType.CleanAndJerk);

            bool_Loading = _bool_Loading;
        }
        private Profile Add_Profile(string _str_ProfileName)
        {
            int _int_ProfileId;
            if (profiles.Count == 0)
            {
                _int_ProfileId = 1;
            }
            else
            {
                _int_ProfileId = profiles.Max(r => r.Key) + 1;
            }
            Profile _profile = new(
                id: _int_ProfileId,
                name: _str_ProfileName,
                barbellWeight: int_default_Barbell,
                start: new(hours: 9, minutes: 0, seconds: 0),
                snatch_SecondsStage: int_default_snatch_SecondsStage,
                snatch_OpenerWeight: int_default_snatch_OpenerWeight,
                snatch_OpenerInWarmup: bool_default_snatch_OpenerInWarmup,
                snatch_SecondsEnd: int_default_snatch_SecondsEnd,
                snatch_LiftsOut: int_default_snatch_Lifts_Out,
                cJ_SecondsStage: int_default_cj_SecondsStage,
                cJ_SecondsBreak: int_default_cJ_SecondsBreak,
                cJ_OpenerWeight: int_default_cj_OpenerWeight,
                cJ_OpenerInWarmup: bool_default_cj_OpenerInWarmup,
                cJ_SecondsEnd: int_default_cj_SecondsEnd,
                cJ_LiftsOut: int_default_cj_LiftsOut,
                cJ_SnatchLifts_Out: int_default_cJ_SnatchLifts_Out,
                beep: bool_default_Beep,
                snatchExtras: [new()],
                snatchJumps: [],
                snatchTimes: [],
                cJExtras: [],
                cJJumps: [],
                cJTimes: []);
            profiles.Add(_int_ProfileId, _profile);
            SaveSettings();
            return _profile;
        }
        private bool ParseOutSetting(string record, out int id, out int setting)
        {
            setting = default;
            return ParseOutSetting(record: record, id: out id, setting: out string _setting) &&
                int.TryParse(_setting, out setting);
        }
        private bool ParseOutSetting(string record, out int id, out bool setting)
        {
            setting = default;
            return ParseOutSetting(record: record, id: out id, setting: out string _setting) &&
                bool.TryParse(_setting, out setting);
        }
        private bool ParseOutSetting(string record, out int id, out TimeSpan setting)
        {
            if (ParseOutSetting(record: record, id: out id, setting: out string _setting) && _setting.Length == 4)
            {
                setting = new(
                    hours: int.Parse(_setting.Substring(0, 2)),
                    minutes: int.Parse(_setting.Substring(2, 2)),
                    seconds: 0);
                return true;
            }
            setting = default;
            return false;
        }
        private bool ParseOutSetting(string record, out int id, out string setting)
        {
            if (string.IsNullOrEmpty(record) || record.Length < 3 ||
                !int.TryParse(record.Substring(0, 3), out id))
            {
                id = -1;
                setting = default;
                return false;
            }
            else
            {
                setting = (record.Length > 3 ? record.Substring(3) : string.Empty);
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
            if (setting.Length < 11 &&
                int.TryParse(setting.Substring(0, 3), out int extraId) && extraId > -1 &&
                int.TryParse(setting.Substring(3, 3), out int order) && order > -1 &&
                int.TryParse(setting.Substring(6, 5), out int length) && length > 0)
            {
                string action = (record.Length == 11 ? string.Empty : record.Substring(11, record.Length - 11));
                extra = new(id: extraId, action: action, length: length, order: order);
                return true;
            }
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
            return int.TryParse(record.Substring(0, 3), out fromWeight) &&
                   int.TryParse(record.Substring(3, 3), out stepValue) && stepValue > 0;
        }
        private void button_snatch_ClearSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"This will erase all profiles and restore all defaults.{Environment.NewLine}{Environment.NewLine}Continue?",
                "Reset settings?", buttons: MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                savedSettings.Reset();
                savedSettings.Save();
                profiles.Clear();
                profileActive = Add_Profile(_str_ProfileName: "default");
                Initialise_Form();
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
        private void ToolStripMenu_Load_Profile(object sender, EventArgs e)
        {
            SaveSettings();
            ToolStripButton toolStripButton = (ToolStripButton)sender;
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
        private void ToolStripMenu_Delete_Profile(object sender, EventArgs e)
        {
            ToolStripButton toolStripButton = (ToolStripButton)sender;
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
                SaveSettings();
                if (_int_ProfileId == _int_SelectedProfileId)
                {
                    if (profiles.Any())
                    {
                        _int_ProfileId = profiles.First().Key;
                    }
                    else
                    {
                        _int_ProfileId = Add_Profile(_str_ProfileName: "default").id;
                    }
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
        private void ToolStripMenu_Duplicate_Profile(object sender, EventArgs e)
        {
            ToolStripButton toolStripButton = (ToolStripButton)sender;
            string _string_Tag = toolStripButton.Tag.ToString();
            if (int.TryParse(s: _string_Tag, result: out int _int_ProfileId) &&
                profiles.TryGetValue(_int_ProfileId, out Profile _profile_ToCopy))
            {
                string _str_Name = Interaction.InputBox(Prompt: "Enter a new name:", DefaultResponse: _profile_ToCopy.Name);
                if (!string.IsNullOrEmpty(_str_Name) && !string.Equals(_str_Name, _profile_ToCopy.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    int idNew = profiles.Max(r => r.Key) + 1;
                    Profile _profile_New = _profile_ToCopy.Clone(idNew: idNew);
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
