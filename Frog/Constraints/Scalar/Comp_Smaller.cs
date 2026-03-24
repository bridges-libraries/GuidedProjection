using System;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Types_Var = Frog.Types.Variables;
using Types_Cstr = Frog.Types.Constraints;
using Params_Var = Frog.Parameters.Variables;
using Params_Cstr = Frog.Parameters.Constraints;


namespace Frog.Constraints.Scalar
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.ConstraintTypes.Scalar.Smaller"/>-based <see cref="GP.Constraint"/>..
    /// </summary>
    public class Comp_Smaller : GH_Kernel.GH_Component
    {
        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Smaller"/> class.
        /// </summary>
        public Comp_Smaller()
          : base("Smaller", "Smaller",
              "Constraint enforcing a scalar variable to be smaller than a fixed value.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Constraints])
        {
            // Do Nothing
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Scalar Variable", "V", "Scalar variable whose value should be smaller than a fixed value.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Upper Bound", "B", "Upper Bound of the scalar variable.", GH_Kernel.GH_ParamAccess.item);

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

            Types_Var.Gh_IVariable gh_Scalar = new Types_Var.Gh_IVariable();
            double bound = 0d;

            double weight = 0d;


            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref gh_Scalar)) { return; }
            if (!DA.GetData(1, ref bound)) { return; }

            if (!DA.GetData(2, ref weight)) { return; }

            //    ----    Verifications

            ArgumentNullException.ThrowIfNull(gh_Scalar, "The input V is null");
            if (gh_Scalar.Size != 1) { throw new ArgumentException("The input V is not a scalar variable."); }


            //     -----     Core     -----    //

            // Create a dummy variable
            double dummyValue = 0;
            if (gh_Scalar.Value is Types_Var.Gh_Variable gh_ScalarVar) { dummyValue = gh_ScalarVar[0] - bound;}
            else if (gh_Scalar.Value is Types_Var.Gh_SubVariable gh_ScalarSubVar) { dummyValue = gh_ScalarSubVar[0] - bound;}
            else { throw new InvalidCastException("The input V could not be casted to a variable or a subvariable"); }

            dummyValue = dummyValue > 0 ? Math.Sqrt(dummyValue) : Math.Sqrt(-dummyValue);

            double[] coordinates = new double[1] { dummyValue };
            Types_Var.Gh_Variable gh_Dummy = new Types_Var.Gh_Variable(coordinates, "Dummy - Smaller", -1, Types_Var.RhinoDataType.Scalar);


            // Create the list of variables
            Types_Var.Gh_IVariable[] gh_Variables = new Types_Var.Gh_IVariable[2] ;
            gh_Variables[0] = gh_Scalar;
            gh_Variables[1] = new Types_Var.Gh_IVariable(gh_Dummy);


            // Create the constraint type
            GP.ConstraintTypes.Scalar.Smaller constraintType = new GP.ConstraintTypes.Scalar.Smaller(bound);
            Types_Cstr.Gh_IConstraintType gh_ConstraintType = new Types_Cstr.Gh_IConstraintType(constraintType);


            // Create the constraint
            Types_Cstr.Gh_Constraint gh_Constraint = new Types_Cstr.Gh_Constraint(gh_Variables, gh_ConstraintType, weight);


            //     -----     Set Output     -----     //

            DA.SetData(0, gh_Constraint);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{B9CFDFFD-E490-4FE1-9A3D-94C29BAF9C93}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Scalar;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
