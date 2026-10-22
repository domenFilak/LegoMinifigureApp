namespace LegoMinifigureApp.Services.Storage
{
    public sealed record ImageUpload
    (
         Stream Content, 
         string FileName, 
         long Length
    );
}
