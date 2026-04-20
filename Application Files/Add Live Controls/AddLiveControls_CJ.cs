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
    public partial class form_Main
    {
        private void Populate_cj_Live_Steps()
        {
            Clear_cj_Live_Steps();

            if (cjStepsPLAN is null)
            {
                PopulateSteps(LiftType.CleanAndJerk);
            }
            if (cjStepsPLAN is null || !cjStepsPLAN.Any())
            {
                MessageBox.Show("An error has occurred. Step plan could not be determined.");
                this.Close();
                return;
            }
            cjStepsLIVE = [.. cjStepsPLAN.Select(r => r.Clone())];
            Step _stepLast = cjStepsLIVE.Last();
            cjStepsLIVE.Add(new(
                action: $"open at {profileActive.cj_OpenerWeight}",
                weight: profileActive.cj_OpenerWeight,
                length: 0,
                totalLength: _stepLast.TotalLength,
                totalLengthReverse: 0,
                order: _stepLast.Order + 1,
                preStep: false,
                isOpener: true));

            int intY = 1;
            int _int_panel_Live_Step_Width = panel_cj_Live_Steps.Width - 4;
            int _int_progressBar_Step_Width_NoScroll = _int_panel_Live_Step_Width - 350;
            int _int_progressBar_Step_Width_Scroll = _int_progressBar_Step_Width_NoScroll - SystemInformation.VerticalScrollBarWidth;
            int _int_progressBar_Step_Height = 65;
            int _int_progressBar_Step_Width;
            int _int_label_Weight_Width = 105;
            Point _point_progressBar_Step_Location = new(300, 6);

            SuspendLayout();
            foreach (Step _step in cjStepsLIVE)
            {
                if (panel_cj_Live_Steps.VerticalScroll.Visible)
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
                    BackColor = AppColors.cj_Live_BG,
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
                    Size = new Size(180, 30),
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
                        weight: (int)_step.Weight)
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
                panel_cj_Live_Steps.Controls.Add(panel_Live_Step);

                intY += 81;
            }
            int_cj_Warmup_Step = -1;
            label_cj_Live_CurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
            panel_cj_Live_Times.Visible = true;
            ResumeLayout();

            //if (cjStepsPLAN is null)
            //{
            //    PopulateSteps(LiftType.CleanAndJerk);
            //}
            //if (cjStepsPLAN is null)
            //{
            //    MessageBox.Show("An error has occurred. Step plan could not be determined.");
            //    this.Close();
            //    return;
            //}
            //cjStepsLIVE = [.. cjStepsPLAN.Select(r => r.Clone())];
            //Step _stepLast = cjStepsLIVE.Last();
            //cjStepsLIVE.Add(new(
            //    action: $"open at {profileActive.CJ_OpenerWeight}",
            //    weight: profileActive.CJ_OpenerWeight,
            //    length: 0,
            //    totalLength: _stepLast.TotalLength,
            //    totalLengthReverse: 0,
            //    order: _stepLast.Order + 1,
            //    preStep: false,
            //    isOpener: true));

            //int intY = 1;
            //int _int_panel_Live_Step_Width = panel_cj_Live_Steps.Width - 4;
            //int _int_progressBar_Step_Width_NoScroll = _int_panel_Live_Step_Width - 350;
            //int _int_progressBar_Step_Width_Scroll = _int_progressBar_Step_Width_NoScroll - SystemInformation.VerticalScrollBarWidth;
            //int _int_progressBar_Step_Height = 65;
            //int _int_progressBar_Step_Width;
            //int _int_label_Weight_Width = 105;
            //Point _point_progressBar_Step_Location = new(300, 6);

            //SuspendLayout();
            //foreach (Step _step in cjStepsLIVE)
            //{
            //    if (panel_cj_Live_Steps.VerticalScroll.Visible)
            //    {
            //        _int_progressBar_Step_Width = _int_progressBar_Step_Width_Scroll;
            //    }
            //    else
            //    {
            //        _int_progressBar_Step_Width = _int_progressBar_Step_Width_NoScroll;
            //    }
            //    bool _isLift = (_step.Weight > 0);
            //    string strActionText = ActionTextString(_step: _step, _isFuture: true);
            //    Panel panel_Live_Step = new()
            //    {
            //        Size = new Size(_int_panel_Live_Step_Width, 80),
            //        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            //        BackColor = AppColors.Cj_Live_BG,
            //        ForeColor = AppColors.Live_Default_FG,
            //        Location = new Point(1, intY)
            //    };
            //    Label label_Action = new()
            //    {
            //        Text = strActionText,
            //        AutoSize = false,
            //        Size = new Size(280, 75),
            //        TextAlign = ContentAlignment.TopRight,
            //        Location = new Point(6, 1)
            //    };
            //    ProgressBar progressBar_Step = new()
            //    {
            //        Size = new Size(_int_progressBar_Step_Width, _int_progressBar_Step_Height),
            //        Location = _point_progressBar_Step_Location,
            //        Maximum = _step.Length,
            //        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            //    };
            //    if (progressBar_Step.Maximum < 1)
            //    {
            //        progressBar_Step.Maximum = 1;
            //        progressBar_Step.Value = 1;
            //    }
            //    Label label_Weight = null;
            //    if (_isLift)
            //    {
            //        label_Weight = new Label
            //        {
            //            BorderStyle = BorderStyle.FixedSingle,
            //            BackColor = Color.Black,
            //            AutoSize = false,
            //            Size = new Size(_int_label_Weight_Width, _int_progressBar_Step_Height),
            //            Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width, _point_progressBar_Step_Location.Y),
            //            ForeColor = SystemColors.Window,
            //            Text = $"{(_step.isOpener ? "open at" : "lift")} {_step.Weight}",
            //            TextAlign = ContentAlignment.MiddleCenter,
            //            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            //            Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
            //        };
            //        label_Action.Font = new Font("Gadugi", 14.0F, FontStyle.Regular);
            //    }
            //    Label label_Time = new()
            //    {
            //        Text = String.Empty,
            //        BorderStyle = BorderStyle.FixedSingle,
            //        AutoSize = false,
            //        Size = new Size(180, 30),
            //        Location = _point_progressBar_Step_Location,
            //        Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
            //        Visible = false,
            //        TextAlign = ContentAlignment.MiddleCenter,
            //        Anchor = AnchorStyles.Top | AnchorStyles.Left,
            //    };
            //    Label label_Progress_Time = new()
            //    {
            //        Text = String.Empty,
            //        BorderStyle = BorderStyle.FixedSingle,
            //        AutoSize = false,
            //        Size = new Size(_int_label_Weight_Width, _int_progressBar_Step_Height),
            //        Location = new Point(_point_progressBar_Step_Location.X + _int_progressBar_Step_Width, _point_progressBar_Step_Location.Y),
            //        Font = new Font("Gadugi", 18.0F, FontStyle.Bold),
            //        Visible = false,
            //        TextAlign = ContentAlignment.MiddleCenter,
            //        Anchor = AnchorStyles.Top | AnchorStyles.Right,
            //    };
            //    panel_Live_Step.Controls.AddRange(
            //    [
            //        label_Action,
            //        progressBar_Step,
            //        label_Progress_Time,
            //        label_Time
            //    ]);
            //    WeightBox weightBoxGraphic = null;
            //    if (_isLift)
            //    {
            //        weightBoxGraphic = new(
            //            isOpener: false,
            //            isCompCollars: _step.isOpener,
            //            barWeight: profileActive.BarbellWeight,
            //            weight: (int)_step.Weight)
            //        {
            //            Size = new Size(120, 65),
            //            BackColor = Color.FromArgb(196, 196, 196),
            //            BorderStyle = BorderStyle.Fixed3D,
            //            Location = new Point(10, 6),
            //            Visible = true
            //        };
            //        panel_Live_Step.Controls.Add(weightBoxGraphic);
            //    }
            //    progressBar_Step.BringToFront();
            //    _step.Controls.PanelLiveStep = panel_Live_Step;
            //    _step.Controls.LabelAction = label_Action;
            //    _step.Controls.ProgressBarStep = progressBar_Step;
            //    weightBoxGraphic?.BringToFront();
            //    if (label_Weight != null)
            //    {
            //        panel_Live_Step.Controls.Add(label_Weight);
            //        _step.Controls.LabelWeight = label_Weight;
            //        label_Weight.BringToFront();
            //    }
            //    label_Time.BringToFront();
            //    _step.Controls.LabelTime = label_Time;
            //    label_Progress_Time.BringToFront();
            //    _step.Controls.LabelProgressTime = label_Progress_Time;
            //    panel_cj_Live_Steps.Controls.Add(panel_Live_Step);

            //    intY += 81;
            //}
            //int_cj_Warmup_Step = -1;
            //label_cj_Live_CurrentTime.Text = _now.ToString("HH:mm:ss");
            //panel_cj_Live_Times.Visible = true;
            //ResumeLayout();
        }
    }
}
