
namespace TestingToken
{
    public interface ICore
    {
        string GenerateToken();
        string GetBearerToken(string token);
        PredictResponse Predict(string bearerToken);

    }
}
