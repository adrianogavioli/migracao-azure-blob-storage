namespace MigracaoFotosItens
{
    public static class AppSettings
    {
        public static string ConnectionString { get; } = "connection string database";

        public static string ConnectionStorage { get; } = "connection string azure blob storage";

        public static string ContainerName { get; } = "nome do container";
    }
}
