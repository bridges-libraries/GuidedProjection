using System;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Variables
{
    /// <summary>
    /// Exposure of the current subcategory tabs.
    /// </summary>
    enum TabExposure
    {
        PreTreatment = GH_Kernel.GH_Exposure.primary,
        PostTreatment = GH_Kernel.GH_Exposure.secondary,
    }
}
 