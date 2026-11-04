using System;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_V = Frog.Types.Variables;
using Types_W = Frog.Types.Weights;
using Types_C = Frog.Types.Constraints;
using Params_V = Frog.Parameters.Variables;
using Params_W = Frog.Parameters.Weights;
using Params_C = Frog.Parameters.Constraints;


namespace Frog.Constraints.Scalar
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Scalar.Equal"/>-based <see cref="GP.Constraint"/>.
    /// </summary>
    public class Comp_Equal : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Equal"/> class.
        /// </summary>
        public Comp_Equal()
          : base("Equal", "Equal",
              "Constraint enforcing a scalar variable to be equal to a fixed value.",
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

            pManager.AddParameter(new Params_V.Param_IVariable(), "Scalar Variable", "V", "Variable whose value should be equal to a target value.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Target Value", "T", "Fixed target value for the variable.", GH_Kernel.GH_ParamAccess.item);

            pManager.AddParameter(new Params_W.Param_Weight(), "Weight", "W", "Weight of the constraint.", GH_Kernel.GH_ParamAccess.item);

            // Set default value of the weight 
            Params_W.Param_Weight param = (Params_W.Param_Weight)pManager[3];
            param.PersistentData.Append(new Types_W.Gh_Weight(1.0));
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

            Types_V.IGH_GooVariable? gh_Scalar = null;
            double target = 0d;

            Types_W.Gh_Weight? gh_Weight = null;


            //     -----     Get Inputs     -----     //

            if (!DA.GetData(0, ref name)) { return; }

            if (!DA.GetData(1, ref gh_Scalar)) { return; }
            if (!DA.GetData(2, ref target)) { return; }

            if (!DA.GetData(3, ref gh_Weight)) { return; }


            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(gh_Scalar, "The input V is null");
            if (gh_Scalar.Size != 1) { throw new ArgumentException("The input V is not a scalar variable."); }

            ArgumentNullException.ThrowIfNull(gh_Weight, "The input W is null");


            //     -----     Core     -----     //

            // Create the list of variables
            Types_V.IGH_GooVariable[] gh_Variables = new Types_V.IGH_GooVariable[1] { gh_Scalar };

            // Create the constraint type
            GP.ConstraintTypes.Scalar.Equal constraintType = new GP.ConstraintTypes.Scalar.Equal(target);
            Types_C.Gh_IConstraintType gh_ConstraintType = new Types_C.Gh_IConstraintType(constraintType);

            // Create the constraint
            Types_C.Gh_Constraint gh_Constraint = new Types_C.Gh_Constraint(gh_Variables, gh_ConstraintType, gh_Weight, name);


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
        public override Guid ComponentGuid => new Guid("{7D10E61F-BD86-4DB7-A286-9266EE723ED5}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Scalar;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
