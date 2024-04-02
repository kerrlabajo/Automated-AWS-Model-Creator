using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    class Utils
    {
        public static Control GetControlNamed(Control ParentControl, string name, bool searchAllChildren)
        {
            Control control = null;
            Control[] allControls = ParentControl.Controls.Find(name, searchAllChildren);

            if (allControls.Length > 0)
            {
                control = allControls[0];
            }

            return control;
        }
    }
}
