using System;
using System.Drawing;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Display
{
    /// <summary>
    /// Colour palette for the render of <see cref="GH_Kernel.IGH_Param"/> and <see cref="GH_Kernel.IGH_Component"/> in Frog.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// Cyan colour, for the normal state.
        /// </summary>
        public static Color Cyan = ColorTranslator.FromHtml("#47B3D8");

        /// <summary>
        /// Cyan colour, for the hidden state.
        /// </summary>
        public static Color Cyan_Hidden = ColorTranslator.FromHtml("#7A9AA5");
    }
}
