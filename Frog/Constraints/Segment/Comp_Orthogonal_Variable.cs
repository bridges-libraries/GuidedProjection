using System;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_Var = Frog.Types.Variables;
using Types_Cstr = Frog.Types.Constraints;
using Params_Var = Frog.Parameters.Variables;
using Params_Cstr = Frog.Parameters.Constraints;


namespace Frog.Constraints.Segment
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Segment.Orthogonal_Variable"/>-based <see cref="GP.Energy"/>.
    /// </summary>
    public class Comp_Orthogonal_Variable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_SegmentOrthogonality"/> class.
        /// </summary>
        public Comp_Orthogonal_Variable()
          : base("Segment - Orthogonal (Var)", "Ortho. (V)",
              "Constraint enforcing a segment (defined from two point variables) to be orthogonal to a vector variable.",
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
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Vector", "V", "Variable representing the vector to which the segment must be orthogonal.", GH_Kernel.GH_ParamAccess.item);

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
            Types_Var.Gh_IVariable normal = new Types_Var.Gh_IVariable();

            double weight = 0d;


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref start)) { return; }
            if (!DA.GetData(1, ref end)) { return; }
            if (!DA.GetData(2, ref normal)) { return; }

            if (!DA.GetData(3, ref weight)) { weight = 1.0; };

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(start, "The input S is null");
            ArgumentNullException.ThrowIfNull(end, "The input E is null");
            ArgumentNullException.ThrowIfNull(normal, "The input V is null");

            if (start.Size != end.Size) { throw new ArgumentException("The input variables S and E don't have the same size."); }
            if (start.Size != normal.Size) { throw new ArgumentException("The input variables S and V don't have the same size."); }


            //     -----     Core     -----    //

            int dimension = normal.Size;

            // Create the list of variables
            Types_Var.Gh_IVariable[] gh_Variables = new Types_Var.Gh_IVariable[3] { start, end, normal };

            // Create the constraint type
            GP.ConstraintTypes.Segment.Orthogonal_Variable constraintType = new GP.ConstraintTypes.Segment.Orthogonal_Variable(dimension);
            Types_Cstr.Gh_IConstraintType gh_ConstraintType = new Types_Cstr.Gh_IConstraintType(constraintType);

            // Create the constraint
            Types_Cstr.Gh_Constraint gh_Constraint = new Types_Cstr.Gh_Constraint(gh_Variables, gh_ConstraintType, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, gh_Constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{EEFF6016-F2B7-4A66-B798-D419CB3F97F4}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Segment;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
