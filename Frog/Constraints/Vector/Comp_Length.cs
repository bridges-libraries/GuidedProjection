using System;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_V = Frog.Types.Variables;
using Types_C = Frog.Types.Constraints;
using Params_V = Frog.Parameters.Variables;
using Params_C = Frog.Parameters.Constraints;


namespace Frog.Constraints.Vector
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Vector.Length"/>-based <see cref="GP.Constraint"/>..
    /// </summary>
    public class Comp_Length : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_VectorLength"/> class.
        /// </summary>
        public Comp_Length()
          : base("Vector - Length", "Length",
              "Constraint enforcing the length a vector variable to be equal to a fixed value.",
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

            pManager.AddParameter(new Params_V.Param_IVariable(), "Vector Variable", "V", "Variable representing a vector whose length should equal a fixed value.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Target Length", "L", "Fixed target length of the vector variable.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Parameters.Weights.Param_Weight(), "Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item);

            // Set default value of the weight 
            Parameters.Weights.Param_Weight param = (Parameters.Weights.Param_Weight)pManager[3];
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

            Types_V.IGH_GooVariable? gh_Vector = null;
            double length = 0d;

            double weight = 0d;


            //     -----     Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; }

            if (!DA.GetData(1, ref gh_Vector)) { return; };
            if (!DA.GetData(2, ref length)) { return; };

            if (!DA.GetData(3, ref weight)) { weight = 1d; };

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Vector, "The input V is null");


            //     -----     Core     -----     //

            int dimension = gh_Vector.Size;

            // Create the list of variables
            Types_V.IGH_GooVariable[] gh_Variables = new Types_V.IGH_GooVariable[1] { gh_Vector };

            // Create the constraint type
            GP.ConstraintTypes.Vector.Length constraintType = new GP.ConstraintTypes.Vector.Length(length, dimension);
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

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{3925C384-0698-405D-9C77-ACD0A85AC720}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Vector;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
