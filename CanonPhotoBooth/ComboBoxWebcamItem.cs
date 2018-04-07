namespace CanonPhotoBooth
{
    internal class ComboBoxWebcamItem
    {
        public WebCameraId ID { get; set; }
        public string Value { get; set; }
        public ComboBoxWebcamItem(WebCameraId id)
        {
            this.ID = id;
            this.Value = id.Name;
        }
    }
}