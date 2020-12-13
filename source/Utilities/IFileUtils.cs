namespace Kyameru.Component.File.Utilities
{
    public interface IFileUtils
    {
        void CopyFile(string source, string destination);

        void CreateDirectory(string directory);

        void Delete(string file);

        void Move(string source, string destination);

        void WriteAllBytes(string path, byte[] file);
    }
}