using System;

using GH_Kernel = Grasshopper.Kernel;

using GP = BRIDGES.Solvers.GuidedProjection;

using Types_Var = Frog.Types.Variables;
using Types_Cstr = Frog.Types.Constraints;
using Params_Var = Frog.Parameters.Variables;
using Params_Cstr = Frog.Parameters.Constraints;


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
            pManager.AddParameter(new Params_Var.Param_IVariable(), "Vector Variable", "V", "Variable representing a vector whose length should equal a fixed value.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddNumberParameter("Target Length", "L", "Target length of the vector variable.", GH_Kernel.GH_ParamAccess.item);

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

            Types_Var.Gh_IVariable vector = new Types_Var.Gh_IVariable();
            double length = 0d;

            double weight = 0d;


            //     -----     Get Inputs     -----     //

            if (!DA.GetData(0, ref vector)) { return; };
            if (!DA.GetData(1, ref length)) { return; };

            if (!DA.GetData(2, ref weight)) { weight = 1d; };

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(vector, "The input V is null");


            //     -----     Core     -----     //

            int dimension = vector.Size;

            // Create the list of variables
            Types_Var.Gh_IVariable[] gh_Variables = new Types_Var.Gh_IVariable[1] { vector };

            // Create the constraint type
            GP.ConstraintTypes.Vector.Length constraintType = new GP.ConstraintTypes.Vector.Length(length, dimension);
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
        public override Guid ComponentGuid => new Guid("{3925C384-0698-405D-9C77-ACD0A85AC720}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Others;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        #endregion
    }
}
