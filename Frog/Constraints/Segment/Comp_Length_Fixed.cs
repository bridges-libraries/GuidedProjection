using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Params_Var = Frog.Parameters.Variables;
using Params_Cstr = Frog.Parameters.Constraints;


namespace Frog.Constraints.Segment
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Segment.Length_Fixed"/>-based <see cref="GP.Constraint"/>..
    /// </summary>
    public class Comp_Length_Fixed : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Length_Fixed"/> class.
        /// </summary>
        public Comp_Length_Fixed()
          : base("Segment - Length (Fix)", "Length (F)",
              "Constraint enforcing the length of a segment (defined from two point variables) to be equal to a fixed value.",
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
            pManager.AddNumberParameter("Target Length", "L", "Target length of the segment.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddNumberParameter("Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item, 1.0);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_Cstr.Param_Constraint(), "Constraint", "C", "Constraint", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            GP.Interfaces.IVariable? start = null;
            GP.Interfaces.IVariable? end = null;
            double length = 0d;

            double weight = 0d;


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref start)) { return; }
            if (!DA.GetData(1, ref end)) { return; }
            if (!DA.GetData(2, ref length)) { return; }

            if (!DA.GetData(3, ref weight)) { return; }

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(start, "The input S is null");
            ArgumentNullException.ThrowIfNull(end, "The input E is null");

            if (start.Size != end.Size) { throw new ArgumentException("The input variables S and E must have the same size."); }


            //     -----     Core     -----    //

            int dimension = start.Size;
            GP.Interfaces.IVariable[] variables = new GP.Interfaces.IVariable[2] { start, end };

            GP.ConstraintTypes.Segment.Length_Fixed constraintType = new GP.ConstraintTypes.Segment.Length_Fixed(length, dimension);
            GP.Constraint constraint = new GP.Constraint(constraintType, variables, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{82CC4A44-328C-4D4A-9C29-8D09290F68D0}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Segment;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon => null;

        #endregion
    }
}
