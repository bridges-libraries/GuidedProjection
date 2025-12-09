using System;
using System.Collections.Generic;


namespace BRIDGES.Solvers.GuidedProjection.Interfaces
{
    /// <summary>
    /// Abstract class defining a linearised constraint type.
    /// </summary>
    public interface IUpdatedConstraintType : IConstraintType
    {
        #region Methods

        /// <summary>
        /// Updates the members (i.e. LocalHi and LocalBi and Ci) of the constraint using auxiliary variables or auxiliary objects.
        /// </summary>
        public abstract void UpdateMembers();

        #endregion
    }
}
