using System;
using System.Collections.Generic;

using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Class defining a grasshopper type which contains a <see cref="GP.SubVariable"/>, enhanced with additional information.
    /// </summary>
    public class Gh_Variable : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Values of the coordinates of this grasshopper variable.
        /// </summary>
        private readonly double[] _coordinates;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Rhino data type to which the Gh_IVariable corresponds.
        /// </summary>
        public RhinoDataType DataType { get; init; }

        /// <summary>
        /// Gets the size of this variable.
        /// </summary>
        public int Size => _coordinates.Length;

        /// <summary>
        /// Gets or set the coordinate value at the given index.
        /// </summary>
        /// <param name="index"> Zero-based index of the coordinate. </param>
        /// <returns> The <see langword="double"/>-precision value of the coordinate. </returns>
        public double this[int index] => _coordinates[index];


        /// <summary>
        /// Gets the set name of this Gh_IVariable.
        /// </summary>
        public string SetName { get; init; }

        /// <summary>
        /// Gets the index of this Gh_IVariable in the set.
        /// </summary>
        /// <remarks> If the index is equal to -1, there is two possibilities :
        /// <list type="number">
        ///     <item>
        ///         <description> The contained <see cref="GP.Interfaces.IVariable"/> is a SubVariable. </description>
        ///     </item>
        ///     <item>
        ///         <description> The contained <see cref="GP.Interfaces.IVariable"/> is a Variable but the indexing could not be achieved. </description>
        ///     </item>
        ///     <item>
        ///         <description> The contained <see cref="GP.Interfaces.IVariable"/> is null. </description>
        ///     </item>
        /// </list>either</remarks>
        public int IndexInSet { get; init; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Variable" /> class.
        /// </summary>
        public Gh_Variable()
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _coordinates = new double[0];

            // Initialise Properties
            SetName = string.Empty;
            IndexInSet = -1;

            DataType = RhinoDataType.Scalar;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Variable" /> class from a <see cref="GP.Variable"/>.
        /// </summary>
        /// <param name="iVariable"> <see cref="GP.Variable"/> to contain in the Gh_IVariable. </param>
        /// <param name="name"> Name of the set to which the Gh_IVariable belongs. </param>
        /// <param name="index"> Index of the Gh_IVariable in the set. </param>
        /// <param name="type"> Gets the Rhino data type to which the IVariable corresponds.</param>
        public Gh_Variable(IList<double> coordinates, string name, int index, RhinoDataType type)
        {
            //     -----     Verifications     -----     //

            ArgumentNullException.ThrowIfNull(coordinates, nameof(coordinates));
            if(coordinates.Count == 0) { throw new ArgumentException("The list of coordinate values is empty."); }

            if (type == RhinoDataType.Point & coordinates.Count != 3 ) { throw new ArgumentException("The size of the point variable is not equal three."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _coordinates = new double[coordinates.Count];
            for (int i = 0; i < _coordinates.Length; i++) { _coordinates[i] = coordinates[i]; }

            // Initialise Properties
            SetName = name;
            IndexInSet = index;

            DataType = type;
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Variable" /> class from another <see cref="Gh_Variable"/>.
        /// </summary>
        /// <param name="gh_Variable"> <see cref="Gh_Variable"/> to deep copy. </param>
        public Gh_Variable(Gh_Variable gh_Variable)
        {
            //     -----     Verifications     -----     //

            if (!gh_Variable.IsValid) { throw new ArgumentException($"The {typeof(Gh_Variable)} to duplicate is not valid."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _coordinates = new double[gh_Variable._coordinates.Length];
            Array.Copy(gh_Variable._coordinates, _coordinates, 0);

            // Initialise Properties
            SetName = gh_Variable.SetName;
            IndexInSet = gh_Variable.IndexInSet;

            DataType = gh_Variable.DataType;
        }

        #endregion


        #region Implement : GH_Types.IGH_Goo

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid => _coordinates.Length != 0;

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else
                {
                    if (_coordinates.Length == 0) { return "The variable's list of coordinate is empty."; }
                    else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
                }
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => $"Grasshopper type representing a {typeof(GP.Variable)}.";

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName => nameof(Gh_Variable);


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_Variable(this);


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            //     -----     Otherwise     -----     //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{Q}(ref Q)"/>
        public bool CastTo<T>(out T? target)
        {
            target = default;

            //     -----     Grasshopper Objects     -----     //

            // Casts a Gh_Variable to a Rhino.Geometry.Point3d
            if (DataType == RhinoDataType.Point & typeof(T).IsAssignableFrom(typeof(GH_Types.GH_Point)) )
            {
                Rhino.Geometry.Point3d point = new Rhino.Geometry.Point3d(_coordinates[0], _coordinates[1], _coordinates[2]);
                GH_Types.GH_Point gh_Point = new GH_Types.GH_Point(point);
                target = (T)(object)gh_Point;

                return true;
            }

            //     -----     Otherwise     -----     //

            return false;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.ToString"/>
        public override string ToString()
        {
            if (IsValid)
            {
                string text = "Variable";

                text += ": [" + String.Format({0:0000}, _coordinates[0]);
                for (int i = 1; i < _coordinates.Length; i++)
                {
                    text += ", " + _coordinates[i];
                }
                text += "]";

                return text;
            }
            else { return $"Invalid {TypeName}"; }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable()
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => true;

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => true;


        /// <inheritdoc cref="GH_Types.IGH_Goo.EmitProxy()"/>
        public GH_Types.IGH_GooProxy? EmitProxy() => null;

        #endregion
    }
}
