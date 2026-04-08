using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using GH_Kernel = Grasshopper.Kernel;
using GP = BRIDGES.Solvers.GuidedProjection;
using Types_Models = Frog.Types.Models;

namespace Frog.Variables.Deconstruct
{
    /// <summary>
    /// A grasshopper component retrieving sets of <see cref="GP.Variable"/>.
    /// </summary>
    public class Comp_FindVariables : GH_Kernel.GH_Component
    {
        #region Event Handlers

        public void OnModelParameterSourcesChanged(object sender, GH_Kernel.GH_ParamServerEventArgs e)
        {
            GH_Kernel.GH_ComponentParamServer paramServer = (GH_Kernel.GH_ComponentParamServer)sender;

            //     -----     Verifications     -----     //

            if (e.ParameterSide != GH_Kernel.GH_ParameterSide.Input | e.ParameterIndex != 0) { return; }    // Watches the "Model" input parameter.
            // The VolatileData is not necessarily colleted. Hence, checking the sources seems more robust
            if (paramServer.Input[0].SourceCount == 0) { return; }

            if (this.OnPingDocument() is null) { return; }                                                  // When the component is being deleted.



            ////     -----     Core     -----     //

            // There is only one input model
            if (paramServer.Input[0].SourceCount == 1 && paramServer.Input[0].Sources[0].VolatileDataCount == 1)
            {
                if (paramServer.Input[1].VolatileDataCount == 0)
                {
                    GH_Kernel.Data.GH_Structure<Types_Models.Gh_Model> gh_Models
                                        = (GH_Kernel.Data.GH_Structure<Types_Models.Gh_Model>)this.Params.Input[0].Sources[0].VolatileData;

                    Types_Models.Gh_Model? gh_Model = gh_Models[0][0];

                    if (gh_Model is null) { return; }
                    else { AddSetNamesValueList(gh_Model); }
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_FindVariables"/> class.
        /// </summary>
        public Comp_FindVariables()
          : base("Find Set", "Find", "Retrieves the variables of a set.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Variables])
        {
            this.Params.ParameterSourcesChanged += OnModelParameterSourcesChanged;
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Adds a <see cref="GH_Kernel.Special.GH_ValueList"/> containing the names of the set of variables.
        /// </summary>
        /// <param name="gh_Model"> Grasshopper model. </param>
        private void AddSetNamesValueList(Types_Models.Gh_Model gh_Model)
        {
            int paramIndex = 1;

            //     -----     Initialise the ValueList     -----     //

            GH_Kernel.Special.GH_ValueList valueList = new GH_Kernel.Special.GH_ValueList();
            valueList.Description = "Provides the list of names of the set of variables in the computed model.";
            valueList.Hidden = true;

            // Fill the ValueList

            var items = new List<GH_Kernel.Special.GH_ValueListItem>(gh_Model.SetsOfVariables.Count);
            foreach (string setName in gh_Model.SetsOfVariables.Keys)
            {
                items.Add(new GH_Kernel.Special.GH_ValueListItem(setName, "\"" + setName + "\""));
            }

            valueList.ListItems.Clear();
            valueList.ListItems.AddRange(items);

            // Custom ValueList position
            valueList.CreateAttributes();

            float xCoord =  this.Params.Input[paramIndex].Attributes.InputGrip.X - valueList.Attributes.OutputGrip.X;
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
            pManager.AddParameter(new Parameters.Models.Param_Model(), "Computed Model", "M", "Computed model obtained with the Guided Projection Algorithm.", GH_Kernel.GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the set of variables to retrieve.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Variable's Components", "V", "Components of the variables contained in Set.", GH_Kernel.GH_ParamAccess.tree);
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


            //     -----     Core     -----    //

            Grasshopper.DataTree<double> tree = new Grasshopper.DataTree<double>();

            if (gh_Model.SetsOfVariables is not null && 
                gh_Model.SetsOfVariables.TryGetValue(name, out GP.Variable[]? set))
            {
                for (int i_Variable = 0; i_Variable < set.Length; i_Variable++)
                {
                    GP.Variable variable = set[i_Variable];

                    tree.AddRange(variable.ToArray(), new GH_Kernel.Data.GH_Path(i_Variable));
                }
            }
            else { AddRuntimeMessage(GH_Kernel.GH_RuntimeMessageLevel.Warning, "No set with the specified name was found in the model."); }

            //     -----     Set Output     -----     //

            DA.SetDataTree(0, tree);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{48082F7A-C36C-431C-88C3-6A52008FCEE5}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Deconstruct;

        #endregion
    }
}
