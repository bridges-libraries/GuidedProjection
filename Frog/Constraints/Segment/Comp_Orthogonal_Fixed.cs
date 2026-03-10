using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Params_Cstr = Frog.Parameters.Constraints;
using Params_Var = Frog.Parameters.Variables;


namespace Frog.Constraints.Segment
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Segment.Orthogonal_Fixed"/>-based <see cref="GP.Energy"/>.
    /// </summary>
    public class Comp_Orthogonal_Fixed : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Orthogonal_Fixed"/> class.
        /// </summary>
        public Comp_Orthogonal_Fixed()
          : base("Segment - Orthogonal (Fix)", "Ortho Dir. (F)",
              "Constraint enforcing a segment (defined from two point variables) to be orthogonal to a fixed direction.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Constraints])
        {
            /* Do Nothing */
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Start Variable", "S", "Variable representing the start of the segment.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Params_Var.Param_IVariable(), "End Variable", "E", "Variable representing the end of the segment.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddVectorParameter("Vector", "V", "Vector to which the segment must be orthogonal.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddNumberParameter("Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item, 1.0);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_Cstr.Param_Constraint(), "Constraint", "C", "Constraint.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            GP.Interfaces.IVariable? start = null;
            GP.Interfaces.IVariable? end = null;

            Rhino.Geometry.Vector3d vector3d = new Rhino.Geometry.Vector3d(0d, 0d, 0d);

            double weight = 0d;


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref start)) { return; }
            if (!DA.GetData(1, ref end)) { return; }
            if (!DA.GetData(2, ref vector3d)) { return; }

            if (!DA.GetData(3, ref weight)) { return; }

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(start, "The input S is null");
            ArgumentNullException.ThrowIfNull(end, "The input E is null");

            if (start.Size != end.Size) { throw new ArgumentException("The input variables S and E must have the same size."); }
            if (start.Size > 3) { throw new ArgumentException("The size of the input variable S must be lower or equal to 3."); }


            //     -----     Core     -----    //

            int dimension = start.Size;
            double[] components = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                components[i] = vector3d[i];
            }


            GP.ConstraintTypes.Segment.Orthogonal_Fixed constraintType = new GP.ConstraintTypes.Segment.Orthogonal_Fixed(components);

            GP.Interfaces.IVariable[] variables = new GP.Interfaces.IVariable[2] { start, end };

            GP.Constraint constraint = new GP.Constraint(constraintType, variables, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{9FC632E5-85B5-4857-84D8-B1B22FE7D7FC}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Segment;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;

        #endregion
    }
}
