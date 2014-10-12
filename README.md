DataAnalysisTools
=================

This repository contains a few C# tools I have written and found useful in different predictive modelling tasks. There are three main contributions here:

1) Modelling

A few general-purpose tools for use in pattern recognition / predictive modelling tasks. Included so far are classes for cross-validation, feature selection, dataset manipulation, calculating calibration curves, and a modified logistic regression. See the LogRegTest project for an example.

2) GeneticAlgorithm

A versatile genetic algorithm which can accomodate genomes of any type (support is provided for type double[], but the GA can accomodate anything that can be represented by a C# object). Single population and parallelized multi-population genetic algorithms are provided. See the GAtest program for an example.

3) GeneticFuzzyModel

The genetic fuzzy model consists of a fuzzy rule base used for fuzzy inference. The rule base - including the rules themselves and the underlying fuzzy sets - is optimized by a genetic algorithm given a user-supplied set of training data. Currently, the fuzzy rule base only supports one defuzzification method: weighted average. See the GeneticFuzzyModelTest program for an example.


The repository is a work in progress. Exception handling needs to be added, and there are other improvements to be made as well. I welcome any feedback or suggestions!
