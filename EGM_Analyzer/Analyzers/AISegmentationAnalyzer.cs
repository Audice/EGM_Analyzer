using EGM_Analyzer.Analyzers.Classes;
using EGM_Analyzer.DataLoader;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NumSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace EGM_Analyzer.Analyzers
{
    public class AISegmentationAnalyzer : IAnalyzer
    {
        public List<IClass>? Marks
        {
            get; private set;
        }
        public AnalyzeType AnalyzeType
        {
            get
            {
                return AnalyzeType.Segmentation;
            }
        }
        public string? ModelPath
        {
            get; private set;
        }

        public long[]? AnalyzeShape
        {
            get; private set;
        }

        public long AnalizeSignalSize = 0;

        public AISegmentationAnalyzer(string modelPath, List<IClass> marks)
        {
            ModelPath = modelPath;
            ModelInitialization();
            Marks = new List<IClass>(marks);
            AnalizeSignalSize = AnalyzeShape[2];
        }

        public IAnalysisResult? AnalysisResult
        {
            get; private set;
        }

        public Task Analyze(IDataLoader dataLoader)
        {
            return Task.Run(() =>
            {
                if (dataLoader.Data == null)
                    throw new ArgumentNullException("Ошибка загрузки сигнала!");

                int samplesNum = 20100;//dataLoader.Data[0].Count; //нахлёст 100 сэмплов... для верности начнём искать с двух нахлёстов...
                AnalysisResult = new SegmentationResult((byte)dataLoader.Data.Count, AnalyzeShape, Marks);

                long startPart = 0;

                while (startPart + AnalizeSignalSize < samplesNum)
                {
                    var newPart = AnalyzePart(dataLoader.GetSubSignal(startPart, AnalizeSignalSize), (byte)dataLoader.Data.Count);
                    AnalysisResult.Concatinate(newPart);
                    startPart += AnalizeSignalSize;
                }

            });
        }

        

        private IAnalysisResult? AnalyzePart(Data<float> dataPart, byte channelNumber)
        {
            float[] inputData = dataPart.GetOneDimData();

            IAnalysisResult? analysisResult;

            using (var session = new InferenceSession(ModelPath))
            {
                var inputMeta = session.InputMetadata;
                var container = new List<NamedOnnxValue>();

                foreach (var name in inputMeta.Keys)
                {
                    var tensor = new DenseTensor<float>(inputData, inputMeta[name].Dimensions);
                    container.Add(NamedOnnxValue.CreateFromTensor<float>(name, tensor));
                }

                analysisResult = new SegmentationResult(channelNumber, AnalyzeShape, Marks);

                // Run the inference
                using (var results = session.Run(container))
                {
                    // Get the results
                    foreach (var r in results)
                    {
                        var prediction = r.AsTensor<float>();
                        var a = prediction.ToArray();
                        analysisResult.AppendPart(dataPart.StartSample.Value, dataPart.EndSample.Value, a);
                    }
                }
            }

            return analysisResult;
        }

        public void ModelInitialization()
        {
            if (ModelPath == null)
                throw new ArgumentNullException("Путь до модели не указан или некорректен!");
            using var session = new InferenceSession(ModelPath);
            AnalyzeShape = Array.ConvertAll(session.InputMetadata.First().Value.Dimensions, Convert.ToInt64);
            



            Array multiDimensionalArray = Array.CreateInstance(typeof(float), AnalyzeShape);
            Data<float> example = new Data<float>()
            {
                Signal = multiDimensionalArray,
                Shape = AnalyzeShape,
                StartSample = 0,
                EndSample = 10000
            };

            using var inputOrtValue = OrtValue.CreateTensorValueFromMemory(example.GetOneDimData(), AnalyzeShape);

            // Create input data for session. Request all outputs in this case.
            var inputs1 = new Dictionary<string, OrtValue>
            {
                { "x", inputOrtValue }
            };

            using var runOptions = new RunOptions();

            // session1 inference
            using (var outputs1 = session.Run(runOptions, inputs1, session.OutputNames))
            {
                // get intermediate value
                var outputToFeed = outputs1.First();
                // modify the name of the ONNX value
                // create input list for session2
                var inputs2 = new Dictionary<string, OrtValue>
                {
                    { "inputNameForModelB", outputToFeed }
                };
                int s = 0;
            }
        }
    }
}
