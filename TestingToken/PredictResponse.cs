using System.Collections.Generic;


namespace TestingToken
{

        public class PredictResponse
        {
            public List<PayloadResponse> Payload { get; set; }
        }

        public class ClassificationResponse
        {
            public double Score { get; set; }
        }

        public class PayloadResponse
        {
            public string AnnotationSpecId { get; set; }
            public ClassificationResponse Classification { get; set; }
            public string DisplayName { get; set; }
        }
    
}
