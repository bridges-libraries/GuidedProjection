using System;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Parameters
{
    /// <summary>
    /// Exposure of the current subcategory tabs.
    /// </summary>
    enum TabExposure
    {
        Variables = GH_Kernel.GH_Exposure.primary,
        Weights = GH_Kernel.GH_Exposure.secondary,
        Constraints = GH_Kernel.GH_Exposure.tertiary,
        Models = GH_Kernel.GH_Exposure.quarternary
    }
}
