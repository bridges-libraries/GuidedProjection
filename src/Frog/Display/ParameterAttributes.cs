using System;
using System.Drawing;

using GH_Kernel = Grasshopper.Kernel;
using GH_Gui = Grasshopper.GUI.Canvas;


namespace Frog.Display
{
    /// <summary>
    /// Class controling the attributes of a <see cref="GH_Kernel.GH_Param{T}"/>
    /// </summary>
    public class ParameterAttributes : GH_Kernel.Attributes.GH_FloatingParamAttributes
    {        
        #region Constructors

        /// <summary>
        /// Class defining the attributes of an <see cref="GH_Kernel.IGH_Param"/>
        /// </summary>
        /// <param name="param"> Parameter whose attribute to modify. </param>
        public ParameterAttributes(GH_Kernel.IGH_Param param)
            : base(param)
        {
            // Do Nothing
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Replaces the color palettes with custom ones.
        /// </summary>
        /// <param name="defaultPalettes"> The three default palettes : Normal, Hidden, Locked. </param>
        public static void ReplaceColorPalettes(out GH_Gui.GH_PaletteStyle[] defaultPalettes)
        {
            // Cache the default style.
            defaultPalettes = new GH_Gui.GH_PaletteStyle[3];
            defaultPalettes[0] = GH_Gui.GH_Skin.palette_normal_standard;
            defaultPalettes[1] = GH_Gui.GH_Skin.palette_hidden_standard;
            defaultPalettes[2] = GH_Gui.GH_Skin.palette_locked_standard;

            // Replace the palettes with custom ones.
            GH_Gui.GH_Skin.palette_normal_standard = new GH_Gui.GH_PaletteStyle(Colors.Cyan, Color.Black, Color.Black);
            GH_Gui.GH_Skin.palette_hidden_standard = new GH_Gui.GH_PaletteStyle(Colors.Cyan_Hidden, Color.Black, Color.Black);
            GH_Gui.GH_Skin.palette_locked_standard = new GH_Gui.GH_PaletteStyle(Color.SlateGray, Color.Black, Color.Black);
        }

        /// <summary>
        /// Reset the color palettes to the default ones.
        /// </summary>
        /// <param name="defaultPalettes"> The three default palettes : Normal, Hidden, Locked </param>
        public static void ResetColorPalettes(GH_Gui.GH_PaletteStyle[] defaultPalettes)
        {
            // Put the original style back.
            GH_Gui.GH_Skin.palette_normal_standard = defaultPalettes[0];
            GH_Gui.GH_Skin.palette_hidden_standard = defaultPalettes[1];
            GH_Gui.GH_Skin.palette_locked_standard = defaultPalettes[2];
        }

        #endregion


        #region Override : GH_Ker.Attributes.GH_FloatingParamAttributes

        /// <summary>
        /// Method defining the render settings of the parameter.
        /// </summary>
        /// <param name="canvas"> Canvas in which the parameter is displayed. </param>
        /// <param name="graphics"> Graphic controls. </param>
        /// <param name="channel"> Drawing channels handled inside the Grasshopper canvas.</param>
        protected override void Render(GH_Gui.GH_Canvas canvas, Graphics graphics, GH_Gui.GH_CanvasChannel channel)
        {
            if (channel == GH_Gui.GH_CanvasChannel.Objects)
            {
                // Replace the color palettes of Grasshopper
                ReplaceColorPalettes(out GH_Gui.GH_PaletteStyle[] defaultPalettes);

                // Create the common component render
                base.Render(canvas, graphics, channel);

                // Reset the color palettes of Grasshopper
                ResetColorPalettes(defaultPalettes);
            }
            else
            {
                // Create the common component render
                base.Render(canvas, graphics, channel);
            }
        }

        #endregion
    }
}
