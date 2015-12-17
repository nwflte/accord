> _This page is about the MachineLearning module of Accord.NET. For general descriptions on Machine Learning, please see [Machine learning (Wikipedia)](http://en.wikipedia.org/wiki/Machine_learning)._


# Accord.MachineLearning #

The [Accord.MachineLearning namespace](http://accord.googlecode.com/svn/docs/html/N_Accord_MachineLearning.htm) contains key classes for many machine learning algorithms and related tasks. One of the most prominent tools are the Support Vector Machines (SVMs), located in the [MachineLearning.VectorMachines namespace](http://accord.googlecode.com/svn/docs/html/N_Accord_MachineLearning_VectorMachines.htm). Other interesting applications are given by Decision Trees (DTs), clustering algorithms such as K-Means and meta-algorithms such as Cross-validation and RANSAC.

## Support Vector Machines ##

The Support Vector Machines are a popular class of machine learning methods, with many learning algorithms. The framework includes support for a variety of [kernel functions](http://accord.googlecode.com/svn/docs/html/N_Accord_Statistics_Kernels.htm), including kernels suitable to be used with [sparse data](http://accord.googlecode.com/svn/docs/html/N_Accord_Statistics_Kernels_Sparse.htm) in [LibSVM's format](http://www.csie.ntu.edu.tw/~cjlin/libsvm/) (LibSVM is a great SVM package, comparisons between Accord.NET and LibSVM are highly encouraged).

Accord.NET offers [multi-class](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_VectorMachines_MulticlassSupportVectorMachine.htm) and [multi-label](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_VectorMachines_MultilabelSupportVectorMachine.htm) SVMs. By definition, the SVM is only a binary classifier. To perform the classification of a sample among more than two classes it is necessary to consider a multiple class decision approach. The most common approaches are the one-vs-one and one-vs-all approaches. Another approach is given by Directed Acyclic Graphs (Platt, 1999). The framework [offers all of them](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_VectorMachines_MulticlassComputeMethod.htm).

Sometimes there is also need to produce probabilistic results for the classifiers. The framework implements [output calibration](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_VectorMachines_Learning_ProbabilisticOutputLearning.htm) (Platt, 2000; H.T. Lin, 2007) and is able to produce probabilistic outputs for any of the aforementioned classification approaches.

Moreover, the framework also offers the automatic determination of some learning parameters. For instance, the framework is able to determine [suitable values for C](http://accord.googlecode.com/svn/docs/html/M_Accord_MachineLearning_VectorMachines_Learning_SequentialMinimalOptimization_EstimateComplexity.htm), for the [Gaussian kernel's sigma](http://accord.googlecode.com/svn/docs/html/M_Accord_Statistics_Kernels_Gaussian_Estimate.htm) and [other kernels](http://accord.googlecode.com/svn/docs/html/N_Accord_Statistics_Kernels.htm). Those values are computed using heuristics detailed on the respective method documentations.

## Decision Trees ##

[Decision Trees](http://accord.googlecode.com/svn/docs/html/N_Accord_MachineLearning_DecisionTrees.htm) are among the fastest to evaluate classifiers. The framework implements the [ID3 and C4.5 algorithms](http://accord.googlecode.com/svn/docs/html/N_Accord_MachineLearning_DecisionTrees_Learning.htm) for learning decision trees, although learning is mostly limited to simple scenarios. However, the most useful feature of the framework is certainly the ability of generating machine code versions of Decision Trees on-the-fly, as detailed on the post [Decision Trees in C#](http://crsouza.blogspot.com.br/2012/01/decision-trees-in-c.html).

## Naive Bayes ##

[Naive Bayes](http://accord.googlecode.com/svn/docs/html/N_Accord_MachineLearning_Bayes.htm) classifiers are useful when there is apparent independence between input variables in a learning problem. The framework supports either [discrete](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_Bayes_NaiveBayes.htm) or [arbitrary density models](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_Bayes_NaiveBayes_1.htm) through the use of [Generics](http://en.wikipedia.org/wiki/Generic_programming).

## Meta-algorithms ##

Meta-algorithms are algorithms designed to control other algorithms. For instance, consider the RANSAC algorithm, which can be used to create a robust version of any other classification algorithm; or the cross-validation algorithm, which is able to determine more statistically accurate performance estimates for any learning algorithm.

#### RANdom SAmple Consensus (RANSAC) ####

The [RANdom SAmple Consensus (RANSAC)](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_RANSAC_1.htm) algorithm can be used to turn virtually any algorithm in a robust algorithm. It works by learning nested models in different subsets of the data, hoping it will detect outliers (distinguishable data points which could have been the result of noise contamination or data acquisition error) in the process.

One of the most famous application of the RANSAC algorithm is to help detect inliers in homography estimation and image matching, such as demonstrated in the CodeProject article [Automatic Image Stitching with Accord.NET](http://www.codeproject.com/Articles/95453/Automatic-Image-Stitching-with-Accord-NET). A convenience class for achieving such results is available in the Accord.Imaging namespace: [RansacHomographyEstimator](http://accord.googlecode.com/svn/docs/html/T_Accord_Imaging_RansacHomographyEstimator.htm).

#### Grid Search ####

[Grid-Search](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_GridSearch_1.htm) is a fancy name for selecting the best parameters of a model through a multivariate search. It can be used, for example, to determine the best C and sigma parameters in Gaussian-kernel SVM learning.

#### Cross-validation ####

[Cross-validation](http://accord.googlecode.com/svn/docs/html/T_Accord_MachineLearning_CrossValidation_1.htm) is one of the basic methods for reliable performance assessment, absolutely useful when one have limited amounts of data and wish to obtain performance estimates for a given model.


<br />

---


> Please take a look on [the integral Accord.NET documentation](http://accord.googlecode.com/svn/docs/Index.html) to get a more detailed overview of the available classes, methods and interfaces.