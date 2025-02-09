namespace Domain.Common.BaseEntities
{
    public class BaseApiResponse<T>
    {
        public List<T>? Value { get; set; } //Mac dinh thuoc tinh value de lay data tu json
    }
}
