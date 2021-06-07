using System;

namespace TestingToken
{
    class ConsoleApplication
    {
        private readonly ICore _core;
        public ConsoleApplication(ICore _core)
        {
            this._core = _core;
        }

        public void Run()
        {
            var token = _core.GenerateToken();

            var bearerToken = _core.GetBearerToken(token);

            var predictedResult = _core.Predict(bearerToken);

            var count = predictedResult.Payload.Count;
            Console.WriteLine($"Number of result in response is {count}");

        }
    }
}