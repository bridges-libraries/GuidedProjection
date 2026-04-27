using System;
using System.Collections.Generic;

using LA = BRIDGES.Numerics.LinearAlgebra;
using GP = BRIDGES.Solvers.GuidedProjection;

using GH_Kernel = Grasshopper.Kernel;

using Types_Models = Frog.Types.Models;
using Params_Models = Frog.Parameters.Models;


namespace Frog.Results.Variables
{
    /// <summary>
    /// A grasshopper component retrieving sets of <see cref="GP.Energy"/>.
    /// </summary>
    public class Comp_FindEnergy : GH_Kernel.GH_Component
    {
        #region Event Handlers

        /// <summary>
        /// Method managing the response to a source change for the input <see cref="Params_Models.Param_Model"/>.
        /// </summary>
        /// <param name="sender"> The <see cref="GH_Kernel.GH_ComponentParamServer"/> raising the event. </param>
        /// <param name="e"> Arguments describing the parameter changes. </param>
        public void OnModelParameterSourcesChanged(object sender, GH_Kernel.GH_ParamServerEventArgs e)
        {
            GH_Kernel.GH_ComponentParamServer paramServer = (GH_Kernel.GH_ComponentParamServer)sender;

            //     -----     Verifications     -----     //

            if (e.ParameterSide != GH_Kernel.GH_ParameterSide.Input | e.ParameterIndex != 0) { return; }    // Watches the "Model" input parameter.

            if (this.OnPingDocument() is null) { return; }                                                  // When the component is being deleted.

            // The VolatileData is not necessarily colleted. Hence, checking the sources seems more robust
            if (paramServer.Input[0].SourceCount == 0) { return; }


            ////     -----     Core     -----     //

            if (paramServer.Input[0].SourceCount == 1 && paramServer.Input[0].Sources[0].VolatileDataCount == 1)
            {
                if (paramServer.Input[1].VolatileDataCount == 0)
                {
                    GH_Kernel.Data.GH_Structure<Types_Models.Gh_Model> gh_Models
                                        = (GH_Kernel.Data.GH_Structure<Types_Models.Gh_Model>)this.Params.Input[0].Sources[0].VolatileData;

                    Types_Models.Gh_Model? gh_Model = gh_Models[0][0];

                    if (gh_Model is null || !gh_Model.IsValid) { return; }
                    else { AddSetNamesValueList(gh_Model); }
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_FindEnergy"/> class.
        /// </summary>
        public Comp_FindEnergy()
          : base("Find Energy", "Find E.", "Retrieves the energies with a given name.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Results])
        {
            this.Params.ParameterSourcesChanged += OnModelParameterSourcesChanged;
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Adds a <see cref="GH_Kernel.Special.GH_ValueList"/> containing the names of the set of energies.
        /// </summary>
        /// <param name="gh_Model"> Grasshopper model containing the set names. </param>
        private void AddSetNamesValueList(Types_Models.Gh_Model gh_Model)
        {
            int paramIndex = 1;

            //     -----     Initialise the ValueList     -----     //

            GH_Kernel.Special.GH_ValueList valueList = new GH_Kernel.Special.GH_ValueList();
            valueList.Description = "Provides the list of energy set names in the computed model.";
            valueList.Hidden = true;

            // Fill the ValueList

            var items = new List<GH_Kernel.Special.GH_ValueListItem>(gh_Model.SetsOfEnergies.Count);
            foreach (string setName in gh_Model.SetsOfEnergies.Keys)
            {
                items.Add(new GH_Kernel.Special.GH_ValueListItem(setName, "\"" + setName + "\""));
            }

            valueList.ListItems.Clear();
            valueList.ListItems.AddRange(items);

            // Custom ValueList position
            valueList.CreateAttributes();

            float xCoord = this.Params.Input[paramIndex].Attributes.InputGrip.X - valueList.Attributes.OutputGrip.X;
            float yCoord = this.Params.Input[paramIndex].Attributes.InputGrip.Y - valueList.Attributes.OutputGrip.Y - 1;

            System.Drawing.PointF cornerPt = new System.Drawing.PointF(xCoord, yCoord);
            valueList.Attributes.Pivot = cornerPt;


            //     -----     Add to the Canvas     -----     //

            // Adds to the canvas.
            GH_Kernel.GH_Document document = this.OnPingDocument();
            if (document is null) { return; } // The component is being deleted

            document.AddObject(valueList, false);

            // Connect to the input parameter
            Params.Input[paramIndex].AddSource(valueList);
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Params_Models.Param_Model(), "Computed Model", "M", "Computed model obtained with the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the energies to retrieve.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Energy Remainders", "R", "Remainders of the energies.", GH_Kernel.GH_ParamAccess.list);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Initialisation     -----     //

            Types_Models.Gh_Model gh_Model = new Types_Models.Gh_Model();
            string name = "";

            //     -----      Get Inputs     -----     //

            if (!DA.GetData(0, ref gh_Model)) { return; };
            if (!DA.GetData(1, ref name)) { return; };

            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(gh_Model);

            if (gh_Model.SetsOfConstraints is null || !gh_Model.SetsOfEnergies.ContainsKey(name))
            {
                AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, "No set with the specified name was found in the model.");
                return;
            }


            //     -----     Core     -----    //

            List<GP.Energy> set = gh_Model.SetsOfEnergies[name];

            List<double> remainders = new List<double>(set.Count);

            for (int i_Energy = 0; i_Energy < set.Count; i_Energy++)
            {
                GP.Energy constraint = set[i_Energy];

                int count = 0;
                LA.Vectors.SparseVector<double> localX = new LA.Vectors.SparseVector<double>(constraint.Size);
                for (int i_EnerVariable = 0; i_EnerVariable < constraint.Variables.Count; i_EnerVariable++)
                {
                    GP.Interfaces.IVariable iVariable = constraint.Variables[i_EnerVariable];
                    for (int i_EnerVarComp = 0; i_EnerVarComp < iVariable.Size; i_EnerVarComp++)
                    {
                        if (iVariable[i_EnerVarComp] != 0d) { localX.Add(count, iVariable[i_EnerVarComp]); }
                        
                        count++;
                    }
                }


                double remainder = 0d;
                if (constraint.EnergyType.LocalKi is not null)
                {
                    remainder += LA.Vectors.SparseVector<double>.TransposeMultiply(constraint.EnergyType.LocalKi, localX);
                }
                if (constraint.EnergyType.Si != 0d) {  remainder -= constraint.EnergyType.Si;  }

                remainders.Add(remainder);
            }


            //     -----     Set Output     -----     //

            DA.SetDataList(0, remainders);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{035B5A54-B3FB-48E7-984D-7912BCECE500}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap? Icon => null;

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Energies;

        #endregion
    }
}
