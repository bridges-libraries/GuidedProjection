using System;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Results
{
    /// <summary>
    /// Exposure of the current subcategory tabs.
    /// </summary>
    enum TabExposure
    {
        Variables = GH_Kernel.GH_Exposure.primary,
        Constraints = GH_Kernel.GH_Exposure.secondary,
        Energies = GH_Kernel.GH_Exposure.tertiary,
    }
}
 