using System;
using System.Collections.Generic;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Interface for grasshopper types representing a <see cref="GP.Interfaces.IVariable"/>.
    /// </summary>
    public interface IGH_GooVariable : GH_Types.IGH_Goo
    {
        #region Properties

        /// <summary>
        /// Gets the size of the represented <see cref="GP.Interfaces.IVariable"/>.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Gets the coordinate value at the given index of the represented <see cref="GP.Interfaces.IVariable"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index] { get; }

        #endregion
    }
}
