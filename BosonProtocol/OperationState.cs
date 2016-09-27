using System;

namespace QuaternionProtocol
{
    [Serializable]
    public class OperationState
    {
        public bool IsSuccessful { get; set; }
        public string Details { get; set; }
        public int Counter;
    }
}