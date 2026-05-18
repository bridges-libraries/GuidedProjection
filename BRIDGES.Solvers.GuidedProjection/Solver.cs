using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Numerics.LinearAlgebra.Matrices.Double;

using BRIDGES.Solvers.GuidedProjection.Interfaces;
using BRIDGES.Solvers.GuidedProjection.Abstracts;

using Helpers = BRIDGES.Solvers.GuidedProjection.Helpers;


namespace BRIDGES.Solvers.GuidedProjection
{
    /// <summary>
    /// Class defining a Guided Projection Algorithm solver.<br/>
    /// The algorithm is described in <see href="https://doi.org/10.1145/2601097.2601213"/>.
    /// </summary>
    public sealed class Solver
    {
        #region Events

        //     -----     For Weigths     -----     //

        /// <summary>
        /// Event raised whenever <see cref="Constraint"/> weights needs to be updated.
        /// </summary>
        private event Action<int>? WeigthUpdate;

        /// <summary>
        /// Raises the event which updates the <see cref="Constraint"/> weights. 
        /// </summary>
        /// <param name="iteration"> Index of the current iteration. </param>
        private void OnWeigthUpdate(int iteration)
        {
            WeigthUpdate?.Invoke(iteration);
        }


        //     -----     For Constraints     -----     //

        /// <summary>
        /// Event raised whenever the members of <see cref="UpdatedConstraintType"/> needs to be updated.
        /// </summary>
        private event Action? ConstraintUpdate;

        /// <summary>
        /// Raises the event which updates the members of <see cref="UpdatedConstraintType"/>.
        /// </summary>
        private void OnConstraintUpdate()
        {
            ConstraintUpdate?.Invoke();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Collection of variables of this <see cref="Solver"/>.
        /// </summary>
        /// <remarks> The use of the <see cref="HashSet{T}"/> collection allows for efficient search, but does not provide indexing. </remarks>
        private readonly HashSet<Variable> _variables;

        /// <summary>
        /// List of constraints of this <see cref="Solver"/>.
        /// <remarks> The use of the <see cref="HashSet{T}"/> collection allows for efficient search, but does not provide indexing. </remarks>
        /// </summary>
        private readonly HashSet<Constraint> _constraints;


        /// <summary>
        /// Array containing the variables of this <see cref="Solver"/>.
        /// </summary>
        private double[] _x;


        //     -----     Utilities     -----     //

        /// <summary>
        /// Identity matrix multiplied by Epsilon*Epsilon.
        /// </summary>
        private Mat.SparseMatrix<double>? _epsEpsIdentity;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of variables of this <see cref="Solver"/>.  
        /// </summary>
        public int VariableCount => _variables.Count;

        /// <summary>
        /// Gets the number of coordinate in the global vector X of this <see cref="Solver"/>.  
        /// </summary>
        public int CoordinateCount { get; private set; } = 0;

        /// <summary>
        /// Gets the number of constraints of this <see cref="Solver"/>.  
        /// </summary>
        public int ConstraintCount => _constraints.Count;


        //     -----     Settings     -----     //

        /// <summary>
        /// Gets or sets the maximum number of iteration after which the solver is stopped.
        /// </summary>
        public int MaxIteration { get; set; }

        /// <summary>
        /// Gets index of the current iteration.
        /// </summary>
        /// <remarks> Zero means that no iteration was runned. </remarks>
        public int Iteration { get; internal set; }


        //     -----     For Solving     -----     //

        /// <summary>
        /// Gets or sets the weight of the distance to the previous iteration.
        /// </summary>
        public double Epsilon { get; private set; }


        //     -----     For Results     -----     //
        
        /// <summary>
        /// Vector containg the variables.
        /// </summary>
        public IReadOnlyList<double> X => _x; 

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Solver"/> class.
        /// </summary>
        /// <param name="epsilon"> The weights of the distance to the previous iteration. </param>
        /// <param name="maxIteration"> The iteration index after which the solver is stopped. </param>
        public Solver(double epsilon, int maxIteration)
        {
            //     -----     Instanciate Fields

            _variables = new HashSet<Variable>();
            _constraints = new HashSet<Constraint>();

            _x = new double[0];


            //     -----    Initialise Properties

            MaxIteration = maxIteration;
            Iteration = 0;

            Epsilon = epsilon;

        }

        #endregion

        #region Public Methods

        //     -----     For Variables     -----     //

        /// <summary>
        /// Creates a variable from its coordinates and adds it to this model.
        /// </summary>
        /// <param name="coordinates"> Coordinates of the variable. </param>
        /// <returns> The newly added <see cref="Variable"/>. </returns>
        public Variable AddVariable(params double[] coordinates)
        {
            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(coordinates);


            Variable variable = new Variable(coordinates);
            AddVariable(variable);

            return variable;
        }

        /// <summary>
        /// Creates a variable from its coordinates and adds it to this model.
        /// </summary>
        /// <param name="coordinates"> List of coordinates of the variable. </param>
        /// <returns> The newly added <see cref="Variable"/>. </returns>
        public Variable AddVariable(IList<double> coordinates)
        {
            //     -----     Verifications

            ArgumentNullException.ThrowIfNull(coordinates);


            Variable variable = new Variable(coordinates);
            AddVariable(variable);

            return variable;
        }


        /// <summary>
        /// Attempts to add the specified variable to this model.
        /// </summary>
        /// <param name="variable"> Variable to add. </param>
        /// <returns> <see langword="true"/> if the variable was added, <see langword="false"/> if the variable already exists in the model. </returns>
        public bool TryAddVariable(Variable variable)
        {
            if(_variables.Contains(variable)) // Complexity : O(1) 
            { 
                return false; 
            }
            else
            {
                AddVariable(variable);

                return true;
            }
        }


        //     -----     For Constraints     -----     //

        /// <summary>
        /// Creates a new <see cref="Constraint"/> with a constant weight and adds it to this model.
        /// </summary>
        /// <param name="constraintType"> Constraint type defining the constraint locally. </param>
        /// <param name="variables"> 
        /// Variables composing the local vector localX.
        /// <para> See the <paramref name="constraintType"/> class description to learn about the expected variables. The order of the variables matters.</para>
        /// </param>
        /// <param name="weight"> Weight for the constraint. </param>
        public Constraint AddConstraint(IConstraintType constraintType, IReadOnlyList<IVariable> variables, double weight = 1.0)
        {
            Constraint constraint = new Constraint(constraintType, variables, weight);

            AddConstraint(constraint);

            return constraint;
        }

        /// <summary>
        /// Creates a new <see cref="Constraint"/> with a varying weight and adds it to this model.
        /// </summary>
        /// <param name="constraintType"> Quadratic constraint type defining the constraint locally. </param>
        /// <param name="variables"> 
        /// Variables composing the local vector localX.
        /// <para> See the <paramref name="constraintType"/> class description to learn about the expected variables. The order of the variables matters.</para>
        /// </param>
        /// <param name="weightFunction"> Function computing the weight from the iteration index. </param>
        public Constraint AddConstraint(IConstraintType constraintType, IReadOnlyList<IVariable> variables, Func<int, double> weightFunction)
        {
            Constraint constraint = new Constraint(constraintType, variables, 0.0);

            AddConstraint(constraint, weightFunction);

            return constraint;
        }


        /// <summary>
        /// Attempts to add the specified constraint to this model.
        /// </summary>
        /// <param name="constraint"> Constraint to add. </param>
        /// <returns> <see langword="true"/> if the constraint was added, <see langword="false"/> if the constraint already exists in the model. </returns>
        public bool TryAddConstraint(Constraint constraint)
        {
            if (_constraints.Contains(constraint)) // Complexity : O(1) 
            {
                return false;
            }
            else
            {
                AddConstraint(constraint);

                return true;
            }
        }

        /// <summary>
        /// Attempts to add the specified constraint with a varying weight to this model.
        /// </summary>
        /// <param name="constraint"> Constraint to add. </param>
        /// <param name="weightFunction"> Function computing the weight from the iteration index. </param>
        /// <returns> <see langword="true"/> if the constraint was added, <see langword="false"/> if the constraint already exists in the model. </returns>
        public bool TryAddConstraint(Constraint constraint, Func<int, double> weightFunction)
        {
            if (_constraints.Contains(constraint)) // Complexity : O(1) 
            {
                return false;
            }
            else
            {
                AddConstraint(constraint, weightFunction);

                return true;
            }
        }


        //     -----     For Solving     -----     //

        /// <summary>
        /// Initialises the members of this <see cref="Solver"/>.
        /// </summary>
        public void Initialise()
        {
            //     -----     Global vector X     -----     //

            InitialiseX();

            //     -----     ε²·I      -----     //

            int[] columnPointers = new int[_x.Length + 1];
            List<int> rowIndices = new List<int>(_x.Length);
            List<double> values = new List<double>(_x.Length);

            double value = Epsilon * Epsilon;
            for (int i = 0; i < _x.Length; i++)
            {
                columnPointers[i] = i;
                rowIndices.Add(i);
                values.Add(value);
            }

            columnPointers[_x.Length] = _x.Length;

            Mat.SparseStorages.CompressedColumn<double> storage = new Mat.SparseStorages.CompressedColumn<double>(_x.Length, _x.Length, ref values, ref rowIndices, ref columnPointers);

            _epsEpsIdentity = new Mat.SparseMatrix<double>(storage);

            //     -----     Iteration     -----     //

            Iteration = 0;
        }

        /// <summary>
        /// Runs one iteration.
        /// </summary>
        public void RunIteration()
        {
            // ----- Iteration Updates ----- //

            OnConstraintUpdate();

            OnWeigthUpdate(Iteration);


            // ----- Formulate and Solve the System ----- //

            Vect.DenseVector<double> x;
            
            x = FormAndSolveSystem();


            // ----- Update Variables ----- //

            // Fill the _x with the actualised values
            for (int i = 0; i < x.Size; i++) { _x[i] = x[i]; }

            Iteration++;
        }

        #endregion

        #region Private Methods

        //     -----     For Variables     -----     //

        /// <summary>
        /// Adds a variable to this model.
        /// </summary>
        /// <param name="variable"> Varaible to add. </param>
        private void AddVariable(Variable variable)
        {
            _variables.Add(variable);
            CoordinateCount += variable.Size;
        }


        //     -----     For Constraints     -----     //

        /// <summary>
        /// Adds a constraint to the this model.
        /// </summary>
        /// <param name="constraint"> Constraint to add. </param>
        private void AddConstraint(Constraint constraint)
        {
            _constraints.Add(constraint);

            if (constraint.ConstraintType is UpdatedConstraintType updatedConstraintType)
            {
                ConstraintUpdate += () =>
                {
                    updatedConstraintType.ComputeMembers(constraint.Variables, out Mat.SparseMatrix<double>? localHi, out Vect.SparseVector<double>? localBi, out double ci);

                    updatedConstraintType.LocalHi = localHi;
                    updatedConstraintType.LocalBi = localBi;
                    updatedConstraintType.Ci = ci;
                };
            }
        }

        /// <summary>
        /// Adds a constraint with a varying weight to the this model.
        /// </summary>
        /// <param name="constraint"> Constraint to add. </param>
        /// <param name="weightFunction"> Function computing the weight from the iteration index. </param>
        private void AddConstraint(Constraint constraint, Func<int, double> weightFunction)
        {
            AddConstraint(constraint);

            WeigthUpdate += (int iteration) => constraint.Weight = weightFunction(iteration);
        }


        //     -----     For Solving     -----     //

        /// <summary>
        /// Initialises the members of this <see cref="Solver"/>.
        /// </summary>
        private void InitialiseX()
        {
            //     -----     Global vector X     -----     //

            double[] array = new double[CoordinateCount];    // CoordinateCount is updated

            int i_Comp = 0;

            foreach (Variable variable in _variables)
            {
                // Set the coordinate in the global vector X
                for (int i_VarComp = 0; i_VarComp < variable.Size; i_VarComp++)
                {
                    array[i_Comp + i_VarComp] = variable[i_VarComp];
                }

                // Change the reference in the variable to point toward a segment of the global vector X
                variable.ChangeReference(array, i_Comp);


                i_Comp += variable.Size;
            }

            _x = array;

            //     -----     Utilities     -----     //

            int[] columnPointers = new int[_x.Length + 1];
            List<int> rowIndices = new List<int>(_x.Length);
            List<double> values = new List<double>(_x.Length);

            double value = Epsilon * Epsilon;
            for (int i = 0; i < _x.Length; i++)
            {
                columnPointers[i] = i;
                rowIndices.Add(i);
                values.Add(value);
            }

            columnPointers[_x.Length] = _x.Length;

            Mat.SparseStorages.CompressedColumn<double> storage = new Mat.SparseStorages.CompressedColumn<double>(_x.Length, _x.Length, ref values, ref rowIndices, ref columnPointers);

            _epsEpsIdentity = new Mat.SparseMatrix<double>(storage);
        }

        /// <summary>
        /// Computes the members of the system and solve it using Cholesky factorisation.
        /// </summary>
        /// <returns> The solution of the system. </returns>
        private Vect.DenseVector<double> FormAndSolveSystem()
        {
            //     -----     Create H and r     -----     //

            FormConstraintMembers(out Mat.SparseMatrix<double> H, out Vect.DenseVector<double> r);

            //     -----     Solve the minimisation problem     -----     //
            
            // Left hand side of the equation
            Mat.SparseMatrix<double> LHS;

            LHS = Mat.SparseMatrix<double>.TransposeMultiply(H, H);
            LHS = Mat.SparseMatrix<double>.Add(LHS, _epsEpsIdentity!);

            // Right hand side of the equation
            Vect.DenseVector<double> RHS; 

            RHS = Mat.SparseMatrix<double>.TransposeMultiply(H, r);

            Vect.DenseVector<double> eeX = new Vect.DenseVector<double>(_x.Length);
            for (int i = 0; i < _x.Length; i++) { eeX[i] = Epsilon * Epsilon * _x[i]; }
            RHS = Vect.DenseVector<double>.Add(RHS, eeX);
            
            // Solving linear system
            return LHS.SolveCholesky(RHS);
        }

        /// <summary>
        /// Forms the global system members derived from the constraints.
        /// </summary>
        /// <param name="H"> The global matrix H. </param>
        /// <param name="r"> The global vector r. </param>
        private void FormConstraintMembers(out Mat.SparseMatrix<double> H, out Vect.DenseVector<double> r)
        {
            Helpers.DictionaryOfKeys dok_H = new Helpers.DictionaryOfKeys(10 * _constraints.Count /* Random */);
            List<double> list_r = new List<double>(_constraints.Count);

            int constraintCount = 0;

            foreach (Constraint cstr in _constraints)
            {
                // Verifications
                if (cstr.Weight == 0d) { continue; }


                IReadOnlyList<IVariable> variables = cstr.Variables;
                IConstraintType constraintType = cstr.ConstraintType;

                int localSize = cstr.Size;

                //     -----     Create the Row Indices     -----     //

                // Translate the local indices of the constraint defined on localX into global indices defined on X.
                List<int> rowIndices = new List<int>(localSize);
                for (int i_Variable = 0; i_Variable < variables.Count; i_Variable++)
                {
                    IVariable variable = variables[i_Variable];
                    for (int i_VarComp = 0; i_VarComp < variable.Size; i_VarComp++)
                    {
                        rowIndices.Add(variable.ReferenceIndex(i_VarComp));
                    }
                }

                //     -----     Create localX     -----     //

                double[] coordinates = new double[localSize];
                for (int i_Comp = 0; i_Comp < localSize; i_Comp++)
                {
                    coordinates[i_Comp] = _x[rowIndices[i_Comp]];
                }
                Vect.DenseVector<double> localX = new Vect.DenseVector<double>(coordinates);


                //     -----     Compute H and r     -----     //


                if (constraintType.LocalHi is null)
                {
                    //     -----     For r     -----     //

                    if (constraintType.Ci == 0.0) { list_r.Add(0d); }
                    else { list_r.Add(cstr.Weight * (-constraintType.Ci)); }


                    //     -----     For H     -----     //

                    // Local cannot be null since LocalHi already is.
                    Vect.DenseVector<double> tmp_Vect = new Vect.DenseVector<double>(constraintType.LocalBi!);


                    for (int i_Comp = 0; i_Comp < localSize; i_Comp++)
                    {
                        if (tmp_Vect[i_Comp] == 0d) { continue; }
                        dok_H.Add(cstr.Weight * tmp_Vect[i_Comp], constraintCount, rowIndices[i_Comp]);
                    }

                    constraintCount++;
                }
                else
                {
                    // Temporary values : HiX and XtHiX
                    Vect.DenseVector<double> tmp_Vect = constraintType.LocalHi * localX;

                    double tmp_Val = Vect.DenseVector<double>.TransposeMultiply(localX, tmp_Vect);


                    //     -----     For r     -----     //

                    if (constraintType.Ci == 0.0) { list_r.Add(cstr.Weight * 0.5 * tmp_Val); }
                    else { list_r.Add(cstr.Weight * ((0.5 * tmp_Val) - constraintType.Ci)); }


                    //     -----     For H     -----     //

                    if (!(constraintType.LocalBi is null))
                    {
                        tmp_Vect = tmp_Vect + constraintType.LocalBi;
                    }

                    for (int i_Comp = 0; i_Comp < localSize; i_Comp++)
                    {
                        if (tmp_Vect[i_Comp] == 0d) { continue; }
                        dok_H.Add(cstr.Weight * tmp_Vect[i_Comp], constraintCount, rowIndices[i_Comp]);
                    }

                    constraintCount++;
                }
            }

            Mat.SparseStorages.CompressedColumn<double> storage = dok_H.ToCompressedColumn(constraintCount, _x.Length);
            H = new Mat.SparseMatrix<double>(storage);

            r = new Vect.DenseVector<double>(list_r.ToArray());
        }

        #endregion
    }
}
