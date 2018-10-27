using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;

namespace MarkThree.Forms
{
    public partial class RangeSlider : UserControl
    {
       
        /// <summary>
        /// Member variables for buffered graphics
        /// </summary>
        private BufferedGraphicsContext graphicsContext = null;
        private BufferedGraphics graphicsBuffer = null;

        // brushes/pens
        private Brush redBrush = Brushes.Red;
        private Brush greenBrush = Brushes.Green;
        private Brush blackBrush = Brushes.Black;
        private Pen linePen = Pens.Yellow;
        private Pen blackPen = Pens.Black;
        
        private Font textFont = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private Font labelFont = new Font("Microsoft Sans Serif", 10);

        private static int VERTICAL_OFFSET = 13;
        private static int BAR_WIDTH = 150;
        private static String THRESHOLD = "Threshold";
        private static int ROUND_TO_NUMBER = 10000;

        public RangeSlider() 
        {
            InitializeComponent();

            // set the default properties - not sure why this needs to be done,
            // because there is an attribute for DefaultValue
            // the designer complains if there are not defaults set.
            this.Minimum = 0;
            this.Maximum = 500000;
            this.BucketLabel = "Low";

            // styles for double buffering
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.Opaque, true);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            // TODO: initialize the form
        }


        [Category("ADV Slider"),
        Browsable(true),
        Description("The maximum value of the threshold slider for the given ADV bucket."),
        DefaultValue(100000)]
        public int Maximum
        {
            get
            {
                return this.trackBar.Maximum;
            }
            set
            {
                if (this.trackBar.Maximum != value)
                {
                    this.trackBar.Maximum = value;
                    ComputeSteps();
                }
            }
        }

        [Category("ADV Slider"),
        Browsable(true),
        Description("The minimum value of the threshold slider for the given ADV bucket."),
        DefaultValue(25000)]
        public int Minimum
        {
            get
            {
                return this.trackBar.Minimum;
            }
            set
            {
                if (this.trackBar.Minimum != value)
                {
                    this.trackBar.Minimum = value;
                    ComputeSteps();
                }
            }
        }


        [Category("ADV Slider"),
        Browsable(true),
        Description("The label for the ADV bucket (Low, Medium, High)."),
        DefaultValue("Low")]
        public String BucketLabel
        {
            get
            {
                return this.labelADV.Text;
            }
            set
            {
                if (this.labelADV.Text != value)
                {
                    this.labelADV.Text = value;
                    this.labelADV.Invalidate();
                }
            }
        }

        /// <summary>
        /// True if auto execute is selected, false otherwise.
        /// </summary>
        public bool AutoExecuteChecked
        {
            get
            {
                return this.checkAutoEx.Checked;
            }
        }

        
        /// <summary>
        /// The threshold value for the ADV bucket
        /// </summary>
        public int Threshold
        {
            get
            {
                return this.trackBar.Value;
            }

            set
            {
                if (value < this.trackBar.Minimum || value > this.trackBar.Maximum)
                    this.trackBar.Value = this.trackBar.Minimum;
                else 
                    this.trackBar.Value = value;
            }
        }

       
        /// <summary>
        /// Called when the slider value changes.  Forces the slider bar to redraw and updates the textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValueChanged(object sender, EventArgs e)
        {
            if (this.trackBar.Value % ROUND_TO_NUMBER != 0)
            {
                this.trackBar.Value = ((int)this.trackBar.Value / ROUND_TO_NUMBER) * ROUND_TO_NUMBER;
            }

            this.Invalidate();     
        }

        private void ComputeSteps()
        {
            if (this.Minimum == this.Maximum || this.Minimum > this.Maximum)
                return;

            this.trackBar.TickFrequency = (this.Maximum - this.Minimum) / 10;
            this.trackBar.LargeChange = this.trackBar.TickFrequency;
            this.trackBar.SmallChange = this.trackBar.TickFrequency;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // initialize the graphics variables
            Graphics g = e.Graphics;
            graphicsContext = BufferedGraphicsManager.Current;
            graphicsBuffer = graphicsContext.Allocate(g, new Rectangle(0, 0, this.Width, this.Height));
            graphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            // draw the control background
            graphicsBuffer.Graphics.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(0, 0, this.Width, this.Height));

            int yHeight = this.Height - 2 * VERTICAL_OFFSET;       
        
            int yPoint = GetYPointInBar();

            int xStart = this.trackBar.Right;
            int xEnd = xStart + BAR_WIDTH;
           
            // draw the green background of the bar
            Rectangle rect = new Rectangle(xStart, VERTICAL_OFFSET, xEnd - xStart, yHeight);
            LinearGradientBrush backBrush = new LinearGradientBrush(
                     new Point(rect.Right, rect.Top),
                     new Point(rect.Left, rect.Bottom),
                     Color.FromArgb(255, 0, 200, 0),
                     Color.FromArgb(255, 20, 190, 20));
            graphicsBuffer.Graphics.FillRectangle(backBrush, rect);
            graphicsBuffer.Graphics.DrawRectangle(blackPen, rect);
            
            // draw the red bar
            Rectangle bottomRect = GetBottomRect(this.trackBar.Value);
            if (bottomRect.Height > 0)
            {
                LinearGradientBrush linGrBrush = new LinearGradientBrush(
                     new Point(bottomRect.Right, bottomRect.Top),
                     new Point(bottomRect.Left, bottomRect.Bottom),
                     Color.FromArgb(255, 190, 20, 20),
                     Color.FromArgb(255, 200, 0, 0));
                graphicsBuffer.Graphics.FillRectangle(linGrBrush, bottomRect);
            }

            // draw a border around the display bar
            graphicsBuffer.Graphics.DrawRectangle(blackPen, rect);
                        
            // draw the yellow line where the bar is set
            graphicsBuffer.Graphics.DrawLine(linePen, xStart, yPoint, xEnd, yPoint);

            // draw the shares label
            String str = string.Format("{0:0,0}", this.trackBar.Value);

            str += " shares";
            SizeF textSize = graphicsBuffer.Graphics.MeasureString(str, textFont);
            if (yPoint - textSize.Height - 1 > VERTICAL_OFFSET)
            {
                graphicsBuffer.Graphics.DrawString(str, textFont, Brushes.Black, xStart + 10, yPoint - textSize.Height - 1);
            }
            else
            {
                graphicsBuffer.Graphics.DrawString(str, textFont, Brushes.Black, xStart + 10, yPoint + 1);
            }

            // draw the "Threshold" vertical label
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            SizeF labelSize = graphicsBuffer.Graphics.MeasureString(THRESHOLD, labelFont, 10, drawFormat);
            graphicsBuffer.Graphics.DrawString(THRESHOLD, 
                labelFont, 
                Brushes.Gray, 
                this.trackBar.Left - 20, 
                this.trackBar.Top + 25, drawFormat);
        

            // write graphics buffer to screen
            graphicsBuffer.Render(g);

            base.OnPaint(e);
        }

        /// <summary>
        /// Returns the bottom rectangle in client coords of the display bar.  This area will 
        /// be drawn red on the control
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Rectangle GetBottomRect(int value)
        {
            int xStart = this.trackBar.Right;
            int xEnd = xStart + BAR_WIDTH;

            int yHeight = this.Height - 2 * VERTICAL_OFFSET;
            int yPoint = GetYPointInBar();

            return new Rectangle(xStart, yPoint, xEnd - xStart, yHeight + VERTICAL_OFFSET - yPoint);
        }

        /// <summary>
        /// Returns the height of the current value in the display bar
        /// </summary>
        /// <returns></returns>
        private int GetYPointInBar()
        {
            int yHeight = this.Height - 2 * VERTICAL_OFFSET;

            int yFromBottom = yHeight * (this.trackBar.Value - this.Minimum) / (this.Maximum - this.Minimum);

            return yHeight + VERTICAL_OFFSET - yFromBottom;
        }

        
       
        /// <summary>
        /// Updates the slider control with the current value
        /// </summary>
        /// <param name="value">Value to set the slider control</param>
        private void UpdateSliderValue(int value)
        {
            this.trackBar.Value = value;
        }      
        
    }

   
 

}
