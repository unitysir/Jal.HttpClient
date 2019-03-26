using Jal.HttpClient.Interface.Fluent;

namespace Jal.HttpClient.Extensions
{
    public static class HttpMultiPartFormDataContentTypeDescriptorExtensions
    {
        public static IHttpMultiPartFormDataContentTypeDescriptor Utf8(this IHttpMultiPartFormDataContentTypeDescriptor descriptor)
        {
            return descriptor.WithEncoding("utf-8");
        }

        public static IHttpMultiPartFormDataContentTypeDescriptor Utf16(this IHttpMultiPartFormDataContentTypeDescriptor descriptor)
        {
            return descriptor.WithEncoding("utf-16");
        }

        public static IHttpMultiPartFormDataContentTypeDescriptor Utf7(this IHttpMultiPartFormDataContentTypeDescriptor descriptor)
        {
            return descriptor.WithEncoding("utf-7");
        }
    }
}