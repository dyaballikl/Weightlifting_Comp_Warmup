using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace Weightlifting_Comp_Warmup.Main
{
    public partial class form_Main : Form
    {
        #region Form Level
        public form_Main()
        {
            //defaults.Reset();
            //Settings_Changes_Save();
            Clean_Settings();
            //defaults.Reset();
            //Settings_Changes_Save();

            int _int_ProfileId = -1;
            try
            {
                _int_ProfileId = savedSettings.int_ProfileId;
            }
            catch { }
            ;

            if (_int_ProfileId < 1)
            {
                if (savedSettings.ii_int_ProfileIds != null &&
                    savedSettings.ii_int_ProfileIds.Count > 0)
                {
                    int.TryParse(s: savedSettings.ii_int_ProfileIds[0], result: out _int_ProfileId);
                }
            }
            if (_int_ProfileId < 1)
            {
                _int_ProfileId = Add_Profile(_str_ProfileName: "default");
                Print_All_Settings();
            }
            ProfileId_Select(_int_ProfileId: _int_ProfileId);

            InitializeComponent();
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void buttonRestore_Click(object sender, EventArgs e)
        {
            buttonRestore.Dispose();
            buttonClose.Dispose();
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
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
        private void Form_WL_Comp_Warmup_FormClosing(object sender, FormClosingEventArgs e)
        {
            Update_Settings();
            Settings_Changes_Save();
        }
        private void InitialiseCollections()
        {
            snatchExtras = [];
            snatchJumps = [];
            snatchTimes = [];
            cjExtras = [];
            cjJumps = [];
            cjTimes = [];
        }
        private void CheckCollections()
        {
            snatchExtras ??= [];
            snatchJumps ??= [];
            snatchTimes ??= [];
            cjExtras ??= [];
            cjJumps ??= [];
            cjTimes ??= [];

            if (!snatchJumps.TryGetValue(1, out _))
            {
                snatchJumps[1] = 1;
            }
            if (!snatchTimes.TryGetValue(1, out _))
            {
                snatchTimes[1] = 1;
            }
            if (!cjJumps.TryGetValue(1, out _))
            {
                cjJumps[1] = 1;
            }
            if (!cjTimes.TryGetValue(1, out _))
            {
                cjTimes[1] = 1;
            }
        }
        private void numericUpDown_snatch_weight_barbell_ValueChanged(object sender, EventArgs e)
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

            int_Barbell = _int_Barbell;

            numericUpDown_snatch_weight_opener.Minimum = _int_Barbell;
            numericUpDown_cj_weight_opener.Minimum = _int_Barbell;

            Snatch_Opener_Set();
            CJ_Opener_Set();
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
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

        #region snatch_Setup_Controls
        private void numericUpDown_snatch_time_stage_ValueChanged(object sender, EventArgs e)
        {
            int _int_snatch_Sec_Stage;
            try
            {
                _int_snatch_Sec_Stage = (int)(numericUpDown_snatch_time_stage.Value);
            }
            catch
            {
                (numericUpDown_snatch_time_stage).BackColor = Color.Yellow;
                return;
            }

            if (_int_snatch_Sec_Stage < 1)
            {
                (numericUpDown_snatch_time_stage).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_snatch_time_stage).BackColor = Color.White;

            int_snatch_Sec_Stage = _int_snatch_Sec_Stage;

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_snatch_weight_opener_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            Snatch_Opener_Set();
        }
        private void checkBox_snatch_Param_OpenerWarmup_CheckedChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            bool_snatch_OpenerWarmup = checkBox_snatch_Param_OpenerWarmup.Checked;
            Snatch_Opener_Set();
        }
        private void Snatch_Opener_Set()
        {
            int _int_snatch_Wgt_Opener;
            try
            {
                _int_snatch_Wgt_Opener = (int)(numericUpDown_snatch_weight_opener.Value);
            }
            catch
            {
                numericUpDown_snatch_weight_opener.BackColor = Color.Yellow;
                return;
            }

            if (_int_snatch_Wgt_Opener < 1)
            {
                numericUpDown_snatch_weight_opener.BackColor = Color.Yellow;
                return;
            }
            numericUpDown_snatch_weight_opener.BackColor = Color.White;

            int_snatch_Wgt_Opener = _int_snatch_Wgt_Opener;

            Apply_Opener_Graphic_Vector(_intWeightBar: int_Barbell, _intWeightOpener: int_snatch_Wgt_Opener, _boolSnatch: true);
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void numericUpDown_snatch_time_PostWarmup_ValueChanged(object sender, EventArgs e)
        {
            int _int_snatch_Sec_End;
            try
            {
                _int_snatch_Sec_End = (int)(numericUpDown_snatch_time_PostWarmup.Value);
            }
            catch
            {
                (numericUpDown_snatch_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }

            if (_int_snatch_Sec_End < 0)
            {
                (numericUpDown_snatch_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_snatch_time_PostWarmup).BackColor = Color.White;

            int_snatch_Sec_End = _int_snatch_Sec_End;

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        #endregion

        #region snatch extras
        private void snatch_Populate_Extras()
        {
            snatch_Stop_Live();
            int intY = 1;
            panel_snatch_extra.Controls.Clear();

            void snatch_Add_Extra_IndividualControls(
                int intY,
                int intId,
                string strTBText,
                int intLength,
                bool bool_Add_Blank)
            {
                TextBox tb = new()
                {
                    Text = strTBText,
                    Location = new Point(6, intY),
                    Size = new Size(157, 25),
                    Tag = intId,
                    BackColor = Color.White
                };
                NumericUpDown nmud = new()
                {
                    Location = new Point(169, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intLength, 0, 0, 0]),
                    Tag = intId,
                    BackColor = Color.White
                };
                Label lbl = new()
                {
                    Location = new Point(250, intY + 3),
                    Text = Seconds_To_String(intLength),
                    Tag = intId
                };
                tb.TextChanged += textBox_snatch_extra_TextChanged;
                nmud.ValueChanged += numericUpDown_snatch_extra_ValueChanged;
                panel_snatch_extra.Controls.AddRange([tb, nmud, lbl]);

                if (bool_Add_Blank)
                {
                    Button btn4 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(258 + 100, intY),
                        Size = new Size(125, 25),
                        Text = str_buttontext_commit,
                        UseVisualStyleBackColor = true
                    };
                    btn4.Click += button_snatch_extra_commit_click;
                    panel_snatch_extra.Controls.Add(btn4);

                    tb.Select();
                }
                else
                {
                    Button btn1 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(258 + 100, intY),
                        Size = new Size(32, 25),
                        Text = str_buttontext_up,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    Button btn2 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(301 + 100, intY),
                        Size = new Size(32, 25),
                        Text = str_buttontext_down,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    Button btn3 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(347 + 100, intY),
                        Size = new Size(36, 25),
                        Text = str_buttontext_delete,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    btn1.Click += button_snatch_extra_up_click;
                    btn2.Click += button_snatch_extra_down_click;
                    btn3.Click += button_snatch_extra_delete_click;
                    panel_snatch_extra.Controls.AddRange([btn1, btn2, btn3]);
                }
            }

            foreach (Extra _extra in snatchExtras.OrderBy(r => r.Order))
            {
                snatch_Add_Extra_IndividualControls(
                    intY: intY,
                    intId: _extra.id,
                    strTBText: _extra.Action,
                    intLength: _extra.Length,
                    bool_Add_Blank: false);
                intY += 30;
            }
            snatch_Add_Extra_IndividualControls(
                intY: intY,
                intId: -1,
                strTBText: string.Empty,
                intLength: 60,
                bool_Add_Blank: true);
        }
        private void button_snatch_extra_up_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            Extra _extra = snatchExtras.FirstOrDefault(r => r.id == _id);
            if (_extra.id < 1)
            {
                return;
            }
            int _order = _extra.Order;

            if (_order < 1)
            { return; }

            for (int i = 0; i < snatchExtras.Count; i++)
            {
                _extra = snatchExtras[i];
                if (_extra.id == _id && _extra.Order == _order)
                {
                    _extra.Order = _order - 1;
                }
                else if (_extra.Order == _order - 1)
                {
                    _extra.Order = _order;
                }
                snatchExtras[i] = _extra;
            }

            snatch_Populate_Extras();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_extra_down_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            Extra _extra = snatchExtras.FirstOrDefault(r => r.id == _id);
            if (_extra.id < 1)
            {
                return;
            }
            int _order = _extra.Order;
            int _max = snatchExtras_Max_Order();

            if (_order < 0 || _order == _max)
            {
                return;
            }

            for (int i = 0; i < snatchExtras.Count; i++)
            {
                _extra = snatchExtras[i];
                if (_extra.id == _id && _extra.Order == _order)
                {
                    _extra.Order = _order + 1;
                }
                else if (_extra.Order == _order + 1)
                {
                    _extra.Order = _order;
                }
                snatchExtras[i] = _extra;
            }

            snatch_Populate_Extras();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_extra_delete_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);

            snatchExtras.RemoveAll(r => r.id == _id);

            snatchExtras_Reassign_Order();
            snatch_Populate_Extras();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_extra_commit_click(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            string strAction = string.Empty;
            int intLength = -1;

            foreach (Control ctrl in panel_snatch_extra.Controls)
            {
                if (strAction != String.Empty & intLength > -1)
                { break; }
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if ((int)(((TextBox)ctrl).Tag) == -1)
                    {
                        strAction = ((TextBox)ctrl).Text;
                        if (strAction == String.Empty)
                        {
                            MessageBox.Show("Action cannot be blank");
                            return;
                        }
                    }
                }
                else if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(_numericUpDown.Tag) == -1)
                    {
                        intLength = (int)(_numericUpDown.Value);
                    }
                }
            }
            if (strAction != String.Empty & intLength > -1)
            {
                if (intLength < 1)
                {
                    MessageBox.Show("Length cannot be < 1");
                    return;
                }
                Extra _extra = new(action: strAction, length: intLength, order: snatchExtras_Max_Order() + 1);
                snatchExtras.Add(_extra);
                snatchExtras_Reassign_Order();
                snatch_Populate_Extras();
                snatch_Populate_Steps(boolPreserveLifts: true);
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                snatch_Populate_Extras();
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void textBox_snatch_extra_TextChanged(object sender, EventArgs e)
        {
            int _id = (int)(((TextBox)(sender)).Tag);

            if (_id < 1) { return; }

            string strAction = ((TextBox)sender).Text;

            if (strAction == String.Empty)
            {
                ((TextBox)sender).BackColor = Color.Yellow;
                return;
            }
            else
            {
                ((TextBox)sender).BackColor = Color.White;
            }

            for (int i = 0; i < snatchExtras.Count; i++)
            {
                Extra _extra = snatchExtras[i];
                if (_extra.id == _id)
                {
                    _extra.Action = strAction;
                    snatchExtras[i] = _extra;
                    return;
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_snatch_extra_ValueChanged(object sender, EventArgs e)
        {
            int _id = (int)(((NumericUpDown)(sender)).Tag);

            if (_id < 1) { return; }

            int intLength;
            try
            {
                intLength = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intLength < 1) { return; }
            ((NumericUpDown)sender).BackColor = Color.White;

            for (int i = 0; i < snatchExtras.Count; i++)
            {
                Extra _extra = snatchExtras[i];
                if (_extra.id == _id)
                {
                    _extra.Length = intLength;
                    snatchExtras[i] = _extra;
                    break;
                }
            }

            foreach (Control control in panel_snatch_extra.Controls)
            {
                if (control.GetType() == typeof(Label))
                {
                    if ((int)((Label)control).Tag == _id)
                    {
                        ((Label)control).Text = Seconds_To_String(intLength);
                        break;
                    }
                }
            }

            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private int snatchExtras_Max_Order()
        {
            return snatchExtras.Max(r => r.Order);
        }
        private void snatchExtras_Reassign_Order()
        {
            snatchExtras = [.. snatchExtras.OrderBy(r => r.Order)];
            int _order = 0;
            for (int i = 0; i < snatchExtras.Count; i++)
            {
                Extra _extra = snatchExtras[i];
                _extra.Order = _order; _order++;
                snatchExtras[i] = _extra;
            }
        }
        #endregion

        #region snatch jumps
        private void snatch_Populate_Jumps()
        {
            snatch_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intJump = 1;
            panel_snatch_jump.Controls.Clear();

            if (snatchJumps.Count == 0)
            {
                snatchJumps = Defaults.default_snatchJumps();
            }

            void snatch_Add_Jump_IndividualControls(
                int intY,
                int intFromWeight,
                int intJump,
                bool bool_Add_Blank)
            {
                NumericUpDown nmud1 = new()
                {
                    Location = new Point(6, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intFromWeight, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                NumericUpDown nmud2 = new()
                {
                    Location = new Point(100, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intJump, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                nmud1.ValueChanged += numericUpDown_snatch_jump_FromWeight_ValueChanged;
                nmud2.ValueChanged += numericUpDown_snatch_jump_Jump_ValueChanged;
                panel_snatch_jump.Controls.AddRange([nmud1, nmud2]);

                if (bool_Add_Blank)
                {
                    Button btn4 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(90, 25),
                        Text = str_buttontext_commit,
                        UseVisualStyleBackColor = true
                    };
                    btn4.Click += button_snatch_jump_commit_click;
                    panel_snatch_jump.Controls.Add(btn4);

                    nmud1.Select();
                }
                else
                {
                    if (intFromWeight > 1)
                    {
                        Button btn1 = new()
                        {
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                            Location = new Point(200, intY),
                            Size = new Size(36, 25),
                            Text = str_buttontext_delete,
                            UseVisualStyleBackColor = true,
                            Tag = intFromWeight
                        };
                        btn1.Click += button_snatch_jump_delete_click;
                        panel_snatch_jump.Controls.Add(btn1);
                    }
                }
            }

            foreach (KeyValuePair<int, int> _jump in snatchJumps.OrderBy(r => r.Key))
            {
                intFromWeight = _jump.Key;
                intJump = _jump.Value;
                snatch_Add_Jump_IndividualControls(
                    intY,
                    intFromWeight,
                    intJump,
                    bool_Add_Blank: false);
                intY += 30;
            }
            snatch_Add_Jump_IndividualControls(
                intY,
                intFromWeight + 1,
                intJump,
                bool_Add_Blank: true);
        }
        private void button_snatch_jump_delete_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            if (snatchJumps.TryGetValue(_id, out _))
            {
                snatchJumps.Remove(_id);
            }

            snatch_Populate_Jumps();
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void button_snatch_jump_commit_click(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int intFromWeight = -1;
            int intJump = -1;

            foreach (Control ctrl in panel_snatch_jump.Controls)
            {
                if (intFromWeight > -1 & intJump > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(_numericUpDown.Tag) == -1)
                    {
                        if (_numericUpDown.Left < 10)
                        {
                            intFromWeight = (int)_numericUpDown.Value;
                        }
                        else
                        {
                            intJump = (int)(_numericUpDown.Value);
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intJump > 0)
            {
                if (snatchJumps.TryGetValue(intFromWeight, out _))
                {
                    MessageBox.Show("From Weight - Jump already exists");
                    return;
                }
                else
                {
                    snatchJumps[intFromWeight] = intJump;
                    snatch_Populate_Jumps();
                    snatch_Populate_Steps(boolPreserveLifts: false);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                snatch_Populate_Jumps();
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void numericUpDown_snatch_jump_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intFromWeight = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            if (_id == intFromWeight)
            {
                return;
            }
            if (snatchJumps.TryGetValue(_id, out int _step))
            {
                snatchJumps.Remove(_id);
                snatchJumps[intFromWeight] = _step;
            }
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void numericUpDown_snatch_jump_Jump_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intJump = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            snatchJumps[_id] = intJump;
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        #endregion

        #region snatch times
        private void snatch_Populate_Times()
        {
            snatch_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intTime = 1;
            panel_snatch_time.Controls.Clear();

            if (snatchTimes.Count == 0)
            {
                snatchTimes = Defaults.default_snatchTimes();
            }

            void snatch_Add_Time_IndividualControls(
                int intY,
                int intFromWeight,
                int intTime,
                bool bool_Add_Blank)
            {
                NumericUpDown nmud1 = new()
                {
                    Location = new Point(6, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intFromWeight, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                NumericUpDown nmud2 = new()
                {
                    Location = new Point(100, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intTime, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                nmud1.ValueChanged += numericUpDown_snatch_time_FromWeight_ValueChanged;
                nmud2.ValueChanged += numericUpDown_snatch_time_Time_ValueChanged;
                panel_snatch_time.Controls.AddRange([nmud1, nmud2]);

                if (bool_Add_Blank)
                {
                    Button btn4 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(90, 25),
                        Text = str_buttontext_commit,
                        UseVisualStyleBackColor = true
                    };
                    btn4.Click += button_snatch_time_commit_click;
                    panel_snatch_time.Controls.Add(btn4);

                    nmud1.Select();
                }
                else
                {
                    if (intFromWeight > 1)
                    {
                        Button btn1 = new()
                        {
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                            Location = new Point(200, intY),
                            Size = new Size(36, 25),
                            Text = str_buttontext_delete,
                            UseVisualStyleBackColor = true,
                            Tag = intFromWeight
                        };
                        btn1.Click += button_snatch_time_delete_click;
                        panel_snatch_time.Controls.Add(btn1);
                    }
                }
            }

            foreach (KeyValuePair<int, int> _time in snatchTimes.OrderBy(r => r.Key))
            {
                intFromWeight = _time.Key;
                intTime = _time.Value;
                snatch_Add_Time_IndividualControls(
                    intY,
                    intFromWeight,
                    intTime,
                    bool_Add_Blank: false);
                intY += 30;
            }
            snatch_Add_Time_IndividualControls(
                intY,
                intFromWeight + 1,
                intTime,
                bool_Add_Blank: true);
        }
        private void button_snatch_time_delete_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            if (snatchTimes.TryGetValue(_id, out _))
            {
                snatchTimes.Remove(_id);
            }

            snatch_Populate_Times();
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_snatch_time_commit_click(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int intFromWeight = -1;
            int intTime = -1;

            foreach (Control ctrl in panel_snatch_time.Controls)
            {
                if (intFromWeight > -1 & intTime > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(_numericUpDown.Tag) == -1)
                    {
                        if (_numericUpDown.Left < 10)
                        {
                            intFromWeight = (int)_numericUpDown.Value;
                        }
                        else
                        {
                            intTime = (int)(_numericUpDown.Value);
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intTime > 0)
            {
                if (snatchTimes.TryGetValue(intFromWeight, out _))
                {
                    MessageBox.Show("From Weight - Time already exists");
                    return;
                }
                else
                {
                    snatchTimes[intFromWeight] = intTime;
                    snatch_Populate_Times();
                    snatch_Populate_Steps(boolPreserveLifts: true);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                snatch_Populate_Times();
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void numericUpDown_snatch_time_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intFromWeight = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            if (_id == intFromWeight)
            {
                return;
            }
            if (snatchTimes.TryGetValue(_id, out int _step))
            {
                snatchTimes.Remove(_id);
                snatchTimes[intFromWeight] = _step;
            }
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_snatch_time_Time_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intTime = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            snatchTimes[_id] = intTime;
            snatch_Populate_Steps(boolPreserveLifts: true);
        }
        #endregion

        #region snatch Steps
        private void snatch_Populate_Steps(bool boolPreserveLifts)
        {
            snatch_Stop_Live();
            int intY = 1;
            bool boolHasOverrides = false;
            panel_snatch_steps.Controls.Clear();

            snatchStepsPLAN = snatchSteps(
                _bool_PreserveLifts: boolPreserveLifts,
                _stepsIn: snatchStepsPLAN);

            if (snatchStepsPLAN == null) { return; }

            void snatch_Add_Step_IndividualControls(int intY, Step _step)
            {
                Label lbl1 = new()
                {
                    Location = new Point(6, intY),
                    AutoSize = false,
                    Size = new Size(150, 28),
                    Text = _step.Action,
                    Tag = _step.Weight
                };
                Label lbl3 = new()
                {
                    Location = new Point(226, intY),
                    AutoSize = false,
                    Size = new Size(90, 28),
                    Text = Seconds_To_String(_step.Length),
                    Tag = _step.Weight
                };
                Label lbl4 = new()
                {
                    Location = new Point(317, intY),
                    AutoSize = false,
                    Size = new Size(90, 28),
                    Text = Seconds_To_String(_step.TotalLength),
                    Tag = _step.Weight
                };
                panel_snatch_steps.Controls.AddRange([lbl1, lbl3, lbl4]);

                if (_step.Weight > 0)
                {
                    lbl1.Click += snatch_Weight_Override_Click;
                    lbl3.Click += snatch_Weight_Override_Click;
                    lbl4.Click += snatch_Weight_Override_Click;
                    Label lbl2 = new()
                    {
                        Location = new Point(152, intY),
                        AutoSize = false,
                        Size = new Size(50, 28),
                        Text = _step.Weight.ToString(),
                        Tag = _step.Weight
                    };
                    lbl2.Click += snatch_Weight_Override_Click;
                    if (_step.Override)
                    {
                        Font fontx = new("Gadugi", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
                        lbl1.Font = fontx;
                        lbl2.Font = fontx;
                        lbl3.Font = fontx;
                        lbl4.Font = fontx;
                    }
                    panel_snatch_steps.Controls.Add(lbl2);
                }
            }

            foreach (Step _step in snatchStepsPLAN.OrderBy(r => r.Order))
            {
                if (!boolHasOverrides)
                {
                    if (_step.Override)
                    {
                        boolHasOverrides = true;
                    }
                }
                if (!_step.PreStep)
                {
                    snatch_Add_Step_IndividualControls(intY: intY, _step: _step);
                    intY += 30;
                }
            }
            Button btn = new()
            {
                Location = new Point(6, intY),
                Size = new Size(50, 28),
                Text = "+"
            };
            btn.Click += snatch_Step_Add;
            panel_snatch_steps.Controls.Add(btn);
            if (boolHasOverrides)
            {
                Button btn2 = new()
                {
                    Location = new Point(70, intY),
                    Size = new Size(100, 28),
                    Text = "reset overrides"
                };
                btn2.Click += snatch_Step_ResetOverrides;
                panel_snatch_steps.Controls.Add(btn2);
            }
            label_snatch_Setup_StepCount.Text = (snatchStepsPLAN.Count - 1).ToString() + " steps";
        }
        private List<Step> snatchSteps(
            bool _bool_PreserveLifts,
            List<Step> _stepsIn = null
            )
        {
            return x_Steps(
                _bool_PreserveLifts: _bool_PreserveLifts,
                _extras: snatchExtras,
                _jumps: snatchJumps,
                _times: snatchTimes,
                _int_x_Sec_End: int_snatch_Sec_End,
                _int_x_Wgt_Opener: int_snatch_Wgt_Opener,
                _bool_Opener_in_Warmup: bool_snatch_OpenerWarmup,
                _stepsIn: _stepsIn);
        }
        private void snatch_Step_Add(object sender, EventArgs e)
        {
            if (bool_snatch_Live)
            {
                snatch_Stop_Live();
            }

            if (snatchStepsPLAN != null && snatchStepsPLAN.Count > 2 &&
                snatchStepsPLAN[snatchStepsPLAN.Count - 1].Weight > 0)
            {
                int _int_RowIndex = snatchStepsPLAN.Count - 1;
                string _str_NewWeight = snatchStepsPLAN[_int_RowIndex].Weight.ToString();
                ShowInputDialog(ref _str_NewWeight);
                if (int.TryParse(_str_NewWeight, out int _int_NewWeight))
                {
                    if (snatchStepsPLAN.Any(r => r.Weight == _int_NewWeight))
                    {
                        MessageBox.Show(_int_NewWeight.ToString() + " is already a step");
                    }
                    else
                    {
                        snatchStepsPLAN.Add(new(
                            action: "Lift",
                            weight: _int_NewWeight,
                            @override: true));
                        snatch_Populate_Steps(boolPreserveLifts: true);
                    }
                }
            }
        }
        private void snatch_Step_ResetOverrides(object sender, EventArgs e)
        {
            if (bool_snatch_Live)
            {
                snatch_Stop_Live();
            }
            snatch_Populate_Steps(boolPreserveLifts: false);
        }
        private void snatch_Weight_Override_Click(object sender, EventArgs e)
        {
            int _int_StartWeight = 0;
            Label _labelI = (Label)sender;
            try
            {
                _int_StartWeight = (int)_labelI.Tag;
            }
            catch { }

            if (_int_StartWeight > 0 & snatchStepsPLAN != null)
            {
                Step _step = snatchStepsPLAN.FirstOrDefault(r => r.Weight.HasValue && r.Weight.Value == _int_StartWeight);
                if (_step != null)
                {
                    if (bool_snatch_Live)
                    {
                        snatch_Stop_Live();
                    }
                    string strNewWeight = _int_StartWeight.ToString();
                    if (ShowInputDialog(ref strNewWeight) == DialogResult.OK)
                    {
                        if (int.TryParse(strNewWeight, out int _int_NewWeight))
                        {
                            if (_int_NewWeight != _int_StartWeight)
                            {
                                if (snatchStepsPLAN.Any(r => r.Weight == _int_NewWeight))
                                {
                                    MessageBox.Show(_int_NewWeight.ToString() + " is already a step");
                                }
                                else
                                {
                                    _step.Weight = _int_NewWeight;
                                    _step.Override = true;
                                    snatch_Populate_Steps(boolPreserveLifts: true);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region snatch LIVE
        private void snatch_Stop_Live()
        {
            bool_snatch_Live = false;
            if (timer_snatch_Live != null)
            {
                timer_snatch_Live.Enabled = false;
                try
                {
                    timer_snatch_Live.Tick -= timer_snatch_Live_Tick;
                }
                catch { }
                timer_snatch_Live = null;
            }
            textBox_snatch_Live_LiftsOut.Visible = false;
            label_snatch_Live_TimeTillStart.Text = String.Empty;
            label_snatch_Live_TimeTillOpener.Text = String.Empty;
            label_snatch_Live_LiftsPassed.Text = String.Empty;
            Clear_snatch_Live_Steps();
            progressBar_snatch_Live_StageLift.Value = 0;
            button_snatch_Live_StageAdvance.BackColor = color_snatch_Live_BG;
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
            if (timer_snatch_Live != null)
            {
                if (timer_snatch_Live.Enabled)
                {
                    timer_snatch_Live.Enabled = false;
                    try
                    {
                        timer_snatch_Live.Tick -= timer_snatch_Live_Tick;
                    }
                    catch { }
                }
            }
            datetime_snatch_Start = DateTime.Today.AddHours(dateTimePicker_snatch_Start.Value.Hour).AddMinutes(dateTimePicker_snatch_Start.Value.Minute);
            int_snatch_Lifts_Passed = 0;
            textBox_snatch_Live_LiftsOut.Visible = false;
            label_snatch_Live_LiftsPassed.Text = "0";
            Populate_snatch_Live_Steps();
            progressBar_snatch_Live_StageLift.Value = 0;
            progressBar_snatch_Live_StageLift.Maximum = int_snatch_Sec_Stage;
            label_snatch_Live_TimeTillStart.Text = String.Empty;
            label_snatch_Live_TimeTillOpener.Text = String.Empty;
            sim_timer_snatch_Live_Tick();
            timer_snatch_Live = new Timer { Interval = 1000 };
            timer_snatch_Live.Tick += timer_snatch_Live_Tick;
            timer_snatch_Live.Start();
            button_snatch_Live_StartStop.Text = "stop";
            button_snatch_Live_StageAdvance.Select();
            PreventMonitorPowerdown();
        }
        private void button_snatch_Live_StartStop_Click(object sender, EventArgs e)
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
                snatch_Populate_Steps(boolPreserveLifts: false);
            }
            if (snatchStepsPLAN is null)
            {
                MessageBox.Show("An error has occurred. Step plan could not be determined.");
                this.Close();
                return;
            }
            snatchStepsLIVE = [.. snatchStepsPLAN.Select(r => r.Clone())];

            int intY = 1;
            int _int_panel_Live_Step_Width = panel_snatch_Live_Steps.Width - 4;
            int _int_progressBar_Step_Width_NoScroll = _int_panel_Live_Step_Width - 350;
            int _int_progressBar_Step_Width_Scroll = _int_progressBar_Step_Width_NoScroll - SystemInformation.VerticalScrollBarWidth;
            int _int_progressBar_Step_Width;
            Point _point_progressBar_Step_Location = new(300, 6);

            SuspendLayout();
            foreach (Step _step in snatchStepsLIVE)
            {
                if (panel_snatch_Live_Steps.VerticalScroll.Visible)
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_Scroll;
                }
                else
                {
                    _int_progressBar_Step_Width = _int_progressBar_Step_Width_NoScroll;
                }
                bool _isLift = (_step.Weight > 0);
                string strActionText = ActionTextString(_step: _step, _isFuture: true);
                Panel panel_Live_Step = new()
                {
                    Size = new Size(_int_panel_Live_Step_Width, 80),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    BackColor = color_snatch_Live_BG,
                    ForeColor = color_Live_Default_FG,
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
                if (_isLift)
                {
                    label_Weight = new Label
                    {
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.Black,
                        AutoSize = false,
                        Size = new Size(105, 30),
                        Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width - 105, 6),
                        ForeColor = SystemColors.Window,
                        Text = "lift " + _step.Weight.ToString(),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    };
                    label_Weight.Text = "lift " + _step.Weight.ToString();

                    if (_step.Override)
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Italic);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold | FontStyle.Italic);
                    }
                    else
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Regular);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold);
                    }
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
                if (_isLift)
                {
                    weightBoxGraphic = new()
                    {
                        boolOpener = false,
                        intWeightBar = int_Barbell,
                        intWeight = (int)_step.Weight,
                        intShadowWidth = 1,
                        intPlateGap = -1,
                        Size = new Size(120, 65),
                        BackColor = Color.FromArgb(128, 128, 128),
                        BorderStyle = BorderStyle.Fixed3D,
                        Location = new Point(10, 6),
                        Visible = true,
                    };
                    weightBoxGraphic.Paint += Apply_Vector_Weight_Graphic;
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
        private void timer_snatch_Live_Tick(object sender, EventArgs e)
        {
            sim_timer_snatch_Live_Tick();
        }
        private void sim_timer_snatch_Live_Tick()
        {
            DateTime _dateTime_Now = DateTime.Now;
            int intSecondsToStart = (int)datetime_snatch_Start.Subtract(_dateTime_Now).TotalSeconds;
            int intSecondsToOpen = 0;

            if (propertyData_Battery is null)
            {
                System.Management.ObjectQuery query = new("Select * FROM Win32_Battery");
                ManagementObjectSearcher searcher = new(query);
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject mo in collection.Cast<ManagementObject>())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        if (property.Name == "EstimatedChargeRemaining")
                        {
                            propertyData_Battery = property;
                        }
                    }
                }
            }
            if (propertyData_Battery is not null)
            {
                System.UInt16 _uInt_Battery = 100;
                try
                {
                    _uInt_Battery = (System.UInt16)propertyData_Battery.Value;
                }
                catch { }
                if (_uInt_Battery > 0 && _uInt_Battery < 100)
                {
                    panel_Battery.Visible = true;
                    progressBar_Battery.Value = _uInt_Battery;
                    label_Battery_Percentage.Text = _uInt_Battery.ToString() + "%";
                }
                else
                {
                    panel_Battery.Visible = false;
                }
            }
            else
            {
                panel_Battery.Visible = false;
            }

            if (intSecondsToStart > 0)
            {
                if ((int)button_snatch_Live_StageAdvance.Tag == 1)
                {
                    button_snatch_Live_StageAdvance.Tag = 0;
                    button_snatch_Live_StageAdvance.BackColor = color_snatch_Live_BG;
                }
                label_snatch_Live_TimeTillStart.Text = Seconds_To_String(intSecondsToStart);
                intSecondsToOpen = intSecondsToStart + (int_snatch_Lifts_Out * int_snatch_Sec_Stage);
                bool_snatch_LiveLifting = false;
            }
            else
            {
                if (!bool_snatch_LiveLifting)
                {
                    if (int_snatch_Lifts_Out > 0)
                    {
                        bool_snatch_LiveLifting = true;
                        progressBar_snatch_Live_StageLift.Value = 0;
                    }
                    label_snatch_Live_TimeTillStart.Text = "passed";
                }

                if (int_snatch_Lifts_Out > 0)
                {
                    if ((int)button_snatch_Live_StageAdvance.Tag == 0)
                    {
                        button_snatch_Live_StageAdvance.Tag = 1;
                        button_snatch_Live_StageAdvance.BackColor = color_AdvanceButton_Active;
                    }
                    progressBar_snatch_Live_StageLift.PerformStep();
                    if (bool_snatch_AutoAdvance)
                    {
                        if (progressBar_snatch_Live_StageLift.Value == progressBar_snatch_Live_StageLift.Maximum)
                        {
                            snatch_Advance_StageLift();
                        }
                    }
                    intSecondsToOpen += (int_snatch_Lifts_Out - 1) * int_snatch_Sec_Stage +
                        progressBar_snatch_Live_StageLift.Maximum - progressBar_snatch_Live_StageLift.Value;
                }
                else
                {
                    if ((int)button_snatch_Live_StageAdvance.Tag == 1)
                    {
                        button_snatch_Live_StageAdvance.Tag = 0;
                        button_snatch_Live_StageAdvance.BackColor = color_snatch_Live_BG;
                    }
                }
            }


            DateTime _dateTime_Open = _dateTime_Now.AddSeconds(intSecondsToOpen);
            if (intSecondsToOpen > 0)
            {
                label_snatch_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
                label_snatch_Live_OpenTime.Text = _dateTime_Open.ToString("HH:mm:ss");
            }
            else
            {
                label_snatch_Live_TimeTillOpener.Text = "-";
                label_snatch_Live_OpenTime.Text = "passed";
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
                bool boolUpdBGsFGs = (_intStep != int_snatch_Warmup_Step);

                foreach (Step _step in snatchStepsLIVE)
                {
                    int _int_Order = _step.Order;
                    Label label_Time = _step.Controls.LabelTime;
                    if (_int_Order > _intStep)
                    {
                        label_Time.Visible = true;
                        label_Time.Text = _dateTime_Open.AddSeconds(-_step.TotalLengthReverse).ToString("HH:mm:ss");
                    }
                    else
                    {
                        label_Time.Visible = false;
                    }
                }

                foreach (Step _step in snatchStepsLIVE)
                {
                    int _int_Order = _step.Order;
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

                        if (boolUpdBGsFGs)
                        {
                            label_Action = _step.Controls.LabelAction;
                            label_Action.Text = ActionTextString(_step: _step, _isFuture: false);
                            label_Progress_Time.Visible = true;
                        }

                        if (boolUpdBGsFGs)
                        {
                            panel_Live_Step.BackColor = color_Live_Highlight_BG;
                            panel_Live_Step.ForeColor = color_Live_Highlight_FG;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (boolUpdBGsFGs)
                    {
                        panel_Live_Step = _step.Controls.PanelLiveStep;
                        panel_Live_Step.BackColor = color_snatch_Live_BG;
                        panel_Live_Step.ForeColor = color_Live_Default_FG;
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
                if (boolUpdBGsFGs & bool_Beep)
                {
                    Console.Beep(750, 600);
                }
            }
            else
            {
                snatch_Stop_Live();
            }

            label_snatch_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void menuStrip_Profile_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        private void progressBar_snatch_Live_StageLift_MouseClick(object sender, MouseEventArgs e)
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
        private void button_snatch_Live_LiftsDecr_Click(object sender, EventArgs e)
        {
            if (int_snatch_Lifts_Out > 0)
            {
                int_snatch_Lifts_Out--;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
                if (int_snatch_Lifts_Out == 0)
                {
                    if (bool_snatch_LiveLifting)
                    {
                        bool_snatch_LiveLifting = false;
                        progressBar_snatch_Live_StageLift.Value = 0;
                    }
                }
            }
        }
        private void button_snatch_Live_LiftsIncr_Click(object sender, EventArgs e)
        {
            if (int_snatch_Lifts_Out < 99)
            {
                int_snatch_Lifts_Out++;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            }
        }
        private void button_snatch_Live_StageAdvance_Click(object sender, EventArgs e)
        {
            if (int_snatch_Lifts_Out >= 0 & bool_snatch_LiveLifting)
            {
                snatch_Advance_StageLift();
            }
        }
        private void label_snatch_Live_LiftsOut_Click(object sender, EventArgs e)
        {
            bool_Loading = true;
            textBox_snatch_Live_LiftsOut.Location = label_snatch_Live_LiftsOut.Location;
            textBox_snatch_Live_LiftsOut.Size = label_snatch_Live_LiftsOut.Size;
            textBox_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            textBox_snatch_Live_LiftsOut.Visible = true;
            textBox_snatch_Live_LiftsOut.BringToFront();
            textBox_snatch_Live_LiftsOut.Select();
            bool_Loading = false;
        }
        private void textBox_snatch_Live_LiftsOut_TextChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_snatch_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_snatch_Lifts_Out) &&
                _int_snatch_Lifts_Out >= 0 &&
                _int_snatch_Lifts_Out < 100 &&
                int_snatch_Lifts_Out != _int_snatch_Lifts_Out)
            {
                int_snatch_Lifts_Out = _int_snatch_Lifts_Out;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            }
        }
        private void textBox_snatch_Live_LiftsOut_Leave(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_snatch_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_snatch_Lifts_Out) &&
                _int_snatch_Lifts_Out >= 0 &&
                _int_snatch_Lifts_Out < 100 &&
                int_snatch_Lifts_Out != _int_snatch_Lifts_Out)
            {
                int_snatch_Lifts_Out = _int_snatch_Lifts_Out;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
            }
            textBox_snatch_Live_LiftsOut.Visible = false;
        }
        private void snatch_Advance_StageLift()
        {
            if (int_snatch_Lifts_Out > 0)
            {
                int_snatch_Lifts_Out--;
                label_snatch_Live_LiftsOut.Text = int_snatch_Lifts_Out.ToString();
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
        private void dateTimePicker_snatch_Start_ValueChanged(object sender, EventArgs e)
        {
            datetime_snatch_Start = DateTime.Today.AddHours(dateTimePicker_snatch_Start.Value.Hour).AddMinutes(dateTimePicker_snatch_Start.Value.Minute);
            if (datetime_snatch_Start > DateTime.Now && int_snatch_Lifts_Passed != 0)
            {
                int_snatch_Lifts_Passed = 0;
                label_snatch_Live_LiftsPassed.Text = (bool_snatch_Live ? int_snatch_Lifts_Passed.ToString() : string.Empty);
            }
        }
        private void checkBox_snatch_Live_Auto_CheckedChanged(object sender, EventArgs e)
        {
            bool_snatch_AutoAdvance = checkBox_snatch_Live_Auto.Checked;
        }
        private void checkBox_Live_Beep_CheckedChanged(object sender, EventArgs e)
        {
            if (!bool_Loading)
            {
                bool_Loading = true;
                bool_Beep = ((CheckBox)sender).Checked;
                checkBox_snatch_Live_Beep.Checked = bool_Beep;
                checkBox_cj_Live_Beep.Checked = bool_Beep;
                bool_Loading = false;
            }
        }
        private void splitContainer_snatch_DoubleClick(object sender, EventArgs e)
        {
            splitContainer_snatch.SplitterDistance = 0;
        }
        #endregion

        #region cj_Setup_Controls
        private void numericUpDown_cj_time_stage_ValueChanged(object sender, EventArgs e)
        {
            int _int_cj_Sec_Stage;
            try
            {
                _int_cj_Sec_Stage = (int)(numericUpDown_cj_time_stage.Value);
            }
            catch
            {
                (numericUpDown_cj_time_stage).BackColor = Color.Yellow;
                return;
            }

            if (_int_cj_Sec_Stage < 1)
            {
                (numericUpDown_cj_time_stage).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_cj_time_stage).BackColor = Color.White;

            int_cj_Sec_Stage = _int_cj_Sec_Stage;

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_cj_weight_opener_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            CJ_Opener_Set();
        }
        private void checkBox_cj_Param_OpenerWarmup_CheckedChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            bool_cj_OpenerWarmup = checkBox_cj_Param_OpenerWarmup.Checked;
            CJ_Opener_Set();
        }
        private void CJ_Opener_Set()
        {
            int _int_cj_Wgt_Opener;
            try
            {
                _int_cj_Wgt_Opener = (int)(numericUpDown_cj_weight_opener.Value);
            }
            catch
            {
                (numericUpDown_cj_weight_opener).BackColor = Color.Yellow;
                return;
            }

            if (_int_cj_Wgt_Opener < 1)
            {
                (numericUpDown_cj_weight_opener).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_cj_weight_opener).BackColor = Color.White;

            int_cj_Wgt_Opener = _int_cj_Wgt_Opener;

            Apply_Opener_Graphic_Vector(_intWeightBar: int_Barbell, _intWeightOpener: int_cj_Wgt_Opener, _boolSnatch: false);
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void numericUpDown_cj_time_PostWarmup_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            int _int_cj_Sec_End;
            try
            {
                _int_cj_Sec_End = (int)(numericUpDown_cj_time_PostWarmup.Value);
            }
            catch
            {
                (numericUpDown_cj_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }

            if (_int_cj_Sec_End < 0)
            {
                (numericUpDown_cj_time_PostWarmup).BackColor = Color.Yellow;
                return;
            }
            (numericUpDown_cj_time_PostWarmup).BackColor = Color.White;

            int_cj_Sec_End = _int_cj_Sec_End;

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        #endregion

        #region cj extras
        private void cj_Populate_Extras()
        {
            cj_Stop_Live();
            int intY = 1;
            panel_cj_extra.Controls.Clear();

            void cj_Add_Extra_IndividualControls(
                int intY,
                int intId,
                string strTBText,
                int intLength,
                bool bool_Add_Blank)
            {
                TextBox tb = new()
                {
                    Text = strTBText,
                    Location = new Point(6, intY),
                    Size = new Size(157, 25),
                    Tag = intId,
                    BackColor = Color.White
                };
                NumericUpDown nmud = new()
                {
                    Location = new Point(169, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intLength, 0, 0, 0]),
                    Tag = intId,
                    BackColor = Color.White
                };
                Label lbl = new()
                {
                    Location = new Point(250, intY + 3),
                    Text = Seconds_To_String(intLength),
                    Tag = intId
                };
                tb.TextChanged += textBox_cj_extra_TextChanged;
                nmud.ValueChanged += numericUpDown_cj_extra_ValueChanged;
                panel_cj_extra.Controls.AddRange([tb, nmud, lbl]);

                if (bool_Add_Blank)
                {
                    Button btn4 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(258 + 100, intY),
                        Size = new Size(125, 25),
                        Text = str_buttontext_commit,
                        UseVisualStyleBackColor = true
                    };
                    btn4.Click += button_cj_extra_commit_click;
                    panel_cj_extra.Controls.Add(btn4);

                    tb.Select();
                }
                else
                {
                    Button btn1 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(258 + 100, intY),
                        Size = new Size(32, 25),
                        Text = str_buttontext_up,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    Button btn2 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(301 + 100, intY),
                        Size = new Size(32, 25),
                        Text = str_buttontext_down,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    Button btn3 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(347 + 100, intY),
                        Size = new Size(36, 25),
                        Text = str_buttontext_delete,
                        UseVisualStyleBackColor = true,
                        Tag = intId
                    };
                    btn1.Click += button_cj_extra_up_click;
                    btn2.Click += button_cj_extra_down_click;
                    btn3.Click += button_cj_extra_delete_click;
                    panel_cj_extra.Controls.AddRange([btn1, btn2, btn3]);
                }
            }

            foreach (Extra _extra in cjExtras.OrderBy(r => r.Order))
            {
                cj_Add_Extra_IndividualControls(
                    intY: intY,
                    intId: _extra.id,
                    strTBText: _extra.Action,
                    intLength: _extra.Length,
                    bool_Add_Blank: false);
                intY += 30;
            }
            cj_Add_Extra_IndividualControls(
                intY: intY,
                intId: -1,
                strTBText: string.Empty,
                intLength: 60,
                bool_Add_Blank: true);
        }
        private void button_cj_extra_up_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            Extra _extra = cjExtras.FirstOrDefault(r => r.id == _id);
            if (_extra.id < 1)
            {
                return;
            }
            int _order = _extra.Order;

            if (_order < 1)
            { return; }

            for (int i = 0; i < cjExtras.Count; i++)
            {
                _extra = cjExtras[i];
                if (_extra.id == _id && _extra.Order == _order)
                {
                    _extra.Order = _order - 1;
                }
                else if (_extra.Order == _order - 1)
                {
                    _extra.Order = _order;
                }
                cjExtras[i] = _extra;
            }

            cj_Populate_Extras();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_extra_down_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            Extra _extra = cjExtras.FirstOrDefault(r => r.id == _id);
            if (_extra.id < 1)
            {
                return;
            }
            int _order = _extra.Order;
            int _max = cjExtras_Max_Order();

            if (_order < 0 || _order == _max)
            {
                return;
            }

            for (int i = 0; i < cjExtras.Count; i++)
            {
                _extra = cjExtras[i];
                if (_extra.id == _id && _extra.Order == _order)
                {
                    _extra.Order = _order + 1;
                }
                else if (_extra.Order == _order + 1)
                {
                    _extra.Order = _order;
                }
                cjExtras[i] = _extra;
            }

            cj_Populate_Extras();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_extra_delete_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);

            cjExtras.RemoveAll(r => r.id == _id);

            cjExtras_Reassign_Order();
            cj_Populate_Extras();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_extra_commit_click(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            string strAction = string.Empty;
            int intLength = -1;

            foreach (Control ctrl in panel_cj_extra.Controls)
            {
                if (strAction != String.Empty & intLength > -1)
                { break; }
                if (ctrl.GetType() == typeof(TextBox))
                {
                    if ((int)(((TextBox)ctrl).Tag) == -1)
                    {
                        strAction = ((TextBox)ctrl).Text;
                        if (strAction == String.Empty)
                        {
                            MessageBox.Show("Action cannot be blank");
                            return;
                        }
                    }
                }
                else if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(_numericUpDown.Tag) == -1)
                    {
                        intLength = (int)_numericUpDown.Value;
                    }
                }
            }
            if (strAction != String.Empty & intLength > -1)
            {
                if (intLength < 1)
                {
                    MessageBox.Show("Length cannot be < 1");
                    return;
                }
                Extra _extra = new(action: strAction, length: intLength, order: cjExtras_Max_Order() + 1);
                cjExtras.Add(_extra);
                cjExtras_Reassign_Order();
                cj_Populate_Extras();
                cj_Populate_Steps(boolPreserveLifts: true);
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                cj_Populate_Extras();
                cj_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void textBox_cj_extra_TextChanged(object sender, EventArgs e)
        {
            int _id = (int)(((TextBox)(sender)).Tag);

            if (_id < 1) { return; }

            string strAction = ((TextBox)sender).Text;

            if (strAction == String.Empty)
            {
                ((TextBox)sender).BackColor = Color.Yellow;
                return;
            }
            else
            {
                ((TextBox)sender).BackColor = Color.White;
            }

            for (int i = 0; i < cjExtras.Count; i++)
            {
                Extra _extra = cjExtras[i];
                if (_extra.id == _id)
                {
                    _extra.Action = strAction;
                    cjExtras[i] = _extra;
                    return;
                }
            }

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_cj_extra_ValueChanged(object sender, EventArgs e)
        {
            int _id = (int)(((NumericUpDown)(sender)).Tag);

            if (_id < 1) { return; }

            int intLength;
            try
            {
                intLength = (int)(((NumericUpDown)sender).Value);
            }
            catch
            {
                ((NumericUpDown)sender).BackColor = Color.Yellow;
                return;
            }

            if (intLength < 1) { return; }
            ((NumericUpDown)sender).BackColor = Color.White;

            for (int i = 0; i < cjExtras.Count; i++)
            {
                Extra _extra = cjExtras[i];
                if (_extra.id == _id)
                {
                    _extra.Length = intLength;
                    cjExtras[i] = _extra;
                    break;
                }
            }

            foreach (Control control in panel_cj_extra.Controls)
            {
                if (control.GetType() == typeof(Label))
                {
                    if ((int)((Label)control).Tag == _id)
                    {
                        ((Label)control).Text = Seconds_To_String(intLength);
                        break;
                    }
                }
            }

            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private int cjExtras_Max_Order()
        {
            return cjExtras.Max(r => r.Order);
        }
        private void cjExtras_Reassign_Order()
        {
            cjExtras = [.. cjExtras.OrderBy(r => r.Order)];
            int _order = 0;
            for (int i = 0; i < cjExtras.Count; i++)
            {
                Extra _extra = cjExtras[i];
                _extra.Order = _order; _order++;
                cjExtras[i] = _extra;
            }
        }
        #endregion

        #region cj jumps
        private void cj_Populate_Jumps()
        {
            cj_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intJump = 1;
            panel_cj_jump.Controls.Clear();

            if (cjJumps.Count == 0)
            {
                cjJumps = Defaults.default_cjJumps();
            }

            void cj_Add_Jump_IndividualControls(
                int intY,
     int intFromWeight,
     int intJump,
     bool bool_Add_Blank
                )
            {
                NumericUpDown nmud1 = new()
                {
                    Location = new Point(6, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intFromWeight, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                NumericUpDown nmud2 = new()
                {
                    Location = new Point(100, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intJump, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                nmud1.ValueChanged += numericUpDown_cj_jump_FromWeight_ValueChanged;
                nmud2.ValueChanged += numericUpDown_cj_jump_Jump_ValueChanged;
                panel_cj_jump.Controls.AddRange([nmud1, nmud2]);

                if (bool_Add_Blank)
                {
                    Button btn4 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(90, 25),
                        Text = str_buttontext_commit,
                        UseVisualStyleBackColor = true
                    };
                    btn4.Click += button_cj_jump_commit_click;
                    panel_cj_jump.Controls.Add(btn4);

                    nmud1.Select();
                }
                else
                {
                    if (intFromWeight > 1)
                    {
                        Button btn1 = new()
                        {
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                            Location = new Point(200, intY),
                            Size = new Size(36, 25),
                            Text = str_buttontext_delete,
                            UseVisualStyleBackColor = true,
                            Tag = intFromWeight
                        };
                        btn1.Click += button_cj_jump_delete_click;
                        panel_cj_jump.Controls.Add(btn1);
                    }
                }
            }

            foreach (KeyValuePair<int, int> _jump in cjJumps.OrderBy(r => r.Key))
            {
                intFromWeight = _jump.Key;
                intJump = _jump.Value;
                cj_Add_Jump_IndividualControls(
                    intY,
                    intFromWeight,
                    intJump,
                    bool_Add_Blank: false);
                intY += 30;
            }
            cj_Add_Jump_IndividualControls(
                intY,
                intFromWeight + 1,
                intJump,
                bool_Add_Blank: true);
        }
        private void button_cj_jump_delete_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            if (snatchJumps.TryGetValue(_id, out _))
            {
                snatchJumps.Remove(_id);
            }

            cj_Populate_Jumps();
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void button_cj_jump_commit_click(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int intFromWeight = -1;
            int intJump = -1;

            foreach (Control ctrl in panel_cj_jump.Controls)
            {
                if (intFromWeight > -1 & intJump > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(_numericUpDown.Tag) == -1)
                    {
                        if (_numericUpDown.Left < 10)
                        {
                            intFromWeight = (int)_numericUpDown.Value;
                        }
                        else
                        {
                            try
                            {
                                intJump = (int)(_numericUpDown.Value);
                                if (intJump < 1)
                                {
                                    MessageBox.Show("Jump Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse Jump Weight");
                                return;
                            }
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intJump > 0)
            {
                if (cjJumps.TryGetValue(intFromWeight, out _))
                {
                    MessageBox.Show("From Weight - Jump already exists");
                    return;
                }
                else
                {
                    cjJumps[intFromWeight] = intJump;
                    cj_Populate_Jumps();
                    cj_Populate_Steps(boolPreserveLifts: false);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                cj_Populate_Jumps();
                cj_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void numericUpDown_cj_jump_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intFromWeight = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            if (_id == intFromWeight)
            {
                return;
            }
            if (cjJumps.TryGetValue(_id, out int _step))
            {
                cjJumps.Remove(_id);
                cjJumps[intFromWeight] = _step;
            }
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void numericUpDown_cj_jump_Jump_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intJump = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            cjJumps[_id] = intJump;
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        #endregion

        #region cj times
        private void cj_Populate_Times()
        {
            cj_Stop_Live();
            int intY = 1;
            int intFromWeight = 0, intTime = 1;
            panel_cj_time.Controls.Clear();

            if (cjTimes.Count == 0)
            {
                cjTimes = Defaults.default_cjTimes();
            }

            void cj_Add_Time_IndividualControls(
                int intY,
                int intFromWeight,
                int intTime,
                bool bool_Add_Blank)
            {
                NumericUpDown nmud1 = new()
                {
                    Location = new Point(6, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intFromWeight, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                NumericUpDown nmud2 = new()
                {
                    Location = new Point(100, intY),
                    Maximum = new decimal([9999, 0, 0, 0]),
                    Minimum = new decimal([1, 0, 0, 0]),
                    Size = new Size(72, 25),
                    TextAlign = HorizontalAlignment.Center,
                    Value = new decimal([intTime, 0, 0, 0]),
                    Tag = intFromWeight,
                    BackColor = Color.White
                };
                nmud1.ValueChanged += numericUpDown_cj_time_FromWeight_ValueChanged;
                nmud2.ValueChanged += numericUpDown_cj_time_Time_ValueChanged;
                panel_cj_time.Controls.AddRange([nmud1, nmud2]);

                if (bool_Add_Blank)
                {
                    Button btn4 = new()
                    {
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Gadugi", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        Location = new Point(200, intY),
                        Size = new Size(90, 25),
                        Text = str_buttontext_commit,
                        UseVisualStyleBackColor = true
                    };
                    btn4.Click += button_cj_time_commit_click;
                    panel_cj_time.Controls.Add(btn4);

                    nmud1.Select();
                }
                else
                {
                    if (intFromWeight > 1)
                    {
                        Button btn1 = new()
                        {
                            FlatStyle = FlatStyle.Flat,
                            Font = new Font("Gadugi", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                            Location = new Point(200, intY),
                            Size = new Size(36, 25),
                            Text = str_buttontext_delete,
                            UseVisualStyleBackColor = true,
                            Tag = intFromWeight
                        };
                        btn1.Click += button_cj_time_delete_click;
                        panel_cj_time.Controls.Add(btn1);
                    }
                }
            }

            foreach (KeyValuePair<int, int> _time in cjTimes.OrderBy(r => r.Key))
            {
                intFromWeight = _time.Key;
                intTime = _time.Value;
                cj_Add_Time_IndividualControls(
                    intY,
                    intFromWeight,
                    intTime,
                    bool_Add_Blank: false);
                intY += 30;
            }
            cj_Add_Time_IndividualControls(
                intY,
                intFromWeight + 1,
                intTime,
                bool_Add_Blank: true);
        }
        private void button_cj_time_delete_click(object sender, EventArgs e)
        {
            int _id = (int)(((Button)(sender)).Tag);
            if (cjTimes.TryGetValue(_id, out _))
            {
                cjTimes.Remove(_id);
            }

            cj_Populate_Times();
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void button_cj_time_commit_click(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int intFromWeight = -1;
            int intTime = -1;

            foreach (Control ctrl in panel_cj_time.Controls)
            {
                if (intFromWeight > -1 & intTime > -1)
                { break; }
                if (ctrl.GetType() == typeof(NumericUpDown))
                {
                    if ((int)(_numericUpDown.Tag) == -1)
                    {
                        if (_numericUpDown.Left < 10)
                        {
                            intFromWeight = (int)_numericUpDown.Value;
                        }
                        else
                        {
                            try
                            {
                                intTime = (int)(_numericUpDown.Value);
                                if (intTime < 1)
                                {
                                    MessageBox.Show("Time Weight cannot be < 1");
                                    return;
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Failed to parse Time Weight");
                                return;
                            }
                        }
                    }
                }
            }
            if (intFromWeight > 0 & intTime > 0)
            {
                if (cjTimes.TryGetValue(intFromWeight, out _))
                {
                    MessageBox.Show("From Weight - Time already exists");
                    return;
                }
                else
                {
                    cjTimes[intFromWeight] = intTime;
                    cj_Populate_Times();
                    cj_Populate_Steps(boolPreserveLifts: true);
                }
            }
            else
            {
                MessageBox.Show("Failed to find some data");
                cj_Populate_Times();
                cj_Populate_Steps(boolPreserveLifts: false);
            }
        }
        private void numericUpDown_cj_time_FromWeight_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intFromWeight = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            if (_id == intFromWeight)
            {
                return;
            }
            if (cjTimes.TryGetValue(_id, out int _step))
            {
                cjTimes.Remove(_id);
                cjTimes[intFromWeight] = _step;
            }
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        private void numericUpDown_cj_time_Time_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown _numericUpDown = sender as NumericUpDown;
            int _id = (int)_numericUpDown.Tag;
            if (_id < 1) { return; }
            int intTime = (int)_numericUpDown.Value;
            _numericUpDown.BackColor = Color.White;
            cjTimes[_id] = intTime;
            cj_Populate_Steps(boolPreserveLifts: true);
        }
        #endregion

        #region cj Steps
        private void cj_Populate_Steps(bool boolPreserveLifts)
        {
            cj_Stop_Live();
            int intY = 1;
            bool boolHasOverrides = false;
            panel_cj_steps.Controls.Clear();

            cjStepsPLAN = cjSteps(
                _bool_PreserveLifts: boolPreserveLifts,
                _stepsIn: cjStepsPLAN);

            if (cjStepsPLAN == null) { return; }

            void cj_Add_Step_IndividualControls(int intY, Step _step)
            {
                Label lbl1 = new()
                {
                    Location = new Point(6, intY),
                    AutoSize = false,
                    Size = new Size(150, 28),
                    Text = _step.Action,
                    Tag = _step.Weight
                };
                Label lbl3 = new()
                {
                    Location = new Point(226, intY),
                    AutoSize = false,
                    Size = new Size(90, 28),
                    Text = Seconds_To_String(_step.Length),
                    Tag = _step.Weight
                };
                Label lbl4 = new()
                {
                    Location = new Point(317, intY),
                    AutoSize = false,
                    Size = new Size(90, 28),
                    Text = Seconds_To_String(_step.TotalLength),
                    Tag = _step.Weight
                };
                panel_cj_steps.Controls.AddRange([lbl1, lbl3, lbl4]);

                if (_step.Weight > 0)
                {
                    lbl1.Click += cj_Weight_Override_Click;
                    lbl3.Click += cj_Weight_Override_Click;
                    lbl4.Click += cj_Weight_Override_Click;
                    Label lbl2 = new()
                    {
                        Location = new Point(152, intY),
                        AutoSize = false,
                        Size = new Size(50, 28),
                        Text = _step.Weight.ToString(),
                        Tag = _step.Weight
                    };
                    lbl2.Click += cj_Weight_Override_Click;
                    if (_step.Override)
                    {
                        Font fontx = new("Gadugi", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
                        lbl1.Font = fontx;
                        lbl2.Font = fontx;
                        lbl3.Font = fontx;
                        lbl4.Font = fontx;
                    }
                    panel_cj_steps.Controls.Add(lbl2);
                }
            }

            foreach (Step _step in cjStepsPLAN.OrderBy(r => r.Order))
            {
                if (!boolHasOverrides)
                {
                    if (_step.Override)
                    {
                        boolHasOverrides = true;
                    }
                }
                if (!_step.PreStep)
                {
                    cj_Add_Step_IndividualControls(intY: intY, _step: _step);
                    intY += 30;
                }
            }
            Button btn = new()
            {
                Location = new Point(6, intY),
                Size = new Size(50, 28),
                Text = "+"
            };
            btn.Click += cj_Step_Add;
            panel_cj_steps.Controls.Add(btn);
            if (boolHasOverrides)
            {
                Button btn2 = new()
                {
                    Location = new Point(70, intY),
                    Size = new Size(100, 28),
                    Text = "reset overrides"
                };
                btn2.Click += cj_Step_ResetOverrides;
                panel_cj_steps.Controls.Add(btn2);
            }
            label_cj_Setup_StepCount.Text = (cjStepsPLAN.Count - 1).ToString() + " steps";
        }
        private List<Step> cjSteps(
            bool _bool_PreserveLifts,
            List<Step> _stepsIn = null
            )
        {
            return x_Steps(
                _bool_PreserveLifts: _bool_PreserveLifts,
                _extras: cjExtras,
                _jumps: cjJumps,
                _times: cjTimes,
                _int_x_Sec_End: int_cj_Sec_End,
                _int_x_Wgt_Opener: int_cj_Wgt_Opener,
                _bool_Opener_in_Warmup: bool_cj_OpenerWarmup,
                _stepsIn: _stepsIn);
        }
        private void cj_Add_Step_IndividualControls(int intY, Step _step)
        {
            Label lbl1 = new()
            {
                Location = new Point(6, intY),
                AutoSize = false,
                Size = new Size(150, 28),
                Text = _step.Action,
                Tag = _step.Weight
            };
            Label lbl3 = new()
            {
                Location = new Point(226, intY),
                AutoSize = false,
                Size = new Size(90, 28),
                Text = Seconds_To_String(_step.Length),
                Tag = _step.Weight
            };
            Label lbl4 = new()
            {
                Location = new Point(317, intY),
                AutoSize = false,
                Size = new Size(90, 28),
                Text = Seconds_To_String(_step.TotalLength),
                Tag = _step.Weight
            };
            panel_cj_steps.Controls.AddRange([lbl1, lbl3, lbl4]);

            if (_step.Weight > 0)
            {
                lbl1.Click += cj_Weight_Override_Click;
                lbl3.Click += cj_Weight_Override_Click;
                lbl4.Click += cj_Weight_Override_Click;
                Label lbl2 = new()
                {
                    Location = new Point(152, intY),
                    AutoSize = false,
                    Size = new Size(50, 28),
                    Text = _step.Weight.ToString(),
                    Tag = _step.Weight
                };
                lbl2.Click += cj_Weight_Override_Click;
                if (_step.Override)
                {
                    Font fontx = new("Gadugi", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
                    lbl1.Font = fontx;
                    lbl2.Font = fontx;
                    lbl3.Font = fontx;
                    lbl4.Font = fontx;
                }
                panel_cj_steps.Controls.Add(lbl2);
            }
        }
        private void cj_Step_Add(object sender, EventArgs e)
        {
            if (bool_cj_Live)
            {
                cj_Stop_Live();
            }

            if (cjStepsPLAN != null && cjStepsPLAN.Count > 2 &&
                cjStepsPLAN[cjStepsPLAN.Count - 1].Weight > 0)
            {
                int _int_RowIndex = cjStepsPLAN.Count - 1;
                string _str_NewWeight = cjStepsPLAN[_int_RowIndex].Weight.ToString();
                ShowInputDialog(ref _str_NewWeight);
                if (int.TryParse(_str_NewWeight, out int _int_NewWeight))
                {
                    if (cjStepsPLAN.Any(r => r.Weight == _int_NewWeight))
                    {
                        MessageBox.Show(_int_NewWeight.ToString() + " is already a step");
                    }
                    else
                    {
                        cjStepsPLAN.Add(new(
                            action: "Lift",
                            weight: _int_NewWeight,
                            @override: true));
                        cj_Populate_Steps(boolPreserveLifts: true);
                    }
                }
            }
        }
        private void cj_Step_ResetOverrides(object sender, EventArgs e)
        {
            if (bool_cj_Live)
            {
                cj_Stop_Live();
            }
            cj_Populate_Steps(boolPreserveLifts: false);
        }
        private void cj_Weight_Override_Click(object sender, EventArgs e)
        {
            int _int_StartWeight = 0;
            Label _labelI = (Label)sender;
            try
            {
                _int_StartWeight = (int)_labelI.Tag;
            }
            catch { }

            if (_int_StartWeight > 0 & cjStepsPLAN != null)
            {
                Step _step = cjStepsPLAN.FirstOrDefault(r => r.Weight.HasValue && r.Weight.Value == _int_StartWeight);
                if (_step != null)
                {
                    if (bool_cj_Live)
                    {
                        cj_Stop_Live();
                    }
                    string strNewWeight = _int_StartWeight.ToString();
                    if (ShowInputDialog(ref strNewWeight) == DialogResult.OK)
                    {
                        if (int.TryParse(strNewWeight, out int _int_NewWeight))
                        {
                            if (_int_NewWeight != _int_StartWeight)
                            {
                                if (cjStepsPLAN.Any(r => r.Weight == _int_NewWeight))
                                {
                                    MessageBox.Show(_int_NewWeight.ToString() + " is already a step");
                                }
                                else
                                {
                                    _step.Weight = _int_NewWeight;
                                    _step.Override = true;
                                    cj_Populate_Steps(boolPreserveLifts: true);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region cj LIVE
        private void cj_Stop_Live()
        {
            bool_cj_Live = false;
            if (timer_cj_Live != null)
            {
                timer_cj_Live.Enabled = false;
                try
                {
                    timer_cj_Live.Tick -= timer_cj_Live_Tick;
                }
                catch { }
                timer_cj_Live = null;
            }
            label_cj_Live_TimeTillOpener.Text = String.Empty;
            textBox_cj_Live_snLeft.Visible = false;
            textBox_cj_Live_LiftsOut.Visible = false;
            label_cj_Live_LiftsPassed.Text = string.Empty;
            Clear_cj_Live_Steps();
            progressBar_cj_Live_StageLift.Value = 0;
            progressBar_cj_Live_sn.Value = 0;
            progressBar_cj_Live_Break.Value = 0;
            label_cj_Live_Break.Text = String.Empty;
            button_cj_Live_StageAdvance.BackColor = color_cj_Live_BG;
            button_cj_Live_StageAdvance.Tag = 0;
            button_cj_Live_snStageAdvance.BackColor = color_cj_Live_BG;
            button_cj_Live_snStageAdvance.Tag = 0;
            bool_cj_LiveLifting = false;
            bool_cj_BreakRunning = false;
            bool_cj_sn_Lifting = false;
            button_cj_Live_StartStop.Text = "start";
            panel_cj_Live_Times.Visible = false;
            panel_Battery.Visible = false;
            if (!bool_snatch_Live) { AllowMonitorPowerdown(); }
        }
        private void cj_Start_Live()
        {
            bool_cj_Live = true;
            if (timer_cj_Live != null)
            {
                if (timer_cj_Live.Enabled)
                {
                    timer_cj_Live.Enabled = false;
                    try
                    {
                        timer_cj_Live.Tick -= timer_cj_Live_Tick;
                    }
                    catch { }
                }
            }
            textBox_cj_Live_snLeft.Visible = false;
            int_cj_Lifts_Passed = 0;
            textBox_cj_Live_LiftsOut.Visible = false;
            label_cj_Live_LiftsPassed.Text = "0";
            Populate_cj_Live_Steps();
            progressBar_cj_Live_StageLift.Value = 0;
            progressBar_cj_Live_StageLift.Maximum = int_cj_Sec_Stage;
            progressBar_cj_Live_sn.Value = 0;
            progressBar_cj_Live_sn.Maximum = int_snatch_Sec_Stage;
            progressBar_cj_Live_Break.Value = 0;
            progressBar_cj_Live_Break.Maximum = int_cj_Sec_Break;
            label_cj_Live_Break.Text = String.Empty;
            label_cj_Live_TimeTillOpener.Text = String.Empty;
            sim_timer_cj_Live_Tick();
            timer_cj_Live = new Timer { Interval = 1000 };
            timer_cj_Live.Tick += timer_cj_Live_Tick;
            timer_cj_Live.Start();
            button_cj_Live_StartStop.Text = "stop";
            button_cj_Live_StageAdvance.Select();
            PreventMonitorPowerdown();
        }
        private void button_cj_Live_StartStop_Click(object sender, EventArgs e)
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
                cj_Populate_Steps(boolPreserveLifts: false);
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
                    BackColor = color_cj_Live_BG,
                    ForeColor = color_Live_Default_FG,
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
                        Text = "lift " + _step.Weight.ToString(),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    };
                    label_Weight.Text = "lift " + _step.Weight.ToString();

                    if (_step.Override)
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Italic);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold | FontStyle.Italic);
                    }
                    else
                    {
                        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Regular);
                        label_Weight.Font = new Font("Gadugi", 18.0F, FontStyle.Bold);
                    }
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
                    weightBoxGraphic = new()
                    {
                        boolOpener = false,
                        intWeightBar = int_Barbell,
                        intWeight = _step.Weight ?? 0,
                        intShadowWidth = 1,
                        intPlateGap = -1,
                        Size = new Size(120, 65),
                        BackColor = Color.FromArgb(128, 128, 128),
                        BorderStyle = BorderStyle.Fixed3D,
                        Location = new Point(10, 6),
                        Visible = true
                    };
                    weightBoxGraphic.Paint += Apply_Vector_Weight_Graphic;
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
            label_cj_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
            panel_cj_Live_Times.Visible = true;
            ResumeLayout();
        }
        private void timer_cj_Live_Tick(object sender, EventArgs e)
        {
            sim_timer_cj_Live_Tick();
        }
        private void sim_timer_cj_Live_Tick()
        {
            DateTime _dateTime_Now = DateTime.Now;
            int intSecondsToOpen = 0;

            if (propertyData_Battery is null)
            {
                System.Management.ObjectQuery query = new("Select * FROM Win32_Battery");
                ManagementObjectSearcher searcher = new(query);
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject mo in collection.Cast<ManagementObject>())
                {
                    foreach (PropertyData property in mo.Properties)
                    {
                        if (property.Name == "EstimatedChargeRemaining")
                        {
                            propertyData_Battery = property;
                        }
                    }
                }
            }
            if (propertyData_Battery is not null)
            {
                System.UInt16 _uInt_Battery = 100;
                try
                {
                    _uInt_Battery = (System.UInt16)propertyData_Battery.Value;
                }
                catch { }
                if (_uInt_Battery > 0 && _uInt_Battery < 100)
                {
                    panel_Battery.Visible = true;
                    progressBar_Battery.Value = _uInt_Battery;
                    label_Battery_Percentage.Text = _uInt_Battery.ToString() + "%";
                }
                else
                {
                    panel_Battery.Visible = false;
                }
            }
            else
            {
                panel_Battery.Visible = false;
            }

            if (bool_cj_sn_Lifting) // snatches still running
            {
                if (int_cj_snLifts_Out == 0)
                {
                    bool_cj_sn_Lifting = false;
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

                    if (int_cj_snLifts_Out == 0)
                    {
                        bool_cj_sn_Lifting = false;
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
            else if (int_cj_snLifts_Out > 0)
            {
                bool_cj_sn_Lifting = true;
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
            else if (!bool_cj_sn_Lifting)
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
                    if (bool_cj_AutoAdvance | int_cj_Lifts_Out == 1)
                    {
                        cj_Advance_StageLift();
                    }
                }

                if (int_cj_Lifts_Out == 0)
                {
                    bool_cj_LiveLifting = false;
                }
            }
            else if (!bool_cj_sn_Lifting & !bool_cj_BreakRunning)
            {
                if (int_cj_Lifts_Out > 0)
                {
                    if (progressBar_cj_Live_StageLift.Value < progressBar_cj_Live_StageLift.Maximum)
                    {
                        bool_cj_LiveLifting = true;
                        progressBar_cj_Live_StageLift.Value = 0;
                    }
                }
            }


            if (bool_cj_sn_Lifting)
            {
                if ((int)button_cj_Live_snStageAdvance.Tag == 0)
                {
                    button_cj_Live_snStageAdvance.Tag = 1;
                    button_cj_Live_snStageAdvance.BackColor = color_AdvanceButton_Active;
                }
            }
            else
            {
                if ((int)button_cj_Live_snStageAdvance.Tag == 1)
                {
                    button_cj_Live_snStageAdvance.Tag = 0;
                    button_cj_Live_snStageAdvance.BackColor = color_cj_Live_BG;
                }
            }

            if (bool_cj_LiveLifting)
            {
                if ((int)button_cj_Live_StageAdvance.Tag == 0)
                {
                    button_cj_Live_StageAdvance.Tag = 1;
                    button_cj_Live_StageAdvance.BackColor = color_AdvanceButton_Active;
                }
            }
            else
            {
                if ((int)button_cj_Live_StageAdvance.Tag == 1)
                {
                    button_cj_Live_StageAdvance.Tag = 0;
                    button_cj_Live_StageAdvance.BackColor = color_cj_Live_BG;
                }
            }


            if (bool_cj_sn_Lifting | bool_cj_BreakRunning | bool_cj_LiveLifting)
            {
                intSecondsToOpen = (int_cj_Lifts_Out - 1) * int_cj_Sec_Stage +
                    progressBar_cj_Live_StageLift.Maximum - progressBar_cj_Live_StageLift.Value;
            }

            if (bool_cj_sn_Lifting | bool_cj_BreakRunning)
            {
                intSecondsToOpen += progressBar_cj_Live_Break.Maximum - progressBar_cj_Live_Break.Value;
            }

            if (bool_cj_sn_Lifting) // snatches still running
            {
                intSecondsToOpen += (int_cj_snLifts_Out - 1) * int_snatch_Sec_Stage +
                    progressBar_cj_Live_sn.Maximum - progressBar_cj_Live_sn.Value;
            }


            if (intSecondsToOpen == 0)
            {
                label_cj_Live_TimeTillOpener.Text = "passed";
            }
            else
            {
                label_cj_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
            }
            DateTime _dateTime_Open = _dateTime_Now.AddSeconds(intSecondsToOpen);
            if (intSecondsToOpen > 0)
            {
                label_cj_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
                label_cj_Live_OpenTime.Text = _dateTime_Open.ToString("HH:mm:ss");
            }
            else
            {
                label_cj_Live_TimeTillOpener.Text = "-";
                label_cj_Live_OpenTime.Text = "passed";
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
                bool boolUpdBGsFGs = (_intStep != int_cj_Warmup_Step);

                foreach (Step _step in cjStepsLIVE)
                {
                    int _int_Order = _step.Order;
                    Label label_Time = _step.Controls.LabelTime;
                    if (_int_Order > _intStep)
                    {
                        label_Time.Visible = true;
                        label_Time.Text = _dateTime_Open.AddSeconds(-_step.TotalLengthReverse).ToString("HH:mm:ss");
                    }
                    else
                    {
                        label_Time.Visible = false;
                    }
                }

                foreach (Step _step in cjStepsLIVE)
                {
                    int _int_Order = _step.Order;
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

                        if (boolUpdBGsFGs)
                        {
                            string strActionText = ActionTextString(_step: _step, _isFuture: false);
                            label_Action = _step.Controls.LabelAction;
                            label_Action.Text = strActionText;
                            label_Progress_Time.Visible = true;
                        }

                        if (boolUpdBGsFGs)
                        {
                            panel_Live_Step.BackColor = color_Live_Highlight_BG;
                            panel_Live_Step.ForeColor = color_Live_Highlight_FG;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if (boolUpdBGsFGs)
                    {
                        panel_Live_Step = _step.Controls.PanelLiveStep;
                        panel_Live_Step.BackColor = color_cj_Live_BG;
                        panel_Live_Step.ForeColor = color_Live_Default_FG;
                        label_Progress_Time = _step.Controls.LabelProgressTime;
                        label_Progress_Time.Visible = false;
                        progressBar_Step = _step.Controls.ProgressBarStep;
                        label_Action = _step.Controls.LabelAction;
                        string strActionText = ActionTextString(_step: _step, _isFuture: !(_int_Order < _intStep));
                        label_Action.Text = strActionText;
                    }
                }
                int_cj_Warmup_Step = _intStep;
                if (boolUpdBGsFGs & bool_Beep)
                {
                    Console.Beep(750, 600);
                }
            }
            else
            {
                cj_Stop_Live();
            }

            label_cj_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }
        private void progressBar_cj_Live_StageLift_MouseClick(object sender, MouseEventArgs e)
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
        private void progressBar_cj_Live_sn_MouseClick(object sender, MouseEventArgs e)
        {
            if (bool_cj_Live & bool_cj_sn_Lifting)
            {
                double dbl_Percent = e.X / (double)(progressBar_cj_Live_sn.Width);
                if (dbl_Percent >= 0 & dbl_Percent <= 1)
                {
                    progressBar_cj_Live_sn.Value = (int)(progressBar_cj_Live_sn.Maximum * dbl_Percent);
                }
            }
        }
        private void progressBar_cj_Live_Break_MouseClick(object sender, MouseEventArgs e)
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
        private void button_cj_Live_LiftsDecr_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out > 0)
            {
                int_cj_Lifts_Out--;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
                if (int_cj_Lifts_Out == 0)
                {
                    if (bool_cj_LiveLifting)
                    {
                        bool_cj_LiveLifting = false;
                        progressBar_cj_Live_StageLift.Value = 0;
                    }
                }
            }
        }
        private void button_cj_Live_LiftsIncr_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out < 99)
            {
                int_cj_Lifts_Out++;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            }
        }
        private void label_cj_Live_LiftsOut_Click(object sender, EventArgs e)
        {
            bool_Loading = true;
            textBox_cj_Live_LiftsOut.Location = label_cj_Live_LiftsOut.Location;
            textBox_cj_Live_LiftsOut.Size = label_cj_Live_LiftsOut.Size;
            textBox_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            textBox_cj_Live_LiftsOut.Visible = true;
            textBox_cj_Live_LiftsOut.BringToFront();
            textBox_cj_Live_LiftsOut.Select();
            bool_Loading = false;
        }
        private void textBox_cj_Live_LiftsOut_TextChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_Lifts_Out) &&
                _int_cj_Lifts_Out >= 0 &&
                _int_cj_Lifts_Out < 100 &&
                int_cj_Lifts_Out != _int_cj_Lifts_Out)
            {
                int_cj_Lifts_Out = _int_cj_Lifts_Out;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            }
        }
        private void textBox_cj_Live_LiftsOut_Leave(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_LiftsOut.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_Lifts_Out) &&
                _int_cj_Lifts_Out >= 0 &&
                _int_cj_Lifts_Out < 100 &&
                int_cj_Lifts_Out != _int_cj_Lifts_Out)
            {
                int_cj_Lifts_Out = _int_cj_Lifts_Out;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
            }
            textBox_cj_Live_LiftsOut.Visible = false;
        }
        private void button_cj_Live_StageAdvance_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out >= 0 & bool_cj_LiveLifting)
            {
                cj_Advance_StageLift();
            }
            else if (int_cj_snLifts_Out >= 0 & bool_cj_sn_Lifting)
            {
                cj_Advance_snLift();
            }
        }
        private void cj_Advance_StageLift()
        {
            if (int_cj_Lifts_Out > 0)
            {
                int_cj_Lifts_Out--;
                label_cj_Live_LiftsOut.Text = int_cj_Lifts_Out.ToString();
                if (bool_cj_Live && !bool_cj_sn_Lifting && !bool_cj_BreakRunning)
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
        private void checkBox_cj_Live_Auto_CheckedChanged(object sender, EventArgs e)
        {
            bool_cj_AutoAdvance = checkBox_cj_Live_Auto.Checked;
        }
        private void numericUpDown_cj_Live_Break_ValueChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            int _int_cj_Sec_Break = (int)(numericUpDown_cj_Live_Break.Value);
            if (_int_cj_Sec_Break < 1)
            {
                int_cj_Sec_Break = 1;
            }
            else
            {
                int_cj_Sec_Break = _int_cj_Sec_Break * 60;
            }

            if (progressBar_cj_Live_Break.Value > int_cj_Sec_Break)
            { progressBar_cj_Live_Break.Value = int_cj_Sec_Break; }
            progressBar_cj_Live_Break.Maximum = int_cj_Sec_Break;
        }
        private void button_cj_Live_snDecr_Click(object sender, EventArgs e)
        {
            if (int_cj_snLifts_Out > 0)
            {
                int_cj_snLifts_Out--;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
                if (int_cj_snLifts_Out == 0)
                {
                    if (bool_cj_sn_Lifting)
                    {
                        bool_cj_sn_Lifting = false;
                        progressBar_cj_Live_sn.Value = 0;
                    }
                }
            }
        }
        private void button_cj_Live_snIncr_Click(object sender, EventArgs e)
        {
            if (int_cj_snLifts_Out < 98)
            {
                int_cj_snLifts_Out++;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
        }
        private void label_cj_Live_snLeft_Click(object sender, EventArgs e)
        {
            bool_Loading = true;
            textBox_cj_Live_snLeft.Location = label_cj_Live_snLeft.Location;
            textBox_cj_Live_snLeft.Size = label_cj_Live_snLeft.Size;
            textBox_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            textBox_cj_Live_snLeft.Visible = true;
            textBox_cj_Live_snLeft.BringToFront();
            textBox_cj_Live_snLeft.Select();
            bool_Loading = false;
        }
        private void textBox_cj_Live_snLeft_TextChanged(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_snLeft.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_snLifts_Out) &&
                _int_cj_snLifts_Out >= 0 &&
                _int_cj_snLifts_Out < 100 &&
                int_cj_snLifts_Out != _int_cj_snLifts_Out)
            {
                int_cj_snLifts_Out = _int_cj_snLifts_Out;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
        }
        private void textBox_cj_Live_snLeft_Leave(object sender, EventArgs e)
        {
            if (bool_Loading) { return; }
            string _str_Input = textBox_cj_Live_snLeft.Text;
            if (int.TryParse(s: _str_Input, result: out int _int_cj_snLifts_Out) &&
                _int_cj_snLifts_Out >= 0 &&
                _int_cj_snLifts_Out < 100 &&
                int_cj_snLifts_Out != _int_cj_snLifts_Out)
            {
                int_cj_snLifts_Out = _int_cj_snLifts_Out;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
            textBox_cj_Live_snLeft.Visible = false;
        }
        private void button_cj_Live_snStageAdvance_Click(object sender, EventArgs e)
        {
            if (int_cj_Lifts_Out >= 0 & bool_cj_LiveLifting)
            {
                cj_Advance_StageLift();
            }
            else if (int_cj_snLifts_Out >= 0 & bool_cj_sn_Lifting)
            {
                cj_Advance_snLift();
            }
        }
        private void cj_Advance_snLift()
        {
            if (int_cj_snLifts_Out > 0)
            {
                int_cj_snLifts_Out--;
                label_cj_Live_snLeft.Text = int_cj_snLifts_Out.ToString();
            }
            progressBar_cj_Live_sn.Value = 0;
        }
        private void splitContainer_cj_DoubleClick(object sender, EventArgs e)
        {
            splitContainer_cj.SplitterDistance = 0;
        }
        #endregion

        #region utilities
        private string ActionTextString(Step _step, bool _isFuture)
        {
            return (_step.Weight.HasValue ? $"lift {_step.Weight}" : $"{_step.Action}") + (_isFuture ?
                $" ({Seconds_To_String(_step.Length)})" +
                $"{Environment.NewLine}from {Seconds_To_String(_step.TotalLengthReverse)} out" : string.Empty);
        }
        private string Seconds_To_String(int _int_Seconds, bool _bool_ShortString = false)
        {
            int intHrs = (_int_Seconds - (_int_Seconds % 3600)) / 3600;
            _int_Seconds -= (intHrs * 3600);
            int intMns = (_int_Seconds - (_int_Seconds % 60)) / 60;
            _int_Seconds -= (intMns * 60);
            if (intHrs == 0)
            {
                if (intMns == 0)
                {
                    return _int_Seconds.ToString() + "s";
                }
                else
                {
                    return intMns.ToString() + "m" + _int_Seconds.ToString() + "s";
                }
            }
            else if (_bool_ShortString)
            {
                return intHrs.ToString() + "h" + intMns.ToString() + "m";
            }
            else
            {
                return intHrs.ToString() + "h" + intMns.ToString() + "m" + _int_Seconds.ToString() + "s";
            }
        }
        private void Smooth_Last_Jumps(List<Step> _steps, int intOpener)
        {
            List<int> topThreeWeights = [.. _steps
                .Where(step => (step.Weight ?? 0) > 0)
                .Select(step => step.Weight.Value)
                .Distinct()
                .OrderByDescending(weight => weight)];
            int _int_LastWarmup = topThreeWeights.ElementAtOrDefault(0);
            int _int_SecondToLastWarmup = topThreeWeights.ElementAtOrDefault(1);
            int _int_ThirdToLastWarmup = topThreeWeights.ElementAtOrDefault(2);

            if (intOpener > 0)
            {
                if (intOpener == _int_LastWarmup)
                {
                    if ((_int_LastWarmup > 0 & _int_SecondToLastWarmup > 0 & _int_ThirdToLastWarmup > 0) &&
                        (_int_LastWarmup - _int_ThirdToLastWarmup > 3) &&
                        (((int)(decimal)(_int_LastWarmup - _int_ThirdToLastWarmup) / 2 + (decimal)_int_ThirdToLastWarmup) != _int_SecondToLastWarmup))
                    {
                        Step _step = _steps.FirstOrDefault(r => r.Weight == _int_SecondToLastWarmup && !r.Override);
                        if (_step != null)
                        {
                            _step.Weight = (int)Math.Floor((decimal)(_int_LastWarmup - _int_ThirdToLastWarmup) / 2 + _int_ThirdToLastWarmup);
                        }
                    }
                }
                else if (_int_LastWarmup > 0 & _int_SecondToLastWarmup > 0)
                {
                    if ((intOpener - _int_SecondToLastWarmup > 3) &&
                        (((int)(decimal)(intOpener - _int_SecondToLastWarmup) / 2 + (decimal)_int_SecondToLastWarmup) != _int_LastWarmup))
                    {
                        Step _step = _steps.FirstOrDefault(r => r.Weight == _int_LastWarmup && !r.Override);
                        if (_step != null)
                        {
                            _step.Weight = (int)Math.Floor((decimal)(intOpener - _int_SecondToLastWarmup) / 2 + _int_SecondToLastWarmup);
                        }
                    }
                }
            }
        }
        private List<Step> x_Steps(
            bool _bool_PreserveLifts,
            List<Extra> _extras,
            Dictionary<int, int> _jumps,
            Dictionary<int, int> _times,
            int _int_x_Sec_End,
            int _int_x_Wgt_Opener,
            bool _bool_Opener_in_Warmup,
            List<Step> _stepsIn = null
            )
        {
            if (_extras == null | _jumps == null | _times == null) { return null; }
            Dictionary<int, bool> _weightInputs = [];
            Dictionary<int, bool> _weights = [];
            List<Step> _steps = [];
            if (_bool_PreserveLifts & _stepsIn != null)
            {
                foreach (Step _step in _stepsIn)
                {
                    if ((_step.Weight ?? 0) > 0)
                    {
                        _weightInputs[(int)_step.Weight] = _step.Override;
                    }
                }
                //remove final auto populated weights
                int _max = _weightInputs.Max().Key;
                if (_max > 0)
                {
                    foreach (int key in _weightInputs.Keys.Where(r => r > _max))
                    {
                        _weightInputs.Remove(key);
                    }
                }
            }
            _weights = dictionary_Weights(
                _jumps: _jumps,
                _weightInputs: _weightInputs,
                bool_Opener_in_Warmup: _bool_Opener_in_Warmup,
                _int_Weight_Opener: _int_x_Wgt_Opener);

            int _int_TotalSeconds = _extras.Sum(r => r.Length);
            int _int_Order = 1;
            foreach (Extra _extra in _extras.OrderBy(r => r.Order))
            {
                _steps.Add(new Step(
                    action: _extra.Action,
                    weight: 0,
                    length: _extra.Length,
                    totalLength: _int_TotalSeconds,
                    order: _int_Order,
                    preStep: false,
                    @override: false
                    ));
                _int_Order++;
            }

            _times = _times.OrderBy(r => r.Key).ToDictionary(r => r.Key, r => r.Value);
            int _int_Weight_Last = _weights.Last().Key;
            foreach (KeyValuePair<int, bool> _kvp_Weight in _weights)
            {
                int intTime = _times.Where(r => r.Key <= _kvp_Weight.Key).Max(r => r.Value);
                if (_kvp_Weight.Key == _int_Weight_Last)
                {
                    intTime += _int_x_Sec_End;
                }
                _int_TotalSeconds += intTime;
                _steps.Add(new Step(
                    action: "Lift",
                    weight: _kvp_Weight.Key,
                    length: intTime,
                    totalLength: _int_TotalSeconds,
                    order: _int_Order,
                    preStep: false,
                    @override: _kvp_Weight.Value));
                _int_Order++;
            }

            _int_TotalSeconds = 0;
            for (int i = _steps.Count - 1; i >= 0; i--)
            {
                _int_TotalSeconds += _steps[i].Length;
                _steps[i].TotalLengthReverse = _int_TotalSeconds;
            }

            Smooth_Last_Jumps(_steps: _steps, intOpener: _int_x_Wgt_Opener);

            _steps.Insert(0, new Step(
                action: "wait",
                weight: 0,
                length: 0,
                totalLength: 0,
                totalLengthReverse: _int_TotalSeconds,
                order: 0,
                preStep: true,
                @override: false));
            return _steps;
        }
        private Dictionary<int, bool> dictionary_Weights(
            Dictionary<int, int> _jumps,
            Dictionary<int, bool> _weightInputs,
            int _int_Weight_Opener,
            bool bool_Opener_in_Warmup
            )
        {
            List<KeyValuePair<int, bool>> _weightInputs_List = [.. _weightInputs.OrderBy(r => r.Key)];
            Dictionary<int, bool> _weights = [];
            int _int_Weight = 0;
            int _int_Lift = 0;
            bool _bool_Override;
            do
            {
                int _int_Jump = 0;
                if (_int_Lift < _weightInputs_List.Count)
                {
                    _int_Jump = _weightInputs_List[_int_Lift].Key - _int_Weight;
                    _bool_Override = _weightInputs_List[_int_Lift].Value;
                }
                else if (_int_Weight == 0)
                {
                    _int_Jump = int_Barbell;
                    _bool_Override = false;
                }
                else
                {
                    _bool_Override = false;
                    foreach (KeyValuePair<int, int> _jump in _jumps.OrderBy(r => r.Key))
                    {
                        if (_jump.Key <= _int_Weight)
                        {
                            _int_Jump = _jump.Value;
                        }
                        else if (_int_Jump > 0 && _jump.Key + _jump.Value <= _int_Weight + _int_Jump)
                        {
                            _int_Jump = _jump.Key - _int_Weight + _jump.Value;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (_int_Jump < 1 || (_int_Weight + _int_Jump >= _int_Weight_Opener))
                {
                    break;
                }
                else
                {
                    _int_Weight += _int_Jump;
                    _weights[_int_Weight] = _bool_Override;
                    _int_Lift++;
                }
            } while (_int_Weight < _int_Weight_Opener);
            if (bool_Opener_in_Warmup && _weights.Count > 0 &&
                _weights.Max(r => r.Key) < _int_Weight_Opener)
            {
                _weights[_int_Weight_Opener] = _bool_Override;
            }

            return _weights;
        }
        private static DialogResult ShowInputDialog(ref string input)
        {
            Size size = new(200, 70);
            Form inputBox = new()
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = "Input Weight",
                BackColor = Color.Yellow
            };

            TextBox textBox = new()
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, 5),
                Text = input
            };
            inputBox.Controls.Add(textBox);

            Button okButton = new()
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "&OK",
                Location = new Point(size.Width - 80 - 80, 39),
                BackColor = SystemColors.Control
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new()
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "&Cancel",
                Location = new Point(size.Width - 80, 39),
                BackColor = SystemColors.Control
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            if (result == DialogResult.OK)
            {
                input = textBox.Text;
            }
            else
            {
                input = String.Empty;
            }
            return result;
        }
        private void Apply_Opener_Graphic_Vector(
            int _intWeightBar,
            int _intWeightOpener,
            bool _boolSnatch
            )
        {
            Panel _panelGraphic;
            if (_boolSnatch)
            {
                _panelGraphic = panel_snatch_Param_Opener;
            }
            else
            {
                _panelGraphic = panel_cj_Param_Opener;
            }

            _panelGraphic.Controls.Clear();

            WeightBox weightBoxGraphic = new()
            {
                intWeight = _intWeightOpener,
                intWeightBar = _intWeightBar,
                boolOpener = true,
                intShadowWidth = 1,
                intPlateGap = -1,
                Location = new Point(0, 0),
                Size = _panelGraphic.Size,
            };

            weightBoxGraphic.Paint += Apply_Vector_Weight_Graphic;
            _panelGraphic.Controls.Add(weightBoxGraphic);
        }
        private void Apply_Vector_Weight_Graphic(object sender, PaintEventArgs e)
        {
            WeightBox _weightBox_Graphic = (WeightBox)sender;
            int _int_WeightBar = _weightBox_Graphic.intWeightBar;
            int _int_Weight = _weightBox_Graphic.intWeight;
            int _int_Shadow_Width = _weightBox_Graphic.intShadowWidth;
            int _int_PlateGap = _weightBox_Graphic.intPlateGap;
            bool _bool_Opener = _weightBox_Graphic.boolOpener;

            int _int_LBuffer;
            int _int_TBuffer;
            int _int_BBuffer;
            bool _bool_Collars;
            bool _bool_5KGCollars;
            if (_bool_Opener)
            {
                _bool_Collars = _int_Weight - 5 >= _int_WeightBar;
                _bool_5KGCollars = ((_int_Weight - 15) >= _int_WeightBar);
                _int_LBuffer = 0;
                _int_TBuffer = 0;
                _int_BBuffer = 0;
            }
            else
            {
                _bool_Collars = false;
                _bool_5KGCollars = false;
                _int_TBuffer = 1;
                _int_LBuffer = _int_TBuffer;
                _int_BBuffer = _int_TBuffer + 4;
            }

            Dictionary<decimal, int> _plates = Plates_Count_For_Weight(
                _int_WeightBar: _int_WeightBar,
                _bool_5KGCollar: _bool_5KGCollars,
                _int_WeightLift: _int_Weight);

            int _int_Full_Height = _weightBox_Graphic.Height - _int_TBuffer - _int_BBuffer;
            Dictionary<decimal, PlateParameter> _plateParameters = new()
            {
                [25m] = new PlateParameter(width: 16, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Red)),
                [20m] = new PlateParameter(width: 15, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Blue)),
                [15m] = new PlateParameter(width: 13, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Yellow)),
                [10m] = new PlateParameter(width: 11, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_Green)),
            };
            // 5.0s
            int _int_Plate_Height = Convert.ToInt32(_int_Full_Height * .75);
            if (_plates.Where(r => r.Value > 0).Max(r => r.Key) == 5.0m)
            {
                _plateParameters[5.0m] = new PlateParameter(width: _plateParameters[25m].Width * 2, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_White));
            }
            else
            {
                _plateParameters[5.0m] = new PlateParameter(width: 9, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_White));
            }
            // 2.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .85);
            if (_plates.Where(r => r.Value > 0).Max(r => r.Key) == 2.5m)
            {
                _plateParameters[2.5m] = new PlateParameter(width: _plateParameters[25m].Width * 2, height: _int_Full_Height, brush: new SolidBrush(color: color_Plate_White));
            }
            else
            {
                _plateParameters[2.5m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Red));
            }
            // 2.0s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[2.0m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Blue));
            // 1.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[1.5m] = new PlateParameter(width: 8, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Yellow));
            // 1.0s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[1.0m] = new PlateParameter(width: 7, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_Green));
            // 0.5s
            _int_Plate_Height = Convert.ToInt32(_int_Plate_Height * .90);
            _plateParameters[0.5m] = new PlateParameter(width: 6, height: _int_Plate_Height, brush: new SolidBrush(color: color_Plate_White));

            int _int_Collar_Height = _bool_5KGCollars ? 18 : 12;
            int _int_Collar_Width = _bool_5KGCollars ? 8 : 6;
            int _int_MainBar_Width = (
                    _bool_Opener ?
                    Math.Min(400, _weightBox_Graphic.Width - _int_LBuffer - (2 * _int_Shadow_Width)) :
                    10);
            int _int_Sleeve_Width =
                (
                    _bool_Opener ?
                    (
                        _int_Weight > 229 ?
                        125 :
                        (
                            _int_Weight > 149 ?
                            100 :
                            75
                        )
                    ) :
                    75
                );
            int _int_SleeveKnuckle_Width = 9;
            bool _bool_Shadow = (_int_Shadow_Width > 0);

            SolidBrush _brush_BarShadow = new(color: Color.Black);
            SolidBrush _brush_PlateShadow = new(color: Color.Black);
            SolidBrush _brush_CollarSilver = new(color: Color.Gainsboro);
            SolidBrush _brush_BarGrey = new(color: color_BarGrey);

            //add bar
            //add bar shadow
            Rectangle _rect_MainBar;
            Rectangle _rectShadow_MainBar;
            Rectangle _rect_SleeveKnuckle_Right;
            Rectangle _rectShadow_SleeveKnuckle_Right;
            Rectangle _rect_Sleeve_Right, _rectShadow_Sleeve_Right;
            Rectangle _rect_SleeveKnuckle_Left;
            Rectangle _rectShadow_SleeveKnuckle_Left;
            Rectangle _rect_Sleeve_Left;
            Rectangle _rectShadow_Sleeve_Left;

            _rect_MainBar = new(
                x: _int_LBuffer + _int_Shadow_Width,
                y: 0,
                width: _int_MainBar_Width,
                height: 6);
            _rect_MainBar.Y = (_int_Full_Height / 2) - (_rect_MainBar.Height / 2) + _int_TBuffer;
            if (_bool_Opener)
            {
                _rect_Sleeve_Right = new(
                    x: _rect_MainBar.X + _rect_MainBar.Width - _int_Sleeve_Width,
                    y: 0,
                    width: _int_Sleeve_Width,
                    height: 10);
                _rect_SleeveKnuckle_Right = new(
                    x: _rect_Sleeve_Right.X - _int_SleeveKnuckle_Width,
                    y: 0,
                    width: _int_SleeveKnuckle_Width,
                    height: 16);
                _rect_Sleeve_Left = new(
                    x: _int_LBuffer + _int_Shadow_Width,
                    y: 0,
                    width: _int_Sleeve_Width,
                    height: 10);
                _rect_SleeveKnuckle_Left = new(
                    x: _rect_Sleeve_Left.X + _rect_Sleeve_Left.Width - 1,
                    y: 0,
                    width: _int_SleeveKnuckle_Width,
                    height: 16);
            }
            else
            {
                _rect_SleeveKnuckle_Right = new(
                    x: _rect_MainBar.X + _rect_MainBar.Width - 1,
                    y: 0,
                    width: _int_SleeveKnuckle_Width,
                    height: 16);
                _rect_Sleeve_Right = new(
                    x: _rect_SleeveKnuckle_Right.X + _rect_SleeveKnuckle_Right.Width - 1,
                    y: 0,
                    width: _int_Sleeve_Width,
                    height: 10);
                _rect_Sleeve_Left = new();
                _rect_SleeveKnuckle_Left = new();
            }
            _rect_SleeveKnuckle_Right.Y = (_int_Full_Height / 2) - (_rect_SleeveKnuckle_Right.Height / 2) + _int_TBuffer;
            _rect_Sleeve_Right.Y = (_int_Full_Height / 2) - (_rect_Sleeve_Right.Height / 2) + _int_TBuffer;
            _rect_SleeveKnuckle_Left.Y = _rect_SleeveKnuckle_Right.Y;
            _rect_Sleeve_Left.Y = _rect_Sleeve_Right.Y;
            if (_bool_Shadow)
            {
                _rectShadow_MainBar = _rect_MainBar;
                _rectShadow_MainBar.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                _rectShadow_SleeveKnuckle_Right = _rect_SleeveKnuckle_Right;
                _rectShadow_SleeveKnuckle_Right.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                _rectShadow_Sleeve_Right = _rect_Sleeve_Right;
                _rectShadow_Sleeve_Right.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                e.Graphics.FillRectangle(
                    brush: _brush_BarShadow,
                    rect: _rectShadow_MainBar);
                e.Graphics.FillRectangle(
                    brush: _brush_BarShadow,
                    rect: _rectShadow_SleeveKnuckle_Right);
                e.Graphics.FillRectangle(
                    brush: _brush_BarShadow,
                    rect: _rectShadow_Sleeve_Right);
                if (_bool_Opener)
                {
                    _rectShadow_SleeveKnuckle_Left = _rect_SleeveKnuckle_Left;
                    _rectShadow_SleeveKnuckle_Left.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                    _rectShadow_Sleeve_Left = _rect_Sleeve_Left;
                    _rectShadow_Sleeve_Left.Inflate(width: _int_Shadow_Width, height: _int_Shadow_Width);
                    e.Graphics.FillRectangle(
                        brush: _brush_BarShadow,
                        rect: _rectShadow_MainBar);
                    e.Graphics.FillRectangle(
                        brush: _brush_BarShadow,
                        rect: _rectShadow_SleeveKnuckle_Left);
                    e.Graphics.FillRectangle(
                        brush: _brush_BarShadow,
                        rect: _rectShadow_Sleeve_Left);
                }
            }
            e.Graphics.FillRectangle(
                brush: _brush_BarGrey,
                rect: _rect_MainBar);
            e.Graphics.FillRectangle(
                brush: _brush_BarGrey,
                rect: _rect_SleeveKnuckle_Right);
            e.Graphics.FillRectangle(
                brush: _brush_BarGrey,
                rect: _rect_Sleeve_Right);
            if (_bool_Opener)
            {
                e.Graphics.FillRectangle(
                    brush: _brush_BarGrey,
                    rect: _rect_MainBar);
                e.Graphics.FillRectangle(
                    brush: _brush_BarGrey,
                    rect: _rect_SleeveKnuckle_Left);
                e.Graphics.FillRectangle(
                    brush: _brush_BarGrey,
                    rect: _rect_Sleeve_Left);
            }

            // add plates
            int _int_Left = _rect_SleeveKnuckle_Right.X + _int_SleeveKnuckle_Width + _int_Shadow_Width + 1;
            int _int_Right = _rect_SleeveKnuckle_Left.X - _int_Shadow_Width - 1;
            bool _bool_CollarsDone = !_bool_Collars; // if not doing collar, mark them already done
            foreach (KeyValuePair<decimal, int> _plate in _plates.OrderByDescending(r => r.Key).Where(r => r.Value > 0))
            {
                if (!_bool_CollarsDone && _plate.Value < 2.5m)
                {
                    Rectangle _rect = new(
                        x: _int_Left,
                        y: (_int_Full_Height / 2) - (_int_Collar_Height / 2) + _int_TBuffer,
                        width: _int_Collar_Width,
                        height: _int_Collar_Height);
                    Rectangle _rect_Shadow = _rect;
                    if (_bool_Shadow)
                    {
                        _rect_Shadow.Width += _int_Shadow_Width * 2;
                        _rect.X += _int_Shadow_Width;
                        _rect.Inflate(width: 0, height: -_int_Shadow_Width);
                        e.Graphics.FillRectangle(
                            brush: _brush_BarShadow,
                            rect: _rect_Shadow);
                    }
                    Brush b = _bool_5KGCollars ? _brush_CollarSilver : Brushes.Black;
                    e.Graphics.FillRectangle(
                        brush: b,
                        rect: _rect);
                    _int_Left += _rect_Shadow.Width + _int_PlateGap;
                    if (_bool_Opener)
                    {
                        _int_Right -= _rect_Shadow.Width;
                        _rect.X = _int_Right;
                        if (_bool_Shadow)
                        {
                            _rect_Shadow.X = _rect.X;
                            _rect.X += _int_Shadow_Width;
                            e.Graphics.FillRectangle(
                                brush: _brush_PlateShadow,
                                rect: _rect_Shadow);
                        }
                        e.Graphics.FillRectangle(
                            brush: b,
                            rect: _rect);
                        _int_Right -= _int_PlateGap;
                    }
                    _bool_CollarsDone = true;
                }
                if (_plateParameters.TryGetValue(_plate.Key, out PlateParameter _parameter))
                {
                    for (int _int_Plate = 1; _int_Plate <= _plate.Value; _int_Plate++)
                    {
                        Rectangle _rect = new(
                            x: _int_Left,
                            y: (_int_Full_Height / 2) - (_parameter.Height / 2) + _int_TBuffer,
                            width: _parameter.Width,
                            height: _parameter.Height);
                        Rectangle _rect_Shadow = _rect;
                        if (_bool_Shadow)
                        {
                            _rect_Shadow.Width += _int_Shadow_Width * 2;
                            _rect.X += _int_Shadow_Width;
                            _rect.Inflate(width: 0, height: -_int_Shadow_Width);
                            e.Graphics.FillRectangle(
                                brush: _brush_PlateShadow,
                                rect: _rect_Shadow);
                        }
                        e.Graphics.FillRectangle(
                            brush: _parameter.Brush,
                            rect: _rect);
                        _int_Left += _rect_Shadow.Width + _int_PlateGap;
                        if (_bool_Opener)
                        {
                            _int_Right -= _rect_Shadow.Width;
                            _rect.X = _int_Right;
                            if (_bool_Shadow)
                            {
                                _rect_Shadow.X = _rect.X;
                                _rect.X += _int_Shadow_Width;
                                e.Graphics.FillRectangle(
                                    brush: _brush_PlateShadow,
                                    rect: _rect_Shadow);
                            }
                            e.Graphics.FillRectangle(
                                brush: _parameter.Brush,
                                rect: _rect);
                            _int_Right -= _int_PlateGap;
                        }
                    }
                }
            }
        }
        private Dictionary<decimal, int> Plates_Count_For_Weight(
            int _int_WeightBar,
            bool _bool_5KGCollar,
            int _int_WeightLift)
        {
            Dictionary<decimal, int> _plates = [];
            if (_bool_5KGCollar) { _int_WeightBar += 5; }
            if (_int_WeightLift > _int_WeightBar)
            {
                decimal _decToGo = Convert.ToDecimal(_int_WeightLift - _int_WeightBar) / 2m;
                decimal[] _decPlateWeights = [25m, 20m, 15m, 10m, 5.0m, 2.5m, 2.0m, 1.5m, 1.0m, 0.5m];
                foreach (decimal _decPlateWeight in _decPlateWeights)
                {
                    int _int = Convert.ToInt32((_decToGo - _decToGo % _decPlateWeight) / _decPlateWeight);
                    if (_int > 0)
                    {
                        _plates[_decPlateWeight] = _int;
                        _decToGo -= Convert.ToDecimal(_int) * _decPlateWeight;
                    }
                }
            }
            return _plates;
        }
        #endregion

    }
    public class WeightBox : PictureBox
    {
        public bool boolOpener; //is opener vs is warmup
        public int intWeightBar;
        public int intWeight;
        public int intShadowWidth;
        public int intPlateGap;

    }
}