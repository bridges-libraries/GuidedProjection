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
        /// <param name="variables"> Variables composing the local vector localX on which the local symmetric matrix Hi and the local vector Bi are defined. </param>
        public abstract void UpdateMembers(IReadOnlyList<IVariable> variables);

        #endregion
    }
}
