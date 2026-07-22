using System;
using System.Collections.Generic;

using Mat = BRIDGES.Numerics.LinearAlgebra.Matrices;
using Vect = BRIDGES.Numerics.LinearAlgebra.Vectors;

using BRIDGES.Numerics.LinearAlgebra.Matrices.Double;

using BRIDGES.Solvers.GuidedProjection.Interfaces;
using BRIDGES.Solvers.GuidedProjection.Abstracts;


namespace BRIDGES.Solvers.GuidedProjection
{
    

    /// <summary>
    /// Class defining a Guided Projection Algorithm solver.<br/>
    /// The algorithm is described in <see href="https://doi.org/10.1145/2601097.2601213"/>.
    /// </summary>
    public sealed class Solver
    {
        /// <summary>
        /// Status of the Sovler.
        /// </summary>
        public enum SolverStatus : short
        {
            /// <summary>
            /// The solver as been created but does not contain data.
            /// </summary>
            Empty = 0,


            //     -----     For Updates     -----     //

            /// <summary>
            /// The data of the solver has been modified (either the Variables or the Constraints)
            /// </summary>
            Modified = 10,

            /// <summary>
            /// The variables or the constraints have been modified.
            /// </summary>
            AddedData = 20,
            /// <summary>
            /// The variables have been modified.
            /// </summary>
            AddedVariables = 21,
            /// <summary>
            /// The constraints have been modified.
            /// </summary>
            AddedConstraints = 22,


            //     -----     For Preparation     -----     //

            /// <summary>
            /// The solver is being prepared.
            /// </summary>
            Preparing = 100,
            /// <summary>
            /// The solver is ready to run.
            /// </summary>
            Prepared = 101,


            //     -----     For Building     -----     //

            /// <summary>
            /// The matrix <see cref="_H"/> and the column vector <see cref="_r"/> are being assembled.
            /// </summary>
            Assembling = 110,
            /// <summary>
            /// The matrix <see cref="_H"/> and the column vector <see cref="_r"/> are assembled.
            /// </summary>
            Assembled = 111,


            //     -----     For Solving     -----     //

            /// <summary>
            /// The solver is running.
            /// </summary>
            Solving = 120,
            /// <summary>
            /// The solver is runned.
            /// </summary>
            Solved = 121,


            //     -----     For Final Status     -----     //

            /// <summary>
            /// The solving has been stopped because of the number of iteration.
            /// </summary>
            Stopped = 200,

            /// <summary>
            /// The global remainder reached a low value.
            /// </summary>
            Converged = 210,

            /// <summary>
            /// The vector X is not significantly changing.
            /// </summary>
            Stagnates = 220,
        }


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
        /// Array containing the current values of the variables of this <see cref="Solver"/>.
        /// </summary>
        private double[] _x;

        private Mat.SparseMatrix<double> _H;

        private Vect.DenseVector<double> _r;

        private double _remainder;


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
        /// Gets index of the current iteration.
        /// </summary>
        /// <remarks> Zero means that no iteration was runned. </remarks>
        public int Iteration { get; internal set; }

        /// <summary>
        /// Gets or sets the maximum number of iteration.
        /// </summary>
        public int MaxIteration { get; set; }

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

        /// <summary>
        /// Gets the Status of te Solver.
        /// </summary>
        public SolverStatus Status { get; private set; } = SolverStatus.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises a new instance of the <see cref="Solver"/> class.
        /// </summary>
        /// <param name="epsilon"> The weights of the distance to the previous iteration. </param>
        public Solver(double epsilon)
        {
            //     -----     Instanciate Fields

            _variables = new HashSet<Variable>();
            _constraints = new HashSet<Constraint>();

            _x = new double[0];

            //     -----    Initialise Properties

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

        //     -----     For Solving     -----     //

        public void Run(int maxIteration)
        {
            while (Status < SolverStatus.Stopped)
            {
                RunOneIteration();
            }
        }


        public void RunOneIteration()
        {
            //     -----     Actualise Status     -----     //

            if (Status < SolverStatus.Prepared) { Prepare(); }

            if (Status < SolverStatus.Assembled) { Assemble(); }


            // ----- Solve ----- //

            Vect.DenseVector<double> x = Solve();

            double previousRemainder = _remainder;
            double[] previousX = new double[_x.Length];
            Array.Copy(_x, previousX, _x.Length);

            _x = x.ToArray();

            //     -----     Compute H, r and the remainder    -----     //

            Assemble();

            //     -----     Convergence     -----     //

            EvaluateConvergence(previousX, previousRemainder);

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

            ManageStatus_AddedVariable();
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


        //     -----     For Preparation     -----     //

        /// <summary>
        /// Prepares the solver for running after updates.
        /// </summary>
        /// <returns> <see langword="true"/> if the preparation suceeded, <see langword="false"/> otherwise. </returns>
        private void Prepare()
        {
            Status = SolverStatus.Preparing;

            Prepare_X();
            Prepare_eeI();

            Status = SolverStatus.Prepared;
        }


        /// <summary>
        /// Fills the vector <see cref="X"/> and updates the reference of the <see cref="_variables"/>.
        /// </summary>
        /// <returns> <see langword="true"/> if the preparation suceeded, <see langword="false"/> otherwise. </returns>
        private void Prepare_X()
        {
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
        }

        private void Prepare_eeI()
        {
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
        }


        //     -----     Building the System     -----     //

        /// <summary>
        /// Assemble the matrix <see cref="_H"/>, and the column vector <see cref="_r"/>.
        /// </summary>
        private void Assemble()
        {
            Status = SolverStatus.Assembling;

            //     -----     Update Constraints     -----     //

            OnConstraintUpdate();

            OnWeigthUpdate(Iteration + 1);

            //     -----     Assemble Constraints     -----     //

            AssembleConstraintMembers(out Mat.SparseMatrix<double> H, out Vect.DenseVector<double> r, out double remainder);

            //     -----     Finalise     -----     //

            _H = H;
            _r = r;
            _remainder = remainder;

            Status = SolverStatus.Assembled;
        }


        /// <summary>
        /// Forms the global system members derived from the constraints.
        /// </summary>
        /// <param name="H"> The global matrix H. </param>
        /// <param name="r"> The global vector r. </param>
        /// <param name="remainder"> sum of the squared remainders of each constraint. </param>
        private void AssembleConstraintMembers(out Mat.SparseMatrix<double> H, out Vect.DenseVector<double> r,
            out double remainder)
        {
            Helpers.DictionaryOfKeys dok_H = new Helpers.DictionaryOfKeys(10 * _constraints.Count /* Random */);
            List<double> list_r = new List<double>(_constraints.Count);

            remainder = 0d;
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

                    // LocalBi cannot be null since LocalHi already is.
                    foreach ((int Index, double Value) component in constraintType.LocalBi!.NonZeros())
                    {
                        dok_H.Add(cstr.Weight * component.Value, constraintCount, rowIndices[component.Index]);
                    }


                    //     -----     For remainders     -----     //

                    double tmp_BiLocalX = Vect.DenseVector<double>.TransposeMultiply(constraintType.LocalBi, localX);

                    remainder += Math.Pow(cstr.Weight * (tmp_BiLocalX + constraintType.Ci), 2);

                    //     -----     Finalise     -----     //

                    constraintCount++;
                    continue;
                }
                else
                {
                    // Temporary values : HiX and XtHiX
                    Vect.DenseVector<double> tmp_Vect = constraintType.LocalHi * localX;

                    double tmp_LocalXHiLocalX = Vect.DenseVector<double>.TransposeMultiply(localX, tmp_Vect);


                    //     -----     For r     -----     //

                    if (constraintType.Ci == 0.0) { list_r.Add(cstr.Weight * 0.5 * tmp_LocalXHiLocalX); }
                    else { list_r.Add(cstr.Weight * ((0.5 * tmp_LocalXHiLocalX) - constraintType.Ci)); }


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


                    //     -----     For remainders     -----     //

                    if (!(constraintType.LocalBi is null))
                    {
                        remainder += Math.Pow(cstr.Weight * ((0.5 * tmp_LocalXHiLocalX) + constraintType.Ci), 2);
                    }
                    else
                    {
                        double tmp_BiLocalX = Vect.DenseVector<double>.TransposeMultiply(constraintType.LocalBi!, localX);
                        remainder += Math.Pow(cstr.Weight * ((0.5 * tmp_LocalXHiLocalX) + tmp_BiLocalX + constraintType.Ci), 2);
                    }


                    //     -----     Finalise     -----     //

                    constraintCount++;
                    continue;
                }
            }

            Mat.SparseStorages.CompressedColumn<double> storage = dok_H.ToCompressedColumn(constraintCount, _x.Length);
            H = new Mat.SparseMatrix<double>(storage);

            r = new Vect.DenseVector<double>(list_r.ToArray());
        }


        //     -----     Solving the System     -----     //

        /// <summary>
        /// Solve the linear system using Cholesky factorisation.
        /// </summary>
        /// <returns> The solution of the system. </returns>
        private Vect.DenseVector<double> Solve()
        {
            Status = SolverStatus.Solving;

            //     -----     Solve the minimisation problem     -----     //

            // Left hand side of the equation
            Mat.SparseMatrix<double> LHS;

            LHS = Mat.SparseMatrix<double>.TransposeMultiply(_H, _H);
            LHS = Mat.SparseMatrix<double>.Add(LHS, _epsEpsIdentity!);

            // Right hand side of the equation
            Vect.DenseVector<double> RHS;

            RHS = Mat.SparseMatrix<double>.TransposeMultiply(_H, _r);

            Vect.DenseVector<double> eeX = new Vect.DenseVector<double>(_x.Length);
            for (int i = 0; i < _x.Length; i++) { eeX[i] = Epsilon * Epsilon * _x[i]; }
            RHS = Vect.DenseVector<double>.Add(RHS, eeX);

            // Solving linear system
            Vect.DenseVector<double> x = LHS.SolveCholesky(RHS);

            //     -----     Finalise     -----     //

            Status = SolverStatus.Solved;
            Iteration++;
            return x;
        }


        //     -----     Evaluate Convergence     -----     //

        private void EvaluateConvergence(double[] previousX, double previousRemainder)
        {

            // Evaluate for exceeding iteration; 
            if (Iteration >= MaxIteration)
            {
                Status = SolverStatus.Stopped;
                return;
            }
            // Evaluate for convergence 
            if (_remainder > previousRemainder)
            {
                Status = SolverStatus.Converged;
                _x = previousX;
                return;
            }

            // Evaluate for stagnation
            double dist = 0d;
            for (int i = 0; i < _x.Length; i++) { dist = Math.Pow(_x[i] - previousX[i], 2); }
            if (dist < 1e-12) 
            { 
                Status = SolverStatus.Stagnates;
                return;
            }

        }

        #endregion

        #region Status Methods

        /// <summary>
        /// Changes the status of the solver with regard to the proposed <paramref name="status"/>.
        /// </summary>
        /// <param name="status"> Proposed status.</param>
        private void ManagedStatus(SolverStatus status)
        {
            switch (status)
            {
                case SolverStatus.AddedVariables: ManageStatus_AddedVariable(); break;
                case SolverStatus.AddedConstraints: ManageStatus_AddedConstraint(); break;
                default: throw new NotImplementedException($"The change of status to {status.ToString()} has not been implemented.");
            }
        }

        /// <summary>
        /// Changes the status of the solver when a variable is added.
        /// </summary>
        private void ManageStatus_AddedVariable()
        {
            switch (Status)
            {
                case SolverStatus.Empty: 
                    Status = SolverStatus.AddedVariables; break;
                case SolverStatus.Modified | SolverStatus.AddedData | SolverStatus.AddedVariables: 
                    break;
                case SolverStatus.AddedConstraints: 
                    Status = SolverStatus.AddedData; break;

                default: 
                    Status = SolverStatus.AddedVariables; break;
            }
        }

        /// <summary>
        /// Changes the status of the solver when a constraint is added.
        /// </summary>
        /// <exception cref="NotImplementedException"> The change of status from <see cref="Status"/> to <see cref="SolverStatus.AddedConstraints"/> has not been implemented.</exception>
        private void ManageStatus_AddedConstraint()
        {
            switch (Status)
            {
                case SolverStatus.Empty:
                    Status = SolverStatus.AddedConstraints; break;
                case SolverStatus.Modified | SolverStatus.AddedData | SolverStatus.AddedConstraints: 
                    break;
                case SolverStatus.AddedVariables: 
                    Status = SolverStatus.AddedData; break;

                default:
                    Status = SolverStatus.AddedConstraints; break;
            }
        }

        #endregion
    }
}
