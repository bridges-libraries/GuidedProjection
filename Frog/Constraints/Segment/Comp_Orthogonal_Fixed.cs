using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Types_V = Frog.Types.Variables;
using Types_C = Frog.Types.Constraints;
using Params_V = Frog.Parameters.Variables;
using Params_C = Frog.Parameters.Constraints;


namespace Frog.Constraints.Segment
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Segment.Orthogonal_Fixed"/>-based <see cref="GP.Constraint"/>.
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
            pManager.AddTextParameter("Name", "N", "Name of the constraint.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Params_V.Param_IVariable(), "Start Variable", "S", "Variable representing the start of the segment.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddParameter(new Params_V.Param_IVariable(), "End Variable", "E", "Variable representing the end of the segment.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddVectorParameter("Vector", "V", "Fixed vector to which the segment must be orthogonal.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Parameters.Weights.Param_Weight(), "Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item);

            // Set default value of the weight 
            Parameters.Weights.Param_Weight param = (Parameters.Weights.Param_Weight)pManager[4];
            param.PersistentData.Append(new Types.Weights.Gh_Weight(1.0));
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_C.Param_Constraint(), "Constraint", "C", "Newly created constraint.", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            string name = string.Empty;

            Types_V.IGH_GooVariable? gh_StartPoint = null;
            Types_V.IGH_GooVariable? gh_EndPoint = null;

            Rhino.Geometry.Vector3d vector3d = new Rhino.Geometry.Vector3d(0d, 0d, 0d);

            double weight = 0d;


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; }

            if (!DA.GetData(1, ref gh_StartPoint)) { return; }
            if (!DA.GetData(2, ref gh_EndPoint)) { return; }
            if (!DA.GetData(3, ref vector3d)) { return; }

            if (!DA.GetData(4, ref weight)) { return; }

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(gh_StartPoint, "The input S is null");
            ArgumentNullException.ThrowIfNull(gh_EndPoint, "The input E is null");

            if (gh_StartPoint.Size != gh_EndPoint.Size) { throw new ArgumentException("The input variables S and E don't have the same size."); }
            if (gh_StartPoint.Size > 3) { throw new ArgumentException("The size of the input variable S must be lower or equal to 3."); }


            //     -----     Core     -----    //

            int dimension = gh_StartPoint.Size;
            double[] coordinates = new double[dimension];
            for (int i = 0; i < dimension; i++)
            {
                coordinates[i] = vector3d[i];
            }

            // Create the list of variables
            Types_V.IGH_GooVariable[] gh_Variables = new Types_V.IGH_GooVariable[2] { gh_StartPoint, gh_EndPoint };

            // Create the constraint type
            GP.ConstraintTypes.Segment.Orthogonal_Fixed constraintType = new GP.ConstraintTypes.Segment.Orthogonal_Fixed(coordinates);
            Types_C.Gh_IConstraintType gh_ConstraintType = new Types_C.Gh_IConstraintType(constraintType);

            // Create the constraint
            Types_C.Gh_Constraint gh_Constraint = new Types_C.Gh_Constraint(gh_Variables, gh_ConstraintType, weight, name);


            //     -----     Set Output     -----     //

            DA.SetData(0, gh_Constraint);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Display.ComponentAttributes(this);
        }

        #endregion

        #region Override : GH_DocumentObject

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{9FC632E5-85B5-4857-84D8-B1B22FE7D7FC}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Segment;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
