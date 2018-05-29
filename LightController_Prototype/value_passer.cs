using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace LightController_Prototype
{
    public static class FormValues
    {
        private static Color _message;
        public static Color Message
        {
            get { return _message; }
            set
            {
                // Do stuff with value ...
                // Handle any errors with the value ...
                // Throwing an exception here will tell you which form it was thrown at too ...
                _message = value;
            }
        }
    }
}
