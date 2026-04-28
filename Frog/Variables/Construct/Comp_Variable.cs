using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Sys_Drawing = System.Drawing;

using GP = BRIDGES.Solvers.GuidedProjection;

using GH_GUI = Grasshopper.GUI;
using GH_Kernel = Grasshopper.Kernel;

using Types_Var = Frog.Types.Variables;


namespace Frog.Variables.Construct
{
    /// <summary>
    /// A grasshopper component creating a <see cref="GP.Variable"/>.
    /// </summary>
    public class Comp_Variable : GH_Kernel.GH_Component, GH_Kernel.IGH_VariableParameterComponent
    {
        #region Fields

        /// <summary>
        /// Type of data that the variable respresents.
        /// </summary>
        Types_Var.VariableDataType _dataType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of data for the variable
        /// </summary>
        public Types_Var.VariableDataType DataType 
        {
            get { return _dataType; }
            set { _dataType = value; OnDataTypeChanged(); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_Variable"/> class.
        /// </summary>
        public Comp_Variable()
          : base("Construct Variable", "Var.",
              "Defines the coordinate of a variable.",
              GhLibraryInfo.CategoryName, GhLibraryInfo.SubCategoryName[Frog.SubCategory.Variables])
        {
            //     -----     Initialisations     -----     //

            // Initialise Properties

            DataType = Types_Var.VariableDataType.Scalar;
        }

        #endregion

        #region Other Methods

        //     -----     Data Type Changed     -----     //

        /// <summary>
        /// Updates the input parameter depending on the <see cref="DataType"/>.
        /// </summary>
        /// <exception cref="NotImplementedException"> The data type update is not implemented for a <see cref="DataType"/>. </exception>
        private void OnDataTypeChanged()
        {
            List<GH_Kernel.IGH_Param> sources = new List<GH_Kernel.IGH_Param>();
            if (this.Params.Input.Count == 1) { /* Do Nothing */}
            else if (this.Params.Input.Count == 2)
            {
                sources = new List<GH_Kernel.IGH_Param>(this.Params.Input[1].Sources);
                Params.UnregisterInputParameter(this.Params.Input[1]);
            }
            else { throw new Exception("The state of component is not managed."); }

            GH_Kernel.IGH_Param parameter;
            switch (DataType)
            {
                case Types_Var.VariableDataType.Scalar:
                    parameter = OnDataTypeChanged_ToScalar(sources);
                    break;
                case Types_Var.VariableDataType.Array:
                    parameter = OnDataTypeChanged_ToArray(sources);
                    break;
                case Types_Var.VariableDataType.Point:
                    parameter = OnDataTypeChanged_ToPoint(sources);
                    break;
                default:
                    throw new NotImplementedException($"The data type update is not implemented for a {DataType}.");
            }

            Params.RegisterInputParam(parameter, 1);

            this.Params.OnParametersChanged();
            ExpireSolution(true);
        }


        private GH_Kernel.IGH_Param OnDataTypeChanged_ToScalar(List<GH_Kernel.IGH_Param> sources)
        {
            // Updates the GH_ParamAccess for the input names
            this.Params.Input[0].Access = GH_Kernel.GH_ParamAccess.item;

            // Create the parameter for the input values
            GH_Kernel.Parameters.Param_Number parameter = new GH_Kernel.Parameters.Param_Number();
            parameter.Name = "Coordinate Value";
            parameter.NickName = "C";
            parameter.Description = "Value for the coordinate of the scalar variable.";
            parameter.Access = GH_Kernel.GH_ParamAccess.list;
            for (int i = 0; i < sources.Count; i++) { parameter.AddSource(sources[i]); }

            return parameter;
        }

        private GH_Kernel.IGH_Param OnDataTypeChanged_ToArray(List<GH_Kernel.IGH_Param> sources)
        {
            // Updates the GH_ParamAccess for the input names
            this.Params.Input[0].Access = GH_Kernel.GH_ParamAccess.item;

            // Create the parameter for the input values
            GH_Kernel.Parameters.Param_Number parameter = new GH_Kernel.Parameters.Param_Number();
            parameter.Name = "Coordinate Values";
            parameter.NickName = "C";
            parameter.Description = "Values for the coordinates of the array variable.";
            parameter.Access = GH_Kernel.GH_ParamAccess.tree;
            for (int i = 0; i < sources.Count; i++) { parameter.AddSource(sources[i]); }

            return parameter;
        }

        private GH_Kernel.IGH_Param OnDataTypeChanged_ToPoint(List<GH_Kernel.IGH_Param> sources)
        {
            // Updates the GH_ParamAccess for the input names
            this.Params.Input[0].Access = GH_Kernel.GH_ParamAccess.item;

            // Create the parameter for the input values
            GH_Kernel.Parameters.Param_Point parameter = new GH_Kernel.Parameters.Param_Point();
            parameter.Name = "Point";
            parameter.NickName = "P";
            parameter.Description = "Point whose coordinate are use for the variable.";
            parameter.Access = GH_Kernel.GH_ParamAccess.list;
            for (int i = 0; i < sources.Count; i++) { parameter.AddSource(sources[i]); }

            return parameter;
        }


        //     -----     Create Variables     -----     //

        /// <summary>
        /// Creates the variables from the input values.
        /// </summary>
        /// <param name="DA"> <see cref="GH_Kernel.IGH_DataAccess"/> object used to retrieve input parameters or assign output parameters. </param>
        /// <param name="gh_Variables"> The variables created. </param>
        /// <returns> <see langword="true"/> if the variables were created, <see langword="false"/> otherwise. </returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CreateVariables(GH_Kernel.IGH_DataAccess DA, out GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable> gh_Variables)
        {
            switch (DataType)
            {
                case Types_Var.VariableDataType.Scalar:
                    return CreateScalarVariables(DA, out gh_Variables);
                case Types_Var.VariableDataType.Array:
                    return CreateArrayVariables(DA, out gh_Variables);
                case Types_Var.VariableDataType.Point:
                    return CreatePointVariables(DA, out gh_Variables);
                default:
                    throw new NotImplementedException($"The creation of a variable for {DataType} type is not implemented");
            }
        }


        private bool CreateScalarVariables(GH_Kernel.IGH_DataAccess DA, out GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable> gh_Variables)
        {
            //     -----      Initialise     -----     //

            gh_Variables = new GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable>();


            //     -----      Get Inputs     -----     //

            string setName = string.Empty;
            List<double> values = new List<double>();

            if (!DA.GetData(0, ref setName)) { return false; }
            if (!DA.GetDataList(1, values)) { return false; }


            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(setName);
            ArgumentNullException.ThrowIfNull(values);

            if (setName == string.Empty) { throw new ArgumentException("The input N is empty."); }
            if (values.Count == 0) { throw new ArgumentException("The input V is empty."); }


            //     -----     Core     -----    //

            for (int i = 0; i < values.Count; i++)
            {
                double[] coordinates = new double[1] { values[i] };

                Types_Var.Gh_Variable gh_Variable = new Types_Var.Gh_Variable(coordinates, setName, i, Types_Var.VariableDataType.Scalar);
                gh_Variables.Append(gh_Variable);
            }

            return true;
        }
        
        private bool CreateArrayVariables(GH_Kernel.IGH_DataAccess DA, out GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable> gh_Variables)
        {
            //     -----      Initialise     -----     //

            gh_Variables = new GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable>();


            //     -----     Get Inputs     -----     //

            string setName = string.Empty;
            GH_Kernel.Data.GH_Structure<GH_Kernel.Types.GH_Number> gh_Numbers = new GH_Kernel.Data.GH_Structure<GH_Kernel.Types.GH_Number>();

            if (!DA.GetData(0, ref setName)) { return false; }
            if (!DA.GetDataTree(1, out gh_Numbers)) { return false; }


            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(setName);
            ArgumentNullException.ThrowIfNull(gh_Numbers);

            if (setName == string.Empty) { throw new ArgumentException("The input N is empty."); }
            if (gh_Numbers.IsEmpty) { throw new ArgumentException("The input V is empty."); }


            //     -----     Core     -----    //

            for (int i = 0; i < gh_Numbers.PathCount; i++)
            {
                GH_Kernel.Data.GH_Path path = gh_Numbers.Paths[i];
                List<GH_Kernel.Types.GH_Number> branch_Numbers = gh_Numbers[path];

                double[] coordinates = new double[branch_Numbers.Count];
                for (int j = 0; j < branch_Numbers.Count; j++) { coordinates[j] = branch_Numbers[j].Value; }

                Types_Var.Gh_Variable gh_Variable = new Types_Var.Gh_Variable(coordinates, setName, i, Types_Var.VariableDataType.Array);
                gh_Variables.Append(gh_Variable);
            }

            return true;
        }
        
        private bool CreatePointVariables(GH_Kernel.IGH_DataAccess DA, out GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable> gh_Variables)
        {
            //     -----      Initialise     -----     //

            gh_Variables = new GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable>();


            //     -----      Get Inputs     -----     //

            string setName = string.Empty;
            List<Rhino.Geometry.Point3d> points = new List<Rhino.Geometry.Point3d>();

            if (!DA.GetData(0, ref setName)) { return false; }
            if (!DA.GetDataList(1, points)) { return false; }


            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(setName);
            ArgumentNullException.ThrowIfNull(points);

            if (setName == string.Empty) { throw new ArgumentException("The input N is empty."); }
            if (points.Count == 0) { throw new ArgumentException("The input P is empty."); }


            //     -----     Core     -----    //

            for (int i = 0; i < points.Count; i++)
            {
                double[] coordinates = new double[3] { points[i].X, points[i].Y, points[i].Z };

                Types_Var.Gh_Variable gh_Variable = new Types_Var.Gh_Variable(coordinates, setName, i, Types_Var.VariableDataType.Point);
                gh_Variables.Append(gh_Variable);
            }


            return true;
        }

        #endregion


        #region Override : GH_Component

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterInputParams(GH_InputParamManager)"/>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "N", "Name of the set of variables.", GH_Kernel.GH_ParamAccess.item);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Component.RegisterOutputParams(GH_OutputParamManager)"/>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Parameters.Variables.Param_IVariable(), "Variable", "V", "Newly created variable.", GH_Kernel.GH_ParamAccess.list);
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.SolveInstance(GH_Kernel.IGH_DataAccess)"/>
        protected override void SolveInstance(GH_Kernel.IGH_DataAccess DA)
        {
            //     -----     Core     -----    //

            bool created = CreateVariables(DA, out GH_Kernel.Data.GH_Structure<Types_Var.Gh_Variable> gh_Variables);

            //     -----     Set Output     -----     //

            if (!created) { return; }
            else { DA.SetDataList(0, gh_Variables); }
        }


        /// <inheritdoc cref="GH_Kernel.GH_Component.CreateAttributes()"/>
        public override void CreateAttributes()
        {
            m_attributes = new Comp_VariableAttribute(this);
        }

        #endregion

        #region Override : GH_DocumentObject

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.ComponentGuid"/>
        public override Guid ComponentGuid => new Guid("{9D77FFE5-326B-4A3B-B2FE-A75E67E2D09C}");

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Icon"/>
        protected override Sys_Drawing.Bitmap? Icon
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Exposure"/>
        public override GH_Kernel.GH_Exposure Exposure => (GH_Kernel.GH_Exposure)TabExposure.Construct;


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Write(GH_IO.Serialization.GH_IWriter)"/>
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("DataType", (int)DataType);
            return base.Write(writer);
        }

        /// <inheritdoc cref="GH_Kernel.GH_DocumentObject.Read(GH_IO.Serialization.GH_IReader)"/>
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            DataType = (Types_Var.VariableDataType)reader.GetInt32("DataType");
            return base.Read(reader);
        }

        #endregion


        #region Implement : IGH_VariableParameterComponent

        /// <inheritdoc cref="GH_Kernel.IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide, int)"/>
        public bool CanInsertParameter(GH_Kernel.GH_ParameterSide side, int index) => false;

        /// <inheritdoc cref="GH_Kernel.IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide, int)"/>
        public bool CanRemoveParameter(GH_Kernel.GH_ParameterSide side, int index) => false;

        /// <inheritdoc cref="GH_Kernel.IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide, int)"/>
        public GH_Kernel.IGH_Param? CreateParameter(GH_Kernel.GH_ParameterSide side, int index) => null;

        /// <inheritdoc cref="GH_Kernel.IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide, int)"/>
        public bool DestroyParameter(GH_Kernel.GH_ParameterSide side, int index) => true;

        /// <inheritdoc cref="GH_Kernel.IGH_VariableParameterComponent.VariableParameterMaintenance()"/>
        public void VariableParameterMaintenance()
        {
            // Do Nothing
        }

        #endregion
    }


    /// <summary>
    /// Attributes of the <see cref="Comp_Variable"/>.
    /// </summary>
    public class Comp_VariableAttribute : GH_Kernel.Attributes.GH_ComponentAttributes
    {
        #region Fields

        //     -----     For the Dropdown Menu Head     -----     // 

        private Sys_Drawing.RectangleF _dropDownBounds;

        /// <summary>
        /// Height of the Dropdown Menu
        /// </summary>
        private const float DropDownHeight = 22f;
        /// <summary>
        /// Padding of the Dropdown Menu
        /// </summary>
        /// <remarks> The padding corresponds to ? </remarks>
        private const float DropDownPadding = 4f;


        //     -----     For the Popup Menu     -----     // 

        // Popup state

        /// <summary>
        /// Evaluates whether the Popup Menu is open.
        /// </summary>
        private bool _popupOpen;

        /// <summary>
        /// Region of the Popup Menu.
        /// </summary>
        private Sys_Drawing.RectangleF _popupBounds;

        /// <summary>
        /// Height of an item contained in the Popup Menu.
        /// </summary>
        private const float ItemHeight = 18f;

        /// <summary>
        /// Padding of the Popup Menu.
        /// </summary>
        /// <remarks> The padding corresponds to ? </remarks>
        private const float PopupPadding = 2f;

        private int _hoverIndex = -1;

        /// <summary>
        /// List of region for each item in the Popup Menu.
        /// </summary>
        private readonly List<Sys_Drawing.RectangleF> _itemBounds = new List<Sys_Drawing.RectangleF>();


        //     -----     Other     -----     // 

        /// <summary>
        /// Canvas to which this <see cref="GH_Kernel.Attributes.GH_ComponentAttributes"/> might be hooked.
        /// </summary>
        private GH_GUI.Canvas.GH_Canvas? _hookedCanvas;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Comp_VariableAttribute"/> class from its owner .
        /// </summary>
        /// <param name="owner"> <see cref="Comp_Variable"/> whose attributes will be modified. </param>
        public Comp_VariableAttribute(Comp_Variable owner) : base(owner) 
        { 

        }

        #endregion

        #region Other Methods

        //     -----     Layout     -----     //

        /// <summary>
        /// Rebuild the <see cref="_popupBounds"/> and the <see cref="_itemBounds"/>
        /// </summary>
        private void RebuildPopupLayout()
        {
            // Clear the region for the items in the Popup Menu
            _itemBounds.Clear();
            _hoverIndex = -1;

            // If the PopupMenu is not opened, create an empty region
            if (!_popupOpen)
            {
                _popupBounds = Sys_Drawing.RectangleF.Empty;
                return;
            }


            //var comp = Owner as Comp_Variable;
            //if (comp == null)
            //{
            //    _popupOpen = false;
            //    _popupBounds = RectangleF.Empty;
            //    return;
            //}

            Types_Var.VariableDataType[] knownDataTypes = (Types_Var.VariableDataType[])Enum.GetValues(typeof(Types_Var.VariableDataType));

            // Position of the Top-Left corner of the Popup Menu
            float popupX = _dropDownBounds.Left;
            float popupY = _dropDownBounds.Top; // Old : _dropDownBounds.Bottom + 2,
            // Size of the Popup Menu
            float popupWidth = _dropDownBounds.Width;
            float popupHeight = (2 * PopupPadding) + (knownDataTypes.Length * ItemHeight);

            _popupBounds = new Sys_Drawing.RectangleF(popupX, popupY, popupWidth, popupHeight);


            // Add the items in the list of item Region.
            float y = _popupBounds.Top + PopupPadding;
            for (int i = 0; i < knownDataTypes.Length; i++)
            {
                // Position of the Top-Left corner of the item in the Popup Menu
                float itemX = _popupBounds.Left + PopupPadding;
                float itemY = y;
                // Size of the item in the Popup Menu
                float itemWidth = _popupBounds.Width - 2 * PopupPadding;
                float itemHeight = ItemHeight;

                _itemBounds.Add(new Sys_Drawing.RectangleF(itemX, itemY, itemWidth, itemHeight));

                y += ItemHeight;
            }
        }

        //     -----     Render     -----     //

        /// <summary>
        /// Draws an arrow in the rectangle, on its right
        /// </summary>
        /// <param name="graphics"> Graphics object to draw with. </param>
        /// <param name="rectangle"> Region in which the arrow must be drawn. </param>
        private void DrawArrow(Sys_Drawing.Graphics graphics, Sys_Drawing.RectangleF rectangle)
        {
            float cx = rectangle.Right - 12f;
            float cy = rectangle.Top + rectangle.Height / 2f;

            Sys_Drawing.PointF p1 = new Sys_Drawing.PointF(cx - 4f, cy - 2f);
            Sys_Drawing.PointF p2 = new Sys_Drawing.PointF(cx + 4f, cy - 2f);
            Sys_Drawing.PointF p3 = new Sys_Drawing.PointF(cx, cy + 3f);

            using (var b = new Sys_Drawing.SolidBrush(Sys_Drawing.Color.FromArgb(60, 60, 60)))
                graphics.FillPolygon(b, new[] { p1, p2, p3 });
        }

        /// <summary>
        /// Draws the Popup Menu
        /// </summary>
        /// <param name="graphics"> Graphics object to draw with. </param>
        /// <param name="component"> Grasshopper component to draw over. </param>
        private void DrawPopup(Sys_Drawing.Graphics graphics, Comp_Variable? component)
        {
            //     -----     Verification

            if (component == null) { return; }

            // Draw the background of the Popup Menu
            using (var back = new Sys_Drawing.SolidBrush(Sys_Drawing.Color.FromArgb(252, 252, 252)))
                graphics.FillRectangle(back, _popupBounds);

            // Draw the countour of the Popup Menu
            using (var pen = new Sys_Drawing.Pen(Sys_Drawing.Color.FromArgb(160, 160, 160)))
                graphics.DrawRectangle(pen, _popupBounds.X, _popupBounds.Y, _popupBounds.Width, _popupBounds.Height);

            // Draw each items in the Popup Menu
            Types_Var.VariableDataType[] knownDataTypes = (Types_Var.VariableDataType[])Enum.GetValues(typeof(Types_Var.VariableDataType));

            for (int i = 0; i < _itemBounds.Count; i++)
            {
                Sys_Drawing.RectangleF itemRectangle = _itemBounds[i];

                bool isItemSelected = (knownDataTypes[i] == component.DataType);
                bool isItemHovered = (i == _hoverIndex);

                if (isItemHovered)
                {
                    using (var hb = new Sys_Drawing.SolidBrush(Sys_Drawing.Color.FromArgb(230, 240, 255)))
                        graphics.FillRectangle(hb, itemRectangle);
                }
                if (isItemSelected)
                {
                    var dot = new Sys_Drawing.RectangleF(itemRectangle.Left + 4, itemRectangle.Top + itemRectangle.Height / 2f - 2, 4, 4);
                    using (var b = new Sys_Drawing.SolidBrush(Sys_Drawing.Color.FromArgb(60, 120, 220)))
                        graphics.FillEllipse(b, dot);
                }

                var tr = Sys_Drawing.RectangleF.Inflate(itemRectangle, -10, 0); // Leave Space for the selection dot

                using (var font = new Sys_Drawing.Font(GH_Kernel.GH_FontServer.Console.FontFamily, 5.5f, Sys_Drawing.FontStyle.Regular))
                using (var sf = new Sys_Drawing.StringFormat { Alignment = Sys_Drawing.StringAlignment.Near, LineAlignment = Sys_Drawing.StringAlignment.Center })
                using (var brush = new Sys_Drawing.SolidBrush(Sys_Drawing.Color.Black))
                {
                    graphics.DrawString(knownDataTypes[i].ToString(), font, brush, tr, sf);
                }
            }
        }

        //     -----     Interactions     -----     //

        /// <summary>
        /// Gets the position of the mouse on the canvas
        /// </summary>
        /// <param name="sender"> Base canvas. </param>
        /// <param name="e"> Class used for mouse events on the canvas.</param>
        /// <returns> The relative position of the mouse on the canvas. </returns>
        private static Sys_Drawing.PointF GetMouseCanvasPoint(GH_GUI.Canvas.GH_Canvas sender, GH_GUI.GH_CanvasMouseEvent e)
        {
            // CONTROL -> CANVAS conversion (robust with zoom/pan)
            return sender.Viewport.UnprojectPoint(new Sys_Drawing.PointF(e.ControlLocation.X, e.ControlLocation.Y));
        }

        /// <summary>
        /// Evaluates whether the clic happens on an item of the PopupMenu.
        /// </summary>
        /// <param name="canvasPt"> Position of the clic on the canvas. </param>
        /// <returns> The index of the item if one was clicked, otherwise -1. </returns>
        private int HitTestItem(Sys_Drawing.PointF canvasPt)
        {
            for (int i = 0; i < _itemBounds.Count; i++)
                if (_itemBounds[i].Contains(canvasPt))
                    return i;
            return -1;
        }


        /// <summary>
        /// Opens the Popup Menu, and subscribes to the canvas mouse events.
        /// </summary>
        /// <param name="canvas"> Base canvas.</param>
        private void OpenPopup(GH_GUI.Canvas.GH_Canvas canvas)
        {
            _popupOpen = true;
            RebuildPopupLayout();

            // FORCE recalcul Bounds/Layout
            //Owner.Attributes?.ExpireLayout();

            HookCanvas(canvas);
            canvas?.Invalidate();
        }

        /// <summary>
        /// Opens the Popup Menu, and unsubscribe to the canvas mouse events.
        /// </summary>
        /// <param name="canvas"> Base canvas.</param>
        private void ClosePopup(GH_GUI.Canvas.GH_Canvas canvas)
        {
            _popupOpen = false;
            _hoverIndex = -1;
            RebuildPopupLayout();

            // FORCE recalcul Bounds/Layout
            //Owner.Attributes?.ExpireLayout();

            UnhookCanvas();
            canvas?.Invalidate();
        }

        //     -----     Interactions in Canvas     -----     //

        /// <summary>
        /// Subscribes to the canvas mouse events.
        /// </summary>
        /// <remarks> The subscription should be achieved when the PopupMenu is opened. </remarks>
        /// <param name="canvas"> Base canvas. </param>
        private void HookCanvas(GH_GUI.Canvas.GH_Canvas canvas)
        {
            if (canvas == null) return;
            if (_hookedCanvas == canvas) return;

            UnhookCanvas();
            _hookedCanvas = canvas;

            // Close popup when clicking anywhere on the canvas
            _hookedCanvas.MouseDown += Canvas_MouseDown_ClosePopup;
            _hookedCanvas.MouseMove += Canvas_MouseMove_Hover;
        }

        /// <summary>
        /// Unsubscribes to the canvas mouse events.
        /// </summary>
        /// <remarks> The unsubscription should be achieved when the PopupMenu is closed. </remarks>
        private void UnhookCanvas()
        {
            if (_hookedCanvas == null) { return; }

            _hookedCanvas.MouseDown -= Canvas_MouseDown_ClosePopup;
            _hookedCanvas.MouseMove -= Canvas_MouseMove_Hover;
            _hookedCanvas = null;
        }


        /// <summary>
        /// Method for the canvas mouse event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> </param>
        private void Canvas_MouseDown_ClosePopup(object? sender, MouseEventArgs e)
        {
            var canvas = sender as GH_GUI.Canvas.GH_Canvas;
            if (canvas == null) return;

            // CONTROL -> CANVAS
            Sys_Drawing.PointF mouseCanvas = canvas.Viewport.UnprojectPoint(new Sys_Drawing.PointF(e.X, e.Y));

            // Si popup ouvert: priorité à la sélection dans la liste
            if (_popupOpen)
            {
                // Clic dans popup => tenter de sélectionner
                if (_popupBounds.Contains(mouseCanvas))
                {
                    int hitItem = HitTestItem(mouseCanvas);
                    if (hitItem >= 0)
                    {
                        Comp_Variable component = (Owner as Comp_Variable)!;

                        Types_Var.VariableDataType[] knownDataTypes = (Types_Var.VariableDataType[])Enum.GetValues(typeof(Types_Var.VariableDataType));

                        component.DataType = knownDataTypes[hitItem];
                    }

                    // Dans tous les cas, on ferme après un clic dans la popup (UX classique)
                    ClosePopup(canvas);
                    return;
                }

                // Clic sur le bouton dropdown => toggle (ferme)
                if (_dropDownBounds.Contains(mouseCanvas))
                {
                    ClosePopup(canvas);
                    return;
                }

                // Clic ailleurs => fermer
                ClosePopup(canvas);
                return;
            }

            // Si popup fermé: clic sur dropdown => ouvrir
            if (_dropDownBounds.Contains(mouseCanvas) & !_popupOpen)
            {
                OpenPopup(canvas);
                return;
            }
        }

        /// <summary>
        /// Method for the canvas mouse event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> </param>
        private void Canvas_MouseMove_Hover(object? sender, MouseEventArgs e)
        {
            if (!_popupOpen) return;

            var canvas = sender as GH_GUI.Canvas.GH_Canvas;
            if (canvas == null) return;

            // CONTROL -> CANVAS
            Sys_Drawing.PointF mouseCanvas = canvas.Viewport.UnprojectPoint(new Sys_Drawing.PointF(e.X, e.Y));

            int newHover = HitTestItem(mouseCanvas);
            if (newHover != _hoverIndex)
            {
                _hoverIndex = newHover;
                canvas.Invalidate();
            }
        }

        #endregion


        #region Override : GH_ComponentAttributes

        //     -----     Layout     -----     //

        /// <inheritdoc cref="GH_Kernel.Attributes.GH_ComponentAttributes.Layout()"/>
        protected override void Layout()
        {
            // Create the common component layout
            base.Layout();

            // Extend the component for the Dropdown Menu, at the bottom.
            Sys_Drawing.RectangleF baseBounds = Bounds;
            Bounds = new Sys_Drawing.RectangleF(baseBounds.X, baseBounds.Y, baseBounds.Width, baseBounds.Height + DropDownHeight + DropDownPadding);


            // Position of the Top-Left corner of the DropDown Menu
            float dropdownX = baseBounds.X + 2;
            float dropdownY = baseBounds.Bottom + DropDownPadding / 2f;
            // Size of the DropDown Menu
            float dropdownWidth = baseBounds.Width - 4;
            float dropdownHeight = DropDownHeight;

            _dropDownBounds = new Sys_Drawing.RectangleF(dropdownX, dropdownY, dropdownWidth, dropdownHeight);


            // Rebuild the Popup Menu (if needed)
            RebuildPopupLayout();
        }

        //     -----     Render     -----     //

        /// <inheritdoc cref="GH_Kernel.Attributes.GH_ComponentAttributes.Render(GH_GUI.Canvas.GH_Canvas, Sys_Drawing.Graphics, GH_CanvasChannel)"/>
        protected override void Render(GH_GUI.Canvas.GH_Canvas canvas, Sys_Drawing.Graphics graphics, GH_GUI.Canvas.GH_CanvasChannel channel)
        {
            // Create the common component render
            base.Render(canvas, graphics, channel);

            // If the GH_CanvasChannel is for the rendering of components
            if (channel == GH_GUI.Canvas.GH_CanvasChannel.Objects)
            {
                // Draw the background of the Dropdown Menu
                using (Sys_Drawing.SolidBrush back = new Sys_Drawing.SolidBrush(Sys_Drawing.Color.FromArgb(245, 245, 245)))
                    graphics.FillRectangle(back, _dropDownBounds);

                // Draw the countour of the Dropdown Menu
                using (var pen = new Sys_Drawing.Pen(Sys_Drawing.Color.FromArgb(120, 120, 120)))
                    graphics.DrawRectangle(pen, _dropDownBounds.X, _dropDownBounds.Y, _dropDownBounds.Width, _dropDownBounds.Height);


                // Draw the text in the Dropdown Menu
                Comp_Variable component = (Owner as Comp_Variable)!;
                string text = component?.DataType.ToString() ?? string.Empty;

                var textRect = Sys_Drawing.RectangleF.Inflate(_dropDownBounds, -6, -3); // Leave space for the Arrow

                using (var font = new Sys_Drawing.Font(GH_Kernel.GH_FontServer.Console.FontFamily, 5.5f, Sys_Drawing.FontStyle.Regular))
                using (var sf = new Sys_Drawing.StringFormat { Alignment = Sys_Drawing.StringAlignment.Near, LineAlignment = Sys_Drawing.StringAlignment.Center })
                using (var brush = new Sys_Drawing.SolidBrush(Sys_Drawing.Color.Black))
                {
                    graphics.DrawString(text, font, brush, textRect, sf);
                }

                DrawArrow(graphics, _dropDownBounds);

                // Draw popup overlay (if needed)
                if (_popupOpen) { DrawPopup(graphics, component); }
                    
            }
        }

        //     -----     Interactions in Component     -----     //

        /// <inheritdoc cref="GH_Kernel.Attributes.GH_ComponentAttributes.RespondToMouseDown(GH_Canvas, GH_CanvasMouseEvent)"/>
        public override GH_GUI.Canvas.GH_ObjectResponse RespondToMouseDown(GH_GUI.Canvas.GH_Canvas sender, GH_GUI.GH_CanvasMouseEvent e)
        {
            // Ensure that the layout of the Popup Menu is always up-to-date when open
            if (_popupOpen) { RebuildPopupLayout(); return GH_GUI.Canvas.GH_ObjectResponse.Handled; }

            Sys_Drawing.PointF mouseCanvas = GetMouseCanvasPoint(sender, e);


            // Quand le Popup Menu est ouvert, le clic est géré via le Canvas par
            // Canvas_MouseDown_ClosePopup pour éviter un double traitement.

            // Toggle popup when clicking the dropdown area
            if (_dropDownBounds.Contains(mouseCanvas))
            {
                OpenPopup(sender);
                return GH_GUI.Canvas.GH_ObjectResponse.Handled;
            }

            return base.RespondToMouseDown(sender, e);
        }

        /// <inheritdoc cref="GH_Kernel.GH_Attributes{T}.RespondToMouseMove(GH_Canvas, GH_CanvasMouseEvent)"/>
        public override GH_GUI.Canvas.GH_ObjectResponse RespondToMouseMove(GH_GUI.Canvas.GH_Canvas sender, GH_GUI.GH_CanvasMouseEvent e)
        {
            if (_popupOpen)
            {
                Sys_Drawing.PointF mouseCanvas = GetMouseCanvasPoint(sender, e);
                int newHover = HitTestItem(mouseCanvas);

                if (newHover != _hoverIndex)
                {
                    _hoverIndex = newHover;
                    sender.Invalidate();
                }

                // Optional: consume move while popup is open to avoid odd GH interactions
                return GH_GUI.Canvas.GH_ObjectResponse.Handled;
            }

            return base.RespondToMouseMove(sender, e);
        }

        #endregion
    }
}