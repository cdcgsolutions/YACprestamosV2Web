using Microsoft.AspNetCore.Components;

namespace WEB1.Module.Client.Services
{
    #region Modelos y Enums en Inglés
    public enum FileInputMode
    {
        Single,
        Multiple
    }

    public enum AcceptFileType
    {
        Document,
        Image,
        Audio,
        Video,
        Custom,
        All,
        Other
    }

    public enum DataType
    {
        Document,
        Image,
        Audio,
        Video,
        Custom,
        Other
    }

    public class Files
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public int Size { get; set; } // En KB
        public int Upload { get; set; } // Porcentaje 0 - 100
        public DataType Type { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public ElementReference Element { get; set; }
        public bool Preview { get; set; }
        public IProgress<int>? Progress { get; set; }
        public TaskProcess<byte[]> Task { get; set; } = new();

        public enum Status
        {
            Waiting,
            Completed,
            Error
        }

        public class TaskProcess<T>
        {
            public Status Status { get; set; } = Status.Waiting;
            public T? Data { get; set; }
            public string Message { get; set; } = string.Empty;
        }
    }

    public class FileValidator
    {
        public DataType DataType { get; set; } = DataType.Other;

        private readonly string[] _images = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/svg+xml", ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
        private readonly string[] _audios = new[] { "audio/mpeg", "audio/wav", "audio/ogg", "audio/aac", ".mp3", ".wav", ".ogg", ".aac" };
        private readonly string[] _videos = new[] { "video/mp4", "video/webm", "video/ogg", "video/quicktime", ".mp4", ".webm", ".avi", ".mov" };
        private readonly string[] _documents = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation", "text/plain", "text/csv", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".csv" };
        private readonly string[] _custom = new[] { ".json", ".xml", ".zip", ".rar", ".7z", "application/json", "application/xml", "application/zip" };

        public string[] GetImageList() => _images;
        public string[] GetAudioList() => _audios;
        public string[] GetVideoList() => _videos;
        public string[] GetDocumentList() => _documents;
        public string[] GetCustomList() => _custom;

        public bool Validate(string contentType, DataType expectedType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                DataType = expectedType;
                return true;
            }

            var typeLower = contentType.ToLowerInvariant();

            switch (expectedType)
            {
                case DataType.Image:
                    DataType = DataType.Image;
                    return _images.Any(ext => typeLower.Contains(ext.Replace(".", "")));

                case DataType.Audio:
                    DataType = DataType.Audio;
                    return _audios.Any(ext => typeLower.Contains(ext.Replace(".", "")));

                case DataType.Video:
                    DataType = DataType.Video;
                    return _videos.Any(ext => typeLower.Contains(ext.Replace(".", "")));

                case DataType.Document:
                    DataType = DataType.Document;
                    return _documents.Any(ext => typeLower.Contains(ext.Replace(".", ""))) || typeLower.StartsWith("text/") || typeLower.StartsWith("application/");

                case DataType.Custom:
                    DataType = DataType.Custom;
                    return _custom.Any(ext => typeLower.Contains(ext.Replace(".", "")));

                case DataType.Other:
                default:
                    DataType = DataType.Other;
                    return true;
            }
        }
    }
    #endregion

    #region Modelos y Enums en Español (Compatibilidad Total)
#pragma warning disable CS8981
    public enum archivo
    {
        unico = FileInputMode.Single,
        multiple = FileInputMode.Multiple
    }

    public enum Accept
    {
        documento = AcceptFileType.Document,
        imagen = AcceptFileType.Image,
        audio = AcceptFileType.Audio,
        video = AcceptFileType.Video,
        personalizado = AcceptFileType.Custom,
        all = AcceptFileType.All,
        imagenstatica = AcceptFileType.Image,
        otro = AcceptFileType.Other
    }

    public enum Dato
    {
        documento = DataType.Document,
        imagen = DataType.Image,
        audio = DataType.Audio,
        video = DataType.Video,
        personalizado = DataType.Custom,
        otro = DataType.Other
    }

    public enum status
    {
        esperando,
        completada,
        error,
        denegada
    }

    public class TaskProcess<T>
    {
        public status status { get; set; } = status.esperando;
        public T? data { get; set; }
        public string message { get; set; } = string.Empty;
    }

    public class Archivos
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string extension { get; set; } = string.Empty;
        public int size { get; set; }
        public int upload { get; set; }
        public Dato type { get; set; }
        public string thumbnail { get; set; } = string.Empty;
        public string contentType { get; set; } = string.Empty;
        public ElementReference element { get; set; }
        public bool preview { get; set; }
        public IProgress<int>? progress { get; set; }
        public TaskProcess<byte[]> task { get; set; } = new();
    }

    public class ValidadordeArchivo
    {
        private readonly FileValidator _validator = new();

        public Dato TipoDato { get; set; } = Dato.otro;

        public string[] ObtenerListaImagen() => _validator.GetImageList();
        public string[] ObtenerListaaudio() => _validator.GetAudioList();
        public string[] ObtenerListavideo() => _validator.GetVideoList();
        public string[] ObtenerListaDocumento() => _validator.GetDocumentList();
        public string[] ObtenerListaPersonalizado() => _validator.GetCustomList();

        public bool Validar(string contentType, Dato expectedType)
        {
            var result = _validator.Validate(contentType, (DataType)expectedType);
            TipoDato = (Dato)_validator.DataType;
            return result;
        }
    }
    #endregion
}
