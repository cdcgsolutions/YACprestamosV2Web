namespace WEB1.SandBox.DTOs
{
    public class ModuleDTO
    {
        public string code { get; set; } = Guid.NewGuid().ToString();
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public string icon { get; set; } = "bi bi-app";
        public bool status { get; set; } = true;
        public string typeName { get; set; } = "WEB - Interface";
        public List<InterfaceDTO> interfaceList { get; set; } = new();
    }

    public class InterfaceDTO
    {
        public string code { get; set; } = Guid.NewGuid().ToString();
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public string icon { get; set; } = "bi bi-circle";
        public bool status { get; set; } = true;
        public string typeName { get; set; } = "WEB - Interface";
    }

    public class ValuesParameters
    {
        public List<ModuleDTO> Modules { get; set; } = new();
    }

    public class NavMenu
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
