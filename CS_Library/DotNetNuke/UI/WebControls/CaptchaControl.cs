#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Image=System.Web.UI.WebControls.Image;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The CaptchaControl control provides a Captcha Challenge control
    /// </Summary>
    [ToolboxData( "<{0}:CaptchaControl Runat=\"server\" CaptchaHeight=\"100px\" CaptchaWidth=\"300px\" />" )]
    public class CaptchaControl : WebControl, INamingContainer, IPostBackDataHandler
    {

        public event ServerValidateEventHandler UserValidated
        {
            add
            {
                this.UserValidatedEvent += value;
            }
            remove
            {
                this.UserValidatedEvent -= value;
            }
        }
        private const string CHARS_DEFAULT = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        private const int EXPIRATION_DEFAULT = 120;

        internal const string KEY = "captcha";
        private const int LENGTH_DEFAULT = 6;
        private const string RENDERURL_DEFAULT = "ImageChallenge.captcha.aspx";

        private bool _Authenticated;
        private Color _BackGroundColor = Color.Transparent;
        private string _BackGroundImage = "";
        private string _CaptchaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private Unit _CaptchaHeight = Unit.Pixel(100);
        private int _CaptchaLength = 6;
        private string _CaptchaText;
        private Unit _CaptchaWidth = Unit.Pixel(300);
        private string _ErrorMessage;
        private Style _ErrorStyle = new Style();
        private int _Expiration = 120;

        private static string[] _FontFamilies = new string[] { "Arial", "Comic Sans MS", "Courier New", "Georgia", "Lucida Console", "MS Sans Serif", "Stencil", "Tahoma", "Times New Roman", "Trebuchet MS", "Verdana" };
        private Image _image;
        private bool _IsValid = false;
        private static Random _Rand = new Random();
        private string _RenderUrl = "ImageChallenge.captcha.aspx";
        private static string _Separator = ":-:";
        private string _Text = "Enter the code shown above:";
        private ServerValidateEventHandler UserValidatedEvent;
        
        /// <Summary>Gets and sets the BackGroundColor</Summary>
        [Category( "Appearance" ), DescriptionAttribute( "The Background Color to use for the Captcha Image." )]
        public Color BackGroundColor
        {
            get
            {
                return this._BackGroundColor;
            }
            set
            {
                this._BackGroundColor = value;
            }
        }

        /// <Summary>Gets and sets the BackGround Image</Summary>
        [CategoryAttribute( "Appearance" ), DescriptionAttribute( "A Background Image to use for the Captcha Image." )]
        public string BackGroundImage
        {
            get
            {
                return this._BackGroundImage;
            }
            set
            {
                this._BackGroundImage = value;
            }
        }

        /// <Summary>Gets and sets the list of characters</Summary>
        [DescriptionAttribute( "Characters used to render CAPTCHA text. A character will be picked randomly from the string." ), DefaultValueAttribute( "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789" ), CategoryAttribute( "Behavior" )]
        public string CaptchaChars
        {
            get
            {
                return this._CaptchaChars;
            }
            set
            {
                this._CaptchaChars = value;
            }
        }

        /// <Summary>Gets and sets the height of the Captcha image</Summary>
        [CategoryAttribute( "Appearance" ), DescriptionAttribute( "Height of Captcha Image." )]
        public Unit CaptchaHeight
        {
            get
            {
                return this._CaptchaHeight;
            }
            set
            {
                this._CaptchaHeight = value;
            }
        }

        /// <Summary>Gets and sets the length of the Captcha string</Summary>
        [CategoryAttribute( "Behavior" ), DefaultValueAttribute( 6 ), DescriptionAttribute( "Number of CaptchaChars used in the CAPTCHA text" )]
        public int CaptchaLength
        {
            get
            {
                return this._CaptchaLength;
            }
            set
            {
                this._CaptchaLength = value;
            }
        }

        /// <Summary>Gets and sets the width of the Captcha image</Summary>
        [CategoryAttribute( "Appearance" ), DescriptionAttribute( "Width of Captcha Image." )]
        public Unit CaptchaWidth
        {
            get
            {
                return this._CaptchaWidth;
            }
            set
            {
                this._CaptchaWidth = value;
            }
        }

        /// <Summary>Gets and sets whether the Viewstate is enabled</Summary>
        [BrowsableAttribute( false )]
        public override bool EnableViewState
        {
            get
            {
                return base.EnableViewState;
            }
            set
            {
                base.EnableViewState = value;
            }
        }

        /// <Summary>
        /// Gets and sets the ErrorMessage to display if the control is invalid
        /// </Summary>
        [DescriptionAttribute( "The Error Message to display if invalid." ), CategoryAttribute( "Behavior" ), DefaultValueAttribute( "" )]
        public string ErrorMessage
        {
            get
            {
                return this._ErrorMessage;
            }
            set
            {
                this._ErrorMessage = value;
            }
        }

        /// <Summary>Gets the Style to use for the ErrorMessage</Summary>
        [TypeConverterAttribute( typeof( ExpandableObjectConverter ) ), BrowsableAttribute( true ), DescriptionAttribute( "Set the Style for the Error Message Control." ), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ), CategoryAttribute( "Appearance" )]
        public Style ErrorStyle
        {
            get
            {
                return this._ErrorStyle;
            }
        }

        /// <Summary>Gets and sets the Expiration time in seconds</Summary>
        [DescriptionAttribute( "The duration of time (seconds) a user has before the challenge expires." ), CategoryAttribute( "Behavior" ), DefaultValueAttribute( 120 )]
        public int Expiration
        {
            get
            {
                return this._Expiration;
            }
            set
            {
                this._Expiration = value;
            }
        }

        private bool IsDesignMode
        {
            get
            {
                return ( HttpContext.Current == null );
            }
        }

        /// <Summary>Gets whether the control is valid</Summary>
        [DescriptionAttribute( "Returns True if the user was CAPTCHA validated after a postback." ), CategoryAttribute( "Validation" )]
        public bool IsValid
        {
            get
            {
                return this._IsValid;
            }
        }

        /// <Summary>Gets and sets the Url to use to render the control</Summary>
        [CategoryAttribute( "Behavior" ), DescriptionAttribute( "The URL used to render the image to the client." ), DefaultValueAttribute( "ImageChallenge.captcha.aspx" )]
        public string RenderUrl
        {
            get
            {
                return this._RenderUrl;
            }
            set
            {
                this._RenderUrl = value;
            }
        }

        /// <Summary>Gets and sets the Help Text to use</Summary>
        [DefaultValueAttribute( "Enter the code shown above:" ), DescriptionAttribute( "Instructional text displayed next to CAPTCHA image." ), CategoryAttribute( "Captcha" )]
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                this._Text = value;
            }
        }

        

        /// <Summary>Creates the Image</Summary>
        /// <Param name="width">The width of the image</Param>
        /// <Param name="height">The height of the image</Param>
        private static Bitmap CreateImage( int width, int height )
        {
            Bitmap bmp = new Bitmap(width, height);
            Graphics g;
            Rectangle rect = new Rectangle(0, 0, width, height);
            RectangleF rectF = new RectangleF(0, 0, width, height);

            g = Graphics.FromImage(bmp);

            Brush b = new LinearGradientBrush(rect, Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)), Color.FromArgb(_Rand.Next(192), _Rand.Next(192), _Rand.Next(192)), Convert.ToSingle(_Rand.NextDouble()) * 360, false);
            g.FillRectangle(b, rectF);

            if (_Rand.Next(2) == 1)
            {
                DistortImage(ref bmp, _Rand.Next(5, 10));
            }
            else
            {
                DistortImage(ref bmp, -_Rand.Next(5, 10));
            }

            return bmp;
        }

        /// <Summary>Creates the Text</Summary>
        /// <Param name="text">The text to display</Param>
        /// <Param name="width">The width of the image</Param>
        /// <Param name="height">The height of the image</Param>
        private static GraphicsPath CreateText( string text, int width, int height, Graphics g )
        {
            GraphicsPath textPath = new GraphicsPath();
            FontFamily ff = GetFont();
            int emSize = Convert.ToInt32(width * 2 / text.Length);
            Font f = null;
            try
            {
                SizeF measured = new SizeF(0, 0);
                SizeF workingSize = new SizeF(width, height);
                while (emSize > 2)
                {
                    f = new Font(ff, emSize);
                    measured = g.MeasureString(text, f);
                    if (!(measured.Width > workingSize.Width || measured.Height > workingSize.Height))
                    {
                        break;
                    }
                    f.Dispose();
                    emSize -= 2;
                }
                emSize += 8;
                f = new Font(ff, emSize);

                StringFormat fmt = new StringFormat();
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;

                textPath.AddString(text, f.FontFamily, Convert.ToInt32(f.Style), f.Size, new RectangleF(0, 0, width, height), fmt);
                WarpText(ref  textPath, new Rectangle(0, 0, width, height));
            }
            catch (Exception)
            {
            }
            finally
            {
                f.Dispose();
            }

            return textPath;
        }

        /// <Summary>Decrypts the CAPTCHA Text</Summary>
        /// <Param name="encryptedContent">The encrypted text</Param>
        private static string Decrypt( string encryptedContent )
        {
            string decryptedText = string.Empty;
            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedContent);
                if (!ticket.Expired)
                {
                    decryptedText = ticket.UserData;
                }
            }
            catch (ArgumentException)
            {
            }

            return decryptedText;
        }

        /// <Summary>Encodes the querystring to pass to the Handler</Summary>
        private string EncodeTicket()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(CaptchaWidth.Value.ToString());
            sb.Append(_Separator + CaptchaHeight.Value.ToString());
            sb.Append(_Separator + _CaptchaText);
            sb.Append(_Separator + BackGroundImage);

            return sb.ToString();
        }

        /// <Summary>Encrypts the CAPTCHA Text</Summary>
        /// <Param name="content">The text to encrypt</Param>
        /// <Param name="expiration">The time the ticket expires</Param>
        private static string Encrypt( string content, DateTime expiration )
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, HttpContext.Current.Request.UserHostAddress, DateTime.Now, expiration, false, content);
            return FormsAuthentication.Encrypt(ticket);
        }

        /// <Summary>GenerateImage generates the Captch Image</Summary>
        /// <Param name="encryptedText">The Encrypted Text to display</Param>
        internal static Bitmap GenerateImage( string encryptedText )
        {
            string encodedText = Decrypt(encryptedText);

            string[] Settings = encodedText.Split(_Separator.ToCharArray()[0]);
            int width = int.Parse(Settings[0]);
            int height = int.Parse(Settings[1]);
            string text = Settings[2];
            string backgroundImage = Settings[3];

            Bitmap bmp;
            Graphics g;
            Brush b = new SolidBrush(Color.LightGray);
            Brush b1 = new SolidBrush(Color.Black);

            if (backgroundImage == "")
            {
                bmp = CreateImage(width, height);
            }
            else
            {
                bmp = (Bitmap)Bitmap.FromFile(HttpContext.Current.Request.MapPath(backgroundImage));
            }
            g = Graphics.FromImage(bmp);

            //Create Text
            GraphicsPath textPath = CreateText(text, width, height, g);
            if (backgroundImage == "")
            {
                g.FillPath(b, textPath);
            }
            else
            {
                g.FillPath(b1, textPath);
            }

            return bmp;
        }

        /// <Summary>GetFont gets a random font to use for the Captcha Text</Summary>
        private static FontFamily GetFont()
        {
            FontFamily _font = null;
            while (_font == null)
            {
                try
                {
                    _font = new FontFamily(_FontFamilies[_Rand.Next(_FontFamilies.Length)]);
                }
                catch (Exception)
                {
                    _font = null;
                }
            }
            return _font;
        }

        /// <Summary>Gets the next Captcha</Summary>
        protected virtual string GetNextCaptcha()
        {
            StringBuilder sb = new StringBuilder();
            Random _rand = new Random();
            int n;

            int intMaxLength = CaptchaChars.Length;

            for (n = 0; n <= CaptchaLength - 1; n++)
            {
                sb.Append(CaptchaChars.Substring(_rand.Next(intMaxLength), 1));
            }
            return sb.ToString();
        }

        /// <Summary>Builds the url for the Handler</Summary>
        private string GetUrl()
        {
            string url = ResolveUrl(RenderUrl);
            url += "?" + KEY + "=" + Encrypt(EncodeTicket(), DateTime.Now.AddSeconds(Expiration));
            return url;
        }

        /// <Summary>
        /// LoadPostData loads the Post Back Data and determines whether the value has change
        /// </Summary>
        /// <Param name="postDataKey">A key to the PostBack Data to load</Param>
        /// <Param name="postCollection">
        /// A name value collection of postback data
        /// </Param>
        public virtual bool LoadPostData( string postDataKey, NameValueCollection postCollection )
        {
            string returnedValue = postCollection[postDataKey];
            Validate(returnedValue);

            //Generate Random Challenge Text
            _CaptchaText = GetNextCaptcha();

            return false;
        }

        /// <summary>
        /// Generates a random point
        /// </summary>
        /// <param name="xmin">The minimum x value</param>
        /// <param name="xmax">The maximum x value</param>
        /// <param name="ymin">The minimum y value</param>
        /// <param name="ymax">The maximum y value</param>
        private static PointF RandomPoint( int xmin, int xmax, ref int ymin, ref int ymax )
        {
            return new PointF(_Rand.Next(xmin, xmax), _Rand.Next(ymin, ymax));
        }

        /// <Summary>Save the controls Voewstate</Summary>
        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] allStates = new object[3];
            allStates[0] = baseState;
            allStates[1] = _CaptchaText;
            return allStates;
        }

        /// <Summary>Validates the posted back data</Summary>
        /// <Param name="userData">The user entered data</Param>
        public bool Validate( string userData )
        {
            if (string.Compare(userData, this._CaptchaText, true) == 0)
            {
                _IsValid = true;
            }
            else
            {
                _IsValid = false;
            }
            if (UserValidatedEvent != null)
            {
                UserValidatedEvent(this, new ServerValidateEventArgs(_CaptchaText, _IsValid));
            }
            return _IsValid;
        }

        /// <Summary>Creates the child controls</Summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            if (this.CaptchaWidth.IsEmpty || this.CaptchaWidth.Type != UnitType.Pixel || this.CaptchaHeight.IsEmpty || this.CaptchaHeight.Type != UnitType.Pixel)
            {
                throw (new InvalidOperationException("Must specify size of control in pixels."));
            }

            _image = new Image();
            _image.BorderColor = this.BorderColor;
            _image.BorderStyle = this.BorderStyle;
            _image.BorderWidth = this.BorderWidth;
            _image.ToolTip = this.ToolTip;
            _image.EnableViewState = false;
            Controls.Add(_image);
        }

        /// <summary>
        /// DistortImage distorts the captcha image
        /// </summary>
        /// <param name="b">The Image to distort</param>
        private static void DistortImage( ref Bitmap b, double distortion )
        {
            int width = b.Width;
            int height = b.Height;

            Bitmap copy = (Bitmap)b.Clone();

            for (int y = 0; y <= height - 1; y++)
            {
                for (int x = 0; x <= width - 1; x++)
                {
                    int newX = Convert.ToInt32(x + (distortion * Math.Sin(Math.PI * y / 64.0)));
                    int newY = Convert.ToInt32(y + (distortion * Math.Cos(Math.PI * x / 64.0)));
                    if (newX < 0 || newX >= width)
                    {
                        newX = 0;
                    }
                    if (newY < 0 || newY >= height)
                    {
                        newY = 0;
                    }
                    b.SetPixel(x, y, copy.GetPixel(newX, newY));
                }
            }
        }

        /// <Summary>Loads the previously saved Viewstate</Summary>
        /// <Param name="savedState">The saved state</Param>
        protected override void LoadViewState( object savedState )
        {
            if (!(savedState == null))
            {
                // Load State from the array of objects that was saved at SaveViewState.
                object[] myState = (object[])savedState;

                //Load the ViewState of the Base Control
                if (!(myState[0] == null))
                {
                    base.LoadViewState(myState[0]);
                }

                //Load the CAPTCHA Text from the ViewState
                if (!(myState[1] == null))
                {
                    _CaptchaText = myState[1].ToString();
                }
            }
        }

        /// <Summary>Runs just before the control is to be rendered</Summary>
        protected override void OnPreRender( EventArgs e )
        {
            //Generate Random Challenge Text
            _CaptchaText = GetNextCaptcha();

            //Call Base Class method
            base.OnPreRender(e);
        }

        /// <Summary>
        /// RaisePostDataChangedEvent runs when the PostBackData has changed.
        /// </Summary>
        public virtual void RaisePostDataChangedEvent()
        {
        }

        /// <Summary>Render the  control</Summary>
        /// <Param name="writer">An Html Text Writer</Param>
        protected override void Render( HtmlTextWriter writer )
        {
            ControlStyle.AddAttributesToRender(writer);

            //Render outer <div> Tag
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            //Render image <img> Tag
            writer.AddAttribute(HtmlTextWriterAttribute.Src, GetUrl());
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            if (ToolTip.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, ToolTip);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();

            //Render Help Text
            if (Text.Length > 0)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(Text);
                writer.RenderEndTag();
            }

            //Render text box <input> Tag
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Style, "width:" + Width.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, _CaptchaText.Length.ToString());
            writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            if (AccessKey.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Accesskey, AccessKey);
            }
            if (!Enabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            if (TabIndex > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, TabIndex.ToString());
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            //Render error message
            if (!IsValid && Page.IsPostBack)
            {
                ErrorStyle.AddAttributesToRender(writer);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(ErrorMessage);
                writer.RenderEndTag();
            }

            //Render </div>
            writer.RenderEndTag();
        }

        /// <summary>
        /// Warps the Text
        /// </summary>
        /// <param name="textPath">The Graphics Path for the text</param>
        /// <param name="rect">a rectangle which defines the image</param>
        private static void WarpText( ref GraphicsPath textPath, Rectangle rect )
        {
            int intWarpDivisor;
            RectangleF rectF = new RectangleF(0, 0, rect.Width, rect.Height);

            intWarpDivisor = _Rand.Next(4, 8);

            int intHrange = Convert.ToInt32(rect.Height / intWarpDivisor);
            int intWrange = Convert.ToInt32(rect.Width / intWarpDivisor);
            int zero = 0;

            PointF p1 = RandomPoint(0, intWrange, ref zero, ref intHrange);
            PointF p2 = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p1.X)), rect.Width, ref zero, ref intHrange);
            int half = rect.Height - (intHrange - Convert.ToInt32(p1.Y));
            int h = rect.Height;
            PointF p3 = RandomPoint(0, intWrange, ref half, ref h);
            int y = rect.Height - (intHrange - Convert.ToInt32(p2.Y));
            PointF p4 = RandomPoint(rect.Width - (intWrange - Convert.ToInt32(p3.X)), rect.Width, ref y, ref h);

            PointF[] points = new PointF[] { p1, p2, p3, p4 };
            Matrix m = new Matrix();
            m.Translate(0, 0);
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0);
        }
    }
}