using System;

namespace SampleProject
{
    public class MyService
    {
        private readonly IDataProvider _dataProvider;
        private readonly IOutputWriter _outputWriter;
        private readonly IMyTransformer _transformer;

        public MyService(IDataProvider dataProvider, IMyTransformer transformer, IOutputWriter outputWriter)
        {
            _dataProvider = dataProvider;
            _transformer = transformer;
            _outputWriter = outputWriter;
        }

        public void DoStuff()
        {
            var data = _dataProvider.GetData();
            data = _transformer.Transform(data);
            _outputWriter.WriteData(data);
        }
    }

    public interface IMyTransformer
    {
        object Transform(object data);
    }

    public class MyTransformer : IMyTransformer
    {
        public object Transform(object data)
        {
            return data = data + " (transformed)";
        }
    }

    public interface IDataProvider
    {
        object GetData();
    }

    public class MyDataProvider : IDataProvider
    {
        public object GetData()
        {
            return "xxx";
        }
    }

    public interface IOutputWriter
    {
        void WriteData(object data);
    }

    public class MyOutputWriter : IOutputWriter
    {
        public void WriteData(object data)
        {
            Console.WriteLine(data);
        }
    }
}