using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace DotNetNuke.UI.WebControls
{
    /// <Summary>
    /// The CaptchaHandler control provides a validator to validate a CAPTCHA Challenge
    /// </Summary>
    public class CaptchaHandler : IHttpHandler
    {
        private const int MAX_IMAGE_HEIGHT = 600;
        private const int MAX_IMAGE_WIDTH = 600;

        public virtual bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public virtual void ProcessRequest( HttpContext context )
        {
            NameValueCollection queryString = context.Request.QueryString;
            string text = queryString[CaptchaControl.KEY];
            HttpResponse response = context.Response;
            Bitmap bmp = CaptchaControl.GenerateImage(text);
            bmp.Save(response.OutputStream, ImageFormat.Jpeg);
        }
    }
}