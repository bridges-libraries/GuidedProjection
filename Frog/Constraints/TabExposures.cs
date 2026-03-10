using System;

using GH_Kernel = Grasshopper.Kernel;


namespace Frog.Constraints
{
    /// <summary>
    /// Exposure of the current subcategory tabs.
    /// </summary>
    enum TabExposure
    {
        Scalar = GH_Kernel.GH_Exposure.primary,
        Vector = GH_Kernel.GH_Exposure.secondary,
        Segment = GH_Kernel.GH_Exposure.tertiary,
        Others = GH_Kernel.GH_Exposure.octonary
    }
}
 