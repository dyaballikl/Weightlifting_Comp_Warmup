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
        private void sim_timer_cj_Live_Tick()
        {
            CJ_Variant_TimerTickProcessing(intSecondsToOpen: out int intSecondsToOpen, _dateTime_Now: out  DateTime _dateTime_Now);
         
            TimeSpan _timeSpan_Open = TimeSpan.FromSeconds(intSecondsToOpen);
            if (intSecondsToOpen > 0)
            {
                label_cj_Live_TimeTillOpener.Text = Seconds_To_String(intSecondsToOpen);
                label_cj_Live_OpenTime.Text = _dateTime_Now.Add(_timeSpan_Open).ToString(@"HH\:mm\:ss");
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
                            label_Action = _step.Controls.LabelAction;
                            label_Action.Text = ActionTextString(_step: _step, _isFuture: false);
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
                        _panel_Live_Step.BackColor = AppColors.cj_Live_BG;
                        _panel_Live_Step.ForeColor = AppColors.Live_Default_FG;
                        label_Progress_Time = _step.Controls.LabelProgressTime;
                        label_Progress_Time.Visible = false;
                        progressBar_Step = _step.Controls.ProgressBarStep;
                        bool _isFuture = (_int_Order >= _intStep);
                        progressBar_Step.Value = (_isFuture ? 0 : progressBar_Step.Maximum);
                        label_Action = _step.Controls.LabelAction;
                        label_Action.Text = ActionTextString(_step: _step, _isFuture: _isFuture);
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

            label_cj_Live_CurrentTime.Text = _dateTime_Now.ToString("HH:mm:ss");
        }
        private void CJ_Variant_TimerTickProcessing(out int intSecondsToOpen, out DateTime _dateTime_Now)
        {
            _dateTime_Now = _now;
            intSecondsToOpen = 0;
            if (bool_cj_SnStillLifting) // snatches still running
            {
                if (profileActive.cj_SnatchLifts_Out == 0)
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

                    if (profileActive.cj_SnatchLifts_Out == 0)
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
            else if (profileActive.cj_SnatchLifts_Out > 0)
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
                    if (bool_cj_AutoAdvance | profileActive.cj_LiftsOut == 1)
                    {
                        cj_Advance_StageLift();
                    }
                }

                if (profileActive.cj_LiftsOut == 0)
                {
                    bool_cj_LiveLifting = false;
                }
            }
            else if (!bool_cj_SnStillLifting & !bool_cj_BreakRunning)
            {
                if (profileActive.cj_LiftsOut > 0)
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
                    button_cj_Live_snStageAdvance.BackColor = AppColors.cj_Live_BG;
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
                    button_cj_Live_StageAdvance.BackColor = AppColors.cj_Live_BG;
                }
            }


            if (bool_cj_SnStillLifting | bool_cj_BreakRunning | bool_cj_LiveLifting)
            {
                intSecondsToOpen = (profileActive.cj_LiftsOut - 1) * profileActive.cj_SecondsStage +
                    progressBar_cj_Live_StageLift.Maximum - progressBar_cj_Live_StageLift.Value;
            }

            if (bool_cj_SnStillLifting | bool_cj_BreakRunning)
            {
                intSecondsToOpen += progressBar_cj_Live_Break.Maximum - progressBar_cj_Live_Break.Value;
            }

            if (bool_cj_SnStillLifting) // snatches still running
            {
                intSecondsToOpen += (profileActive.cj_SnatchLifts_Out - 1) * profileActive.snatch_SecondsStage +
                    progressBar_cj_Live_sn.Maximum - progressBar_cj_Live_sn.Value;
            }
        }
    }
}
