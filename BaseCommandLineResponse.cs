namespace ESGamePadDetect
{
    public class BaseCommandLineResponse<T> {
        public T Data { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
