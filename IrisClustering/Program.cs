﻿using Microsoft.ML;
using IrisClustering;


string _dataPath = Path.Combine("Dados", "iris.data");
string _modelPath = Path.Combine("Dados", "IrisClusteringModel.zip");

var mlContext = new MLContext(seed: 0);
IDataView dataView = mlContext.Data.LoadFromTextFile<IrisData>(_dataPath, hasHeader: false, separatorChar: ',');

string featuresColumnName = "Features";
var pipeline = mlContext.Transforms
    .Concatenate(featuresColumnName, "SepalLength", "SepalWidth", "PetalLength", "PetalWidth")
    .Append(mlContext.Clustering.Trainers.KMeans(featuresColumnName, numberOfClusters: 3));

var model = pipeline.Fit(dataView);

using (var fileStream = new FileStream(_modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
{
    mlContext.Model.Save(model, dataView.Schema, fileStream);
}

var predictor = mlContext.Model.CreatePredictionEngine<IrisData, ClusterPrediction>(model);

var prediction = predictor.Predict(TestIrisData.Setosa);
Console.WriteLine($"Cluster: {prediction.PredictedClusterId}");
Console.WriteLine($"Distances: {string.Join(" ", prediction.Distances)}");