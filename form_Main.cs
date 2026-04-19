using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
    public partial class form_Main : Form
    {
        #region Form Level
        public form_Main()
        {
            //if (Debugger.IsAttached)
            //{
            //    savedSettings.Reset();
            //}
            LoadSettings();
            SaveSettings();

            int _int_ProfileId;
            try
            {
                _int_ProfileId = savedSettings.int_ProfileId;
            }
            catch
            {
                _int_ProfileId = -1;
            }

            if (_int_ProfileId < 1 && profiles.Any())
            {
                _int_ProfileId = profiles.Min(r => r.Key);
            }
            if (_int_ProfileId < 1)
            {
                _int_ProfileId = Add_Profile(_str_ProfileName: "DefaultName_1").id;
            }
            if (!ProfileSelect(_int_ProfileId: _int_ProfileId))
            {
                MessageBox.Show("An error occurred and the profile could not be loaded. Please reopen and try again.", "Profile Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            InitializeComponent();
            numericUpDown_snatch_time_stage.ValueChanged += (s, ev) => numericUpDown_time_stage_ValueChanged(s, ev, LiftType.Snatch);
            numericUpDown_snatch_weight_opener.ValueChanged += (s, ev) => Opener_Set(LiftType.Snatch);
            checkBox_snatch_Param_OpenerWarmup.CheckedChanged += (s, ev) => Opener_Set(LiftType.Snatch);
            numericUpDown_snatch_time_PostWarmup.ValueChanged += (s, ev) => numericUpDown_time_PostWarmup_ValueChanged(s, ev, LiftType.Snatch);
            numericUpDown_cj_time_stage.ValueChanged += (s, ev) => numericUpDown_time_stage_ValueChanged(s, ev, LiftType.CleanAndJerk);
            numericUpDown_cj_weight_opener.ValueChanged += (s, ev) => Opener_Set(LiftType.CleanAndJerk);
            checkBox_cj_Param_OpenerWarmup.CheckedChanged += (s, ev) => Opener_Set(LiftType.CleanAndJerk);
            numericUpDown_cj_time_PostWarmup.ValueChanged += (s, ev) => numericUpDown_time_PostWarmup_ValueChanged(s, ev, LiftType.CleanAndJerk);
            dataGridView_snatch_steps.AutoGenerateColumns = false;
            dataGridView_cj_steps.AutoGenerateColumns = false;
        }
        private void Form_WL_Comp_Warmup_Load(object sender, EventArgs e)
        {
            Bounds = Screen.PrimaryScreen.Bounds;
            Initialise_Form();
        }
        private void Initialise_Form()
        {
            bool_Loading = true;
            Populate_MenuStrip();

            Load_Profile_Values_To_Controls();

            buttonClose.BringToFront();
            bool_Loading = false;
        }
        private void Restore()
        {
            buttonRestore.Dispose();
            buttonClose.Dispose();
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
        }
        private void Form_WL_Comp_Warmup_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }
        private void snatch_weight_barbell_ValueChanged()
        {
            if (bool_Loading) { return; }
            int _int_Barbell;
            try
            {
                _int_Barbell = (int)(numericUpDown_snatch_weight_barbell.Value);
            }
            catch
            {
                (numericUpDown_snatch_weight_barbell).BackColor = Color.Yellow;
                return;
            }

            if (_int_Barbell < 1)
            {
                (numericUpDown_snatch_weight_barbell).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_snatch_weight_barbell).BackColor = Color.White;
            profileActive.BarbellWeight = _int_Barbell;
            numericUpDown_snatch_weight_opener.Minimum = _int_Barbell;
            numericUpDown_cj_weight_opener.Minimum = _int_Barbell;
            ApplyOpener(liftType: LiftType.Snatch);
            ApplyOpener(liftType: LiftType.CleanAndJerk);
            PopulateSteps(LiftType.Snatch);
            PopulateSteps(LiftType.CleanAndJerk);
        }
        private void Snatch_CJ_TabSwitch()
        {
            snatch_Stop_Live();
            cj_Stop_Live();
        }
        private void PreventMonitorPowerdown()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }
        private void AllowMonitorPowerdown()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }
        #endregion

        #region Control Handlers
        #region Form level controls
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void buttonRestore_Click(object sender, EventArgs e)
        {
            Restore();
        }
        private void tabControl_SnatchCJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            Snatch_CJ_TabSwitch();
        }
        #endregion
        #region Live Timers
        private void timer_Battery_Tick(object sender, EventArgs e)
        {
            UpdateBattery();
        }
        private void timer_Live_Tick(object sender, EventArgs e, LiftType liftType)
        {
            Timer timer = (sender as Timer);
            timer_Live_Tick(timer: timer, liftType: liftType);
        }
        #endregion
        #region Snatch controls
        private void numericUpDown_snatch_weight_barbell_ValueChanged(object sender, EventArgs e)
        {
            snatch_weight_barbell_ValueChanged();
        }
        private void button_snatch_Live_StartStop_Click(object sender, EventArgs e)
        {
            button_snatch_Live_StartStop_Click();
        }
        private void progressBar_snatch_Live_StageLift_MouseClick(object sender, MouseEventArgs e)
        {
            progressBar_snatch_Live_StageLift_MouseClick(e: e);
        }
        private void button_snatch_Live_LiftsDecr_Click(object sender, EventArgs e)
        {
            button_snatch_Live_LiftsDecr_Click();
        }
        private void button_snatch_Live_LiftsIncr_Click(object sender, EventArgs e)
        {
            button_snatch_Live_LiftsIncr_Click();
        }
        private void button_snatch_Live_StageAdvance_Click(object sender, EventArgs e)
        {
            button_snatch_Live_StageAdvance_Click();
        }
        private void label_snatch_Live_LiftsOut_Click(object sender, EventArgs e)
        {
            label_snatch_Live_LiftsOut_Click();
        }
        private void textBox_snatch_Live_LiftsOut_TextChanged(object sender, EventArgs e)
        {
            textBox_snatch_Live_LiftsOut_TextChanged();
        }
        private void textBox_snatch_Live_LiftsOut_Leave(object sender, EventArgs e)
        {
            textBox_snatch_Live_LiftsOut_Leave();
        }
        private void dateTimePicker_snatch_Start_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_snatch_Start_ValueChanged();
        }
        private void checkBox_snatch_Live_Auto_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_snatch_Live_Auto_CheckedChanged();
        }
        private void checkBox_Live_Beep_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_Live_Beep_CheckedChanged(checkBox: (CheckBox)sender);
        }
        private void splitContainer_snatch_DoubleClick(object sender, EventArgs e)
        {
            splitContainer_snatch_DoubleClick();
        }
        #endregion
        #region CJ controls
        private void button_cj_Live_StartStop_Click(object sender, EventArgs e)
        {
            button_cj_Live_StartStop_Click();
        }
        private void progressBar_cj_Live_StageLift_MouseClick(object sender, MouseEventArgs e)
        {
            progressBar_cj_Live_StageLift_MouseClick(e: e);
        }
        private void progressBar_cj_Live_sn_MouseClick(object sender, MouseEventArgs e)
        {
            progressBar_cj_Live_sn_MouseClick(e: e);
        }
        private void progressBar_cj_Live_Break_MouseClick(object sender, MouseEventArgs e)
        {
            progressBar_cj_Live_Break_MouseClick(e: e);
        }
        private void button_cj_Live_LiftsDecr_Click(object sender, EventArgs e)
        {
            button_cj_Live_LiftsDecr_Click();
        }
        private void button_cj_Live_LiftsIncr_Click(object sender, EventArgs e)
        {
            button_cj_Live_LiftsIncr_Click();
        }
        private void label_cj_Live_LiftsOut_Click(object sender, EventArgs e)
        {
            label_cj_Live_LiftsOut_Click();
        }
        private void textBox_cj_Live_LiftsOut_TextChanged(object sender, EventArgs e)
        {
            textBox_cj_Live_LiftsOut_TextChanged();
        }
        private void textBox_cj_Live_LiftsOut_Leave(object sender, EventArgs e)
        {
            textBox_cj_Live_LiftsOut_Leave();
        }
        private void button_cj_Live_StageAdvance_Click(object sender, EventArgs e)
        {
            button_cj_Live_StageAdvance_Click();
        }
        private void checkBox_cj_Live_Auto_CheckedChanged(object sender, EventArgs e)
        {
            checkBox_cj_Live_Auto_CheckedChanged();
        }
        private void numericUpDown_cj_Live_Break_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown_cj_Live_Break_ValueChanged();
        }
        private void button_cj_Live_snDecr_Click(object sender, EventArgs e)
        {
            button_cj_Live_snDecr_Click();
        }
        private void button_cj_Live_snIncr_Click(object sender, EventArgs e)
        {
            button_cj_Live_snIncr_Click();
        }
        private void label_cj_Live_snLeft_Click(object sender, EventArgs e)
        {
            label_cj_Live_snLeft_Click();
        }
        private void textBox_cj_Live_snLeft_TextChanged(object sender, EventArgs e)
        {
            textBox_cj_Live_snLeft_TextChanged();
        }
        private void textBox_cj_Live_snLeft_Leave(object sender, EventArgs e)
        {
            textBox_cj_Live_snLeft_Leave();
        }
        private void label_cj_Live_Break_Click(object sender, EventArgs e)
        {
            label_cj_Live_Break_Click();
        }
        private void textBox_cj_Live_Break_TextChanged(object sender, EventArgs e)
        {
            textBox_cj_Live_Break_TextChanged();
        }
        private void textBox_cj_Live_Break_Leave(object sender, EventArgs e)
        {
            textBox_cj_Live_Break_Leave();
        }
        private void button_cj_Live_snStageAdvance_Click(object sender, EventArgs e)
        {
            Cj_Advance_SnatchLift();
        }
        private void splitContainer_cj_DoubleClick(object sender, EventArgs e)
        {
            Cj_Splitter_DoubleClick();
        }
        #endregion
        #region Save Settings buttons
        private void button_SaveSettings_Click(object sender, EventArgs eventArgs)
        {
            SaveSettings();
        }
        private void button_snatch_ClearSettings_Click(object sender, EventArgs e)
        {
            ClearSettings();
        }
        #endregion
        #region ToolStripMenu buttons
        private void ToolStripMenu_Load_Profile(object sender, EventArgs e)
        {
            Load_Profile(toolStripButton: (ToolStripButton)sender);
        }
        private void ToolStripMenu_Delete_Profile(object sender, EventArgs e)
        {
            Delete_Profile(toolStripButton: (ToolStripButton)sender);
        }
        private void ToolStripMenu_AddNew_Profile(object sender, EventArgs e)
        {
            AddNew_Profile();
        }
        private void ToolStripMenu_Rename_Profile(object sender, EventArgs e)
        {
            Rename_Profile(toolStripButton: (ToolStripButton)sender);
        }
        private void ToolStripMenu_Duplicate_Profile(object sender, EventArgs e)
        {
            Duplicate_Profile(toolStripButton: (ToolStripButton)sender);
        }
        #endregion
        #region extras
        private void button_extra_up_click(object sender, EventArgs e, LiftType liftType)
        {
            int id = (int)((Button)sender).Tag;
            button_extra_up_click(id: id, liftType: liftType);
        }
        private void button_extra_down_click(object sender, EventArgs e, LiftType liftType)
        {
            int id = (int)((Button)sender).Tag;
            button_extra_down_click(id: id, liftType: liftType);
        }
        private void button_extra_delete_click(object sender, EventArgs e, LiftType liftType)
        {
            int id = (int)((Button)sender).Tag;
            button_extra_delete_click(id: id, liftType: liftType);
        }
        private void button_extra_commit_click(object sender, EventArgs e, LiftType liftType)
        {
            button_extra_commit_click(liftType: liftType);
        }
        private void textBox_extra_TextChanged(object sender, EventArgs e, LiftType liftType)
        {
            TextBox textBox = (TextBox)sender;
            textBox_extra_TextChanged(textBox: textBox, liftType: liftType);
        }
        private void numericUpDown_extra_ValueChanged(object sender, EventArgs e, LiftType liftType)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            numericUpDown_extra_ValueChanged(numericUpDown: numericUpDown, liftType: liftType);
        }
        #endregion

        #region jumps
        private void button_jump_delete_click(object sender, EventArgs e, LiftType liftType)
        {
            int id = (int)((Button)sender).Tag;
            button_jump_delete_click(id: id, liftType: liftType);
        }
        private void button_jump_commit_click(object sender, EventArgs e, LiftType liftType)
        {
            button_jump_commit_click(liftType: liftType);
        }
        private void numericUpDown_jump_FromWeight_ValueChanged(object sender, EventArgs e, LiftType liftType)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            numericUpDown_jump_FromWeight_ValueChanged(numericUpDown: numericUpDown, liftType: liftType);
        }
        private void numericUpDown_jump_Jump_ValueChanged(object sender, EventArgs e, LiftType liftType)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            numericUpDown_jump_Jump_ValueChanged(numericUpDown: numericUpDown, liftType: liftType);
        }
        #endregion

        #region times
        private void button_time_delete_click(object sender, EventArgs e, LiftType liftType)
        {
            int id = (int)((Button)sender).Tag;
            button_time_delete_click(id: id, liftType: liftType);
        }
        private void button_time_commit_click(object sender, EventArgs e, LiftType liftType)
        {
            button_time_commit_click(liftType: liftType);
        }
        private void numericUpDown_time_FromWeight_ValueChanged(object sender, EventArgs e, LiftType liftType)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            numericUpDown_time_FromWeight_ValueChanged(numericUpDown: numericUpDown, liftType: liftType);
        }
        private void numericUpDown_time_Time_ValueChanged(object sender, EventArgs e, LiftType liftType)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            numericUpDown_time_Time_ValueChanged(numericUpDown: numericUpDown, liftType: liftType);
        }
        #endregion
        #region steps
        #endregion
        #endregion

        #region extras
        private (
            List<Extra> extras,
            Panel panelExtra,
            Action populateExtras) GetContext_Extras(LiftType liftType)
        {
            if (liftType == LiftType.Snatch)
            {
                return (
                    profileActive.SnatchExtras,
                    panel_snatch_extra,
                    () => PopulateExtras(LiftType.Snatch));
            }
            else
            {
                return (
                    profileActive.CJExtras,
                    panel_cj_extra,
                    () => PopulateExtras(LiftType.CleanAndJerk));
            }
        }
        private void PopulateExtras(LiftType liftType)
        {
            (List<Extra> extras, Panel panelExtra, _) = GetContext_Extras(liftType);
            int intY = 1;
            panelExtra.Controls.Clear();

            // This local function creates the controls for a single row
            void Add_Extra_IndividualControls(int y, int id, string tbText, int length, bool isAddBlankRow)
            {
                TextBox tb = new() { Text = tbText, Location = new Point(6, y), Size = new Size(157, 25), Tag = id, BackColor = Color.White };
                NumericUpDown numericUpDown = new()
                {
                    Location = new Point(169, y),
                    Maximum = 9999,
                    Minimum = 1,
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = length,
                    Tag = id,
                    BackColor = Color.White
                };
                Label lbl = new() { Location = new Point(250, y + 3), Text = Seconds_To_String(length), Tag = id };

                tb.TextChanged += (sender, e) => textBox_extra_TextChanged(sender, e, liftType);
                numericUpDown.ValueChanged += (sender, e) => numericUpDown_extra_ValueChanged(sender, e, liftType);
                panelExtra.Controls.AddRange([tb, numericUpDown, lbl]);

                if (isAddBlankRow)
                {
                    Button btnCommit = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 10F), Location = new Point(358, y), Size = new Size(125, 25), Text = str_buttontext_commit, Tag = id };
                    btnCommit.Click += (sender, e) => button_extra_commit_click(sender, e, liftType);
                    panelExtra.Controls.Add(btnCommit);
                    tb.Select();
                }
                else
                {
                    Button btnUp = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 10F), Location = new Point(358, y), Size = new Size(32, 25), Text = str_buttontext_up, Tag = id };
                    Button btnDown = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 9F), Location = new Point(401, y), Size = new Size(32, 25), Text = str_buttontext_down, Tag = id };
                    Button btnDelete = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 9F), Location = new Point(447, y), Size = new Size(36, 25), Text = str_buttontext_delete, Tag = id };

                    btnUp.Click += (sender, e) => button_extra_up_click(sender, e, liftType);
                    btnDown.Click += (sender, e) => button_extra_down_click(sender, e, liftType);
                    btnDelete.Click += (sender, e) => button_extra_delete_click(sender, e, liftType);
                    panelExtra.Controls.AddRange([btnUp, btnDown, btnDelete]);
                }
            }

            // Loop through existing extras and add their controls
            foreach (Extra extra in extras.OrderBy(r => r.Order))
            {
                Add_Extra_IndividualControls(intY, extra.id, extra.Action, extra.Length, false);
                intY += 30;
            }

            // Add the final blank row for new entries
            Add_Extra_IndividualControls(intY, -1, string.Empty, 60, true);
        }
        private void button_extra_up_click(int id, LiftType liftType)
        {
            (List<Extra> extras, _, Action populateExtras) = GetContext_Extras(liftType);
            Extra extra = extras.FirstOrDefault(r => r.id == id);

            if (extra.id < 1 || extra.Order < 1) return;

            int oldOrder = extra.Order;
            for (int i = 0; i < extras.Count; i++)
            {
                if (extras[i].id == id)
                {
                    Extra temp = extras[i];
                    temp.Order = oldOrder - 1;
                    extras[i] = temp;
                }
                else if (extras[i].Order == oldOrder - 1)
                {
                    Extra temp = extras[i];
                    temp.Order = oldOrder;
                    extras[i] = temp;
                }
            }

            populateExtras();
            PopulateSteps(liftType: liftType);
        }
        private void button_extra_down_click(int id, LiftType liftType)
        {
            (List<Extra> extras, _, Action populateExtras) = GetContext_Extras(liftType);
            Extra extra = extras.FirstOrDefault(r => r.id == id);

            if (extra.id < 1) return;

            int oldOrder = extra.Order;
            int maxOrder = Extras_Max_Order(extras);
            if (oldOrder < 0 || oldOrder == maxOrder) return;

            for (int i = 0; i < extras.Count; i++)
            {
                if (extras[i].id == id)
                {
                    Extra temp = extras[i];
                    temp.Order = oldOrder + 1;
                    extras[i] = temp;
                }
                else if (extras[i].Order == oldOrder + 1)
                {
                    Extra temp = extras[i];
                    temp.Order = oldOrder;
                    extras[i] = temp;
                }
            }

            populateExtras();
            PopulateSteps(liftType: liftType);
        }
        private void button_extra_delete_click(int id, LiftType liftType)
        {
            (List<Extra> extras, _, Action populateExtras) = GetContext_Extras(liftType);

            List<Extra> extrasList = extras;
            extrasList.RemoveAll(r => r.id == id);
            Extras_Reassign_Order(ref extrasList);

            populateExtras();
            PopulateSteps(liftType: liftType);
        }
        private void button_extra_commit_click(LiftType liftType)
        {
            (List<Extra> extras, Panel panelExtra, Action populateExtras) = GetContext_Extras(liftType);
            string action = string.Empty;
            int length = -1;

            TextBox newActionTb = panelExtra.Controls.OfType<TextBox>().FirstOrDefault(c => (int)c.Tag == -1);
            NumericUpDown newLengthnumericUpDown = panelExtra.Controls.OfType<NumericUpDown>().FirstOrDefault(c => (int)c.Tag == -1);

            if (newActionTb != null) action = newActionTb.Text;
            if (newLengthnumericUpDown != null) length = (int)newLengthnumericUpDown.Value;

            if (string.IsNullOrWhiteSpace(action))
            {
                MessageBox.Show("Action cannot be blank.");
                return;
            }
            if (length < 1)
            {
                MessageBox.Show("Length cannot be less than 1.");
                return;
            }

            List<Extra> extrasList = extras;
            Extra newExtra = new(action, length, Extras_Max_Order(extrasList) + 1);
            extrasList.Add(newExtra);
            Extras_Reassign_Order(ref extrasList);

            populateExtras();
            PopulateSteps(liftType: liftType);
        }
        private void textBox_extra_TextChanged(TextBox textBox, LiftType liftType)
        {
            (List<Extra> extras, _, _) = GetContext_Extras(liftType);
            int id = (int)textBox.Tag;

            if (id < 1) return;

            string newAction = textBox.Text;
            textBox.BackColor = string.IsNullOrWhiteSpace(newAction) ? Color.Yellow : Color.White;

            for (int i = 0; i < extras.Count; i++)
            {
                if (extras[i].id == id)
                {
                    Extra temp = extras[i];
                    temp.Action = newAction;
                    extras[i] = temp;
                    break;
                }
            }
            PopulateSteps(liftType: liftType);
        }
        private void numericUpDown_extra_ValueChanged(NumericUpDown numericUpDown, LiftType liftType)
        {
            (List<Extra> extras, Panel panelExtra, _) = GetContext_Extras(liftType);
            int id = (int)numericUpDown.Tag;

            if (id < 1) return;

            int newLength = (int)numericUpDown.Value;
            if (newLength < 1) return;

            for (int i = 0; i < extras.Count; i++)
            {
                if (extras[i].id == id)
                {
                    Extra temp = extras[i];
                    temp.Length = newLength;
                    extras[i] = temp;
                    break;
                }
            }

            Label labelToUpdate = panelExtra.Controls.OfType<Label>().FirstOrDefault(lbl => (int)lbl.Tag == id);
            if (labelToUpdate != null)
            {
                labelToUpdate.Text = Seconds_To_String(newLength);
            }

            PopulateSteps(liftType: liftType);
        }
        private int Extras_Max_Order(List<Extra> extras)
        {
            return extras.Any() ? extras.Max(r => r.Order) : -1;
        }
        private void Extras_Reassign_Order(ref List<Extra> extras)
        {
            List<Extra> orderedExtras = [.. extras.OrderBy(r => r.Order)];
            for (int i = 0; i < orderedExtras.Count; i++)
            {
                Extra temp = orderedExtras[i];
                temp.Order = i;
                orderedExtras[i] = temp;
            }
            extras = orderedExtras;
        }
        #endregion

        #region jumps
        private (
            Dictionary<int, int> jumps,
            Panel panel,
            //Action stopLive,
            Dictionary<int, int> getDefaultJumps) GetJumpContext(LiftType liftType)
        {
            if (liftType == LiftType.Snatch)
            {
                return (
                    profileActive.SnatchJumps,
                    panel_snatch_jump,
                    Defaults.default_snatchJumps);
            }
            else // CleanAndJerk
            {
                return (
                    profileActive.CJJumps,
                    panel_cj_jump,
                    Defaults.default_cjJumps);
            }
        }
        private void PopulateJumps(LiftType liftType)
        {
            (Dictionary<int, int> jumps, Panel panel, Dictionary<int, int> getDefaultJumps) = GetJumpContext(liftType);
            panel.Controls.Clear();

            if (jumps.Count == 0)
            {
                if (liftType == LiftType.Snatch)
                {
                    profileActive.SnatchJumps = getDefaultJumps;
                }
                else profileActive.CJJumps = getDefaultJumps;
            }

            // A local function to create the UI controls for each row
            void Add_Jump_IndividualControls(int y, int fromWeight, int jump, bool isAddBlankRow)
            {
                int tag = isAddBlankRow ? -1 : fromWeight;

                NumericUpDown nmudFromWeight = new() { Location = new Point(6, y), Maximum = 9999, Minimum = 0, Size = new Size(72, 25), TextAlign = HorizontalAlignment.Center, Value = fromWeight, Tag = tag, BackColor = Color.White };
                NumericUpDown nmudJump = new() { Location = new Point(100, y), Maximum = 9999, Minimum = 1, Size = new Size(72, 25), TextAlign = HorizontalAlignment.Center, Value = jump, Tag = tag, BackColor = Color.White };

                nmudFromWeight.ValueChanged += (s, e) => numericUpDown_jump_FromWeight_ValueChanged(s, e, liftType);
                nmudJump.ValueChanged += (s, e) => numericUpDown_jump_Jump_ValueChanged(s, e, liftType);
                panel.Controls.AddRange([nmudFromWeight, nmudJump]);

                if (isAddBlankRow)
                {
                    Button btnCommit = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 10F), Location = new Point(200, y), Size = new Size(90, 25), Text = str_buttontext_commit };
                    btnCommit.Click += (s, e) => button_jump_commit_click(s, e, liftType);
                    panel.Controls.Add(btnCommit);
                    nmudFromWeight.Select();
                }
                else if (fromWeight > 0) // Don't allow deleting the '0' weight baseline
                {
                    Button btnDelete = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 9F), Location = new Point(200, y), Size = new Size(36, 25), Text = str_buttontext_delete, Tag = tag };
                    btnDelete.Click += (s, e) => button_jump_delete_click(s, e, liftType);
                    panel.Controls.Add(btnDelete);
                }
            }

            int yPos = 1;
            foreach (KeyValuePair<int, int> jumpPair in jumps.OrderBy(r => r.Key))
            {
                Add_Jump_IndividualControls(yPos, jumpPair.Key, jumpPair.Value, false);
                yPos += 30;
            }

            int nextWeight = jumps.Any() ? jumps.Keys.Max() + 1 : 1;
            int lastJump = jumps.Any() ? jumps.Last().Value : 1;
            Add_Jump_IndividualControls(yPos, nextWeight, lastJump, true);
        }
        private void button_jump_delete_click(int id, LiftType liftType)
        {
            (Dictionary<int, int> jumps, _, _) = GetJumpContext(liftType);

            if (jumps.ContainsKey(id))
            {
                jumps.Remove(id);
            }

            PopulateJumps(liftType);
            PopulateSteps(liftType: liftType);
        }
        private void button_jump_commit_click(LiftType liftType)
        {
            (Dictionary<int, int> jumps, Panel panel, _) = GetJumpContext(liftType);

            NumericUpDown fromWeightnumericUpDown = panel.Controls.OfType<NumericUpDown>().FirstOrDefault(c => (int)c.Tag == -1 && c.Left < 50);
            NumericUpDown jumpnumericUpDown = panel.Controls.OfType<NumericUpDown>().FirstOrDefault(c => (int)c.Tag == -1 && c.Left > 50);

            if (fromWeightnumericUpDown == null || jumpnumericUpDown == null)
            {
                MessageBox.Show("Failed to find input controls.");
                return;
            }

            int fromWeight = (int)fromWeightnumericUpDown.Value;
            int jump = (int)jumpnumericUpDown.Value;

            if (jumps.ContainsKey(fromWeight))
            {
                MessageBox.Show("The 'From Weight' you entered already exists.");
                return;
            }

            jumps[fromWeight] = jump;
            PopulateJumps(liftType);
            PopulateSteps(liftType: liftType);
        }
        private void numericUpDown_jump_FromWeight_ValueChanged(NumericUpDown numericUpDown, LiftType liftType)
        {
            (Dictionary<int, int> jumps, _, _) = GetJumpContext(liftType);
            int oldFromWeightKey = (int)numericUpDown.Tag;

            if (oldFromWeightKey == -1) return; // Ignore the 'add new' row

            int newFromWeightKey = (int)numericUpDown.Value;
            if (oldFromWeightKey == newFromWeightKey) return;

            if (jumps.TryGetValue(oldFromWeightKey, out int jumpValue))
            {
                if (jumps.ContainsKey(newFromWeightKey))
                {
                    MessageBox.Show($"The weight '{newFromWeightKey}' already exists. Please choose a different weight.");
                    numericUpDown.Value = oldFromWeightKey; // Revert
                    return;
                }
                jumps.Remove(oldFromWeightKey);
                jumps[newFromWeightKey] = jumpValue;
            }

            // Repopulate to re-sort and update tags
            PopulateJumps(liftType);
            PopulateSteps(liftType: liftType);
        }
        private void numericUpDown_jump_Jump_ValueChanged(NumericUpDown numericUpDown, LiftType liftType)
        {
            (Dictionary<int, int> jumps, _, _) = GetJumpContext(liftType);
            int fromWeightKey = (int)numericUpDown.Tag;

            if (fromWeightKey == -1) return; // Ignore the 'add new' row

            if (jumps.ContainsKey(fromWeightKey))
            {
                jumps[fromWeightKey] = (int)numericUpDown.Value;
                PopulateSteps(liftType: liftType);
            }
        }
        #endregion

        #region times
        private (
            Dictionary<int, int> times,
            Panel panel,
            Dictionary<int, int> getDefaultTimes) GetTimeContext(LiftType liftType)
        {
            if (liftType == LiftType.Snatch)
            {
                return (
                    profileActive.SnatchTimes,
                    panel_snatch_time,
                    Defaults.default_snatchTimes);
            }
            else // CleanAndJerk
            {
                return (
                    profileActive.CJTimes,
                    panel_cj_time,
                    Defaults.default_cjTimes);
            }
        }
        private void PopulateTimes(LiftType liftType)
        {
            (Dictionary<int, int> times, Panel panel, Dictionary<int, int> getDefaultTimes) context = GetTimeContext(liftType);
            context.panel.Controls.Clear();

            // Assign default times if the current list is empty
            if (context.times.Count == 0)
            {
                if (liftType == LiftType.Snatch)
                {
                    profileActive.SnatchTimes = context.getDefaultTimes;
                }
                else
                {
                    profileActive.CJTimes = context.getDefaultTimes;
                }
                // Re-fetch context to ensure we have the newly assigned dictionary
                context = GetTimeContext(liftType);
            }

            // A local function to create the UI controls for each row
            void Add_Time_IndividualControls(int y, int fromWeight, int time, bool isAddBlankRow)
            {
                int tag = isAddBlankRow ? -1 : fromWeight;

                NumericUpDown nmudFromWeight = new() { Location = new Point(6, y), Maximum = 9999, Minimum = 0, Size = new Size(72, 25), TextAlign = HorizontalAlignment.Center, Value = fromWeight, Tag = tag, BackColor = Color.White };
                NumericUpDown nmudTime = new() { Location = new Point(100, y), Maximum = 9999, Minimum = 1, Size = new Size(72, 25), TextAlign = HorizontalAlignment.Center, Value = time, Tag = tag, BackColor = Color.White };

                nmudFromWeight.ValueChanged += (s, e) => numericUpDown_time_FromWeight_ValueChanged(s, e, liftType);
                nmudTime.ValueChanged += (s, e) => numericUpDown_time_Time_ValueChanged(s, e, liftType);
                context.panel.Controls.AddRange([nmudFromWeight, nmudTime]);

                if (isAddBlankRow)
                {
                    Button btnCommit = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 10F), Location = new Point(200, y), Size = new Size(90, 25), Text = str_buttontext_commit };
                    btnCommit.Click += (s, e) => button_time_commit_click(s, e, liftType);
                    context.panel.Controls.Add(btnCommit);
                    nmudFromWeight.Select();
                }
                else if (fromWeight > 0) // Don't allow deleting the '0' weight baseline
                {
                    Button btnDelete = new() { FlatStyle = FlatStyle.Flat, Font = new Font("Gadugi", 9F), Location = new Point(200, y), Size = new Size(36, 25), Text = str_buttontext_delete, Tag = tag };
                    btnDelete.Click += (s, e) => button_time_delete_click(s, e, liftType);
                    context.panel.Controls.Add(btnDelete);
                }
            }

            int yPos = 1;
            foreach (KeyValuePair<int, int> timePair in context.times.OrderBy(r => r.Key))
            {
                Add_Time_IndividualControls(yPos, timePair.Key, timePair.Value, false);
                yPos += 30;
            }

            int nextWeight = context.times.Any() ? context.times.Keys.Max() + 1 : 1;
            int lastTime = context.times.Any() ? context.times.Last().Value : 60; // Default to 60s
            Add_Time_IndividualControls(yPos, nextWeight, lastTime, true);
        }
        private void button_time_delete_click(int id, LiftType liftType)
        {
            (Dictionary<int, int> times, _, _) = GetTimeContext(liftType);

            if (times.ContainsKey(id))
            {
                times.Remove(id);
            }

            PopulateTimes(liftType);
            PopulateSteps(liftType: liftType);
        }
        private void button_time_commit_click(LiftType liftType)
        {
            (Dictionary<int, int> times, Panel panel, _) = GetTimeContext(liftType);

            NumericUpDown fromWeightnumericUpDown = panel.Controls.OfType<NumericUpDown>().FirstOrDefault(c => (int)c.Tag == -1 && c.Left < 50);
            NumericUpDown timenumericUpDown = panel.Controls.OfType<NumericUpDown>().FirstOrDefault(c => (int)c.Tag == -1 && c.Left > 50);

            if (fromWeightnumericUpDown == null || timenumericUpDown == null)
            {
                MessageBox.Show("Failed to find input controls.");
                return;
            }

            int fromWeight = (int)fromWeightnumericUpDown.Value;
            int time = (int)timenumericUpDown.Value;

            if (times.ContainsKey(fromWeight))
            {
                MessageBox.Show("The 'From Weight' you entered already exists.");
                return;
            }

            times[fromWeight] = time;
            PopulateTimes(liftType);
            PopulateSteps(liftType: liftType);
        }
        private void numericUpDown_time_FromWeight_ValueChanged(NumericUpDown numericUpDown, LiftType liftType)
        {
            (Dictionary<int, int> times, _, _) = GetTimeContext(liftType);
            int oldFromWeightKey = (int)numericUpDown.Tag;

            if (oldFromWeightKey == -1) return; // Ignore the 'add new' row

            int newFromWeightKey = (int)numericUpDown.Value;
            if (oldFromWeightKey == newFromWeightKey) return;

            if (times.TryGetValue(oldFromWeightKey, out int timeValue))
            {
                if (times.ContainsKey(newFromWeightKey))
                {
                    MessageBox.Show($"The weight '{newFromWeightKey}' already exists. Please choose a different weight.");
                    numericUpDown.Value = oldFromWeightKey; // Revert to old value
                    return;
                }
                times.Remove(oldFromWeightKey);
                times[newFromWeightKey] = timeValue;
            }

            PopulateTimes(liftType); // Repopulate to re-sort and update control tags
            PopulateSteps(liftType: liftType);
        }
        private void numericUpDown_time_Time_ValueChanged(NumericUpDown numericUpDown, LiftType liftType)
        {
            (Dictionary<int, int> times, _, _) = GetTimeContext(liftType);
            int fromWeightKey = (int)numericUpDown.Tag;

            if (fromWeightKey == -1) return; // Ignore the 'add new' row

            if (times.ContainsKey(fromWeightKey))
            {
                times[fromWeightKey] = (int)numericUpDown.Value;
                PopulateSteps(liftType: liftType);
            }
        }
        #endregion

        #region steps
        private (
            Func<List<Step>> getStepsPlan,
            Action<List<Step>> setStepsPlan,
            Func<bool> isLive,
            Label stepCountLabel,
            DataGridView dataGridViewSteps
        ) GetStepContext(LiftType liftType)
        {
            if (liftType == LiftType.Snatch)
            {
                return (
                    () => snatchStepsPLAN,
                    steps => snatchStepsPLAN = steps,
                    () => bool_snatch_Live,
                    label_snatch_Setup_StepCount,
                    dataGridView_snatch_steps
                );
            }
            else // CleanAndJerk
            {
                return (
                     () => cjStepsPLAN,
                     steps => cjStepsPLAN = steps,
                    () => bool_cj_Live,
                    label_cj_Setup_StepCount,
                    dataGridView_cj_steps
                );
            }
        }
        private List<Step> GenerateStepsList(LiftType liftType)
        {
            // Based on the liftType, select the appropriate data sources and parameters
            if (liftType == LiftType.Snatch)
            {
                return x_Steps(
                    _extras: profileActive.SnatchExtras,
                    _jumps: profileActive.SnatchJumps,
                    _times: profileActive.SnatchTimes,
                    _int_x_Sec_End: profileActive.Snatch_SecondsEnd,
                    _int_x_Wgt_Opener: profileActive.Snatch_OpenerWeight,
                    _bool_Opener_in_Warmup: profileActive.Snatch_OpenerInWarmup
                );
            }
            else // CleanAndJerk
            {
                return x_Steps(
                    _extras: profileActive.CJExtras,
                    _jumps: profileActive.CJJumps,
                    _times: profileActive.CJTimes,
                    _int_x_Sec_End: profileActive.CJ_SecondsEnd,
                    _int_x_Wgt_Opener: profileActive.CJ_OpenerWeight,
                    _bool_Opener_in_Warmup: profileActive.CJ_OpenerInWarmup
                );
            }
        }
        private void PopulateSteps(LiftType liftType)
        {
            (Func<List<Step>> getStepsPlan, Action<List<Step>> setStepsPlan, _, Label stepCountLabel, DataGridView dataGridViewSteps) =
                GetStepContext(liftType);

            List<Step> newStepsPlan = GenerateStepsList(liftType);
            setStepsPlan(newStepsPlan);

            List<Step> currentSteps = getStepsPlan();
            if (currentSteps == null)
            {
                dataGridViewSteps.DataSource = null;
                stepCountLabel.Text = "0 steps";
                return;
            }
            BindingList<Step> displaySteps = new([.. currentSteps.Where(s => !s.PreStep).OrderBy(r => r.Order)]);
            dataGridViewSteps.DataSource = displaySteps;
            stepCountLabel.Text = $"{displaySteps.Count} steps ({displaySteps.Count(r => r.Weight > 0)} lifts)";
            if (liftType == LiftType.Snatch && bool_snatch_Live)
            {
                Populate_snatch_Live_Steps();
            }
            else if (liftType == LiftType.CleanAndJerk && bool_cj_Live)
            {
                Populate_cj_Live_Steps();
            }
        }
        #endregion

        #region setup controls
        private (
            // Func/Action to get/set class-level variables
            Action<int> setSecStage,
            Action<int> setWgtOpener,
            Action<bool> setOpenerWarmup,
            Action<int> setSecEnd,
            // UI Controls
            NumericUpDown numSecStage,
            NumericUpDown numWgtOpener,
            CheckBox chkOpenerWarmup,
            NumericUpDown numSecEnd,
            // Method to call
            Action<int, int, bool> applyOpenerGraphic
        ) GetSetupContext(LiftType liftType)
        {
            if (liftType == LiftType.Snatch)
            {
                return (
                    (v) => profileActive.Snatch_SecondsStage = v,
                    (v) => profileActive.Snatch_OpenerWeight = v,
                    (v) => profileActive.Snatch_OpenerInWarmup = v,
                    (v) => profileActive.Snatch_SecondsEnd = v,
                    numericUpDown_snatch_time_stage,
                    numericUpDown_snatch_weight_opener,
                    checkBox_snatch_Param_OpenerWarmup,
                    numericUpDown_snatch_time_PostWarmup,
                    (bar, opener, isSnatch) => Apply_Opener_Graphic_Vector(bar, opener, true)
                );
            }
            else // CleanAndJerk
            {
                return (
                    (v) => profileActive.CJ_SecondsStage = v,
                    (v) => profileActive.CJ_OpenerWeight = v,
                    (v) => profileActive.CJ_OpenerInWarmup = v,
                    (v) => profileActive.CJ_SecondsEnd = v,
                    numericUpDown_cj_time_stage,
                    numericUpDown_cj_weight_opener,
        checkBox_cj_Param_OpenerWarmup,
                    numericUpDown_cj_time_PostWarmup,
                    (bar, opener, isSnatch) => Apply_Opener_Graphic_Vector(bar, opener, false)
                );
            }
        }
        // Generic event handlers
        private void numericUpDown_time_stage_ValueChanged(object sender, EventArgs e, LiftType liftType)
        {
            (Action<int> setSecStage,
                _,
                _,
                _,
                NumericUpDown numSecStage,
                _,
                _,
                _,
                _) = GetSetupContext(liftType);
            if (int.TryParse(numSecStage.Value.ToString(), out int secStage) && secStage >= 1)
            {
                setSecStage(secStage);
                numSecStage.BackColor = Color.White;
                PopulateSteps(liftType);
            }
            else
            {
                numSecStage.BackColor = Color.Yellow;
            }
        }
        private void Opener_Set(LiftType liftType)
        {
            if (bool_Loading) { return; }
            ApplyOpener(liftType: liftType);
        }
        private void ApplyOpener(LiftType liftType)
        {
            (_,
                Action<int> setWgtOpener,
                Action<bool> setOpenerWarmup,
                _,
                _,
                NumericUpDown numWgtOpener,
                CheckBox chkOpenerWarmup,
                _,
                Action<int, int, bool> applyOpenerGraphic) = GetSetupContext(liftType);

            if (int.TryParse(numWgtOpener.Value.ToString(), out int wgtOpener) && wgtOpener >= 1)
            {
                setWgtOpener(wgtOpener);
                setOpenerWarmup(chkOpenerWarmup.Checked);
                numWgtOpener.BackColor = Color.White;

                applyOpenerGraphic(profileActive.BarbellWeight, wgtOpener, liftType == LiftType.Snatch);
                PopulateSteps(liftType);
            }
            else
            {
                numWgtOpener.BackColor = Color.Yellow;
            }
        }
        private void numericUpDown_time_PostWarmup_ValueChanged(object sender, EventArgs e, LiftType liftType)
        {
            if (bool_Loading)
            {
                return;
            }
            (_, _, _, Action<int> setSecEnd, _, _, _, NumericUpDown numSecEnd, _) = GetSetupContext(liftType);
            if (int.TryParse(numSecEnd.Value.ToString(), out int secEnd) && secEnd >= 0)
            {
                setSecEnd(secEnd);
                numSecEnd.BackColor = Color.White;
                PopulateSteps(liftType);
            }
            else
            {
                numSecEnd.BackColor = Color.Yellow;
            }
        }
        #endregion

        #region Live Timer
        private int timerInterval
        {
            get
            {
                int delayUntilNextSecond = 1000 - DateTime.Now.Millisecond;
                if (delayUntilNextSecond <= 0)
                {
                    delayUntilNextSecond = 1000;
                }
                return delayUntilNextSecond;
            }
        }
        private void timer_Live_Tick(Timer timer, LiftType liftType)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            timer.Stop();
            if (liftType == LiftType.Snatch)
            {
                if (bool_snatch_Live) 
                {
                    sim_timer_snatch_Live_Tick();
                }
                else
                {
                    snatch_Stop_Live();
                }
            }
            else // CleanAndJerk
            {
                if (bool_cj_Live)
                {
                    sim_timer_cj_Live_Tick();
                }
                else
                {
                    cj_Stop_Live();
                }
            }
            timer.Interval = timerInterval;
            timer.Start();
            stopwatch.Stop();
            Console.WriteLine($"timer_Live_Tick execution time: {stopwatch.ElapsedMilliseconds} ms");
        }
        private void stopBatteryTimer()
        {
            if (timer_Battery != null)
            {
                timer_Battery.Stop();
                timer_Battery.Dispose();
                timer_Battery = null;
            }
        }
        #endregion

        #region snatch LIVE
        private void stopSnatchTimer()
        {
            if (timer_snatch_Live != null)
            {
                timer_snatch_Live.Stop();
                timer_snatch_Live.Dispose();
                timer_snatch_Live = null;
            }
            stopBatteryTimer();
        }
        private void snatch_Stop_Live()
        {
            bool_snatch_Live = false;
            stopSnatchTimer();
            textBox_snatch_Live_LiftsOut.Visible = false;
            label_snatch_Live_TimeTillStart.Text = String.Empty;
            label_snatch_Live_TimeTillOpener.Text = String.Empty;
            label_snatch_Live_LiftsPassed.Text = String.Empty;
            Clear_snatch_Live_Steps();
            progressBar_snatch_Live_StageLift.Value = 0;
            button_snatch_Live_StageAdvance.BackColor = AppColors.Snatch_Live_BG;
            button_snatch_Live_StageAdvance.Tag = 0;
            bool_snatch_LiveLifting = false;
            button_snatch_Live_StartStop.Text = "start";
            panel_snatch_Live_Times.Visible = false;
            panel_Battery.Visible = false;
            if (!bool_cj_Live) { AllowMonitorPowerdown(); }
        }
        private void snatch_Start_Live()
        {
            bool_snatch_Live = true;
            datetime_snatch_Start = DateTime.Today.AddHours(dateTimePicker_snatch_Start.Value.Hour).AddMinutes(dateTimePicker_snatch_Start.Value.Minute);
            int_snatch_Lifts_Passed = 0;
            textBox_snatch_Live_LiftsOut.Visible = false;
            label_snatch_Live_LiftsPassed.Text = "0";
            Populate_snatch_Live_Steps();
            progressBar_snatch_Live_StageLift.Value = 0;
            progressBar_snatch_Live_StageLift.Maximum = profileActive.Snatch_SecondsStage;
            label_snatch_Live_TimeTillStart.Text = String.Empty;
            label_snatch_Live_TimeTillOpener.Text = String.Empty;

            // Run one tick immediately to populate UI without waiting
            sim_timer_snatch_Live_Tick();
            timer_snatch_Live = new Timer();
            timer_snatch_Live.Tick += (s, e) => timer_Live_Tick(s, e, LiftType.Snatch);
            timer_snatch_Live.Interval = timerInterval;
            timer_snatch_Live.Start();
            timer_Battery = new Timer();
            timer_Battery.Tick += (s, e) => timer_Battery_Tick(s, e);
            timer_Battery.Interval = 60000;
            timer_Battery.Start();

            UpdateBattery();
            button_snatch_Live_StartStop.Text = "stop";
            button_snatch_Live_StageAdvance.Select();
            PreventMonitorPowerdown();
        }
        private void button_snatch_Live_StartStop_Click()
        {
            if (bool_snatch_Live)
            {
                snatch_Stop_Live();
            }
            else
            {
                snatch_Start_Live();
            }
        }
        private void Clear_snatch_Live_Steps()
        {
            snatchStepsLIVE = null;
            panel_snatch_Live_Steps.Controls.Clear();
        }
        private void Populate_snatch_Live_Steps()
        {
            Clear_snatch_Live_Steps();

            if (snatchStepsPLAN is null)
            {
                PopulateSteps(LiftType.Snatch);
            }
            if (snatchStepsPLAN is null || !snatchStepsPLAN.Any())
            {
                MessageBox.Show("An error has occurred. Step plan could not be determined.");
                this.Close();
                return;
            }
            snatchStepsLIVE = [.. snatchStepsPLAN.Select(r => r.Clone())];
            Step _stepLast = snatchStepsLIVE.Last();
            snatchStepsLIVE.Add(new(
                action: $"open at {profileActive.Snatch_OpenerWeight}",
                weight: profileActive.Snatch_OpenerWeight,
                length: 0,
                totalLength: _stepLast.TotalLength,
                totalLengthReverse: 0,
                order: _stepLast.Order + 1,
                preStep: false,
                isOpener: true));

            int intY = 1;
            int _int_panel_Live_Step_Width = panel_snatch_Live_Steps.Width - 4;
            int _int_progressBar_Step_Width_NoScroll = _int_panel_Live_Step_Width - 350;
            int _int_progressBar_Step_Width_Scroll = _int_progressBar_Step_Width_NoScroll - SystemInformation.VerticalScrollBarWidth;
            int _int_progressBar_Step_Height = 65;
            int _int_progressBar_Step_Width;
            int _int_label_Weight_Width = 105;
            Point _point_progressBar_Step_Location = new(300, 6);

            SuspendLayout();
            foreach (Step _step in snatchStepsLIVE)
            {
                if (panel_snatch_Live_Steps.VerticalScroll.Visible)
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_Scroll - _int_label_Weight_Width;
                }
                else
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_NoScroll - _int_label_Weight_Width;
                }
                bool _isLift = (_step.Weight > 0);
                string strActionText = ActionTextString(_step: _step, _isFuture: true);
                Panel panel_Live_Step = new()
                {
                    Size = new Size(_int_panel_Live_Step_Width, 80),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = AppColors.Snatch_Live_BG,
                    ForeColor = AppColors.Live_Default_FG,
                    Location = new Point(1, intY)
                };
                Label label_Action = new()
                {
                    Text = strActionText,
                    AutoSize = false,
                    Size = new Size(280, 75),
                    TextAlign = ContentAlignment.TopRight,
                    Location = new Point(6, 1)
                };
                ProgressBar progressBar_Step = new()
                {
                    Size = new Size(_int_progressBar_Step_Width, _int_progressBar_Step_Height),
                    Location = _point_progressBar_Step_Location,
                    Maximum = _step.Length,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                if (progressBar_Step.Maximum < 1)
                {
                    progressBar_Step.Maximum = 1;
                    progressBar_Step.Value = 1;
                }
                Label label_Weight = null;
                if (_isLift)
                {
                    label_Weight = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.Black,
                        AutoSize = false,
                        Size = new Size(_int_label_Weight_Width, _int_progressBar_Step_Height),
                        Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width, _point_progressBar_Step_Location.Y),
                        ForeColor = SystemColors.Window,
                        Text = $"{(_step.isOpener ? "open at" : "lift")} {_step.Weight}",
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                        Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    };
                    label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Regular);
                }
                Label label_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(160, 30),
                    Location = _point_progressBar_Step_Location,
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                };
                Label label_Progress_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(_int_label_Weight_Width, _int_progressBar_Step_Height),
                    Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width, _point_progressBar_Step_Location.Y),
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                };
                panel_Live_Step.Controls.AddRange(
                [
                    label_Action,
                    progressBar_Step,
                    label_Progress_Time,
                    label_Time
                ]);
                WeightBox weightBoxGraphic = null;
                if (_isLift)
                {
                    weightBoxGraphic = new(
                        isOpener: false,
                        isCompCollars: _step.isOpener,
                        barWeight: profileActive.BarbellWeight,
                        weight: (int)_step.Weight
                    )
                    {
                        Size = new Size(120, 65),
                        BackColor = Color.FromArgb(196, 196, 196),
                        BorderStyle = BorderStyle.Fixed3D,
                        Location = new Point(10, 6),
                        Visible = true,
                    };
                    panel_Live_Step.Controls.Add(weightBoxGraphic);
                }
                progressBar_Step.BringToFront();
                _step.Controls.PanelLiveStep = panel_Live_Step;
                _step.Controls.LabelAction = label_Action;
                _step.Controls.ProgressBarStep = progressBar_Step;
                weightBoxGraphic?.BringToFront();
                if (label_Weight != null)
                {
                    panel_Live_Step.Controls.Add(label_Weight);
                    _step.Controls.LabelWeight = label_Weight;
                    label_Weight.BringToFront();
                }
                label_Time.BringToFront();
                _step.Controls.LabelTime = label_Time;
                label_Progress_Time.BringToFront();
                _step.Controls.LabelProgressTime = label_Progress_Time;
                panel_snatch_Live_Steps.Controls.Add(panel_Live_Step);

                intY += 81;
            }
            int_snatch_Warmup_Step = -1;
            label_snatch_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
            panel_snatch_Live_Times.Visible = true;
            ResumeLayout();
        }
        private void sim_timer_snatch_Live_Tick()
        {
            DateTime _dateTime_Now = _now;
            int intSecondsToStart = (int)datetime_snatch_Start.Subtract(_dateTime_Now).TotalSeconds + 1;
            int intSecondsToOpen = 0;

            if (intSecondsToStart > 0)
            {
                if ((int)button_snatch_Live_StageAdvance.Tag == 1)
                {
                    button_snatch_Live_StageAdvance.Tag = 0;
                    button_snatch_Live_StageAdvance.BackColor = AppColors.Snatch_Live_BG;
                }
                label_snatch_Live_TimeTillStart.Text = Seconds_To_String(intSecondsToStart);
                intSecondsToOpen = intSecondsToStart + (profileActive.Snatch_LiftsOut * profileActive.Snatch_SecondsStage);
                bool_snatch_LiveLifting = false;
            }
            else
            {
                if (!bool_snatch_LiveLifting)
                {
                    if (profileActive.Snatch_LiftsOut > 0)
                    {
                        bool_snatch_LiveLifting = true;
                        progressBar_snatch_Live_StageLift.Value = 0;
                    }
                    label_snatch_Live_TimeTillStart.Text = "passed";
                }

                if (profileActive.Snatch_LiftsOut > 0)
                {
                    if ((int)button_snatch_Live_StageAdvance.Tag == 0)
                    {
                        button_snatch_Live_StageAdvance.Tag = 1;
                        button_snatch_Live_StageAdvance.BackColor = AppColors.AdvanceButton_Active;
                    }
                    progressBar_snatch_Live_StageLift.PerformStep();
                    if (bool_snatch_AutoAdvance)
                    {
                        if (progressBar_snatch_Live_StageLift.Value == progressBar_snatch_Live_StageLift.Maximum)
                        {
                            snatch_Advance_StageLift();
                        }
                    }
                    intSecondsToOpen += (profileActive.Snatch_LiftsOut - 1) * profileActive.Snatch_SecondsStage +
                        progressBar_snatch_Live_StageLift.Maximum - progressBar_snatch_Live_StageLift.Value;
                }
                else
                {
                    if ((int)button_snatch_Live_StageAdvance.Tag == 1)
                    {
                        button_snatch_Live_StageAdvance.Tag = 0;
                        button_snatch_Live_StageAdvance.BackColor = AppColors.Snatch_Live_BG;
                    }
                }
            }


            TimeSpan _timeSpan_Open = TimeSpan.FromSeconds(intSecondsToOpen);
            if (intSecondsToOpen > 0)
            {
                label_snatch_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
                label_cj_Live_OpenTime.Text = _now.Add(_timeSpan_Open).ToString(@"HH\:mm\:ss");
            }
            else
            {
                label_snatch_Live_TimeTillOpener.Text = "-";
                label_snatch_Live_OpenTime.Text = "passed";
                bool_snatch_Live = false;
            }

            int _intStep = -1;
            foreach (Step _step in snatchStepsLIVE)
            {
                if (_step.TotalLengthReverse >= intSecondsToOpen &&
                    _step.Order > _intStep)
                {
                    _intStep = _step.Order;
                }
            }

            if (_intStep == -1) // adjust wait time
            {
                int intTLR = 0;
                foreach (Step _step in snatchStepsLIVE)
                {
                    if (_step.Order == 1)
                    {
                        intTLR = _step.TotalLengthReverse;
                        break;
                    }
                }
                if (intTLR > 0 & intSecondsToOpen > intTLR)
                {
                    int intSecToAdd = 0;
                    foreach (Step _step in snatchStepsLIVE)
                    {
                        if (_step.PreStep)
                        {
                            intSecToAdd = (intSecondsToOpen - intTLR) - _step.Length;
                            _step.Length = intSecondsToOpen - intTLR;
                            _step.Controls.ProgressBarStep.Maximum = intSecondsToOpen - intTLR;
                            _step.TotalLength = intSecondsToOpen - intTLR;
                            _step.TotalLengthReverse = intSecondsToOpen;
                            _intStep = 0;
                            break;
                        }
                    }
                    if (_intStep == 0 & intSecToAdd != 0)
                    {
                        foreach (Step _step in snatchStepsLIVE)
                        {
                            if (!_step.PreStep)
                            {
                                _step.TotalLength += intSecToAdd;
                            }
                        }
                    }
                }
            }

            if (_intStep > -1)
            {
                Label label_Action;
                ProgressBar progressBar_Step;
                Label label_Progress_Time;
                bool bool_UpdateStepHighlights = (_intStep != int_snatch_Warmup_Step);

                foreach (Step _step in snatchStepsLIVE)
                {
                    int _int_Order = _step.Order;
                    Label label_Time = _step.Controls.LabelTime;
                    if (_int_Order > _intStep)
                    {
                        label_Time.Visible = true;
                        label_Time.Text = $"in {Seconds_To_String((int)_timeSpan_Open.Add(TimeSpan.FromSeconds(-_step.TotalLengthReverse)).TotalSeconds)}";
                    }
                    else
                    {
                        label_Time.Visible = false;
                    }
                    Panel panel_Live_Step;
                    if (_int_Order == _intStep)
                    {
                        panel_Live_Step = _step.Controls.PanelLiveStep;
                        label_Progress_Time = _step.Controls.LabelProgressTime;
                        progressBar_Step = _step.Controls.ProgressBarStep;

                        int intStepLength = _step.Length;
                        int intSecIntoStep = _step.TotalLengthReverse - intSecondsToOpen;
                        label_Progress_Time.Text = Seconds_To_String(_int_Seconds: intStepLength - intSecIntoStep, _bool_ShortString: true);
                        progressBar_Step.Value = intSecIntoStep;

                        if (bool_UpdateStepHighlights)
                        {
                            label_Action = _step.Controls.LabelAction;
                            label_Action.Text = ActionTextString(_step: _step, _isFuture: false);
                            label_Progress_Time.Visible = true;
                        }

                        if (bool_UpdateStepHighlights)
                        {
                            panel_Live_Step.BackColor = AppColors.Live_Highlight_BG;
                            panel_Live_Step.ForeColor = AppColors.Live_Highlight_FG;
                        }
                    }
                    else if (bool_UpdateStepHighlights)
                    {
                        panel_Live_Step = _step.Controls.PanelLiveStep;
                        panel_Live_Step.BackColor = AppColors.Snatch_Live_BG;
                        panel_Live_Step.ForeColor = AppColors.Live_Default_FG;
                        label_Progress_Time = _step.Controls.LabelProgressTime;
                        label_Progress_Time.Visible = false;
                        progressBar_Step = _step.Controls.ProgressBarStep;
                        label_Action = _step.Controls.LabelAction;
                        bool _isFuture = (_int_Order >= _intStep);
                        string strActionText = ActionTextString(_step: _step, _isFuture: _isFuture);
                        bool boolIsLift = (_step.Weight > 0);
                        progressBar_Step.Value = (_isFuture ? 0 : progressBar_Step.Maximum);

                        label_Action.Text = strActionText;
                    }
                }
                int_snatch_Warmup_Step = _intStep;
                if (bool_UpdateStepHighlights & profileActive.Beep)
                {
                    Console.Beep(750, 600);
                }
            }
            else
            {
                snatch_Stop_Live();
            }

            label_snatch_Live_CurrentTime.Text = _dateTime_Now.ToString("HH:mm:ss");
        }
        private void progressBar_snatch_Live_StageLift_MouseClick(MouseEventArgs e)
        {
            if (bool_snatch_Live & bool_snatch_LiveLifting)
            {
                double dbl_Percent = e.X / (double)(progressBar_snatch_Live_StageLift.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_snatch_Live_StageLift.Value = (int)(progressBar_snatch_Live_StageLift.Maximum * dbl_Percent);
                }
            }
        }
        private void button_snatch_Live_LiftsDecr_Click()
        {
            if (profileActive.Snatch_LiftsOut > 0)
            {
                profileActive.Snatch_LiftsOut--;
                label_snatch_Live_LiftsOut.Text = profileActive.Snatch_LiftsOut.ToString();
                if (profileActive.Snatch_LiftsOut == 0)
                {
                    if (bool_snatch_LiveLifting)
                    {
                        bool_snatch_LiveLifting = false;
                        progressBar_snatch_Live_StageLift.Value = 0;
                    }
                }
            }
        }
        private void button_snatch_Live_LiftsIncr_Click()
        {
            if (profileActive.Snatch_LiftsOut < 99)
            {
                profileActive.Snatch_LiftsOut++;
                label_snatch_Live_LiftsOut.Text = profileActive.Snatch_LiftsOut.ToString();
            }
        }
        private void button_snatch_Live_StageAdvance_Click()
        {
            if (profileActive.Snatch_LiftsOut >= 0 & bool_snatch_LiveLifting)
            {
                snatch_Advance_StageLift();
            }
            else if (!bool_snatch_Live)
            {
                snatch_Start_Live();
            }
        }
        private void label_snatch_Live_LiftsOut_Click()
        {
            bool_Loading = true;
            textBox_snatch_Live_LiftsOut.Location = label_snatch_Live_LiftsOut.Location;
            textBox_snatch_Live_LiftsOut.Size = label_snatch_Live_LiftsOut.Size;
            textBox_snatch_Live_LiftsOut.Text = profileActive.Snatch_LiftsOut.ToString();
            textBox_snatch_Live_LiftsOut.Visible = true;
            textBox_snatch_Live_LiftsOut.BringToFront();
            textBox_snatch_Live_LiftsOut.Select();
            bool_Loading = false;
        }
        private void textBox_snatch_Live_LiftsOut_TextChanged()
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_snatch_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_snatch_Lifts_Out) &&
                _int_snatch_Lifts_Out >= 0 &&
                _int_snatch_Lifts_Out < 100 &&
                profileActive.Snatch_LiftsOut != _int_snatch_Lifts_Out)
            {
                profileActive.Snatch_LiftsOut = _int_snatch_Lifts_Out;
                label_snatch_Live_LiftsOut.Text = profileActive.Snatch_LiftsOut.ToString();
            }
        }
        private void textBox_snatch_Live_LiftsOut_Leave()
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_snatch_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_snatch_Lifts_Out) &&
                _int_snatch_Lifts_Out >= 0 &&
                _int_snatch_Lifts_Out < 100 &&
                profileActive.Snatch_LiftsOut != _int_snatch_Lifts_Out)
            {
                profileActive.Snatch_LiftsOut = _int_snatch_Lifts_Out;
                label_snatch_Live_LiftsOut.Text = profileActive.Snatch_LiftsOut.ToString();
            }
            textBox_snatch_Live_LiftsOut.Visible = false;
        }
        private void snatch_Advance_StageLift()
        {
            if (profileActive.Snatch_LiftsOut > 0)
            {
                profileActive.Snatch_LiftsOut--;
                label_snatch_Live_LiftsOut.Text = profileActive.Snatch_LiftsOut.ToString();
                if (bool_snatch_Live && datetime_snatch_Start < DateTime.Now)
                {
                    int_snatch_Lifts_Passed++;
                }
                else
                {
                    int_snatch_Lifts_Passed = 0;
                }
                label_snatch_Live_LiftsPassed.Text = (bool_snatch_Live ? int_snatch_Lifts_Passed.ToString() : string.Empty);
            }
            progressBar_snatch_Live_StageLift.Value = 0;
        }
        private void dateTimePicker_snatch_Start_ValueChanged()
        {
            datetime_snatch_Start = DateTime.Today.AddHours(dateTimePicker_snatch_Start.Value.Hour).AddMinutes(dateTimePicker_snatch_Start.Value.Minute);
            if (datetime_snatch_Start > DateTime.Now && int_snatch_Lifts_Passed != 0)
            {
                int_snatch_Lifts_Passed = 0;
                label_snatch_Live_LiftsPassed.Text = (bool_snatch_Live ? int_snatch_Lifts_Passed.ToString() : string.Empty);
            }
        }
        private void checkBox_snatch_Live_Auto_CheckedChanged()
        {
            bool_snatch_AutoAdvance = checkBox_snatch_Live_Auto.Checked;
        }
        private void checkBox_Live_Beep_CheckedChanged(CheckBox checkBox)
        {
            if (!bool_Loading)
            {
                bool_Loading = true;
                profileActive.Beep = checkBox.Checked;
                checkBox_snatch_Live_Beep.Checked = profileActive.Beep;
                checkBox_cj_Live_Beep.Checked = profileActive.Beep;
                bool_Loading = false;
            }
        }
        private void splitContainer_snatch_DoubleClick()
        {
            splitContainer_snatch.SplitterDistance = 0;
        }
        #endregion

        #region cj LIVE
        private void stopCJTimer()
        {
            if (timer_cj_Live != null)
            {
                timer_cj_Live.Stop();
                timer_cj_Live.Dispose();
                timer_cj_Live = null;
            }
            stopBatteryTimer();
        }
        private void cj_Stop_Live()
        {
            bool_cj_Live = false;
            stopCJTimer();
            label_cj_Live_TimeTillOpener.Text = String.Empty;
            textBox_cj_Live_snLeft.Visible = false;
            textBox_cj_Live_LiftsOut.Visible = false;
            label_cj_Live_LiftsPassed.Text = string.Empty;
            Clear_cj_Live_Steps();
            progressBar_cj_Live_StageLift.Value = 0;
            progressBar_cj_Live_sn.Value = 0;
            progressBar_cj_Live_Break.Value = 0;
            label_cj_Live_Break.Text = String.Empty;
            button_cj_Live_StageAdvance.BackColor = AppColors.Cj_Live_BG;
            button_cj_Live_StageAdvance.Tag = 0;
            button_cj_Live_snStageAdvance.BackColor = AppColors.Cj_Live_BG;
            button_cj_Live_snStageAdvance.Tag = 0;
            bool_cj_LiveLifting = false;
            bool_cj_BreakRunning = false;
            bool_cj_SnStillLifting = false;
            button_cj_Live_StartStop.Text = "start";
            panel_cj_Live_Times.Visible = false;
            panel_Battery.Visible = false;
            if (!bool_snatch_Live) { AllowMonitorPowerdown(); }
        }
        private void cj_Start_Live()
        {
            bool_cj_Live = true;
            stopCJTimer();
            textBox_cj_Live_snLeft.Visible = false;
            int_cj_Lifts_Passed = 0;
            textBox_cj_Live_LiftsOut.Visible = false;
            label_cj_Live_LiftsPassed.Text = "0";
            Populate_cj_Live_Steps();
            progressBar_cj_Live_StageLift.Value = 0;
            progressBar_cj_Live_StageLift.Maximum = profileActive.CJ_SecondsStage;
            progressBar_cj_Live_sn.Value = 0;
            progressBar_cj_Live_sn.Maximum = profileActive.Snatch_SecondsStage;
            progressBar_cj_Live_Break.Value = 0;
            progressBar_cj_Live_Break.Maximum = profileActive.CJ_SecondsBreak;
            label_cj_Live_Break.Text = String.Empty;
            label_cj_Live_TimeTillOpener.Text = String.Empty;

            // Run one tick immediately to populate UI without waiting
            sim_timer_cj_Live_Tick();
            timer_cj_Live = new Timer();
            timer_cj_Live.Tick += (s, e) => timer_Live_Tick(s, e, LiftType.CleanAndJerk);
            timer_cj_Live.Interval = timerInterval;
            timer_cj_Live.Start();
            timer_Battery = new Timer();
            timer_Battery.Tick += (s, e) => timer_Battery_Tick(s, e);
            timer_Battery.Interval = 60000;
            timer_Battery.Start();

            UpdateBattery();
            button_cj_Live_StartStop.Text = "stop";
            button_cj_Live_StageAdvance.Select();
            PreventMonitorPowerdown();
        }
        private void button_cj_Live_StartStop_Click()
        {
            if (bool_cj_Live)
            {
                cj_Stop_Live();
            }
            else
            {
                cj_Start_Live();
            }
        }
        private void Clear_cj_Live_Steps()
        {
            cjStepsLIVE = null;
            panel_cj_Live_Steps.Controls.Clear();
        }
        private void Populate_cj_Live_Steps()
        {
            Clear_cj_Live_Steps();

            if (cjStepsPLAN is null)
            {
                PopulateSteps(LiftType.CleanAndJerk);
            }
            if (cjStepsPLAN is null)
            {
                MessageBox.Show("An error has occurred. Step plan could not be determined.");
                this.Close();
                return;
            }
            cjStepsLIVE = [.. cjStepsPLAN.Select(r => r.Clone())];

            int intY = 1;
            int _int_panel_Live_Step_Width = panel_cj_Live_Steps.Width - 4;
            int _int_progressBar_Step_Width_NoScroll = _int_panel_Live_Step_Width - 350;
            int _int_progressBar_Step_Width_Scroll = _int_progressBar_Step_Width_NoScroll - SystemInformation.VerticalScrollBarWidth;
            int _int_progressBar_Step_Width;
            Point _point_progressBar_Step_Location = new(300, 6);

            SuspendLayout();
            foreach (Step _step in cjStepsLIVE)
            {
                if (panel_cj_Live_Steps.VerticalScroll.Visible)
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_Scroll;
                }
                else
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_NoScroll;
                }
                string strActionText = ActionTextString(_step: _step, _isFuture: true);
                Panel panel_Live_Step = new()
                {
                    Size = new Size(_int_panel_Live_Step_Width, 80),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = AppColors.Cj_Live_BG,
                    ForeColor = AppColors.Live_Default_FG,
                    Location = new Point(1, intY)
                };
                Label label_Action = new()
                {
                    Text = strActionText,
                    AutoSize = false,
                    Size = new Size(280, 75),
                    TextAlign = ContentAlignment.TopRight,
                    Location = new Point(6, 1)
                };
                ProgressBar progressBar_Step = new()
                {
                    Size = new Size(_int_progressBar_Step_Width, 65),
                    Location = _point_progressBar_Step_Location,
                    Maximum = _step.Length,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                };
                if (progressBar_Step.Maximum == 0)
                {
                    progressBar_Step.Maximum = 1;
                    progressBar_Step.Value = 1;
                }
                Label label_Weight = null;
                bool boolIsLift = ((_step.Weight ?? 0) > 0);
                if (boolIsLift)
                {
                    label_Weight = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.Black,
                        AutoSize = false,
                        Size = new Size(105, 30),
                        Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width - 105, 6),
                        ForeColor = SystemColors.Window,
                        Text = $"{(_step.isOpener ? "open at" : "lift")} {_step.Weight}",
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    };
                    label_Weight.Text = "lift " + _step.Weight.ToString();
                    label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Regular);
                    label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold);
                }
                Label label_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(120, 30),
                    Location = _point_progressBar_Step_Location,
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                };
                Label label_Progress_Time = new()
                {
                    Text = String.Empty,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = false,
                    Size = new Size(105, 30),
                    Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width - 105, 6),
                    Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
                    Visible = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                };
                panel_Live_Step.Controls.AddRange(
                [
                    label_Action,
                    progressBar_Step,
                    label_Progress_Time,
                    label_Time
                ]);
                WeightBox weightBoxGraphic = null;
                if (boolIsLift)
                {
                    weightBoxGraphic = new(
                        isOpener: false,
                        isCompCollars: _step.isOpener,
                        barWeight: profileActive.BarbellWeight,
                        weight: _step.Weight ?? 0)
                    {
                        Size = new Size(120, 65),
                        BackColor = Color.FromArgb(196, 196, 196),
                        BorderStyle = BorderStyle.Fixed3D,
                        Location = new Point(10, 6),
                        Visible = true
                    };
                    panel_Live_Step.Controls.Add(weightBoxGraphic);
                }
                progressBar_Step.BringToFront();
                _step.Controls.PanelLiveStep = panel_Live_Step;
                _step.Controls.LabelAction = label_Action;
                _step.Controls.ProgressBarStep = progressBar_Step;
                weightBoxGraphic?.BringToFront();
                if (label_Weight != null)
                {
                    panel_Live_Step.Controls.Add(label_Weight);
                    _step.Controls.LabelWeight = label_Weight;
                    label_Weight.BringToFront();
                }
                label_Time.BringToFront();
                _step.Controls.LabelTime = label_Time;
                label_Progress_Time.BringToFront();
                _step.Controls.LabelProgressTime = label_Progress_Time;
                panel_cj_Live_Steps.Controls.Add(panel_Live_Step);

                intY += 81;
            }
            int_cj_Warmup_Step = -1;
            label_cj_Live_CurrentTime.Text = _now.ToString("HH:mm:ss");
            panel_cj_Live_Times.Visible = true;
            ResumeLayout();
        }
        private void sim_timer_cj_Live_Tick()
        {
            int intSecondsToOpen = 0;

            if (bool_cj_SnStillLifting) // snatches still running
            {
                if (profileActive.CJ_SnatchLifts_Out == 0)
                {
                    bool_cj_SnStillLifting = false;
                    bool_cj_BreakRunning = true;
                    progressBar_cj_Live_Break.Value = 0;
                    bool_cj_LiveLifting = false;
                }
                else
                {
                    progressBar_cj_Live_sn.PerformStep();
                    if (bool_cj_AutoAdvance)
                    {
                        if (progressBar_cj_Live_sn.Value == progressBar_cj_Live_sn.Maximum)
                        {
                            cj_Advance_snLift();
                        }
                    }

                    if (profileActive.CJ_SnatchLifts_Out == 0)
                    {
                        bool_cj_SnStillLifting = false;
                        progressBar_cj_Live_Break.Value = 0;
                        bool_cj_BreakRunning = true;
                    }
                    else
                    {
                        bool_cj_BreakRunning = false;
                    }
                    bool_cj_LiveLifting = false;
                }
            }
            else if (profileActive.CJ_SnatchLifts_Out > 0)
            {
                bool_cj_SnStillLifting = true;
                progressBar_cj_Live_Break.Value = 0;
                bool_cj_BreakRunning = false;
                bool_cj_LiveLifting = false;
            }

            if (bool_cj_BreakRunning) // break is running
            {
                progressBar_cj_Live_Break.PerformStep();
                if (progressBar_cj_Live_Break.Value == progressBar_cj_Live_Break.Maximum)
                {
                    bool_cj_BreakRunning = false;
                    progressBar_cj_Live_StageLift.Value = 0;
                    label_cj_Live_Break.Text = String.Empty;
                    bool_cj_LiveLifting = true;
                }
                else
                {
                    bool_cj_LiveLifting = false;
                    label_cj_Live_Break.Text = Seconds_To_String(progressBar_cj_Live_Break.Maximum - progressBar_cj_Live_Break.Value);
                }
            }
            else if (!bool_cj_SnStillLifting)
            {
                if (progressBar_cj_Live_Break.Value < progressBar_cj_Live_Break.Maximum)
                {
                    bool_cj_BreakRunning = true;
                    progressBar_cj_Live_StageLift.Value = 0;
                    label_cj_Live_Break.Text = Seconds_To_String(0);
                    bool_cj_LiveLifting = false;
                }
            }
            else
            {
                label_cj_Live_Break.Text = String.Empty;
            }

            if (bool_cj_LiveLifting) // stage lifts are going
            {
                progressBar_cj_Live_StageLift.PerformStep();
                if (progressBar_cj_Live_StageLift.Value == progressBar_cj_Live_StageLift.Maximum)
                {
                    if (bool_cj_AutoAdvance | profileActive.CJ_LiftsOut == 1)
                    {
                        cj_Advance_StageLift();
                    }
                }

                if (profileActive.CJ_LiftsOut == 0)
                {
                    bool_cj_LiveLifting = false;
                }
            }
            else if (!bool_cj_SnStillLifting & !bool_cj_BreakRunning)
            {
                if (profileActive.CJ_LiftsOut > 0)
                {
                    if (progressBar_cj_Live_StageLift.Value < progressBar_cj_Live_StageLift.Maximum)
                    {
                        bool_cj_LiveLifting = true;
                        progressBar_cj_Live_StageLift.Value = 0;
                    }
                }
            }


            if (bool_cj_SnStillLifting)
            {
                if ((int)button_cj_Live_snStageAdvance.Tag == 0)
                {
                    button_cj_Live_snStageAdvance.Tag = 1;
                    button_cj_Live_snStageAdvance.BackColor = AppColors.AdvanceButton_Active;
                }
            }
            else
            {
                if ((int)button_cj_Live_snStageAdvance.Tag == 1)
                {
                    button_cj_Live_snStageAdvance.Tag = 0;
                    button_cj_Live_snStageAdvance.BackColor = AppColors.Cj_Live_BG;
                }
            }

            if (bool_cj_LiveLifting)
            {
                if ((int)button_cj_Live_StageAdvance.Tag == 0)
                {
                    button_cj_Live_StageAdvance.Tag = 1;
                    button_cj_Live_StageAdvance.BackColor = AppColors.AdvanceButton_Active;
                }
            }
            else
            {
                if ((int)button_cj_Live_StageAdvance.Tag == 1)
                {
                    button_cj_Live_StageAdvance.Tag = 0;
                    button_cj_Live_StageAdvance.BackColor = AppColors.Cj_Live_BG;
                }
            }


            if (bool_cj_SnStillLifting | bool_cj_BreakRunning | bool_cj_LiveLifting)
            {
                intSecondsToOpen = (profileActive.CJ_LiftsOut - 1) * profileActive.CJ_SecondsStage +
                    progressBar_cj_Live_StageLift.Maximum - progressBar_cj_Live_StageLift.Value;
            }

            if (bool_cj_SnStillLifting | bool_cj_BreakRunning)
            {
                intSecondsToOpen += progressBar_cj_Live_Break.Maximum - progressBar_cj_Live_Break.Value;
            }

            if (bool_cj_SnStillLifting) // snatches still running
            {
                intSecondsToOpen += (profileActive.CJ_SnatchLifts_Out - 1) * profileActive.Snatch_SecondsStage +
                    progressBar_cj_Live_sn.Maximum - progressBar_cj_Live_sn.Value;
            }


            TimeSpan _timeSpan_Open = TimeSpan.FromSeconds(intSecondsToOpen);
            if (intSecondsToOpen > 0)
            {
                label_cj_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
                label_cj_Live_OpenTime.Text = _now.Add(_timeSpan_Open).ToString(@"HH\:mm\:ss");
            }
            else
            {
                label_cj_Live_TimeTillOpener.Text = "-";
                label_cj_Live_OpenTime.Text = "passed";
                bool_cj_Live = false;
            }

            int _intStep = -1;
            foreach (Step _step in cjStepsLIVE)
            {
                if (_step.TotalLengthReverse >= intSecondsToOpen && _step.Order > _intStep)
                {
                    _intStep = _step.Order;
                }
            }

            if (_intStep == -1) // adjust wait time
            {
                int intTLR = 0;
                foreach (Step _step in cjStepsLIVE)
                {
                    if (_step.Order == 1)
                    {
                        intTLR = _step.TotalLengthReverse;
                        break;
                    }
                }
                if (intTLR > 0 & intSecondsToOpen > intTLR)
                {
                    int intSecToAdd = 0;
                    foreach (Step _step in cjStepsLIVE)
                    {
                        if (_step.PreStep)
                        {
                            intSecToAdd = (intSecondsToOpen - intTLR) - _step.Length;
                            _step.Length = intSecondsToOpen - intTLR;
                            _step.Controls.ProgressBarStep.Maximum = intSecondsToOpen - intTLR;
                            _step.TotalLength = intSecondsToOpen - intTLR;
                            _step.TotalLengthReverse = intSecondsToOpen;
                            _intStep = 0;
                            break;
                        }
                    }
                    if (_intStep == 0 & intSecToAdd != 0)
                    {
                        foreach (Step _step in cjStepsLIVE)
                        {
                            if (!_step.PreStep)
                            {
                                _step.TotalLength += intSecToAdd;
                            }
                        }
                    }
                }
            }

            if (_intStep > -1)
            {
                Label label_Action;
                ProgressBar progressBar_Step;
                Label label_Progress_Time;
                bool bool_UpdateStepHighlights = (_intStep != int_cj_Warmup_Step);

                foreach (Step _step in cjStepsLIVE)
                {
                    int _int_Order = _step.Order;
                    Label label_Time = _step.Controls.LabelTime;
                    if (_int_Order > _intStep)
                    {
                        label_Time.Visible = true;
                        label_Time.Text = $"in {Seconds_To_String((int)_timeSpan_Open.Add(TimeSpan.FromSeconds(-_step.TotalLengthReverse)).TotalSeconds)}";
                    }
                    else
                    {
                        label_Time.Visible = false;
                    }
                    if (_int_Order == _intStep)
                    {
                        Panel _panel_Live_Step = _step.Controls.PanelLiveStep;
                        label_Progress_Time = _step.Controls.LabelProgressTime;
                        progressBar_Step = _step.Controls.ProgressBarStep;

                        int intStepLength = _step.Length;
                        int intSecIntoStep = _step.TotalLengthReverse - intSecondsToOpen;
                        label_Progress_Time.Text = Seconds_To_String(_int_Seconds: intStepLength - intSecIntoStep, _bool_ShortString: true);
                        progressBar_Step.Value = intSecIntoStep;

                        if (bool_UpdateStepHighlights)
                        {
                            string strActionText = ActionTextString(_step: _step, _isFuture: false);
                            label_Action = _step.Controls.LabelAction;
                            label_Action.Text = strActionText;
                            label_Progress_Time.Visible = true;
                        }

                        if (bool_UpdateStepHighlights)
                        {
                            _panel_Live_Step.BackColor = AppColors.Live_Highlight_BG;
                            _panel_Live_Step.ForeColor = AppColors.Live_Highlight_FG;
                        }
                    }
                    else if (bool_UpdateStepHighlights)
                    {
                        Panel _panel_Live_Step = _step.Controls.PanelLiveStep;
                        _panel_Live_Step.BackColor = AppColors.Cj_Live_BG;
                        _panel_Live_Step.ForeColor = AppColors.Live_Default_FG;
                        label_Progress_Time = _step.Controls.LabelProgressTime;
                        label_Progress_Time.Visible = false;
                        progressBar_Step = _step.Controls.ProgressBarStep;
                        bool _isFuture = (_int_Order >= _intStep);
                        progressBar_Step.Value = (_isFuture ? 0 : progressBar_Step.Maximum);
                        label_Action = _step.Controls.LabelAction;
                        string strActionText = ActionTextString(_step: _step, _isFuture: _isFuture);
                        label_Action.Text = strActionText;
                    }
                }
                int_cj_Warmup_Step = _intStep;
                if (bool_UpdateStepHighlights & profileActive.Beep)
                {
                    Console.Beep(750, 600);
                }
            }
            else
            {
                cj_Stop_Live();
            }

            label_cj_Live_CurrentTime.Text = _now.ToString("HH:mm:ss");
        }
        private void progressBar_cj_Live_StageLift_MouseClick(MouseEventArgs e)
        {
            if (bool_cj_Live & bool_cj_LiveLifting)
            {
                double dbl_Percent = e.X / (double)(progressBar_cj_Live_StageLift.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_cj_Live_StageLift.Value = (int)(progressBar_cj_Live_StageLift.Maximum * dbl_Percent);
                }
            }
        }
        private void progressBar_cj_Live_sn_MouseClick(MouseEventArgs e)
        {
            if (bool_cj_Live & bool_cj_SnStillLifting)
            {
                double dbl_Percent = e.X / (double)(progressBar_cj_Live_sn.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_cj_Live_sn.Value = (int)(progressBar_cj_Live_sn.Maximum * dbl_Percent);
                }
            }
        }
        private void progressBar_cj_Live_Break_MouseClick(MouseEventArgs e)
        {
            if (bool_cj_Live & bool_cj_BreakRunning)
            {
                double dbl_Percent = e.X / (double)(progressBar_cj_Live_Break.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_cj_Live_Break.Value = (int)(progressBar_cj_Live_Break.Maximum * dbl_Percent);
                }
            }
        }
        private void button_cj_Live_LiftsDecr_Click()
        {
            if (profileActive.CJ_LiftsOut > 0)
            {
                profileActive.CJ_LiftsOut--;
                label_cj_Live_LiftsOut.Text = profileActive.CJ_LiftsOut.ToString();
                if (profileActive.CJ_LiftsOut == 0)
                {
                    if (bool_cj_LiveLifting)
                    {
                        bool_cj_LiveLifting = false;
                        progressBar_cj_Live_StageLift.Value = 0;
                    }
                }
            }
        }
        private void button_cj_Live_LiftsIncr_Click()
        {
            if (profileActive.CJ_LiftsOut < 99)
            {
                profileActive.CJ_LiftsOut++;
                label_cj_Live_LiftsOut.Text = profileActive.CJ_LiftsOut.ToString();
            }
        }
        private void label_cj_Live_LiftsOut_Click()
        {
            bool_Loading = true;
            textBox_cj_Live_LiftsOut.Location = label_cj_Live_LiftsOut.Location;
            textBox_cj_Live_LiftsOut.Size = label_cj_Live_LiftsOut.Size;
            textBox_cj_Live_LiftsOut.Text = profileActive.CJ_LiftsOut.ToString();
            textBox_cj_Live_LiftsOut.Visible = true;
            textBox_cj_Live_LiftsOut.BringToFront();
            textBox_cj_Live_LiftsOut.Select();
            bool_Loading = false;
        }
        private void textBox_cj_Live_LiftsOut_TextChanged()
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_Lifts_Out) &&
                _int_cj_Lifts_Out >= 0 &&
                _int_cj_Lifts_Out < 100 &&
                profileActive.CJ_LiftsOut != _int_cj_Lifts_Out)
            {
                profileActive.CJ_LiftsOut = _int_cj_Lifts_Out;
                label_cj_Live_LiftsOut.Text = profileActive.CJ_LiftsOut.ToString();
            }
        }
        private void textBox_cj_Live_LiftsOut_Leave()
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_Lifts_Out) &&
                _int_cj_Lifts_Out >= 0 &&
                _int_cj_Lifts_Out < 100 &&
                profileActive.CJ_LiftsOut != _int_cj_Lifts_Out)
            {
                profileActive.CJ_LiftsOut = _int_cj_Lifts_Out;
                label_cj_Live_LiftsOut.Text = profileActive.CJ_LiftsOut.ToString();
            }
            textBox_cj_Live_LiftsOut.Visible = false;
        }
        private void button_cj_Live_StageAdvance_Click()
        {
            if (profileActive.CJ_LiftsOut >= 0 && bool_cj_LiveLifting)
            {
                cj_Advance_StageLift();
            }
            else if (profileActive.CJ_SnatchLifts_Out >= 0 && bool_cj_SnStillLifting)
            {
                cj_Advance_snLift();
            }
            else if (!bool_cj_Live)
            {
                cj_Start_Live();
            }
        }
        private void cj_Advance_StageLift()
        {
            if (profileActive.CJ_LiftsOut > 0)
            {
                profileActive.CJ_LiftsOut--;
                label_cj_Live_LiftsOut.Text = profileActive.CJ_LiftsOut.ToString();
                if (bool_cj_Live && !bool_cj_SnStillLifting && !bool_cj_BreakRunning)
                {
                    int_cj_Lifts_Passed++;
                }
                else
                {
                    int_cj_Lifts_Passed = 0;
                }
                label_cj_Live_LiftsPassed.Text = (bool_cj_Live ? int_cj_Lifts_Passed.ToString() : string.Empty);
            }
            progressBar_cj_Live_StageLift.Value = 0;
        }
        private void checkBox_cj_Live_Auto_CheckedChanged()
        {
            bool_cj_AutoAdvance = checkBox_cj_Live_Auto.Checked;
        }
        private void numericUpDown_cj_Live_Break_ValueChanged()
        {
            if (bool_Loading) { return; }
            cj_Break_Updated();
        }
        private void cj_Break_Updated()
        {
            int _int_cj_Sec_Break = (int)(numericUpDown_cj_Live_Break.Value);
            if (_int_cj_Sec_Break < 1)
            {
                profileActive.CJ_SecondsBreak = 1;
            }
            else
            {
                profileActive.CJ_SecondsBreak = _int_cj_Sec_Break * 60;
            }

            if (progressBar_cj_Live_Break.Value > profileActive.CJ_SecondsBreak)
            { progressBar_cj_Live_Break.Value = profileActive.CJ_SecondsBreak; }
            progressBar_cj_Live_Break.Maximum = profileActive.CJ_SecondsBreak;
        }
        private void button_cj_Live_snDecr_Click()
        {
            if (profileActive.CJ_SnatchLifts_Out > 0)
            {
                profileActive.CJ_SnatchLifts_Out--;
                label_cj_Live_snLeft.Text = profileActive.CJ_SnatchLifts_Out.ToString();
                if (profileActive.CJ_SnatchLifts_Out == 0)
                {
                    if (bool_cj_SnStillLifting)
                    {
                        bool_cj_SnStillLifting = false;
                        progressBar_cj_Live_sn.Value = 0;
                    }
                }
            }
        }
        private void button_cj_Live_snIncr_Click()
        {
            if (profileActive.CJ_SnatchLifts_Out < 98)
            {
                profileActive.CJ_SnatchLifts_Out++;
                label_cj_Live_snLeft.Text = profileActive.CJ_SnatchLifts_Out.ToString();
            }
        }
        private void label_cj_Live_snLeft_Click()
        {
            bool_Loading = true;
            textBox_cj_Live_snLeft.Location = label_cj_Live_snLeft.Location;
            textBox_cj_Live_snLeft.Size = label_cj_Live_snLeft.Size;
            textBox_cj_Live_snLeft.Text = profileActive.CJ_SnatchLifts_Out.ToString();
            textBox_cj_Live_snLeft.Visible = true;
            textBox_cj_Live_snLeft.BringToFront();
            textBox_cj_Live_snLeft.Select();
            bool_Loading = false;
        }
        private void textBox_cj_Live_snLeft_TextChanged()
        {
            if (bool_Loading) { return; }
            Override_SnatchesLeft();
        }
        private void textBox_cj_Live_snLeft_Leave()
        {
            if (bool_Loading) { return; }
            Override_SnatchesLeft();
            textBox_cj_Live_snLeft.Visible = false;
        }
        private void Override_SnatchesLeft()
        {
            string _str_Input = textBox_cj_Live_snLeft.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_snLifts_Out) &&
                _int_cj_snLifts_Out >= 0 &&
                _int_cj_snLifts_Out < 100 &&
                profileActive.CJ_SnatchLifts_Out != _int_cj_snLifts_Out)
            {
                profileActive.CJ_SnatchLifts_Out = _int_cj_snLifts_Out;
                label_cj_Live_snLeft.Text = profileActive.CJ_SnatchLifts_Out.ToString();
            }
        }
        private void label_cj_Live_Break_Click()
        {
            bool_Loading = true;
            textBox_cj_Live_Break.Location = label_cj_Live_Break.Location;
            textBox_cj_Live_Break.Size = label_cj_Live_Break.Size;
            textBox_cj_Live_Break.Text = profileActive.CJ_SnatchLifts_Out.ToString();
            textBox_cj_Live_Break.Visible = true;
            textBox_cj_Live_Break.BringToFront();
            textBox_cj_Live_Break.Select();
            bool_Loading = false;
        }
        private void textBox_cj_Live_Break_TextChanged()
        {
            if (bool_Loading) { return; }
            Override_BreakLeft();
        }
        private void textBox_cj_Live_Break_Leave()
        {
            if (bool_Loading) { return; }
            Override_BreakLeft();
            textBox_cj_Live_Break.Visible = false;
        }
        private void Override_BreakLeft()
        {
            string _str_Input = textBox_cj_Live_Break.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_Break) &&
                _int_cj_Break >= 0 &&
                _int_cj_Break <= numericUpDown_cj_Live_Break.Maximum * 60)
            {
                if (_int_cj_Break > (int)(numericUpDown_cj_Live_Break.Value * 60m))
                {
                    numericUpDown_cj_Live_Break.Value = Math.Ceiling(_int_cj_Break / 60m);
                    cj_Break_Updated();
                }
                progressBar_cj_Live_Break.Value = progressBar_cj_Live_Break.Maximum - _int_cj_Break;
                label_cj_Live_Break.Text = $"{_int_cj_Break}";
            }
        }
        private void Cj_Advance_SnatchLift()
        {
            if (profileActive.CJ_LiftsOut >= 0 & bool_cj_LiveLifting)
            {
                cj_Advance_StageLift();
            }
            else if (profileActive.CJ_SnatchLifts_Out >= 0 & bool_cj_SnStillLifting)
            {
                cj_Advance_snLift();
            }
        }
        private void cj_Advance_snLift()
        {
            if (profileActive.CJ_SnatchLifts_Out > 0)
            {
                profileActive.CJ_SnatchLifts_Out--;
                label_cj_Live_snLeft.Text = profileActive.CJ_SnatchLifts_Out.ToString();
            }
            progressBar_cj_Live_sn.Value = 0;
        }
        private void Cj_Splitter_DoubleClick()
        {
            splitContainer_cj.SplitterDistance = 0;
        }
        #endregion
    }
}