using System;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_V = Frog.Types.Variables;
using Types_C = Frog.Types.Constraints;
using Params_V = Frog.Parameters.Variables;
using Params_C = Frog.Parameters.Constraints;


namespace Frog.Constraints.Segment
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Segment.Length_Variable"/>-based <see cref="GP.Constraint"/>..
    /// </summary>
    public class Comp_Length_Variable : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Length_Variable"/> class.
        /// </summary>
        public Comp_Length_Variable()
          : base("Segment - Length (Var)", "Length (V)",
              "Constraint enforcing the length of a segment (defined from two point variables) to be equal to a scalar variable.",
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
            pManager.AddParameter(new Params_V.Param_IVariable(), "Length Variable", "L", "Variable representing the distance between the start and the end of the segment.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Parameters.Weights.Param_Weight(), "Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item);

            // Set default value of the weight 
            Parameters.Weights.Param_Weight param = (Parameters.Weights.Param_Weight)pManager[4];
            param.PersistentData.Append(new Types.Weights.Gh_Weight(1.0));
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Params_C.Param_Constraint(), "Constraint", "C", "Newly created constraint", GH_Kernel.GH_ParamAccess.item);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            string name = string.Empty;

            Types_V.IGH_GooVariable? gh_StartPoint = null;
            Types_V.IGH_GooVariable? gh_EndPoint = null;
            Types_V.IGH_GooVariable? gh_Length = null;

            double weight = 0d;


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; }

            if (!DA.GetData(1, ref gh_StartPoint)) { return; }
            if (!DA.GetData(2, ref gh_EndPoint)) { return; }
            if (!DA.GetData(3, ref gh_Length)) { return; }

            if (!DA.GetData(4, ref weight)) { return; }

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(gh_StartPoint, "The input S is null");
            ArgumentNullException.ThrowIfNull(gh_EndPoint, "The input E is null");
            ArgumentNullException.ThrowIfNull(gh_Length, "The input L is null");

            if (gh_StartPoint.Size != gh_EndPoint.Size) { throw new ArgumentException("The input variables S and E don't have the same size."); } 


            //     -----     Core     -----    //

            int dimension = gh_StartPoint.Size;

            // Create the list of variables
            Types_V.IGH_GooVariable[] gh_Variables = new Types_V.IGH_GooVariable[3] { gh_StartPoint, gh_EndPoint, gh_Length };

            // Create the constraint type
            GP.ConstraintTypes.Segment.Length_Variable constraintType = new GP.ConstraintTypes.Segment.Length_Variable(dimension);
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

        // ---------- Properties ---------- //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{DD82A6D7-ABFA-48A1-9A86-858015DA2A2D}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Segment;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion          
    }
}
