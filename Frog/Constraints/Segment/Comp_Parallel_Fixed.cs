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
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Segment.Direction"/>-based <see cref="GP.Energy"/>.
    /// </summary>
    public class Comp_Parallel_Fixed : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Parallel_Fixed"/> class.
        /// </summary>
        public Comp_Parallel_Fixed()
          : base("Segment - Parallel (Fix)", "Parallel (F)",
              "Constraint enforcing a segment (defined from two point variables) to be parallel to a fixed direction.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Constraints])
        {
            // Do Nothing
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the constraint.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Params_Var.Param_IVariable(), "Start Variable", "S", "Variable representing the start of the segment.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Params_Var.Param_IVariable(), "End Variable", "E", "Variable representing the end of the segment.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Length Variable", "L", "Variable representing the distance between the start and the end of the segment", GH_Kernel.GH_ParamAccess.item);

            pManager.AddVectorParameter("Vector", "V", "Fixed vector to which the segment must be aligned.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddNumberParameter("Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item, 1.0);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_Cstr.Param_Constraint(), "Constraint", "C", "Newly created constraint.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            string name = string.Empty;

            Types_Var.IGH_GooVariable? gh_StartPoint = null;
            Types_Var.IGH_GooVariable? gh_EndPoint = null;
            Types_Var.IGH_GooVariable? gh_Length = null;

            Rhino.Geometry.Vector3d vector3d = new Rhino.Geometry.Vector3d(0d, 0d, 0d);

            double weight = 0d;


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; }

            if (!DA.GetData(1, ref gh_StartPoint)) { return; }
            if (!DA.GetData(2, ref gh_EndPoint)) { return; }
            if (!DA.GetData(3, ref gh_Length)) { return; }

            if (!DA.GetData(4, ref vector3d)) { return; }

            if (!DA.GetData(5, ref weight)) { return; }

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(gh_StartPoint, "The input S is null");
            ArgumentNullException.ThrowIfNull(gh_EndPoint, "The input E is null");
            ArgumentNullException.ThrowIfNull(gh_Length, "The input L is null");

            if (gh_Length.Size != 1) { throw new ArgumentException("The input L is not a scalar variable."); }

            if (gh_StartPoint.Size != gh_EndPoint.Size) { throw new ArgumentException("The input variables S and E don't have the same size."); }
            if (gh_StartPoint.Size > 3) { throw new ArgumentException("The size of the input variable S don't be lower or equal to 3."); }


            //     -----     Core     -----    //

            int dimension = gh_StartPoint.Size;
            double[] components = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                components[i] = vector3d[i];
            }

            // Create the list of variables
            Types_Var.IGH_GooVariable[] gh_Variables = new Types_Var.IGH_GooVariable[3] { gh_StartPoint, gh_EndPoint, gh_Length };

            // Create the constraint type
            GP.ConstraintTypes.Segment.Direction constraintType = new GP.ConstraintTypes.Segment.Direction(components);
            Types_Cstr.Gh_IConstraintType gh_ConstraintType = new Types_Cstr.Gh_IConstraintType(constraintType);

            // Create the constraint
            Types_Cstr.Gh_Constraint gh_Constraint = new Types_Cstr.Gh_Constraint(gh_Variables, gh_ConstraintType, weight, name);


            //     -----     Set Output     -----     //

            DA.SetData(0, gh_Constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{4F5A0AE6-807B-4A65-A5E1-A7C848022A03}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Segment;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
