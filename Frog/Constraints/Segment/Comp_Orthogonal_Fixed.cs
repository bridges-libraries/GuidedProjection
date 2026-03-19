using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Types_Var = Frog.Types.Variables;
using Types_Cstr = Frog.Types.Constraints;
using Params_Var = Frog.Parameters.Variables;
using Params_Cstr = Frog.Parameters.Constraints;


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
            // Do Nothing
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

            Types_Var.Gh_IVariable start = new Types_Var.Gh_IVariable();
            Types_Var.Gh_IVariable end = new Types_Var.Gh_IVariable();

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

            if (start.Value.Size != end.Value.Size) { throw new ArgumentException("The input variables S and E don't have the same size."); }
            if (start.Value.Size > 3) { throw new ArgumentException("The size of the input variable S must be lower or equal to 3."); }


            //     -----     Core     -----    //

            int dimension = start.Value.Size;
            double[] components = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                components[i] = vector3d[i];
            }

            // Create the list of variables
            Types_Var.Gh_IVariable[] gh_Variables = new Types_Var.Gh_IVariable[2] { start, end };

            // Create the constraint type
            GP.ConstraintTypes.Segment.Orthogonal_Fixed constraintType = new GP.ConstraintTypes.Segment.Orthogonal_Fixed(components);
            Types_Cstr.Gh_IConstraintType gh_ConstraintType = new Types_Cstr.Gh_IConstraintType(constraintType);

            // Create the constraint
            Types_Cstr.Gh_Constraint gh_Constraint = new Types_Cstr.Gh_Constraint(gh_Variables, gh_ConstraintType, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, gh_Constraint);
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
