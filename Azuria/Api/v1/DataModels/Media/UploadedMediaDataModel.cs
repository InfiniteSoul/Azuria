using Newtonsoft.Json;

namespace Azuria.Api.v1.DataModels.Media
{
    /// <summary>
    /// </summary>
    public class UploadedMediaDataModel : DataModelBase
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("tid")]
        public int? TranslatorId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("tname")]
        public string TranslatorName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("uploader")]
        public int UploaderId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("username")]
        public string UploaderName { get; set; }
    }
}