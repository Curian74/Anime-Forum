﻿namespace Domain.Common.BaseEntities
{
    public class BaseApiResponse<T>
    {
        public T? Value { get; set; } //Mac dinh thuoc tinh value de lay data tu json
    }
}
