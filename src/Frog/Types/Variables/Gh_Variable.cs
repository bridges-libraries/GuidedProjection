using System;
using System.Collections.Generic;

using GH_Kernel = Grasshopper.Kernel;
using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Variables
{
    /// <summary>
    /// Class defining a grasshopper type which represents a <see cref="GP.Variable"/>, enhanced with additional information.
    /// </summary>
    public class Gh_Variable : IGH_GooVariable
    {
        #region Fields

        /// <summary>
        /// coordinate values of the represented <see cref="GP.Variable"/>.
        /// </summary>
        private readonly double[] _coordinates;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data type to which this grasshopper variable corresponds.
        /// </summary>
        public VariableDataType DataType { get; init; }


        /// <summary>
        /// Gets the name of the set to which this grasshopper variable belongs.
        /// </summary>
        public string SetName { get; init; }

        /// <summary>
        /// Gets the index of this grasshopper variable in the set.
        /// </summary>
        /// <remarks> If the value is '-1', then the set is not ordered. </remarks>
        public int Index { get; init; }

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
            Index = -1;

            DataType = VariableDataType.Scalar;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Variable" /> class from its single coordinate.
        /// </summary>
        /// <param name="coordinate"> Value of the coordinate for the represented variable. </param>
        /// <param name="name"> Name of the set to which the grasshopper variable belongs. </param>
        /// <param name="index"> Index of the grasshopper variable in the set, or '-1' if the set is not ordered. </param>
        /// <param name="type"> Gets the data type to which the grasshopper variable corresponds.</param>
        public Gh_Variable(double coordinate, string name, int index, VariableDataType type = VariableDataType.Scalar)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _coordinates = new double[1] { coordinate };

            // Initialise Properties
            SetName = name;
            Index = index;

            DataType = type;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Variable" /> class from its coordinates.
        /// </summary>
        /// <param name="coordinate"> Values of the coordinates for the represented variable. </param>
        /// <param name="name"> Name of the set to which the grasshopper variable belongs. </param>
        /// <param name="index"> Index of the grasshopper variable in the set, or '-1' if the set is not ordered. </param>
        /// <param name="type"> Gets the data type to which the grasshopper variable corresponds.</param>
        public Gh_Variable(IList<double> coordinates, string name, int index, VariableDataType type)
        {
            //     -----     Verifications     -----     //

            ArgumentNullException.ThrowIfNull(coordinates, nameof(coordinates));
            if(coordinates.Count == 0) { throw new ArgumentException("The list of coordinate values is empty."); }

            if (name == string.Empty) { throw new ArgumentException("The set name is empty."); }

            if (type == VariableDataType.Scalar & coordinates.Count != 1) { throw new ArgumentException("The number of coordinate of a scalar variable must be equal to 1."); }
            if (type == VariableDataType.Array & coordinates.Count < 2) { throw new ArgumentException("The number of coordinate of an array variable must be strictly larger than 1."); }
            if (type == VariableDataType.Point & coordinates.Count != 3 ) { throw new ArgumentException("The number of coordinate of a point variable must be equal to 3."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _coordinates = new double[coordinates.Count];
            for (int i = 0; i < _coordinates.Length; i++) { _coordinates[i] = coordinates[i]; }

            // Initialise Properties
            SetName = name;
            Index = index;

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
            Index = gh_Variable.Index;

            DataType = gh_Variable.DataType;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString()
        {
            if (IsValid)
            {
                string text = "Var";
                text += " [" + GH_Kernel.GH_Format.FormatDouble(this[0]);
                for (int i = 1; i < _coordinates.Length; i++)
                {
                    text += ", " + GH_Kernel.GH_Format.FormatDouble(this[i]);
                }
                text += "]";

                return text;
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion


        #region Implement : IGH_GooVariable

        //     -----     Properties     -----     //

        /// <summary>
        /// Gets the size of the represented <see cref="GP.Variable"/>.
        /// </summary>
        public int Size => _coordinates.Length;

        /// <summary>
        ///  Gets the coordinate value at the given index of the represented <see cref="GP.Variable"/>.
        /// </summary>
        /// <param name="index"> Zero-based index of the coordinate. </param>
        /// <returns> The <see langword="double"/>-precision value of the coordinate. </returns>
        public double this[int index] => _coordinates[index];

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
                    if (_coordinates.Length == 0) { return "The variable's list of coordinates is empty."; }
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
            if (DataType == VariableDataType.Point & typeof(T).IsAssignableFrom(typeof(GH_Types.GH_Point)) )
            {
                Rhino.Geometry.Point3d point = new Rhino.Geometry.Point3d(_coordinates[0], _coordinates[1], _coordinates[2]);
                GH_Types.GH_Point gh_Point = new GH_Types.GH_Point(point);
                target = (T)(object)gh_Point;

                return true;
            }

            //     -----     Otherwise     -----     //

            return false;
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
