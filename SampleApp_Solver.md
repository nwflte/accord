  * Download [source code](http://accord.googlecode.com/svn/wiki/samples/accord-solver-source.zip)
  * Download [sample application](http://accord.googlecode.com/svn/wiki/samples/accord-solver-demo.zip)

The Solver sample application demonstrates how to solve constrained quadratic programming (QP) problems using the dual method of Goldfarb and Idnani (1982). The code is based on the original Fortran implementation by Berwin A. Turlach. The solution of the QP problem shown in the picture can be checked against [WolframAlpha's answer](http://www.wolframalpha.com/input/?i=min+2x%C2%B2%2Bxy%2By%C2%B2-5y%2C+x+%2B+y+%3C%3D+2%2C+x%2By+%3E%3D+1%2C+y+%3E%3D0). The sample can hopefully demonstrate the flexibility on specifying target functions and constraints, which can be specified either using lambda expressions, text strings or constraint matrices.

<br /><p align='center'>
<img src='http://accord.googlecode.com/svn/wiki/samples/accord-math-solver-img.png' />
<br />Constrained optimization solver for QP problems<br>
</p><br />



---

The source code available in this page may contain a stripped down version of the Accord.NET Framework with only the strictly necessary to support the sample application. Thus it is by no means a production version, nor it is supported, and is mainly intended to provide a better understanding of the code. Anyone downloading the sources below is highly encouraged to download the complete framework in order to obtain the latest version of this code, which may have the latest bug fixes and enhancements.