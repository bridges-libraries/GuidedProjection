using System;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Variables
{
    /// <summary>
    /// Exposure of the current subcategory tabs.
    /// </summary>
    enum TabExposure
    {
        Construct = GH_Kernel.GH_Exposure.primary,
        Deconstruct = GH_Kernel.GH_Exposure.secondary,
    }
}
 