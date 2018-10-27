using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MarkThree.Forms
{
    public partial class TimeRangeSlider : UserControl
    {
        // base time for computing the slider positions
        private static TimeSpan timeNineAM = new TimeSpan(9, 0, 0);
        
        /// <summary>
        /// Constants used in drawing the time span bar.
        /// </summary>
        private static int XBAR_LEFT_OFFSET = 13;
        private static int XBAR_RIGHT_OFFSET = 11;


        public TimeRangeSlider()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The start time of eligibility for stock orders.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object StartTime
        {
            get
            {
                // return the value of the time picker control
                return this.startTimePicker.Value;
            }

            set
            {
                // DBNull will clear the control of any selected items.
                if (value is DBNull)
                {
                    // if there is no value, set to 9:00AM
                    this.startTimePicker.Value = new DateTime(2006, 1, 1, 9, 0, 0);
                }
                else
                {

                    // This will select the item from the list that corresponds to the value.  If the value is between ranges, it
                    // will add the value to the items in the domain as well.
                    if (value is DateTime)
                    {
                        // set the value in the time picker control
                        this.startTimePicker.Value = value;

                        // update the slider control to the right position
                        UpdateSlider(this.startTimeSlider, (DateTime)this.startTimePicker.Value);
                    }

                }
            }
        }

        /// <summary>
        /// The end time of eligibility for stock orders.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object EndTime
        {
            get
            {
                // return the value of the end time picker control
                return this.endTimePicker.Value;
            }

            set
            {
                // DBNull will clear the control of any selected items.
                if (value is DBNull)
                {
                    // if there is no value, then set to 4:00PM
                    this.endTimePicker.Value = new DateTime(2006, 1, 1, 16, 0, 0);
                }
                else
                {

                    // This will select the item from the list that corresponds to the value.  If the value is between ranges, it
                    // will add the value to the items in the domain as well.
                    if (value is DateTime)
                    {
                        // set the time picker value
                        this.endTimePicker.Value = value;

                        // update the slider control position
                        UpdateSlider(this.endTimeSlider, (DateTime)this.endTimePicker.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the control from the data model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
        {
            //  DateTime dateTime = startTimePicker.Value;
            //startTimePicker.Value = new DateTime(dateTime
        }

        /// <summary>
        /// Updates the slider with the new DateTime value.
        /// </summary>
        /// <param name="trackBar">The slider to update with the new value.</param>
        /// <param name="dateTime">The new DateTime value</param>
        private void UpdateSlider(TrackBar trackBar, DateTime dateTime)
        {
            try
            {
                TimeSpan timeSpan = new TimeSpan(dateTime.Hour, dateTime.Minute, 0);
                int trackBarValue = ((int)timeSpan.Subtract(timeNineAM).TotalMinutes) / 5;
                if (trackBarValue > trackBar.Maximum)
                    trackBar.Value = trackBar.Maximum;
                else if (trackBarValue < trackBar.Minimum)
                    trackBar.Value = trackBar.Minimum;
                else
                    trackBar.Value = trackBarValue;
            }
            catch (Exception e)
            {
                // Write the error and stack trace out to the debug listener
                EventLog.Error("{0}, {1}", e.Message, e.StackTrace);
            }
        }

        /// <summary>
        /// Updates the time picker with the DateTime value computer from the slider/trackBar
        /// </summary>
        /// <param name="timePicker">The time picker control to update</param>
        /// <param name="trackBar">The trackBar control to calculate the new time from.</param>
        private void UpdateTimePicker(TimePicker timePicker, TrackBar trackBar)
        {
            try
            {

                // get the new time based on the slider offset
                DateTime dateTime = (DateTime)timePicker.Value;
                TimeSpan timeSpan = timeNineAM.Add(TimeSpan.FromMinutes(5 * trackBar.Value));

                // set the new value
                timePicker.Value = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, timeSpan.Hours, timeSpan.Minutes, 0);

            }
            catch (Exception e)
            {
                // Write the error and stack trace out to the debug listener
                EventLog.Error("{0}, {1}", e.Message, e.StackTrace);
            }
        }
        /// <summary>
        /// Called when the top slider is being scrolled.  This handler makes
        /// sure the top time is never less than the bottom time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndTimeChanging(object sender, EventArgs e)
        {
            if (endTimeSlider.Value <= startTimeSlider.Value)
            {
                endTimeSlider.Value = startTimeSlider.Value;
            }

            UpdateTimePicker(endTimePicker, endTimeSlider);
        }
        
        /// <summary>
        /// Called when the bottom slider is being scrolled.  This handler makes
        /// sure the bottom time is never less than the top time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartTimeChanging(object sender, EventArgs e)
        {
            if (startTimeSlider.Value >= endTimeSlider.Value)
            {
                startTimeSlider.Value = endTimeSlider.Value;
            }
            UpdateTimePicker(startTimePicker, startTimeSlider);
        }

        /// <summary>
        /// Called when the user modifies the time using the time picker control.  This method will
        /// update the slider bar position to the new time value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEndTimeValueChanged(object sender, EventArgs e)
        {
            // if the user is editing the time field, but the time is not valid,
            // then the cast to DateTime willt hrow a InvalidCastException
            try
            {
                UpdateSlider(endTimeSlider, (DateTime)endTimePicker.Value);
            }
            catch (System.InvalidCastException)
            { 
            }
        }

        /// <summary>
        /// Called when the user modifies the time using the time picker control.  This method will
        /// update the slider bar position to the new time value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartTimeValueChanged(object sender, EventArgs e)
        {
            // if the user is editing the time field, but the time is not valid,
            // then the cast to DateTime willt hrow a InvalidCastException
            try
            {
                UpdateSlider(startTimeSlider, (DateTime)startTimePicker.Value);
            }
            catch (System.InvalidCastException)
            {
            }
        }

        /// <summary>
        /// Called when the one of the slider bar position changes.  This method will invalidate
        /// the custom drawn bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValueChanged(object sender, EventArgs e)
        {
            InvalidateSliderBar();
        }

        /// <summary>
        /// Invalidates the regions that have changed so the time bar is redrawn
        /// </summary>
        private void InvalidateSliderBar()
        {
            this.Invalidate();  
        }
       
        /// <summary>
        /// Paints the left rectangle, middle rectangle, and right rectangle of the time bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //////////////////////////////////////////////////////////////
            // draw the left rectangle (ie, the region before the start time)
            Rectangle leftBarRect = GetLeftBarRect(startTimeSlider.Value);
            if (leftBarRect.Width > 0)
            {
                LinearGradientBrush leftGrBrush = new LinearGradientBrush(
                         new Point(leftBarRect.Left, leftBarRect.Bottom),
                         new Point(leftBarRect.Right, leftBarRect.Top),
                         Color.FromArgb(255, 210, 0, 0),
                         Color.FromArgb(255, 190, 20, 20));
                g.FillRectangle(leftGrBrush, leftBarRect);
            }

            //////////////////////////////////////////////////////////////
            // draw the right rectangle (ie, the region after the end time)
            Rectangle rightBarRect = GetRightBarRect(endTimeSlider.Value);
            if (rightBarRect.Width > 0)
            {

                LinearGradientBrush rightGrBrush = new LinearGradientBrush(
                         new Point(rightBarRect.Left, rightBarRect.Top),
                         new Point(rightBarRect.Right, rightBarRect.Bottom),
                         Color.FromArgb(255, 190, 20, 20),
                         Color.FromArgb(255, 210, 0, 0));

                g.FillRectangle(rightGrBrush, rightBarRect);
            }


            //////////////////////////////////////////////////////////////
            // draw the mid rectangle (ie, the region between the start and end time)
            Rectangle midBarRect = GetMidBarRect(startTimeSlider.Value, endTimeSlider.Value);
            if (midBarRect.Width > 0)
            {
                LinearGradientBrush midGrBrush = new LinearGradientBrush(
                         new Point(midBarRect.Left, midBarRect.Top),
                         new Point(midBarRect.Right, midBarRect.Bottom),
                         Color.FromArgb(255, 0, 200, 0),
                         Color.FromArgb(255, 20, 190, 20));
                g.FillRectangle(midGrBrush, midBarRect);
            }

            ////////////////////////////////////////////////////////////////
            // draw the vertical lines to mark the start/end times
            Rectangle barRect = GetBarRect();
            int leftTickPos = GetLeftTickPosition(startTimeSlider.Value);
            int rightTickPos = GetRightTickPosition(endTimeSlider.Value);
            Pen yellowPen = Pens.Yellow;
            g.DrawLine(yellowPen, leftTickPos, barRect.Top, leftTickPos, barRect.Bottom);
            g.DrawLine(yellowPen, rightTickPos, barRect.Top, rightTickPos, barRect.Bottom);

            Pen blackPen = Pens.Black;
            g.DrawRectangle(blackPen, barRect);
            
        }

        
        ////////////////////////////////////////////////////////////////
        // Helper methods to return coordinates for drawing the slider bar
        /////////////////////////////////////////////////////////////////

        private Rectangle GetBarRect()
        {
            // calculate the rectangle of the owner drawn timespan bar
            return
                new Rectangle(labelBarPosition.Left + XBAR_LEFT_OFFSET, labelBarPosition.Top,
                              labelBarPosition.Width - XBAR_LEFT_OFFSET - XBAR_RIGHT_OFFSET,
                              labelBarPosition.Height);
        }

        private Rectangle GetLeftBarRect(int bottomValue)
        {
            Rectangle barRect = GetBarRect();
            int leftXPos = GetLeftTickPosition(bottomValue);

            // calculate the leftmost rectangle of the bar (ie, the time range
            // that is before the start time)
            return new Rectangle(barRect.Left, barRect.Top, leftXPos - barRect.Left, barRect.Height);
        }

        private Rectangle GetMidBarRect(int bottomValue, int topValue)
        {
            Rectangle barRect = GetBarRect();
            int leftXPos = GetLeftTickPosition(bottomValue);
            int rightXPos = GetRightTickPosition(topValue);

            // calculate the middle rectangle of the bar (ie, the time range
            // that is between the start and end times)
            return new Rectangle(leftXPos, barRect.Top, rightXPos - leftXPos, barRect.Height);

        }

        private Rectangle GetRightBarRect(int topValue)
        {
            Rectangle barRect = GetBarRect();
            int rightXPos = GetRightTickPosition(topValue);

            // calculate the rightmost rectangle of the bar (ie, the time range
            // this is after the end time)
            return new Rectangle(rightXPos, barRect.Top, barRect.Right - rightXPos, barRect.Height);
        }

        private int GetLeftTickPosition(int bottomValue)
        {
            Rectangle barRect = GetBarRect();
            return (bottomValue * barRect.Width) / startTimeSlider.Maximum + barRect.Left;
        }

        private int GetRightTickPosition(int topValue)
        {
            Rectangle barRect = GetBarRect();
            return (topValue * barRect.Width) / endTimeSlider.Maximum + barRect.Left;
        }

        private void OnStartTimeChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime newValue = Convert.ToDateTime(startTimePicker.Text);
                UpdateSlider(startTimeSlider, newValue);
            }
            catch (Exception)
            {

            }
        }

        private void OnEndTimeChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime newValue = Convert.ToDateTime(endTimePicker.Text);
                UpdateSlider(endTimeSlider, newValue);
            }
            catch (Exception)
            {

            }
        }

    }
}
