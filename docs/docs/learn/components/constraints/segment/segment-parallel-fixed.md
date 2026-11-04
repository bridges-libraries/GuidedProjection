# Segment Parallelity (Fixed Target Direction)

This component creates a `Constraint` enforcing the direction of a segment to be parallel to a fixed direction.
The segment is defined from a start and an end `Variable`. The size of the start and end `Variable` must be the same as the size of the direction vector. 

The formulation of this constraint requires the length of the segment to be enclosed into a scalar `Variable`. Therefore, it is necessary to create the  scalar `Variable` corresponding to the segment length and to accompanied this `Constraint` with the [Segment Length (Var)](./segment-length-variable.md) `Constraint`.