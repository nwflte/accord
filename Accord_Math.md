> _This page is about the Mathematics module of Accord.NET. For general descriptions on Mathematics, please see [Mathematics (Wikipedia)](http://en.wikipedia.org/wiki/Math)._

# Accord.Math #

The Accord.Math namespace contains support classes for almost all other modules in the framework. It features a matrix extension library, alongside a suite of numerical matrix decomposition methods, numerical optimization algorithms for constrained and unconstrained problems, special functions and other tools for scientific applications.

## Matrices ##

Importing the Accord.Math namespace automatically brings support for the Extension Matrix Library, which extends common .NET datatypes with several mathematics-related extension methods.

<p align='center'>
<img src='http://accord.googlecode.com/svn/wiki/samples/accord-math-matrixoperations-img.png' />
</p>

## Decompositions ##

The framework comes with a wide range of numerical [matrix decompositions](http://en.wikipedia.org/wiki/Matrix_decomposition), with dedicated versions for different .NET datatypes. Those decompositions can be used to solve linear system, compute matrix inverses and pseudo-inverses and extract other useful information about the decomposed matrices.

<table width='600px' border='0'>
<tbody align='left'>
<tr>
<th>Decompositions</th>
<th>Multidimensional</th>
<th>Jagged</th>
</tr>
<tr>
<td>
Cholesky<br>
</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_CholeskyDecomposition.htm'>double</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_CholeskyDecompositionF.htm'>float</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_CholeskyDecompositionD.htm'>decimal</a>)</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_JaggedCholeskyDecomposition.htm'>double</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_JaggedCholeskyDecompositionF.htm'>float</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_JaggedCholeskyDecompositionD.htm'>decimal</a>)</td>
</tr>
<tr>
<td>Eigenvalue (EVD)</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_EigenvalueDecomposition.htm'>double</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_EigenvalueDecompositionF.htm'>float</a>)</td>
<td></td>
</tr>
<tr>
<td>Generalized Eigenvalue [<a href='http://crsouza.blogspot.com.br/2010/06/generalized-eigenvalue-decomposition-in.html'>1</a>]</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_GeneralizedEigenvalueDecomposition.htm'>double</a>)</td>
<td></td>
</tr>
<tr>
<td>Nonnegative Factorization</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_NonnegativeMatrixFactorization.htm'>double</a>)</td>
<td></td>
</tr>
<tr>
<td>LU</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_LuDecomposition.htm'>double</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_LuDecompositionF.htm'>float</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_LuDecompositionD.htm'>decimal</a>)</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_JaggedLuDecomposition.htm'>double</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_LuDecompositionF.htm'>float</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_JaggedLuDecompositionD.htm'>decimal</a>)</td>
</tr>
<tr>
<td>QR</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_QrDecomposition.htm'>double</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_QrDecompositionF.htm'>float</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_QrDecompositionD.htm'>decimal</a>)</td>
<td></td>
</tr>
<tr>
<td>Singular value (SVD)</td>
<td>(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_SingularValueDecomposition.htm'>double</a>)(<a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Decompositions_SingularValueDecompositionF.htm'>float</a>)</td>
<td></td>
</tr>
</tbody>
</table>
_`*` Further type variants may be implemented under request_

## Optimization ##

The Accord.Math.Optimization namespace contains classes for constrained and unconstrained optimization. Includes, for example, the [Conjugate Gradient](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Optimization_ConjugateGradient.htm) (CG) and the [Broyden–Fletcher–Goldfarb–Shanno](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Optimization_BroydenFletcherGoldfarbShanno.htm) (BFGS) methods for unconstrained optimization; the [Goldfarb-Idnani](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Optimization_GoldfarbIdnaniQuadraticSolver.htm) solver for Quadratic Programming (QP) problems and the [Augmented Lagrangian](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Optimization_AugmentedLagrangianSolver.htm) method for nonlinear optimization.

Also contains methods for univariate search, such as [Brent's method](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Optimization_BrentSearch.htm) for finding function roots, maximum or minimum values of an univariate function.

## Wavelets ##

Wavelet transform classes, such as the 1D and 2D [Cohen-Daubechies-Feauveau](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Wavelets_CDF97.htm) and [Haar Wavelet](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Wavelets_Haar.htm) transforms.

## Special functions ##

Complete sets of special functions, such as factorials, log-factorials, combinatorics and more specialized functions, such as the [Bessel](http://accord.googlecode.com/svn/docs/html/AllMembers_T_Accord_Math_Bessel.htm), [Gamma](http://accord.googlecode.com/svn/docs/html/AllMembers_T_Accord_Math_Gamma.htm), [Beta](http://accord.googlecode.com/svn/docs/html/AllMembers_T_Accord_Math_Beta.htm) and [Normal (Gaussian)](http://accord.googlecode.com/svn/docs/html/AllMembers_T_Accord_Math_Normal.htm) functions.

## Parsing ##

Contains methods and classes to [convert to and from various matrix formats](http://accord.googlecode.com/svn/docs/html/N_Accord_Math_Formats.htm), such as the formats used by .NET (e.g. [multidimensional](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Formats_CSharpMatrixFormatProvider.htm) and [jagged](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Formats_CSharpJaggedMatrixFormatProvider.htm) C# matrices) and [Octave](http://accord.googlecode.com/svn/docs/html/T_Accord_Math_Formats_OctaveMatrixFormatProvider.htm).

## And many more ##

There are much more in the framework than the classes presented here. For a full description of the Accord.Math namespace, please take a look on the [namespace documentation](http://accord.googlecode.com/svn/docs/Index.html).

<br />

---


> Please take a look on [the integral Accord.NET documentation](http://accord.googlecode.com/svn/docs/Index.html) to get a more detailed overview of the available classes, methods and interfaces.