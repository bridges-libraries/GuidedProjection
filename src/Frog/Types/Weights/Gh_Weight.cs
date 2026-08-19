using System;

using GH_Kernel = Grasshopper.Kernel;
using GH_Types = Grasshopper.Kernel.Types;

using GP = BRIDGES.Solvers.GuidedProjection;


namespace Frog.Types.Weights
{
    /// <summary>
    /// Class defining a grasshopper type which represents a weight for a <see cref="GP.Constraint"/>.
    /// </summary>
    /// <remarks> The weight can be either constant or evolutive. </remarks>
    public class Gh_Weight : GH_Types.IGH_Goo
    {
        #region Fields

        /// <summary>
        /// Weight function computed from the iteration index and returning a <see langword="double"/>-precision value.
        /// </summary>
        private Func<int, double> _function;


        //     -----     Constant Weight     -----     //

        /// <summary>
        /// Constant <see langword="double"/>-precision value weight.
        /// </summary>
        private double? _constant;


        //     -----     Evolutive Weight     -----     //

        /// <summary>
        /// Expression from which the <see cref="_function"/> is built by parsing and wrapping.
        /// </summary>
        private string? _expression;

        #endregion

        #region Properties

        /// <summary>
        /// Evaluates whether this weight is constant.
        /// </summary>
        public bool IsConstant => _constant.HasValue && _expression is null;

        /// <summary>
        /// Evaluates whether this weight is evolutive.
        /// </summary>
        public bool IsEvolutive => _constant is null && _expression is not null;


        /// <summary>
        /// Gets the function computing the weight from the iteration index. 
        /// </summary>
        public Func<int, double> Function => _function;

        #endregion

        #region Constructors

// Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning disable CS8618

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Weight" /> class. 
        /// The weight is initialised to a constant value of 1.0.
        /// </summary>

        public Gh_Weight()
        {
            //     -----     Initialise Fields

            TryChangeWeight(1d);
        }

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Weight" /> class from a constant value.
        /// </summary>
        /// <param name="weight"> Value of the constant weight. </param>
        public Gh_Weight(double weight)
        {
            //     -----     Initialisations     -----     //

            if (!TryChangeWeight(weight))
            {
                throw new ArgumentOutOfRangeException(nameof(weight), weight, "The weight must be non-negative.");
            }
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

            if (!TryChangeWeight(function))
            {
                throw new ArgumentException("The function could not be stored.", nameof(function));
            }
            
        }

#pragma warning restore CS8618 

        /// <summary>
        /// Initialises a new instance of <see cref= "Gh_Weight" /> class from another <see cref="Gh_Weight"/>.
        /// </summary>
        /// <param name="Gh_Weight"> <see cref="Gh_Weight"/> to shallow copy. </param>
        public Gh_Weight(Gh_Weight gh_Weight)
        {
            //     -----     Verifications     -----     //

            ArgumentNullException.ThrowIfNull(gh_Weight, nameof(gh_Weight));

            //     -----     Initialisations     -----     //

            _constant = gh_Weight._constant;
            _expression = gh_Weight._expression;
            _function = gh_Weight._function;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Evaluates the weight at a given iteration index.
        /// </summary>
        /// <param name="i"> Index of the iteration. </param>
        /// <returns> The <see langword="double"/>-precision value of the weight. </returns>
        public double EvaluateAt(int i) => _function.Invoke(i);

        #endregion

        #region Private Methods

        private bool TryChangeWeight(double constant)
        {
            //     -----     Verifications

            if(constant < 0d) { return false; }


            //     -----

            _constant = constant;
            _expression = null;

            _function = (int i) => { return constant; };

            return true;
        }

        private bool TryChangeWeight(string expression)
        {
            //     -----     Verifications

            if (expression is null || expression == string.Empty) { return false; }


            //     -----     For Constant

            if (double.TryParse(expression, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double c)
                || double.TryParse(expression, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out c))
            {
                return TryChangeWeight(c);
            }

            //     -----     For Evolutive

            string cleanExpression;
            try { cleanExpression = GH_Kernel.Expressions.GH_ExpressionSyntaxWriter.RewriteAll(expression); }
            catch { return false; }

            var parser = new GH_Kernel.Expressions.GH_ExpressionParser();
            if (!parser.CacheSymbols(cleanExpression)) { return false; }

            // Probe once so an unusable expression is rejected at cast time.
            try
            {
                parser.ClearVariables();
                parser.AddVariable("i", 1);
                var probe = parser.Evaluate();
                if (probe is null || !probe.IsNumeric) { return false; }
            }
            catch { return false; }

            // Used to lock to avoid clashes when accessed multiple times.
            object gate = new object();

            _constant = null;
            _expression = expression;
            _function = i =>
            {
                lock (gate)
                {
                    parser.ClearVariables();
                    parser.AddVariable("i", i);

                    var variant = parser.Evaluate();
                    if (variant is null || !variant.IsNumeric)
                    {
                        throw new InvalidOperationException(
                            $"The weight expression '{expression}' did not evaluate to a number at iteration {i}.");
                    }
                    return variant._Double;
                }
            };

            return true;
        }

        private bool TryChangeWeight(Func<int,double> function)
        {
            _constant = null;
            _expression = "";
            _function = function;

            return true;
        }

        #endregion


        #region Override : Object

        /// <inheritdoc cref="object.ToString()"/>
        public override string ToString()
        {
            if (!IsValid)
            {
                return $"Invalid {TypeName}";
            }
            else if (IsConstant)
            {
                return GH_Kernel.GH_Format.FormatDouble(_constant!.Value);
            }
            else // IsEvolutive
            {
                if ( 0 < _expression?.Length) { return _expression!; }
                else { return "Evolutive Weight"; }
            }
        }

        #endregion


        #region Implement : GH_Types.IGH_Goo

        //     -----     Properties     -----     //

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValid"/>
        public bool IsValid => IsConstant || IsEvolutive;

        /// <inheritdoc cref="GH_Types.IGH_Goo.IsValidWhyNot"/>
        public string IsValidWhyNot
        {
            get
            {
                if (IsValid) { return string.Empty; }
                else
                {
                    if (_constant is null && _expression is null) { return "The grasshopper type stores neither a constant nor a function."; }
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
            //     -----     Verifications

            if (source == null) { return false; }


            var type = source.GetType();

            //     -----     From Grasshopper Objects

            // Cast a GH_Types.GH_Number to a Gh_Weight
            if (typeof(GH_Types.GH_Number).IsAssignableFrom(type))
            {
                GH_Types.GH_Number gh_Number = (GH_Types.GH_Number)source;
                return TryChangeWeight(gh_Number.Value);
            }
            // Cast a GH_Types.GH_String to a Gh_Weight
            else if (typeof(GH_Types.GH_String).IsAssignableFrom(type))
            {
                GH_Types.GH_String gh_String = (GH_Types.GH_String)source;
                return TryChangeWeight(gh_String.Value);
            }


            //     -----     From C# Objects     -----     //

            // Cast a double to a Gh_Weight
            if (typeof(double).IsAssignableFrom(type))
            {
                return TryChangeWeight((double)source);
            }

            // Cast a Func<int,double> to a Gh_Weight
            if (typeof(Func<int, double>).IsAssignableFrom(type))
            {
                return TryChangeWeight((Func<int, double>)source);
            }

            //     -----     Otherwise     -----     //

            return false;
        }

        /// <inheritdoc cref="GH_Types.IGH_Goo.CastTo{T}(out T)"/>
        public bool CastTo<T>(out T? target)
        {
            target = default;

            //     -----     To Grasshopper Objects     -----     //

            // Casts a Gh_Weight to a GH_Types.GH_Number
            if (IsValid && IsConstant && typeof(T).IsAssignableFrom(typeof(GH_Types.GH_Number)))
            {
                GH_Types.GH_Number gh_Number = new GH_Types.GH_Number(_constant!.Value);
                target = (T)(object)gh_Number;

                return true;
            }


            //     -----     To C# Objects     -----     //

            if (IsValid && IsConstant && typeof(T).IsAssignableFrom(typeof(double)))
            {
                target = (T)(object)_constant!.Value;

                return true;
            }

            if (IsValid && typeof(T).IsAssignableFrom(typeof(Func<int, double>)))
            {
                target = (T)(object)_function;

                return true;
            }

            //     -----     Otherwise     -----     //

            return false;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.ScriptVariable()"/>
        public object ScriptVariable() => _constant ?? (object)_function!;


        /// <inheritdoc cref="GH_IO.GH_ISerializable.Write(GH_IO.Serialization.GH_IWriter)"/>
        public bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            if (IsConstant) 
            { 
                writer.SetDouble("Constant", _constant!.Value); 
                return true; 
            }
            else if (0 < _expression?.Length) 
            { 
                writer.SetString("Expression", _expression); 
                return true; 
            }
            // Serialisation not managed is the evolutive function is not wrapping an expression.

            return false;   
        }
        

        /// <inheritdoc cref="GH_IO.GH_ISerializable.Read(GH_IO.Serialization.GH_IReader)"/>
        public bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            if (reader.ItemExists("Constant")) 
            { 
                return TryChangeWeight(reader.GetDouble("Constant")); 
            }
            else if (reader.ItemExists("Expression")) 
            { 
                return TryChangeWeight(reader.GetString("Expression")); 
            }

            return false;
        }


        /// <inheritdoc cref="GH_Types.IGH_Goo.EmitProxy()"/>
        public GH_Types.IGH_GooProxy? EmitProxy() => null;

        #endregion
    }
}
