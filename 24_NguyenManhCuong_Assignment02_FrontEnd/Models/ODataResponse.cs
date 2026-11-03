using System.Text.Json.Serialization;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Models
{
    public class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = new List<T>();

        [JsonPropertyName("@odata.count")]
        public int? Count { get; set; }
    }
}
