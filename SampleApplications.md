# Sample Applications #

The framework comes with over 25 sample applications demonstrating the framework features. All sample applications are written in C#. You are also invited to watch the video demonstrations in the section below in case you are interested in pattern recognition, image processing and computer vision with the framework.

<table><tr>
<blockquote><td align='center'><a href='http://www.youtube.com/watch?feature=player_embedded&v=tqAfqJsW2Wo' target='_blank'><img src='http://img.youtube.com/vi/tqAfqJsW2Wo/0.jpg' width='425' height=344 /></a></td>
<td align='center'><a href='http://www.youtube.com/watch?feature=player_embedded&v=BesKtH4Qln8' target='_blank'><img src='http://img.youtube.com/vi/BesKtH4Qln8/0.jpg' width='425' height=344 /></a></td>
</blockquote><blockquote></tr>
<tr>
<blockquote><td align='center'>Head tracking with the Accord.Vision namespace.</td>
<td align='center'>Handwritten digit recognition using the Accord.MachineLearning namespace.</td>
</tr></table></blockquote></blockquote>


<br />

# [\*](Accord_Audio.md) Audio #

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='SampleApp_Fourier.md'>Fourier</a></td>
<td>A simple audio spectrum analyzer using Fast Fourier Transform (FFT). Can optionally use audio windows (i.e. Hamming) to reduce bin leakage in the spectrum.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_BeatDetector.md'>Beat Detector</a></td>
<td>A simple beat detector which listens to the default input device and tries to detect peaks in the audio signal. It is a statistics based beat detector in the sense it searches local energy peaks which may contain a beat. The application also demonstrates the usage of the Metronome class of the framework, which allows one to detect the current tempo by "tapping" the metronome.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_Recorder.md'>Recorder</a></td>
<td>A simple wave recorder able to capture sound from the microphone.</td>
</tr>
</table></blockquote>

<br />
# [\*](Accord_Imaging.md) Imaging #

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='SampleApp_ImageClassification.md'>Image Classification</a></td>
<td>Shows how to classify images using the Baf of Visual Words (BoW) model and Support Vector Machines (SVM).</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_FAST.md'>FAST Corners Detector</a></td>
<td>Demonstration of the FAST corners detector in the world-famous Lena Söderberg's picture.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_Harris.md'>Harris Corners Detector</a></td>
<td>Demonstration of the Harris corners detector in the world-famous Lena Söderberg's picture.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_SURF.md'>SURF Features Detector</a></td>
<td>Demonstration of the SURF features detector in the world-famous Lena Söderberg's picture.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_ImageStitching.md'>Image Stitching (panorama)</a></td>
<td>The Panorama sample application demonstrates how the framework can be used to automatically stitches two images together by using the Harris corners detector, Correlation matching, homography estimation, RANSAC and the image blending filter.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_Wavelets.md'>Wavelets</a></td>
<td>Demonstration of the Haar and CDF9/7 wavelet transform for images.</td>
</tr>
</table></blockquote>


<br />
# [\*](Accord_MachineLearning.md) Machine Learning #

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='http://www.codeproject.com/KB/recipes/handwriting-svm.aspx'>Handwriting (Multi-class SVM)</a></td>
<td>Handwritten digits recognition by using Multi-class Kernel Support Vector Machines.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_Ransac.md'>RANdom SAmple Consensus (RANSAC)</a></td>
<td>The RANSAC sample application demonstrates how to use RANSAC to robustly fit a linear regression, avoiding the negative impact of outliers in the regression data.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_KSVM.md'>Kernel Support Vector Machines</a></td>
<td>A sample application demonstrating how to perform classification and regression using (Kernel) Support Vector Machines. The sample datasets which can be used in the application can be found in the Resources folder in the application main directory.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_KMeans.md'>K-Means Color Clustering</a></td>
<td>A sample application demonstrating the use of K-Means for color reduction (color clustering) in images.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_GMM.md'>Gaussian Mixture Models (GMM)</a></td>
<td>Multivariate Gaussian mixture distribution fitting using Expectation-Maximization. The method is first initialized using K-Means clustering.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_DecisionTrees.md'>Decision Trees (DT)</a></td>
<td>Decision tree learning with ID3 and C4.5 algorithms.</td>
</tr>
</table></blockquote>


<br />
# [\*](Accord_Math.md) Math #

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='Sample_App_Solver.md'>Quadratic Programming (QP) Solver</a></td>
<td>Quadratic Programming (QP) problem solving using the dual method of Goldfarb and Idnani. Translated from the original Fortran code by Berwin A. Turlach.</td>
</tr>
</table></blockquote>


<br />
# [\*](Accord_Neuro.md) Neuro #

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='SampleApp_LevenbergMarquardt.md'>Levenberg-Marquardt</a></td>
<td>An adaptation of the original AForge.NET Neuro sample applications to work with Levenberg-Marquardt instead of Backpropagation. Includes solutions for approximation, time-series prediction and the exclusive-or (XOR) problem using neural networks trained by Levenberg-Marquardt.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_ResilientBackpropagation.md'>Resilient Backpropagation (RProp)</a></td>
<td>An adaptation of the original AForge.NET Neuro sample applications to work with the parallel Resilient Backpropagation instead of the standard Backpropagation. Includes solutions for approximation, time-series prediction and the exclusive-or (XOR) problem using neural networks trained by RProp.</td>
</tr>
</table></blockquote>


<br />
# [\*](Accord_Statistics.md) Statistics #

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='http://www.codeproject.com/KB/recipes/handwriting-kda.aspx'>Handwriting (KDA)</a></td>
<td>Handwritten digits recognition by using Non-linear (Multiple) Discriminant Analysis using Kernels (KDA).</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_HiddenMarkovModels.md'>Hidden Markov Models</a></td>
<td>Demonstrates how to use Hidden Markov Models (HMMs) and Accord.NET Markov Sequence Classifiers to recognize sequences of discrete observations.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_KernelDiscriminativeAnalysis.md'>Kernel Discriminant Analysis (KDA)</a></td>
<td>Sample application demonstrating how to use Kernel Discriminant Analysis (also known as KDA, or ''Non-linear (Multiple) Discriminant Analysis using Kernels'') to perform non-linear transformation and classification. The sample datasets which can be used in the application are available under the Resources folder in the main directory of the application.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_KernelPrincipalComponentAnalysis.md'>Kernel Principal Component Analysis (KPCA)</a></td>
<td>Sample application demonstrating how to use Kernel Principal Component Analysis (KPCA) to perform non-linear transformations and dimensionality reduction. The sample datasets which can be used in the application are available under the Resources folder in the main directory of the application.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_LinearDiscriminantAnalysis.md'>Linear Discriminant Analysis (LDA)</a></td>
<td>Sample application demonstrating how to use Linear Discriminant Analysis (also known as LDA, or ''Fisher's (Multiple) Linear Discriminant Analysis'') to perform linear transformations and classification. The sample datasets which can be used in the application are available under the Resources folder in the main directory of the application.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_PrincipalComponentAnalysis.md'>Principal Component Analysis (PCA)</a></td>
<td>Sample application demonstrating how to use Principal Component Analysis (PCA) to perform linear transformations and dimensionality reduction. The sample datasets which can be used in the application are available under the Resources folder in the main directory of the application.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_IndependentComponentAnalysis.md'>Independent Component Analysis (ICA)</a></td>
<td>Sample application demonstrating how to use Independent Component Analysis (ICA) to perform blind source separation of audio signals. The audio is processed using the Accord.Audio modules of the framework.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_PartialLeastSquares.md'>Partial Least Squares (PLS)</a></td>
<td>Demonstrates how to use Partial Least Squares to fit a (multiple and multivariate) linear regression model from high-dimensionality data.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_LinearLogisticRegression.md'>Linear and Logistic Regression Analysis</a></td>
<td>Sample application for creating and fitting Logistic Regression models. Also fits a multiple linear regression model for comparison purposes, and performs chi-square tests and computes Wald's statistics for the logistic regression coefficients.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_ReceiverOperatingCharacteristic.md'>Receiver Operating Characteristic (ROC) Curves</a></td>
<td>Sample application demonstrating how to create and visualize Receiver-Operating Characteristic Curves from a given set of results from a test or a classification process.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_MouseGestures.md'>Mouse Gestures</a></td>
<td>Learning and recognition of mouse gestures using  <a href='http://accord.googlecode.com/svn/docs/html/T_Accord_Statistics_Models_Markov_HiddenMarkovClassifier.htm'>hidden Markov model-based classifiers</a>.</td>
</tr>
</table></blockquote>


<br />
# [\*](Accord_Vision.md) Vision #

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='SampleApp_FaceDetection.md'>Face Detection (Haar object detector)</a></td>
<td>Face detection using the Face detection based in Haar-like rectangular features method often known as the Viola-Jones method.</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_FaceTracking.md'>Face Tracking (Camshift)</a></td>
<td>Face (or object) tracking using Viola-Jones for face detection and Camshift as the object tracker. Can be used in RGB and HSL color spaces (may require some tuning for HSL).</td>
</tr>
<tr>
<td width='104px'><a href='SampleApp_Controller.md'>Head-based Controller</a></td>
<td>Sample application demonstrating how to use the Accord.Vision.Controls.Controller component to provide joystick-like controls for a Windows Form application. Its component design makes adding support for head-based controlling as easy as dragging and dropping a component into a Form.</td>
</tr>
</table></blockquote>



<br />
# **Video #**

<table width='840px' cellspacing='12px'>
<tr>
<blockquote><td width='104px'><a href='https://code.google.com/p/screencast-capture/'>Screencast Capture Lite</a></td>
<td>Screencast Capture Lite is a tool for recording the desktop screen and saving it to a video file, preserving quality as much as possible. It is a real application, and also a demonstration of the use of the AForge.NET and Accord.NET Frameworks to build multimedia applications, capturing video and audio streams from different sources.</td>
</tr>
</table>