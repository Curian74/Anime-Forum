namespace WibuBlog.Common.ApiResponse
{
    public class ApiResponse<T>
    {
        public T? Value { get; set; } //Mac dinh thuoc tinh value de lay data tu json
		public List<IdentityResponse>? Errors { get; set; } //Mac dinh thuoc tinh de lay response tu Identity
		public bool Succeeded { get; set; } //Mac dinh thuoc tinh de lay result tu Identity(true/false)
    }

}
