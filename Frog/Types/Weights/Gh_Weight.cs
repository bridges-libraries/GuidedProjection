using System;

using GH_Kernel = Grasshopper.Kernel;
using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Weights
{
    /// <summary>
    /// Class defining a grasshopper type which represents a weight for a <see cref="GP.Constraint"/>.
    /// </summary>
    public class Gh_Weight : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Stores the <see langword="double"/>-precision value of the grasshopper weight if it is constant.
        /// </summary>
        internal double? _constant;

        /// <summary>
        /// Stores the function computing the <see langword="double"/>-precision value of the grasshopper weight from the iteration index, if it is evolutive.
        /// </summary>
        private Func<int, double>? _function;

        #endregion

        #region Properties

        /// <summary>
        /// Evaluates whether this grasshopper weight is constant or not.
        /// </summary>
        public bool IsConstant => _constant.HasValue && _function is null;

        /// <summary>
        /// Evaluates whether this grasshopper weight is evolutive or not.
        /// </summary>
        public bool IsEvolutive => _constant is null && _function is not null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Weight" /> class.
        /// </summary>
        public Gh_Weight()
        {
            // Do Nothing
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Weight" /> class from a constant value.
        /// </summary>
        /// <param name="weight"> Value of the constant weight. </param>
        public Gh_Weight(double weight)
        {
            //     -----     Initialisations     -----     //

            // Initialise Fields
            _constant = weight;
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Weight" /> class from a function.
        /// </summary>
        /// <param name="function"> Function computing the value of the weight from the iteration index. </param>
        public Gh_Weight(Func<int, double> function)
        {
            //     -----     Verifications     -----     //

            ArgumentNullException.ThrowIfNull(function, nameof(function));


            //     -----     Initialisations     -----     //

            // Initialise Fields
            _function = (Func<int, double>)_function!.Clone();
        }


        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Weight" /> class from another <see cref="Gh_Weight"/>.
        /// </summary>
        /// <param name="Gh_Weight"> <see cref="Gh_Weight"/> to deep copy. </param>
        public Gh_Weight(Gh_Weight gh_Weight)
        {
            //     -----     Verifications     -----     //

            if (!gh_Weight.IsValid) { throw new ArgumentException($"The {typeof(Gh_Weight)} to duplicate is not valid."); }


            //     -----     Initialisations     -----     //

            // Initialise Fields
            if (gh_Weight.IsConstant)
            {
                _constant = gh_Weight._constant;
            }
            else if (gh_Weight.IsEvolutive)
            {
                _function = (Func<int, double>)gh_Weight._function!.Clone();
            }
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString()
        {
            if (IsValid && IsConstant)
            {
                return GH_Kernel.GH_Format.FormatDouble(_constant!.Value);
            }
            else if (IsValid && IsEvolutive)
            {
                return "Evolutive Weight";
            }
            else { return $"Invalid {TypeName}"; }
        }

        #endregion


        #region Implement : GH_Types.IGH_Goo

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid => (_constant.HasValue && _function is null) || (_constant is null && _function is not null);

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else
                {
                    if (_constant.HasValue && _function is not null) { return "The grasshopper type stores both a constant and a function."; }
                    else if (_constant is null && _function is null) { return "The grasshopper type stores neither a constant nor a function."; }
                    else { return $"This {TypeName} is not valid, but the reason is not detailed"; }
                }
            }
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeDescription"/>
        public string TypeDescription => $"Grasshopper type representing a weight.";

        /// <inheritdoc cref="GH_Types.IGH_Goo.TypeName"/>
        public string TypeName => nameof(Gh_Weight);


        //     -----     Methods     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.Duplicate"/>
        public GH_Types.IGH_Goo Duplicate() => new Gh_Weight(this);


        /// <inheritdoc cref="GH_Types.IGH_Goo.CastFrom(object)"/>
        public bool CastFrom(object source)
        {
            if (source == null) { return false; }

            var type = source.GetType();

            //     -----     Grasshopper Objects     -----     //

            // Cast a GH_Types.GH_Number to a Gh_Weight
            if (typeof(GH_Types.GH_Number).IsAssignableFrom(type))
            {
                GH_Types.GH_Number gh_Number = (GH_Types.GH_Number)source;
                _constant = gh_Number.Value;

                return true;
            }
            // Cast a GH_Types.GH_String to a Gh_Weight
            else if (typeof(GH_Types.GH_String).IsAssignableFrom(type))
            {
                GH_Types.GH_String gh_String = (GH_Types.GH_String)source;
                try
                {
                    _constant = Convert.ToDouble(gh_String.Value);
                }
                catch (Exception) { return false; }

                return true;
            }


            //     -----     C# Objects     -----     //

            // Cast a double to a Gh_Weight
            if (typeof(double).IsAssignableFrom(type))
            {
                _constant = (double)source;

                return true;
            }


            //     -----     Otherwise     -----     //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{Q}(ref Q)"/>
        public bool CastTo<T>(out T? target)
        {
            target = default;

            //     -----     Grasshopper Objects     -----     //

            // Casts a Gh_Weight to a GH_Types.GH_Number
            if (IsValid & IsConstant & typeof(T).IsAssignableFrom(typeof(GH_Types.GH_Number)))
            {
                GH_Types.GH_Number gh_Number = new GH_Types.GH_Number(_constant!.Value);
                target = (T)(object)gh_Number;

                return true;
            }


            //     -----     C# Objects     -----     //

            if (IsValid & IsConstant & typeof(T).IsAssignableFrom(typeof(double)))
            {
                target = (T)(object)_constant!.Value;

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
