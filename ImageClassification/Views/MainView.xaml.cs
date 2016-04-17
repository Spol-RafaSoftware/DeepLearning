﻿using System.Linq;
using ImageClassification.Core;
using ImageClassification.Infrastructure;
using ImageClassification.Infrastructure.Logging;
using ImageClassification.Models.Configurations;

namespace ImageClassification.Views
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        public void Run()
        {
            var dataProvider = new DataProvider(new DataProviderConfiguration
            {
                CropWidth = 300,
                CropHeight = 200,
                ProcessingWidth = 30,
                ProcessingHeight = 20,
                FilesLocationPath = @"C:\Users\Szymon\Desktop\101_ObjectCategories",
                FileExtensions = new[] { "jpg" },
                TrainDataRatio = 0.8,
            });

            var availableCategories = dataProvider.GetAvailableCategories();
            var selectedCategories = availableCategories.Take(5).ToList();

            var learningSet = dataProvider.GetLearningSetForCategories(selectedCategories);

            var trainer = new DeepLearningTrainer(new TrainerConfiguration
            {
                Layers = new[] { 600, 400, 5, 5 },
                Categories = selectedCategories,
                InputsOutputsData = learningSet.TrainingData.ToInputOutputsDataNative(),
            }, new Logger());

            trainer.RunTraining1(new Training1Parameters
            {
                Momentum = 0.5,
                Decay = 0.001,
                LearningRate = 0.1,
                UnsupervisedEpochs = 200,
            });

            trainer.RunTraining2(new Training2Parameters
            {
                Momentum = 0.5,
                LearningRate = 0.1,
                SupervisedEpochs = 300,
            });

            trainer.CheckAccuracy(learningSet.TestData.ToInputOutputsDataNative());
        }
    }
}
